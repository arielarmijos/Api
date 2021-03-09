using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Management.External
{

    [MessageContract(IsWrapped = false)]
    public class GetParentListByReferenceIDExtendedRequest
    {
        [MessageBodyMember(Name = "GetParentListByReferenceIDExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public GetParentListByReferenceIDExtendedRequestBody Request { set; get; }

        public GetParentListByReferenceIDExtendedRequest()
        {
            Request = new GetParentListByReferenceIDExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetParentListByReferenceIDExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(Order = 0, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 1, IsRequired = true)]
        public long AgentID { set; get; }

    }
}