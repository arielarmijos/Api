using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Management.External
{

    [MessageContract(IsWrapped = false)]
    public class GetAgentGroupsRequest
    {
        [MessageBodyMember(Name = "GetAgentGroupsRequest", Namespace = "http://api.movilway.net/schema")]
        public GetAgentGroupsRequestBody Request { set; get; }

        public GetAgentGroupsRequest()
        {
            Request = new GetAgentGroupsRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetAgentGroupsRequestBody : SessionApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public long AgentID { set; get; }
    }
}