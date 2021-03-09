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
    public class BuyResponse
    {
        [MessageBodyMember(Name = "BuyResponse", Namespace = "http://api.movilway.net/schema")]
        public BuyResponseBody Response { set; get; }

        public BuyResponse()
        {
            Response = new BuyResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class BuyResponseBody : ApiResponse
    {

    }
}