using Movilway.API.Core;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.ValidateDevice)]
    public class ValidateDeviceProvider : AKinacuProvider
    {

        private Logging.ILogger logger = Logging.LoggerFactory.GetLogger(typeof(ValidateDeviceProvider));
        protected override Logging.ILogger ProviderLogger
        {
            get
            {
                return logger;
            }
        }

        public override DataContract.Common.IMovilwayApiResponse PerformKinacuOperation(DataContract.Common.IMovilwayApiRequest requestObject, KinacuWebService.SaleInterface kinacuWS, string sessionID)
        {

            // throw new NotImplementedException();
            ValidateDeviceRequestBody request = (ValidateDeviceRequestBody)requestObject;
            ValidateDeviceResponseBody response = new ValidateDeviceResponseBody();

            response.LastVersion = "";

            try
            {
                // int idUser = Utils.GetUserId(request.AuthenticationData.Username, cons.ACCESS_POSWEB);


                GenericApiResult<TrustedDevice> device = Utils.GetDeviceByKey(request.TokenToValidate, requestObject.AuthenticationData.Username);


                if (device.IsObjectValidResult())
                {


                    GenericApiResult<bool> result = Utils.IsActiveDevice(device.ObjectResult); //Utils.ValidateDevice(request.TokenToValidate, idUser);

                    response.IsValid = result.ObjectResult;
                    response.ResponseCode = result.ResponseCode;
                    response.ResponseMessage = result.ResponseMessage;//"ERROR INESPERADO";

                    /// SE CAMBIO A A ESTADO SI NO ES ACTIVO Y SI ES TEMPORAL
                    if (!result.ObjectResult && device.ObjectResult.Status == cons.DEVICE_TEMPORAL)
                    {

                        //CAMBIAR A SERVICE EXCUTE DELEGATOR
                        string ERROR = "[NO SE PUDO CAMBIAR ESTADO A DISPOSITIVO TEMPORAL]";
                        try
                        {
                            //SE CAMBIA EL ESTADO BORRANDO
                            device.ObjectResult.Status = cons.DEVICE_DELETE;
                            if (!
                             Utils.SetStatusDevice(device.ObjectResult)//;
                                )
                            {
                                string mensaje = String.Concat("[API] ", base.LOG_PREFIX, "[ValidateDeviceProvider] ", ERROR);
                                logger.InfoLow(mensaje);
                            }

                        }
                        catch (Exception ex)
                        {


                            string mensaje = String.Concat("[API] ", base.LOG_PREFIX, "[ValidateDeviceProvider] ", ERROR, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                            logger.ErrorLow(mensaje);
                            response.ResponseMessage = String.Concat(response.ResponseMessage, " ", ERROR);

                        }
                    }



                    if (device.ObjectResult.IdType != cons.DEVICE_TYPE_WEB)
                    {
                        response.LastVersion = System.Configuration.ConfigurationManager.AppSettings[String.Concat("VERSION_APP_", device.ObjectResult.IdType)] ?? System.Configuration.ConfigurationManager.AppSettings["VERSION_APP"] ?? "0";
                    }



                }
                else
                {
                    // RESPONSE CODE


                    response.ResponseCode = 100;
                    response.ResponseMessage = device.ResponseMessage;//"ERROR INESPERADO";
                                                                      // response.LastVersion =   System.Configuration.ConfigurationManager.AppSettings["VERSION_APP"] ?? "0";

                }


            }
            catch (Exception ex)
            {

                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;//"ERROR INESPERADO";
                response.TransactionID = 0;

                string mensaje = String.Concat("[API] ", base.LOG_PREFIX, "[ValidateDeviceProvider] ", ". Exception: ", ex.Message, ". ", ex.StackTrace);
                logger.ErrorLow(mensaje);

            }
            return response;
        }
    }
}