using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Movilway.API.Service.ExtendedApi.DataContract.MacroInternational
{
    [DataContract]
    [XmlRoot]
    public class GetMacroProductInternationalResponse
    {
        [DataMember(Order = 0), XmlElement]
        public int? ResponseCode { set; get; }

        [DataMember(Order = 1), XmlElement]
        public String ResponseMessage { set; get; }

        [DataMember(Order = 2), XmlElement]
        public int? TransactionID { set; get; }

        [DataMember(Order = 3), XmlElement]
        public String Rank { set; get; }

        [DataMember(Order = 4, EmitDefaultValue = false), XmlArray("Countries", IsNullable = false), XmlArrayItem("Country")]
        public List<IntCountry> Countries { set; get; }

        [DataMember(Order = 5, EmitDefaultValue = false), XmlArray("MacroProducts", IsNullable = false), XmlArrayItem("MacroProduct")]
        public List<IntMacroProduct> MacroProducts { set; get; }

        public GetMacroProductInternationalResponse()
        {
            ResponseCode = 99;
        }
    }

    [DataContract]
    public class IntCountry
    {
        [DataMember(Name = "Id", Order = 0), XmlAttribute(AttributeName = "Id")]
        public int CountryId { set; get; }

        [DataMember(Name = "Name", Order = 1), XmlAttribute(AttributeName = "Name")]
        public string CountryName { set; get; }

        [DataMember(Order = 3), XmlArray("Categories"), XmlArrayItem("Category")]
        public List<IntCategory> Categories { set; get; }

        public IntCountry()
        {
            Categories = new List<IntCategory>();
        }
    }

    [DataContract]
    public class IntCategory
    {
        [DataMember(Name = "Id", Order = 0), XmlAttribute(AttributeName = "Id")]
        public int CategoryId { set; get; }

        [DataMember(Name = "Name", Order = 1), XmlAttribute(AttributeName = "Name")]
        public string CategoryName { set; get; }

        [DataMember(Name = "Description", Order = 2), XmlAttribute(AttributeName = "Description")]
        public string CategoryDescription { set; get; }

        [DataMember(Order = 3), XmlArray("Subcategories"), XmlArrayItem("Subcategory")]
        public List<IntSubCategory> Subcategories { set; get; }
    }

    [DataContract]
    public class IntSubCategory
    {
        [DataMember(Name = "Id", Order = 0), XmlAttribute(AttributeName = "Id")]
        public int SubcategoryId { set; get; }

        [DataMember(Name = "Name", Order = 1), XmlAttribute(AttributeName = "Name")]
        public string SubcategoryName { set; get; }

        [DataMember(Name = "Description", Order = 2), XmlAttribute(AttributeName = "Description")]
        public string SubcategoryDescription { set; get; }

        [DataMember(Order = 3), XmlArray("MacroProducts"), XmlArrayItem("MacroProduct")]
        public List<IntMacroProduct> MacroProducts { set; get; }

        public IntSubCategory()
        {
            MacroProducts = new List<IntMacroProduct>();
        }
    }

    [DataContract]
    public class IntMacroProduct
    {
        [DataMember(Name = "Id", Order = 0), XmlAttribute(AttributeName = "Id")]
        public int MacroProductId { set; get; }

        [DataMember(Name = "Name", Order = 1), XmlElement(ElementName = "Name")]
        public String MacroProductName { set; get; }

        [DataMember(Order = 2), XmlElement]
        public int CountryId { set; get; }

        [DataMember(Order = 3), XmlElement]
        public double PercentageFee { set; get; }

        [DataMember(Order = 4), XmlElement]
        public double FlatFee { set; get; }

        [DataMember(Order = 5), XmlElement]
        public bool RePrint { set; get; }

        //Tipos: User y Query (consulta, es decir, bajo demanda)
        [DataMember(Order = 6), XmlAttribute]
        public String AmountType { set; get; }

        [DataMember(Order = 7), XmlElement]
        public String Description { set; get; }

        [DataMember(Order = 8), XmlElement]
        public String BalanceType { set; get; }

        public int Rank { set; get; }

        public bool MandatoryPrint { set; get; }

        public String AmountMapValueTo { set; get; }

        public String AmountMapKeyTo { set; get; }

        //Pais al que pertenece el Macro Producto Internacional, inicialmente todos seran USA.
        [DataMember(Order = 9), XmlElement]
        public int OwnerCountryId { set; get; }

        [DataMember(Order = 10), XmlElement]
        public String CurrencyPurchase { set; get; }

        [DataMember(Order = 11), XmlElement]
        public String CurrencySale { set; get; }

        [DataMember(Order = 12), XmlElement]
        public Decimal ExchangeRate { set; get; }

        [DataMember(Order = 13), XmlElement]
        public String AmountField { set; get; }

        [DataMember(Order = 14), XmlArray("Fields"), XmlArrayItem("Field")]
        public List<IntField> Fields { set; get; }

        [DataMember(Order = 15), XmlArray("Channels"), XmlArrayItem("Channel")]
        public List<IntChannel> Channels { set; get; }
    }

    [DataContract]
    public class IntField
    {
        [DataMember(Name = "Id", Order = 0), XmlAttribute(AttributeName = "Id")]
        public int Id { set; get; }

        [DataMember(Name = "Name", Order = 1), XmlAttribute(AttributeName = "Name")]
        public String Name { set; get; }

        //String, Integer, Decimal, Date, List, Constant
        [DataMember(Name = "Type", Order = 2), XmlElement(ElementName = "Type")]
        public String Type { set; get; }

        //Posibles valores son: Amount, ExternalTransactionReference, MNO, Recipient
        [DataMember(Name = "MapValueNameTo", Order = 3), XmlElement(ElementName = "MapValueNameTo")]
        public String MapValueNameTo { set; get; }

        [DataMember(Name = "MapValueTo", Order = 3), XmlElement(ElementName = "MapValueTo")]
        public String MapValueTo { set; get; }

        [DataMember(Name = "Format", Order = 4), XmlElement(ElementName = "Format")]
        public String Format { set; get; }

        [DataMember(Name = "Length", Order = 5), XmlElement(ElementName = "Length")]
        public int Length { set; get; }

        [DataMember(Order = 6), XmlElement]
        public double Min { set; get; }

        [DataMember(Order = 7), XmlElement]
        public double Max { set; get; }

        //Tres ejempos de lista.
        //Venta de Entradas: Patio 20 / Balcón 10 / VIP 30
        //Venta de Pines: 10 / 20 / 30 / 40 / 50
        //Cruz Vital: Ex Sangre 10 / HIV 20 / Orina 15

        [DataMember(Order = 8), XmlArray("Values"), XmlArrayItem("Value")]
        public List<IntFieldValues> Values { set; get; }

        //Este campo nuevo puede ofrecer mensajes más largos ofreciendo ayuda. Por ahora clabeado a "Ayuda" mientras se actualiza el Backoffice.
        [DataMember(Name = "Description", Order = 9), XmlElement]
        public String Description { set; get; }

        //Indica si el campo es opcional o mandatorio. Por ahora fijo en mandatorio mientras arreglamos el Backoffice
        [DataMember(Name = "Mandatory", Order = 10), XmlElement]
        public bool Mandatory { set; get; }

        [DataMember(Name = "MacroProductId", Order = 11), XmlIgnore]
        public int MacroProductId { set; get; }

        [DataMember(Name = "CountryId", Order = 12), XmlIgnore]
        public int CountryId { set; get; }

        public IntField()
        {
            MapValueNameTo = String.Empty;
            MapValueTo = String.Empty;
            Min = 0;
            Max = 0;
            Values = new List<IntFieldValues>();
            Description = String.Empty;
            Mandatory = true;
            MacroProductId = 0;
            CountryId = 0;
        }
    }

    [DataContract(Name = "Amount"), XmlType("Amount")]
    public class AmountEntity
    {
        [DataMember(Name = "Name", Order = 0), XmlAttribute(AttributeName = "Name")]
        public String AmountName { set; get; }

        [DataMember(Name = "Value", Order = 1), XmlAttribute(AttributeName = "Value")]
        public Double Amount { set; get; }
    }

    [DataContract]
    public class IntChannel
    {
        [DataMember(Name = "Id", Order = 0), XmlAttribute(AttributeName = "Id")]
        public int MacroProductChannelId { set; get; }

        [DataMember(Name = "Name", Order = 1), XmlAttribute(AttributeName = "Name")]
        public String Name { set; get; }

        [DataMember(Name = "Load", Order = 2), XmlAttribute(AttributeName = "Load")]
        public int Load { set; get; }
    }

    [DataContract]
    public class IntFieldValues
    {
        [DataMember(Name = "ValueName", Order = 0), XmlText()]
        public String ValueName { set; get; }

        [DataMember(Name = "Value", Order = 1), XmlAttribute(AttributeName = "Value")]
        public String Value { set; get; }
    }
}