// <copyright file="Emitir472.cs" company="Movilway">
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

    /// <summary>
    /// Implementación método para una nueva emisión de un giro
    /// </summary>
    internal partial class CashProvider : AGenericPlatformAuthentication
    {
        /// <summary>
        /// Emite un nuevo giro a partir de llamados directos a los WS de MultiPay 472
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del giro</param>
        /// <returns>Respuesta de la cotización</returns>
        public EmitirResponse Emitir472(EmitirRequest request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.LogRequest(request);

            EmitirResponse response = new EmitirResponse();
            string sessionId = this.GetSessionId(request, response, out this.errorMessage);
            if (this.errorMessage != ErrorMessagesMnemonics.None)
            {
                return response;
            }

            DwhModel.Cliente infoEmisor = this.GetInfoCliente(sessionId, request.Emisor.TipoIdentificacion, request.Emisor.NumeroIdentificacion, out this.errorMessage);
            DwhModel.Cliente infoReceptor = this.GetInfoCliente(sessionId, request.Receptor.TipoIdentificacion, request.Receptor.NumeroIdentificacion, out this.errorMessage);

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

                return response;
            }

            if (infoEmisor.Id == infoReceptor.Id)
            {
                this.SetResponseErrorCode(response, ErrorMessagesMnemonics.IssuingUserAndReceiverUserAreTheSame);
                return response;
            }

            Exception ex;
            MultiPay472.RespuestaValidacionConstitucionGiro constitucion = this.EmitirPaso1Cash472(request, sessionId, out ex);
            MultiPay472.RespuestaEmisionGiro emision = null;

            if (this.errorMessage == ErrorMessagesMnemonics.None)
            {
                emision = this.EmitirPaso2Cash472(request, sessionId, constitucion, out ex);
            }

            if (this.errorMessage == ErrorMessagesMnemonics.None && emision != null)
            {
                response.ResponseCode = 0;
                response.Pin = emision.PIN;
                response.CodigoTransaccion = emision.CodigoTransaccion;
                response.CodigoTransaccionConstitucion = constitucion.CodigoTransaccion;
                response.NumeroTransaccion472 = emision.NumeroReferencia;

                response.NumeroFactura = string.IsNullOrEmpty(emision.NumeroFactura) ? string.Empty : emision.NumeroFactura;
                response.CodigoAutorizacion = string.IsNullOrEmpty(emision.CodigoAutorizacion) ? string.Empty : emision.CodigoAutorizacion;
                response.Fecha = Cash472.CashProvider.ObtenerFechaDesdeString(emision.Fecha);

                int id = this.InsertGiroCash472(sessionId, request, infoEmisor, infoReceptor, constitucion, emision, out this.errorMessage);
                if (this.errorMessage != ErrorMessagesMnemonics.None || id == 0)
                {
                    // TODO: manejar excepción de creación del giro
                }
            }
            else
            {
                response.ResponseCode = (int)this.errorMessage;
                if (this.errorMessage == ErrorMessagesMnemonics.WebServiceException)
                {
                    response.ResponseMessage = string.Concat(this.errorMessage.ToDescription(), " => ", ex != null ? ex.Message : string.Empty);
                }
                else
                {
                    response.ResponseMessage = this.errorMessage == ErrorMessagesMnemonics.WebServiceDoesNotRespond ?
                        this.errorMessage.ToDescription() :
                        CashProvider.ObtenerMensajeCodigoRespuesta(constitucion.CodigoRespuesta);
                }
            }

            return response;
        }

        /// <summary>
        /// Paso 1 emisión de un nuevo giro => Constitución del giro
        /// </summary>
        /// <param name="request">Objeto que contiene la información del giro</param>
        /// <param name="sessionId">ID de sesión para poner en los mensajes de log</param>
        /// <param name="exc">Excepción generada al llamar el servicio</param>
        /// <returns>Respuesta de la constitución del giro</returns>
        private MultiPay472.RespuestaValidacionConstitucionGiro EmitirPaso1Cash472(EmitirRequest request, string sessionId, out Exception exc)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            exc = null;

            MultiPay472.RespuestaValidacionConstitucionGiro resp = null;
            MultiPay472.Service1SoapClient client = this.GetSoapClient();
            string endpointName = "ValidacionConstitucionGiro";
            try
            {
                MultiPay472.ValidacionConstitucionGiro peticion = new MultiPay472.ValidacionConstitucionGiro();
                peticion.NitRed = this.multipayNitRed;
                peticion.CodigoTerminal = this.multipayTerminal;
                peticion.CodigoTransaccion = this.GenerarCodigoTransaccion(sessionId);

                peticion.CodigoPuntoVenta = request.Pdv;
                peticion.ValorRecibido = request.ValorRecibido;
                peticion.IncluyeFlete = request.IncluyeFlete;
                peticion.CodigoDaneCiudadPuntoVenta = request.CiudadPdv;
                peticion.CodigoDaneCiudadPagoSugerido = request.CiudadDestino;

                peticion.Originador = new MultiPay472.Originador();
                this.EstablecerValoresCliente472(request.Emisor, peticion.Originador, null);

                peticion.Destinatario = new MultiPay472.Destinatario();
                this.EstablecerValoresCliente472(request.Receptor, null, peticion.Destinatario);

                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Llamando servicio \"" + endpointName + "\" ..."));

                resp = client.ValidacionConstitucionGiro(peticion, this.multipayUsuario);

                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Respuesta servicio \"" + endpointName + "\"")
                    .Tag("Respuesta").Value(resp != null ? resp.CodigoRespuesta : "NULL"));

                if (resp == null || resp.CodigoRespuesta != CashProvider.CodigoRespuestaExitoso)
                {
                    this.errorMessage = resp == null ? ErrorMessagesMnemonics.WebServiceDoesNotRespond : ErrorMessagesMnemonics.Cash472WsError;
                }
            }
            catch (Exception ex)
            {
                this.ProviderLogger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Error llamando servicio \"" + endpointName + "\"")
                    .Exception(ex));
                this.errorMessage = ErrorMessagesMnemonics.WebServiceException;
                exc = ex;
            }

            return resp;
        }

        /// <summary>
        /// Paso 2 emisión de un nuevo giro => Confirmación del giro
        /// </summary>
        /// <param name="request">Objeto que contiene la información del giro</param>
        /// <param name="sessionId">ID de sesión para poner en los mensajes de log</param>
        /// <param name="constitution">Mensaje de respuesta de la constitución del giro</param>
        /// <param name="exc">Excepción generada al llamar el servicio</param>
        /// <returns>Respuesta de la constitución del giro</returns>
        private MultiPay472.RespuestaEmisionGiro EmitirPaso2Cash472(EmitirRequest request, string sessionId, MultiPay472.RespuestaValidacionConstitucionGiro constitution, out Exception exc)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            exc = null;

            MultiPay472.RespuestaEmisionGiro resp = null;
            MultiPay472.Service1SoapClient client = this.GetSoapClient();
            string endpointName = "EmisionGiro";
            try
            {
                MultiPay472.EmisionGiro peticion = new MultiPay472.EmisionGiro();
                peticion.Token = constitution.Token;
                peticion.CodigoTransaccion = this.GenerarCodigoTransaccion(sessionId);

                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Llamando servicio \"" + endpointName + "\" ..."));

                resp = client.EmisionGiro(peticion, this.multipayUsuario);

                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Respuesta servicio \"" + endpointName + "\"")
                    .Tag("Respuesta").Value(resp != null ? resp.CodigoRespuesta : "NULL"));

                if (resp == null || resp.CodigoRespuesta != CashProvider.CodigoRespuestaExitoso)
                {
                    this.errorMessage = resp == null ? ErrorMessagesMnemonics.WebServiceDoesNotRespond : ErrorMessagesMnemonics.Cash472WsError;
                }
            }
            catch (Exception ex)
            {
                this.ProviderLogger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Error llamando servicio \"" + endpointName + "\"")
                    .Exception(ex));
                this.errorMessage = ErrorMessagesMnemonics.WebServiceException;
                exc = ex;
            }

            return resp;
        }

        /// <summary>
        /// Inserta en base de datos la información del giro creado
        /// </summary>
        /// <param name="sessionId">Session ID que será escrito en los logs</param>
        /// <param name="request">Petición original</param>
        /// <param name="emisor">Cliente emisor</param>
        /// <param name="receptor">Cliente receptor</param>
        /// <param name="constitucion">Respuesta WS constitución</param>
        /// <param name="emision">Respuesta WS emisión</param>
        /// <param name="returnCode">Codigo de error en caso de que algo falle (-1 = OK, >-1 = Error)</param>
        /// <param name="connection">Objeto de conexión a base de datos</param>
        /// <returns>Un <c>int</c> que contiene el ID del cliente creado, cero en caso de falla</returns>
        private int InsertGiroCash472(
            string sessionId,
            EmitirRequest request,
            DwhModel.Cliente emisor,
            DwhModel.Cliente receptor,
            MultiPay472.RespuestaValidacionConstitucionGiro constitucion,
            MultiPay472.RespuestaEmisionGiro emision,
            out ErrorMessagesMnemonics returnCode,
            SqlConnection connection = null)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            returnCode = ErrorMessagesMnemonics.None;
            int ret = 0;

            try
            {
                this.ProviderLogger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Ejecutando query ..."));

                Dictionary<string, object> queryParams = new Dictionary<string, object>()
                {
                    { "@EmisorId", emisor.Id },
                    { "@ReceptorId", receptor.Id },
                    { "@Pdv", request.Pdv },
                    { "@TotalRecibido", request.ValorRecibido },
                    { "@TotalAEntregar", Convert.ToInt64(constitucion.Monto) },
                    { "@Flete", Convert.ToInt64(constitucion.ValorFlete) },
                    { "@IncluyeFlete", request.IncluyeFlete },
                    { "@FechaConstitucion", Cash472.CashProvider.ObtenerFechaDesdeString(constitucion.Fecha) },
                    { "@FechaEmision", Cash472.CashProvider.ObtenerFechaDesdeString(emision.Fecha) },
                    { "@CiudadOrigen", request.CiudadPdv },
                    { "@CiudadDestino", request.CiudadDestino },
                    { "@CodigoTransaccionConstitucion", constitucion.CodigoTransaccion },
                    { "@CodigoTransaccionEmision", emision.CodigoTransaccion },
                    { "@Token", constitucion.Token },
                    { "@Pin", emision.PIN },
                    { "@CodigoAutorizacion", emision.CodigoAutorizacion },
                    { "@NumeroFactura", emision.NumeroFactura },
                    { "@NumeroTransaccion", emision.NumeroReferencia },
                    { "@ExternalId", emision.IdGiro }
                };

                if (connection == null)
                {
                    using (connection = Utils.Database.GetCash472DbConnection())
                    {
                        connection.Open();
                        ret = (int)Utils.Dwh<int>.ExecuteScalar(
                        connection,
                        Queries.Cash.InsertGiroCash472,
                        queryParams,
                        null);
                    }
                }
                else
                {
                    ret = (int)Utils.Dwh<int>.ExecuteScalar(
                    connection,
                    Queries.Cash.InsertGiroCash472,
                    queryParams,
                    null);
                }

                this.ProviderLogger.ExceptionLow(() => TagValue.New()
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
