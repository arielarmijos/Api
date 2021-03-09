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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetLastDistributions)]
    public class GetLastDistributionsProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetLastDistributionsProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new GetLastDistributionsResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0,
                    Distributions = new DistributionList()
                };

            GetLastDistributionsRequestBody request = requestObject as GetLastDistributionsRequestBody;
            GetLastDistributionsResponseBody response = null;

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[GetLastDistributionsProvider] [SEND-DATA] getLastDistributionsParameters {agentReference=" + request.Agent + ",count=" + request.Count + "}");

            var agentId = new IBank.Utils().GetAgentId(request.AuthenticationData.Username);
            var result = new IBank.Utils().ListaSolicitudes(agentId, request.Count);

            response = new GetLastDistributionsResponseBody()
            {
                ResponseCode = 0,
                ResponseMessage = "OK",
                Distributions = result, //generateRandomDist(request.Count),
                TransactionID = 0
            };

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[GetLastDistributionsProvider] [RECV-DATA] getLastDistributionsResult {response={" + (response.Distributions == null || response.Distributions.Count == 0 ? "" : response.Distributions.ToString()) + "}}");

            return (response);
        }

        public DistributionList generateRandomDist(int count)
        {
            var dists = new DistributionList();
            var dist = new DistributionSummary();
            var rand = new Random();
            bool hasDeposit = false, wasProcessed = false;
            for (int i = 0; i < count; i++)
            {
                hasDeposit = rand.Next(1, 10) % 2 == 0;
                wasProcessed = rand.Next(1, 10) % 2 == 0;

                dist = new DistributionSummary()
                {
                    OriginalDistributionID = rand.Next(100000, 999999),
                    TargetAgentID = rand.Next(100, 10000),
                    TargetAgentName = "Random Branch #" + i,
                    RequestTime = DateTime.Now.AddMinutes(rand.Next(100, 10000) * -1),
                    Amount = rand.Next(10, 10000) * 1000,
                    HasDeposit = hasDeposit,
                    BankName = hasDeposit ? "Random Bank #" + i : null,
                    AccountNumber = hasDeposit ? rand.Next(1000000, 9999999).ToString() : null,
                    ReferenceNumber = hasDeposit ? rand.Next(10000, 99999).ToString() : null,
                    DepositComment = "Deposit comment #" + i
                };
                if (hasDeposit) dist.DepositDate = DateTime.Now.AddMinutes(rand.Next(100, 10000) * -1);
                if (wasProcessed)
                {
                    dist.Status = "AC";
                    dist.ApprobationDate = DateTime.Now.AddMinutes(rand.Next(100, 10000) * -1);
                    dist.ApprovalComment = "Random comment #" + i;
                }

                dists.Add(dist);
            }
            return dists;
        }
    }
}