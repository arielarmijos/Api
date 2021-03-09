using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.External
{

    [MessageContract(IsWrapped = false)]
    public class BalanceRequest
    {
        [MessageBodyMember(Name = "BalanceRequest", Namespace = "http://api.movilway.net/schema")]
        public BalanceRequestBody Request { set; get; }

        public BalanceRequest()
        {
            Request = new BalanceRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class BalanceRequestBody:SessionApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }
        
    }
}