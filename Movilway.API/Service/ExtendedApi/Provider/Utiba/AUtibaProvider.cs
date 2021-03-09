using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Movilway.API.Utiba;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net;
using System.Diagnostics;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.Provider.Utiba.Custom;
using Movilway.Logging;
using Movilway.API.Data;
using System.Configuration;
using System.Web;
using System.Threading;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba
{
    public abstract class AUtibaProvider : IServiceProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(AUtibaProvider));
        protected abstract ILogger ProviderLogger { get; }
        protected virtual TransactionType TransactionType
        {
            get { throw new NotImplementedException("The property <TransactionType> must be implemented for providers for Financial Transactions"); }
        }
        protected string LOG_PREFIX { get; set; }

        public IMovilwayApiResponse PerformOperation(IMovilwayApiRequest request)
        {
            try
            {
                LOG_PREFIX = HttpContext.Current.Session["LOG_PREFIX"].ToString() + "[" + new Random(DateTime.Now.Millisecond * 5).Next(100000000, 999999999) + "] ";
            }
            catch (Exception) 
            {
                LOG_PREFIX = "";
            }

            UMarketSCClient utibaClient = new UMarketSCClient();
            IMovilwayApiResponse response = null;
            try
            {
                try 
                {
                    logger.InfoLow("[API] " + LOG_PREFIX + "[UtibaProvider] [INPUT] UserInfo {IP=" + HttpContext.Current.Request.UserHostAddress + ",Username=" + request.AuthenticationData.Username + "} " + request.ToString());
                }
                catch (Exception)
                {
                    logger.InfoLow("[API] " + LOG_PREFIX + "[UtibaProvider] [INPUT] UserInfo {IP=" + "NULL" + ",Username=" + request.AuthenticationData.Username + "} " + request.ToString());
                }

                int timeOutSeconds = int.Parse(ConfigurationManager.AppSettings["DefaultTimeout"]);
                utibaClient.InnerChannel.OperationTimeout = new TimeSpan(0, 0, timeOutSeconds);

                String sessionID = null;
                if (!(request is GetSessionRequestBody))
                    sessionID = GetSessionID(request);

                response = PerformUtibaOperation(request, utibaClient, sessionID);

                logger.InfoLow("[API] " + LOG_PREFIX + "[UtibaProvider] [OUTPUT] " + response.ToString());
            }
            catch (Exception e)
            {
                logger.ErrorLow("[API] " + LOG_PREFIX + "[UtibaProvider] [EXCEPTION] Exception trying to serve UTIBA Operation {message=" + e.Message + ",stackTrace=" + e.StackTrace + "}");
            }

            return response;
        }

        private String GetSessionID(IMovilwayApiRequest request)
        {
            //string username = request.AuthenticationData.Username, platform = request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"];
            //var cacheObj = new LoginDataCache();

            if (request != null)
            {
                //ObjectCache cache = MemoryCache.Default;
                //var cacheKey = username + platform;

                //ProviderLogger.InfoLow("*** Chequeo de cache ***");
                //foreach (var item in cache)
                //ProviderLogger.InfoLow(item.Key + ": " + item.Value);
                //ProviderLogger.InfoLow("************************");

                if (request.AuthenticationData.SessionID != null)
                {
                    return request.AuthenticationData.SessionID;
                }
                else
                {
                    //if (cache.Contains(cacheKey))
                    //    cacheObj = (LoginDataCache)cache.Get(cacheKey);
                    //else
                    //{


                    var getSessionResponse = new ServiceExecutionDelegator
                        <GetSessionResponseBody, GetSessionRequestBody>().ResolveRequest(
                            new GetSessionRequestBody()
                            {
                                Username = request.AuthenticationData.Username,
                                Password = request.AuthenticationData.Password,
                                DeviceType = request.DeviceType
                            }, ApiTargetPlatform.Utiba, ApiServiceName.GetSession);


                    //if (ConfigurationManager.AppSettings["ProcessMigration"].ToLower() == "true")
                    //{
                    //    logger.InfoHigh("Comienza la migración del usuario: " + request.AuthenticationData.Username);
                    //    bool migrateAgent = MigrateAgent(request.AuthenticationData.Username);
                    //    if (migrateAgent && getSessionResponse.ResponseCode == 0)
                    //    {
                    //        // Cambio de password Kinacu
                    //        var changePinResponse = new ServiceExecutionDelegator<ChangePinResponseBody, ChangePinRequestBody>().ResolveRequest(
                    //                                            new ChangePinRequestBody()
                    //                                            {
                    //                                                AuthenticationData = new AuthenticationData()
                    //                                                {
                    //                                                    Username = request.AuthenticationData.Username,
                    //                                                    Password = ConfigurationManager.AppSettings["StandardOldPin"]
                    //                                                },
                    //                                                DeviceType = int.Parse(ConfigurationManager.AppSettings["StandardNewDeviceType"]),
                    //                                                Agent = request.AuthenticationData.Username,
                    //                                                OldPin = ConfigurationManager.AppSettings["StandardOldPin"],
                    //                                                NewPin = request.AuthenticationData.Password
                    //                                            }, ApiTargetPlatform.Kinacu, ApiServiceName.ChangePin);

                    //        // Login con Kinacu - NOT NOW - La proxima vez que entre va por Kinacu de una

                    //        // Save in DB
                    //        if (changePinResponse.ResponseCode == 0)
                    //        {
                    //            logger.InfoHigh("Se migró exitosamente la clave del usuario: " + request.AuthenticationData.Username);
                    //            SaveAgentMigrated(request.AuthenticationData.Username);
                    //        }
                    //    }
                    //}

                    return getSessionResponse.SessionID;
         
                    //var newCacheObject = new LoginDataCache()
                    //{
                    //    UserName = username,
                    //    Platform = platform,
                    //    Token = getSessionResponse.SessionID
                    //};

                    //// Store data in the cache
                    //var cacheMinutes = ConfigurationManager.AppSettings["UtibaSessionTTL"] ?? "10";
                    //var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddSeconds(Convert.ToInt32(cacheMinutes) * 1000) };
                    ////cache.Add(cacheKey, newCacheObject, cacheItemPolicy);
                    //ProviderLogger.InfoHigh("cacheKey: " + cacheKey + " - newCacheObject: {" + newCacheObject.UserName + "," + newCacheObject.Platform + "," + newCacheObject.Token + "} - cacheItemPolicy: " + cacheItemPolicy.AbsoluteExpiration);

                    //cacheObj = newCacheObject;
                    //}
                }
            }

            return "error";
            //return cacheObj.Token;
        }


        public abstract IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID);

    }
}