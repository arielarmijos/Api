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
    public class GetTransactionsInRangeRequest : IMovilwayApiRequestWrapper<GetTransactionsInRangeRequestBody>
    {
        [MessageBodyMember(Name = "GetTransactionsInRangeRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public GetTransactionsInRangeRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetTransactionsInRangeRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetTransactionsInRangeRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public String Agent { set; get; }

        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public DateTime StartDate { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public DateTime EndDate { set; get; }

        [Loggable]
        [DataMember(Order =5, IsRequired = true)]
        public string TransactionType { set; get; }
    }
}