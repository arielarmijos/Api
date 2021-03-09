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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.GetAgentInfo)]
    public class GetAgentInfoProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetAgentInfoProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            GetAgentInfoRequestBody request = requestObject as GetAgentInfoRequestBody;
            GetAgentInfoResponseBody response = null;

            logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetAgentInfoProvider] [SEND-DATA] getAgentByReferenceRequest {sessionid=" + sessionID + ",device_type=" + request.DeviceType + ",reference=" + request.Agent + ",category=agent}");

            AgentResponse utibaGetAgentResponse = utibaClientProxy.getAgentByReference(new getAgentByReference()
            {
                getAgentByReferenceRequest = new getAgentByReferenceRequest()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType,
                    reference = request.Agent,
                    category = "agent"
                }
            });

            StringBuilder sb = new StringBuilder();
            foreach (var agentData in utibaGetAgentResponse.AgentReturn.agent.agentData)
                sb.Append("agentData={key=" + agentData.key + ",value=" + agentData.value + "},");
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);

            logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetAgentInfoProvider] [RECV-DATA] agentResponse {transid=" + utibaGetAgentResponse.AgentReturn.transid + ",result=" + utibaGetAgentResponse.AgentReturn.result + 
                    ",result_namespace=" + utibaGetAgentResponse.AgentReturn.result_namespace + ",agent={ID=" + utibaGetAgentResponse.AgentReturn.agent.ID + ",referenceID=" + utibaGetAgentResponse.AgentReturn.agent.referenceID +
                    ",agentID=" + utibaGetAgentResponse.AgentReturn.agent.agentID + ",ownerID=" + utibaGetAgentResponse.AgentReturn.agent.ownerID + ",name=" + utibaGetAgentResponse.AgentReturn.agent.name + 
                    ",MSISDN=" + utibaGetAgentResponse.AgentReturn.agent.MSISDN + ",reference=" + utibaGetAgentResponse.AgentReturn.agent.reference + ",entityReference={reference=" + utibaGetAgentResponse.AgentReturn.agent.entityReference.reference + "}" +
                    ",salt=" + utibaGetAgentResponse.AgentReturn.agent.salt + ",emailAddress=" + utibaGetAgentResponse.AgentReturn.agent.emailAddress + ",SMSAddress=" + utibaGetAgentResponse.AgentReturn.agent.SMSAddress + 
                    ",Language=" + utibaGetAgentResponse.AgentReturn.agent.Language + ",upstream=" + utibaGetAgentResponse.AgentReturn.agent.upstream + ",status=" + utibaGetAgentResponse.AgentReturn.agent.status +
                    ",agentType=" + utibaGetAgentResponse.AgentReturn.agent.agentType + ",primaryGroup=" + utibaGetAgentResponse.AgentReturn.agent.primaryGroup + ",depth=" + utibaGetAgentResponse.AgentReturn.agent.depth + "," + sb.ToString() + 
                    ",createdDate=" + utibaGetAgentResponse.AgentReturn.agent.createdDate + ",organisation=" + utibaGetAgentResponse.AgentReturn.agent.organisation + ",DSComission=" + utibaGetAgentResponse.AgentReturn.agent.DSComission +
                    ",POSComission=" + utibaGetAgentResponse.AgentReturn.agent.POSComission + ",category=" + utibaGetAgentResponse.AgentReturn.agent.category + "}}");

            Func<String, KeyValuePair[], String> findItem = (k, a) => 
                {
                    if (a != null)
                    {
                        var node = a.FirstOrDefault(kvp => kvp.key.Equals(k));
                        if (node != null)
                            return node.value;
                    }
                    return null;
                };


            if (utibaGetAgentResponse != null && utibaGetAgentResponse.AgentReturn.result == 0)
            {
                response = new GetAgentInfoResponseBody()
                {
                    ResponseCode = Utils.BuildResponseCode(utibaGetAgentResponse.AgentReturn.result, utibaGetAgentResponse.AgentReturn.result_namespace),
                    ResponseMessage = utibaGetAgentResponse.AgentReturn.result_namespace,
                    TransactionID = utibaGetAgentResponse.AgentReturn.transid,
                    AgentInfo = new AgentInfo()
                    {
                        Agent = request.Agent,
                        NationalIDType = findItem("nat_id_type", utibaGetAgentResponse.AgentReturn.agent.agentData),
                        NationalID = findItem("nat_id", utibaGetAgentResponse.AgentReturn.agent.agentData),
                        Address = utibaGetAgentResponse.AgentReturn.agent.address ?? findItem("address", utibaGetAgentResponse.AgentReturn.agent.agentData),
                        Email = utibaGetAgentResponse.AgentReturn.agent.emailAddress,
                        Name = utibaGetAgentResponse.AgentReturn.agent.name,
                        Depth = utibaGetAgentResponse.AgentReturn.agent.depth,
                        BirthDate = findItem("dob", utibaGetAgentResponse.AgentReturn.agent.agentData),
                        Gender = findItem("gender", utibaGetAgentResponse.AgentReturn.agent.agentData),
                        LegalName = findItem("legal_name", utibaGetAgentResponse.AgentReturn.agent.agentData),

                        //Elementos privados
                        AgentID = utibaGetAgentResponse.AgentReturn.agent.agentID,
                        ReferenceID = utibaGetAgentResponse.AgentReturn.agent.referenceID,
                        OwnerID = utibaGetAgentResponse.AgentReturn.agent.ownerID,
                        BranchID = utibaGetAgentResponse.AgentReturn.agent.agentID
                    }
                };
            }
            else
            {
                response = new GetAgentInfoResponseBody
                {
                    ResponseCode=99,
                    ResponseMessage="Ocurrio un problema procesando su solicitud"
                };
            }
            return (response);
        }
    }
}