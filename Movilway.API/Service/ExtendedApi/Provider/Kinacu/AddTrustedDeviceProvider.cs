using Movilway.API.Core;
using Movilway.API.Core.Security;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.AddTrustedDevice)]
    public class AddTrustedDeviceProvider : AKinacuProvider
    {

        private Logging.ILogger logger = Logging.LoggerFactory.GetLogger(typeof(AddTrustedDeviceProvider));
        protected override Logging.ILogger ProviderLogger
        {
            get
            {
                return logger;
            }

        }

        public override DataContract.Common.IMovilwayApiResponse PerformKinacuOperation(DataContract.Common.IMovilwayApiRequest requestObject, KinacuWebService.SaleInterface kinacuWS, string sessionID)
        {

            //throw new NotImplementedException();
            AddTrustedDeviceRequestBody request = (AddTrustedDeviceRequestBody)requestObject;
            AddTrustedDeviceResponseBody response = new AddTrustedDeviceResponseBody();

            try
            {

         

              if (request.DeviceType != (int)cons.ACCESS_POSWEB) { 
                    response.ResponseCode = 90;
                    response.ResponseMessage = "NO ESTA AUTORIZADO PARA ACCEDER A ESTE SERVICIO";//"ERROR INESPERADO";
                    response.TransactionID = 0;
                    return response;
                }
                
                //desnecriptar
                //String jsonString = request.InfoTokken;


                //int idUser = Utils.GetUserId(request.AuthenticationData.Username, cons.ACCESS_POSWEB);

                String jsonDecrypt = Cryptography.decrypt(request.InfoTokken);


                Dictionary<string, string> dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecrypt);

                bool status = Convert.ToBoolean(dictionary["status"]);
                int type =  Convert.ToInt32(dictionary["type"]);
                
                TrustedDevice device = new TrustedDevice()
                {
                    UserId = request.AuthenticationData.Username,
                    Token = dictionary["cookieMonster"],
                    Hash = dictionary["hash"],
                    IdType = type,
                    FriendlyName= dictionary["friendlyName"],
                    Description = dictionary["description"],
                    Status =status? cons.DEVICE_ACTIVE: cons.DEVICE_TEMPORAL,//cons.DEVICE_ACTIVE, //status? cons.DEVICE_ACTIVE: cons.DEVICE_TEMPORAL,
                     Secure=status,
                    //IsActive = true,
                    DateActivated = DateTime.MinValue,
                    Model =dictionary["model"],
                    OS = dictionary["os"],
                    DateCreated=DateTime.Now,
                    Ticks = Convert.ToInt64(dictionary["ticks"])

                  
                };



                if (!device.IsValid())
                {
                    response.ResponseCode = 90;
                    response.ResponseMessage = "LOS DATOS DEL DISPOSITIVO ESTAN INCOMPLETOS";//"ERROR INESPERADO";
                    response.TransactionID = 0;
                    //
                    return response;
                }


                GenericApiResult<bool> result = Utils.AddTrustedDeviceNuevo(device);
      
                
                response.ResponseCode = result.ResponseCode;
                response.ResponseMessage = result.ResponseMessage;
                response.Result = result.ObjectResult;



            }
            catch (Exception ex)
            {

                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;//"ERROR INESPERADO";
                response.TransactionID = 0;

                string mensaje = String.Concat("[API] " , base.LOG_PREFIX," [AddTrustedDeviceProvider]  [", ex.GetType().FullName, "] . Exception: ", ex.Message, ". ", ex.StackTrace);
                logger.ErrorLow(mensaje);

            }
            return response;
        }
    }

}