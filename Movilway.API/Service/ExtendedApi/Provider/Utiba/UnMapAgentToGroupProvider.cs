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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.UnMapAgentToGroup)]
    public class UnMapAgentToGroupProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(UnMapAgentToGroupProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            UnMapAgentToGroupRequestBody request = requestObject as UnMapAgentToGroupRequestBody;
            UnMapAgentToGroupResponseBody response = null;

            unmapAgentResponse utibaUnMapAgentResponse = utibaClientProxy.unmapAgent(new unmapAgentRequest()
            {
                unmapAgentRequestType = new unmapAgentRequestType()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType,
                    agid = request.GroupID,
                    agent = request.Agent
                }
            });
            if (utibaUnMapAgentResponse != null)
            {
                response = new UnMapAgentToGroupResponseBody()
                    {
                        ResponseCode = Utils.BuildResponseCode(utibaUnMapAgentResponse.unmapAgentReturn.result, utibaUnMapAgentResponse.unmapAgentReturn.result_namespace),
                        ResponseMessage = utibaUnMapAgentResponse.unmapAgentReturn.result_message,
                        TransactionID = utibaUnMapAgentResponse.unmapAgentReturn.transid
                    };
            }
            return (response);
        }
    }
}