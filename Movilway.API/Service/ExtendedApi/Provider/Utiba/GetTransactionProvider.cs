using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Utiba;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.Logging;
using Movilway.API.Data;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba
{
    [ServiceProviderImpl(Platform=ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.GetTransaction)]
    public class GetTransactionProvider:AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetTransactionProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, string sessionID)
        {
            GetTransactionRequestBody request = requestObject as GetTransactionRequestBody;
            GetTransactionResponseBody response = null;

            queryTransactionResponse utibaQueryTransactionResponse=null;
            switch (request.ParameterType)
            {
                case GetTransactionRequestParameterType.ExternalTransactionReference:

                    logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetTransactionProvider] [SEND-DATA] queryTransactionRequest {sessionId=" + sessionID + 
                                                            ",device_type=" + request.DeviceType + ",hostTransRef=" + (request.ParameterValue ?? "") + "}");
                    utibaQueryTransactionResponse = utibaClientProxy.queryTransaction(new queryTransaction()
                    {
                        queryTransactionRequest = new queryTransactionRequestType()
                        {
                            sessionid = sessionID,
                            device_type = request.DeviceType,
                            hostTransRef = request.ParameterValue??""                            
                        }
                    });
                    logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetTransactionProvider] [RECV-DATA] queryTransactionResponse {transid=" + utibaQueryTransactionResponse.queryTransactionReturn.transid + 
                                                ",result=" + utibaQueryTransactionResponse.queryTransactionReturn.result + ",result_namespace=" + utibaQueryTransactionResponse.queryTransactionReturn.result_namespace + 
                                                ",result_message=" + utibaQueryTransactionResponse.queryTransactionReturn.result_message + ",initiator=" + utibaQueryTransactionResponse.queryTransactionReturn.initiator + 
                                                ",creditor=" + utibaQueryTransactionResponse.queryTransactionReturn.creditor + ",debtor=" + utibaQueryTransactionResponse.queryTransactionReturn.debtor + 
                                                ",amount=" + utibaQueryTransactionResponse.queryTransactionReturn.amount + ",type=" + utibaQueryTransactionResponse.queryTransactionReturn.type + 
                                                ",external=" + utibaQueryTransactionResponse.queryTransactionReturn.external + ",date=" + utibaQueryTransactionResponse.queryTransactionReturn.date + 
                                                ",state=" + utibaQueryTransactionResponse.queryTransactionReturn.state + ",trans_result=" + utibaQueryTransactionResponse.queryTransactionReturn.trans_result + 
                                                ",trans_result_namespace=" + utibaQueryTransactionResponse.queryTransactionReturn.trans_result_namespace + 
                                                ",transaction_type=" + utibaQueryTransactionResponse.queryTransactionReturn.transaction_type + 
                                                ",recipient=" + utibaQueryTransactionResponse.queryTransactionReturn.recipient + "}");
                    response = MapUtibaQueryTransactionResponseToGetTransactionResponseBody(utibaQueryTransactionResponse);
                    break;

                
                case GetTransactionRequestParameterType.TransactionID:
                    logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetTransactionProvider] [SEND-DATA] queryTransactionRequest {sessionId=" + sessionID + 
                                                            ",device_type=" + request.DeviceType + ",ID=" + (request.ParameterValue ?? "") + "}");
                    utibaQueryTransactionResponse = utibaClientProxy.queryTransaction(new queryTransaction()
                    {
                        queryTransactionRequest = new queryTransactionRequestType()
                        {
                            sessionid = sessionID,
                            device_type = request.DeviceType,
                            ID = request.ParameterValue??""                            
                        }
                    });
                    logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetTransactionProvider] [RECV-DATA] queryTransactionResponse {transid=" + utibaQueryTransactionResponse.queryTransactionReturn.transid + 
                                                ",result=" + utibaQueryTransactionResponse.queryTransactionReturn.result + ",result_namespace=" + utibaQueryTransactionResponse.queryTransactionReturn.result_namespace + 
                                                ",result_message=" + utibaQueryTransactionResponse.queryTransactionReturn.result_message + ",initiator=" + utibaQueryTransactionResponse.queryTransactionReturn.initiator + 
                                                ",creditor=" + utibaQueryTransactionResponse.queryTransactionReturn.creditor + ",debtor=" + utibaQueryTransactionResponse.queryTransactionReturn.debtor + 
                                                ",amount=" + utibaQueryTransactionResponse.queryTransactionReturn.amount + ",type=" + utibaQueryTransactionResponse.queryTransactionReturn.type + 
                                                ",external=" + utibaQueryTransactionResponse.queryTransactionReturn.external + ",date=" + utibaQueryTransactionResponse.queryTransactionReturn.date + 
                                                ",state=" + utibaQueryTransactionResponse.queryTransactionReturn.state + ",trans_result=" + utibaQueryTransactionResponse.queryTransactionReturn.trans_result + 
                                                ",trans_result_namespace=" + utibaQueryTransactionResponse.queryTransactionReturn.trans_result_namespace + 
                                                ",transaction_type=" + utibaQueryTransactionResponse.queryTransactionReturn.transaction_type + 
                                                ",recipient=" + utibaQueryTransactionResponse.queryTransactionReturn.recipient + "}");
                    response = MapUtibaQueryTransactionResponseToGetTransactionResponseBody(utibaQueryTransactionResponse);
                    break;
                case GetTransactionRequestParameterType.TargetAgent:
                    logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetTransactionProvider] [SEND-DATA] queryTransactionRequest {sessionId=" + sessionID + 
                                                            ",device_type=" + request.DeviceType + ",targetMSISDN=" + request.ParameterValue + ",transactionType=buy}");
                    utibaQueryTransactionResponse = utibaClientProxy.queryTransaction(new queryTransaction()
                    {
                        queryTransactionRequest = new queryTransactionRequestType()
                        {
                            sessionid = sessionID,
                            device_type = request.DeviceType,
                            targetMSISDN=request.ParameterValue,
                            transactionType="buy"
                        }
                    });
                    logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetTransactionProvider] [RECV-DATA] queryTransactionResponse {transid=" + utibaQueryTransactionResponse.queryTransactionReturn.transid + 
                                                ",result=" + utibaQueryTransactionResponse.queryTransactionReturn.result + ",result_namespace=" + utibaQueryTransactionResponse.queryTransactionReturn.result_namespace + 
                                                ",result_message=" + utibaQueryTransactionResponse.queryTransactionReturn.result_message + ",initiator=" + utibaQueryTransactionResponse.queryTransactionReturn.initiator + 
                                                ",creditor=" + utibaQueryTransactionResponse.queryTransactionReturn.creditor + ",debtor=" + utibaQueryTransactionResponse.queryTransactionReturn.debtor + 
                                                ",amount=" + utibaQueryTransactionResponse.queryTransactionReturn.amount + ",type=" + utibaQueryTransactionResponse.queryTransactionReturn.type + 
                                                ",external=" + utibaQueryTransactionResponse.queryTransactionReturn.external + ",date=" + utibaQueryTransactionResponse.queryTransactionReturn.date + 
                                                ",state=" + utibaQueryTransactionResponse.queryTransactionReturn.state + ",trans_result=" + utibaQueryTransactionResponse.queryTransactionReturn.trans_result + 
                                                ",trans_result_namespace=" + utibaQueryTransactionResponse.queryTransactionReturn.trans_result_namespace + 
                                                ",transaction_type=" + utibaQueryTransactionResponse.queryTransactionReturn.transaction_type + 
                                                ",recipient=" + utibaQueryTransactionResponse.queryTransactionReturn.recipient + "}");
                    response = MapUtibaQueryTransactionResponseToGetTransactionResponseBody(utibaQueryTransactionResponse);
                    break;
                case GetTransactionRequestParameterType.TransactionType:
                    logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetTransactionProvider] [SEND-DATA] queryTransactionRequest {sessionId=" + sessionID + ",device_type=" + request.DeviceType + 
                                                            ",targetMSISDN=" + request.AuthenticationData.Username + ",transactionType=" + request.ParameterValue + "}");
                    utibaQueryTransactionResponse = utibaClientProxy.queryTransaction(new queryTransaction()
                    {
                        queryTransactionRequest = new queryTransactionRequestType()
                        {
                            sessionid = sessionID,
                            device_type = request.DeviceType,
                            targetMSISDN = request.AuthenticationData.Username,
                            transactionType = request.ParameterValue
                        }
                    });
                    logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetTransactionProvider] [RECV-DATA] queryTransactionResponse {transid=" + utibaQueryTransactionResponse.queryTransactionReturn.transid + 
                                                ",result=" + utibaQueryTransactionResponse.queryTransactionReturn.result + ",result_namespace=" + utibaQueryTransactionResponse.queryTransactionReturn.result_namespace + 
                                                ",result_message=" + utibaQueryTransactionResponse.queryTransactionReturn.result_message + ",initiator=" + utibaQueryTransactionResponse.queryTransactionReturn.initiator + 
                                                ",creditor=" + utibaQueryTransactionResponse.queryTransactionReturn.creditor + ",debtor=" + utibaQueryTransactionResponse.queryTransactionReturn.debtor + 
                                                ",amount=" + utibaQueryTransactionResponse.queryTransactionReturn.amount + ",type=" + utibaQueryTransactionResponse.queryTransactionReturn.type + 
                                                ",external=" + utibaQueryTransactionResponse.queryTransactionReturn.external + ",date=" + utibaQueryTransactionResponse.queryTransactionReturn.date + 
                                                ",state=" + utibaQueryTransactionResponse.queryTransactionReturn.state + ",trans_result=" + utibaQueryTransactionResponse.queryTransactionReturn.trans_result + 
                                                ",trans_result_namespace=" + utibaQueryTransactionResponse.queryTransactionReturn.trans_result_namespace + 
                                                ",transaction_type=" + utibaQueryTransactionResponse.queryTransactionReturn.transaction_type + 
                                                ",recipient=" + utibaQueryTransactionResponse.queryTransactionReturn.recipient + "}");
                    response = MapUtibaQueryTransactionResponseToGetTransactionResponseBody(utibaQueryTransactionResponse);
                    break;
                default:
                    logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetTransactionProvider] [SEND-DATA] queryTransactionRequest {sessionId=" + sessionID + "}");
                    utibaQueryTransactionResponse = utibaClientProxy.queryTransaction(new queryTransaction()
                    {
                        queryTransactionRequest = new queryTransactionRequestType()
                        {
                            sessionid = sessionID
                        }
                    });
                    logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetTransactionProvider] [RECV-DATA] queryTransactionResponse {transid=" + utibaQueryTransactionResponse.queryTransactionReturn.transid + 
                                                ",result=" + utibaQueryTransactionResponse.queryTransactionReturn.result + ",result_namespace=" + utibaQueryTransactionResponse.queryTransactionReturn.result_namespace + 
                                                ",result_message=" + utibaQueryTransactionResponse.queryTransactionReturn.result_message + ",initiator=" + utibaQueryTransactionResponse.queryTransactionReturn.initiator + 
                                                ",creditor=" + utibaQueryTransactionResponse.queryTransactionReturn.creditor + ",debtor=" + utibaQueryTransactionResponse.queryTransactionReturn.debtor + 
                                                ",amount=" + utibaQueryTransactionResponse.queryTransactionReturn.amount + ",type=" + utibaQueryTransactionResponse.queryTransactionReturn.type + 
                                                ",external=" + utibaQueryTransactionResponse.queryTransactionReturn.external + ",date=" + utibaQueryTransactionResponse.queryTransactionReturn.date + 
                                                ",state=" + utibaQueryTransactionResponse.queryTransactionReturn.state + ",trans_result=" + utibaQueryTransactionResponse.queryTransactionReturn.trans_result + 
                                                ",trans_result_namespace=" + utibaQueryTransactionResponse.queryTransactionReturn.trans_result_namespace + 
                                                ",transaction_type=" + utibaQueryTransactionResponse.queryTransactionReturn.transaction_type + 
                                                ",recipient=" + utibaQueryTransactionResponse.queryTransactionReturn.recipient + "}");
                    response = MapUtibaQueryTransactionResponseToGetTransactionResponseBody(utibaQueryTransactionResponse);
                    break;
            }
            return (response);
        }

        private GetTransactionResponseBody MapUtibaQueryTransactionResponseToGetTransactionResponseBody(queryTransactionResponse utibaQueryResponse)
        {
            return new GetTransactionResponseBody()
            {
                ResponseCode = Utils.BuildResponseCode(utibaQueryResponse.queryTransactionReturn.result, utibaQueryResponse.queryTransactionReturn.result_namespace),
                ResponseMessage = String.IsNullOrEmpty(utibaQueryResponse.queryTransactionReturn.result_message) ? "N/A" : BuildGetTransactionResponseMessage(utibaQueryResponse),
                TransactionID = utibaQueryResponse.queryTransactionReturn.transid,
                OriginalTransactionId = String.IsNullOrEmpty(utibaQueryResponse.queryTransactionReturn.result_message) ? "" : Clean(utibaQueryResponse.queryTransactionReturn.result_message.Split(' ').Last()),

                Amount = utibaQueryResponse.queryTransactionReturn.amount,
                Recipient = utibaQueryResponse.queryTransactionReturn.recipient,
                TransactionDate = UtibaUtils.FromEpochToLocalTime(utibaQueryResponse.queryTransactionReturn.date),
                TransactionResult = Utils.BuildResponseCode(utibaQueryResponse.queryTransactionReturn.trans_result, utibaQueryResponse.queryTransactionReturn.result_namespace),
                TransactionType = utibaQueryResponse.queryTransactionReturn.transaction_type,
                Initiator = utibaQueryResponse.queryTransactionReturn.initiator,
                Debtor = utibaQueryResponse.queryTransactionReturn.debtor,
                Creditor = utibaQueryResponse.queryTransactionReturn.creditor
            };
        }

        private string BuildGetTransactionResponseMessage(queryTransactionResponse utibaQueryResponse)
        {
            return "::Movilway:: ;; Agente Afiliado: " + utibaQueryResponse.queryTransactionReturn.initiator
                    + "; Fecha: " + UtibaUtils.FromEpochToLocalTime(utibaQueryResponse.queryTransactionReturn.date).ToString("dd-MM-yyyy hh:mm:ss tt")
                    + "; Transaccion: " + (String.IsNullOrEmpty(utibaQueryResponse.queryTransactionReturn.result_message) ? "" : Clean(utibaQueryResponse.queryTransactionReturn.result_message.Split(' ').Last()))
                    + "; Tipo: " + utibaQueryResponse.queryTransactionReturn.transaction_type 
                    + ";  Producto: " + utibaQueryResponse.queryTransactionReturn.creditor
                    + ";; ::Gracias por Utilizar ;         Movilway::;;";
        }

        private string Clean(string p)
        {
            StringBuilder result = new StringBuilder();
            foreach (char item in p)
                result.Append(Char.IsDigit(item) ? item : ' ');
            return result.ToString().Replace(" ","");
        }
    }
}