using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using Movilway.API.KinacuWebService;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetDistributionsMadeByAgent)]
    public class GetDistributionsMadeByAgentProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetDistributionsMadeByAgentProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            string providerName = "GetDistributionsMadeByAgentProvider";

            if (sessionID.Equals("0"))
            {
                return new GetDistributionsMadeByAgentResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0,
                    Distributions = new DistributionMadeList()
                };
            }

            GetDistributionsMadeByAgentRequestBody request = requestObject as GetDistributionsMadeByAgentRequestBody;
            GetDistributionsMadeByAgentResponseBody response = null;

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[" + providerName + "] [SEND-DATA] GetDistributionsMadeByAgentParameters {LowerLimit=" + request.LowerLimit.ToString("yyyy-MM-dd") + ",UpperLimit=" + request.UpperLimit.ToString("yyyy-MM-dd") + ",DistributionTypeId=" + request.DistributionTypeId + "}");

            //int distributionTypeId = request.DistributionTypeId != 0 ? request.DistributionTypeId : 201;
            int[] distributionTypeId = request.DistributionTypeId.Count()> 0 ? request.DistributionTypeId :new int[]{ 201};
            int countryId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["CountryID"]);
            int platformId = 1;
            try
            {
                platformId = int.Parse(request.Platform ?? System.Configuration.ConfigurationManager.AppSettings["DefaultPlatform"]);
            }
            catch (Exception)
            {
            }

            DistributionMadeList dists = Utils.GetDistributionsMadeByAgent(countryId, platformId, request.AuthenticationData.Username, request.LowerLimit, request.UpperLimit, distributionTypeId);

            response = new GetDistributionsMadeByAgentResponseBody()
            {
                ResponseCode = 0,
                ResponseMessage = "OK",
                Distributions = dists,
                TransactionID = 0
            };

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[" + providerName + "] [RECV-DATA] GetDistributionsMadeByAgentResult {response={" + (response.Distributions == null || response.Distributions.Count == 0 ? string.Empty : response.Distributions.ToString()) + "}}");

            return (response);
        }
    }
}
