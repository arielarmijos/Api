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
    [ServiceProviderImpl(Platform=ApiTargetPlatform.Utiba, ServiceName=ApiServiceName.GetBalance)]
    public class GetBalanceProvider:AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetBalanceProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, API.Utiba.UMarketSCClient utibaClientProxy, string sessionID)
        {
            GetBalanceRequestBody request = requestObject as GetBalanceRequestBody;

            logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetBalanceProvider] [SEND-DATA] balanceRequest {sessionid=" + sessionID + ",device_type=" + request.DeviceType + "}");

            balanceResponse utibaBalanceResponse = utibaClientProxy.balance(new balance()
            {
                balanceRequest = new balanceRequestType()
                {
                    sessionid = sessionID,
                    device_type = request.DeviceType
                }
            });

            StringBuilder sb = new StringBuilder("balances={");
            foreach (var pair in utibaBalanceResponse.balanceReturn.balances)
                sb.Append("keyValuePair={key=" + pair.key + ",value=" + pair.value + "},");
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");

            logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetBalanceProvider] [RECV-DATA] balanceResponse {transid=" + utibaBalanceResponse.balanceReturn.transid + ",result=" + utibaBalanceResponse.balanceReturn.result +
                                ",result_namespace=" + utibaBalanceResponse.balanceReturn.result_namespace + ",result_message=" + utibaBalanceResponse.balanceReturn.result_message +
                                ",avail_1=" + utibaBalanceResponse.balanceReturn.avail_1 + ",avail_2=" + utibaBalanceResponse.balanceReturn.avail_2 + ",avail_3=" + utibaBalanceResponse.balanceReturn.avail_3 +
                                ",avail_5=" + utibaBalanceResponse.balanceReturn.avail_5 + ",current_1=" + utibaBalanceResponse.balanceReturn.current_1 + ",current_2=" + utibaBalanceResponse.balanceReturn.current_2 +
                                ",current_3=" + utibaBalanceResponse.balanceReturn.current_3 + ",current_5=" + utibaBalanceResponse.balanceReturn.current_5 + ",pending_1=" + utibaBalanceResponse.balanceReturn.pending_1 +
                                ",pending_2=" + utibaBalanceResponse.balanceReturn.pending_2 + ",pending_3=" + utibaBalanceResponse.balanceReturn.pending_3 + ",pending_5=" + utibaBalanceResponse.balanceReturn.pending_5 + 
                                "," + sb.ToString() + "}");
                

            GetBalanceResponseBody response = new GetBalanceResponseBody()
            {
                ResponseCode = Utils.BuildResponseCode(utibaBalanceResponse.balanceReturn.result, utibaBalanceResponse.balanceReturn.result_namespace),
                ResponseMessage = utibaBalanceResponse.balanceReturn.result_message,
                TransactionID = utibaBalanceResponse.balanceReturn.transid,
                WalletBalance = utibaBalanceResponse.balanceReturn.avail_1,
                StockBalance = utibaBalanceResponse.balanceReturn.avail_2,
                PointsBalance = utibaBalanceResponse.balanceReturn.avail_3,
                DebtBalance = utibaBalanceResponse.balanceReturn.avail_5
            };
            return (response);
        }
    }
}