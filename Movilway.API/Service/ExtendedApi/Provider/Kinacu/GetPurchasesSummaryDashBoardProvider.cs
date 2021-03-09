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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetPurchasesSummaryDashBoard)]
    public class GetPurchasesSummaryDashBoardProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetPurchasesSummaryDashBoardProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new GetPurchasesSummaryDashBoardResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0,
                    PurchasesSummariesDashBoard = new PurchasesSummaryDashBoardItems()
                };

            GetPurchasesSummaryDashBoardRequestBody request = requestObject as GetPurchasesSummaryDashBoardRequestBody;
            GetPurchasesSummaryDashBoardResponseBody response = null;

            ManagementInterface managementWS = new ManagementInterface();
            LogisticsInterface logisticsWS = new LogisticsInterface();

            string message;

            PurchasesSummaryDashBoardItems summary = new PurchasesSummaryDashBoardItems();

            summary = Utils.PurchasesSummaryByAgentDashBoard(request.AuthenticationData.Username, request.InitialDate, request.EndDate, out message);

            if (message.Equals("OK"))
            {
                decimal auxTotalPurchases = 0;
                if (summary != null)
                {
                    auxTotalPurchases = summary.Sum(r => r.TotalAmount);
                }
                
                CommissionPurchasesSummaryDashItem summaryCommi = new CommissionPurchasesSummaryDashItem();

                summaryCommi = Utils.CommissionPurchasesSummaryByAgentDashBoard(request.AuthenticationData.Username, request.InitialDate, request.EndDate, out message);

                if (message.Equals("OK") && summaryCommi != null)
                {
                    response = new GetPurchasesSummaryDashBoardResponseBody()
                   {
                       ResponseCode = 0,
                       ResponseMessage = message,
                       PurchasesSummariesDashBoard = summary,
                       SummaryDate = request.EndDate,
                       TransactionID = 0,
                       TotalPurchases = auxTotalPurchases,
                       TotalRevenue = summaryCommi.TotalCommiPurchases,
                       AverageCommission = summaryCommi.CommiPercentage
                   };
                }
                else
                {
                    response = new GetPurchasesSummaryDashBoardResponseBody()
                    {
                        ResponseCode = 90,
                        ResponseMessage = message,
                        PurchasesSummariesDashBoard = new PurchasesSummaryDashBoardItems(),
                        SummaryDate = request.EndDate,
                        TransactionID = 0
                    };
                }
            }
            else
            {
                response = new GetPurchasesSummaryDashBoardResponseBody()
               {
                   ResponseCode = 90,
                   ResponseMessage = message,
                   PurchasesSummariesDashBoard = new PurchasesSummaryDashBoardItems(),
                   SummaryDate = request.EndDate,
                   TransactionID = 0
               };
            }

            logger.InfoLow(() => TagValue.New().Message("[API] " + base.LOG_PREFIX + "[GetPurchasesSummaryDashBoardResult]").Tag(" GetPurchasesSummaryDashBoardProvider ").Value(response.PurchasesSummariesDashBoard.Count).Tag("Mensaje").Value(message));

            return (response);
        }
    }
}