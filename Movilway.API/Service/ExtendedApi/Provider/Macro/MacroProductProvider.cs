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
using Movilway.API.Service.ExtendedApi.Provider.Kinacu;
using Movilway.Logging;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.Provider.Macro
{
    public class MacroProductProvider : AGenericPlatformAuthentication
    {
        ///<summary>Variable para almacenar todos los mensajes de retorno de los diferentes llamados</summary>
        private ErrorMessagesMnemonics errorMessage = ErrorMessagesMnemonics.None;

        ///<summary>Inicializacion de la libreria log</summary>
        static MacroProductProvider()
        {
            logger = LoggerFactory.GetLogger(typeof(MacroProductProvider));
        }

        public DataContract.Macro.GetMacroProductListByCategoryResponse GetMacroProductsByCategory(DataContract.Macro.GetMacroProductListRequest request)
        {
            LogRequest(request);

            var response = new DataContract.Macro.GetMacroProductListByCategoryResponse();
            var sessionId = GetSessionId(request, out errorMessage);

            logger.InfoLow("[API] [" + sessionId + "] [MacroProductProvider] [SEND-DATA] GetMacroProductsByCategory {sessionId = " + sessionId + ", UserId=" + request.Agent + ", DeviceType=" + request.DeviceType + "}");

            if (errorMessage != ErrorMessagesMnemonics.None)
            {
                response.ResponseCode = (int)errorMessage;
                response.ResponseMessage = errorMessage.ToDescription();
                logger.InfoLow("[API Binwus] [" + sessionId + "] [MacroProductProvider] [SEND-DATA] GetMacroProductsByCategory Error: " + errorMessage.ToDescription());

                return response;
            }

            try
            {
                var platformId = Convert.ToInt32(request.Platform); //1
                var countryId = Convert.ToInt32(ConfigurationManager.AppSettings["CountryId"]); //10 
                logger.InfoLow("[API BinwusA] [MacroProductProvider]");

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
                logger.InfoLow("[API BinwusB] [MacroProductProvider]");
                if (productList.ProductList == null)
                {
                    response = new GetMacroProductListByCategoryResponse
                                   {
                                       ResponseCode = 99,
                                       ResponseMessage = "Error: El usuario no tiene productos asignados en la plataforma.",
                                       TransactionID = 0
                                   };
                    logger.InfoLow("[API Binwus1] [" + sessionId + "] [MacroProductProvider] [SEND-DATA] GetMacroProductsByCategory Error: El usuario no tiene productos asignados en la plataforma.");
                }
                else
                {
                    logger.InfoLow("[API BinwusC.] [MacroProductProvider]");
                    var dm = new Movilway.API.Data.MacroProduct.MacroProductDataManager();
                    logger.InfoLow("[API BinwusD.] [MacroProductProvider]");
                    response = dm.GetMacroProductsByCategory(platformId, countryId, productList.ProductList, request.DeviceType);
                    logger.InfoLow("[API BinwusE.] [MacroProductProvider]");

                    logger.InfoLow("[API Binwus2] [" + sessionId + "] [MacroProductProvider] [RECV-DATA] GetMacroProductsByCategory {response={" + productList.ProductList.ToString() + "}}");

                    response.ResponseCode = 0;
                    response.ResponseMessage = "Exito.";
                    response.TransactionID = 0;
                }
            }
            catch (Exception e)
            {
                logger.InfoLow("[API Binwus3] [" + sessionId + "] [MacroProductProvider] [RECV-DATA] GetMacroProductsByCategory Error: " + e.Message);

                ProviderLogger.ExceptionLow(() => TagValue.New().Message("Exception trying to serve KINACU Operation").Exception(e));
            }

            LogResponse(response);

            return response;
        }

        public DataContract.Macro.GetMacroProductListByCategoryLightResponse GetMacroProductsByCategoryLight(DataContract.Macro.GetMacroProductListRequest request)
        {
            LogRequest(request);

            var response = new DataContract.Macro.GetMacroProductListByCategoryLightResponse();
            var sessionId = GetSessionId(request, out errorMessage);

            if (errorMessage != ErrorMessagesMnemonics.None)
            {
                response.ResponseCode = (int)errorMessage;
                response.ResponseMessage = errorMessage.ToDescription();
                return response;
            }

            try
            {
                var platformId = Convert.ToInt32(request.Platform); //1
                var countryId = Convert.ToInt32(ConfigurationManager.AppSettings["CountryId"]); //10 

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
                    response = new GetMacroProductListByCategoryLightResponse
                    {
                        ResponseCode = 99,
                        ResponseMessage = "Error: El usuario no tiene productos asignados en la plataforma.",
                        TransactionID = 0
                    };
                }
                else
                {
                    var dm = new Movilway.API.Data.MacroProduct.MacroProductDataManager();
                    response = dm.GetMacroProductsByCategoryLight(platformId, countryId, productList.ProductList, request.DeviceType);

                    response.ResponseCode = 0;
                    response.ResponseMessage = "Exito.";
                    response.TransactionID = 0;
                }
            }
            catch (Exception e)
            {
                ProviderLogger.ExceptionLow(() => TagValue.New().Message("Exception trying to serve KINACU Operation").Exception(e));
            }

            LogResponse(response);

            return response;
        }

        public DataContract.Macro.GetMacroProductDetailsResponse GetMacroProductDetails(DataContract.Macro.GetMacroProductDetailsRequest request)
        {
            LogRequest(request);

            var response = new DataContract.Macro.GetMacroProductDetailsResponse();
            var sessionId = GetSessionId(request, out errorMessage);

            if (errorMessage != ErrorMessagesMnemonics.None)
            {
                response.ResponseCode = (int)errorMessage;
                response.ResponseMessage = errorMessage.ToDescription();
                return response;
            }

            try
            {
                var platformId = Convert.ToInt32(request.Platform); //1
                var countryId = Convert.ToInt32(ConfigurationManager.AppSettings["CountryId"]); //10 

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
                    response = new GetMacroProductDetailsResponse
                    {
                        ResponseCode = 99,
                        ResponseMessage = "Error: El usuario no tiene productos asignados en la plataforma.",
                        TransactionID = 0
                    };
                }
                else
                {
                    var dm = new Movilway.API.Data.MacroProduct.MacroProductDataManager();
                    response = dm.GetMacroProductDetails(platformId, countryId, request.MacroProductId, productList.ProductList, request.DeviceType);

                    response.ResponseCode = 0;
                    response.ResponseMessage = "Exito.";
                    response.TransactionID = 0;
                }
            }
            catch (Exception e)
            {
                ProviderLogger.ExceptionLow(() => TagValue.New().Message("Exception trying to serve KINACU Operation").Exception(e));
            }

            LogResponse(response);

            return response;
        }

        public DataContract.Macro.GetMacroProductListResponse GetMacroProducts(DataContract.Macro.GetMacroProductListRequest request)
        {
            LogRequest(request);


            var response = new DataContract.Macro.GetMacroProductListResponse();

            var sessionId = GetSessionId(request, out errorMessage);

            if (errorMessage != ErrorMessagesMnemonics.None)
            {
                response.ResponseCode = (int)errorMessage;
                response.ResponseMessage = errorMessage.ToDescription();
                return response;
            }

            try
            {
                var platformId = Convert.ToInt32(request.Platform); //1
                var countryId = Convert.ToInt32(ConfigurationManager.AppSettings["CountryId"]); //10 

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
                    response = new GetMacroProductListResponse
                    {
                        ResponseCode = 99,
                        ResponseMessage = "Error: El usuario no tiene productos asignados en la plataforma.",
                        TransactionID = 0
                    };
                }
                else
                {
                    var dm = new MacroProductDataManager();
                    response = dm.GetMacroProducts(platformId, countryId, productList.ProductList, request.DeviceType);
                    response.ResponseCode = 0;
                    response.ResponseMessage = "Exito.";
                    response.TransactionID = 0;
                }
            }
            catch (Exception e)
            {
                ProviderLogger.ExceptionLow(() => TagValue.New().Message("Exception trying to serve KINACU Operation").Exception(e));
            }

            LogResponse(response);

            return response;
        }

        internal GetFavoriteAmountsResponse GetFavoriteAmounts(GetFavoriteAmountsRequest request)
        {
            LogRequest(request);

            var response = new GetFavoriteAmountsResponse();

            var sessionId = GetSessionId(request, out errorMessage);

            if (errorMessage != ErrorMessagesMnemonics.None)
            {
                response.ResponseCode = (int)errorMessage;
                response.ResponseMessage = errorMessage.ToDescription();
                return response;
            }

            try
            {
                var platformId = Convert.ToInt32(request.Platform); //1
                var countryId = Convert.ToInt32(ConfigurationManager.AppSettings["CountryId"]); //10 

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
                    response = new GetFavoriteAmountsResponse()
                    {
                        ResponseCode = 99,
                        ResponseMessage = "Error: El usuario no tiene productos asignados en la plataforma.",
                        TransactionID = 0
                    };
                }
                else
                {
                    var dm = new MacroProductDataManager();
                    response.FavoriteAmounts = dm.GetFavoriteAmounts(countryId);
                    response.ResponseCode = 0;
                    response.ResponseMessage = "Exito.";
                    response.TransactionID = 0;
                }
            }
            catch (Exception e)
            {
                ProviderLogger.ExceptionLow(() => TagValue.New().Message("Exception trying to serve KINACU Operation").Exception(e));
            }

            LogResponse(response);

            return response;
        }
    }
}