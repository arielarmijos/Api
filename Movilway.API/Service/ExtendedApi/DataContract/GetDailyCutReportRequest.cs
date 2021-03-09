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
    public class GetDailyCutReportRequest : IMovilwayApiRequestWrapper<GetDailyCutReportRequestBody>
    {
        [MessageBodyMember(Name = "GetDailyCutReportRequest")]
        public GetDailyCutReportRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetDailyCutReportRequestBody", Namespace="http://api.movilway.net/schema/extended")]
    public class GetDailyCutReportRequestBody : ASecuredApiRequest
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