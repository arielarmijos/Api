using System;
using System.Collections;
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
    public class GetMacroProductListResponse : IMovilwayApiResponseWrapper<GetMacroProductListResponseBody>
    {
        [MessageBodyMember(Name = "GetMacroProductResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetMacroProductListResponseBody Response { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetMacroProductResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetMacroProductListResponseBody : AGenericApiResponse
    {
        [DataMember(Order = 0)]
        public ProductTypeList ProductTypes { set; get; }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class ProductTypeList : List<ProductType>
    {
    }

    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class ProductType
    {
        [DataMember(Order = 0)]
        public int ProductTypeId { set; get; }

        [DataMember(Order = 1)]
        public MacroProductList MacroProducts { set; get; }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class MacroProductList : List<MacroProduct>
    {
    }

    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class MacroProduct
    {
        [DataMember(Order = 0)]
        public int Id { set; get; }

        [DataMember(Order = 1)]
        public String Description { set; get; }

        [DataMember(Order = 2)]
        public String Category { set; get; }

        [DataMember(Order = 3)]
        public double FeePercentage { set; get; }

        [DataMember(Order = 4)]
        public double FeePlus { set; get; }

        [DataMember(Order = 5)]
        public int RePrint { set; get; }

        [DataMember(Order = 6)]
        public SubProductList SubProducts { set; get; }

        [DataMember(Order = 7)]
        public FieldList Fields { set; get; }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class SubProductList : List<SubProduct>
    {
    }

    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class SubProduct
    {
        [DataMember(Order = 0)]
        public int Id { set; get; }

        [DataMember(Order = 1)]
        public string Name { set; get; }

        [DataMember(Order = 2)]
        public double Amount { set; get; }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class FieldList : List<Field>
    {

    }

    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class Field
    {
        [DataMember(Order = 0)]
        public String Name { set; get; }

        [DataMember(Order = 1)]
        public String Type { set; get; }

        [DataMember(Order = 2)]
        public String Format { set; get; }

        [DataMember(Order = 3)]
        public int Length { set; get; }
    }

}