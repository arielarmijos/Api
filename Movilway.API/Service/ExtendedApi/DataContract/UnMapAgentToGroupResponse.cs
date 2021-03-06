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
    public class UnMapAgentToGroupResponse : IMovilwayApiResponseWrapper<UnMapAgentToGroupResponseBody>
    {
        [MessageBodyMember(Name = "UnMapAgentToGroupResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public UnMapAgentToGroupResponseBody Response { set; get; }

        public UnMapAgentToGroupResponse()
        {
            Response = new UnMapAgentToGroupResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "UnMapAgentToGroupResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class UnMapAgentToGroupResponseBody : AGenericApiResponse
    {
        
    }
}