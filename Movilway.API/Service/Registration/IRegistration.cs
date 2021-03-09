using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Movilway.API.Service.Registration;
using Movilway.API.Service.Registration.External;

namespace Movilway.API.Service.Registration
{
    [ServiceContract(Namespace = "http://api.movilway.net/schema")]
    public interface IRegistration:ISessionServiceContract, ILoginServiceContract, IChangePinContract
    {
        [OperationContract]
        RegisterAgentResponse RegisterAgent(RegisterAgentRequest externalRequest);

        [OperationContract]
        RegisterAgentBulkResponse RegisterAgentBulk(RegisterAgentBulkRequest externalRequest);

        [OperationContract]
        GetProvincesResponse GetProvinces(GetProvincesRequest externalRequest);

        [OperationContract]
        GetProvinceCitiesResponse GetProvinceCities(GetProvinceCitiesRequest externalRequest);
    }

}
