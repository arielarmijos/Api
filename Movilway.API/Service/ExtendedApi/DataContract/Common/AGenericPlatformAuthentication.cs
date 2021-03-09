// <copyright file="AGenericPlatformAuthentication.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Common
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Web;

    using Movilway.API.Service.ExtendedApi.Provider;
    using Movilway.Logging;

    /// <summary>
    /// Clase que contiene los mentodos de autenticacion basicos de interaccion con las
    /// plataformas, como lo son:
    ///     * LogRequest => escribe en log los datos de la peticion
    ///     * LogResponse => escribe en log los datos de la respuesta
    ///     * GetSessionId => realiza la autenticacion en la plataforma y retorna el SessionId
    /// </summary>
    public abstract class AGenericPlatformAuthentication
    {
        /// <summary>
        /// Cada clase debe implementar este atributo para que quede registrado su nombre
        /// en los logs
        /// </summary>
        protected static ILogger logger;

        /// <summary>
        /// Gets ProviderLogger
        /// <para />
        /// Acceso del objeto de log
        /// </summary>
        protected ILogger ProviderLogger
        {
            get
            {
                return logger;
            }
        }

        /// <summary>
        /// Escribe en log los datos de la peticion
        /// </summary>
        /// <param name="request">Base request object</param>
        protected void LogRequest(object request)
        {
            try
            {
                try
                {
                    this.ProviderLogger.BeginLow(() => TagValue.New().Tag("Client IP").Value(!string.IsNullOrEmpty(HttpContext.Current.Request.UserHostAddress) ? HttpContext.Current.Request.UserHostAddress : "NULL"));
                }
                catch (Exception)
                {
                    this.ProviderLogger.BeginLow(() => TagValue.New().Tag("Client IP").Value("NULL"));
                }

                this.ProviderLogger.BeginLow(() => TagValue.New().Tag("Request").Value(request));

                int timeOutSeconds = int.Parse(ConfigurationManager.AppSettings["DefaultTimeout"]);
                var to = timeOutSeconds * 1000;

                this.ProviderLogger.BeginLow(() => TagValue.New().Tag("TimeOutProvider").Value(to));
            }
            catch (Exception e)
            {
                this.ProviderLogger.ExceptionLow(() => TagValue.New().Message("Exception trying to log the Request").Exception(e));
            }
        }

        /// <summary>
        /// Escribe en log los datos de la respuesta
        /// </summary>
        /// <param name="response">Base request object</param>
        protected void LogResponse(object response)
        {
            try
            {
                this.ProviderLogger.CheckPointLow(() => TagValue.New().Tag("Response").Value(response));
            }
            catch (Exception e)
            {
                this.ProviderLogger.ExceptionLow(() => TagValue.New().Message("Exception trying to log the Response").Exception(e));
            }
        }

        /// <summary>
        /// Realiza la autenticacion en la plataforma y retorna el SessionId
        /// </summary>
        /// <param name="request">Objeto que contiene las credenciales del usuario</param>
        /// <param name="returnCode">Codigo de error en caso de que algo falle (-1 = OK, >-1 = Error)</param>
        /// <returns>Un <c>string</c> que contiene el <c>SessionId</c> del usuario autenticado</returns>
        protected string GetSessionId(ASecuredApiRequest request, out ErrorMessagesMnemonics returnCode)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            returnCode = ErrorMessagesMnemonics.None;

            this.ProviderLogger.InfoLow(() => TagValue.New()
                .MethodName(methodName)
                .Message("Obteniendo sesion ...")
                .Tag("Login").Value((request.AuthenticationData != null && request.AuthenticationData.Username != null) ? request.AuthenticationData.Username : "NULL")
                .Tag("DeviceType").Value(request.DeviceType)
                .Tag("RequestPlatformId").Value(string.IsNullOrEmpty(request.Platform) ? string.Empty : request.Platform));

            if (!this.ValidSecureApiRequest(request))
            {
                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("Imposible obtener sesion")
                    .Tag("Login").Value((request.AuthenticationData != null && request.AuthenticationData.Username != null) ? request.AuthenticationData.Username : "NULL")
                    .Tag("DeviceType").Value(request.DeviceType)
                    .Tag("RequestPlatformId").Value(string.IsNullOrEmpty(request.Platform) ? string.Empty : request.Platform));

                returnCode = ErrorMessagesMnemonics.MissingAuthenticationInformation;
                return string.Empty;
            }

            GetSessionResponseBody sessionResponse = new ServiceExecutionDelegator<GetSessionResponseBody, GetSessionRequestBody>()
                .ResolveRequest(
                    new GetSessionRequestBody()
                    {
                        Username = request.AuthenticationData.Username,
                        Password = request.AuthenticationData.Password,
                        DeviceType = request.DeviceType
                    },
                    request.Platform ?? string.Empty,
                    ApiServiceName.GetSession);

            if (sessionResponse == null
                || string.IsNullOrEmpty(sessionResponse.SessionID)
                || sessionResponse.SessionID.Equals("0")
                || sessionResponse.SessionID.Equals("error"))
            {
                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("Imposible obtener sesion")
                    .Tag("Login").Value((request.AuthenticationData != null && request.AuthenticationData.Username != null) ? request.AuthenticationData.Username : "NULL")
                    .Tag("DeviceType").Value(request.DeviceType)
                    .Tag("RequestPlatformId").Value(string.IsNullOrEmpty(request.Platform) ? string.Empty : request.Platform));

                returnCode = this.MapAuthenticationErrorMessageToErrorMnemonic(sessionResponse != null ? sessionResponse.ResponseMessage : null);
                return string.Empty;
            }
            else
            {
                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionResponse.SessionID + "] " + "Sesion obtenida")
                    .Tag("SessionId").Value(sessionResponse.SessionID));
            }

            return sessionResponse.SessionID;
        }

        /// <summary>
        /// Realiza la autenticacion en la plataforma y retorna el SessionId
        /// </summary>
        /// <param name="request">Objeto que contiene las credenciales del usuario</param>
        /// <param name="response">Objeto base de respuesta</param>
        /// <param name="returnCode">Codigo de error en caso de que algo falle (-1 = OK, >-1 = Error)</param>
        /// <returns>Un <c>String</c> que contiene el <c>SessionId</c> del usuario autenticado</returns>
        protected string GetSessionId(ASecuredApiRequest request, AGenericApiResponse response, out ErrorMessagesMnemonics returnCode)
        {
            returnCode = ErrorMessagesMnemonics.None;

            string sessionId = this.GetSessionId(request, out returnCode);

            if (returnCode != ErrorMessagesMnemonics.None)
            {
                response.ResponseCode = (int)returnCode;
                response.ResponseMessage = returnCode.ToDescription();
            }

            return sessionId;
        }

        /// <summary>
        /// Valida que la informacion proveniente de la solitud tenga todos los parametros obligatorios:
        /// AuthenticationData => Username, Password
        /// DeviceType
        /// </summary>
        /// <param name="request">Base request object</param>
        /// <returns><c>true</c> si es una petición válida, <c>false</c> en caso contrario</returns>
        private bool ValidSecureApiRequest(ASecuredApiRequest request) 
        {
            return request != null 
                && request.AuthenticationData != null 
                && !string.IsNullOrEmpty(request.AuthenticationData.Username) 
                && !string.IsNullOrEmpty(request.AuthenticationData.Password) 
                && request.DeviceType >= 0;
        }

        /// <summary>
        /// Mapea los mensajes de error identificados a un código mnemotécnico
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <returns>Código mnemotécnico de error</returns>
        private ErrorMessagesMnemonics MapAuthenticationErrorMessageToErrorMnemonic(string message) 
        {
            ErrorMessagesMnemonics ret = ErrorMessagesMnemonics.UnableToAuthenticate;
            
            if (!string.IsNullOrEmpty(message)) 
            {
                switch (message) 
                {
                    case "UserNotFound-Error de Login":
                        ret = ErrorMessagesMnemonics.InvalidUser;
                        break;
                    case "WrongPassword-Error de Login":
                        ret = ErrorMessagesMnemonics.InvalidPassword;
                        break;
                    default:
                        ret = ErrorMessagesMnemonics.UnableToAuthenticate;
                        break;
                }
            }

            return ret;
        }
    }
}
