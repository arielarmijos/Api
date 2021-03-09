using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Sales.External
{

    [MessageContract(IsWrapped = false)]
    public class GetTransactionRequest
    {
        [MessageBodyMember(Name = "GetTransactionRequest", Namespace = "http://api.movilway.net/schema")]
        public GetTransactionRequestBody Request { set; get; }

        public GetTransactionRequest()
        {
            Request = new GetTransactionRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetTransactionRequestBody : SessionApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public GetTransactionRequestParameterType ParameterType { set; get; }

        [DataMember(Order = 3, IsRequired = true)]
        public String Parameter { set; get; }
    }
}