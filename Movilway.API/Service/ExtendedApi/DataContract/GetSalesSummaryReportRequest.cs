using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.KinacuWebService;
using Movilway.API.KinacuManagementWebService;
using Movilway.API.KinacuLogisticsWebService;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using Movilway.Logging.Attribute;


namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetSalesSummaryReportRequest : IMovilwayApiRequestWrapper<GetSalesSummaryReportRequestBody>
    {
        [MessageBodyMember(Name = "GetSalesSummaryReportRequest")]
        public GetSalesSummaryReportRequestBody Request
        {
            get;
            set;
        }
    }

    [Loggable]
    [DataContract(Name = "GetSalesSummaryReportRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetSalesSummaryReportRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 2, IsRequired = false)]
        public WalletType WalletType { set; get; }

        [Loggable]
        [DataMember(Order = 3, IsRequired = false)]
        public SummaryType SummaryType { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public DateTime InitialDate { set; get; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public DateTime EndDate { set; get; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = true)]
        public String Type { set; get; }

        [Loggable]
        [DataMember(Order =7, IsRequired = true)]
        public String Agent { set; get; }
    }
}