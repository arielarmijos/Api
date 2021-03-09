using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.Logging.Attribute;
using Movilway.API.Service.ExtendedApi.DataContract.Common;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class CreateMoviPinResponse : IMovilwayApiResponseWrapper<CreateMoviPinResponseBody>
    {
        [MessageBodyMember(Name = "CreateMoviPinResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public CreateMoviPinResponseBody Response { set; get; }

        public CreateMoviPinResponse()
        {
            Response = new CreateMoviPinResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "CreateMoviPinResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class CreateMoviPinResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AFinancialApiResponse
    {
        [DataMember(Order = 0, EmitDefaultValue=false)]
        public String MoviPin { set; get; }

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public DateTime ExpiryDate { set; get; }
    }
}