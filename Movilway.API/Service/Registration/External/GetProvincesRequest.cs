using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Registration.External
{

    [MessageContract(IsWrapped = false)]
    public class GetProvincesRequest
    {
        [MessageBodyMember(Name = "GetProvincesRequest", Namespace = "http://api.movilway.net/schema")]
        public GetProvincesRequestBody Request { set; get; }

        public GetProvincesRequest()
        {
            Request = new GetProvincesRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetProvincesRequestBody : SessionApiRequest
    {

    }
}