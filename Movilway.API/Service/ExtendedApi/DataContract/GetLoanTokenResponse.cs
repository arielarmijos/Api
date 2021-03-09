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
    public class GetLoanTokenResponse:IMovilwayApiResponseWrapper<GetLoanTokenResponseBody>
    {
        [MessageBodyMember(Name = "GetLoanTokenResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetLoanTokenResponseBody Response { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetLoanTokenResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetLoanTokenResponseBody : AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3, EmitDefaultValue=false)]
        public string Token { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
            //return "GetBalanceResponse {ResponseCode=" + base.ResponseCode + ",ResponseMessage=" + base.ResponseMessage + ",TransactionId=" + base.TransactionID +
            //            ",StockBalance=" + this.StockBalance + ",WalletBalance=" + this.WalletBalance + ",PointsBalance=" + this.PointsBalance + ",DebtBalance=" + this.DebtBalance + "}";
        }
    }
}