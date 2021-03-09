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
    public class ConsolidateMoviPinResponse : IMovilwayApiResponseWrapper<ConsolidateMoviPinResponseBody>
    {
        [MessageBodyMember(Name = "ConsolidateMoviPinResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public ConsolidateMoviPinResponseBody Response { set; get; }

        public ConsolidateMoviPinResponse()
        {
            Response = new ConsolidateMoviPinResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "ConsolidateMoviPinResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class ConsolidateMoviPinResponseBody : AGenericApiResponse
    {
        [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public String ConsolidatedMoviPin { set; get; }

        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public decimal? ConsolidatedAmount { set; get; }

        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public MoviPins MoviPins { set; get; }

        [DataMember(Order = 4, EmitDefaultValue = false)]
        public DateTime ExpiryDate { set; get; }
    }
}