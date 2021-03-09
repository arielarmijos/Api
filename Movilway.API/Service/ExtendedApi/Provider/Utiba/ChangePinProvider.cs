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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.ChangePin)]
    public class ChangePinProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(ChangePinProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            ChangePinRequestBody request = requestObject as ChangePinRequestBody;
            ChangePinResponseBody response = null;

            logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[ChangePinProvider] [SEND-DATA] pinRequest {sessionId=" + sessionID + ",device_type=" + request.DeviceType +
                                                            ",new_pin=******,pin=******,initiator=" + request.Agent + "}");

            string currentPinHash = Utils.GenerateHash(sessionID, request.Agent, request.OldPin);
            //string newPinHash = Utils.GenerateHash(sessionID, internalRequest.Initiator, internalRequest.CurrentPin);
            pinResponse pinChangeUtiba = utibaClientProxy.pin(new pin() 
            { 
                pinRequest = new pinRequestType() 
                { 
                    sessionid = sessionID, 
                    device_type = request.DeviceType, 
                    new_pin = request.NewPin,
                    pin = currentPinHash, 
                    initiator = request.Agent
                } 
            });

            logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[ChangePinProvider] [RECV-DATA] pinResponse {transid=" + pinChangeUtiba.pinReturn.transid +
                                                                                            ",result=" + pinChangeUtiba.pinReturn.result +
                                                                                            ",result_namespace=" + pinChangeUtiba.pinReturn.result_namespace +
                                                                                            ",result_message=" + pinChangeUtiba.pinReturn.result_message + "}");

            if (pinChangeUtiba != null)
            {
                response = new ChangePinResponseBody()
                {
                    ResponseCode = Utils.BuildResponseCode(pinChangeUtiba.pinReturn.result, pinChangeUtiba.pinReturn.result_namespace),
                    ResponseMessage = pinChangeUtiba.pinReturn.result_message,
                    TransactionID = pinChangeUtiba.pinReturn.transid
                };
            }
            return (response);
        }
    }
}