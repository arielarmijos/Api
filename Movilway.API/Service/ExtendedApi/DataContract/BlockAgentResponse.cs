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
    public class BlockAgentResponse : IMovilwayApiResponseWrapper<BlockAgentResponseBody>
    {
        [MessageBodyMember(Name = "BlockAgentResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public BlockAgentResponseBody Response { set; get; }

        public BlockAgentResponse()
        {
            Response = new BlockAgentResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "BlockAgentResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class BlockAgentResponseBody : Common.AGenericApiResponse
    {

    }
}