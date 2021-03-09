using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.Logging.Attribute;
using Movilway.API.Service.ExtendedApi.DataContract.Common;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class SetAgentCloseResponse : IMovilwayApiResponseWrapper<SetAgentCloseResponseBody>
    {
        [MessageBodyMember(Name = "SetAgentCloseResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public SetAgentCloseResponseBody Response { set; get; }

        public SetAgentCloseResponse()
        {
            Response = new SetAgentCloseResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "SetAgentCloseResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class SetAgentCloseResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        public SetAgentCloseResponseBody()
        {

        }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}
