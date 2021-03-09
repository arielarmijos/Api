using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Data;
using Movilway.Logging;
using Movilway.API.KinacuWebService;
using Movilway.API.KinacuLogisticsWebService;
using System.Configuration;
namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetAuditoryUsersReport)]
    public class GetAuditoryUsersReportProvider : AKinacuProvider
    {

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetAuditoryUsersReportProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public GetAuditoryUsersReportProvider()
        {
           
        }


        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {


            //si es la agencia 4 obtnere todos los usuarios nuevos de una red (PROESA) segun la fecha
            //SI ES CUALQUIER OTRA AGENCIA MOSTRAR LOS USUARIOS DE PROESA
            GetAuditoryUsersReportRequestBody request = requestObject as GetAuditoryUsersReportRequestBody;

            GetAuditoryUsersReportResponsetBody response = new GetAuditoryUsersReportResponsetBody() {

                ResponseCode= 99,
                ResponseMessage="Error"
            };

            try
            {

                logger.InfoHigh("[GetAuditoryUsersReportProvider] REQUEST " + base.LOG_PREFIX + ",  InitialDate = " + request.InitialDate+ ";  EndDate = " + request.EndDate + ";  FilterRequest = " + request.FilterRequest);

                //Ariel 2021-Ma-09 Comentado 
               // response =  Utils.GetAuditoryUsersReport( base.LOG_PREFIX,request);

           
            }
            catch (Exception ex)
            {

                response.ResponseCode = 99;
                response.ResponseMessage = "Error inesperado obteniendo el reporte de auditoria de agencias.";
                logger.ErrorHigh("[GetAuditoryUsersReportProvider] " + base.LOG_PREFIX + " " + ex.Message + ", " + ex.StackTrace);
            }

            logger.InfoHigh("[GetAuditoryUsersReportProvider] END ["+response.ResponseMessage+"]");
            return (response);
        }
    }
}