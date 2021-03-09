using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetPendingDistributionsResponse : IMovilwayApiResponseWrapper<GetPendingDistributionsResponseBody>
    {
        [MessageBodyMember(Name = "GetPendingDistributionsResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetPendingDistributionsResponseBody Response { set; get; }

        public GetPendingDistributionsResponse()
        {
            Response = new GetPendingDistributionsResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetPendingDistributionsResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetPendingDistributionsResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3)]
        public DistributionList Distributions { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}