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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.Sell)]
    public class SellProvider:AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(SellProvider));
        protected override ILogger ProviderLogger { get { return logger; } }
        protected override TransactionType TransactionType { get { return TransactionType.sell; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            SellRequestBody request = requestObject as SellRequestBody;
            SellResponseBody response = null;

            // 5 minutos de timeout para la operacion
            utibaClientProxy.InnerChannel.OperationTimeout = new TimeSpan(0, 5, 0);

            sellResponse utibaSellResponse = utibaClientProxy.sell(new sell()
            {
                sellRequest = new sellRequestType()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType,
                    amount = request.Amount,
                    to = request.Agent,
                    wait = true,
                    waitSpecified = true,
                    type = 1, // Wallet Type = eWallet para el valor 1. 
                    typeSpecified = true,
                    extra_trans_data = new KeyValuePair[]
                                        {
                                            new KeyValuePair() { key = "host_trans_ref", value = request.ExternalTransactionReference }
                                        }
                }
            });
            if (utibaSellResponse != null)
            {
                response = new SellResponseBody()
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