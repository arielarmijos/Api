using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract.Common
{
    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class AFinancialApiResponse : AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order=3, EmitDefaultValue=false, IsRequired=false)]
        public decimal? Fee { set; get; }

        public AFinancialApiResponse()
        {
            ResponseCode = 99;
        }
    }
}