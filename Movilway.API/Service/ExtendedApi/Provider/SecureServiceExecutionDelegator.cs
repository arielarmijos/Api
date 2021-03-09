using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Movilway.API.Core.Security;
using Movilway.API.Core;
using Movilway.API.Service.External;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;

namespace Movilway.API.Service.ExtendedApi.Provider
{
    /// <summary>
    /// Enumeracion
    /// </summary>
    public enum ApiSecurityMode {
            //valida las credenciales completas
            CREDENTIALS,
            //solo valida por usario
            USER
        };

    public class SecureServiceExecutionDelegator<K, T> : ServiceExecutionDelegator<K, T>
        where T : IMovilwayApiRequest
        where K : IMovilwayApiResponse
    {

        private ApiSecurityMode _securemode = ApiSecurityMode.CREDENTIALS;

        public SecureServiceExecutionDelegator()
        {
            
        }

        public SecureServiceExecutionDelegator(ApiSecurityMode mode)
        {
            _securemode = mode;
        }

        public override  K ResolveRequest(T request, ApiTargetPlatform targetPlatform, ApiServiceName serviceName)
        {
            //TODO herencia
           // BeginResolveRequest(request, ref targetPlatform, serviceName);

            IServiceProvider serviceImpl = ServiceProviderFactory.GetServiceProvider(targetPlatform, serviceName);

            SecureProvider secureprovider = new SecureProvider(_securemode, serviceImpl,typeof(K),targetPlatform);

            return ((K)secureprovider.PerformOperation(request));
        
        }

        public override K ResolveRequest(T request, string platform, ApiServiceName serviceName)
        {
            //TODO herencia
         //   BeginResolveRequest(request, ref platform, serviceName);
          
            IServiceProvider serviceImpl;
            if (platform.Equals("1"))
                serviceImpl = ServiceProviderFactory.GetServiceProvider(ApiTargetPlatform.Kinacu, serviceName);
            else if (platform.Equals("2"))
                serviceImpl = ServiceProviderFactory.GetServiceProvider(ApiTargetPlatform.Utiba, serviceName);
            else
                throw new Exception("Problemas identificando el platform");
            //else
            //{
            //    if ((defaultPlatform ?? "0") == "2")
            //        serviceImpl = ServiceProviderFactory.GetServiceProvider(ApiTargetPlatform.Kinacu, serviceName);
            //    else
            //        serviceImpl = ServiceProviderFactory.GetServiceProvider(ApiTargetPlatform.Utiba, serviceName);
            //}

            SecureProvider secureprovider = new SecureProvider(_securemode, serviceImpl, typeof(K), platform);

            return ((K)secureprovider.PerformOperation(request));

        }
       
    }

    /// <summary>
    /// Provider para validar una sola vez la seguridad de las peticiones al api
    /// </summary>
    class SecureProvider : IServiceProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(SecureProvider));

        private ApiSecurityMode _securemode;
        private IServiceProvider _serviceImpl = null;
        private Type _typeresponse = null;
        private ApiTargetPlatform _target ;
     


        public SecureProvider(ApiSecurityMode mode, IServiceProvider provider, Type typeresponse, ApiTargetPlatform targetPlatform)
        {

            _securemode = mode;
            _serviceImpl = provider;
            _typeresponse = typeresponse;
            _target = targetPlatform;
        }

        public SecureProvider(ApiSecurityMode mode, IServiceProvider provider, Type typeresponse, string platform)
        {

            _securemode = mode;
            _serviceImpl = provider;
            _typeresponse = typeresponse;
            switch (platform)
            {
                case "1":
                     _target = ApiTargetPlatform.Kinacu;
                     break;
                case "2":
                     _target = ApiTargetPlatform.Utiba;
                 break;
            }
        
            
        }


        public IMovilwayApiResponse PerformOperation(IMovilwayApiRequest requestObject )
        {
            try
            {
                //validar una vez la ip


                //ASecuredApiRequest securityrequest = null;

                GetSessionResponseBody responseBody = null;
                switch (_securemode)
                {
                    case ApiSecurityMode.CREDENTIALS:

                        //securityrequest = Reflection.FactoryObject<ASecuredApiRequest>(typeof(ASecuredApiRequest));

                        //securityrequest.AuthenticationData = new AuthenticationData();
                        //securityrequest.AuthenticationData.Username = requestObject.AuthenticationData.Username;
                        //securityrequest.AuthenticationData.Password = requestObject.AuthenticationData.Password;
                        //securityrequest.AuthenticationData.SessionID = requestObject.AuthenticationData.SessionID;
                        //securityrequest.AuthenticationData.Tokken = requestObject.AuthenticationData.Tokken;

                        responseBody = new ServiceExecutionDelegator
                          <GetSessionResponseBody, IMovilwayApiRequest>().ResolveRequest(
                              requestObject
                              , _target, ApiServiceName.GetSession);

                        break;
                    case ApiSecurityMode.USER:

                        //securityrequest = Reflection.FactoryObject<ASecuredApiRequest>(typeof(ASecuredApiRequest));
                        //securityrequest.AuthenticationData = new AuthenticationData();
                        //securityrequest.AuthenticationData.Username = requestObject.AuthenticationData.Username;
                        //securityrequest.AuthenticationData.Password = requestObject.AuthenticationData.Password;
                        //securityrequest.AuthenticationData.SessionID = string.Empty;
                        //securityrequest.AuthenticationData.Tokken = string.Empty;

                        responseBody = new ServiceExecutionDelegator
                    <GetSessionResponseBody, IMovilwayApiRequest>().ResolveRequest(
                           requestObject
                        , _target, ApiServiceName.GetSession);

                        break;
                }


                if (responseBody.ResponseCode == 0)//&& tokken es valido
                {
                    // si la validacion es segura
                    return _serviceImpl.PerformOperation(requestObject);
                }
                else
                {
                    IMovilwayApiResponse response = Reflection.FactoryObject<IMovilwayApiResponse>(_typeresponse);
                    response.ResponseCode = 90;
                    response.ResponseMessage = responseBody.ResponseMessage; //"ERROR DE SEGURIDAD DATOS ASOCIADOS AL TOKKEN INVALIDO";

                    return response;
                }

            }
            catch (Exception ex)
            {
                //TODO DESHABILITAR LA EJECUCION DE EST HANDLER
                IMovilwayApiResponse response = Reflection.FactoryObject<IMovilwayApiResponse>(_typeresponse);
                response.ResponseCode = 500;
                response.ResponseMessage = "ERROR INESPERADO EJECUTANDO SECUREPROVIDER";
                logger.ErrorHigh(String.Concat(response.ResponseCode, "-", response.ResponseMessage, " ", ex.Message, "-.", ex.StackTrace));
                return response;
            }

        }

        #region GetSessionID
      
        #endregion

    }
}