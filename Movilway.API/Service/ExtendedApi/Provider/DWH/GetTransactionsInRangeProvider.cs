using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Data;
using System.Configuration;

namespace Movilway.API.Service.ExtendedApi.Provider.DWH
{
    [ServiceProviderImpl(Platform=ApiTargetPlatform.DWH, ServiceName=ApiServiceName.GetTransactionsInRange)]
    public class GetTransactionsInRangeProvider:IServiceProvider
    {
        public IMovilwayApiResponse PerformOperation(IMovilwayApiRequest requestObject)
        {
            int CountryID = int.Parse(ConfigurationManager.AppSettings["CountryID"]);

            GetTransactionsInRangeRequestBody request = requestObject as GetTransactionsInRangeRequestBody;
            GetTransactionsInRangeResponseBody response=null;

            MovilBaseDataContext dataContext = new MovilBaseDataContext();

            List<String> lista = new List<string>();

            var trans = dataContext.Branches.Where(b => b.CellPhone == request.Agent && b.CountryId==CountryID).Join(dataContext.Transactions,
                b => b.BranchId,
                t => t.BranchId,
                (branch, transaction) => new
                {
                    TransactionID = transaction.PrimaryCode,
                    TransactionType = 1,
                    Amount = transaction.Amount,
                    TransactionDate = transaction.DateValue,
                    TransactionDateTime = transaction.TransactionDate,
                    CountryID = transaction.CountryId
                }).Where(t => t.TransactionDate >= request.StartDate && t.TransactionDate <= request.EndDate && t.CountryID==CountryID)
                .OrderBy(t => t.TransactionDateTime);
            if (trans.Count() > 0)
            {
                response = new GetTransactionsInRangeResponseBody()
                    {
                        ResponseCode = 0,
                        Transactions = new TransactionSummaryLiteList()
                    };
                foreach (var tranDetails in trans)
                {
                    response.Transactions.Add(new TransactionSummaryLite()
                    {
                        Amount = tranDetails.Amount,
                        Date = tranDetails.TransactionDate,
                        OriginalTransactionID = tranDetails.TransactionID,
                        Type = tranDetails.TransactionType.ToString()
                    });
                }
            }
            else
            {
                response = new GetTransactionsInRangeResponseBody()
                    {
                        ResponseCode = 0,
                        ResponseMessage = "No Transactions Found",
                        Transactions = new TransactionSummaryLiteList()
                    };
            }
            return (response);
        }
    }
}