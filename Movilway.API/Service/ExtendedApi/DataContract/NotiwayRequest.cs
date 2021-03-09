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
    public class NotiwayRequest : IMovilwayApiRequestWrapper<NotiwayRequestBody>
    {
        [MessageBodyMember(Name = "NotiwayRequest")]
        public NotiwayRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "NotiwayRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class NotiwayRequestBody : ASecuredApiRequest
    {

    }
}