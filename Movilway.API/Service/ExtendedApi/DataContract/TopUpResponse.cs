using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class TopUpResponse : IMovilwayApiResponseWrapper<TopUpResponseBody>
    {
        [MessageBodyMember(Name = "TopUpResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public TopUpResponseBody Response { set; get; }

        public TopUpResponse()
        {
            Response = new TopUpResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "TopUpResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class TopUpResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AFinancialApiResponse
    {
        [Loggable]
        [DataMember(Order = 4, IsRequired=false, EmitDefaultValue=false)]
        public String ExternalTransactionReference { set; get; }

        [Loggable]
        [DataMember(Order = 5, EmitDefaultValue = true)]
        public Decimal StockBalance{ set; get; }

        [Loggable]
        [DataMember(Order = 6, EmitDefaultValue = true)]
        public Decimal WalletBalance { set; get; }

        [Loggable]
        [DataMember(Order = 7, EmitDefaultValue = true)]
        public Decimal PointBalance { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
            //return "TopUpResponse {ResponseCode=" + base.ResponseCode + ",ResponseMessage=" + base.ResponseMessage + ",TransactionId=" + base.TransactionID + 
            //            ",Fee=" + base.Fee + ",ExternalTransactionRefernce=" + this.ExternalTransactionReference + ",StockBalance=" + this.StockBalance + "}";
        }
    }
}