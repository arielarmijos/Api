using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Movilway.API.Utiba;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.BlockAgent)]
    public class BlockAgentProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(BlockAgentProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            var request = requestObject as BlockAgentRequestBody;
            BlockAgentResponseBody response = null;


            var utibaModifyAgentResponse = utibaClientProxy.modify(new modify()
            {
                modifyRequest = new modifyRequestType()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType,
                    agent = request.Agent,
                    status = new modifyRequestTypeStatus()
                    {
                        Suspended = true,
                        SuspendedSpecified = true
                    }
                }
            });
            if (utibaModifyAgentResponse != null)
            {
                response = new BlockAgentResponseBody()
                    {
                        ResponseCode = Utils.BuildResponseCode(utibaModifyAgentResponse.modifyReturn.result, utibaModifyAgentResponse.modifyReturn.result_namespace),
                        ResponseMessage = utibaModifyAgentResponse.modifyReturn.result_message,
                        TransactionID = utibaModifyAgentResponse.modifyReturn.transid
                    };
            }
            return (response);
        }
    }
}