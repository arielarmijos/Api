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
    public class GetAgentClosingsResponse : IMovilwayApiResponseWrapper<GetAgentClosingsResponseBody>
    {
        [MessageBodyMember(Name = "GetAgentClosingsResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetAgentClosingsResponseBody Response { set; get; }

        public GetAgentClosingsResponse()
        {
            Response = new GetAgentClosingsResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetAgentClosingsResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetAgentClosingsResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3)]
        public ReportClosingsList reportData { get; set; }

        public GetAgentClosingsResponseBody()
        {
            reportData = new ReportClosingsList();
        }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }


    [Loggable]
    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "ItemReport")]
    public class ReportClosingsList : List<ReportClosingData>
    {

        public ReportClosingsList()
            : base()
        {

        }


        public ReportClosingsList(IEnumerable<ReportClosingData> collection)
            : base(collection)
        {

        }
    }

    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class ReportClosingData
    {

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public long ID
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 2, EmitDefaultValue = false)]
        public int AgentID
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 3, EmitDefaultValue = false)]
        public DateTime ClosingDate
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 4, EmitDefaultValue = false)]
        public int MinTransactionID
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 5, EmitDefaultValue = false)]
        public int MaxTransactionID
        {
            get;
            set;
        }

        [DataMember(Order = 6, EmitDefaultValue = false)]
        public String Type
        {
            get;
            set;
        }
    }
}
