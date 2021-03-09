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
    public class AddTrustedDeviceResponse : IMovilwayApiResponseWrapper<AddTrustedDeviceResponseBody>
    {
        [MessageBodyMember(Name = "AddTrustedDeviceResponseBody")]
        public AddTrustedDeviceResponseBody Response
        {
            get;
            set;
        }

        public AddTrustedDeviceResponse()
        {
            Response = new AddTrustedDeviceResponseBody();
        }

    }


    [Loggable]
    [DataContract(Name = "AddTrustedDeviceResponseBody", Namespace = "http://api.movilway.net/schema/extended")]
    public class AddTrustedDeviceResponseBody : AGenericApiResponse
    {
        [DataMember]
        public bool Result
        {
            get;
            set;
        }
    }
}