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
    public class RegenerateMoviPinRequest : IMovilwayApiRequestWrapper<RegenerateMoviPinRequestBody>
    {
        [MessageBodyMember(Name = "RegenerateMoviPinRequest")]
        public RegenerateMoviPinRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "RegenerateMoviPinRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class RegenerateMoviPinRequestBody : AUnsecuredApiRequest
    {
        [DataMember(Order = 2, IsRequired = true)]
        //public MoviPinDetails MoviPin { set; get; }
        public int TransactionNumber { set; get; }
    }
}