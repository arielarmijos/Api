using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Log;

namespace Movilway.API.Service.External
{
    [MessageContract(IsWrapped = false)]
    public class LoginRequest
    {
        [MessageBodyMember(Name = "LoginRequest", Namespace = "http://api.movilway.net/schema")]
        public LoginRequestBody Request { set; get; }

        public LoginRequest()
        {
            Request = new LoginRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class LoginRequestBody
    {
        [DataMember(Order=0, IsRequired=true)]
        public String AccessId { set; get; }

        [NotLoggable]
        [DataMember(Order = 1, IsRequired = true)]
        public String Password { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public int AccessType { set; get; }
    }
        
}