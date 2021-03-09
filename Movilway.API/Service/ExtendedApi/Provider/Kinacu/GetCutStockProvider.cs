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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetCutStock)]
    public class GetCutStockProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetCutStockProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            GetCutStockResponseBody response = new GetCutStockResponseBody();http://localhost:54689/Web References/
            if (sessionID.Equals("0"))
            {
                response.ResponseCode = 90;
                response.ResponseMessage = "error session";
                response.TransactionID = 0;
                return response;
            }

            GetCutStockRequestBody request = requestObject as GetCutStockRequestBody;
            try
            {
                int currentUserId = Utils.GetUserId(request.AuthenticationData.Username);
                //Ariel 2021-Ma-09 Comentado 
               // response = Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetCutStockReport(request.Date, currentUserId);
            }
            catch (Exception ex)
            {
                logger.ErrorHigh(String.Concat(LOG_PREFIX, " [RECEIVED-DATA] GetCusStock. ", ex.GetType().FullName + " " + ex.Message + " " + ex.StackTrace));

                response.ResponseCode = 95;
                response.ResponseMessage = "Error. " + ex.Message;
            }       
            return response;
        }
    }
}