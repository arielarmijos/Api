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
    public class ProcessDistributionResponse : IMovilwayApiResponseWrapper<ProcessDistributionResponseBody>
    {
        [MessageBodyMember(Name = "ProcessDistributionResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public ProcessDistributionResponseBody Response { set; get; }

        public ProcessDistributionResponse()
        {
            Response = new ProcessDistributionResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "ProcessDistributionResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class ProcessDistributionResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}