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
    public class GetPurchasesSummaryDashBoardRequest : IMovilwayApiRequestWrapper<GetPurchasesSummaryDashBoardRequestBody>
    {
        [MessageBodyMember(Name = "GetPurchasesSummaryDashBoardRequest")]
        public GetPurchasesSummaryDashBoardRequestBody Request
        {
            get;
            set;
        }
    }

    [Loggable]
    [DataContract(Name = "GetPuchasesSummaryDashBoardRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetPurchasesSummaryDashBoardRequestBody : ASecuredApiRequest
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
        [DataMember(Order =7, IsRequired = true)]
        public String Agent { set; get; }
    }
}