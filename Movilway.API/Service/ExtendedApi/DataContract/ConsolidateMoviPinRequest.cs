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
    [MessageContract(IsWrapped=false)]
    public class ConsolidateMoviPinRequest : IMovilwayApiRequestWrapper<ConsolidateMoviPinRequestBody>
    {
        [MessageBodyMember(Name = "ConsolidateMoviPinRequest")]
        public ConsolidateMoviPinRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "ConsolidateMoviPinRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class ConsolidateMoviPinRequestBody : AUnsecuredApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public MoviPins MoviPins { set; get; }
    }
}