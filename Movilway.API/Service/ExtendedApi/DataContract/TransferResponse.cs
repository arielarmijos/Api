using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System.Xml.Serialization;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class TransferResponse : IMovilwayApiResponseWrapper<TransferResponseBody>
    {
        [MessageBodyMember(Name = "TransferResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public TransferResponseBody Response { set; get; }

        public TransferResponse()
        {
            Response = new TransferResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "TransferResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class TransferResponseBody : IMovilwayApiResponse
    {
        [Loggable]
        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = true), XmlElement]
        public int? ResponseCode { set; get; }

        [Loggable]
        [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false), XmlElement]
        public String ResponseMessage { set; get; }

        [Loggable]
        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public String ResponseDate { set; get; }

        [Loggable]
        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false), XmlElement]
        public int? TransactionID { set; get; }

        [Loggable]
        [DataMember(Order = 4, EmitDefaultValue = false, IsRequired = false)]
        public decimal? Fee { set; get; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = false, EmitDefaultValue = false)]
        public Decimal StockBalance { set; get; }

        //[Loggable]
        //[DataMember(Order = 6, IsRequired = false, EmitDefaultValue = false)]
        //public Decimal ChargedAmount { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}