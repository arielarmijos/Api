using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Registration.External
{
    [MessageContract(IsWrapped = false)]
    public class GetProvinceCitiesRequest
    {
        [MessageBodyMember(Name = "GetProvinceCitiesRequest", Namespace = "http://api.movilway.net/schema")]
        public GetProvinceCitiesRequestBody Request { set; get; }

        public GetProvinceCitiesRequest()
        {
            Request = new GetProvinceCitiesRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetProvinceCitiesRequestBody : SessionApiRequest
    {
        [DataMember(IsRequired=true)]
        public int ProvinceID { set; get; }
    }
}