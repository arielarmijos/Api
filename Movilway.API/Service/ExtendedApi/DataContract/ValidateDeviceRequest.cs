using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Web;
using Movilway.Logging.Attribute;


namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class ValidateDeviceRequest : IMovilwayApiRequestWrapper<ValidateDeviceRequestBody>
    {
        [MessageBodyMember(Name = "ValidateDeviceRequestBody")]
        public ValidateDeviceRequestBody Request
        {
            get;
            set;
        }

        public ValidateDeviceRequest()
        {
        }

    }

    [Loggable]
    [DataContract(Name = "ValidateDeviceRequestBody", Namespace = "http://api.movilway.net/schema/extended")]
    public class ValidateDeviceRequestBody : ASecuredApiRequest
    {
        [DataMember(IsRequired= true)]
        public String TokenToValidate { get; set; }

        [DataMember]
        public int UserId { get; set; }
    }
}