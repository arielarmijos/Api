using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped=false)]
    public class DeleteTrustedDeviceResponse : IMovilwayApiResponseWrapper<DeleteTrustedDeviceResponseBody>
    {
        [MessageBodyMember(Name = "DeleteTrustedDeviceResponseBody", Namespace = "http://api.movilway.net/schema/extended")]
        public DeleteTrustedDeviceResponseBody Response
        {
            get; set;
        }

        public DeleteTrustedDeviceResponse()
        {
            Response = new DeleteTrustedDeviceResponseBody();
        }

    }
    [Loggable]
    [DataContract(Name = "DeleteTrustedDeviceResponseBody", Namespace = "http://api.movilway.net/schema/extended")]
    public class DeleteTrustedDeviceResponseBody : ApiResponse
    {
        [DataMember]
        public bool Result { get; set; }

        //public override string ToString()
        //{
        //    return String.Concat("[CAN DELETE]-[", Result, "]");
        //}

    }
}