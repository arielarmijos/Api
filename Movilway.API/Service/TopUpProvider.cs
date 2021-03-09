using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Movilway.API.Service.Internal;
using Movilway.API.Utiba;
using System.ServiceModel.Channels;
using System.Net;
using System.ServiceModel;

namespace Movilway.API.Service
{
    public class TopUpProvider:ApiServiceBase
    {

        public static TopUpResponseInternal TopUpInternal(TopUpRequestInternal request)
        {
            UMarketSCClient utibaClient = new UMarketSCClient();
            TopUpResponseInternal topUpResult=null;
            try
            {
                int timeOutSeconds = int.Parse(ConfigurationManager.AppSettings["DefaultTimeout"]);
                if (ConfigurationManager.AppSettings["TopUp_Timeout_" + request.MNO.ToLower()] != null)
                    timeOutSeconds = int.Parse(ConfigurationManager.AppSettings["TopUp_Timeout_" + request.MNO.ToLower()]);

                Log(Logger.LogMessageType.Info, "-> TimeOut: " + timeOutSeconds + ", mno: " + request.MNO.ToLower(), Logger.LoggingLevelType.Low);

                utibaClient.InnerChannel.OperationTimeout = new TimeSpan(0, 10, timeOutSeconds);
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, ApiServiceBase.UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);

                    topupResponse myTopUp = utibaClient.topup(new Utiba.topup() { topupRequest = new Utiba.topupRequestType() { sessionid = request.SessionID, device_type = request.DeviceType, mno = request.MNO, amount = request.Amount, recipient = request.Recipient, host_trans_ref = request.HostTransRef, mno_defined_id = request.MNODefinedID } });


                    BalanceResponseInternal balanceResponse = BalanceProvider.BalanceInternal(new BalanceRequestInternal()
                    {
                        DeviceType = request.DeviceType,
                        SessionID = request.SessionID
                    });

                    // Remuevo el codigo de respuesta del protocolo en caso de que este disponible
                    String modifiedResultMessage = null;
                    String backendResponseCode = null;
                    if (myTopUp.topupReturn.result_message != null)
                    {
                        int namespaceIndex = myTopUp.topupReturn.result_message.IndexOf("IPR:");
                        namespaceIndex = namespaceIndex > 0 ? namespaceIndex : myTopUp.topupReturn.result_message.IndexOf("MNO:");
                        if (namespaceIndex > 0)
                        {
                            backendResponseCode = myTopUp.topupReturn.result_message.Substring(namespaceIndex + 4,
                                myTopUp.topupReturn.result_message.IndexOf(":", namespaceIndex + 5) - (namespaceIndex + 4));
                        }
                        else
                        {
                            modifiedResultMessage = myTopUp.topupReturn.result_message;
                        }
                    }

                    topUpResult = new TopUpResponseInternal()
                    {
                        ResponseCode = myTopUp.topupReturn.result,
                        ResponseMessage = modifiedResultMessage,
                        TransactionID = myTopUp.topupReturn.transid,
                        HostTransRef = request.HostTransRef,
                        Fee = myTopUp.topupReturn.fee,
                        BalanceStock = balanceResponse.StockBalance
                    };
                    Log(Logger.LogMessageType.Info, "-> HostTransRef: " + topUpResult.HostTransRef + "; Resultado Obtenido (TopUp): Result: " + topUpResult.ResponseCode + "; ResultMessage: " + topUpResult.ResponseMessage + "; TransId: " + topUpResult.TransactionID + "; HostTransRef: " + topUpResult.HostTransRef + "; Fee: " + topUpResult.Fee + ", BalanceStock: " + topUpResult.BalanceStock, Logger.LoggingLevelType.Low);
                    Log(Logger.LogMessageType.Info, "-> HostTransRef: " + topUpResult.HostTransRef + " -------------------- Termina la ejecución del método TopUp", Logger.LoggingLevelType.Low);


                }

                if (topUpResult.ResponseCode != 0)
                    topUpResult.SetResponseNamespace(ApiResponseInternal.ResponseNamespace.BAC);
            }
            catch (Exception e)
            {
                if (topUpResult == null)
                    topUpResult = new TopUpResponseInternal();
                topUpResult.SetThrowedException(e);
            }

            return topUpResult;
        }
    }
}