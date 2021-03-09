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
    public class ValidateDeviceResponse : IMovilwayApiResponseWrapper<ValidateDeviceResponseBody>
    {
        [MessageBodyMember(Name = "ValidateDeviceResponseBody")]
        public ValidateDeviceResponseBody Response
        {
            get;
            set;
        }

        public ValidateDeviceResponse()
        {
            Response = new ValidateDeviceResponseBody();
        }

    }


    [Loggable]
    [DataContract(Name = "ValidateDeviceResponseBody", Namespace = "http://api.movilway.net/schema/extended")]
    public class ValidateDeviceResponseBody : AGenericApiResponse
    {
        [DataMember]
        public bool IsValid
        {
            get;
            set;
        }


        [DataMember]
        public string LastVersion
        {
            get;
            set;
        }

    }
}