using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Management.External
{

    [MessageContract(IsWrapped = false)]
    public class GetParentListByReferenceIDRequest
    {
        [MessageBodyMember(Name = "GetParentListByReferenceIDRequest", Namespace = "http://api.movilway.net/schema")]
        public GetParentListByReferenceIDRequestBody Request { set; get; }

        public GetParentListByReferenceIDRequest()
        {
            Request = new GetParentListByReferenceIDRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetParentListByReferenceIDRequestBody : SessionApiRequest
    {
        [DataMember(Order = 0, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 1, IsRequired = true)]
        public long AgentID { set; get; }

    }
}