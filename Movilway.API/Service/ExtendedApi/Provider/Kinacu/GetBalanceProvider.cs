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
    [ServiceProviderImpl(Platform=ApiTargetPlatform.Kinacu, ServiceName=ApiServiceName.GetBalance)]
    public class GetBalanceProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetBalanceProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, string sessionID)
        {
            if (sessionID.Equals("0"))
                return new GetBalanceResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0
                };

            GetBalanceRequestBody request = requestObject as GetBalanceRequestBody;
            string message;

            logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetBalanceProvider] [SEND-DATA] GetAccountBalanceParameters {UserId=" + sessionID + "}");

            long balance = kinacuWS.GetAccountBalance(int.Parse(sessionID), out message);

            logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetBalanceProvider] [RECV-DATA] GetAccountBalanceResult {response=" + balance + ",message=" + message + "}");

            GetBalanceResponseBody response = new GetBalanceResponseBody()
            {                
                ResponseCode = balance == -1 ? 99 : 0,
                ResponseMessage = message == "" ? (balance / 100m).ToString() : message,
                TransactionID = new Random().Next(100000,999999),
                WalletBalance = 0,
                StockBalance = balance/100m,
                PointsBalance = 0,
                DebtBalance = 0
            };

            if (request.ExtendedValues ?? false)
                response.CheckingAccountBalance = Utils.GetAgentCheckingAccountBalance(request.AuthenticationData.Username);

            return (response);
        }
    }
}