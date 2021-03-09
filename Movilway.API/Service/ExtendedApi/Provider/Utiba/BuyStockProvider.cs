using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Utiba;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.BuyStock)]
    public class BuyStockProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(BuyStockProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            BuyStockRequestBody request = requestObject as BuyStockRequestBody;
            BuyStockResponseBody response = null;

            buyStockResponse utibaBuyStockResponse = utibaClientProxy.buyStock(new buyStock()
            {
                buyStockRequest = new buyStockRequestType()
                {
                    sessionid = sessionID,
                    wait = false,
                    waitSpecified = true,
                    amount = request.Amount,
                    device_type = request.DeviceType,
                    details = ("Bank: " + request.BankName ?? "NULL") + " - Fecha: " + (request.TransactionDate.ToShortDateString() ?? "NULL") + " - REF.: " + (request.TransactionReference ?? "NULL")
                }
            });

            if (utibaBuyStockResponse != null)
            {
                response = new BuyStockResponseBody()
                    {

                        ResponseCode = Utils.BuildResponseCode(utibaBuyStockResponse.buyStockReturn.result, utibaBuyStockResponse.buyStockReturn.result_namespace),
                        ResponseMessage = utibaBuyStockResponse.buyStockReturn.result_message,
                        Fee = utibaBuyStockResponse.buyStockReturn.fee,
                        TransactionID = utibaBuyStockResponse.buyStockReturn.transid
                    };
            }
            return (response);
        }
    }
}