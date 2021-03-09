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
    public class CashInResponse
    {
        [MessageBodyMember(Name = "CashInResponse", Namespace = "http://api.movilway.net/schema")]
        public CashInResponseBody Response { set; get; }

        public CashInResponse()
        {
            Response = new CashInResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class CashInResponseBody : ApiResponse
    {

    }
}