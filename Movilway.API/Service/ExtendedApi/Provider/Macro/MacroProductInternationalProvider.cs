using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using Movilway.API.Data.MacroProduct;
using Movilway.API.KinacuWebService;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Macro;
using Movilway.Logging;

namespace Movilway.API.Service.ExtendedApi.Provider.Macro
{
    public class MacroProductInternationalProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(MacroProductProvider));
        protected ILogger ProviderLogger { get { return logger; } }

        private void LogRequest(object request)
        {
            try
            {
                ProviderLogger.BeginLow(() => TagValue.New().Tag("Client IP").Value(HttpContext.Current.Request.UserHostAddress));
                ProviderLogger.BeginLow(() => TagValue.New().Tag("Request").Value(request));

                int timeOutSeconds = int.Parse(ConfigurationManager.AppSettings["DefaultTimeout"]);
                var to = timeOutSeconds * 1000;

                ProviderLogger.BeginLow(() => TagValue.New().Tag("TimeOutProvider").Value(to));
            }
            catch (Exception e)
            {
                ProviderLogger.ExceptionLow(() => TagValue.New().Message("Exception trying to log the Request").Exception(e));
            }
        }

        private void LogResponse(object response)
        {
            try
            {
                ProviderLogger.CheckPointLow(() => TagValue.New().Tag("Response").Value(response));
            }
            catch (Exception e)
            {
                ProviderLogger.ExceptionLow(() => TagValue.New().Message("Exception trying to log the Response").Exception(e));
            }
        }

        private String GetSessionId(DataContract.MacroInternational.GetMacroProductInternationalRequest request)
        {
            string username = request.AuthenticationData.Username, platform = request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"];
            var cacheObj = new LoginDataCache();

            if (request != null)
            {
                ObjectCache cache = MemoryCache.Default;
                var cacheKey = username + platform;

                ProviderLogger.InfoLow("*** Chequeo de cache ***");
                foreach (var item in cache)
                    ProviderLogger.InfoLow(item.Key + ": " + item.Value);
                ProviderLogger.InfoLow("************************");

                if (!String.IsNullOrEmpty(request.AuthenticationData.SessionID))
                {
                    return (request.AuthenticationData.SessionID);
                }
                else
                {
                    if (cache.Contains(cacheKey))
                        cacheObj = (LoginDataCache)cache.Get(cacheKey);
                    else
                    {
                        var getSessionResponse = new ServiceExecutionDelegator
                            <GetSessionResponseBody, GetSessionRequestBody>().ResolveRequest(
                                new GetSessionRequestBody()
                                {
                                    Username = request.AuthenticationData.Username,
                                    Password = request.AuthenticationData.Password,
                                    DeviceType = request.DeviceType
                                }, ApiTargetPlatform.Kinacu, ApiServiceName.GetSession);

                        var newCacheObject = new LoginDataCache()
                        {
                            UserName = username,
                            Platform = platform,
                            Token = getSessionResponse.SessionID
                        };

                        // Store data in the cache
                        var cacheMinutes = ConfigurationManager.AppSettings["UtibaSessionTTL"] ?? "10";
                        var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddSeconds(Convert.ToInt32(cacheMinutes) * 1000) };
                        //cache.Add(cacheKey, newCacheObject, cacheItemPolicy);
                        ProviderLogger.InfoHigh("cacheKey: " + cacheKey + " - newCacheObject: {" + newCacheObject.UserName + "," + newCacheObject.Platform + "," + newCacheObject.Token + "} - cacheItemPolicy: " + cacheItemPolicy.AbsoluteExpiration);

                        cacheObj = newCacheObject;
                    }
                }
            }

            return cacheObj.Token;
        }

        public DataContract.MacroInternational.GetMacroProductInternationalResponse GetMacroProductsByCategoryInter(DataContract.MacroInternational.GetMacroProductInternationalRequest request)
        {
            var response = new DataContract.MacroInternational.GetMacroProductInternationalResponse();

            try
            {
                LogRequest(request);
                var sessionId = GetSessionId(request);

                //var platformId = Convert.ToInt32(request.Platform); //1
                //var countryId = Convert.ToInt32(ConfigurationManager.AppSettings["CountryId"]); //10 

                //Platform products.
                var productList = new ServiceExecutionDelegator<GetProductListResponseBody, GetProductListRequestBody>().
                    ResolveRequest(new GetProductListRequestBody()
                    {
                        AuthenticationData = new AuthenticationData()
                        {
                            Username = request.AuthenticationData.Username,
                            Password = request.AuthenticationData.Password,
                            SessionID = sessionId
                        },
                        Agent = request.Agent,
                        DeviceType = request.DeviceType
                    }, request.Platform, ApiServiceName.GetProductList);

                if (productList.ProductList == null)
                {
                    response = new DataContract.MacroInternational.GetMacroProductInternationalResponse
                    {
                        ResponseCode = 99,
                        ResponseMessage = "Error: El usuario no tiene productos asignados en la plataforma.",
                        TransactionID = 0
                    };
                }
                else
                {
                    var dm = new Movilway.API.Data.MacroProduct.MacroProductInterDataManager();

                    if (String.IsNullOrEmpty(request.CountryId))
                    {
                        request.CountryId = "0";
                    }

                    response = dm.GetMacroProductsByCategoryInter(Convert.ToInt32(request.CountryId), productList.ProductList, request.DeviceType);

                    response.ResponseCode = 0;
                    response.ResponseMessage = "Exito.";
                    response.TransactionID = 0;
                }

                LogResponse(response);

                return response;
            }
            catch (Exception e)
            {
                ProviderLogger.ExceptionLow(() => TagValue.New().Message("Exception trying to serve KINACU Operation").Exception(e));
            }

            return response;
        }

        public DataContract.MacroInternational.GetMacroProductInternationalResponse GetMacroProductsInter(DataContract.MacroInternational.GetMacroProductInternationalRequest request)
        {
            var response = new DataContract.MacroInternational.GetMacroProductInternationalResponse();

            try
            {
                LogRequest(request);
                var sessionId = GetSessionId(request);

                //var platformId = Convert.ToInt32(request.Platform); //1
                //var countryId = Convert.ToInt32(ConfigurationManager.AppSettings["CountryId"]); //10 

                //Platform products.
                var productList = new ServiceExecutionDelegator<GetProductListResponseBody, GetProductListRequestBody>().
                    ResolveRequest(new GetProductListRequestBody()
                    {
                        AuthenticationData = new AuthenticationData()
                        {
                            Username = request.AuthenticationData.Username,
                            Password = request.AuthenticationData.Password,
                            SessionID = sessionId
                        },
                        Agent = request.Agent,
                        DeviceType = request.DeviceType
                    }, request.Platform, ApiServiceName.GetProductList);

                if (productList.ProductList == null)
                {
                    response = new DataContract.MacroInternational.GetMacroProductInternationalResponse
                    {
                        ResponseCode = 99,
                        ResponseMessage = "Error: El usuario no tiene productos asignados en la plataforma.",
                        TransactionID = 0
                    };
                }
                else
                {
                    var dm = new Movilway.API.Data.MacroProduct.MacroProductInterDataManager();

                    if (String.IsNullOrEmpty(request.CountryId))
                    {
                        request.CountryId = "0";
                    }

                    response = dm.GetMacroProductsInter(Convert.ToInt32(request.CountryId), productList.ProductList, request.DeviceType);

                    response.ResponseCode = 0;
                    response.ResponseMessage = "Exito.";
                    response.TransactionID = 0;
                }

                LogResponse(response);

                return response;
            }
            catch (Exception e)
            {
                ProviderLogger.ExceptionLow(() => TagValue.New().Message("Exception trying to serve KINACU Operation").Exception(e));
            }

            return response;
        }
    }
}