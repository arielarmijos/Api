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
    public class SellExtendedResponse
    {
        [MessageBodyMember(Name = "SellExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public SellExtendedResponseBody Response { set; get; }

        public SellExtendedResponse()
        {
            Response = new SellExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class SellExtendedResponseBody : ApiResponse
    {

    }
}