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
    public class ProcessExternalTransactionResponse : IMovilwayApiResponseWrapper<ProcessExternalTransactionResponseBody>
    {
        [MessageBodyMember(Name = "ProcessExternalTransactionResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public ProcessExternalTransactionResponseBody Response { set; get; }

        public ProcessExternalTransactionResponse()
        {
            Response = new ProcessExternalTransactionResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "ProcessExternalTransactionResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class ProcessExternalTransactionResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AFinancialApiResponse
    {
        //[Loggable]
        //[DataMember(Order = 0, IsRequired=false, EmitDefaultValue=false)]
        //public String ExternalTransactionReference { set; get; }
        
        [Loggable]
        [DataMember(Name = "Result", Order = 5, IsRequired = false, EmitDefaultValue = false)]
        public Boolean Result { get; set; }

        [Loggable]
        [DataMember(Name = "ExternalTransactionReference", Order = 6, IsRequired = false, EmitDefaultValue = false)]
        public String ExternalTransactionReference { get; set; }
    }
}