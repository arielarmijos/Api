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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetBlockedAgenciesByInactivityReport)]
    public class GetBlockedAgenciesByInactivityReport : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetBlockedAgenciesByInactivityReport));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            // GetBlockedAgenciesByInactivityReportRequest
             GetBlockedAgenciesByInactivityResponseBody response = new GetBlockedAgenciesByInactivityResponseBody()
             {
                 ResponseCode = 99,
                 ResponseMessage = "",
                 TransactionID = 0
             };
            
             //GetBlockedAgenciesByInactivityReportResponse
             GetBlockedAgenciesByInactivityRequestBody request = requestObject as GetBlockedAgenciesByInactivityRequestBody;

            try
            {

                logger.InfoHigh(" GetBlockedAgenciesByInactivityReport data  DateMin=" + request.DateMin+ ",DateMax =  " + request.DateMax+ ", Agent =  " + request.Agent);

                //Ariel 2021-Ma-09 Comentado no devolmemos detalle 

                //response.Details = Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetBlockedAgenciesByInactivity(request);
                response.ResponseCode = 0;
                response.ResponseMessage = "OK";
            }
            catch (Exception ex)
            {
                logger.ErrorLow(() => TagValue.New().Exception(ex).Message("Error GetBlockedAgenciesByInactivityReport provider"));
                response.ResponseCode = 99;
                response.ResponseMessage = "Error inesperado.";
                response.TransactionID = 0;
            }
         
            
             return response;

          
        }
    }
}