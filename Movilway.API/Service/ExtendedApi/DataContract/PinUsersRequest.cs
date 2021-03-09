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
    public class GetPinUsersRequest : IMovilwayApiRequestWrapper<GetPinUsersRequestBody>
    {
        [MessageBodyMember(Name = "PinUsersRequest")]
        public GetPinUsersRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "PinUsersRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetPinUsersRequestBody : ASecuredApiRequest//, IEquatable<GetPinUsersRequestBody>
    {
        /*
        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public int AgeId { set; get; }*/
    
    }
}