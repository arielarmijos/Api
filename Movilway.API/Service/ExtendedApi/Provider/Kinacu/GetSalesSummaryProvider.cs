using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.KinacuWebService;
using Movilway.API.KinacuManagementWebService;
using Movilway.API.KinacuLogisticsWebService;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetSalesSummary)]
    public class GetSalesSummaryProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetSalesSummaryProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new GetSalesSummaryResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0,
                    FinalAmount = 0m,
                    InitialAmount = 0m,
                    Summaries = new SummaryItems(),
                    SummaryDate = DateTime.Now
                };

            GetSalesSummaryRequestBody request = requestObject as GetSalesSummaryRequestBody;
            GetSalesSummaryResponseBody response = null;

            ManagementInterface managementWS = new ManagementInterface();
            LogisticsInterface logisticsWS = new LogisticsInterface();
            
            string message;
            
            #region comentario
            //int voucherQtyDownload, retailerId;
            //string retailerName, retailerAddress, retailerLegalId, ticketHeader, currentTime;

            //if (!managementWS.GetRetailerInfo(int.Parse(sessionID), out retailerId, out retailerName, out retailerAddress, out retailerLegalId, out voucherQtyDownload, out ticketHeader, out currentTime, out message))
            //{
            //    return new GetSalesSummaryResponseBody()
            //    {
            //        ResponseCode = 97,
            //        ResponseMessage = message,
            //        Summaries = null,
            //        SummaryDate = request.Date,
            //        TransactionID = new Random().Next(100000, 999999)
            //    };
            //}

            //int userId;
            //long timeOut;
            //string userName, userLastName, userAddress;

            //if (!managementWS.GetUserInfo(int.Parse(sessionID), out userId, out userName, out userLastName, out userAddress, out timeOut, out message))
            //{
            //    return new GetSalesSummaryResponseBody()
            //    {
            //        ResponseCode = 98,
            //        ResponseMessage = message,
            //        Summaries = null,
            //        SummaryDate = request.Date,
            //        TransactionID = new Random().Next(100000, 999999)
            //    };
            //}
            #endregion 

           /* Se comenta porque no hace nada (sólo reventar en caso de overflow
            * int amount, count;

            if (!kinacuWS.SaleCountBySeller(int.Parse(sessionID), out amount, out count, out message))
            {
                return new GetSalesSummaryResponseBody()
                {
                    ResponseCode = 99,
                    ResponseMessage = message,
                    Summaries = null,
                    SummaryDate = request.Date,
                    TransactionID = new Random().Next(100000, 999999)
                };
            }*/

            //long balance = kinacuWS.GetAccountBalance(int.Parse(sessionID), out message);

            SummaryItems summary = new SummaryItems();
            decimal initialAmount, finalAmount;

            if (request.SummaryType == null || request.SummaryType.Equals(SummaryType.NotSpecified) || request.SummaryType.Equals(SummaryType.ByUser))
            {
                summary = Utils.SalesSummary(request.AuthenticationData.Username, request.Date);
                response = new GetSalesSummaryResponseBody()
                {
                    ResponseCode = 0,
                    ResponseMessage = "OK",
                    Summaries = summary,
                    SummaryDate = request.Date,
                    TransactionID = new Random().Next(100000, 999999)
                };
            }
            else
            {
                
                summary = Utils.SalesSummaryByAgent(request.AuthenticationData.Username, request.Date, out initialAmount, out finalAmount);
                response = new GetSalesSummaryResponseBody()
                {
                    ResponseCode = 0,
                    ResponseMessage = "OK",
                    Summaries = summary,
                    SummaryDate = request.Date,
                    TransactionID = new Random().Next(100000, 999999),
                    InitialAmount = initialAmount,
                    FinalAmount = finalAmount
                };
            }


            // Asignar Nombre de agencia y fecha de impresion 
            if (response != null)
            {
                response.BranchName = Utils.GetAgentName(request.AuthenticationData.Username);
                response.DatePrinter =  Utils.GetLocalTimeZone().ToString("yyyy-MM-dd HH:mm:ss");         
            }


            //response.Summaries = new SummaryItems();

            //if (request.WalletType == WalletType.Stock)
            //    response.Summaries.Add(new SummaryItem() { TotalAmount = amount / 100m, TransactionCount = count, TransactionType = "Recargas y pines" });
            //else
            //    response.Summaries.Add(new SummaryItem() { TotalAmount = 0, TransactionCount = 0, TransactionType = "Recargas y pines" });
            logger.InfoLow(() => TagValue.New().Message("[API] " + base.LOG_PREFIX + "[GetSalessSummaryResult]").Tag(" GetSalesSummaries ").Value(response.Summaries.Count));
            return (response);
        }
    }
}