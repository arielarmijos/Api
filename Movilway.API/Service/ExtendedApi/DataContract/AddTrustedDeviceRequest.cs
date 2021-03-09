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
    public class AddTrustedDeviceRequest : IMovilwayApiRequestWrapper<AddTrustedDeviceRequestBody>
    {
        [MessageBodyMember(Name = "AddTrustedDeviceRequestBody")]
        public AddTrustedDeviceRequestBody Request
        {
            get;
            set;
        }

        public AddTrustedDeviceRequest()
        {

        }
    }

    [Loggable]
    [DataContract(Name = "AddTrustedDeviceRequestBody", Namespace = "http://api.movilway.net/schema/extended")]
    public class AddTrustedDeviceRequestBody : ASecuredApiRequest
    {
        /// <summary>
        /// Informacion segura acerca del dispositivo
        /// </summary>
        [DataMember]
        public String InfoTokken { get; set; }
    }
}