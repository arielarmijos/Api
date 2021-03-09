using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetLatestAccess)]
    public class GetLatestAccessProvider : AKinacuProvider
    {
        private static readonly Logging.ILogger logger = Logging.LoggerFactory.GetLogger(typeof(GetLatestAccessProvider));

        protected override Logging.ILogger ProviderLogger
        {
            get { return logger; }
        }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, KinacuWebService.SaleInterface kinacuWS, string sessionID)
        {
            GetLatestAccessRequestBody request = (GetLatestAccessRequestBody)requestObject;
            GetLatestAccessResponseBody response = new GetLatestAccessResponseBody();
            try
            {
                if (sessionID.Equals("0"))
                {
                    response.ResponseMessage = "error session";
                    response.ResponseCode = 90;
                    return response;
                }

              response.List  =   Utils.GetLatestAccess();
            }
            catch (Exception ex)
            {

                  logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, "-", response.ResponseCode, "-", response.ResponseMessage, ". Exception: ", ex.Message, ". ", ex.StackTrace));
            }

            return response;
        }
    }
}