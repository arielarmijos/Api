using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using Movilway.API.KinacuWebService;
using Movilway.API.KinacuManagementWebService;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetAgentInfo)]
    public class GetAgentInfoProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetAgentInfoProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new GetAgentInfoResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0,
                    AgentInfo = new AgentInfo()
                };

            GetAgentInfoRequestBody request = requestObject as GetAgentInfoRequestBody;
            GetAgentInfoResponseBody response = null;

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[GetAgentInfoProvider] [SEND-DATA] getAgentInfoParameters {agentReference=" + request.Agent + "}");

            AgentInfo agentInfo = new AgentInfo();
            string validatedEmail = "";

            if (request.SearchById ?? false)
            {
                agentInfo = Utils.GetAgentInfoById(request.AgentId);
                validatedEmail = Utils.GetValidatedEmailById(request.AgentId);
            }
            else
            {
                agentInfo = Utils.GetAgentInfo(request.Agent);
                validatedEmail = Utils.GetValidatedEmailByLogin(request.Agent);
            }

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[GetAgentInfoProvider] [RECV-DATA] getAgentInfoResult {response={" + DataContract.Utils.logFormat(agentInfo) + "}}");

            if (agentInfo.BranchID >= 0) //userInfo && retailerInfo)
            {
                response = new GetAgentInfoResponseBody()
                {
                    ResponseCode = 0,
                    ResponseMessage = "exito",
                    TransactionID = 0,
                    AgentInfo = new AgentInfo()
                    {
                        Agent = request.Agent,
                        NationalIDType = agentInfo.NationalIDType,
                        NationalID = agentInfo.NationalID,
                        Address = agentInfo.Address,
                        Name = agentInfo.Name,
                        LegalName = agentInfo.LegalName,
                        Email = String.IsNullOrEmpty(validatedEmail) ? agentInfo.Email : validatedEmail,

                        // Elementos privados
                        AgentID = agentInfo.AgentID,
                        OwnerID = agentInfo.OwnerID, 
                        BranchID = agentInfo.BranchID,

                        // Grupo de comisiones nuevo
                        CommissionGroups = agentInfo.CommissionGroups,

                        // Nuevo subniveles
                        SubLevel = agentInfo.SubLevel,

                        // Nuevo PDV Id
                        PDVID = agentInfo.PDVID,

                        PhoneNumber = agentInfo.PhoneNumber,

                        TaxCategory = agentInfo.TaxCategory,
                        TaxCategories = agentInfo.TaxCategories,

                        SegmentId = agentInfo.SegmentId,
                        SegmentList = agentInfo.SegmentList
                    }
                };
            }
            else
            {
                response = new GetAgentInfoResponseBody
                {
                    ResponseCode = 99,
                    ResponseMessage = "El agente consultado no existe"
                };
            }

            return (response);
        }
    }
}
