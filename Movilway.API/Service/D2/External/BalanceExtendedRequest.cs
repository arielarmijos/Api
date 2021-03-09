using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.External
{

    [MessageContract(IsWrapped = false)]
    public class BalanceExtendedRequest
    {
        [MessageBodyMember(Name = "BalanceExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public BalanceExtendedRequestBody Request { set; get; }

        public BalanceExtendedRequest()
        {
            Request = new BalanceExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class BalanceExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }
        
    }
}