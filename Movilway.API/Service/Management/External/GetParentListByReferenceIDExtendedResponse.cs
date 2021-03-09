using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.External;

namespace Movilway.API.Service.Management.External
{
    [MessageContract(IsWrapped = false)]
    public class GetParentListByReferenceIDExtendedResponse
    {
        [MessageBodyMember(Name = "GetParentListByReferenceIDExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public GetParentListByReferenceIDExtendedResponseBody Response { set; get; }

        public GetParentListByReferenceIDExtendedResponse()
        {
            Response = new GetParentListByReferenceIDExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetParentListByReferenceIDExtendedResponseBody : ApiResponse
    {
        [DataMember]
        public ParentList ParentList { set; get; }
    }
}