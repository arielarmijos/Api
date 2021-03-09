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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.GetGroupList)]
    public class GetGroupListProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetGroupListProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            GetGroupListRequestBody request = requestObject as GetGroupListRequestBody;
            GetGroupListResponseBody response = null;

            AgentGroupsResponse utibaGetAllAgentGroupsResponse = utibaClientProxy.getAllAgentGroups(new getAllAgentGroups()
            {
                getAllAgentGroupsRequest = new getAllGroupsRequestType()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType,
                    filter = new getAllGroupsRequestTypeFilter()
                    {
                        category = "agent",
                        includeUncategorised = true
                    }
                }
            });
            if (utibaGetAllAgentGroupsResponse != null)
            {
                response = new GetGroupListResponseBody()
                    {
                        ResponseCode = Utils.BuildResponseCode(utibaGetAllAgentGroupsResponse.AgentGroupsReturn.result, utibaGetAllAgentGroupsResponse.AgentGroupsReturn.result_namespace),
                        ResponseMessage = utibaGetAllAgentGroupsResponse.AgentGroupsReturn.result_namespace,
                        TransactionID = utibaGetAllAgentGroupsResponse.AgentGroupsReturn.transid
                };

                if (utibaGetAllAgentGroupsResponse.AgentGroupsReturn.agentGroups != null &&
                    utibaGetAllAgentGroupsResponse.AgentGroupsReturn.agentGroups.Length > 0)
                {
                    response.GroupList = new GroupList();
                    foreach (AgentGroup agentGroup in utibaGetAllAgentGroupsResponse.AgentGroupsReturn.agentGroups)
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