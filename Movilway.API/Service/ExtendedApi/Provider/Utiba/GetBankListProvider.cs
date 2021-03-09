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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.GetBankList)]
    public class GetBankListProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetBankListProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            GetBankListRequestBody request = requestObject as GetBankListRequestBody;
            GetBankListResponseBody response = null;

            getBankListResponse utibaGetBankListResponse = utibaClientProxy.getBankList(new getBankList()
            {
                getBankListRequest = new getBankListRequestType()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType,
                    agent_reference = request.Agent
                }
            });
            if (utibaGetBankListResponse != null)
            {
                response = new GetBankListResponseBody()
                    {
                        ResponseCode = Utils.BuildResponseCode(utibaGetBankListResponse.getBankListReturn.result, utibaGetBankListResponse.getBankListReturn.result_namespace),
                        ResponseMessage = utibaGetBankListResponse.getBankListReturn.result_message,
                        TransactionID = utibaGetBankListResponse.getBankListReturn.transid
                    };

                if (utibaGetBankListResponse.getBankListReturn.banks.Length > 0)
                {
                    response.BankList = new BankList();
                    foreach (KeyValuePair1 kvp in utibaGetBankListResponse.getBankListReturn.banks)
                    {
                        response.BankList.Add(int.Parse(kvp.key), kvp.value);
                    }
                }
            }
            return (response);
        }
    }
}