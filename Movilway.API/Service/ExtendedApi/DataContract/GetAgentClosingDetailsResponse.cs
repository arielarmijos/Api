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
    public class GetAgentClosingDetailsResponse : IMovilwayApiResponseWrapper<GetAgentClosingDetailsResponseBody>
    {
        [MessageBodyMember(Name = "GetAgentClosingDetailsResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetAgentClosingDetailsResponseBody Response { set; get; }

        public GetAgentClosingDetailsResponse()
        {
            Response = new GetAgentClosingDetailsResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetAgentClosingDetailsResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetAgentClosingDetailsResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3)]
        public string AgentID { get; set; }

        [Loggable]
        [DataMember(Order = 4)]
        public string AgentName { get; set; }

        [Loggable]
        [DataMember(Order = 5, EmitDefaultValue = false)]
        public DateTime? ReportDateTime { get; set; }

        [Loggable]
        [DataMember(Order = 6)]
        public ReportClosingDetails reportData { get; set; }

        public GetAgentClosingDetailsResponseBody()
        {
            reportData = new ReportClosingDetails();
        }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }


    [Loggable]
    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "ItemReport")]
    public class ReportClosingDetails : List<ReportClosingDetailsData>
    {

        public ReportClosingDetails()
            : base()
        {

        }


        public ReportClosingDetails(IEnumerable<ReportClosingDetailsData> collection)
            : base(collection)
        {

        }
    }

    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class ReportClosingDetailsData
    {

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public int ProductID
        {
            get;
            set;
        }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string ProductCode
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 3, EmitDefaultValue = false)]
        public string ProductDescription
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 4, EmitDefaultValue = false)]
        public decimal Pins
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
}
