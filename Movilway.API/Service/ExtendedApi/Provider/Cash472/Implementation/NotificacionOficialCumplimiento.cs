// <copyright file="NotificacionOficialCumplimiento.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.Provider.Cash472
{
    using System;
    using System.Configuration;

    using DataContract.Cash472;
    using DataContract.Common;
    using Movilway.Logging;

    /// <summary>
    /// Implementación método NotificacionOficialCumplimiento
    /// </summary>
    internal partial class CashProvider : AGenericPlatformAuthentication
    {
        /// <summary>
        /// Realiza el proceso de notificación al oficial de cumplimento para un cliente en listas restrictivas
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la notificación</param>
        /// <returns>Respuesta de la notificación</returns>
        public NotificacionOficialCumplimientoResponse NotificacionOficialCumplimiento(NotificacionOficialCumplimientoRequest request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.LogRequest(request);

            NotificacionOficialCumplimientoResponse response = new NotificacionOficialCumplimientoResponse();
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

            DwhModel.Cliente infoCliente = this.GetInfoCliente(sessionId, request.TipoIdentificacion, request.NumeroIdentificacion.Trim(), out this.errorMessage);
            if (this.errorMessage != ErrorMessagesMnemonics.None)
            {
                this.SetResponseErrorCode(response, this.errorMessage);
                this.LogResponse(response);
                return response;
            }

            try
            {
                string mail = ConfigurationManager.AppSettings["MailOficialCumplimiento"].ToString();
                string subject = string.Format("Giros - {0} en Listas Restrictivas", request.TipoCliente.ToString());
                string body = string.Concat(
                    "<div style='font-family: Tahoma, \"Arial\", Tahoma, sans-serif;'>",
                    "El usuario con Tipo Documento : ",
                    request.TipoIdentificacion.ToString(),
                    " Número: ",
                    request.NumeroIdentificacion,
                    request.TipoCliente == TipoCliente.Emisor ? " está emitiendo un giro" : " tiene giros para ser pagados",
                    " y se encontró una coincidencia en listas restrictivas (ONU, OFAC).",
                    "<br/><br/><p style='color:Gray'>PD: por favor no responda este mensaje</p></div>");

                Utils.Mailer.SendMail(mail, subject, body);
            }
            catch (Exception ex)
            {
                this.ProviderLogger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Error enviando correo")
                    .Exception(ex));
                return response;
            }

            response.ResponseCode = 0;
            this.LogResponse(response);
            return response;
        }
    }
}
