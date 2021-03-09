using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Management.External
{

    [MessageContract(IsWrapped = false)]
    public class GetChildListByReferenceRequest
    {
        [MessageBodyMember(Name = "GetChildListByReferenceRequest", Namespace = "http://api.movilway.net/schema")]
        public GetChildListByReferenceRequestBody Request { set; get; }

        public GetChildListByReferenceRequest()
        {
            Request = new GetChildListByReferenceRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetChildListByReferenceRequestBody : SessionApiRequest
    {
        [DataMember(Order = 0, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 1, IsRequired = true)]
        public String Reference { set; get; }

    }
}