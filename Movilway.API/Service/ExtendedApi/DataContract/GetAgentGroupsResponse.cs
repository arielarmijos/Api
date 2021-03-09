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
    public class GetAgentGroupsResponse : IMovilwayApiResponseWrapper<GetAgentGroupsResponseBody>
    {
        [MessageBodyMember(Name = "GetAgentGroupsResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetAgentGroupsResponseBody Response { set; get; }

        public GetAgentGroupsResponse()
        {
            Response = new GetAgentGroupsResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetAgentGroupsResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetAgentGroupsResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [DataMember(Order = 1)]
        public GroupList GroupList { set; get; }
    }

    //[DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    //public class GroupInfo
    //{
    //    [DataMember(Order = 1, EmitDefaultValue=false)]
    //    public long GroupID { set; get; }

    //    [DataMember(Order = 2, EmitDefaultValue = false)]
    //    public String Name { set; get; }

    //    [DataMember(Order = 3, EmitDefaultValue = false)]
    //    public int Type { set; get; }

    //    [DataMember(Order = 4, EmitDefaultValue = false)]
    //    public String Category { set; get; }
    //}

    //[CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", 
    //    ItemName="Group")]
    //public class GroupList : List<GroupInfo> { }
}