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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetAgentClosings)]
    public class GetAgentClosingsProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetAgentClosingsProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            GetAgentClosingsResponseBody response = new GetAgentClosingsResponseBody()
            {
                TransactionID = 0,
                ResponseMessage = "",
                ResponseCode = 99,
             
            };

            GetAgentClosingsRequestBody request = requestObject as GetAgentClosingsRequestBody;
            //Ariel 2021-Ma-09 Comentado 
           // response = Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetAgentClosings(request);

            response.ResponseCode = 0;
            response.ResponseMessage = "OK";


            return response;
        }
    }

}