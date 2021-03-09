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
    public class MapAgentToGroupResponse : IMovilwayApiResponseWrapper<MapAgentToGroupResponseBody>
    {
        [MessageBodyMember(Name = "MapAgentToGroupResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public MapAgentToGroupResponseBody Response { set; get; }

        public MapAgentToGroupResponse()
        {
            Response = new MapAgentToGroupResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "MapAgentToGroupResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class MapAgentToGroupResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        
    }
}