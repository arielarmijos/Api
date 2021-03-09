using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Management.External
{

    [MessageContract(IsWrapped = false)]
    public class MapAgentToGroupExtendedRequest
    {
        [MessageBodyMember(Name = "MapAgentToGroupExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public MapAgentToGroupExtendedRequestBody Request { set; get; }

        public MapAgentToGroupExtendedRequest()
        {
            Request = new MapAgentToGroupExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class MapAgentToGroupExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public String Reference { set; get; }

        [DataMember(Order = 3, IsRequired = true)]
        public int GroupID { set; get; }
    }
}