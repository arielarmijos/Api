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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetAgentClosingDetails)]
    public class GetAgentClosingDetailsProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetAgentClosingDetailsProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            GetAgentClosingDetailsResponseBody response = new GetAgentClosingDetailsResponseBody()
            {
                ResponseCode = 99,
                ResponseMessage = "",
                TransactionID = 0,
                //PDV
                AgentID = "",
                //NOMBRE
                AgentName = "",
               
            };


            GetAgentClosingDetailsRequestBody request = requestObject as GetAgentClosingDetailsRequestBody;

            logger.BeginHigh("[KIN] " + base.LOG_PREFIX + "[GetAgentClosingDetailsProvider] [SEND-DATA]  {UserId=" + sessionID + "} Agent =" + request.Agent + ", IdMax= " + request.IdMax + " ,IdMin = " + request.IdMin );

            //Ariel 2021-Ma-09 Comentado 
            //response = Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetAgentClosingDetails(request);

            response.ResponseCode = 0;
            response.ResponseMessage = "OK";



            return response;
        }
    }
}