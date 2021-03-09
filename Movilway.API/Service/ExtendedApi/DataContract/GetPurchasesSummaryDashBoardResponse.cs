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
    public class GetPurchasesSummaryDashBoardResponse : IMovilwayApiResponseWrapper<GetPurchasesSummaryDashBoardResponseBody>
    {
        [MessageBodyMember(Name = "GetPurchasesSummaryDashBoardResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetPurchasesSummaryDashBoardResponseBody Response { set; get; }

        public GetPurchasesSummaryDashBoardResponse()
        {
            Response = new GetPurchasesSummaryDashBoardResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetPurchasesSummaryDashBoardResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetPurchasesSummaryDashBoardResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {       
        [Loggable]
        [DataMember(Order = 1)]
        public PurchasesSummaryDashBoardItems PurchasesSummariesDashBoard { set; get; }

        [Loggable]
        [DataMember(Order = 2)]
        public DateTime SummaryDate { set; get; }

        [Loggable]
        [DataMember(Order = 3)]
        public Decimal TotalPurchases { set; get; }

        [Loggable]
        [DataMember(Order = 4)]
        public Decimal AverageCommission { set; get; }

        [Loggable]
        [DataMember(Order = 5)]
        public Decimal TotalRevenue { set; get; }
        
        public GetPurchasesSummaryDashBoardResponseBody()
        {
            PurchasesSummariesDashBoard = new PurchasesSummaryDashBoardItems();
        }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    [CollectionDataContract(Name = "PurchasesSummariesDashBoard", Namespace = "http://api.movilway.net/schema/extended", ItemName = "PurchasesSummaryDashBoard")]
    public class PurchasesSummaryDashBoardItems : List<PurchasesSummaryDashItem>
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in this)
            {
                 sb.Append(item.ToString());
            }
            return sb.ToString();
        }
    }

    [Loggable]
    [DataContract(Name = "PurchasesSummaryDashBoard", Namespace = "http://api.movilway.net/schema/extended")]
    public class PurchasesSummaryDashItem
    {
        
        [Loggable]
        [DataMember(Order = 1)]
        public DateTime InitDate { set; get; }

        [Loggable]
        [DataMember(Order = 2)]
        public DateTime EndDate { set; get; }
                                      
        [Loggable]
        [DataMember(Order = 3)]
        public Decimal TotalAmount { set; get; }

        [Loggable]
        [DataMember(Order = 4)]
        public int DepositCount { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    [Loggable]
    [DataContract(Name = "CommissionPurchasesSummaryDashBoard", Namespace = "http://api.movilway.net/schema/extended")]
    public class CommissionPurchasesSummaryDashItem
    {

        [Loggable]
        [DataMember(Order = 1)]
        public Decimal TotalCommiPurchases { set; get; }

        [Loggable]
        [DataMember(Order = 2)]
        public Decimal CommiPercentage { set; get; }

        

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}