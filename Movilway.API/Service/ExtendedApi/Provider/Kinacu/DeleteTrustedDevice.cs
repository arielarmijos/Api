using Movilway.API.Core;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.DeleteTrustedDevice)]
    public class DeleteTrustedDevice:AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(DeleteTrustedDevice));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override DataContract.Common.IMovilwayApiResponse PerformKinacuOperation(DataContract.Common.IMovilwayApiRequest requestObject, KinacuWebService.SaleInterface kinacuWS, string sessionID)
        {
      
            DeleteTrustedDeviceResponseBody response = new DeleteTrustedDeviceResponseBody();
            DeleteTrustedDeviceRequestBody request = (DeleteTrustedDeviceRequestBody)requestObject;
            
            try {


                if (sessionID.Equals("0"))
                {
                    response.ResponseMessage = "error session";
                    response.ResponseCode = 90;
                    return response;
                }


                if (request.DeviceType != (int)cons.ACCESS_POSWEB)
                {
                    response.ResponseCode = 90;
                    response.ResponseMessage = "NO ESTA AUTORIZADO PARA ACCEDER A ESTE SERVICIO";//"ERROR INESPERADO";
                    response.TransactionID = 0;
                    return response;
                }

                //throw new NotImplementedException();

                bool result = Utils.SetStatusDevice(new TrustedDevice()
                {
                    ID = request.DeviceID,
                    Status = cons.DEVICE_DELETE

                });
                //si resultado y 

                //ok
                response.Result = result;
                if (response.Result)
                {
                    response.ResponseCode = 0;
                    response.ResponseMessage = "OK";
                }
                else
                {
                    response.ResponseCode = 101;
                    response.ResponseMessage = string.Concat("NO SE PUDO BORRAR DISPOSITIVO ", request.DeviceID);
                }

            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = string.Concat("ERROR INESPERADO AL BORRAR DISPOSITIVO ", request.DeviceID," " ,ex.Message ," ",ex.StackTrace );

                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " [DeleteTrustedDeviceProvider] ", "-", response.ResponseCode, "-", response.ResponseMessage, ". Exception: ", ex.Message, ". ", ex.StackTrace));
           
            }

            return response;
        }
    }
}