using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.External
{
    [MessageContract(IsWrapped = false)]
    public class GetSessionResponse
    {
        [MessageBodyMember(Name = "GetSessionResponse", Namespace = "http://api.movilway.net/schema")]
        public GetSessionResponseBody Response { set; get; }

        public GetSessionResponse()
        {
            Response = new GetSessionResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetSessionResponseBody : ApiResponse
    {
        [DataMember(Order=0)]
        public String SessionID { set; get; }
    }
}