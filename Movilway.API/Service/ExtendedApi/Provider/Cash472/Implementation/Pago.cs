// <copyright file="Pago.cs" company="Movilway">
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
    using Movilway.API.Service.ExtendedApi.DataContract;

    /// <summary>
    /// Implementación método PagoGiro
    /// </summary>
    internal partial class CashProvider : AGenericPlatformAuthentication
    {
        /// <summary>
        /// Realiza el proceso de pago de un giro
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del pago</param>
        /// <returns>Respuesta del pago</returns>
        public PagoResponse Pago(PagoRequest request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.LogRequest(request);

            PagoResponse response = new PagoResponse();
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

            DwhModel.Cliente infoCliente = this.GetInfoCliente(sessionId, request.TipoIdentificacion, request.NumeroIdentificacion, out this.errorMessage);
            if (this.errorMessage != ErrorMessagesMnemonics.None)
            {
                this.SetResponseErrorCode(response, this.errorMessage);
                this.LogResponse(response);
                return response;
            }

            DwhModel.Agencia agencia = this.GetInfoAgencia(sessionId, request.AuthenticationData.Username);
            if (agencia == null)
            {
                this.SetResponseErrorCode(response, ErrorMessagesMnemonics.UnableToFindAgentInLocalDatabase);
                this.LogResponse(response);
                return response;
            }

            request.CiudadPdv = Convert.ToInt64(agencia.Ciudad);
            DwhModel.GiroUltimaTransaccion infoGiro = this.GetInfoGiroPorExternalId(sessionId, request.Id, request.Pin, out this.errorMessage);
            int id = this.InsertPago(sessionId, request, infoGiro, infoCliente, out this.errorMessage);
            if (this.errorMessage != ErrorMessagesMnemonics.None || id == 0)
            {
                this.SetResponseErrorCode(response, ErrorMessagesMnemonics.InternalDatabaseErrorInsertingPayment);
                this.LogResponse(response);
                return response;
            }

            Exception ex = null;
            string error = string.Empty;

            // Quitar el valor de la Agencia que va a efectuar el pago con valor facial -1
            DataContract.TopUpResponseBody topup = this.PagoPaso1(request, id, sessionId, out ex);
            int topupResponseCode = topup != null && topup.ResponseCode.HasValue ? topup.ResponseCode.Value : -1;
            if (this.errorMessage == ErrorMessagesMnemonics.None && topupResponseCode == 0)
            {
                response.ResponseCode = 0;
                DwhModel.Pago infoPago = this.GetInfoPago(sessionId, id, out this.errorMessage);

                response.NumeroFactura = infoPago.NumeroFactura;
                response.CodigoTransaccion = !string.IsNullOrEmpty(infoPago.CodigoTransaccion) ? infoPago.CodigoTransaccion : string.Empty;
                response.CodigoAutorizacion = infoPago.CodigoAutorizacion;
                response.NumeroComprobantePago = infoPago.NumeroComprobantePago;
                response.NumeroReferencia = infoPago.NumeroReferencia;
                response.Valor = infoPago.ValorPagoRespuesta;
                response.Fecha = infoPago.FechaPago != null && infoPago.FechaPago.HasValue ? infoPago.FechaPago.Value : DateTime.MinValue;

                if (this.smssEnabled)
                {
                    // Envios SMSs
                    string message = string.Format(
                        "Se pago un giro por {0} correspondiente al PIN: {1}. Servicio prestado por Movilway S.A.S",
                        this.FormatMoney(request.Valor),
                        request.Pin);

                    SmsApi.SmsApiSoapClient smsapi = new SmsApi.SmsApiSoapClient();

                    try
                    {
                        smsapi.SendSms(new SmsApi.SendSmsRequest() { To = string.Concat("57", infoCliente.Telefono.ToString()), Message = message });
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
                if (topup != null && topupResponseCode != 0)
                {
                    error = "Error en llamado a TopUp";
                    response.ResponseCode = topup.ResponseCode;
                    response.ResponseMessage = topup.ResponseMessage;
                }
                else
                {
                    error = "Error en llamado a TopUp";
                    response.ResponseCode = (int)this.errorMessage;
                    response.ResponseMessage = string.Concat(this.errorMessage.ToDescription(), ex != null ? string.Concat(" => ", ex.Message) : string.Empty);
                }
            }

            this.LogResponse(response);
            return response;
        }

        /// <summary>
        /// Paso 2 llamado método TopUp con el producto de CashOut, para que el protocolo realice el llamado
        /// de pago en el WS de MultiPay 472
        /// </summary>
        /// <param name="request">Objeto que contiene la información del pago</param>
        /// <param name="idPago">ID del pago creado en base de datos MW</param>
        /// <param name="sessionId">ID de sesión para poner en los mensajes de log</param>
        /// <param name="exc">Excepción generada al llamar el método del API</param>
        /// <returns>Respuesta del TopUp</returns>
        private DataContract.TopUpResponseBody PagoPaso1(PagoRequest request, int idPago, string sessionId, out Exception exc)
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
                        Amount = request.Valor,
                        TerminalID = request.Pdv,
                        Recipient = idPago.ToString(),
                        MNO = this.multipayReverseTopUpMno
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
        /// Obtiene la información de un giro dado a partir de su ExternalId (ID Multipay 472)
        /// </summary>
        /// <param name="sessionId">Session ID que será escrito en los logs</param>
        /// <param name="id">ID Multipay 472 del giro</param>
        /// <param name="returnCode">Codigo de error en caso de que algo falle (-1 = OK, >-1 = Error)</param>
        /// <param name="connection">Objeto de conexión a base de datos</param>
        /// <returns>Un objeto <c>DwhModel.Giro</c> que contiene la información del giro</returns>
        private DwhModel.GiroUltimaTransaccion GetInfoGiroPorExternalId(
            string sessionId,
            long id,
            string pin,
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
                    using (connection = Movilway.API.Utils.Database.GetCash472DbConnection())
                    {
                        connection.Open();
                        ret = Movilway.API.Utils.Dwh<DwhModel.GiroUltimaTransaccion>.ExecuteSingle(
                        connection,
                        Queries.Cash.GetInfoGiroPorExternalId,
                        queryParams,
                        null);
                    }
                }
                else
                {
                    ret = Movilway.API.Utils.Dwh<DwhModel.GiroUltimaTransaccion>.ExecuteSingle(
                    connection,
                    Queries.Cash.GetInfoGiroPorExternalId,
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

        /// <summary>
        /// Inserta en base de datos la información de un pago
        /// </summary>
        /// <param name="sessionId">Session ID que será escrito en los logs</param>
        /// <param name="request">Petición original</param>
        /// <param name="giro">Giro original</param>
        /// <param name="cliente">Información del cliente</param>
        /// <param name="returnCode">Codigo de error en caso de que algo falle (-1 = OK, >-1 = Error)</param>
        /// <param name="connection">Objeto de conexión a base de datos</param>
        /// <returns>Un <c>int</c> que contiene el ID del pago creado, cero en caso de falla</returns>
        private int InsertPago(
            string sessionId,
            PagoRequest request,
            DwhModel.GiroUltimaTransaccion giro,
            DwhModel.Cliente cliente,
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

                bool exists = giro.Id != 0;
                Dictionary<string, object> queryParams = new Dictionary<string, object>()
                {
                    { "@GiroId", giro.Id > 0   ? (object)giro.Id : DBNull.Value },
                    { "@ExternalId", request.Id },
                    { "@EmisorId",  giro.EmisorId > 0 ? (object)giro.EmisorId : DBNull.Value },
                    { "@ReceptorId",  cliente.Id },
                    { "@Pdv", request.Pdv },
                    { "@CiudadPdv", request.CiudadPdv },
                    { "@TotalRecibido", request.ValorRecibidoTotal},
                    { "@TotalAEntregar", request.Valor},
                    { "@Flete", request.ValorFlete },
                    { "@IncluyeFlete", request.IncluyeFlete},
                    { "@ValorPago", request.Valor }
                };

                if (connection == null)
                {
                    using (connection = Movilway.API.Utils.Database.GetCash472DbConnection())
                    {
                        connection.Open();
                        ret = (int)Movilway.API.Utils.Dwh<int>.ExecuteScalar(
                        connection,
                        Queries.Cash.InsertPago,
                        queryParams,
                        null);
                    }
                }
                else
                {
                    ret = (int)Movilway.API.Utils.Dwh<int>.ExecuteScalar(
                    connection,
                    Queries.Cash.InsertPago,
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
        /// Obtiene la información de un pago dado
        /// </summary>
        /// <param name="sessionId">Session ID que será escrito en los logs</param>
        /// <param name="id">ID pago</param>
        /// <param name="returnCode">Codigo de error en caso de que algo falle (-1 = OK, >-1 = Error)</param>
        /// <param name="connection">Objeto de conexión a base de datos</param>
        /// <returns>Un objeto <c>DwhModel.Pago</c> que contiene la información del giro</returns>
        private DwhModel.Pago GetInfoPago(
            string sessionId,
            int id,
            out ErrorMessagesMnemonics returnCode,
            SqlConnection connection = null)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            returnCode = ErrorMessagesMnemonics.None;
            DwhModel.Pago ret = null;

            try
            {
                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Ejecutando query ..."));

                Dictionary<string, object> queryParams = new Dictionary<string, object>()
                {
                    { "@IdPago", id > 0  ? (object)id : DBNull.Value }
                };

                if (connection == null)
                {
                    using (connection = Movilway.API.Utils.Database.GetCash472DbConnection())
                    {
                        connection.Open();
                        ret = Movilway.API.Utils.Dwh<DwhModel.Pago>.ExecuteSingle(
                        connection,
                        Queries.Cash.GetInfoGiro,
                        queryParams,
                        null);
                    }
                }
                else
                {
                    ret = Movilway.API.Utils.Dwh<DwhModel.Pago>.ExecuteSingle(
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
                ret = new DwhModel.Pago();
            }

            return ret;
        }

        /// <summary>
        /// Actualiza en base de datos la información de un pago estableciendolo como anulado
        /// </summary>
        /// <param name="sessionId">Session ID que será escrito en los logs</param>
        /// <param name="id">ID del pago a anular</param>
        /// <param name="error">Fuente del error</param>
        /// <param name="descripcion">Descripción del error</param>
        /// <param name="returnCode">Codigo de error en caso de que algo falle (-1 = OK, >-1 = Error)</param>
        /// <param name="connection">Objeto de conexión a base de datos</param>
        /// <returns>Un <c>int</c> que contiene el número de registros afectados, cero en caso de falla</returns>
        private int AnularPago(
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
                    { "@IdPago", id.ToString() },
                    { "@Error", error },
                    { "@Descripcion", descripcion }
                };

                if (connection == null)
                {
                    using (connection = Movilway.API.Utils.Database.GetCash472DbConnection())
                    {
                        connection.Open();
                        ret = (int)Movilway.API.Utils.Dwh<int>.ExecuteScalar(
                        connection,
                        Queries.Cash.AnularPago,
                        queryParams,
                        null);
                    }
                }
                else
                {
                    ret = (int)Movilway.API.Utils.Dwh<int>.ExecuteScalar(
                    connection,
                    Queries.Cash.AnularPago,
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
    }
}
