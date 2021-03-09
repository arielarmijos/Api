using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.External
{
    [MessageContract(IsWrapped = false)]
    public class ChangePinRequest
    {
        [MessageBodyMember(Name = "ChangePinRequest", Namespace = "http://api.movilway.net/schema")]
        public ChangePinRequestBody Request { set; get; }

        public ChangePinRequest()
        {
            Request = new ChangePinRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class ChangePinRequestBody:SessionApiRequest
    {
        [DataMember(Order = 0, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 1, IsRequired = true)]
        public String Initiator { set; get; }

        [DataMember(Order=2, IsRequired=true)]
        public String CurrentPin { set; get; }

        [DataMember(Order = 3, IsRequired = true)]
        public String NewPin { set; get; }

    }
}