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
    public class ProcessDistributionRequest : IMovilwayApiRequestWrapper<ProcessDistributionRequestBody>
    {
        [MessageBodyMember(Name = "ProcessDistributionRequest")]
        public ProcessDistributionRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "ProcessDistributionRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class ProcessDistributionRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String Comment { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public bool ImmediatelyDistribute { set; get; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public bool IsApproved { set; get; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = true)]
        public long DistributionId { set; get; }

        [Loggable]
        [DataMember(Order = 7, IsRequired = true)]
        public int AccountId { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}