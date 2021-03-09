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
    public class GetNewAgentsReportRequest : IMovilwayApiRequestWrapper<GetNewAgentsReportRequestBody>
    {
        [MessageBodyMember(Name = "GetNewAgentsReportRequest")]
        public GetNewAgentsReportRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetNewAgentsReportRequestBody", Namespace="http://api.movilway.net/schema/extended")]
    public class GetNewAgentsReportRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String Agent { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public DateTime DateMin { set; get; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public DateTime DateMax { set; get; }

       // [Loggable]
       // [DataMember(Order = 6, IsRequired = true)]
       // public int Page { set; get; }
       //
       // [Loggable]
       // [DataMember(Order = 7, IsRequired = true)]
       // public int PageSize { set; get; }
       //
        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}