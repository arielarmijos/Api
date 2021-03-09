using Movilway.API.Data.MacroProduct;
using Movilway.API.KinacuWebService;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Cache;
using Movilway.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{

    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetUsersAgent)]
    public class GetUsersAgentProvider : AKinacuProvider
    {

        public GetUsersAgentProvider()
        {

        }

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetUsersAgentProvider));
        protected override ILogger ProviderLogger { get { return logger; } }


        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {

            try
            {
                GetUsersAgentRequestBody request = requestObject as GetUsersAgentRequestBody;


                GetUsersAgentResponseBody response = null;

                logger.InfoLow(() => TagValue.New().Message("[API] " + base.LOG_PREFIX + "[GetUsersAgentProvider] Start").Tag("User").Value(request.AuthenticationData.Username).Tag("Agent").Value(request.Agent));


                response = new GetUsersAgentResponseBody()
                {
                    ResponseCode = 0,
                    ResponseMessage = "Exito",
                    TransactionID = 0,
                    Users = Utils.GetUsers(request.Agent,request.AuthenticationData.Username,request.DeviceType,request.Onlychildren,request.ShowAccess)
                };

                logger.InfoLow(() => TagValue.New().Message("[API] " + base.LOG_PREFIX + "[GetUsersAgentProvider] End").Tag("User").Value(request.AuthenticationData.Username).Tag("Agent").Value(request.Agent));
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