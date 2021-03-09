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
    public class NotiwayResponse : IMovilwayApiResponseWrapper<NotiwayResponseBody>
    {
        [MessageBodyMember(Name = "NotiwayResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public NotiwayResponseBody Response { set; get; }

        public NotiwayResponse()
        {
            Response = new NotiwayResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "NotiwayResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class NotiwayResponseBody : AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public Decimal StockBalance { set; get; }

    }
}