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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetNewAgentsReport)]
    public class GetNewAgentsReportProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetNewAgentsReportProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            GetNewAgentsReportResponseBody response = new GetNewAgentsReportResponseBody()
            {
                ResponseCode = 99,
                ResponseMessage = "",
                TransactionID = 0
            };

            GetNewAgentsReportRequestBody request = requestObject as GetNewAgentsReportRequestBody;

            //Ariel 2021-Mar-09
            //response = Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetNewAgentsReport(request);
            response.ResponseCode = 0;
            response.ResponseMessage = "OK";

            return response;
        }
    }
}