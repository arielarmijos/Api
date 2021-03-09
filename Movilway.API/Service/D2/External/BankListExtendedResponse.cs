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
    public class BankListExtendedResponse
    {
        [MessageBodyMember(Name = "BankListExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public BankListExtendedResponseBody Response { set; get; }

        public BankListExtendedResponse()
        {
            Response = new BankListExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class BankListExtendedResponseBody : ApiResponse
    {
        [DataMember(Order = 1)]
        public BankList BankList { set; get; }
    }
}