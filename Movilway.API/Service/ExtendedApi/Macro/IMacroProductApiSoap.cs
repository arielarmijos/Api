using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Movilway.API.Service.ExtendedApi.DataContract;

namespace Movilway.API.Service.ExtendedApi.Macro
{
    [ServiceContract]
    [XmlSerializerFormat]
    [Description("Expone los metodos por SOAP")]
    public interface IMacroProductApiSoap
    {
        [OperationContract]
        [Description("Retorna los Macroproductos disponibles para el usuario, agrupados por categorias.")]
        DataContract.Macro.GetMacroProductListByCategoryResponse GetMacroProductsByCategory(DataContract.Macro.GetMacroProductListRequest request);

        [OperationContract]
        [Description("Retorna los Macroproductos disponibles para el usuario.")]
        DataContract.Macro.GetMacroProductListResponse GetMacroProducts(DataContract.Macro.GetMacroProductListRequest request);

        [OperationContract]
        [Description("Retorna los montos favoritos de los Macroproductos.")]
        DataContract.Macro.GetFavoriteAmountsResponse GetFavoriteAmounts(DataContract.Macro.GetFavoriteAmountsRequest request);

        //Operaciones para MacroProduct International

        [OperationContract]
        [Description("Retorna los Macroproductos para el Api International agrupados por Pais. Si se envia el parametro CountryId retorna solo los MacroProductos de ese Pais, si no se envia, retorna los de todos los paises.")]
        DataContract.MacroInternational.GetMacroProductInternationalResponse GetMacroProductsByCategoryInter(DataContract.MacroInternational.GetMacroProductInternationalRequest request);

        [OperationContract]
        [Description("Retorna los Macroproductos para el Api International. Si se envia el parametro CountryId retorna solo los MacroProductos de ese Pais, si no se envia, retorna los de todos los paises.")]
        DataContract.MacroInternational.GetMacroProductInternationalResponse GetMacroProductsInter(DataContract.MacroInternational.GetMacroProductInternationalRequest request);
    }
}
