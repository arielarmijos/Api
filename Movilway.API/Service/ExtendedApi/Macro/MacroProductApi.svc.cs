using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Macro;
using Movilway.API.Service.ExtendedApi.Provider.Macro;
using System.ServiceModel.Activation;
using System.Configuration;

namespace Movilway.API.Service.ExtendedApi.Macro
{
    [InspectorBehavior]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
                     InstanceContextMode = InstanceContextMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MacroProductApi : IMacroProductApiSoap, IMacroProductApiRest
    {
        //Métodos expuestos por SOAP

        public DataContract.Macro.GetMacroProductListByCategoryResponse GetMacroProductsByCategory(DataContract.Macro.GetMacroProductListRequest request)
        {
            if (String.IsNullOrEmpty(request.Platform)) request.Platform = ConfigurationManager.AppSettings["DefaultPlatform"];

            var response = new MacroProductProvider().GetMacroProductsByCategory(request);

            return response;
        }

        public GetFavoriteAmountsResponse GetFavoriteAmounts(GetFavoriteAmountsRequest request)
        {
            //var rand = new Random();
            //var response = new GetFavoriteAmountsResponse();
            //
            //response.ResponseCode = 0;
            //response.ResponseMessage = "Exito";
            //response.TransactionID = rand.Next(1000000, 9999999);
            //response.FavoriteAmounts = new List<FavoriteAmount>();
            //
            //for (int i = 0; i < rand.Next(3,8); i++)
            //{
            //    var amounts = new FavoriteAmount() { Amounts = new List<decimal>(), MacroProductId = i+1000 };
            //
            //    amounts.Amounts.Add(5000m);
            //    amounts.Amounts.Add(3000m);
            //    amounts.Amounts.Add(1000m);
            //    amounts.Amounts.Add(25000m);
            //    amounts.Amounts.Add(15000m);
            //    amounts.Amounts.Add(10000m);
            //
            //    response.FavoriteAmounts.Add(amounts);
            //}
            //
            //return response;

            if (String.IsNullOrEmpty(request.Platform)) request.Platform = ConfigurationManager.AppSettings["DefaultPlatform"];
            
            var response = new MacroProductProvider().GetFavoriteAmounts(request);
            
            return response;
        }

        public DataContract.Macro.GetMacroProductListResponse GetMacroProducts(DataContract.Macro.GetMacroProductListRequest request)
        {
            if (String.IsNullOrEmpty(request.Platform)) request.Platform = ConfigurationManager.AppSettings["DefaultPlatform"];

            var response = new MacroProductProvider().GetMacroProducts(request);

            return response;
        }

        public DataContract.MacroInternational.GetMacroProductInternationalResponse GetMacroProductsByCategoryInter(DataContract.MacroInternational.GetMacroProductInternationalRequest request)
        {
            if (String.IsNullOrEmpty(request.Platform)) request.Platform = ConfigurationManager.AppSettings["DefaultPlatform"];

            var response = new MacroProductInternationalProvider().GetMacroProductsByCategoryInter(request);

            return response;
        }

        public DataContract.MacroInternational.GetMacroProductInternationalResponse GetMacroProductsInter(DataContract.MacroInternational.GetMacroProductInternationalRequest request)
        {
            if (String.IsNullOrEmpty(request.Platform)) request.Platform = ConfigurationManager.AppSettings["DefaultPlatform"];

            var response = new MacroProductInternationalProvider().GetMacroProductsInter(request);

            return response;
        }

        //Métodos expuestos por REST

        public DataContract.Macro.GetMacroProductListByCategoryResponse MacroProductsByCategory(DataContract.Macro.GetMacroProductListRequest request)
        {
            if (String.IsNullOrEmpty(request.Platform)) request.Platform = ConfigurationManager.AppSettings["DefaultPlatform"];

            var response = new MacroProductProvider().GetMacroProductsByCategory(request);

            return response;
        }

        public DataContract.Macro.GetMacroProductListByCategoryLightResponse MacroProductsByCategoryLight(DataContract.Macro.GetMacroProductListRequest request)
        {
            if (String.IsNullOrEmpty(request.Platform)) request.Platform = ConfigurationManager.AppSettings["DefaultPlatform"];

            var response = new MacroProductProvider().GetMacroProductsByCategoryLight(request);

            return response;
        }

        public DataContract.Macro.GetMacroProductDetailsResponse MacroProductDetails(DataContract.Macro.GetMacroProductDetailsRequest request)
        {
            if (String.IsNullOrEmpty(request.Platform)) request.Platform = ConfigurationManager.AppSettings["DefaultPlatform"];

            var response = new MacroProductProvider().GetMacroProductDetails(request);

            return response;
        }

        public DataContract.Macro.GetMacroProductListResponse MacroProducts(DataContract.Macro.GetMacroProductListRequest request)
        {
            if (String.IsNullOrEmpty(request.Platform)) request.Platform = ConfigurationManager.AppSettings["DefaultPlatform"];

            var response = new MacroProductProvider().GetMacroProducts(request);

            return response;
        }

        public DataContract.MacroInternational.GetMacroProductInternationalResponse MacroProductsByCategoryInter(DataContract.MacroInternational.GetMacroProductInternationalRequest request)
        {
            if (String.IsNullOrEmpty(request.Platform)) request.Platform = ConfigurationManager.AppSettings["DefaultPlatform"];

            var response = new MacroProductInternationalProvider().GetMacroProductsByCategoryInter(request);

            return response;
        }

        public DataContract.MacroInternational.GetMacroProductInternationalResponse MacroProductsInter(DataContract.MacroInternational.GetMacroProductInternationalRequest request)
        {
            if (String.IsNullOrEmpty(request.Platform)) request.Platform = ConfigurationManager.AppSettings["DefaultPlatform"];

            var response = new MacroProductInternationalProvider().GetMacroProductsInter(request);

            return response;
        }
    }
}
