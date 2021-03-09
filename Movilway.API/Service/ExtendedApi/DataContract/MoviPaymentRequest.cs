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
    public class MoviPaymentRequest : IMovilwayApiRequestWrapper<MoviPaymentRequestBody>
    {
        [MessageBodyMember(Name = "MoviPaymentRequest")]
        public MoviPaymentRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "MoviPaymentRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class MoviPaymentRequestBody : ASecuredFinancialApiRequest
    {
        [DataMember(Order = 3, IsRequired = true)]
        public String MoviPin { set; get; }

        [DataMember(Order = 4, IsRequired = false)]
        public decimal DollarAmount { set; get; }

        [DataMember(Order = 5, IsRequired = false)]
        public decimal ExchangeRate { get; set; }

        [DataMember(Order = 6, IsRequired = false)]
        public String ProductId { set; get; }
    }
}