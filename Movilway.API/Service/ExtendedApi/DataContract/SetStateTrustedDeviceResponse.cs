using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped= false) ]
    public class SetStateTrustedDeviceResponse :IMovilwayApiResponseWrapper<SetStateTrustedDeviceResponseBody>
    {
        [MessageBodyMember(Name = "SetStateTrustedDeviceResponseBody", Namespace = "http://api.movilway.net/schema/extended")]
        public SetStateTrustedDeviceResponseBody Response
        {
            get;
            set;
        }

        public SetStateTrustedDeviceResponse()
        {
            Response = new SetStateTrustedDeviceResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "SetStateTrustedDeviceResponseBody", Namespace = "http://api.movilway.net/schema/extended")]
    public class SetStateTrustedDeviceResponseBody:ApiResponse 
    {
         [Loggable]
        [DataMember]
        public bool Result { get; set; }



       

        //public override string ToString()
        //{
        //    return String.Concat("[RESULTADO DESACTIVACION DISPOSITIVO ",Result.ToString(),"]");
        //}
    }
}