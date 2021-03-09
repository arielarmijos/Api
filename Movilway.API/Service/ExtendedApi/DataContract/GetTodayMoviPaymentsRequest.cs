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
    public class GetTodayMoviPaymentsRequest : IMovilwayApiRequestWrapper<GetTodayMoviPaymentsRequestBody>
    {
        [MessageBodyMember(Name = "GetTodayMoviPaymentsRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public GetTodayMoviPaymentsRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetTodayMoviPaymentsRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetTodayMoviPaymentsRequestBody : AUnsecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public String Agent { set; get; }

        [Loggable]
        [DataMember(Order = 3, IsRequired = false)]
        public Pagination Pagination { set; get; }
    }

    [Loggable]
    [DataContract(Name = "Pagination", Namespace = "http://api.movilway.net/schema/extended")]
    public class Pagination
    {
        [Loggable]
        [DataMember(Order = 1, IsRequired = true)]
        public int PageNumber { set; get; }

        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public int PageSize { set; get; }
    }
}