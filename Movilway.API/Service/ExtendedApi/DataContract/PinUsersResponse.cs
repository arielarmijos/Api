using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Runtime.Serialization;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetPinUsersResponse : IMovilwayApiResponseWrapper<GetPinUsersResponseBody>
    {
        [MessageBodyMember(Name = "PinUsersResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetPinUsersResponseBody Response { set; get; }

        public GetPinUsersResponse()
        {
            Response = new GetPinUsersResponseBody();
        }
    }
    [Loggable]
    [DataContract(Name = "PinUsersResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetPinUsersResponseBody : AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public List<String> Users { get; set; }
    }
}