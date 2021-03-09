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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.TransferStock)]
    public class TransferStockProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(TransferStockProvider));
        protected override ILogger ProviderLogger { get { return logger; } }
        protected override TransactionType TransactionType { get { return TransactionType.transferstock; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            TransferStockRequestBody request = requestObject as TransferStockRequestBody;
            TransferStockResponseBody response = null;

            transferStockResponse utibaTransferStockResponse = utibaClientProxy.transferStock(new transferStock()
            {
                transferStockRequest = new transferStockRequestType()
                {
                    sessionid = sessionID,
                    amount = request.Amount.ToString(),
                    device_type = request.DeviceType,
                    to = request.Agent
                }
            });

            if (utibaTransferStockResponse != null)
            {
                response = new TransferStockResponseBody()
                    {
                        ResponseCode = Utils.BuildResponseCode(utibaTransferStockResponse.transferStockReturn.result, utibaTransferStockResponse.transferStockReturn.result_namespace),
                        ResponseMessage = utibaTransferStockResponse.transferStockReturn.result_message,
                        Fee = utibaTransferStockResponse.transferStockReturn.fee,
                        TransactionID = utibaTransferStockResponse.transferStockReturn.transid
                    };
            }
            return (response);
        }
    }
}