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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.GetParentList)]
    public class GetParentListProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetParentListProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            GetParentListRequestBody request = requestObject as GetParentListRequestBody;
            GetParentListResponseBody response = null;

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

            getParentListByReferenceIDResponse utibaGetParentListResponse = utibaClientProxy.getParentListByReferenceID(new getParentListByReferenceIDRequest()
            {
                getParentListByReferenceIDRequestType = new getParentListByReferenceIDRequestType()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType,
                    agentReferenceID = agentInfo.AgentInfo.ReferenceID
                }
            });

            //logger.InfoLow("Ajá: " + request.Agent + " vs " + agentInfo.AgentInfo.ReferenceID);

            if (utibaGetParentListResponse != null)
            {

                response = new GetParentListResponseBody()
                    {
                        ResponseCode = Utils.BuildResponseCode(utibaGetParentListResponse.getParentListByReferenceIDResponseType.result, utibaGetParentListResponse.getParentListByReferenceIDResponseType.result_namespace),
                        ResponseMessage = utibaGetParentListResponse.getParentListByReferenceIDResponseType.result_message,
                        TransactionID = utibaGetParentListResponse.getParentListByReferenceIDResponseType.transid
                    };


                if (utibaGetParentListResponse.getParentListByReferenceIDResponseType.agentList != null &&
                    utibaGetParentListResponse.getParentListByReferenceIDResponseType.agentList.Length > 0)
                {

                    response.ParentList = new ParentList();

                    foreach (Agent agent in utibaGetParentListResponse.getParentListByReferenceIDResponseType.agentList)
                    {
                        response.ParentList.Add(new BasicAgentInfo()
                        {
                            Agent = agent.reference,
                            Name = agent.name
                        });
                    }
                }
            }
            
            return (response);
        }
    }
}