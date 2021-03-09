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
    public class GetChildListByReferenceExtendedResponse
    {
        [MessageBodyMember(Name = "GetChildListByReferenceExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public GetChildListByReferenceExtendedResponseBody Response { set; get; }

        public GetChildListByReferenceExtendedResponse()
        {
            Response = new GetChildListByReferenceExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetChildListByReferenceExtendedResponseBody : ApiResponse
    {
        [DataMember]
        public ChildList ParentList { set; get; }
    }
}