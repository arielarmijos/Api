using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped=false)]
    public class GetSalesSummaryRequest : IMovilwayApiRequestWrapper<GetSalesSummaryRequestBody>
    {
        [MessageBodyMember(Name = "GetSalesSummaryRequest")]
        public GetSalesSummaryRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetSalesSummaryRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class GetSalesSummaryRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 2, IsRequired = false)]
        public WalletType WalletType { set; get; }
        
        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public DateTime Date { set; get; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = false)]
        public SummaryType SummaryType { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}