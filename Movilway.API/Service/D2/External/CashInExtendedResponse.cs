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
    public class CashInExtendedResponse
    {
        [MessageBodyMember(Name = "CashInExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public CashInExtendedResponseBody Response { set; get; }

        public CashInExtendedResponse()
        {
            Response = new CashInExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class CashInExtendedResponseBody : ApiResponse
    {

    }
}