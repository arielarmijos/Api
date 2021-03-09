using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped=false)]
    public class RegisterDebtPaymentRequest : IMovilwayApiRequestWrapper<RegisterDebtPaymentRequestBody>
    {
        [MessageBodyMember(Name = "RegisterDebtPaymentRequest")]
        public RegisterDebtPaymentRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "RegisterDebtPaymentRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class RegisterDebtPaymentRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public Decimal Amount { set; get; }

        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String Agent { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = false)]
        public String Description { set; get; }
    }
}