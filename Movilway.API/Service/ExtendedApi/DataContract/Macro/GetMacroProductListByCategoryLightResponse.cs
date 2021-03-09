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
    public class GetMacroProductListByCategoryLightResponse
    {
        [DataMember(Name="rc", Order = 0), XmlElement]
        public int? ResponseCode { set; get; }

        [DataMember(Name="rm",Order = 1), XmlElement]
        public String ResponseMessage { set; get; }

        [DataMember(Name="tid",Order = 2), XmlElement]
        public int? TransactionID { set; get; }

        [DataMember(Name="r",Order = 3), XmlElement]
        public String Rank { set; get; }

        [DataMember(Name="c",Order = 4), XmlArray("Categories"), XmlArrayItem("Category")]
        public List<CategoryLight> Categories { set; get; }

        public GetMacroProductListByCategoryLightResponse()
        {
            ResponseCode = 99;
        }
    }

    [DataContract]
    public class CategoryLight
    {
        [DataMember(Name = "id", Order = 0), XmlAttribute(AttributeName = "Id")]
        public int CategoryId { set; get; }

        [DataMember(Name = "Name", Order = 1), XmlAttribute(AttributeName = "Name")]
        public string CategoryName { set; get; }

        [DataMember(Name = "des", Order = 2), XmlAttribute(AttributeName = "Description")]
        public string CategoryDescription { set; get; }

        [DataMember(Name="sc",Order = 3), XmlArray("Subcategories"), XmlArrayItem("Subcategory")]
        public List<SubCategoryLight> Subcategories { set; get; }
    }

    [DataContract]
    public class SubCategoryLight
    {
        [DataMember(Name = "id", Order = 0), XmlAttribute(AttributeName = "Id")]
        public int SubcategoryId { set; get; }

        [DataMember(Name = "Name", Order = 1), XmlAttribute(AttributeName = "Name")]
        public string SubcategoryName { set; get; }

        [DataMember(Name = "des", Order = 2), XmlAttribute(AttributeName = "Description")]
        public string SubcategoryDescription { set; get; }

        [DataMember(Name="mp",Order = 3), XmlArray("MacroProducts"), XmlArrayItem("MacroProduct")]
        public List<MacroProductLight> MacroProducts { set; get; }

        public SubCategoryLight()
        {
            MacroProducts = new List<MacroProductLight>();
        }
    }

    [DataContract]
    public class MacroProductLight
    {
        [DataMember(Name = "id", Order = 0), XmlAttribute(AttributeName = "Id")]
        public int MacroProductId { set; get; }

        [DataMember(Name = "Name", Order = 1), XmlElement(ElementName = "Name")]
        public String MacroProductName { set; get; }

        //[DataMember(Name="pf",Order = 2), XmlElement]
        //public double PercentageFee { set; get; }
        //
        //[DataMember(Name="ff",Order = 3), XmlElement]
        //public double FlatFee { set; get; }
        //
        //[DataMember(Name = "rp", Order = 4), XmlElement]
        //public bool RePrint { set; get; }
        //
        ////Tipos: User y Query (consulta, es decir, bajo demanda)
        //[DataMember(Name = "at", Order = 5), XmlAttribute]
        //public String AmountType { set; get; }
        //
        //[DataMember(Name = "des", Order = 6), XmlElement]
        //public String Description { set; get; }
        //
        //[DataMember(Name = "bt", Order = 7), XmlElement]
        //public String BalanceType { set; get; }    

        public int Rank { set; get; }
        
        //public bool MandatoryPrint { set; get; }
        //
        //public String AmountMapValueTo { set; get; }
        //        
        //public String AmountMapKeyTo { set; get; }

        //[DataMember(Name = "fs", Order = 8), XmlArray("Fields"), XmlArrayItem("Field")]
        //public List<Field> Fields { set; get; }
        //
        //[DataMember(Name = "ch", Order = 9), XmlArray("Channels"), XmlArrayItem("Channel")]
        //public List<Channel> Channels { set; get; }

        [DataMember( Order = 10, EmitDefaultValue = false, IsRequired = false)]
        public bool? RequireQuery { set; get; }

        public bool ShouldSerializeRequireQuery()
        {
            return RequireQuery.HasValue;
        }
    }

    //[DataContract]
    //public class Field
    //{
    //    [DataMember(Name = "id", Order = 0), XmlAttribute(AttributeName = "Id")]
    //    public int Id { set; get; }
    //
    //    [DataMember(Name = "Name", Order = 1), XmlAttribute(AttributeName = "Name")]
    //    public String Name { set; get; }
    //
    //    //String, Integer, Decimal, Date, List, Constant
    //    [DataMember(Name = "ty", Order = 2), XmlElement(ElementName = "Type")]
    //    public String Type { set; get; }
    //
    //    //Posibles valores son: Amount, ExternalTransactionReference, MNO, Recipient
    //    [DataMember(Name = "mvnt", Order = 3), XmlElement(ElementName = "MapValueNameTo")]
    //    public String MapValueNameTo { set; get; }
    //
    //    [DataMember(Name = "mvt", Order = 3), XmlElement(ElementName = "MapValueTo")]
    //    public String MapValueTo { set; get; }
    //
    //    [DataMember(Name = "ft", Order = 4), XmlElement(ElementName = "Format")]
    //    public String Format { set; get; }
    //
    //    [DataMember(Name = "len", Order = 5), XmlElement(ElementName = "Length")]
    //    public int Length { set; get; }
    //
    //    [DataMember(Name = "min", Order = 6), XmlElement]
    //    public double Min { set; get; }
    //
    //    [DataMember(Name = "max", Order = 7), XmlElement]
    //    public double Max { set; get; }
    //
    //    //Tres ejempos de lista.
    //    //Venta de Entradas: Patio 20 / Balcón 10 / VIP 30
    //    //Venta de Pines: 10 / 20 / 30 / 40 / 50
    //    //Cruz Vital: Ex Sangre 10 / HIV 20 / Orina 15
    //
    //    [DataMember(Name = "vs", Order = 8), XmlArray("Values"), XmlArrayItem("Value")]
    //    public List<FieldValues> Values { set; get; }
    //
    //    //Este campo nuevo puede ofrecer mensajes más largos ofreciendo ayuda. Por ahora clabeado a "Ayuda" mientras se actualiza el Backoffice.
    //    [DataMember(Name = "des", Order = 9), XmlElement]
    //    public String Description { set; get; }
    //
    //    //Indica si el campo es opcional o mandatorio. Por ahora fijo en mandatorio mientras arreglamos el Backoffice
    //    [DataMember(Name = "man", Order = 10), XmlElement]
    //    public bool Mandatory { set; get; }
    //
    //    [DataMember(Name = "mid", Order = 11), XmlIgnore]
    //    public int MacroProductId { set; get; }
    //
    //    [DataMember(Name = "cid", Order = 12), XmlIgnore]
    //    public int CountryId { set; get; }
    //
    //    public Field()
    //    {
    //        MapValueNameTo = String.Empty;
    //        MapValueTo = String.Empty;
    //        Min = 0;
    //        Max = 0;
    //        Values = new List<FieldValues>();
    //        Description = String.Empty;
    //        Mandatory = true;
    //        MacroProductId = 0;
    //        CountryId = 0;
    //    }
    //}
    //
    //[DataContract(Name = "Amount"), XmlType("Amount")]
    //public class AmountEntity
    //{
    //    [DataMember(Name = "n", Order = 0), XmlAttribute(AttributeName = "Name")]
    //    public String AmountName { set; get; }
    //
    //    [DataMember(Name = "v", Order = 1), XmlAttribute(AttributeName = "Value")]
    //    public Double Amount { set; get; }
    //}
    //
    //[DataContract]
    //public class Channel
    //{
    //    [DataMember(Name = "id", Order = 0), XmlAttribute(AttributeName = "Id")]
    //    public int MacroProductChannelId { set; get; }
    //
    //    [DataMember(Name = "Name", Order = 1), XmlAttribute(AttributeName = "Name")]
    //    public String Name { set; get; }
    //
    //    [DataMember(Name = "l", Order = 2), XmlAttribute(AttributeName = "Load")]
    //    public int Load { set; get; }
    //}
    //
    //[DataContract]
    //public class FieldValues
    //{
    //    [DataMember(Name = "vn", Order = 0), XmlText()]
    //    public String ValueName { set; get; }
    //
    //    [DataMember(Name = "va", Order = 1), XmlAttribute(AttributeName = "Value")]
    //    public String Value { set; get; }
    //}
}