using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.External
{

    [MessageContract(IsWrapped = false)]
    public class TopUpExtendedRequest
    {
        [MessageBodyMember(Name = "TopUpExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public TopUpExtendedRequestBody Request { set; get; }

        public TopUpExtendedRequest()
        {
            Request = new TopUpExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class TopUpExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public String MNO { set; get; }

        [DataMember(Order = 3, IsRequired = true)]
        public Decimal Amount { set; get; }

        [DataMember(Order = 4, IsRequired = true)]
        public String Recipient { set; get; }

        [DataMember(Order = 5, IsRequired = true)]
        public String HostTransRef { set; get; }

        [DataMember(Order = 6, IsRequired = true)]
        public String MNODefinedID { set; get; }
    }
}