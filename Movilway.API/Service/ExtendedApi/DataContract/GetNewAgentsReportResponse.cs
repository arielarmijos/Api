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
    public class GetNewAgentsReportResponse : IMovilwayApiResponseWrapper<GetNewAgentsReportResponseBody>
    {
        [MessageBodyMember(Name = "GetNewAgentsReportResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetNewAgentsReportResponseBody Response { set; get; }

        public GetNewAgentsReportResponse()
        {
            Response = new GetNewAgentsReportResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetNewAgentsReportResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetNewAgentsReportResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3)]
        public NewAgentsSummary Summary { get; set; }

        [Loggable]
        [DataMember(Order = 4)]
        public NewAgentsDetails Details { get; set; }

        public GetNewAgentsReportResponseBody()
        {
            Details = new NewAgentsDetails();
        }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class NewAgentsSummary
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
    public class NewAgentsDetails : List<NewAgentsDetailsData>
    {

        public NewAgentsDetails()
            : base()
        {

        }


        public NewAgentsDetails(IEnumerable<NewAgentsDetailsData> collection)
            : base(collection)
        {

        }
    }

    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class NewAgentsDetailsData
    {

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public string CreatorAgentCode
        {
            get;
            set;
        }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string CreatorAgentName
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 3, EmitDefaultValue = false)]
        public string AgentCode
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 4, EmitDefaultValue = false)]
        public string AgentName
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 5, EmitDefaultValue = false)]
        public string MobileNumber
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 6, EmitDefaultValue = false)]
        public string CutDays
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 7, EmitDefaultValue = false)]
        public decimal CreditLimit
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 8, EmitDefaultValue = false)]
        public DateTime CreationDate
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 9, EmitDefaultValue = false)]
        public string PhoneNumber
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string Email
        {
            get;
            set;
        }
    }
}
