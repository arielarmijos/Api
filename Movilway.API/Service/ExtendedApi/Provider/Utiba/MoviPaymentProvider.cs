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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.MoviPayment)]
    public class MoviPaymentProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(MoviPaymentProvider));
        protected override ILogger ProviderLogger { get { return logger; } }
        protected override TransactionType TransactionType { get { return TransactionType.transfercoupon; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            MoviPaymentRequestBody request = requestObject as MoviPaymentRequestBody;
            MoviPaymentResponseBody response = null;

            coupontransferRequestType couponTransferRequest = new coupontransferRequestType()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType,
                    amount = request.Amount,
                    couponid = request.MoviPin,
                    type = int.Parse(ConfigurationManager.AppSettings["CouponWalletType"]),
                    typeSpecified = true
                };

            KeyValuePair[] keyValues;
            if (!String.IsNullOrEmpty(request.ProductId))
                keyValues = new KeyValuePair[]
                                        {
                                            new KeyValuePair() { key = "host_trans_ref", value = request.ExternalTransactionReference },
                                            new KeyValuePair() { key = "dollar_amount", value = request.DollarAmount.ToString() },
                                            new KeyValuePair() { key = "exchange_rate", value = request.ExchangeRate.ToString() },
                                            new KeyValuePair() { key = "product_id", value = request.ProductId }
                                        };
            else
                keyValues = new KeyValuePair[]
                                        {
                                            new KeyValuePair() { key = "host_trans_ref", value = request.ExternalTransactionReference },
                                            new KeyValuePair() { key = "dollar_amount", value = request.DollarAmount.ToString() },
                                            new KeyValuePair() { key = "exchange_rate", value = request.ExchangeRate.ToString() }
                                        };

            couponTransferRequest.extra_trans_data = keyValues;

            coupontransferResponse utibaCouponTransferResponse = utibaClientProxy.coupontransfer(new coupontransfer()
            {
                coupontransferRequestType = couponTransferRequest
            });

            if (utibaCouponTransferResponse != null)
            {
                response = new MoviPaymentResponseBody()
                {
                    ResponseCode = Utils.BuildResponseCode(utibaCouponTransferResponse.coupontransferReturn.result, utibaCouponTransferResponse.coupontransferReturn.result_namespace),
                    ResponseMessage = utibaCouponTransferResponse.coupontransferReturn.result_message,
                    Fee = utibaCouponTransferResponse.coupontransferReturn.fee,
                    TransactionID = utibaCouponTransferResponse.coupontransferReturn.transid
                };
            }
            return (response);
        }
    }
}