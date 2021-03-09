using Movilway.API.KinacuWebService;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.ChangePassword)]
    public class ChangePasswordProvider : AKinacuProvider
    {

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(ChangePasswordProvider));
        protected override ILogger ProviderLogger { get { return logger; } }


        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {

            try
            {
                ChangePasswodRequestBody request = requestObject as ChangePasswodRequestBody;


                ChangePasswordResponseBody response = null;

                logger.InfoLow(() => TagValue.New().Message("[API] " + base.LOG_PREFIX + "[ChangePasswordProvider] Start").Tag("User").Value(request.AuthenticationData.Username).Tag("UserId").Value(request.UserId).Tag("AccessTypeId").Value(request.AccessTypeId));


                response = new ChangePasswordResponseBody()
                {
                    ResponseCode = 0,
                    ResponseMessage = "Exito",
                    TransactionID = 0,
                    Result = Utils.ChangePassword(request.AuthenticationData.Username,request.DeviceType,request.UserId,request.AccessTypeId,request.NewPassword)
                };

                if (response.Result == false)
                {
                    response.ResponseMessage = "Contraseña no actualizada";
                    response.ResponseCode = 99;
                }
                logger.InfoLow(() => TagValue.New().Message("[API] " + base.LOG_PREFIX + "[ChangePasswordProvider] End").Tag("User").Value(request.AuthenticationData.Username).Tag("UserId").Value(request.UserId).Tag("AccessTypeId").Value(request.AccessTypeId));
                return (response);
            }
            catch (Exception e)
            {
                //logger.ExceptionHigh(() => TagValue.New().Message("[API] " + base.LOG_PREFIX + "[GetUsersAgentProvider] Exception").Tag("User").Value(request.AuthenticationData.Username).Tag("Agent").Value(request.Agent));
                logger.ExceptionHigh(() => TagValue.New().Exception(e).MethodName("ChangePassword").Tag("User").Value(requestObject.AuthenticationData.Username));


                throw e;
            }

        }

    
    }
}