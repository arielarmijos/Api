using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Utiba;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.GetProductList)]
    public class GetProductListProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetProductListProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            GetProductListRequestBody request = requestObject as GetProductListRequestBody;
            GetProductListResponseBody response = null;

            logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetProductListProvider] [SEND-DATA] getProductListRequest {sessionId=" + sessionID + ",device_type=" + request.DeviceType + ",agent_reference=" + request.Agent + "}");

            getProductListResponse utibaGetProductListResponse = utibaClientProxy.getProductList(new getProductList()
            {
                getProductListRequest = new getProductListRequestType()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType,
                    agent_reference = request.Agent
                }
            });

            StringBuilder sb = new StringBuilder("products={");
            foreach (var pair in utibaGetProductListResponse.getProductListReturn.products)
                sb.Append("keyValuePair={key=" + pair.key + ",value=" + pair.value + "},");
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");

            logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetProductListProvider] [RECV-DATA] getProductListResponse {transid=" + utibaGetProductListResponse.getProductListReturn.transid + 
                                                                                                        ",result=" + utibaGetProductListResponse.getProductListReturn.result + 
                                                                                                        ",result_namespace=" + utibaGetProductListResponse.getProductListReturn.result_namespace + 
                                                                                                        "," + sb.ToString() + "}");

            if (utibaGetProductListResponse != null)
            {
                response = new GetProductListResponseBody()
                {
                    ResponseCode = Utils.BuildResponseCode(utibaGetProductListResponse.getProductListReturn.result, utibaGetProductListResponse.getProductListReturn.result_namespace),
                    ResponseMessage = utibaGetProductListResponse.getProductListReturn.result_message,
                    TransactionID = utibaGetProductListResponse.getProductListReturn.transid
                };
                if (utibaGetProductListResponse.getProductListReturn.products != null &&
                    utibaGetProductListResponse.getProductListReturn.products.Length > 0)
                {
                    response.ProductList = new ProductList();
                    foreach (KeyValuePair1 keyValuePair1 in utibaGetProductListResponse.getProductListReturn.products)
                    {
                        response.ProductList.Add(keyValuePair1.key, keyValuePair1.value);
                    }
                }
            }
            return (response);
        }
    }
}