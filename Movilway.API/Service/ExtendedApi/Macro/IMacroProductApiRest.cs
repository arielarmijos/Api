using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Movilway.API.Service.ExtendedApi.DataContract;

namespace Movilway.API.Service.ExtendedApi.Macro
{
    [ServiceContract]
    [Description("Expone los metodos por REST")]
    public interface IMacroProductApiRest
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "MacroProductsByCategory",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped),
        Description("Retorna los Macroproductos disponibles para el usuario, agrupados por categorias.")]
        DataContract.Macro.GetMacroProductListByCategoryResponse MacroProductsByCategory(DataContract.Macro.GetMacroProductListRequest request);

        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "MacroProductsByCategoryLight",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped),
        Description("Retorna la lista ligera de los Macroproductos disponibles para el usuario, agrupados por categorias.")]
        DataContract.Macro.GetMacroProductListByCategoryLightResponse MacroProductsByCategoryLight(DataContract.Macro.GetMacroProductListRequest request);

        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "MacroProductDetails",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped),
        Description("Retorna el detalle de un Macroproducto específico.")]
        DataContract.Macro.GetMacroProductDetailsResponse MacroProductDetails(DataContract.Macro.GetMacroProductDetailsRequest request);

        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "MacroProducts",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped),
        Description("Retorna los Macroproductos disponibles para el usuario.")]
        DataContract.Macro.GetMacroProductListResponse MacroProducts(DataContract.Macro.GetMacroProductListRequest request);

        //Operaciones para MacroProduct International

        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "MacroProductsByCategoryInter",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped),
        Description("Retorna los Macroproductos para el API International agrupados por Pais. Si se envia el parametro CountryId retorna solo los MacroProductos de ese Pais, si no se envia, retorna los de todos los paises.")]
        DataContract.MacroInternational.GetMacroProductInternationalResponse MacroProductsByCategoryInter(DataContract.MacroInternational.GetMacroProductInternationalRequest request);

        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "MacroProductsInter",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped),
        Description("Retorna los Macroproductos para el API International. Si se envia el parametro CountryId retorna solo los MacroProductos de ese Pais, si no se envia, retorna los de todos los paises.")]
        DataContract.MacroInternational.GetMacroProductInternationalResponse MacroProductsInter(DataContract.MacroInternational.GetMacroProductInternationalRequest request);


        //[OperationContract]
        //[WebGet(UriTemplate = "MacroProductsBySubcategory?User={id}&Pass={pass}&Session={session}&Device={device}&Platform={platform}&Agent={agent}",
        //        ResponseFormat = WebMessageFormat.Json,
        //        BodyStyle = WebMessageBodyStyle.WrappedResponse),
        //Description("Retorna los Macroproductos disponibles para el usuario.")]
        //DataContract.Macro.GetMacroProductListBySubcategoryResponse MacroProductsBySubcategory(string id, string pass, string session, string device, string platform, string agent);
    }
}