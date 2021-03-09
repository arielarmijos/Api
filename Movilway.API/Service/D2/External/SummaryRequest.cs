using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.External
{

    [MessageContract(IsWrapped = false)]
    public class SummaryRequest
    {
        [MessageBodyMember(Name = "SummaryRequest", Namespace = "http://api.movilway.net/schema")]
        public SummaryRequestBody Request { set; get; }

        public SummaryRequest()
        {
            Request = new SummaryRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class SummaryRequestBody : SessionApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public int WalletType { set; get; }

        [DataMember(Order = 3, IsRequired = true)]
        public String Target{ set; get; }

        [DataMember(Order = 4, IsRequired = true)]
        public DateTime StartDate { set; get; }

        [DataMember(Order = 5, IsRequired = true)]
        public DateTime EndDate { set; get; }
    }
}