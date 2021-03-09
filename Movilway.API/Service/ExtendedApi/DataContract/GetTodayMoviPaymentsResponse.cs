using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetTodayMoviPaymentsResponse : IMovilwayApiResponseWrapper<GetTodayMoviPaymentsResponseBody>
    {
        [MessageBodyMember(Name = "GetTodayMoviPaymentsResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetTodayMoviPaymentsResponseBody Response { set; get; }

        public GetTodayMoviPaymentsResponse()
        {
            Response = new GetTodayMoviPaymentsResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetTodayMoviPaymentsResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetTodayMoviPaymentsResponseBody : AGenericApiResponse
    {
        [DataMember(Order = 1, IsRequired = false)]
        public int RecordCount { set; get; }

        [DataMember(Order =2, IsRequired = false)]
        public MoviPaymentList MoviPayments { set; get; }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "MoviPayment")]
    public class MoviPaymentList : List<MoviPaymentDetail> { }

    [DataContract(Namespace = "http://api.movilway.net/schema/extended", Name = "MoviPaymentDetail")]
    public class MoviPaymentDetail
    {

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public String ExternalTransactionReference { set; get; }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public DateTime Date { set; get; }

        [DataMember(Order = 3, EmitDefaultValue = false)]
        public Decimal Amount { set; get; }

        [DataMember(Order = 4, EmitDefaultValue = false)]
        public int? ResponseCode { set; get; }
    }
}