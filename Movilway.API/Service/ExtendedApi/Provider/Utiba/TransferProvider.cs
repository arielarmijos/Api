using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Utiba;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Data;
using Movilway.Logging;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.Transfer)]
    public class TransferProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(TransferProvider));
        protected override ILogger ProviderLogger { get { return logger; } }
        protected override TransactionType TransactionType { get { return TransactionType.transfer;} }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            TransferRequestBody request = requestObject as TransferRequestBody;
            TransferResponseBody response = null;

            logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[TransferProvider] [SEND-DATA] transferRequest {sessionid=" + sessionID + ",suppress_confirm=true,device_type=" + request.DeviceType + 
                                                                                                ",to=" + request.Recipient + ",amount=" + request.Amount + ",type=" + request.WalletType + "}");

            transferResponse utibaTransferResponse = utibaClientProxy.transfer(new transfer()
            {
                transferRequest = new transferRequestType()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType,
                    amount = request.Amount,
                    to = request.Recipient,
                    suppress_confirm = true,
                    suppress_confirmSpecified = true,
                    type = (int)request.WalletType,
                    typeSpecified = true
                }
            });

            logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[TransferProvider] [RECV-DATA] transferResponse {transid=" + utibaTransferResponse.transferReturn.transid +
                    ",result=" + utibaTransferResponse.transferReturn.result + ",result_namespace=" + utibaTransferResponse.transferReturn.result_namespace +
                    ",result_message=" + utibaTransferResponse.transferReturn.result_message + ",fee=" + utibaTransferResponse.transferReturn.fee +
                    ",trans_ext_reference=" + utibaTransferResponse.transferReturn.trans_ext_reference + ",schedule_id=" + utibaTransferResponse.transferReturn.schedule_id + "}");

            if (utibaTransferResponse != null)
            {
                response = new TransferResponseBody()
                {
                    ResponseCode = Utils.BuildResponseCode(utibaTransferResponse.transferReturn.result, utibaTransferResponse.transferReturn.result_namespace),
                    ResponseMessage = utibaTransferResponse.transferReturn.result_message,
                    Fee = utibaTransferResponse.transferReturn.fee,
                    TransactionID = utibaTransferResponse.transferReturn.transid
                };
            }
            return (response);
        }
    }
}