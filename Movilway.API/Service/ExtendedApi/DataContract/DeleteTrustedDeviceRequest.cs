using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped= false)]
    public class DeleteTrustedDeviceRequest : IMovilwayApiRequestWrapper<DeleteTrustedDeviceRequestBody>
    {
        [MessageBodyMember(Name = "DeleteTrustedDeviceRequestBody", Namespace = "http://api.movilway.net/schema/extended")]
        public DeleteTrustedDeviceRequestBody Request
        {
            get;
            set;
        }

        public DeleteTrustedDeviceRequest()
        {
            Request = new DeleteTrustedDeviceRequestBody();
        }
    }

    [DataContract(Name = "DeleteTrustedDeviceRequestBody", Namespace = "http://api.movilway.net/schema/extended")]
    public class DeleteTrustedDeviceRequestBody:ASecuredApiRequest                                      
    {
      
        [DataMember(IsRequired = true)]
        public long DeviceID { get; set; }
        
        [DataMember]
        public string Serial { get; set; }

        public DeleteTrustedDeviceRequestBody()
        {
          
        }
    }
}