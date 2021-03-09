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
    public class GetFavoriteAmountsResponse
    {
        [DataMember(Order = 0), XmlElement]
        public int? ResponseCode { set; get; }

        [DataMember(Order = 1), XmlElement]
        public String ResponseMessage { set; get; }

        [DataMember(Order = 2), XmlElement]
        public int? TransactionID { set; get; }

        [DataMember(Order = 3), XmlArray("FavoriteAmounts"), XmlArrayItem("FavoriteAmounts")]
        public List<FavoriteAmount> FavoriteAmounts { set; get; }

        public GetFavoriteAmountsResponse()
        {
            ResponseCode = 99;
        }
    }


    [DataContract]
    public class FavoriteAmount
    {
        [DataMember(Name = "id", Order = 0), XmlAttribute(AttributeName = "Id")]
        public int MacroProductId { set; get; }

        [DataMember(Name = "am", Order = 1), XmlArray("Amounts"), XmlArrayItem("Amounts")]
        public List<decimal> Amounts { set; get; }
    }
}