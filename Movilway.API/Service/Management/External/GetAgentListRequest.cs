using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Management.External
{

    [MessageContract(IsWrapped = false)]
    public class GetAgentListRequest
    {
        [MessageBodyMember(Name = "GetAgentListRequest", Namespace = "http://api.movilway.net/schema")]
        public GetAgentListRequestBody Request { set; get; }

        public GetAgentListRequest()
        {
            Request = new GetAgentListRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetAgentListRequestBody : SessionApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

    }
}