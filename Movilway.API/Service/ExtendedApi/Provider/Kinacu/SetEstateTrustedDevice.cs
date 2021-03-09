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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.SetStateTrustedDevice)]
    public class SetStateTrustedDevice: AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(SetStateTrustedDevice));
        protected override ILogger ProviderLogger { get { return logger; } }


        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, KinacuWebService.SaleInterface kinacuWS, string sessionID)
        {

            SetStateTrustedDeviceRequestBody request = (SetStateTrustedDeviceRequestBody)requestObject;
            SetStateTrustedDeviceResponseBody response = new SetStateTrustedDeviceResponseBody();
            try
            {


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


                //se obtiene el dispositivo 
              //GenericApiResult<TrustedDevice> resultadodevice =  Utils.GetDeviceId(request.DeviceID);
               
              //    response.ResponseCode = resultadodevice.ResponseCode;
              //      response.ResponseMessage = resultadodevice.ResponseMessage;

              //  if(resultadodevice.IsObjectValidResult()){


                 //   TrustedDevice device = resultadodevice.ObjectResult;
                 //device.Status = request.Status;

               bool result =   Utils.SetStatusDevice(new TrustedDevice(){
                   ID = request.DeviceID, Status = request.Status
               });

                     if (result)
                    {
                        response.Result = result;
                        response.ResponseCode = 0;
                        response.ResponseMessage = "OK";
                    }
                    else
                    {
                        response.ResponseCode = 101;
                        response.ResponseMessage = string.Concat("NO SE PUDO CAMBIAR DE ESTADO DISPOSITIVO ", request.DeviceID);
                    }

                //}
                
              


            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = string.Concat("ERROR INESPERADO CAMBIANDO ESTADO DISPOSITIVO ", request.DeviceID, " ", ex.Message, " ", ex.StackTrace);
                logger.ErrorLow(String.Concat("[API] ", base.LOG_PREFIX, "[SetStateTrustedDeviceProvider] ", "-", response.ResponseCode, "-", response.ResponseMessage, ". Exception: ", ex.Message, ". ", ex.StackTrace));
            }
            return response;
        }
    }
}