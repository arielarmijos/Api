using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Management.External
{

    [MessageContract(IsWrapped = false)]
    public class GetChildListByReferenceExtendedRequest
    {
        [MessageBodyMember(Name = "GetChildListByReferenceExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public GetChildListByReferenceExtendedRequestBody Request { set; get; }

        public GetChildListByReferenceExtendedRequest()
        {
            Request = new GetChildListByReferenceExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetChildListByReferenceExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(Order = 0, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 1, IsRequired = true)]
        public String Reference { set; get; }

    }
}