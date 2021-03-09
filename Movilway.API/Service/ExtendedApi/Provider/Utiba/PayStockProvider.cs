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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.PayStock)]
    public class PayStockProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(PayStockProvider));
        protected override ILogger ProviderLogger { get { return logger; } }
        protected override TransactionType TransactionType { get { return TransactionType.paystock; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            PayStockRequestBody request = requestObject as PayStockRequestBody;
            PayStockResponseBody response = null;

            payStockResponse utibaPayStockResponse = utibaClientProxy.payStock(new payStock()
            {
                payStockRequest = new payStockRequestType()
                {
                    sessionid=sessionID,
                    wait=false,
                    waitSpecified=true,
                    device_type=request.DeviceType,
                    amount=request.Amount,
                    details = ("Bank: " + request.BankName ?? "NULL") + " - Description: " + (request.Description ?? "NULL") + " - REF.: " + (request.TransactionReference ?? "NULL")
                }
            });

            if (utibaPayStockResponse != null)
            {
                response = new PayStockResponseBody()
                    {

                        ResponseCode = Utils.BuildResponseCode(utibaPayStockResponse.payStockReturn.result, utibaPayStockResponse.payStockReturn.result_namespace),
                        ResponseMessage = utibaPayStockResponse.payStockReturn.result_message,
                        Fee = utibaPayStockResponse.payStockReturn.fee,
                        TransactionID = utibaPayStockResponse.payStockReturn.transid
                    };
            }
            return (response);
        }
    }
}