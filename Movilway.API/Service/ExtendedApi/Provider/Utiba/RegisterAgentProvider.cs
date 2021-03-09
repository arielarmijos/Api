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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.RegisterAgent)]
    public class RegisterAgentProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(RegisterAgentProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {

            RegisterAgentRequestBody request = requestObject as RegisterAgentRequestBody;
            RegisterAgentResponseBody response = new RegisterAgentResponseBody();

            if (AgentRegistrationUtils.AddAgentToFile(request.Agent, 1))
            {
                response.ResponseCode = 0;
                response.ResponseMessage = "Agente agregado satisfactoriamente al archivo de carga";
            }
            else
            {
                response.ResponseCode = 1;
                response.ResponseMessage = "Ocurrio un error tratando de agregar el agente al archivo de carga";
            }
            return (response);
        }
    }
}