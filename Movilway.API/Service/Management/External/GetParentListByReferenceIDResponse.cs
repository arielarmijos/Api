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
    public class GetParentListByReferenceIDResponse
    {
        [MessageBodyMember(Name = "GetParentListByReferenceIDResponse", Namespace = "http://api.movilway.net/schema")]
        public GetParentListByReferenceIDResponseBody Response { set; get; }

        public GetParentListByReferenceIDResponse()
        {
            Response = new GetParentListByReferenceIDResponseBody();
        }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema")]
    public class ParentList : List<AgentInfo> { }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetParentListByReferenceIDResponseBody : ApiResponse
    {
        [DataMember]
        public ParentList ParentList { set; get; }
    }
}