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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.SetAgentClose)]
    public class SetAgentCloseProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(SetAgentCloseProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            SetAgentCloseRequestBody request = requestObject as SetAgentCloseRequestBody;

            SetAgentCloseResponseBody response = new SetAgentCloseResponseBody()
            {
                ResponseCode = 99,
                ResponseMessage = "OK",
                TransactionID = 0
            };

            logger.BeginHigh("[KIN] " + base.LOG_PREFIX + "[SetAgentCloseProvider] [SEND-DATA] getChildRetailersParameters {UserId=" + sessionID + "} Agent =" + request.Agent + ", IdMax= " + request.IdMax + " ,IdMin = " + request.IdMin + " ,Type = " + request.Type);

            //Ariel 2021-Ma-09 Comentado  asignamos true
            bool result = true; // Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.SetAgentCloseProvider(request);


            response.ResponseCode = result ? 0 : 99;
            response.ResponseMessage = result ? "OK" : "";



            return response;
        }
    }
}