using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Movilway.API.Service.Internal;
using Movilway.API.Utiba;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net;

namespace Movilway.API.Service
{
    public class BalanceProvider:ApiServiceBase
    {
        private static int _balanceTimeout;

        static BalanceProvider()
        {
            _balanceTimeout = int.Parse(ConfigurationManager.AppSettings["BalanceTimeout"] ?? ConfigurationManager.AppSettings["DefaultTimeout"]);
        }

        internal static BalanceResponseInternal BalanceInternal(BalanceRequestInternal balanceRequest)
        {
            UMarketSCClient utibaClient = new UMarketSCClient();
            BalanceResponseInternal balanceResult = null;
            try
            {
                utibaClient.InnerChannel.OperationTimeout = new TimeSpan(0, 0, _balanceTimeout);
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);

                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos BalanceProvider.BalanceInternal: SessionID={0}, DeviceType={1}", balanceRequest.SessionID, balanceRequest.DeviceType), Logger.LoggingLevelType.Low);

                    balanceResponse utibaBalanceResponse = utibaClient.balance(new Utiba.balance()
                    {
                        balanceRequest = new Utiba.balanceRequestType()
                        {
                            sessionid = balanceRequest.SessionID,
                            device_type = balanceRequest.DeviceType
                        }
                    });

                    balanceResult = new BalanceResponseInternal()
                    {
                        ResponseCode = utibaBalanceResponse.balanceReturn.result,
                        ResponseMessage = utibaBalanceResponse.balanceReturn.result_message,
                        TransactionID = utibaBalanceResponse.balanceReturn.transid,
                        WalletBalance = utibaBalanceResponse.balanceReturn.avail_1,
                        StockBalance = utibaBalanceResponse.balanceReturn.avail_2,
                        PointsBalance = utibaBalanceResponse.balanceReturn.avail_3,
                        DebtBalance = utibaBalanceResponse.balanceReturn.avail_5
                    };
                }
                if (balanceResult.ResponseCode != 0)
                    balanceResult.SetResponseNamespace(ApiResponseInternal.ResponseNamespace.BAC);
            }
            catch (Exception e)
            {
                if (balanceResult == null)
                    balanceResult = new BalanceResponseInternal();
                balanceResult.SetThrowedException(e);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido BalanceProvider.BalanceInternal: ResponseCode={0}, ResponseMessage={1}, TransactionID={2}, " +
                    "WalletBalance={3}, PointsBalance={4}, PointsBalance={5}, DebtBalance={6}", balanceResult.ResponseCode, balanceResult.ResponseMessage, balanceResult.TransactionID,
                    balanceResult.WalletBalance, balanceResult.StockBalance, balanceResult.PointsBalance, balanceResult.DebtBalance), Logger.LoggingLevelType.Low);
            return balanceResult;
        }
    }
}