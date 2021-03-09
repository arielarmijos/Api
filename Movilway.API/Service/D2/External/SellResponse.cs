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
    public class SellResponse
    {
        [MessageBodyMember(Name = "SellResponse", Namespace = "http://api.movilway.net/schema")]
        public SellResponseBody Response { set; get; }

        public SellResponse()
        {
            Response = new SellResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class SellResponseBody : ApiResponse
    {

    }
}