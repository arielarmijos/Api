using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Management.External
{

    [MessageContract(IsWrapped = false)]
    public class GetAgentByReferenceRequest
    {
        [MessageBodyMember(Name = "GetAgentByReferenceRequest", Namespace = "http://api.movilway.net/schema")]
        public GetAgentByReferenceRequestBody Request { set; get; }

        public GetAgentByReferenceRequest()
        {
            Request = new GetAgentByReferenceRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetAgentByReferenceRequestBody : SessionApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public String Reference { set; get; }
    }
}