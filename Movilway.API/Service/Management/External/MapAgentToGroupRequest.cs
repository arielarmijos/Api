using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Management.External
{

    [MessageContract(IsWrapped = false)]
    public class MapAgentToGroupRequest
    {
        [MessageBodyMember(Name = "MapAgentToGroupRequest", Namespace = "http://api.movilway.net/schema")]
        public MapAgentToGroupRequestBody Request { set; get; }

        public MapAgentToGroupRequest()
        {
            Request = new MapAgentToGroupRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class MapAgentToGroupRequestBody : SessionApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public String Reference { set; get; }

        [DataMember(Order = 3, IsRequired = true)]
        public int GroupID { set; get; }
    }
}