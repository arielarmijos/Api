// <copyright file="Cotizar.cs" company="Movilway">
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
    /// Implementación método CotizadorGiro
    /// </summary>
    internal partial class CashProvider : AGenericPlatformAuthentication
    {
        /// <summary>
        /// Cotizador de giros
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la cotización</param>
        /// <returns>Respuesta de la cotización</returns>
        public CotizarResponse Cotizar(CotizarRequest request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.LogRequest(request);

            CotizarResponse response = new CotizarResponse();
            string sessionId = this.GetSessionId(request, response, out this.errorMessage);
            if (this.errorMessage != ErrorMessagesMnemonics.None)
            {
                this.LogResponse(response);
                return response;
            }

            MultiPay472.Service1SoapClient client = this.GetSoapClient();
            string endpointName = "CotizadorGiro";
            try
            {
                MultiPay472.CotizadorGiro peticion = new MultiPay472.CotizadorGiro();
                peticion.NitRed = this.multipayNitRed;
                peticion.CodigoTerminal = this.multipayTerminal;
                peticion.CodigoTransaccion = this.GenerarCodigoTransaccion(sessionId);

                peticion.CodigoPuntoVenta = request.Pdv;
                peticion.ValorRecibido = request.ValorRecibido;
                peticion.IncluyeFlete = request.IncluyeFlete;

                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Llamando servicio \"" + endpointName + "\" ..."));

                MultiPay472.RespuestaCotizadorGiro resp = client.CotizadorGiro(peticion, this.multipayUsuario);

                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Respuesta servicio \"" + endpointName + "\"")
                    .Tag("Respuesta").Value(resp != null ? resp.CodigoRespuesta : "NULL"));

                if (resp != null && resp.CodigoRespuesta == CashProvider.CodigoRespuestaExitoso)
                {
                    response.ResponseCode = 0;
                    response.TotalARecibir = resp.ValorTotal;
                    response.TotalAEntregar = resp.ValorEntregaBeneficiario;
                    response.Flete = resp.ValorFlete;
                    response.CodigoTransaccion = resp.CodigoTransaccion;
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
