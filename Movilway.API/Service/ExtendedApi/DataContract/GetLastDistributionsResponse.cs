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
    public class GetLastDistributionsResponse : IMovilwayApiResponseWrapper<GetLastDistributionsResponseBody>
    {
        [MessageBodyMember(Name = "GetLastDistributionsResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetLastDistributionsResponseBody Response { set; get; }

        public GetLastDistributionsResponse()
        {
            Response = new GetLastDistributionsResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetLastDistributionsResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetLastDistributionsResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3)]
        public DistributionList Distributions { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "Distribution")]
    public class DistributionList : List<DistributionSummary>
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            //foreach (var item in this)
            //    sb.Append(Utils.logFormat(item) + ",");
            //if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            sb.Append(String.Concat(this.GetType().Name, " Count ", this.Count));
            return sb.ToString();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class DistributionSummary
    {
        [Loggable]
        [DataMember(Order = 0)]
        public long OriginalDistributionID { set; get; }

        [Loggable]
        [DataMember(Order = 1)]
        public long TargetAgentID { set; get; }

        //[Loggable]
        //[DataMember(Order = 2)]
        //public long UserID { set; get; }
        
        [Loggable]
        [DataMember(Order = 3)]
        public string TargetAgentName { set; get; }

        [Loggable]
        [DataMember(Order = 4)]
        public DateTime RequestTime { set; get; }

        [Loggable]
        [DataMember(Order = 5)]
        public Decimal Amount { set; get; }

        [Loggable]
        [DataMember(Order = 6)]
        public bool HasDeposit { set; get; }

        [Loggable]
        [DataMember(Order = 7)]
        public string BankName { set; get; }

        [Loggable]
        [DataMember(Order = 8)]
        public int AccountId { set; get; }

        [Loggable]
        [DataMember(Order = 9)]
        public string AccountNumber { set; get; }

        [Loggable]
        [DataMember(Order = 10)]
        public string ReferenceNumber { set; get; }

        [Loggable]
        [DataMember(Order = 11)]
        public DateTime DepositDate { set; get; }

        [Loggable]
        [DataMember(Order = 12)]
        public string Status { set; get; }

        [Loggable]
        [DataMember(Order = 13)]
        public DateTime ApprobationDate { set; get; }

        [Loggable]
        [DataMember(Order = 14)]
        public string DepositComment { set; get; }

        [Loggable]
        [DataMember(Order = 15)]
        public string ApprovalComment { set; get; }
    }
}