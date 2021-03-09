using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.External;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{

    [MessageContract(IsWrapped = false)]
    public class RegisterAgentBulkRequest:IMovilwayApiRequestWrapper<RegisterAgentBulkRequestBody>
    {
        [MessageBodyMember(Name = "RegisterAgentBulkRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public RegisterAgentBulkRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "RegisterAgentBulkRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class RegisterAgentBulkRequestBody : ASecuredApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public AgentList Agents { set; get; }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended",
        ItemName="Agent", Name="Agents")]
    public class AgentList : List<AgentDetails>
    {

    }
}