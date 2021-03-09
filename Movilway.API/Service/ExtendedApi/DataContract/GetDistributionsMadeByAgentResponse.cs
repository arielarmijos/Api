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
    public class GetDistributionsMadeByAgentResponse : IMovilwayApiResponseWrapper<GetDistributionsMadeByAgentResponseBody>
    {
        [MessageBodyMember(Name = "GetDistributionsMadeByAgentResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetDistributionsMadeByAgentResponseBody Response { set; get; }

        public GetDistributionsMadeByAgentResponse()
        {
            Response = new GetDistributionsMadeByAgentResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetDistributionsMadeByAgentResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetDistributionsMadeByAgentResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3)]
        public DistributionMadeList Distributions { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "Distribution")]
    public class DistributionMadeList : List<DistributionMade>
    {
        public override string ToString()
        {
            return string.Concat(this.GetType().Name, " Count ", this.Count);
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class DistributionMade
    {
        [Loggable]
        [DataMember(Order = 0)]
        public int DistributionId { set; get; }

        [Loggable]
        [DataMember(Order = 1)]
        public int TargetAgentId { set; get; }

        [Loggable]
        [DataMember(Order = 2)]
        public string TargetAgentName { set; get; }

        [Loggable]
        [DataMember(Order = 3)]
        public string TargetAgentLegalNumber { set; get; }

        [Loggable]
        [DataMember(Order = 4)]
        public string TargetAgentPhoneNumber { set; get; }

        [Loggable]
        [DataMember(Order = 5)]
        public string TargetAgentAddress { set; get; }

        [Loggable]
        [DataMember(Order = 6)]
        public decimal Base { set; get; }

        [Loggable]
        [DataMember(Order = 7)]
        public decimal Commission { set; get; }

        [Loggable]
        [DataMember(Order = 8)]
        public float Percentage { set; get; }

        [Loggable]
        [DataMember(Order = 9)]
        public decimal Total { set; get; }

        [Loggable]
        [DataMember(Order = 10)]
        public DateTime ApprovalDate { set; get; }

        [Loggable]
        [DataMember(Order = 11)]
        public int RequestTypeId { set; get; }
    }
}
