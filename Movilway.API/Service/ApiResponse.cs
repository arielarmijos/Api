using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;

namespace Movilway.API.Service
{
    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class ApiResponse:IMovilwayApiResponse
    {
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public int? ResponseCode { set; get; }

        [DataMember(IsRequired=false, EmitDefaultValue = false)]
        public String ResponseMessage { set; get; }

        [DataMember]
        public int? TransactionID { set; get; }

        public ApiResponse()
        {
            ResponseCode = 99;
        }
    }
}