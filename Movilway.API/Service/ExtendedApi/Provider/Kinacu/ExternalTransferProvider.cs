using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Data;
using Movilway.Logging;
using Movilway.API.KinacuWebService;
using Movilway.API.KinacuLogisticsWebService;
using System.Configuration;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.ExternalTransfer)]
    public class ExternalTransferProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(TransferProvider));
        protected override ILogger ProviderLogger { get { return logger; } }
        protected override TransactionType TransactionType { get { return TransactionType.transfer;} }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new TransferResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0, 
                    StockBalance = 0m
                };

            TransferRequestBody request = requestObject as TransferRequestBody;
            TransferResponseBody response = null;

            if (request.AuthenticationData.Username != ConfigurationManager.AppSettings["ProincoUser"])
                return new TransferResponseBody()
                {
                    Fee = 0,
                    ResponseCode = 99,
                    ResponseMessage = "fallo",
                    TransactionID = 0
                };

            string myAgent = new IBank.Utils().GetAgentPdv(request.Recipient);

            var distributionResponse = new IBank.Utils().ExecuteDistribution(ConfigurationManager.AppSettings["id_net"], request.ExternalTransactionReference, DateTime.Now, myAgent, request.Amount, ConfigurationManager.AppSettings["AccountProinco"]);

            if (int.Parse(distributionResponse.responseCode) == 0)
            {
                response = new TransferResponseBody()
                {
                    Fee = 0,
                    ResponseCode = 0,
                    ResponseMessage = distributionResponse.Message,
                    TransactionID = 0
                };
            }
            else
            {
                response = new TransferResponseBody() {
                    Fee = 0,
                    ResponseCode = 99,
                    ResponseMessage = "fallo",
                    TransactionID = 0
                };
            }

            return (response);
        }
    }
}