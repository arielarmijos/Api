using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Management.External
{

    [MessageContract(IsWrapped = false)]
    public class GetAgentListExtendedRequest
    {
        [MessageBodyMember(Name = "GetAgentListExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public GetAgentListExtendedRequestBody Request { set; get; }

        public GetAgentListExtendedRequest()
        {
            Request = new GetAgentListExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetAgentListExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

    }
}