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
    public class GetDailyCutReportResponse : IMovilwayApiResponseWrapper<GetDailyCutReportResponseBody>
    {
        [MessageBodyMember(Name = "GetDailyCutReportResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetDailyCutReportResponseBody Response { set; get; }

        public GetDailyCutReportResponse()
        {
            Response = new GetDailyCutReportResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetDailyCutReportResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetDailyCutReportResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3)]
        public DailyCutSummary Summary { get; set; }

        [Loggable]
        [DataMember(Order = 4)]
        public DailyCutDetails Details { get; set; }

        [Loggable]
        [DataMember(Order = 5)]
        public DailyCutFooter Footer { get; set; }

        public GetDailyCutReportResponseBody()
        {
            Details = new DailyCutDetails();
        }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class DailyCutSummary
    {

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public string ReportName
        {
            get;
            set;
        }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public DateTime ReportDateTime
        {
            get;
            set;
        }
    }

    [Loggable]
    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "ItemReport")]
    public class DailyCutDetails : List<DailyCutDetailsData>
    {

        public DailyCutDetails()
            : base()
        {

        }


        public DailyCutDetails(IEnumerable<DailyCutDetailsData> collection)
            : base(collection)
        {

        }
    }

    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class DailyCutDetailsData
    {

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public string Id
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string AgentName
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 3, EmitDefaultValue = false)]
        public string ProductCode
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 4, EmitDefaultValue = false)]
        public decimal Pin
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 5, EmitDefaultValue = false)]
        public string Unit
        {
            get;
            set;
        }
    }

    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class DailyCutFooter
    {

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public int TotalInvoices
        {
            get;
            set;
        }
    }
}
