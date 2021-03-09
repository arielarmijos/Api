using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Sales.External
{

    [MessageContract(IsWrapped = false)]
    public class SaleStateByExternalIDRequest
    {
        [MessageBodyMember(Name = "SaleStateExternalIDRequest", Namespace = "http://api.movilway.net/schema")]
        public SaleStateByExternalIDRequestBody Request { set; get; }

        public SaleStateByExternalIDRequest()
        {
            Request = new SaleStateByExternalIDRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class SaleStateByExternalIDRequestBody
    {
        [DataMember(Order = 1, IsRequired = true)]
        public String UserID { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public String ExternalID { set; get; }
    }
}