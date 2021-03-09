using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Movilway.API.Service.ExtendedApi.DataContract.Macro
{
    [DataContract]
    [XmlRoot]
    public class GetMacroProductListResponse
    {
        [DataMember(Order = 0), XmlElement]
        public int? ResponseCode { set; get; }

        [DataMember(Order = 1), XmlElement]
        public String ResponseMessage { set; get; }

        [DataMember(Order = 2), XmlElement]
        public int? TransactionID { set; get; }

        [DataMember(Order = 3), XmlArray("MacroProducts"), XmlArrayItem("MacroProduct")]
        public List<MacroProduct> MacroProducts { set; get; }

        public GetMacroProductListResponse()
        {
            ResponseCode = 99;
        }
    }
}