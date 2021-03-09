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
    public class GetSalesSummaryDashBoardResponse : IMovilwayApiResponseWrapper<GetSalesSummaryDashBoardResponseBody>
    {
        [MessageBodyMember(Name = "GetSalesSummaryDashBoardResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetSalesSummaryDashBoardResponseBody Response { set; get; }

        public GetSalesSummaryDashBoardResponse()
        {
            Response = new GetSalesSummaryDashBoardResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetSalesSummaryDashBoardResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetSalesSummaryDashBoardResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {       
        [Loggable]
        [DataMember(Order = 1)]
        public SalesSummaryDashBoardItems SalesSummariesDashBoard { set; get; }

        [Loggable]
        [DataMember(Order = 2)]
        public DateTime SummaryDate { set; get; }

        [Loggable]
        [DataMember(Order = 3)]
        public Decimal TotalSales { set; get; }

        [Loggable]
        [DataMember(Order = 4)]
        public Decimal AverageCommission { set; get; }

        [Loggable]
        [DataMember(Order = 5)]
        public Decimal TotalRevenue { set; get; }
        
        public GetSalesSummaryDashBoardResponseBody()
        {
            SalesSummariesDashBoard = new SalesSummaryDashBoardItems();
        }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    [CollectionDataContract(Name = "SalesSummariesDashBoard", Namespace = "http://api.movilway.net/schema/extended", ItemName = "SalesSummaryDashBoard")]
    public class SalesSummaryDashBoardItems : List<SalesSummaryDashItem>
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
    [DataContract(Name = "SalesSummaryDashBoard", Namespace = "http://api.movilway.net/schema/extended")]
    public class SalesSummaryDashItem
    {
        [Loggable]
        [DataMember(Order = 1)]
        public int NumWeek { set; get; }

        [Loggable]
        [DataMember(Order = 2)]
        public DateTime InitDate { set; get; }

        [Loggable]
        [DataMember(Order = 3)]
        public DateTime EndDate { set; get; }

        [Loggable]
        [DataMember(Order = 4)]
        public int ProductId { set; get; }

        [Loggable]
        [DataMember(Order = 5)]
        public String ProductName { set; get; }
                      
        [Loggable]
        [DataMember(Order = 6)]
        public Decimal TotalAmount { set; get; }

        [Loggable]
        [DataMember(Order = 7)]
        public int TransactionCount { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    [Loggable]
    [DataContract(Name = "CommisssionSalesSummaryDashBoard", Namespace = "http://api.movilway.net/schema/extended")]
    public class CommissionSalesSummaryDashItem
    {
        [Loggable]
        [DataMember(Order = 1)]
        public Decimal TotalCommiSales { set; get; }

        [Loggable]
        [DataMember(Order = 2)]
        public string CommiPercentage { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}