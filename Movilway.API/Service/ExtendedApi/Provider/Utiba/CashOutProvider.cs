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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.CashOut)]
    public class CashOutProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(CashOutProvider));
        protected override ILogger ProviderLogger { get { return logger; } }
        protected override TransactionType TransactionType { get { return TransactionType.cashout; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            CashOutRequestBody request = requestObject as CashOutRequestBody;
            CashOutResponseBody response = null;

            // 5 minutos de timeout para la operacion
            utibaClientProxy.InnerChannel.OperationTimeout = new TimeSpan(0, 5, 0);

            cashoutResponse utibaCashOutResponse = utibaClientProxy.cashout(new cashout()
            {
                cashoutRequest = new cashoutRequestType()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType,
                    amount = request.Amount,
                    to = request.Agent
                }
            });
            if (utibaCashOutResponse != null)
            {
                response = new CashOutResponseBody()
                {
                    ResponseCode = Utils.BuildResponseCode(utibaCashOutResponse.cashoutReturn.result, utibaCashOutResponse.cashoutReturn.result_namespace),
                    ResponseMessage = utibaCashOutResponse.cashoutReturn.result_message,
                    TransactionID = utibaCashOutResponse.cashoutReturn.transid,
                    Fee = utibaCashOutResponse.cashoutReturn.fee
                };
            }
            return (response);
        }
    }
}