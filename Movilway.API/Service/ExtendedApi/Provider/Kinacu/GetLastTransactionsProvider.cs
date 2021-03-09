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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetLastTransactions)]
    public class GetLastTransactionsProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetLastTransactionsProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new GetLastTransactionsResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0,
                    Transactions = new TransactionList()
                };

            GetLastTransactionsRequestBody request = requestObject as GetLastTransactionsRequestBody;
            GetLastTransactionsResponseBody response = null;

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[GetLastTransactionsProvider] [SEND-DATA] getLastTransactionsParameters {agentReference=" + request.Agent + ",count=" + request.Count + "}");

            response = new GetLastTransactionsResponseBody()
            {
                ResponseCode = 0,
                ResponseMessage = "OK",
                Transactions = Utils.LastTransactions(request.Agent, request.Count, request.TransactionType),
                TransactionID = 0
            };

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[GetLastTransactionsProvider] [RECV-DATA] getLastTransactionsResult {response={" + (response.Transactions == null || response.Transactions.Count == 0 ? "" : response.Transactions.ToString()) + "}}");

            return (response);
        }
    }
}