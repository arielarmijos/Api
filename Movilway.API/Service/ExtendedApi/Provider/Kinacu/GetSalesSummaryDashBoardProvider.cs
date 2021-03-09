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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetSalesSummaryDashBoard)]
    public class GetSalesSummaryDashBoardProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetSalesSummaryDashBoardProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new GetSalesSummaryDashBoardResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0,
                    SalesSummariesDashBoard = new SalesSummaryDashBoardItems()
                };

            GetSalesSummaryDashBoardRequestBody request = requestObject as GetSalesSummaryDashBoardRequestBody;
            GetSalesSummaryDashBoardResponseBody response = null;

            ManagementInterface managementWS = new ManagementInterface();
            LogisticsInterface logisticsWS = new LogisticsInterface();

            string message;

            SalesSummaryDashBoardItems summary = new SalesSummaryDashBoardItems();

            summary = Utils.SalesSummaryByAgentDashBoard(request.AuthenticationData.Username, request.InitialDate, request.EndDate, out message);

            if (message.Equals("OK"))
            {
                decimal auxTotalSales = 0;
                if (summary != null)
                {
                    auxTotalSales = summary.Sum(r => r.TotalAmount);

                    CommissionSalesSummaryDashItem summaryCommi = new CommissionSalesSummaryDashItem();
                    summaryCommi = Utils.CommissionSalesSummaryByAgentDashBoard(request.AuthenticationData.Username, request.InitialDate, request.EndDate, out message);

                    if (message.Equals("OK") && summaryCommi != null)
                    {
                        string comissionValue = summaryCommi.CommiPercentage;
                        decimal valueComm = 0;
                        if (!String.IsNullOrEmpty(comissionValue))
                        {
                            valueComm = GetAverageCommission(summary, comissionValue);
                        }

                        response = new GetSalesSummaryDashBoardResponseBody()
                       {
                           ResponseCode = 0,
                           ResponseMessage = message,
                           SalesSummariesDashBoard = summary,
                           SummaryDate = request.EndDate,
                           TransactionID = 0,
                           TotalSales = auxTotalSales,
                           TotalRevenue = summaryCommi.TotalCommiSales,
                           AverageCommission = valueComm
                       };
                    }
                    else
                    {
                        response = new GetSalesSummaryDashBoardResponseBody()
                        {
                            ResponseCode = 90,
                            ResponseMessage = message,
                            SalesSummariesDashBoard = new SalesSummaryDashBoardItems(),
                            SummaryDate = request.EndDate,
                            TransactionID = 0
                        };
                    }
                }
                else
                {
                    response = new GetSalesSummaryDashBoardResponseBody()
                    {
                        ResponseCode = 90,
                        ResponseMessage = message,
                        SalesSummariesDashBoard = new SalesSummaryDashBoardItems(),
                        SummaryDate = request.EndDate,
                        TransactionID = 0
                    };
                }
            }
            else
            {
                response = new GetSalesSummaryDashBoardResponseBody()
               {
                   ResponseCode = 90,
                   ResponseMessage = message,
                   SalesSummariesDashBoard = new SalesSummaryDashBoardItems(),
                   SummaryDate = request.EndDate,
                   TransactionID = 0
               };
            }

            logger.InfoLow(() => TagValue.New().Message("[API] " + base.LOG_PREFIX + "[GetSalesSummaryDashBoardResult]").Tag(" GetSummarySalesDashBoard ").Value(response.SalesSummariesDashBoard.Count).Tag("Mensaje").Value(message));

            return (response);
        }

        private static decimal GetAverageCommission(SalesSummaryDashBoardItems summary, string comissionValue)
        {
            string[] listComission = comissionValue.Split('#');
            int countPercentage = 0;
            decimal sumatoria = 0;
            List<string> productsNames = summary.Select(s => s.ProductName).Distinct().ToList<string>();

            Dictionary<string, decimal> dictionaryCommission = new Dictionary<string, decimal>();

            for (int i = 0; i < listComission.Count(); i++)
            {
                string[] itemValuesCommi = listComission[i].Split(':');
                dictionaryCommission.Add(itemValuesCommi[0].ToString().Trim(), Convert.ToDecimal(itemValuesCommi[1].ToString()));
            }

            for (int i = 0; i < productsNames.Count(); i++)
            {
                decimal itemDictionary = 0;
                if (dictionaryCommission.TryGetValue(productsNames[i], out itemDictionary))
                {
                    sumatoria = sumatoria + itemDictionary;
                    countPercentage++;
                }                
            }

            if (countPercentage > 0)
            {
                sumatoria = sumatoria / countPercentage;
                sumatoria = Math.Truncate(sumatoria * 100) / 100;
            }

            return sumatoria;
        }
    }
}