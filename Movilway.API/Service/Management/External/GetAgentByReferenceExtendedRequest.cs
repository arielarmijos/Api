using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Management.External
{

    [MessageContract(IsWrapped = false)]
    public class GetAgentByReferenceExtendedRequest
    {
        [MessageBodyMember(Name = "GetAgentByReferenceExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public GetAgentByReferenceExtendedRequestBody Request { set; get; }

        public GetAgentByReferenceExtendedRequest()
        {
            Request = new GetAgentByReferenceExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetAgentByReferenceExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public String Reference { set; get; }
    }
}