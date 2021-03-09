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
    public class ChangeBranchStatusRequest : IMovilwayApiRequestWrapper<ChangeBranchStatusRequestBody>
    {
        [MessageBodyMember(Name = "ChangeBranchStatusRequest")]
        public ChangeBranchStatusRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "ChangeBranchStatusRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class ChangeBranchStatusRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String Agent { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = false)]
        public bool Cascade { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}