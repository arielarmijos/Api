using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Utiba;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.GetSalesSummary)]
    public class GetSalesSummaryProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetSalesSummaryProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            GetSalesSummaryRequestBody request = requestObject as GetSalesSummaryRequestBody;
            GetSalesSummaryResponseBody response = null;

            //int startEpochTime = Utils.FromDateTimeToEpoch(request.StartDate);
            //int endEpochTime = Utils.FromDateTimeToEpoch(request.EndDate);

            response = new GetSalesSummaryResponseBody()
            {
                ResponseCode = 0,
                ResponseMessage = "OK",
                Summaries = Utils.SalesSummary(request.AuthenticationData.Username, request.Date, (int)request.WalletType),
                SummaryDate = request.Date,
                TransactionID = new Random().Next(100000,999999)
            };

            //salesResponse utibaSalesResponse = utibaClientProxy.sales(new sales()
            //{
            //    salesRequest = new salesRequestType()
            //    {
            //        sessionid = sessionID,
            //        device_type = request.DeviceType,
            //        start = (startEpochTime * 1000L),
            //        startSpecified = true,
            //        end = (endEpochTime * 1000L),
            //        endSpecified = true,
            //        type = (int)request.WalletType,
            //        target = request.Product
            //    }
            //});

            //if (utibaSalesResponse != null)
            //{
            //    response = new GetSalesSummaryResponseBody()
            //    {
            //        ResponseCode = utibaSalesResponse.salesReturn.result,
            //        ResponseMessage = utibaSalesResponse.salesReturn.result_message,
            //        TransactionID = utibaSalesResponse.salesReturn.transid,
            //        TransactionCount = utibaSalesResponse.salesReturn.count,
            //        TotalAmount = utibaSalesResponse.salesReturn.sum
            //    };
            //}
            return (response);
        }
    }
}