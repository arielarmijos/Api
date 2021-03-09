using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using Movilway.API.Service.ExtendedApi.DataContract.Common;

namespace Movilway.API.Service.ExtendedApi.Provider
{
    public class ServiceProviderFactory
    {
        //AV
        //TODO APESAR DE QUE HAYA UNA NUEVA REFERENCIA EN CADA OPERACION DEBE UTILIZARSE BLOCK PARA EVITAR PROBLEMAS DE CONCURRENCIA
        private static Dictionary<ApiTargetPlatform, Dictionary<ApiServiceName, Type>> _registeredProviders;

        static ServiceProviderFactory()
        {
            _registeredProviders = new Dictionary<ApiTargetPlatform, Dictionary<ApiServiceName, Type>>();
            Assembly currentAssembly = Assembly.GetAssembly(typeof(ServiceProviderFactory));
            foreach (Type definedType in currentAssembly.GetTypes())
            {
                if (definedType.GetCustomAttributes(typeof(ServiceProviderImplAttribute), false).Count() > 0)
                {
                    ServiceProviderImplAttribute metaData = (ServiceProviderImplAttribute)definedType.GetCustomAttributes(typeof(ServiceProviderImplAttribute), false).ElementAt(0);
                    RegisterServiceProviderClass(metaData.Platform, metaData.ServiceName, definedType);
                }
            }
        }

        public static void RegisterServiceProviderClass(ApiTargetPlatform targetPlatform, ApiServiceName serviceName, Type implementingType)
        {
           
            Dictionary<ApiServiceName, Type> platformList;
            if (_registeredProviders.ContainsKey(targetPlatform))
                platformList = _registeredProviders[targetPlatform];
            else
            {
                platformList = new Dictionary<ApiServiceName, Type>();
                _registeredProviders.Add(targetPlatform, platformList);
            }
            platformList.Add(serviceName, implementingType);
        }

        public static IServiceProvider GetServiceProvider(ApiTargetPlatform targetPlatform, ApiServiceName serviceName)
        {
            
            IServiceProvider serviceProviderImpl=null;
            if (_registeredProviders.ContainsKey(targetPlatform))
            {
                var registeredServicerForPlatform = _registeredProviders[targetPlatform];
                if (registeredServicerForPlatform.ContainsKey(serviceName))
                {
                    Type providerType = registeredServicerForPlatform[serviceName];
                    serviceProviderImpl = (IServiceProvider)Activator.CreateInstance(providerType);
                }
            }
            return(serviceProviderImpl);
        }
        public static ISecureServiceProvider GetServiceProviderSecure(ApiTargetPlatform targetPlatform, ApiServiceName serviceName)
        {
            ISecureServiceProvider serviceProviderImpl = null;
            if (_registeredProviders.ContainsKey(targetPlatform))
            {
                var registeredServicerForPlatform = _registeredProviders[targetPlatform];
                if (registeredServicerForPlatform.ContainsKey(serviceName))
                {
                    Type providerType = registeredServicerForPlatform[serviceName];
                    serviceProviderImpl = (ISecureServiceProvider)Activator.CreateInstance(providerType);
                }
            }
            return (serviceProviderImpl);
        }
    }
}