using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Management.External
{

    [MessageContract(IsWrapped = false)]
    public class GetAllAgentGroupsRequest
    {
        [MessageBodyMember(Name = "GetAllAgentGroupsRequest", Namespace = "http://api.movilway.net/schema")]
        public GetAllAgentGroupsRequestBody Request { set; get; }

        public GetAllAgentGroupsRequest()
        {
            Request = new GetAllAgentGroupsRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetAllAgentGroupsRequestBody : SessionApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }
    }
}