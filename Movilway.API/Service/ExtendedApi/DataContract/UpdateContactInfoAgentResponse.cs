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
    public class UpdateContactInfoAgentResponse : IMovilwayApiResponseWrapper<UpdateContactInfoAgentResponseBody>
    {
        [MessageBodyMember(Name = "UpdateContactInfoAgentResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public UpdateContactInfoAgentResponseBody Response { set; get; }

        public UpdateContactInfoAgentResponse()
        {
            Response = new UpdateContactInfoAgentResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "UpdateContactInfoAgentResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class UpdateContactInfoAgentResponseBody : Common.AGenericApiResponse
    {

    }
}