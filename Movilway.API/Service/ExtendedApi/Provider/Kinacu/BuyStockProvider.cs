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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.BuyStock)]
    public class BuyStockProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(BuyStockProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new BuyStockResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0
                };

            BuyStockRequestBody request = requestObject as BuyStockRequestBody;
            BuyStockResponseBody response = null;
            string message, bankName = "";
            //decimal bankNumber = 0; //= int.Parse(request.BankName); //decimal.Parse(request.BankName.Split('-')[1].Trim());

            BankAccount[] banks = kinacuWS.GetAccountBank(int.Parse(sessionID), out message);

            foreach (var bank in banks)
            //if (bank.BankName == request.BankName.Split('-')[0].Trim())
                if (bank.Id == int.Parse(request.BankName))
                {
                    bankName = String.Concat(bank.BankName, " - ", bank.Number);
                    break;
                }

            if (request.TransactionDate.Date <= DateTime.Now.Date)
            {
                if (kinacuWS.InformPayment(int.Parse(sessionID), decimal.Parse(request.BankName), request.TransactionReference, request.TransactionDate, decimal.Parse((request.Amount * 100).ToString("#")), "", out message))
                    response = new BuyStockResponseBody()
                    {
                        ResponseCode = 0,
                        ResponseMessage = ("Bank: " + bankName ?? "NULL") + "; Fecha: " + (request.TransactionDate.ToShortDateString() ?? "NULL") + "; REF.: " + (request.TransactionReference ?? "NULL"),
                        Fee = 0,
                        TransactionID = 0
                    };
                else
                    response = new BuyStockResponseBody()
                    {
                        ResponseCode = 99,
                        ResponseMessage = message,
                        Fee = 0,
                        TransactionID = 0
                    };
            }
            else
            {
                response = new BuyStockResponseBody()
                {
                    ResponseCode = 98,
                    ResponseMessage = "Fecha reportada superior a la fecha actual",
                    Fee = 0,
                    TransactionID = 0
                };
            }

            return (response);
        }
    }
}