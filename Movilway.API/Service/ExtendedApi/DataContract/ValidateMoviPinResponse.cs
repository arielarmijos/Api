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
    public class ValidateMoviPinResponse : IMovilwayApiResponseWrapper<ValidateMoviPinResponseBody>
    {
        [MessageBodyMember(Name = "ValidateMoviPinResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public ValidateMoviPinResponseBody Response { set; get; }

        public ValidateMoviPinResponse()
        {
            Response = new ValidateMoviPinResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "ValidateMoviPinResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class ValidateMoviPinResponseBody : AGenericApiResponse
    {
        [DataMember(Order = 1, IsRequired = true)]
        public MoviPins MoviPins { set; get; }
    }
}