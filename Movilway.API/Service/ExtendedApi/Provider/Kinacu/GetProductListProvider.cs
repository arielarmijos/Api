using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using Movilway.API.KinacuWebService;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetProductList)]
    public class GetProductListProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetProductListProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new GetProductListResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0,
                    ProductList = new ProductList()
                };

            GetProductListRequestBody request = requestObject as GetProductListRequestBody;
            GetProductListResponseBody response = null;

            logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetProductListProvider] [SEND-DATA] getProductsParameters {UserId=" + sessionID + "}");

            Product[] products = kinacuWS.GetProducts(int.Parse(sessionID));

            StringBuilder sb = new StringBuilder();
            foreach (Product product in products)
                sb.Append("Product={Id=" + product.Id + ",ModuleId=" + product.ModuleId + ",Name=" + product.Name + ",ProviderId=" + product.ProviderId + "},");
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);

            logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetProductListProvider] [RECV-DATA] getProductsResult {response={" + sb.ToString() + "}}");

            response = new GetProductListResponseBody()
            {
                ResponseCode = (products == null ? 99 : 0),
                ResponseMessage = (products == null ? "error" : "exito"),
                TransactionID = 0
            };
            if (products != null && products.Length > 0)
            {
                response.ProductList = new ProductList();
                foreach (Product product in products)
                    response.ProductList.Add(product.Id.ToString(), product.Name);
            }
            return (response);
        }
    }
}