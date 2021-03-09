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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetPinUsers)]
    public class GetPinUsersProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetPinUsersProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new GetPinUsersResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0,
                    //Transactions = new TransactionList()
                     Users= new List<string>()
                };

            GetPinUsersRequest request = requestObject as GetPinUsersRequest;
            GetPinUsersResponseBody response = null;

           // logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[GetPinUsersProvider] [SEND-DATA] getLastTransactionsParameters {agentReference=" + request.Request.AgeId + ",count=" + request.Count + "}");

            response = new GetPinUsersResponseBody
            {
                ResponseCode = 0,
                ResponseMessage = "OK",
                TransactionID = 0,
                Users = Utils.GetPinUser(requestObject.AuthenticationData.Username)
            };



            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[GetPinUsersProvider] [RECV-DATA] GetPinUsersProvider {count="+response.Users.Count+"}");

            return (response);
        }
    }
}