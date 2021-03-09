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
    public class RegenerateMoviPinResponse : IMovilwayApiResponseWrapper<RegenerateMoviPinResponseBody>
    {
        [MessageBodyMember(Name = "RegenerateMoviPinResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public RegenerateMoviPinResponseBody Response { set; get; }

        public RegenerateMoviPinResponse()
        {
            Response = new RegenerateMoviPinResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "RegenerateMoviPinResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class RegenerateMoviPinResponseBody : AGenericApiResponse
    {
        [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public String RegeneratedMoviPin { set; get; }

        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public decimal? RegeneratedAmount { set; get; }

        [DataMember(Order = 3, EmitDefaultValue = false)]
        public DateTime ExpiryDate { set; get; }
    }
}