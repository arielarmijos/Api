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
    public class GetBalanceResponse:IMovilwayApiResponseWrapper<GetBalanceResponseBody>
    {
        [MessageBodyMember(Name = "GetBalanceResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetBalanceResponseBody Response { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetBalanceResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetBalanceResponseBody : AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3, EmitDefaultValue=false)]
        public Decimal? StockBalance { set; get; }

        [Loggable]
        [DataMember(Order = 4, EmitDefaultValue = false)]
        public Decimal? WalletBalance { set; get; }

        [Loggable]
        [DataMember(Order = 5, EmitDefaultValue = false)]
        public Decimal? PointsBalance { set; get; }

        [Loggable]
        [DataMember(Order = 6, EmitDefaultValue = false)]
        public Decimal? DebtBalance { set; get; }

        [Loggable]
        [DataMember(Order = 7, IsRequired = false, EmitDefaultValue = false)]
        public Decimal? CheckingAccountBalance { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
            //return "GetBalanceResponse {ResponseCode=" + base.ResponseCode + ",ResponseMessage=" + base.ResponseMessage + ",TransactionId=" + base.TransactionID +
            //            ",StockBalance=" + this.StockBalance + ",WalletBalance=" + this.WalletBalance + ",PointsBalance=" + this.PointsBalance + ",DebtBalance=" + this.DebtBalance + "}";
        }
    }
}