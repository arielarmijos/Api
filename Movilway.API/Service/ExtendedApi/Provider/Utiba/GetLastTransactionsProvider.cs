using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Utiba;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.GetLastTransactions)]
    public class GetLastTransactionsProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetLastTransactionsProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            GetLastTransactionsRequestBody request = requestObject as GetLastTransactionsRequestBody;
            GetLastTransactionsResponseBody response = null;

            logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetLastTransactionsProvider] [SEND-DATA] lastTransactionsRequest {sessionid=" + sessionID + ",device_type=" + request.DeviceType + ",transCount=" + request.Count + ",agent=" + request.Agent + "}");

            lastTransactionsResponse utibaGetLastTransactionsResponse = utibaClientProxy.lastTransactions(new lastTransactionsRequest()
            {
                lastTransactionsRequestType = new lastTransactionsRequestType()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType,
                    agent = request.Agent,
                    transCount = request.Count
                }
            });

            StringBuilder sb = new StringBuilder(), sb2;
            foreach (var transaction in utibaGetLastTransactionsResponse.lastTransactionsReturn.transactionsList)
            {
                sb2 = new StringBuilder();
                foreach (var party in transaction.partiesReferenceIdList)
                    sb2.Append("partiesReferenceIdList=" + party + ",");
                if (sb2.Length > 0) sb2.Remove(sb2.Length - 1, 1);

                sb.Append("transactionsList={transactionId=" + transaction.transactionId + ",transactionType=" + transaction.transactionType + 
                                    ",lastModified=" + transaction.lastModified + ",amount=" + transaction.amount + "," + sb2.ToString() + "},");
            }
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);

            logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetLastTransactionsProvider] [RECV-DATA] lastTransactionsResponse " +
                                "{transid=" + utibaGetLastTransactionsResponse.lastTransactionsReturn.transid +
                                ",result=" + utibaGetLastTransactionsResponse.lastTransactionsReturn.result +
                                ",result_namespace=" + utibaGetLastTransactionsResponse.lastTransactionsReturn.result_namespace + 
                                "," + sb.ToString() + "}");
            
            if (utibaGetLastTransactionsResponse != null)
            {
                response = new GetLastTransactionsResponseBody()
                    {
                        ResponseCode = Utils.BuildResponseCode(utibaGetLastTransactionsResponse.lastTransactionsReturn.result, utibaGetLastTransactionsResponse.lastTransactionsReturn.result_namespace),
                        ResponseMessage = utibaGetLastTransactionsResponse.lastTransactionsReturn.result_namespace,
                        TransactionID = utibaGetLastTransactionsResponse.lastTransactionsReturn.transid
                    };

                if (utibaGetLastTransactionsResponse.lastTransactionsReturn.transactionsList != null &&
                    utibaGetLastTransactionsResponse.lastTransactionsReturn.transactionsList.Length > 0)
                {
                    response.Transactions = new TransactionList();
                    foreach (Movilway.API.Utiba.TransactionSummary transaction in utibaGetLastTransactionsResponse.lastTransactionsReturn.transactionsList)
                    {
                        DataContract.TransactionSummary currentTransactionSummary = new DataContract.TransactionSummary()
                        {
                            TransactionType = Utils.GetTransactionTypeName(transaction.transactionType),
                            Amount = transaction.amount,
                            LastTimeModified = transaction.lastModified,
                            OriginalTransactionID = transaction.transactionId
                        };

                        if(transaction.partiesReferenceIdList!=null && transaction.partiesReferenceIdList.Length>0)
                        {
                            currentTransactionSummary.RelatedParties = new RelatedParties();
                            currentTransactionSummary.RelatedParties.AddRange(transaction.partiesReferenceIdList);
                        }
                        response.Transactions.Add(currentTransactionSummary);
                    }
                }
            }
            return (response);
        }
    }
}