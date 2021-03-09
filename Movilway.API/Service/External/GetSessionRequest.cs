using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.External
{
    [MessageContract(IsWrapped = false)]
    public class GetSessionRequest
    {
        [MessageBodyMember(Name = "GetSessionRequest", Namespace = "http://api.movilway.net/schema")]
        public GetSessionRequestBody Request { set; get; }

        public GetSessionRequest()
        {
            Request = new GetSessionRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetSessionRequestBody
    {
        [DataMember(Order=0, IsRequired=true)]
        public String User { set; get; }

        [DataMember(Order = 1, IsRequired = true)]
        public String Password { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public int DeviceType { set; get; }
    }
        
}