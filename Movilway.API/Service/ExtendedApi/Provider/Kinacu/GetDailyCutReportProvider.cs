using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using Movilway.API.KinacuWebService;
using Movilway.API.KinacuManagementWebService;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetDailyCutReport)]
    public class GetDailyCutReportProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetDailyCutReportProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {

        
            GetDailyCutReportResponseBody response = new GetDailyCutReportResponseBody()
            {
                ResponseCode = 99,
                ResponseMessage = "",
                TransactionID = 0
            };

            GetDailyCutReportRequestBody request = requestObject as GetDailyCutReportRequestBody;

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + " [GetDailyCutReportProvider] [SEND-DATA] parameters {date=" + request.Date + "}");

            try
            {
                //Ariel 2021-Ma-09 Comentado 
               // response = Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetDailyCutReport(request);
                response.ResponseCode = 0;
                response.ResponseMessage = "OK";
            }
            catch (Exception ex)
            {

                response.ResponseCode = 99;
                response.ResponseMessage = ex.Message;
                logger.ErrorHigh(  base.LOG_PREFIX +" " +ex.Message + " " + ex.GetType().FullName + " " + ex.StackTrace);
            }


        

            return response;
        }
    }
}