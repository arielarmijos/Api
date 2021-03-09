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
    public class GetCutStockRequest : IMovilwayApiRequestWrapper<GetCutStockRequestBody>
    {
        [MessageBodyMember(Name = "GetCutStockRequest")]
        public GetCutStockRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetCutStockRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class GetCutStockRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public DateTime Date { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}