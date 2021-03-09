using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using System.Configuration;
using Movilway.API.KinacuWebService;
using Movilway.API.Core;
using Movilway.Cache;
using Movilway.API.Data.MacroProduct;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.TopUp)]
    public class TopUpProvider:AKinacuProvider
    {
        private static bool _secondRequestBandera = Boolean.TryParse(ConfigurationManager.AppSettings["SECOND_REQUEST_TopUpProvider"], out _secondRequestBandera);


        private ICache _cacheSession = null;
        private ICache _cacheSaldo = null;


        

        public TopUpProvider()
        {
            _cacheSession = Cache.CacheFactory.GetSingleInstaceCacheSession;
            _cacheSaldo = Cache.CacheFactory.GetSingleInstaceCacheSaldo;
            _delegateSecondExecution += SecondKinacuOperation;
        }

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(TopUpProvider));
        protected override ILogger ProviderLogger { get { return logger; } }
        protected override TransactionType TransactionType { get { return TransactionType.topup; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            //if (sessionID.Equals("0"))
            //    return new TopUpResponseBody()
            //    {
            //        ResponseCode = 90,
            //        ResponseMessage = "error session",
            //        TransactionID = 0,
            //        ExternalTransactionReference = "",
            //        PointBalance = 0m,
            //        StockBalance = 0m,
            //        WalletBalance = 0m
            //    };

            TopUpRequestBody request = requestObject as TopUpRequestBody;

            int timeOutSeconds = int.Parse(ConfigurationManager.AppSettings["DefaultTimeout"]);
            if (ConfigurationManager.AppSettings["TopUp_Timeout_" + request.MNO.ToLower()] != null)
                timeOutSeconds = int.Parse(ConfigurationManager.AppSettings["TopUp_Timeout_" + request.MNO.ToLower()]);

            kinacuWS.Timeout = timeOutSeconds * 1000;

            int transactionId;
            string saleData = "";
            string message = "";

            logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[TopUpProvider] [SEND-DATA] newSaleWithExternalIdParameters {UserId=" + sessionID + ",IdProduct=" + MapMNOtoProductId(request.MNO) + ",Customer=" + request.Recipient + 
                                                    ",Amount=" + (request.Amount * 100) + ",CommitSale=" + "1" + ",ExternalId=" + request.ExternalTransactionReference + ",PdvRepresented=" + (request.TerminalID ?? "") + "}");

            TopUpResponseBody response = null;
            bool kinacuTopUpResponse = false;

            int _Amount = (int)(request.Amount * 100);
            if (_cacheSession.IsActiveCache())
            {

                //logger.ErrorHigh(String.Concat("[API] ", LOG_PREFIX, "[KinacuProvider] [EXCEPTION] [", ex.GetType().Name.ToUpper(), "] ACCEDIENDO AL CACHE VALIDANDO SECURITY {message=", ex.Message, ",stackTrace=", ex.StackTrace, "}"));

                logger.InfoLow(String.Concat("[KIN] " , base.LOG_PREFIX , "CACHE ACTIVO TOPUP"));
                ///
                kinacuTopUpResponse = kinacuWS.NewSaleWithExternalId(int.Parse(sessionID), int.Parse(MapMNOtoProductId(request.MNO)), request.Recipient, 
                    //(int)(request.Amount * 100)
                    _Amount
                    , 1, request.ExternalTransactionReference, (request.TerminalID ?? ""), out transactionId, out saleData, out message);

                saleData = Movilway.API.Utils.ApiTicket.FormatSaledataTopUp(logger, this.GetType().Name, base.LOG_PREFIX, request, kinacuTopUpResponse, transactionId, saleData);


                response = new TopUpResponseBody()
                {
                    ResponseCode = Utils.BuildResponseCode(kinacuTopUpResponse, message),
                    ResponseMessage = kinacuTopUpResponse ? saleData : message,
                    TransactionID = transactionId,
                    ExternalTransactionReference = request.ExternalTransactionReference,
                    Fee = 0
                };


                //CODIGO REPETIDO
                logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[TopUpProvider] [RECV-DATA] newSaleWithExternalIdResult {response=" + kinacuTopUpResponse + ",IdTransaction=" + transactionId + ",SaleData=" + saleData + ",message=" + message + "}");

                bool errorSession = GetResponseCode(message) == 21 ;

               

                if (errorSession)
                {
                   logger.InfoLow(String.Concat("[KIN] ", base.LOG_PREFIX, " TOP UP ID DE SESSION INVALIDO RECALCULAR SESSION"));
                    //solicitar de nuevo session



                    var sessionRequest = new GetSessionRequestBody()
                    {
                        Username = request.AuthenticationData.Username,
                        Password = request.AuthenticationData.Password,
                        DeviceType = request.DeviceType
                    };

                    var getSessionResponse = new ServiceExecutionDelegator
                    <GetSessionResponseBody, GetSessionRequestBody>().ResolveRequest( 
                        sessionRequest
                        , ApiTargetPlatform.Kinacu, ApiServiceName.GetSession);

                    sessionID = getSessionResponse.SessionID;
                    logger.InfoLow(String.Concat("[KIN] ", base.LOG_PREFIX, " NUEVO ID ", IsValidId(sessionID)));

                         //String llave = String.Concat(cons.CACHE_SESSION_PREFIX, request.AuthenticationData.Username);
                         String llave = String.Concat(request.AuthenticationData.Username, " ", request.AuthenticationData.Password);

                         _cacheSession.AddOrUpdate<String>(llave, sessionID);

                    kinacuTopUpResponse = kinacuWS.NewSaleWithExternalId(int.Parse(sessionID), int.Parse(MapMNOtoProductId(request.MNO)), request.Recipient, 
                    //(int)(request.Amount * 100)
                    _Amount
                    , 1, request.ExternalTransactionReference, (request.TerminalID ?? ""), out transactionId, out saleData, out message);

                    saleData = Movilway.API.Utils.ApiTicket.FormatSaledataTopUp(logger, this.GetType().Name, base.LOG_PREFIX, request, kinacuTopUpResponse, transactionId, saleData);



                    response = new TopUpResponseBody()
                    {
                        ResponseCode = Utils.BuildResponseCode(kinacuTopUpResponse, message),
                        ResponseMessage = kinacuTopUpResponse ? saleData : message,
                        TransactionID = transactionId,
                        ExternalTransactionReference = request.ExternalTransactionReference,
                        Fee = 0
                    };
                }

            }
            else
            {

                kinacuTopUpResponse = kinacuWS.NewSaleWithExternalId(int.Parse(sessionID), int.Parse(MapMNOtoProductId(request.MNO)), request.Recipient, 
                    //(int)(request.Amount * 100)
                    _Amount
                    , 1, request.ExternalTransactionReference, (request.TerminalID ?? ""), out transactionId, out saleData, out message);


                saleData = Movilway.API.Utils.ApiTicket.FormatSaledataTopUp(logger, this.GetType().Name, base.LOG_PREFIX, request, kinacuTopUpResponse, transactionId, saleData);


                response = new TopUpResponseBody()
                {
                    ResponseCode = Utils.BuildResponseCode(kinacuTopUpResponse, message),
                    ResponseMessage = kinacuTopUpResponse ? saleData : message,
                    TransactionID = transactionId,
                    ExternalTransactionReference = request.ExternalTransactionReference,
                    Fee = 0
                };


                //CODIGO REPETIDO
                logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[TopUpProvider] [RECV-DATA] newSaleWithExternalIdResult {response=" + kinacuTopUpResponse + ",IdTransaction=" + transactionId + ",SaleData=" + saleData + ",message=" + message + "}");
            }


            if (request.DeviceType == cons.ACCESS_H2H)
            {
                string llave = String.Concat(request.AuthenticationData.Username);
                decimal[] result = { 0.0m, 0.0m, 0.0m };

                //try
                //{
                //    Func<decimal[]> callback = delegate()//null;
                //    {
                //        logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[TopUpProvider] CALLBACK SALDO");
                //        List<decimal> lista = new List<decimal>();
                //        AuthenticationData cascadeAuth = new AuthenticationData()
                //        {
                //            SessionID = sessionID
                //        };
                //        GetBalanceResponseBody balanceResponse = new ServiceExecutionDelegator<GetBalanceResponseBody, GetBalanceRequestBody>().ResolveRequest(new GetBalanceRequestBody()
                //        {
                //            AuthenticationData = cascadeAuth,
                //            DeviceType = request.DeviceType
                //        }, ApiTargetPlatform.Kinacu, ApiServiceName.GetBalance);
                //        lista.Add(balanceResponse.StockBalance.Value);
                //        lista.Add(response.WalletBalance = balanceResponse.WalletBalance.Value);
                //        lista.Add(response.PointBalance = balanceResponse.PointsBalance.Value);
                //        return lista.ToArray<decimal>();
                //    };
                //    //    ()=>{
                //    //  new int[]{0,0,0};
                //    //};
                //     result = _cacheSaldo.GetValue<decimal[]>(llave, callback);
                //}
                //catch (Exception ex)
                //{
                //    logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[TopUpProvider] ERROR AL CONSULTAR CHACHE SALDO 1");
                //   // throw;
                //}

                try
                {
                    //CUANDO SE RECARGA EL SALDO
                    Func<decimal[]> callback = delegate()
                    {
                        logger.InfoLow(String.Concat("[KIN] ", base.LOG_PREFIX, "[TopUpProvider] SALDO NOT FOUND CACHE "));
                        return new HandlerCacheSaldo().HandlerCache(new AuthenticationData()
                        {
                            SessionID = sessionID
                        }, request);
                    };

                    bool inCache = false;
                    //CUANDO SE ENCUENTRA EL SALDO EN CACHE
                    //TODO plantear un accion por referencia
                    //TODO se puede lanzar la actualizacion en cache asincrona
                    Action<Object, Object> accion = delegate(Object key2, Object value)
                    {
                        inCache = true;
                        logger.InfoLow(String.Concat("[KIN] ", base.LOG_PREFIX, "[TopUpProvider] SALDO FOUND CACHE [", key2, "]"));
                    };

                    result = _cacheSaldo.GetValue<decimal[]>(llave, callback, accion);

                    //si la respuesta es valid
                    //if (inCache && response.ResponseCode == 0)
                    //{
                    //    result[0] -= _Amount;
                    //    logger.InfoLow(String.Concat("[KIN] ", base.LOG_PREFIX, "[TopUpProvider] ACTUALIZANDO STOCKBALANCE EN CACHE [", result[0], "]"));
                    //    _cacheSaldo.AddOrUpdate(llave, result);
                    //}

                }
                catch (Exception ex)
                {
                    logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[TopUpProvider] ERROR AL CONSULTAR CHACHE SALDO ");
                    throw;
                }



                //AuthenticationData cascadeAuth = new AuthenticationData()
                //{
                //    SessionID = sessionID
                //};

                //GetBalanceResponseBody balanceResponse = new ServiceExecutionDelegator<GetBalanceResponseBody, GetBalanceRequestBody>().ResolveRequest(new GetBalanceRequestBody()
                //{
                //    AuthenticationData = cascadeAuth,
                //    DeviceType = request.DeviceType
                //}, ApiTargetPlatform.Kinacu, ApiServiceName.GetBalance);

                //if (response != null)
                //{
                //    response.StockBalance = balanceResponse.StockBalance.Value;
                //    response.WalletBalance = balanceResponse.WalletBalance.Value;
                //    response.PointBalance = balanceResponse.PointsBalance.Value;
                //}


                if (result.Length == 3)
                {
                    response.StockBalance = result[0];
                    response.WalletBalance = result[1];
                    response.PointBalance = result[2];
                }
            }
            else
            {
                AuthenticationData cascadeAuth = new AuthenticationData()
                {
                    SessionID = sessionID
                };

                GetBalanceResponseBody balanceResponse = new ServiceExecutionDelegator<GetBalanceResponseBody, GetBalanceRequestBody>().ResolveRequest(new GetBalanceRequestBody()
                {
                    AuthenticationData = cascadeAuth,
                    DeviceType = request.DeviceType
                }, ApiTargetPlatform.Kinacu, ApiServiceName.GetBalance);

                if (response != null)
                {
                    response.StockBalance = balanceResponse.StockBalance.Value;
                    response.WalletBalance = balanceResponse.WalletBalance.Value;
                    response.PointBalance = balanceResponse.PointsBalance.Value;
                }

            }

            if (response.ResponseCode.Equals(0) && (ConfigurationManager.AppSettings["IncludeTopupMessageExtended"] ?? "").ToLower().Equals("true"))
            {
                // Acá concatenar el mensaje del macroproducto en caso de ser necesario
                response.ResponseMessage = String.Concat(response.ResponseMessage, new MacroProductDataManager().GetMacroProductMessage(int.Parse(ConfigurationManager.AppSettings["CountryID"]), int.Parse(MapMNOtoProductId(request.MNO))));
            }
           
            return (response);
        }



        
         


        protected IMovilwayApiResponse SecondKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            return new TopUpResponseBody()
            {
                ResponseCode = 95,
                ResponseMessage = "ESTA INTENTANDO DUPLICAR UNA MISMA RECARGA",
                TransactionID = 0,
                ExternalTransactionReference = "",
                PointBalance = 0m,
                StockBalance = 0m,
                WalletBalance = 0m
            };
           
        }

        protected override bool ValidateNumberOfExecution(IMovilwayApiRequest request)
        {

            return _secondRequestBandera;//request.DeviceType == cons.ACCESS_H2H || request.DeviceType == cons.ACCESS_POSWEB;

        }


        private bool IsValidId(string sessionID)
        {
            return !string.IsNullOrEmpty(sessionID) && !sessionID.Equals("0");
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

        private string MapMNOtoProductId(string mno)
        {
            string mappings = ConfigurationManager.AppSettings["ProductMappings"];

            foreach (string item in mappings.Split(';'))
                if (item.Split(',')[0] == mno.ToLower())
                    return item.Split(',')[1];

            return mno;
        }
    }


    //clase con la responsabilidad de traer el cache del saldo
  
    public class HandlerCacheSaldo
    {
        private AuthenticationData _cascadeAuth;
        private TopUpRequestBody _request;
        public HandlerCacheSaldo()
        {

        }

        public HandlerCacheSaldo(AuthenticationData cascadeAuth, TopUpRequestBody request)
        {
            _cascadeAuth = cascadeAuth;
            _request = request;
        }

        public decimal[] HandlerCache()
        {
            List<decimal> lista = new List<decimal>(3);

            GetBalanceResponseBody balanceResponse = new ServiceExecutionDelegator<GetBalanceResponseBody, GetBalanceRequestBody>().ResolveRequest(new GetBalanceRequestBody()
            {
                AuthenticationData = _cascadeAuth,
                DeviceType = _request.DeviceType
            }, ApiTargetPlatform.Kinacu, ApiServiceName.GetBalance);


            lista.Add(balanceResponse.StockBalance.Value);
            lista.Add(balanceResponse.WalletBalance.Value);
            lista.Add(balanceResponse.PointsBalance.Value);
            return lista.ToArray<decimal>();
        } 

        public decimal[] HandlerCache(AuthenticationData cascadeAuth, TopUpRequestBody request)
        {
            //se inicializa con valores por defecto
            List<decimal> lista = new List<decimal>(3) { 0m,0m,0m};

            GetBalanceResponseBody balanceResponse = new ServiceExecutionDelegator<GetBalanceResponseBody, GetBalanceRequestBody>().ResolveRequest(new GetBalanceRequestBody()
            {
                AuthenticationData = cascadeAuth,
                DeviceType = request.DeviceType
            }, ApiTargetPlatform.Kinacu, ApiServiceName.GetBalance);

            if (balanceResponse.StockBalance.HasValue)
                lista[0]= balanceResponse.StockBalance.Value;
          
            
            if (balanceResponse.WalletBalance.HasValue)
                lista[1]=  balanceResponse.WalletBalance.Value;
            

            if (balanceResponse.PointsBalance.HasValue)
                lista[2]= balanceResponse.PointsBalance.Value;
            

            return lista.ToArray<decimal>();
        } 
    }
}