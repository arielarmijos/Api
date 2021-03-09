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
    public class GetSalesSummaryResponse : IMovilwayApiResponseWrapper<GetSalesSummaryResponseBody>
    {
        [MessageBodyMember(Name = "GetSalesSummaryResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetSalesSummaryResponseBody Response { set; get; }

        public GetSalesSummaryResponse()
        {
            Response = new GetSalesSummaryResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetSalesSummaryResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetSalesSummaryResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 1)]
        public DateTime SummaryDate { set; get; }

        [Loggable]
        [DataMember(Order = 2)]
        public SummaryItems Summaries { set; get; }

        [Loggable]
        [DataMember(Order = 3, IsRequired=false)]
        public decimal InitialAmount { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = false)]
        public decimal FinalAmount { set; get; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = false)]
        public string BranchName { set; get; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = false)]
        public string DatePrinter { set; get; }


        public GetSalesSummaryResponseBody()
        {
            Summaries = new SummaryItems();
        }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    [CollectionDataContract(Name = "Summaries", Namespace = "http://api.movilway.net/schema/extended", ItemName = "Summary")]
    public class SummaryItems : List<SummaryItem>
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
    [DataContract(Name = "Summary", Namespace = "http://api.movilway.net/schema/extended")]
    public class SummaryItem
    {
        [Loggable]
        [DataMember(Order = 1)]
        public String TransactionType { set; get; }

        [Loggable]
        [DataMember(Order = 2)]
        public int TransactionCount { set; get; }

        [Loggable]
        [DataMember(Order = 3)]
        public Decimal TotalAmount { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}