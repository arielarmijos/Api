using Movilway.API.KinacuWebService;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetAgentDistributionList)]
    public class GetAgentDistributionListProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetChildListProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new GetAgentDistributionListResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0,
                    AgentDistributionList = new AgentDistributionList()
                };

            GetAgentDistributionListRequestBody request = requestObject as GetAgentDistributionListRequestBody;
            GetAgentDistributionListResponseBody response = null;

            logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetAgentDistributionListProvider] [SEND-DATA] getChildRetailersParameters {UserId=" + sessionID + "} Agent =" + request.Agent + ", AgentChild= " + request.AgentChild + " ,CutInfo = " + request.CutInfo);

            //Ariel 2021-Ma-09 Comentado  asigmanos null
            AgentDistributionList distributionList = null; // Utils.GetAgentDistributionList(request.Agent, request.AgentChild, request.CutInfo);

            response = new GetAgentDistributionListResponseBody()
            {
                ResponseCode = (distributionList.Count > 0 ? 0 : 99),
                ResponseMessage = (distributionList.Count > 0 ? "exito" : "No se encontraron agencias"),
                TransactionID = new Random().Next(100000,999999),
                AgentDistributionList = distributionList
            };

            return (response);
        }
    }
}