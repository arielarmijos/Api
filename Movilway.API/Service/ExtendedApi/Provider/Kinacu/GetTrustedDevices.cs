using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetTrustedDevices)]
    public class GetTrustedDevices: AKinacuProvider
    {

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetTrustedDevices));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, KinacuWebService.SaleInterface kinacuWS, string sessionID)
        {
            GetTrustedDevicesRequestBody request = (GetTrustedDevicesRequestBody)requestObject;
            GetTrustedDevicesResponseBody response = new GetTrustedDevicesResponseBody();
            try
            {
                //OK
                response.ResponseCode = 0;
                response.ResponseMessage = "OK";
                response.Devices = Utils.TrustedDevices();
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = string.Concat("ERROR AL SELECCIONAR LISTA DE DISPOSITIVOS ", " ", ex.Message, " ", ex.StackTrace);
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, "-", response.ResponseCode, "-", response.ResponseMessage, ". Exception: ", ex.Message, ". ", ex.StackTrace));
            }

            return response;
        }
    }
}