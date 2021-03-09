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
    public class CreateMoviPinRequest : IMovilwayApiRequestWrapper<CreateMoviPinRequestBody>
    {
        [MessageBodyMember(Name = "CreateMoviPinRequest")]
        public CreateMoviPinRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "CreateMoviPinRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class CreateMoviPinRequestBody : ASecuredFinancialApiRequest
    {
        [Loggable]
        [DataMember(Order = 2, IsRequired = false)]
        public String Recipient { set; get; }

        [Loggable]
        [DataMember(Order = 3, IsRequired = false)]
        public String ProductId { set; get; }
    }
}