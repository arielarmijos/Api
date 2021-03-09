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
   [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.GetAgentGroups)]
    public class GetAgentGroupsProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetAgentGroupsProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            GetAgentGroupsRequestBody request = requestObject as GetAgentGroupsRequestBody;
            GetAgentGroupsResponseBody response = null;

            GetAgentInfoResponseBody agentInfo = new ServiceExecutionDelegator<GetAgentInfoResponseBody, GetAgentInfoRequestBody>().ResolveRequest(
                new GetAgentInfoRequestBody()
                {
                    AuthenticationData = new AuthenticationData()
                    {
                        SessionID = sessionID
                    },
                    Agent = request.Agent,
                    DeviceType = request.DeviceType
                }, ApiTargetPlatform.Utiba, ApiServiceName.GetAgentInfo);

            AgentGroupsResponse utibaAgentGroupsResponse = utibaClientProxy.getAgentGroupByAgentID(new getAgentGroupByAgentID()
            {
                getAgentGroupByAgentIDRequest = new getAgentGroupByAgentIDRequest()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType,
                    agentID = agentInfo.AgentInfo.AgentID
                }
            });
            if (utibaAgentGroupsResponse != null)
            {
                response = new GetAgentGroupsResponseBody()
                {
                    ResponseCode = Utils.BuildResponseCode(utibaAgentGroupsResponse.AgentGroupsReturn.result, utibaAgentGroupsResponse.AgentGroupsReturn.result_namespace),
                    ResponseMessage = utibaAgentGroupsResponse.AgentGroupsReturn.result_namespace,
                    TransactionID = utibaAgentGroupsResponse.AgentGroupsReturn.transid
                };

                if (utibaAgentGroupsResponse.AgentGroupsReturn.agentGroups != null &&
                    utibaAgentGroupsResponse.AgentGroupsReturn.agentGroups.Length > 0)
                {
                    response.GroupList = new GroupList();
                    foreach (AgentGroup agentGroup in utibaAgentGroupsResponse.AgentGroupsReturn.agentGroups)
                    {
                        response.GroupList.Add(new GroupInfo()
                        {
                            GroupID = agentGroup.ID,
                            Name = agentGroup.name,
                            Category = agentGroup.category,
                            Type = agentGroup.type
                        });
                    }
                }
            }
            return (response);
        }
    }
}