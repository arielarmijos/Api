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
    public class BuyExtendedResponse
    {
        [MessageBodyMember(Name = "BuyExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public BuyExtendedResponseBody Response { set; get; }

        public BuyExtendedResponse()
        {
            Response = new BuyExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class BuyExtendedResponseBody : ApiResponse
    {

    }
}