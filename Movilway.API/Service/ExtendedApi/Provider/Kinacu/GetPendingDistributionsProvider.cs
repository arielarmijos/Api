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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetPendingDistributions)]
    public class GetPendingDistributionsProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetPendingDistributionsProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new GetPendingDistributionsResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0,
                    Distributions = new DistributionList()
                };

            GetPendingDistributionsRequestBody request = requestObject as GetPendingDistributionsRequestBody;
            GetPendingDistributionsResponseBody response = null;

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[GetPendingDistributionsProvider] [SEND-DATA] getPendingDistributionsParameters {agentReference=" + request.Agent + ",count=" + (request.Count != null ? request.Count.ToString() : "null") + "}");

            //var agentId = new Movilway.API.Service.ExtendedApi.Provider.IBank.Utils().GetAgentId(request.AuthenticationData.Username);
            var result = new IBank.Utils().ListaSolicitudesPendientes(Convert.ToInt32(request.Agent), -1);

            response = new GetPendingDistributionsResponseBody()
            {
                ResponseCode = 0,
                ResponseMessage = "OK",
                Distributions = result, // generateRandomDist(request.Count ?? new Random().Next(3, 25)),
                TransactionID = 0
            };

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[GetPendingDistributionsProvider] [RECV-DATA] getPendingDistributionsResult {response={" + (response.Distributions == null || response.Distributions.Count == 0 ? "" : response.Distributions.ToString()) + "}}");

            return (response);
        }

        public DistributionList generateRandomDist(int count)
        {
            var dists = new DistributionList();
            var dist = new DistributionSummary();
            var rand = new Random();
            bool hasDeposit = false;
            for (int i = 0; i < count; i++)
            {
                hasDeposit = rand.Next(1, 10) % 2 == 0;

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

                dists.Add(dist);
            }
            return dists;
        }
    }
}