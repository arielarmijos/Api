using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.Logging;
using Movilway.API.Utiba;
using System.Configuration;
using Movilway.API.Config;
using Movilway.API.Service.ExtendedApi.Provider.Utiba;
using Movilway.API.IBank;
 
namespace Movilway.API.Service.ExtendedApi.Provider.IBank
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.iBank, ServiceName = ApiServiceName.ProcessExternalTransaction)]
    public class ProcessExternalTransactionProvider : AIBankProvider, IServiceProvider
    {

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(ProcessExternalTransactionProvider));

        public IMovilwayApiResponse PerformOperation(IMovilwayApiRequest requestObject)
        {
            var request = (ProcessExternalTransactionRequestBody)requestObject;
            var response = new ProcessExternalTransactionResponseBody();

            // Get a session for the configured super user
            var balance = new ServiceExecutionDelegator<GetBalanceResponseBody, GetBalanceRequestBody>().ResolveRequest(
                new GetBalanceRequestBody
                {
                    AuthenticationData = new AuthenticationData() {
                        Username = ConfigurationManager.AppSettings["ProincoLogin"],
                        Password = ConfigurationManager.AppSettings["ProincoPwd"]
                    },
                    DeviceType = int.Parse(ConfigurationManager.AppSettings["ProincoDeviceType"]),
                    Platform = request.Platform
                }, ApiTargetPlatform.Kinacu, ApiServiceName.GetBalance);

            if ((balance.ResponseCode ?? -1) != 0)
            {
                response.ResponseCode = 96;
                response.ResponseMessage = (balance.ResponseCode ?? 0) + "-Su transaccion no pudo ser procesada por problemas al consultar el stock";
                return response;
            }

            if (balance.StockBalance < request.Amount)
            {
                response.ResponseCode = 97;
                response.ResponseMessage = "Su transaccion no pudo ser procesada por falta de stock";
                return response;
            }

            var ibankClient = GetProviderForEntity(request.TargetEntity);

            logger.InfoLow("Comienza el ProcessExternalTransaction");

            string myAgent = new Utils().GetAgentPdv(request.Agent);
            // Need to get the national ID:
            var agentInfo = new ServiceExecutionDelegator<GetAgentInfoResponseBody, GetAgentInfoRequestBody>().ResolveRequest(
                new GetAgentInfoRequestBody()
                {
                    AuthenticationData = request.AuthenticationData,
                    Agent = request.Agent,
                    DeviceType = request.DeviceType
                }, ApiTargetPlatform.Kinacu, ApiServiceName.GetAgentInfo);

            logger.InfoLow("Params: " + request.Agent + " - " + agentInfo.AgentInfo.NationalID + " - " + request.Amount + " - " + request.ExternalTransactionReference);

            var petr = new API.IBank.ProcessTransactionRequest
            {
                Agent = request.Agent,
                AgentNationalID = agentInfo.AgentInfo.NationalID,
                Amount = request.Amount,
                TransactionType = "transfer", //request.TransactionType,
                TransactionReference = request.ExternalTransactionReference
            };

            if (request.AdditionalData != null && request.AdditionalData.Any())
            {
                petr.AdditionalData = new API.IBank.AdditionalData();
                foreach (var item in request.AdditionalData)
                {
                    petr.AdditionalData.Add(item.Key, item.Value);
                }
            }

            logger.InfoLow("request " + petr.Amount + " - " + petr.TransactionReference);

            ProcessTransactionResponse processExternalTransactionResponse = new ProcessTransactionResponse();
            try
            {
                processExternalTransactionResponse = ibankClient.ProcessTransaction(petr);
            }
            catch (Exception ex)
            {
                logger.InfoLow("EX " + ex.Message + " - " + ex.StackTrace);
            }
            logger.InfoLow("response " + processExternalTransactionResponse.ResponseCode + " - " + processExternalTransactionResponse.ResponseMessage);

            if (processExternalTransactionResponse != null)
            {
                if (int.Parse(processExternalTransactionResponse.ResponseCode) == 1)
                {
                    var config = ConfigurationManager.GetSection("Movilway.API.Config") as ApiConfiguration;
                    var adjustmentsUser = config.ManagementUsers["adjustmentsAgent"];

                    string myAutorization = "", myMessage = "", myResponseCode = "";

                    var externalTransferResponse = new Utils().Externaltransfer(decimal.Parse(agentInfo.AgentInfo.BranchID.ToString()), ConfigurationManager.AppSettings["AccountProinco"], int.Parse(ConfigurationManager.AppSettings["ProincoUser"]), double.Parse(request.Amount.ToString()), request.ExternalTransactionReference, DateTime.Now, ref myAutorization, ref myMessage, ref myResponseCode);

                    if (myResponseCode == "00")
                    {
                        response.ResponseCode = 0;
                        response.ResponseMessage = myMessage;
                        response.ExternalTransactionReference = myAutorization;
                    }
                    else
                    {
                        response.ResponseCode = int.Parse(myResponseCode);
                        response.ResponseMessage = myMessage;
                        response.ExternalTransactionReference = myAutorization;
                    }

                }
                else
                {
                    response.ResponseCode = int.Parse(processExternalTransactionResponse.ResponseCode);
                    response.ResponseMessage = processExternalTransactionResponse.ResponseMessage;
                    response.ExternalTransactionReference = processExternalTransactionResponse.ExternalTransactionReference;
                }
                
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