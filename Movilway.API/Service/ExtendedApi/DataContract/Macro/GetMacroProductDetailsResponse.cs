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
    public class GetMacroProductDetailsResponse
    {
        [DataMember(Name="rc", Order = 0), XmlElement]
        public int? ResponseCode { set; get; }

        [DataMember(Name="rm",Order = 1), XmlElement]
        public String ResponseMessage { set; get; }

        [DataMember(Name="tid",Order = 2), XmlElement]
        public int? TransactionID { set; get; }

        [DataMember(Name="c",Order = 4), XmlArray("Categories"), XmlArrayItem("Category")]
        public List<Category> Categories { set; get; }

        public GetMacroProductDetailsResponse()
        {
            ResponseCode = 99;
        }
    }
}