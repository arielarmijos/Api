// <copyright file="ReversoPago.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.Provider.Cash472
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using DataContract.Cash472;
    using DataContract.Common;
    using Movilway.Logging;

    /// <summary>
    /// Implementación método ReversoPagoGiro
    /// </summary>
    internal partial class CashProvider : AGenericPlatformAuthentication
    {
        /// <summary>
        /// Realiza el proceso de reverso de pago para un giro
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del reverso de pago</param>
        /// <returns>Respuesta del pago</returns>
        public ReversoPagoResponse ReversoPago(ReversoPagoRequest request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.LogRequest(request);

            ReversoPagoResponse response = new ReversoPagoResponse();
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

            MultiPay472.Service1SoapClient client = this.GetSoapClient();
            string endpointName = "ReversoPagoGiro";
            try
            {
                MultiPay472.PagoGiro peticion = new MultiPay472.PagoGiro();
                peticion.NitRed = this.multipayNitRed;
                peticion.CodigoTerminal = this.multipayTerminal;
                peticion.CodigoTransaccion = this.GenerarCodigoTransaccion(sessionId);
                peticion.ConHuella = false;

                peticion.IdGiro = request.Id;
                peticion.NumeroComprobantePago = request.NumeroComprobantePago;
                peticion.CodigoPuntoVenta = request.Pdv;
                peticion.CodigoDaneCiudadPuntoVenta = request.CiudadPdv;
                peticion.Valor = request.Valor;
                peticion.OIdentificacionCliente = new MultiPay472.IdentificacionCliente();
                peticion.OIdentificacionCliente.TipoIdentificacion = Cash472.CashProvider.ObtenerCodigoTipoIdentificacion(request.TipoIdentificacion);
                peticion.OIdentificacionCliente.NumeroIdentificacion = request.NumeroIdentificacion;

                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Llamando servicio \"" + endpointName + "\" ..."));

                MultiPay472.RespuestaPagoGiro resp = client.ReversoPagoGiro(peticion, this.multipayUsuario);

                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Respuesta servicio \"" + endpointName + "\"")
                    .Tag("Respuesta").Value(resp != null ? resp.CodigoRespuesta : "NULL"));

                if (resp != null && resp.CodigoRespuesta == CashProvider.CodigoRespuestaExitoso)
                {
                    response.ResponseCode = 0;
                    response.NumeroFactura = resp.NumeroFactura;
                    response.CodigoTransaccion = !string.IsNullOrEmpty(resp.CodigoTransaccion) ? resp.CodigoTransaccion : string.Empty;
                    response.CodigoAutorizacion = resp.CodigoAutorizacion;
                    response.NumeroComprobantePago = resp.NumeroComprobantePago;
                    response.NumeroReferencia = resp.NumeroReferencia;
                    response.Valor = resp.ValorPago;
                    response.Fecha = CashProvider.ObtenerFechaDesdeString(resp.Fecha);
                }
                else
                {
                    if (resp == null)
                    {
                        this.errorMessage = ErrorMessagesMnemonics.WebServiceDoesNotRespond;
                        response.ResponseCode = (int)this.errorMessage;
                        response.ResponseMessage = this.errorMessage.ToDescription();
                    }
                    else
                    {
                        response.ResponseMessage = CashProvider.ObtenerMensajeCodigoRespuesta(resp.CodigoRespuesta);
                    }
                }
            }
            catch (Exception ex)
            {
                this.ProviderLogger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Error llamando servicio \"" + endpointName + "\"")
                    .Exception(ex));
            }

            this.LogResponse(response);
            return response;
        }
    }
}
