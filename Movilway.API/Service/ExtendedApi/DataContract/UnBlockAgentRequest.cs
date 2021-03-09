using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class UnBlockAgentRequest : IMovilwayApiRequestWrapper<UnBlockAgentRequestBody>
    {
        [MessageBodyMember(Name = "UnBlockAgentRequest")]
        public UnBlockAgentRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "UnBlockAgentRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class UnBlockAgentRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 1, IsRequired = true)]
        public String Agent { set; get; }
    }
}