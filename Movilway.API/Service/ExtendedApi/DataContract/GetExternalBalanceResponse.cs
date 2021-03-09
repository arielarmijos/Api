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
    public class GetExternalBalanceResponse : IMovilwayApiResponseWrapper<GetExternalBalanceResponseBody>
    {
        [MessageBodyMember(Name = "GetExternalBalanceResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetExternalBalanceResponseBody Response { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetExternalBalanceResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetExternalBalanceResponseBody : AGenericApiResponse
    {
        //[DataMember(Order = 0, EmitDefaultValue=false)]
        //public ExternalBalances Balances { set; get; }

        //[Loggable]
        //[DataMember(Name = "Available", Order = 1, IsRequired = true, EmitDefaultValue = false)]
        //public Decimal Available { get; set; }

        [Loggable]
        [DataMember(Name = "ApprovedAmount", Order = 2, IsRequired = true, EmitDefaultValue = true)]
        public Decimal ApprovedAmount { get; set; }

        [Loggable]
        [DataMember(Name = "ConsumedAmount", Order = 3, IsRequired = false, EmitDefaultValue = true)]
        public Decimal ConsumedAmount { get; set; }

        [Loggable]
        [DataMember(Name = "AvailableAmount", Order = 4, IsRequired = true, EmitDefaultValue = true)]
        public Decimal AvailableAmount { get; set; }

        [Loggable]
        [DataMember(Name = "NextPaymentAmount", Order = 5, IsRequired = true, EmitDefaultValue = true)]
        public Decimal NextPaymentAmount { get; set; }

        [Loggable]
        [DataMember(Name = "NextPaymentDate", Order = 6, IsRequired = true, EmitDefaultValue = true)]
        public string NextPaymentDate { get; set; }


    }

    [CollectionDataContract(Name = "Balances", Namespace = "http://api.movilway.net/schema/extended", ItemName = "Balance")]
    public class ExternalBalances : List<ExternalBalanceDetail>
    {

    }

    [DataContract(Name = "Balance", Namespace = "http://api.movilway.net/schema/extended")]
    public class ExternalBalanceDetail
    {
        [DataMember(Name = "Description", IsRequired = true, EmitDefaultValue = false)]
        public String Description { set; get; }

        [DataMember(Name = "Amount", IsRequired = true, EmitDefaultValue = true)]
        public decimal Amount { set; get; }
    }

}