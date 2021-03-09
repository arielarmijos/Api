// <copyright file="Emitir.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.Provider.Cash472
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using DataContract.Cash472;
    using DataContract.Common;
    using Movilway.Logging;
    using Movilway.API.Core.Security;

    /// <summary>
    /// Implementación método para una nueva emisión de un giro
    /// </summary>
    internal partial class CashProvider : AGenericPlatformAuthentication
    {
        /// <summary>
        /// Emite un nuevo giro a partir de llamados directos a los WS de Movilway - Kinacu
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del giro</param>
        /// <returns>Respuesta de la cotización</returns>
        public EmitirResponse Emitir(EmitirRequest request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.LogRequest(request);

            EmitirResponse response = new EmitirResponse();
            string sessionId = this.GetSessionId(request, response, out this.errorMessage);
            if (this.errorMessage != ErrorMessagesMnemonics.None)
            {
                this.LogResponse(response);
                return response;
            }

            if (!request.IsValidRequest())
            {
                this.SetResponseErrorCode(response, ErrorMessagesMnemonics.InvalidRequiredFields);
                this.LogResponse(response);
                return response;
            }

            DwhModel.Cliente infoEmisor = this.GetInfoCliente(sessionId, request.Emisor.TipoIdentificacion, request.Emisor.NumeroIdentificacion, out this.errorMessage);
            DwhModel.Cliente infoReceptor = this.GetInfoCliente(sessionId, request.Receptor.TipoIdentificacion, request.Receptor.NumeroIdentificacion, out this.errorMessage);
            DwhModel.Agencia agencia = this.GetInfoAgencia(sessionId, request.AuthenticationData.Username);

            if (agencia == null)
            {
                this.SetResponseErrorCode(response, ErrorMessagesMnemonics.UnableToFindAgentInLocalDatabase);
                this.LogResponse(response);
                return response;
            }

            if (infoEmisor == null || infoReceptor == null)
            {
                if (infoEmisor == null && infoReceptor == null)
                {
                    this.SetResponseErrorCode(response, ErrorMessagesMnemonics.UnableToFindIssuingAndReceiverUserInLocalDatabase);
                }
                else if (infoEmisor == null)
                {
                    this.SetResponseErrorCode(response, ErrorMessagesMnemonics.UnableToFindIssuingUserInLocalDatabase);
                }
                else
                {
                    this.SetResponseErrorCode(response, ErrorMessagesMnemonics.UnableToFindReceiverUserInLocalDatabase);
                }

                this.LogResponse(response);
                return response;
            }

            if (infoEmisor.Id == infoReceptor.Id)
            {
                this.SetResponseErrorCode(response, ErrorMessagesMnemonics.IssuingUserAndReceiverUserAreTheSame);
                this.LogResponse(response);
                return response;
            }

            request.CiudadPdv = agencia.Ciudad;

            int id = this.InsertGiro(sessionId, request, infoEmisor, infoReceptor, agencia, out this.errorMessage);
            if (this.errorMessage != ErrorMessagesMnemonics.None || id == 0)
            {
                this.SetResponseErrorCode(response, ErrorMessagesMnemonics.InternalDatabaseErrorInsertingOrder);
                this.LogResponse(response);
                return response;
            }

            Exception ex = null;

            DataContract.TopUpResponseBody topup = null;

            if (this.errorMessage == ErrorMessagesMnemonics.None)
            {
                topup = this.EmitirPaso2(request, id, sessionId, out ex);
            }

            int topupResponseCode = topup != null && topup.ResponseCode.HasValue ? topup.ResponseCode.Value : -1;
            if (this.errorMessage == ErrorMessagesMnemonics.None && topupResponseCode == 0)
            {
                response.ResponseCode = 0;
                DwhModel.GiroUltimaTransaccion infoGiro = this.GetInfoGiro(sessionId, id, out this.errorMessage);

                response.Pin = Multipay472TripleDes.Decrypt(this.multipayTripleDesKey, infoGiro.Pin);
                response.CodigoTransaccion = infoGiro.CodigoTransaccion ?? " ";
                response.NumeroFactura = infoGiro.NumeroFactura;
                response.CodigoAutorizacion = infoGiro.CodigoAutorizacion472;
                response.Fecha = infoGiro.Fecha.DateTime;
                response.ResponseMessage = topup.ResponseMessage;
                response.Id = infoGiro.Id;
                response.ExternalId = infoGiro.ExternalId;

                if (this.smssEnabled)
                {
                    // Envios SMSs
                    string message = string.Format(
                        "Se emitio un giro por {0} con PIN: {1}. Servicio prestado por Movilway S.A.S",
                        this.FormatMoney(request.ValorAEntregar),
                        response.Pin);

                    SmsApi.SmsApiSoapClient smsapi = new SmsApi.SmsApiSoapClient();

                    string phonePayer = string.Concat("57", infoEmisor.Telefono.ToString());
                    string phonePayee = string.Concat("57", infoReceptor.Telefono.ToString());

                    try
                    {
                        smsapi.SendSms(new SmsApi.SendSmsRequest() { To = phonePayer, Message = message });

                        if (phonePayer != phonePayee)
                        {
                            smsapi.SendSms(new SmsApi.SendSmsRequest() { To = phonePayee, Message = message });
                        }
                    }
                    catch (Exception exs)
                    {
                        this.ProviderLogger.ExceptionLow(() => TagValue.New()
                            .MethodName(methodName)
                            .Message("[" + sessionId + "] " + "Error envíando SMSs")
                            .Exception(exs));
                    }
                }
            }
            else
            {
                string error = string.Empty;
                //if (transfer != null && transferResponseCode != 0)
                //{
                //    error = "Error en llamado a Transfer";
                //    response.ResponseCode = transfer.ResponseCode;
                //    response.ResponseMessage = transfer.ResponseMessage;
                //}
                //else 
                if (topup != null && topupResponseCode != 0)
                {
                    error = "Error en llamado a TopUp";
                    response.ResponseCode = topup.ResponseCode;
                    response.ResponseMessage = topup.ResponseMessage;
                }
                else
                {
                    error = "Error API/Otros";
                    response.ResponseCode = (int)this.errorMessage;
                    response.ResponseMessage = string.Concat(this.errorMessage.ToDescription(), ex != null ? string.Concat(" => ", ex.Message) : string.Empty);
                }

                this.AnularGiro(sessionId, id, error, response.ResponseMessage, out this.errorMessage);

                this.LogResponse(response);
                return response;
            }

            this.LogResponse(response);
            return response;
        }

        /// <summary>
        /// Paso llamado método TopUp con el producto de CashIn, para que el protocolo realice el llamado
        /// de constitución y emisión en el WS de MultiPay 472
        /// </summary>
        /// <param name="request">Objeto que contiene la información del giro</param>
        /// <param name="idGiro">ID del giro creado en base de datos MW</param>
        /// <param name="sessionId">ID de sesión para poner en los mensajes de log</param>
        /// <param name="exc">Excepción generada al llamar el método del API</param>
        /// <returns>Respuesta del TopUp</returns>
        private DataContract.TopUpResponseBody EmitirPaso2(EmitirRequest request, int idGiro, string sessionId, out Exception exc)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            exc = null;

            DataContract.TopUpResponseBody resp = null;
            string endpointName = "TopUp";
            try
            {
                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Llamando método \"" + endpointName + "\" ..."));

                resp = new ServiceExecutionDelegator<DataContract.TopUpResponseBody, DataContract.TopUpRequestBody>().ResolveRequest(
                    new DataContract.TopUpRequestBody()
                    {
                        AuthenticationData = new AuthenticationData()
                        {
                            Username = request.AuthenticationData.Username,
                            Password = request.AuthenticationData.Password
                        },
                        DeviceType = request.DeviceType,
                        Platform = request.Platform,
                        WalletType = DataContract.WalletType.Stock,
                        ExternalTransactionReference = sessionId,
                        Amount = request.ValorRecibido,
                        TerminalID = request.Pdv,
                        Recipient = idGiro.ToString(),
                        MNO = this.multipayTopUpMno
                    },
                    ApiTargetPlatform.Kinacu,
                    ApiServiceName.TopUp);

                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Respuesta método \"" + endpointName + "\"")
                    .Tag("ResponseCode").Value(resp != null && resp.ResponseCode.HasValue ? resp.ResponseCode.Value.ToString() : "NULL")
                    .Tag("ResponseMessage").Value(resp != null && !string.IsNullOrEmpty(resp.ResponseMessage) ? resp.ResponseMessage : "vacío"));
            }
            catch (Exception ex)
            {
                this.ProviderLogger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Error llamando método \"" + endpointName + "\"")
                    .Exception(ex));
                this.errorMessage = ErrorMessagesMnemonics.ApiMethodException;
                exc = ex;
            }

            return resp;
        }

        /// <summary>
        /// Inserta en base de datos la información del giro
        /// </summary>
        /// <param name="sessionId">Session ID que será escrito en los logs</param>
        /// <param name="request">Petición original</param>
        /// <param name="emisor">Cliente emisor</param>
        /// <param name="receptor">Cliente receptor</param>
        /// <param name="agencia">Información de la agencia originadora</param>
        /// <param name="returnCode">Codigo de error en caso de que algo falle (-1 = OK, >-1 = Error)</param>
        /// <param name="connection">Objeto de conexión a base de datos</param>
        /// <returns>Un <c>int</c> que contiene el ID del cliente creado, cero en caso de falla</returns>
        private int InsertGiro(string sessionId, EmitirRequest request, DwhModel.Cliente emisor, DwhModel.Cliente receptor, DwhModel.Agencia agencia, out ErrorMessagesMnemonics returnCode, SqlConnection connection = null)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            returnCode = ErrorMessagesMnemonics.None;
            int ret = 0;

            try
            {
                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Ejecutando query ..."));

                Dictionary<string, object> queryParams = new Dictionary<string, object>()
                {
                    { "@EmisorId", emisor.Id },
                    { "@ReceptorId", receptor.Id },
                    { "@EstadoId", Cash472.CashProvider.ObtenerCodigoEstadoGiro(EstadoGiro.EnProceso)},
                    { "@Pdv", request.Pdv },
                    { "@TotalRecibido", request.ValorRecibido },
                    { "@TotalAEntregar", request.ValorAEntregar },
                    { "@Flete", request.ValorFlete },
                    { "@IncluyeFlete", request.IncluyeFlete },
                    { "@CiudadOrigenDANE", request.CiudadPdv },
                    { "@CiudadDestinoDANE", request.CiudadDestino },
                    { "@AgenciaId", agencia.BranchId },
                    { "@AgenciaNombre", agencia.LegalName.Replace("'", "''") },
                    { "@AgenciaDireccion", agencia.Address.Replace("'", "''") },
                    { "@AccesoTipo", request.DeviceType },
                    { "@Acceso", request.AuthenticationData.Username }
                };

                if (connection == null)
                {
                    using (connection = Utils.Database.GetCash472DbConnection())
                    {
                        connection.Open();
                        ret = (int)Utils.Dwh<int>.ExecuteScalar(
                        connection,
                        Queries.Cash.InsertGiro,
                        queryParams,
                        null);
                    }
                }
                else
                {
                    ret = (int)Utils.Dwh<int>.ExecuteScalar(
                    connection,
                    Queries.Cash.InsertGiro,
                    queryParams,
                    null);
                }

                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Query ejecutado"));
            }
            catch (Exception ex)
            {
                this.ProviderLogger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Error ejecutando query")
                    .Exception(ex));
                returnCode = ErrorMessagesMnemonics.InternalDatabaseError;
                ret = 0;
            }

            return ret;
        }

        /// <summary>
        /// Actualiza en base de datos la información del giro estableciendolo como anulado
        /// </summary>
        /// <param name="sessionId">Session ID que será escrito en los logs</param>
        /// <param name="id">ID del giro a anular</param>
        /// <param name="error">Fuente del error</param>
        /// <param name="descripcion">Descripción del error</param>
        /// <param name="returnCode">Codigo de error en caso de que algo falle (-1 = OK, >-1 = Error)</param>
        /// <param name="connection">Objeto de conexión a base de datos</param>
        /// <returns>Un <c>int</c> que contiene el número de registros afectados, cero en caso de falla</returns>
        private int AnularGiro(
            string sessionId,
            int id,
            string error,
            string descripcion,
            out ErrorMessagesMnemonics returnCode,
            SqlConnection connection = null)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            returnCode = ErrorMessagesMnemonics.None;
            int ret = 0;

            try
            {
                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Ejecutando query ..."));

                Dictionary<string, object> queryParams = new Dictionary<string, object>()
                {
                    { "@IdGiro", id.ToString() },
                    { "@Error", error },
                    { "@IdEstado", Cash472.CashProvider.ObtenerCodigoEstadoGiro(EstadoGiro.ErrorProcesando) },
                    { "@Descripcion", descripcion }
                };

                if (connection == null)
                {
                    using (connection = Utils.Database.GetCash472DbConnection())
                    {
                        connection.Open();
                        ret = (int)Utils.Dwh<int>.ExecuteScalar(
                        connection,
                        Queries.Cash.AnularGiro,
                        queryParams,
                        null);
                    }
                }
                else
                {
                    ret = (int)Utils.Dwh<int>.ExecuteScalar(
                    connection,
                    Queries.Cash.AnularGiro,
                    queryParams,
                    null);
                }

                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Query ejecutado"));
            }
            catch (Exception ex)
            {
                this.ProviderLogger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Error ejecutando query")
                    .Exception(ex));
                returnCode = ErrorMessagesMnemonics.InternalDatabaseError;
                ret = 0;
            }

            return ret;
        }

        /// <summary>
        /// Obtiene la información de un giro dado
        /// </summary>
        /// <param name="sessionId">Session ID que será escrito en los logs</param>
        /// <param name="id">ID giro</param>
        /// <param name="returnCode">Codigo de error en caso de que algo falle (-1 = OK, >-1 = Error)</param>
        /// <param name="connection">Objeto de conexión a base de datos</param>
        /// <returns>Un objeto <c>DwhModel.Giro</c> que contiene la información del giro</returns>
        private DwhModel.GiroUltimaTransaccion GetInfoGiro(
            string sessionId,
            int id,
            out ErrorMessagesMnemonics returnCode,
            SqlConnection connection = null)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            returnCode = ErrorMessagesMnemonics.None;
            DwhModel.GiroUltimaTransaccion ret = null;

            try
            {
                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Ejecutando query ..."));

                Dictionary<string, object> queryParams = new Dictionary<string, object>()
                {
                    { "@IdGiro", id.ToString() }
                };

                if (connection == null)
                {
                    using (connection = Utils.Database.GetCash472DbConnection())
                    {
                        connection.Open();
                        ret = Utils.Dwh<DwhModel.GiroUltimaTransaccion>.ExecuteSingle(
                        connection,
                        Queries.Cash.GetInfoGiro,
                        queryParams,
                        null);
                    }
                }
                else
                {
                    ret = Utils.Dwh<DwhModel.GiroUltimaTransaccion>.ExecuteSingle(
                    connection,
                    Queries.Cash.GetInfoGiro,
                    queryParams,
                    null);
                }

                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Query ejecutado"));
            }
            catch (Exception ex)
            {
                this.ProviderLogger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Error ejecutando query")
                    .Exception(ex));
                returnCode = ErrorMessagesMnemonics.InternalDatabaseError;
                ret = new DwhModel.GiroUltimaTransaccion();
            }

            return ret;
        }
    }
}
