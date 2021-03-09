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
    public class GetValuesRequest : IMovilwayApiRequestWrapper<GetValuesRequestBody>
    {
        [MessageBodyMember(Name = "GetValuesRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public GetValuesRequestBody Request { set; get; }

    }

    [Loggable]
    [DataContract(Name = "GetValuesRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetValuesRequestBody : ASecuredApiRequest
    {
        /*
        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public int Id { set; get; }
        */
        [Loggable]
        [DataMember(Order = 2, IsRequired = false)]
        public bool? ExtendedValues { set; get; }
    }
}