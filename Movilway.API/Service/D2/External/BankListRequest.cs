using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.External
{

    [MessageContract(IsWrapped = false)]
    public class BankListRequest
    {
        [MessageBodyMember(Name = "BankListRequest", Namespace = "http://api.movilway.net/schema")]
        public BankListRequestBody Request { set; get; }

        public BankListRequest()
        {
            Request = new BankListRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class BankListRequestBody : SessionApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public String AgentReference { set; get; }
    }


}