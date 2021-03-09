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
    public class GetChildListByReferenceResponse
    {
        [MessageBodyMember(Name = "GetChildListByReferenceResponse", Namespace = "http://api.movilway.net/schema")]
        public GetChildListByReferenceResponseBody Response { set; get; }

        public GetChildListByReferenceResponse()
        {
            Response = new GetChildListByReferenceResponseBody();
        }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema")]
    public class ChildList : List<AgentInfo> { }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetChildListByReferenceResponseBody : ApiResponse
    {
        [DataMember]
        public ChildList ParentList { set; get; }
    }
}