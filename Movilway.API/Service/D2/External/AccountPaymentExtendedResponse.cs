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
    public class AccountPaymentExtendedResponse
    {
        [MessageBodyMember(Name = "AccountPaymentExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public AccountPaymentExtendedResponseBody Response { set; get; }

        public AccountPaymentExtendedResponse()
        {
            Response = new AccountPaymentExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class AccountPaymentExtendedResponseBody : ApiResponse
    {

    }
}