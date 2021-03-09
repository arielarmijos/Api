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
    public class BuyStockResponse:IMovilwayApiResponseWrapper<BuyStockResponseBody>
    {
        [MessageBodyMember(Name = "BuyStockResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public BuyStockResponseBody Response { set; get; }

        public BuyStockResponse()
        {
            Response = new BuyStockResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "BuyStockResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class BuyStockResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AFinancialApiResponse
    {
        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}