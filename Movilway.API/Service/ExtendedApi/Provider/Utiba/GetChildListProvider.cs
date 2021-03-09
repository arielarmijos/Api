using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Utiba;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.GetChildList)]
    public class GetChildListProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetChildListProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {

            GetChildListRequestBody request = requestObject as GetChildListRequestBody;
            GetChildListResponseBody response = null;

            logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetChildListProvider] [SEND-DATA] getChildListByReferenceRequest {sessionid=" + sessionID + ",device_type=" + request.DeviceType + ",agentReference=" + request.Agent + "}");

            getChildListByReferenceResponse utibaGetChildListResponse = utibaClientProxy.getChildListByReference(new getChildListByReferenceRequest()
            {
                getChildListByReferenceRequestType = new getChildListByReferenceRequestType()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType,
                    agentReference = request.Agent
                }
            });

            StringBuilder sb = new StringBuilder();
            foreach (var agent in utibaGetChildListResponse.getChildListByReferenceResponseType.agentList)
                sb.Append("agentList={ID=" + agent.ID + ",referenceID=" + agent.referenceID + ",agentID=" + agent.agentID + ",ownerID=" + agent.ownerID + ",name=" + agent.name + ",MSISDN=" + agent.MSISDN + 
                    ",reference=" + agent.reference + ",entityReference={reference=" + agent.entityReference.reference + "}" + ",salt=" + agent.salt + ",upstream=" + agent.upstream + ",status=" + agent.status + 
                    ",agentType=" + agent.agentType +  ",primaryGroup=" + agent.primaryGroup + ",depth=" + agent.depth + ",organisation=0,DSComission=0,POSComission=0,category=agent},");
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);

            logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetChildListProvider] [RECV-DATA] getChildListByReferenceResponse {transid=" + utibaGetChildListResponse.getChildListByReferenceResponseType.transid + 
                    ",result=" + utibaGetChildListResponse.getChildListByReferenceResponseType.result + ",result_namespace=" + utibaGetChildListResponse.getChildListByReferenceResponseType.result_message + 
                    "," + sb.ToString() + "}");


            if (utibaGetChildListResponse != null)
            {
                response = new GetChildListResponseBody()
                    {
                        ResponseCode = Utils.BuildResponseCode(utibaGetChildListResponse.getChildListByReferenceResponseType.result, utibaGetChildListResponse.getChildListByReferenceResponseType.result_namespace),
                        ResponseMessage = utibaGetChildListResponse.getChildListByReferenceResponseType.result_message,
                        TransactionID = utibaGetChildListResponse.getChildListByReferenceResponseType.transid
                    };

                if (utibaGetChildListResponse.getChildListByReferenceResponseType.agentList != null &&
                    utibaGetChildListResponse.getChildListByReferenceResponseType.agentList.Length > 0)
                {

                    response.ChildList = new ChildList();

                    foreach (Agent agent in utibaGetChildListResponse.getChildListByReferenceResponseType.agentList)
                    {
                        response.ChildList.Add(new BasicAgentInfo()
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