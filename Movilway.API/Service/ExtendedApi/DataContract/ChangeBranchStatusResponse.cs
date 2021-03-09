using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class ChangeBranchStatusResponse : IMovilwayApiResponseWrapper<ChangeBranchStatusResponseBody>
    {
        [MessageBodyMember(Name = "ChangeBranchStatusResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public ChangeBranchStatusResponseBody Response { set; get; }

        public ChangeBranchStatusResponse()
        {
            Response = new ChangeBranchStatusResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "ChangeBranchStatusResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class ChangeBranchStatusResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}