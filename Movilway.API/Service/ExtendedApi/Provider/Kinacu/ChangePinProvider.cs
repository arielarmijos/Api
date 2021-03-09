using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using Movilway.API.KinacuWebService;
using Movilway.API.KinacuManagementWebService;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.ChangePin)]
    public class ChangePinProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(ChangePinProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0") && !sessionID.Equals("1013"))
                return new ChangePinResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0
                };

            ChangePinRequestBody request = requestObject as ChangePinRequestBody;
            ChangePinResponseBody response = null;
            string message;

            ManagementInterface managementWS = new ManagementInterface();

            var session = new ServiceExecutionDelegator<GetSessionResponseBody, GetSessionRequestBody>().ResolveRequest(
               new GetSessionRequestBody()
               {
                   Username = request.AuthenticationData.Username,
                   Password = request.AuthenticationData.Password,
                   DeviceType = request.DeviceType
               }, ApiTargetPlatform.Kinacu, ApiServiceName.GetSession);

            if (session.ResponseCode == 1013 || session.ResponseCode == 10133 || session.ResponseCode == 1017)
            {
                logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[ChangePinProvider] [SEND-DATA] ChangeUserPasswordByAccessIdParameters {AccessId=" + request.AuthenticationData.Username + ",OldPassword=******,NewPassword=******,AccessType=" + request.DeviceType + "}");

                bool result = managementWS.ChangeUserPasswordByAccessId(request.AuthenticationData.Username, request.OldPin, request.NewPin, request.DeviceType, out message);

                logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[ChangePinProvider] [RECV-DATA] ChangeUserPasswordByAccessIdResult {response=" + result + ",message=" + message + "}");

                response = new ChangePinResponseBody()
                {
                    ResponseCode = (result ? 0 : 99),
                    ResponseMessage = (result ? "exito" : message),
                    TransactionID = new Random().Next(100000, 999999)
                };
            }
            else
            {
                logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[ChangePinProvider] [SEND-DATA] ChangeUserPasswordParameters {UserId=" + sessionID + ",OldPassword=******,NewPassword=******}");

                bool result = managementWS.ChangeUserPassword(int.Parse(sessionID), request.OldPin, request.NewPin, out message);

                logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[ChangePinProvider] [RECV-DATA] ChangeUserPasswordResult {response=" + result + ",message=" + message + "}");

                response = new ChangePinResponseBody()
                {
                    ResponseCode = (result ? 0 : 99),
                    ResponseMessage = (result ? "exito" : message),
                    TransactionID = new Random().Next(100000, 999999)
                };
            }
            return (response);
        }
    }
}