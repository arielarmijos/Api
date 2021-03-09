using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetParentListResponse : IMovilwayApiResponseWrapper<GetParentListResponseBody>
    {
        [MessageBodyMember(Name = "GetParentListResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetParentListResponseBody Response { set; get; }

        public GetParentListResponse()
        {
            Response = new GetParentListResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetParentListResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetParentListResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [DataMember(Order = 1, EmitDefaultValue=false)]
        public ParentList ParentList { set; get; }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended",
        ItemName = "Parent")]
    public class ParentList : List<BasicAgentInfo> { }
}