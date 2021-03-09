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
    public class BankListResponse
    {
        [MessageBodyMember(Name = "BankListResponse", Namespace = "http://api.movilway.net/schema")]
        public BankListResponseBody Response { set; get; }

        public BankListResponse()
        {
            Response = new BankListResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class BankListResponseBody : ApiResponse
    {
        [DataMember(Order = 1)]
        public BankList BankList { set; get; }
    }
}