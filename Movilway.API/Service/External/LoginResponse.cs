using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.External
{
    [MessageContract(IsWrapped = false)]
    public class LoginResponse
    {
        [MessageBodyMember(Name = "LoginResponse", Namespace = "http://api.movilway.net/schema")]
        public LoginResponseBody Response { set; get; }

        public LoginResponse()
        {
            Response = new LoginResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class LoginResponseBody
    {
        [DataMember(Order=0)]
        public String LoginResult { set; get; }

        [DataMember(Order = 1)]
        public String Message { set; get; }
    }
}