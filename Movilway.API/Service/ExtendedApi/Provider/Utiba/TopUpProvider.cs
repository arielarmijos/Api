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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.TopUp)]
    public class TopUpProvider:AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(TopUpProvider));
        protected override ILogger ProviderLogger { get { return logger; } }
        protected override TransactionType TransactionType { get { return TransactionType.topup; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            TopUpRequestBody request = requestObject as TopUpRequestBody;

            int timeOutSeconds = int.Parse(ConfigurationManager.AppSettings["DefaultTimeout"]);
            if (ConfigurationManager.AppSettings["TopUp_Timeout_" + request.MNO.ToLower()] != null)
                timeOutSeconds = int.Parse(ConfigurationManager.AppSettings["TopUp_Timeout_" + request.MNO.ToLower()]);

            utibaClientProxy.InnerChannel.OperationTimeout = new TimeSpan(0, 0, timeOutSeconds);


            TopUpResponseBody response = null;
            if (request.WalletType == WalletType.NotSpecified || request.WalletType == WalletType.Stock)
            {
                logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[TopUpProvider] [SEND-DATA] topupRequest {sessionid=" + sessionID + ",device_type=" + request.DeviceType + ",mno=" + request.MNO + ",amount=" + request.Amount +
                                                                    ",recipient=" + request.Recipient + ",mno_defined_id=" + request.TerminalID + ",host_trans_ref=" + request.ExternalTransactionReference + "}");

                topupResponse utibaTopUpResponse = utibaClientProxy.topup(new topup()
                {
                    topupRequest = new topupRequestType()
                    {
                        sessionid = sessionID,
                        device_type = request.DeviceType,
                        mno = request.MNO,
                        amount = request.Amount,
                        recipient = request.Recipient,
                        host_trans_ref = request.ExternalTransactionReference,
                        mno_defined_id = request.TerminalID
                    }
                });

                logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[TopUpProvider] [RECV-DATA] topupResponse {transid=" + utibaTopUpResponse.topupReturn.transid + ",result=" + utibaTopUpResponse.topupReturn.result + 
                                                                ",result_namespace=" + utibaTopUpResponse.topupReturn.result_namespace + ",result_message=" + utibaTopUpResponse.topupReturn.result_message + "}");

                response = new TopUpResponseBody()
                {
                    ResponseCode = Utils.BuildResponseCode(utibaTopUpResponse.topupReturn.result, utibaTopUpResponse.topupReturn.result_namespace),
                    ResponseMessage = utibaTopUpResponse.topupReturn.result_message,
                    TransactionID = utibaTopUpResponse.topupReturn.transid,
                    ExternalTransactionReference = request.ExternalTransactionReference, //utibaTopUpResponse.topupReturn.trans_ext_reference,
                    Fee = utibaTopUpResponse.topupReturn.fee
                };
            }
            else if (request.WalletType == WalletType.eWallet)
            {
                logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[TopUpProvider] [SEND-DATA] buyRequest {sessionid=" + sessionID + ",device_type=" + request.DeviceType + 
                                                            ",mno=" + request.MNO + ",amount=" + request.Amount + ",recipient=" + request.Recipient + "}");

                buyResponse buyResponse = utibaClientProxy.buy(new buy()
                {
                    buyRequest = new buyRequestType()
                    {
                        sessionid = sessionID,
                        device_type = request.DeviceType,
                        target = request.MNO,
                        amount = request.Amount,
                        recipient = request.Recipient
                    }
                });

                logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[TopUpProvider] [RECV-DATA] buyResponse {transid=" + buyResponse.buyReturn.transid + ",result=" + buyResponse.buyReturn.result +
                                                ",result_namespace=" + buyResponse.buyReturn.result_namespace + ",result_message=" + buyResponse.buyReturn.result_message + "}");
                
                response = new TopUpResponseBody()
                {
                    ResponseCode = Utils.BuildResponseCode(buyResponse.buyReturn.result, buyResponse.buyReturn.result_namespace),
                    ResponseMessage = buyResponse.buyReturn.result_message,
                    TransactionID = buyResponse.buyReturn.transid,
                    ExternalTransactionReference = request.ExternalTransactionReference,
                    Fee = buyResponse.buyReturn.fee
                };
            }
            AuthenticationData cascadeAuth = new AuthenticationData()
            {
                SessionID=sessionID
            };

            GetBalanceResponseBody balanceResponse = new ServiceExecutionDelegator<GetBalanceResponseBody, GetBalanceRequestBody>().ResolveRequest(new GetBalanceRequestBody()
            {
                AuthenticationData = cascadeAuth,
                DeviceType = request.DeviceType
            }, ApiTargetPlatform.Utiba, ApiServiceName.GetBalance);
            if (response != null)
            {
                response.StockBalance = balanceResponse.StockBalance.Value;
                response.WalletBalance = balanceResponse.WalletBalance.Value;
                response.PointBalance = balanceResponse.PointsBalance.Value;
            }
            return (response);
        }
    }
}