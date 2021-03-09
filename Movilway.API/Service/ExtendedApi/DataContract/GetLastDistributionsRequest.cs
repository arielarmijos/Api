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
    public class GetLastDistributionsRequest : IMovilwayApiRequestWrapper<GetLastDistributionsRequestBody>
    {
        [MessageBodyMember(Name = "GetLastDistributionsRequest")]
        public GetLastDistributionsRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetLastDistributionsRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class GetLastDistributionsRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String Agent { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public int Count { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}