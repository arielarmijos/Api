using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Core;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetTransactionsReport)]
    public class GetTransactionsReportProvider : AKinacuProvider
    {

        //private static int COUNTER = 0;

        private String _GenericError = "NO SE PUDO OBTENER REPORTE DE TRANSACCIONES";
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetTransactionsReportProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, KinacuWebService.SaleInterface kinacuWS, string sessionID)
        {
            GetTransactionsReportResponseBody response = new GetTransactionsReportResponseBody();

            try
            {
                if (sessionID.Equals("0"))
                {
                    return new GetTransactionsReportResponseBody()
                    {
                        ResponseCode = 90,
                        ResponseMessage = "error session",
                        TransactionID = 0,
                    };
                }

                GetTransactionsReportRequestBody request = requestObject as GetTransactionsReportRequestBody;

                if (request.InitialDate > request.FinalDate)
                {
                    return new GetTransactionsReportResponseBody()
                    {
                        ResponseCode = 90,
                        ResponseMessage = "RANGO DE FECHAS INVALIDO",
                        TransactionID = 0
                    };

                }

                logger.InfoLow(() => TagValue.New().Message("[API] " + base.LOG_PREFIX + "[GetTransactionsReport]").Tag("[SEND-DATA] GetTransactionsReportParameters ").Value(request));

                switch (request.DeviceType)
                {
                    case 3:
                        //Ariel 2021-Ma-09 Comentado 
                       // response.reportData = Utils.TransactionsForReportH2H(request.Agent, request.InitialDate, request.FinalDate, request.TransactionType, request.Top);
                        break;
                    default:
                        //Ariel 2021-Ma-09 Comentado 
                       // response.reportData = Utils.TransactionsForReport(request.Agent, request.InitialDate, request.FinalDate, request.TransactionType, request.Top);
                        break;
                }

                response.ResponseMessage = "OK"; //string.Concat("OK ",COUNTER);
                response.ResponseCode = 0;
                // COUNTER = ((COUNTER + 1) % 2);
                logger.InfoLow(() => TagValue.New().Message("[API] " + base.LOG_PREFIX + "[GetTransactionsReport]").Tag(" GetTransactionsReportResult ").Value(response.reportData.Count));
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = _GenericError;
                response.TransactionID = 0;

                string mensaje = String.Concat("[API] " + base.LOG_PREFIX + "[UpdateAgentProvider] ", ". Exception: ", ex.Message, ". ", ex.StackTrace);
                logger.ErrorLow(mensaje);
            }

            return response;
        }
    }
}