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
    [ServiceProviderImpl(Platform=ApiTargetPlatform.Utiba, ServiceName=ApiServiceName.RegisterDebtPayment)]
    public class RegisterDebtPayment : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(RegisterDebtPayment));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, API.Utiba.UMarketSCClient utibaClientProxy, string sessionID)
        {
            RegisterDebtPaymentRequestBody request = requestObject as RegisterDebtPaymentRequestBody;
            RegisterDebtPaymentResponseBody response = null;

            accountPaymentResponse utibaAccountPaymentResponse = utibaClientProxy.accountPayment(new accountPayment()
            {
                accountPaymentRequest = new accountPaymentRequestType()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType,
                    amount = request.Amount.ToString(),
                    to = request.Agent,
                    details = request.Description
                }
            });
            response = new RegisterDebtPaymentResponseBody()
            {
                ResponseCode = Utils.BuildResponseCode(utibaAccountPaymentResponse.accountPaymentReturn.result, utibaAccountPaymentResponse.accountPaymentReturn.result_namespace),
                ResponseMessage = utibaAccountPaymentResponse.accountPaymentReturn.result_message,
                TransactionID = utibaAccountPaymentResponse.accountPaymentReturn.transid
            };
            return (response);
        }
    }
}