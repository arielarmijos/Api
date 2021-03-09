using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Sales.External
{

    [MessageContract(IsWrapped = false)]
    public class SaleStateByExternalIDExtendedRequest
    {
        [MessageBodyMember(Name = "SaleStateByExternalIDExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public SaleStateByExternalIDExtendedRequestBody Request { set; get; }

        public SaleStateByExternalIDExtendedRequest()
        {
            Request = new SaleStateByExternalIDExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class SaleStateByExternalIDExtendedRequestBody
    {
        [DataMember(Order = 0, IsRequired = true)]
        public String AccessId { set; get; }

        [DataMember(Order = 1, IsRequired = true)]
        public String Password { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public int AccessType { set; get; }

        [DataMember(Order = 3, IsRequired = true)]
        public String ExternalID { set; get; }
    }
}