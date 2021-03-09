using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.Logging;

namespace Movilway.API.Service.ExtendedApi.Provider.IBank
{
    [ServiceProviderImpl(Platform=ApiTargetPlatform.iBank, ServiceName=ApiServiceName.GetExternalBalance)]
    public class GetExternalBalanceProvider : AIBankProvider, IServiceProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetExternalBalanceProvider));

        public IMovilwayApiResponse PerformOperation(IMovilwayApiRequest requestObject)
        {
            var request = (GetExternalBalanceRequestBody)requestObject;
            var response = new GetExternalBalanceResponseBody();


            var ibankClient = GetProviderForEntity(request.TargetEntity);

            logger.InfoHigh("Comienza el GetExternalBalance");

            // Need to get the national ID:
            var agentInfo = new ServiceExecutionDelegator<GetAgentInfoResponseBody, GetAgentInfoRequestBody>().ResolveRequest(
                new GetAgentInfoRequestBody()
                {
                    AuthenticationData = request.AuthenticationData,
                    Agent = request.Agent,
                    DeviceType = request.DeviceType
                }, ApiTargetPlatform.Kinacu, ApiServiceName.GetAgentInfo);


            logger.InfoHigh("Se ejecutó GetAgentInfo, el resultado fue: ResponseCode " + agentInfo.ResponseCode + ", NationalId " + agentInfo.AgentInfo.NationalID);

            var getExternalBalanceResponse = ibankClient.GetBalance(new API.IBank.GetBalanceRequest
            {
                Agent = request.Agent,
                AgentNationalID = agentInfo.AgentInfo.NationalID
            });

            logger.InfoHigh("Se ejecutó GetBalance en IBank, el resultado fue: ResponseCode " + getExternalBalanceResponse.ResponseCode + ", Balance disponible " + getExternalBalanceResponse.AvailableAmount);

            if (getExternalBalanceResponse != null)
            {
                response.ResponseCode = getExternalBalanceResponse.Result ? 0 : 99;
                response.ResponseMessage = "External Reference: " + getExternalBalanceResponse.ExternalTransactionReference;
                response.ApprovedAmount = getExternalBalanceResponse.ApprovedAmount;
                response.ConsumedAmount = getExternalBalanceResponse.ConsumedAmount;
                response.AvailableAmount = getExternalBalanceResponse.AvailableAmount;
                response.NextPaymentAmount = getExternalBalanceResponse.NextPaymentAmount;
                response.NextPaymentDate = getExternalBalanceResponse.NextPaymentDate;

                //if (getExternalBalanceResponse.Balances != null && getExternalBalanceResponse.Balances.Any())
                //{
                //response.Balances = new ExternalBalances();
                //foreach (var balance in getExternalBalanceResponse.Balances)
                //{
                //    response.Balances.Add(new ExternalBalanceDetail()
                //    {
                //        Amount = balance.Amount,
                //        Description = balance.Type
                //    });
                //}
                //}
            }
            else
            {
                response.ResponseCode = 99;
                response.ResponseMessage = "Su transaccion no pudo ser procesada";
            }
            return response;
        }
    }
}