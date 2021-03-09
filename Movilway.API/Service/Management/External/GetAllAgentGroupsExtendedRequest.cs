using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Management.External
{

    [MessageContract(IsWrapped = false)]
    public class GetAllAgentGroupsExtendedRequest
    {
        [MessageBodyMember(Name = "GetAllAgentGroupsExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public GetAllAgentGroupsExtendedRequestBody Request { set; get; }

        public GetAllAgentGroupsExtendedRequest()
        {
            Request = new GetAllAgentGroupsExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetAllAgentGroupsExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }
    }
}