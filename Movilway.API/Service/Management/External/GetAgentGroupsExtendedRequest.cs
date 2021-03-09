using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Management.External
{

    [MessageContract(IsWrapped = false)]
    public class GetAgentGroupsExtendedRequest
    {
        [MessageBodyMember(Name = "GetAgentGroupsExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public GetAgentGroupsExtendedRequestBody Request { set; get; }

        public GetAgentGroupsExtendedRequest()
        {
            Request = new GetAgentGroupsExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetAgentGroupsExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public long AgentID { set; get; }
    }
}