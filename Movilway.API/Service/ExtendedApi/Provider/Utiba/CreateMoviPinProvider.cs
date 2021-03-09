using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Utiba;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using System.Configuration;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.CreateMoviPin)]
    public class CreateMoviPinProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(CreateMoviPinProvider));
        protected override ILogger ProviderLogger { get { return logger; } }
        protected override TransactionType TransactionType { get { return TransactionType.createcoupon; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            CreateMoviPinRequestBody request = requestObject as CreateMoviPinRequestBody;
            CreateMoviPinResponseBody response = null;

            createcouponRequestType createCouponRequest = new createcouponRequestType()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType,
                    amount = request.Amount,
                    amountSpecified = true,
                    wallet_type = int.Parse(ConfigurationManager.AppSettings["CouponWalletType"]),
                    wallet_typeSpecified = true,
                    reserve = true,
                    reserveSpecified = true,
                    wait = false,
                    expiry = int.Parse(ConfigurationManager.AppSettings["CouponExpiryDays"]),
                    expirySpecified = true
                };

            if (!String.IsNullOrEmpty(request.ProductId))
                createCouponRequest.extra_trans_data = new KeyValuePair[] { new KeyValuePair() { key = "product_id", value = request.ProductId } };

            createcoupon utibaCreateCouponRequest = new createcoupon()
            {
                createcouponRequest = createCouponRequest
            };
            if (request.Recipient != null)
            {
                utibaCreateCouponRequest.createcouponRequest.recipient = request.Recipient;
            }
            createcouponResponse utibaCreateCouponResponse = utibaClientProxy.createcoupon(utibaCreateCouponRequest);

            if (utibaCreateCouponResponse != null)
            {
                response = new CreateMoviPinResponseBody()
                {
                    ResponseCode = Utils.BuildResponseCode(utibaCreateCouponResponse.createcouponReturn.result, utibaCreateCouponResponse.createcouponReturn.result_namespace),
                    ResponseMessage = utibaCreateCouponResponse.createcouponReturn.result_message,
                    TransactionID = utibaCreateCouponResponse.createcouponReturn.transid,
                    MoviPin = utibaCreateCouponResponse.createcouponReturn.couponid,
                    Fee = utibaCreateCouponResponse.createcouponReturn.fee,
                    ExpiryDate = DateTime.Now.Date.AddDays(int.Parse(ConfigurationManager.AppSettings["CouponExpiryDays"]))
                };
            }
            return (response);
        }
    }
}