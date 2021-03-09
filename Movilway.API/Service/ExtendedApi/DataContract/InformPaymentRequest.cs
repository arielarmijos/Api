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
    public class InformPaymentRequest : IMovilwayApiRequestWrapper<InformPaymentRequestBody>
    {
        [MessageBodyMember(Name = "InformPaymentRequest")]
        public InformPaymentRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "InformPaymentRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class InformPaymentRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public decimal Amount { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public long TargetAgentId { set; get; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public bool HasDeposit { set; get; }

        [Loggable]
        [DataMember(Order = 6)]
        public int AccountId { set; get; }

        //[Loggable]
        //[DataMember(Order = 7)]
        //public String AccountNumber { set; get; }

        [Loggable]
        [DataMember(Order = 8)]
        public String TransactionReference { set; get; }

        [Loggable]
        [DataMember(Order = 9)]
        public DateTime TransactionDate { set; get; }

        [Loggable]
        [DataMember(Order = 10)]
        public int SucursalNumber { set; get; }

        [Loggable]
        [DataMember(Order = 11)]
        public String SucursalName { set; get; }

        [Loggable]
        [DataMember(Order = 12)]
        public String Comment { set; get; }

        [Loggable]
        [DataMember(Order = 13)]
        public bool ImmediatelyDistribute { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}