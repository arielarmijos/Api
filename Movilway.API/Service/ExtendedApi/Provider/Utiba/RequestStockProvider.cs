using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Utiba;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.Logging;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.RequestStock)]
    public class RequestStockProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(RequestStockProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            RequestStockRequestBody request = requestObject as RequestStockRequestBody;
            RequestStockResponseBody response =null;
            //SellRequestBody request = requestObject as SellRequestBody;
            //SellResponseBody response = null;
            
            sellResponse utibaSellResponse = utibaClientProxy.sell(new sell()
            {
                sellRequest = new sellRequestType()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType,
                    amount = request.Amount,
                    to = request.To,
                    wait = false, // Do not wait for the toAgent sms confirmation.
                    waitSpecified = true,
                    type = (int)WalletType.Stock, // By default, request from the stock wallet.
                    typeSpecified = true
                }
            });
            if (utibaSellResponse != null)
            {
                //response = new SellResponseBody()
                response = new RequestStockResponseBody()
                {
                    ResponseCode = Utils.BuildResponseCode(utibaSellResponse.sellReturn.result, utibaSellResponse.sellReturn.result_namespace),
                    ResponseMessage = utibaSellResponse.sellReturn.result_message,
                    TransactionID = utibaSellResponse.sellReturn.transid
                };
            }
            return (response);
        }
    }
}