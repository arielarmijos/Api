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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.RegisterAgentBulk)]
    public class RegisterAgentBulkProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(RegisterAgentBulkProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {

            RegisterAgentBulkRequestBody request = requestObject as RegisterAgentBulkRequestBody;
            RegisterAgentBulkResponseBody response = new RegisterAgentBulkResponseBody();

            if (request.Agents != null && request.Agents.Count > 0)
            {
                int counter = 1;
                Boolean failed = false;
                foreach (AgentDetails agentInfo in request.Agents)
                {
                    if(!AgentRegistrationUtils.AddAgentToFile(agentInfo, request.Agents.Count))
                        failed = true;
                    counter++;
                }

                RegisterAgentBulkResponseBody responseBody = new RegisterAgentBulkResponseBody();
                if (failed)
                {
                    responseBody.ResponseCode = 1;
                    responseBody.ResponseMessage = "Algunos de los registros en el archivo fallaron, por favor contacte a soporte";
                }
                else
                {
                    responseBody.ResponseCode = 0;
                    responseBody.ResponseMessage = "Su peticion ha sido procesada";
                }
            }
            else
            {
                response = new RegisterAgentBulkResponseBody()
                {
                    ResponseCode = 1,
                    ResponseMessage = "Su peticion falló por no contener agentes, por favor verifique los datos e intente de nuevo",
                    TransactionID = 0
                };
            }
            return (response);
        }
    }
}