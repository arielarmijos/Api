using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.Logging.Attribute;
using System.Xml.Serialization;

namespace Movilway.API.Service.ExtendedApi.DataContract.Common
{
    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class AGenericApiResponse : IMovilwayApiResponse
    {
        [Loggable]
        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = true), XmlElement]
        public int? ResponseCode { set; get; }

        [Loggable]
        [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false), XmlElement]
        public String ResponseMessage { set; get; }

        [Loggable]
        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false), XmlElement]
        public int? TransactionID { set; get; }

        public AGenericApiResponse()
        {
            ResponseCode = 99;
        }
    }
}