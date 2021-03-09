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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.SetUserStatus)]
    public class SetUserStatusProvider : AKinacuProvider
    {


        public SetUserStatusProvider() { 
        
        }


        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetUsersAgentProvider));
        protected override ILogger ProviderLogger { get { return logger; } }


        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {

            try
            {
                SetUserStatusRequestBody request = requestObject as SetUserStatusRequestBody;


                SetUserStatusResponseBody response = null;

                logger.InfoLow(() => TagValue.New().Message("[API] " + base.LOG_PREFIX + "[SetUserStatusProvider] Start").Tag("User").Value(request.AuthenticationData.Username).Tag("UserId").Value(request.UserId).Tag("Status").Value(request.Status));


                response = new SetUserStatusResponseBody()
                {
                    ResponseCode = 0,
                    ResponseMessage = "Exito",
                    TransactionID = 0,
                       Result= Utils.SetUserStatus(request.UserId,request.Status,request.DeviceType)
                };

                //logger.InfoLow(() => TagValue.New().Message("[API] " + base.LOG_PREFIX + "[GetUsersAgentProvider] End").Tag("User").Value(request.AuthenticationData.Username).Tag("Agent").Value(request.Agent));
                return (response);
            }
            catch (Exception e)
            {
                //logger.ExceptionHigh(() => TagValue.New().Message("[API] " + base.LOG_PREFIX + "[GetUsersAgentProvider] Exception").Tag("User").Value(request.AuthenticationData.Username).Tag("Agent").Value(request.Agent));
                logger.ExceptionHigh(() => TagValue.New().Exception(e).MethodName("GetUsersAgentProvider").Tag("User").Value(requestObject.AuthenticationData.Username));


                throw e;
            }

        }

    
    }
}