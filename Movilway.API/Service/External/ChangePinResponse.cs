using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.External
{
    [MessageContract(IsWrapped = false)]
    public class ChangePinResponse
    {
        [MessageBodyMember(Name = "ChangePinResponse", Namespace = "http://api.movilway.net/schema")]
        public ChangePinResponseBody Response { set; get; }

        public ChangePinResponse()
        {
            Response = new ChangePinResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class ChangePinResponseBody : ApiResponse
    {

    }
}