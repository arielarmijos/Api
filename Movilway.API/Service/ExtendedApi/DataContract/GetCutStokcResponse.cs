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
    public class GetCutStockResponse : IMovilwayApiResponseWrapper<GetCutStockResponseBody>
    {
        [MessageBodyMember(Name = "GetCutStockResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetCutStockResponseBody Response { set; get; }

        public GetCutStockResponse()
        {
            Response = new GetCutStockResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetCutStockResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetCutStockResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3)]
        public GetCutStockSummary Summary { get; set; }

        [Loggable]
        [DataMember(Order = 4)]
        public GetCutStockDetails Details { get; set; }

        [Loggable]
        [DataMember(Order = 5)]
        public GetCutStockFooter Footer { get; set; }

        public GetCutStockResponseBody()
        {
            Summary = new GetCutStockSummary();
            Details = new GetCutStockDetails();
            Footer = new GetCutStockFooter();
        }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class GetCutStockSummary
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
    public class GetCutStockDetails : List<GetCutStockDetailsData>
    {

        public GetCutStockDetails()
            : base()
        {

        }


        public GetCutStockDetails(IEnumerable<GetCutStockDetailsData> collection)
            : base(collection)
        {

        }
    }

    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class GetCutStockDetailsData
    {

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public string ProviderCode
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 2)]
        public decimal InitialTotalStock
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 3)]
        public decimal TotalReception
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 4)]
        public decimal TotalDailyCut
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 5)]
        public decimal TotalFinalStock
        {
            get;
            set;
        }


        [DataMember(Order = 6)]
        public string Unit
        {
            get;
            set;
        }

        [DataMember(Order = 7)]
        public DateTime Fecha
        {
            get;
            set;
        }

    }

    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class GetCutStockFooter
    {

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public int Total
        {
            get;
            set;
        }
    }
}
