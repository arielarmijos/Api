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
    public class RegisterAgentBulkResponse : IMovilwayApiResponseWrapper<RegisterAgentBulkResponseBody>
    {
        [MessageBodyMember(Name = "RegisterAgentBulkResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public RegisterAgentBulkResponseBody Response { set; get; }

        public RegisterAgentBulkResponse()
        {
            Response = new RegisterAgentBulkResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "RegisterAgentBulkResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class RegisterAgentBulkResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {

    }
}