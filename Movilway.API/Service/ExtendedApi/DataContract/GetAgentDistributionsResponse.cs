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
    public class GetAgentDistributionsResponse : IMovilwayApiResponseWrapper<GetAgentDistributionsResponseBody>
    {
        [MessageBodyMember(Name = "GetAgentDistributionsResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetAgentDistributionsResponseBody Response { set; get; }

        public GetAgentDistributionsResponse()
        {
            Response = new GetAgentDistributionsResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetAgentDistributionsResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetAgentDistributionsResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        public GetAgentDistributionsResponseBody()
        {

        }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}
