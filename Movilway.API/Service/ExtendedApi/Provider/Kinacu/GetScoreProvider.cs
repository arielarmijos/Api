using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using Movilway.API.KinacuWebService;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetScore)]
    public class GetScoreProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetScoreProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new GetScoreResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0,
                    Score = new Score()
                };

            GetScoreRequestBody request = requestObject as GetScoreRequestBody;
            GetScoreResponseBody response = null;

            if (sessionID.Equals("0"))
                return response = new GetScoreResponseBody()
                {
                    ResponseCode = 99,
                    ResponseMessage = "Error de login",
                    TransactionID = new Random().Next(100000, 999999),
                    Score = null
                };

            string agentId = "";
            bool isChild = false;

            if (!String.IsNullOrEmpty(request.Agent))
            {
                var getChildListResponse = new ServiceExecutionDelegator
                                                <GetChildListResponseBody, GetChildListRequestBody>().ResolveRequest(
                                                    new GetChildListRequestBody()
                                                    {
                                                        AuthenticationData = new AuthenticationData()
                                                        {
                                                            Username = request.AuthenticationData.Username,
                                                            Password = request.AuthenticationData.Password
                                                        },
                                                        DeviceType = request.DeviceType,
                                                        Agent = request.AuthenticationData.Username,
                                                        Platform = "1"
                                                    }, ApiTargetPlatform.Kinacu, ApiServiceName.GetChildList);

                if (getChildListResponse.ChildList != null && getChildListResponse.ChildList.Count(ch => ch.Agent == request.Agent) > 0)
                {
                    agentId = request.Agent;
                    isChild = true;
                }
            }

            if (!isChild)
                agentId = new Provider.IBank.Utils().GetAgentId(request.AuthenticationData.Username).ToString();

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[GetScoreProvider] [SEND-DATA] getScoreParameters {agentId=" + agentId + "}");

            response = new GetScoreResponseBody()
            {
                ResponseCode = 0,
                ResponseMessage = "exito",
                TransactionID = new Random().Next(100000,999999),
                Score = Utils.GetScore(agentId)
            };

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[GetScoreProvider] [RECV-DATA] getScoreResult {result{Score{BranchId=" + response.Score.BranchId + ",BranchName=" + response.Score.BranchName +
                                    ",LotteryType=" + response.Score.LotteryType + ",Confirmed=" + response.Score.Confirmed + ",YearToDate=" + response.Score.YearToDate + ",CurrentMonth=" + response.Score.CurrentMonth +
                                    ",Standard=" + response.Score.Standard + ",Bonus=" + response.Score.Bonus + ",Behaviour=" + response.Score.Behaviour + ",NetworkStandard=" + response.Score.NetworkStandard +
                                    ",NetworkBonus=" + response.Score.NetworkBonus + ",NetworkBehaviour=" + response.Score.NetworkBehaviour + ",Questionnaire=" + response.Score.Questionnaire + "}}}");

            return response;
        }
    }
}