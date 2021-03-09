using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using System.ServiceModel;
using Movilway.Logging;
using Movilway.API.KinacuWebService;
using System.Runtime.Caching;
using System.Configuration;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetSession)]
    public class GetSessionProvider:AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetSessionProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {

            GetSessionResponseBody response = null;
       
            //TODO APLICAR CACHE
            GetSessionRequestBody request = requestObject as GetSessionRequestBody;
                response = new GetSessionResponseBody();
              

                    if(
                            request.AuthenticationData == null ||
                            ( 
                                String.IsNullOrEmpty( request.AuthenticationData.Username) 
                                || String.IsNullOrEmpty( request.AuthenticationData.Password)
                            )
                        )
                    {

                          response.ResponseCode = 90;
                          response.ResponseMessage = "DATOS DE AUTENTICACIÓN INVALIDOS";
                          response.TransactionID = 0;
                          response.SessionID = "";
                          return response;
                    }
                string result = "";
                #region comentarioanteriores
            
            /*
            var cacheObj = new LoginDataCache();
            ObjectCache cache = MemoryCache.Default;
            var cacheKey = request.AuthenticationData.Username + request.Platform;

            //logger.InfoLow("en el cache tengo " + cache.Count());
            if (cache.Contains(cacheKey))
            {
                kinacuWS.LogOff(int.Parse(((LoginDataCache)cache.Get(cacheKey)).Token));
                //logger.InfoLow("ya hicimos logoff de " + ((LoginDataCache)cache.Get(cacheKey)).Token);
                cache.Remove(cacheKey);
                }*/
                #endregion

            logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetSessionProvider] [SEND-DATA] loginParameters {accessId=" + request.AuthenticationData.Username + ",password=******,accessType=" + request.DeviceType + "}");

            int newSessionResponse = kinacuWS.Login(request.AuthenticationData.Username, request.AuthenticationData.Password, request.DeviceType, out result);

            logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetSessionProvider] [RECV-DATA] loginResult {response=" + newSessionResponse + ",result=" + result + "}");

            var myResponseCode = newSessionResponse != 0 ? 0 : GetResponseCode(result);
            response = new GetSessionResponseBody()
            {
                ResponseCode = myResponseCode,
                TransactionID = 0,
                SessionID = myResponseCode.Equals(1013) ? "1013" : newSessionResponse.ToString()
            };

            if (newSessionResponse == 0)
                response.ResponseMessage = result;

                #region comentarioanteriores
                //if (response.ResponseCode == 0)
                //{
                // REPG2013 - esto queda deshabilitado por ahora
                /*
                var newCacheObject = new LoginDataCache()
                {
                    UserName = request.AuthenticationData.Username,
                    Platform = request.Platform,
                    Token = response.SessionID
                };
                // Store data in the cache
                var cacheMinutes = ConfigurationManager.AppSettings["UtibaSessionTTL"] ?? "10";
                var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddSeconds(Convert.ToInt32(cacheMinutes) * 1000) };
                //cache.Add(cacheKey, newCacheObject, cacheItemPolicy);
                cacheObj = newCacheObject;
                logger.InfoLow("ya guarde en cache");
                */
                //}
                #endregion

            return (response);
        }

        private int GetResponseCode(string result)
        {
            int responseCode = 99;

            if (result.Contains('-'))
                if (int.TryParse(result.Split('-')[0], out responseCode))
                {
                    //if (responseCode == 1013)
                        //responseCode = 10133;
                    if (responseCode == 1017)
                        responseCode = 1013;
                }
                else
                    responseCode = 99;
            
            return responseCode;
        }
    }
}