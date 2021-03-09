using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Utiba;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.MapAgentToGroup)]
    public class MapAgentToGroupProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(MapAgentToGroupProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            MapAgentToGroupRequestBody request = requestObject as MapAgentToGroupRequestBody;
            MapAgentToGroupResponseBody response = null;

            mapAgentResponse utibaMapAgentResponse = utibaClientProxy.mapAgent(new mapAgentRequest()
            {
                mapAgentRequestType = new mapAgentRequestType()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType,
                    agid = request.GroupID,
                    agent = request.Agent
                }
            });
            if (utibaMapAgentResponse != null)
            {
                response = new MapAgentToGroupResponseBody()
                {
                    ResponseCode = Utils.BuildResponseCode(utibaMapAgentResponse.mapAgentReturn.result, utibaMapAgentResponse.mapAgentReturn.result_namespace),
                    ResponseMessage = utibaMapAgentResponse.mapAgentReturn.result_message,
                    TransactionID = utibaMapAgentResponse.mapAgentReturn.transid
                };
            }
            return (response);
        }
    }
}