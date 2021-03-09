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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.CashIn)]
    public class CashInProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(CashInProvider));
        protected override ILogger ProviderLogger { get { return logger; } }
        protected override TransactionType TransactionType { get { return TransactionType.cashin; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            CashInRequestBody request = requestObject as CashInRequestBody;
            CashInResponseBody response = null;

            cashinResponse utibaCashInResponse = utibaClientProxy.cashin(new cashin()
            {
                cashinRequest = new cashinRequestType()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType,
                    amount = request.Amount,
                    to = request.Agent,
                    suppress_confirm = true,
                    suppress_confirmSpecified = true
                }
            });
            if (utibaCashInResponse != null)
            {
                response = new CashInResponseBody()
                    {
                        ResponseCode = Utils.BuildResponseCode(utibaCashInResponse.cashinReturn.result, utibaCashInResponse.cashinReturn.result_namespace),
                        ResponseMessage = utibaCashInResponse.cashinReturn.result_message,
                        TransactionID = utibaCashInResponse.cashinReturn.transid
                    };
            }

            return (response);
        }
    }
}