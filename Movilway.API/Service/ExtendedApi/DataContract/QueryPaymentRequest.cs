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
    [MessageContract(IsWrapped = false)]
    public class QueryPaymentRequest : IMovilwayApiRequestWrapper<QueryPaymentRequestBody>
    {
        [MessageBodyMember(Name = "QueryPaymentRequest")]
        public QueryPaymentRequestBody Request { set; get; }
    }


    [Loggable]
    [DataContract(Name = "QueryPaymentRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class QueryPaymentRequestBody : ASecuredFinancialApiRequest
    {
        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public String MNO { set; get; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = true)]
        public String Recipient { set; get; }

        [Loggable]
        [DataMember(Order = 7, IsRequired = false, EmitDefaultValue = false)]
        public WalletType WalletType { set; get; }

        [Loggable]
        [DataMember(Order = 8, IsRequired = false)]
        public String TerminalID { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }

    }


}