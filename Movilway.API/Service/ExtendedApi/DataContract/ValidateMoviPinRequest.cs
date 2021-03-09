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
    public class ValidateMoviPinRequest : IMovilwayApiRequestWrapper<ValidateMoviPinRequestBody>
    {
        [MessageBodyMember(Name = "ValidateMoviPinRequest")]
        public ValidateMoviPinRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "ValidateMoviPinRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class ValidateMoviPinRequestBody : AUnsecuredApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public MoviPins MoviPins { set; get; }
    }

    [CollectionDataContract(Name = "MoviPins", ItemName = "MoviPin", Namespace = "http://api.movilway.net/schema/extended")]
    public class MoviPins:List<MoviPinDetails>
    {
    }

    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class MoviPinDetails
    {
        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public string Number { set; get; }

        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public Boolean? IsValid { set; get; }

        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public decimal? RemainingAmount { set; get; }

        [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
        public decimal? InitialAmount { set; get; }

        [DataMember(Order = 5, IsRequired = false, EmitDefaultValue = false)]
        public string Agent { set; get; }

        [DataMember(Order = 6, IsRequired = false, EmitDefaultValue = false)]
        public DateTime ExpiryDate { set; get; }
    }
}