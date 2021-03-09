using Movilway.API.Core;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;
namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped=false)]
    public class SetStateTrustedDeviceRequest : IMovilwayApiRequestWrapper<SetStateTrustedDeviceRequestBody>
    {
        [MessageBodyMember(Name = "SetStateTrustedDeviceRequestBody", Namespace = "http://api.movilway.net/schema/extended")]
        public SetStateTrustedDeviceRequestBody Request
        {
            get;
            set;
        }

        public SetStateTrustedDeviceRequest()
        {
            Request = new SetStateTrustedDeviceRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public enum DeviceStatus
    {
        [EnumMember]
        Active = cons.DEVICE_ACTIVE,
        [EnumMember]
        Delete = cons.DEVICE_DELETE,
        [EnumMember]
        Temporal = cons.DEVICE_TEMPORAL
    }

    [DataContract(Name = "SetStateTrustedDeviceRequestBody", Namespace = "http://api.movilway.net/schema/extended")]
    public class SetStateTrustedDeviceRequestBody:ASecuredApiRequest
    {
        //[DataMember]
        //public TrustedDevice TrustedDevice { get; set; }

        [DataMember]
        public long DeviceID { get; set; }

        [DataMember]
        public short Status { get; set; }



        public SetStateTrustedDeviceRequestBody()
        {
            
        }
    } 
}