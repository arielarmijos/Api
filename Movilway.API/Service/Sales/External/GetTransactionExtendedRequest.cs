using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Sales.External
{

    [MessageContract(IsWrapped = false)]
    public class GetTransactionExtendedRequest
    {
        [MessageBodyMember(Name = "GetTransactionExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public GetTransactionExtendedRequestBody Request { set; get; }

        public GetTransactionExtendedRequest()
        {
            Request = new GetTransactionExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetTransactionExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public GetTransactionRequestParameterType ParameterType { set; get; }

        [DataMember(Order = 3, IsRequired = true)]
        public String Parameter { set; get; }
    }
}