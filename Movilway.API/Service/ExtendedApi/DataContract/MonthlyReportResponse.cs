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
    public class MonthlyReportResponse : IMovilwayApiResponseWrapper<MonthlyReportResponseBody>
    {
        [MessageBodyMember(Name = "MonthlyReportResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public MonthlyReportResponseBody Response { set; get; }


        public MonthlyReportResponse()
        {
            Response = new MonthlyReportResponseBody();
        }
    }


    [Loggable]
    [DataContract(Name = "MonthlyReportResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class MonthlyReportResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {

    }
}