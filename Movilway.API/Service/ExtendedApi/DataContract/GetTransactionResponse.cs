using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetTransactionResponse : IMovilwayApiResponseWrapper<GetTransactionResponseBody>
    {
        [MessageBodyMember(Name = "GetTransactionResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetTransactionResponseBody Response { set; get; }

        public GetTransactionResponse()
        {
            Response = new GetTransactionResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetTransactionResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetTransactionResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3, EmitDefaultValue=false)]
        public int? TransactionResult { set; get; }

        [DataMember(Order = 4, EmitDefaultValue = false)]
        public String OriginalTransactionId { set; get; }

        [Loggable]
        [DataMember(Order = 5, EmitDefaultValue=false)]
        public String Recipient { set; get; }

        [Loggable]
        [DataMember(Order = 6, EmitDefaultValue = false)]
        public Decimal Amount { set; get; }

        [Loggable]
        [DataMember(Order = 7, EmitDefaultValue = false)]
        public DateTime TransactionDate { set; get; }

        [Loggable]
        [DataMember(Order = 8, EmitDefaultValue = false)]
        public String TransactionType { set; get; }

        [Loggable]
        [DataMember(Order = 9, EmitDefaultValue = false)]
        public String Initiator { set; get; }

        [Loggable]
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public String Debtor { set; get; }

        [Loggable]
        [DataMember(Order = 11, EmitDefaultValue = false)]
        public String Creditor { set; get; }

        //SE INCLUYE EL CAMPO PARA MOSTRAR EL STOCK DESPUES DE LA RECUPERACION DE TRANSACCION
        [Loggable]
        [DataMember(Order = 12, EmitDefaultValue = false, IsRequired = false)]
        public Decimal? StockBalance { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
            //return "GetTransactionResponse {ReponseCode=" + base.ResponseCode + ",ResponseMessage=" + base.ResponseMessage + ",TransactionID=" + base.TransactionID +
            //            ",TransactionResult=" + this.TransactionResult + ",OriginalTransactionId=" + this.OriginalTransactionId + ",Recipient=" + this.Recipient +
            //            ",Amount=" + this.Amount + ",TransactionDate=" + this.TransactionDate + ",TransactionType=" + this.TransactionType +
            //            ",Initiator=" + this.Initiator + ",Debtor=" + this.Debtor + ",Creditor=" + this.Creditor + "}";
        }
    }
}