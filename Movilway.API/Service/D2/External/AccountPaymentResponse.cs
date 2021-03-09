using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.External;

namespace Movilway.API.Service.D2.External
{
    [MessageContract(IsWrapped = false)]
    public class AccountPaymentResponse
    {
        [MessageBodyMember(Name = "AccountPaymentResponse", Namespace = "http://api.movilway.net/schema")]
        public AccountPaymentResponseBody Response { set; get; }

        public AccountPaymentResponse()
        {
            Response = new AccountPaymentResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class AccountPaymentResponseBody : ApiResponse
    {

    }
}