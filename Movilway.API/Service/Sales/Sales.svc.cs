using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Movilway.API.Utiba;
using System.ServiceModel.Channels;
using System.Net;
using Movilway.API.Service.Sales.Internal;
using Movilway.API.Service.Sales.External;
using Movilway.API.Service.Internal;
using Movilway.API.Service.External;
using System.Configuration;

namespace Movilway.API.Service.Sales
{
    [ServiceBehavior(Namespace = "http://api.movilway.net", ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class Sales : ApiServiceBase, ISales
    {
        private int _deciveTypeForNewSaleWithExternalID;

        public Sales()
        {
            _deciveTypeForNewSaleWithExternalID = int.Parse(ConfigurationManager.AppSettings["DeciveTypeForNewSaleWithExternalID"]);
        }
        public GetSessionResponse GetSession(GetSessionRequest getSessionRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Sales.GetSession", Logger.LoggingLevelType.Medium);
            return (AuthenticationProvider.GetSession(getSessionRequest));
            //Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Sales.GetSession", Logger.LoggingLevelType.Medium);
        }
        public LoginResponse Login(LoginRequest loginRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Sales.Login", Logger.LoggingLevelType.Medium);
            Log(Logger.LogMessageType.Info, String.Format("Llamando a Sales.Login con los parametros: User={0}, Password={1}, DeviceType={2}", loginRequest.Request.AccessId, loginRequest.Request.Password, loginRequest.Request.AccessType), Logger.LoggingLevelType.Low);
            return (AuthenticationProvider.Login(loginRequest));
            //Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Sales.Login", Logger.LoggingLevelType.Medium);
        }

        public TopUpResponse TopUp(TopUpRequest topUpRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Sales.TopUp", Logger.LoggingLevelType.Medium);
            TopUpRequestInternal internalObject = new TopUpRequestInternal()
            {
                Amount = topUpRequest.Request.Amount,
                DeviceType = topUpRequest.Request.DeviceType,
                HostTransRef = topUpRequest.Request.HostTransRef,
                MNO = topUpRequest.Request.MNO,
                MNODefinedID = topUpRequest.Request.MNODefinedID,
                Recipient = topUpRequest.Request.Recipient,
                SessionID = topUpRequest.Request.SessionID
            };
            TopUpResponseInternal internalResponse = TopUpProvider.TopUpInternal(internalObject);
            TopUpResponse response = new TopUpResponse();
            TopUpResponseBody responseBody = new TopUpResponseBody()
            {
                ResponseCode = internalResponse.ResponseCode,
                ResponseMessage = internalResponse.ResponseMessage,
                TransactionID =internalResponse.TransactionID,
                HostTransRef = internalResponse.HostTransRef,
                Fee = internalResponse.Fee,
                BalanceStock = internalResponse.BalanceStock
            };
            response.Response = responseBody;
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Sales.TopUp", Logger.LoggingLevelType.Medium);
            return (response);
        }
        public TopUpExtendedResponse TopUpExtended(TopUpExtendedRequest topUpExtendedRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Sales.TopUpExtended", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = topUpExtendedRequest.Request.DeviceType,
                Password = topUpExtendedRequest.Request.Password,
                User = topUpExtendedRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);

            TopUpRequestInternal internalObject = new TopUpRequestInternal()
            {
                Amount = topUpExtendedRequest.Request.Amount,
                DeviceType = topUpExtendedRequest.Request.DeviceType,
                HostTransRef = topUpExtendedRequest.Request.HostTransRef,
                MNO = topUpExtendedRequest.Request.MNO,
                MNODefinedID = topUpExtendedRequest.Request.MNODefinedID,
                Recipient = topUpExtendedRequest.Request.Recipient,
                SessionID = loginResponse.SessionID
            };
            TopUpResponseInternal internalResponse = TopUpProvider.TopUpInternal(internalObject);
            TopUpExtendedResponse response = new TopUpExtendedResponse();
            TopUpExtendedResponseBody responseBody = new TopUpExtendedResponseBody()
            {
                ResponseCode = internalResponse.ResponseCode,
                ResponseMessage = internalResponse.ResponseMessage,
                TransactionID = internalResponse.TransactionID,
                HostTransRef = internalResponse.HostTransRef,
                Fee = internalResponse.Fee,
                BalanceStock = internalResponse.BalanceStock
            };
            response.Response = responseBody;
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Sales.TopUpExtended", Logger.LoggingLevelType.Medium);
            return (response);
        }

        public GetTransactionResponse GetTransaction(GetTransactionRequest getTransactionRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Sales.GetTransaction", Logger.LoggingLevelType.Medium);
            GetTransactionRequestInternal getTransactionRequestInternal = new GetTransactionRequestInternal()
            {
                SessionID = getTransactionRequest.Request.SessionID,
                DeviceType = getTransactionRequest.Request.DeviceType,
                ParameterType=(GetTransactionRequestInternalParameterType)getTransactionRequest.Request.ParameterType,
                ParameterValue=getTransactionRequest.Request.Parameter
            };
            GetTransactionResponseInternal internalResponse = GetTransactionInternal(getTransactionRequestInternal);
            GetTransactionResponse getTransactionResponse = new GetTransactionResponse();
            GetTransactionResponseBody responseBody = new GetTransactionResponseBody()
            {
                Amount=internalResponse.Amount,
                Recipient = internalResponse.Recipient,
                ResponseCode = internalResponse.ResponseCode,
                ResponseMessage = internalResponse.ResponseMessage,
                TransactionDate = internalResponse.TransactionDate,
                TransactionID=internalResponse.TransactionID,
                TransactionResult = internalResponse.TransactionResult,
                TransactionType = internalResponse.TransactionType,
                Initiator = internalResponse.Initiator,
                Debtor = internalResponse.Debtor,
                Creditor = internalResponse.Creditor
            };
            getTransactionResponse.Response = responseBody;
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Sales.GetTransaction", Logger.LoggingLevelType.Medium);
            return (getTransactionResponse);
        }
        public GetTransactionExtendedResponse GetTransactionExtended(GetTransactionExtendedRequest getTransactionRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Sales.GetTransactionExtended", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = getTransactionRequest.Request.DeviceType,
                Password = getTransactionRequest.Request.Password,
                User = getTransactionRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            
            GetTransactionRequestInternal getTransactionRequestInternal = new GetTransactionRequestInternal()
            {
                SessionID = loginResponse.SessionID,
                DeviceType = getTransactionRequest.Request.DeviceType,
                ParameterType = (GetTransactionRequestInternalParameterType)getTransactionRequest.Request.ParameterType,
                ParameterValue = getTransactionRequest.Request.Parameter
            };
            //switch (getTransactionRequest.Request.ParameterType)
            //{
            //    case GetTransactionRequestInternalParameterType.TransID: getTransactionRequestInternal.TransactionID = int.Parse(getTransactionRequest.Request.Parameter); break;
            //    case "HostReference": getTransactionRequestInternal.HostTransRef = getTransactionRequest.Request.Parameter; break;
            //    case "OperatorReference": getTransactionRequestInternal.HostTransRef = getTransactionRequest.Request.Parameter; break;
            //}
            GetTransactionResponseInternal internalResponse = GetTransactionInternal(getTransactionRequestInternal);
            GetTransactionExtendedResponse getTransactionResponse = new GetTransactionExtendedResponse();
            GetTransactionExtendedResponseBody responseBody = new GetTransactionExtendedResponseBody()
            {
                Amount = internalResponse.Amount,
                Recipient = internalResponse.Recipient,
                ResponseCode = internalResponse.ResponseCode,
                ResponseMessage = internalResponse.ResponseMessage,
                TransactionDate = internalResponse.TransactionDate,
                TransactionID = internalResponse.TransactionID,
                TransactionResult = internalResponse.TransactionResult,
                TransactionType = internalResponse.TransactionType,
                Initiator = internalResponse.Initiator,
                Debtor = internalResponse.Debtor,
                Creditor = internalResponse.Creditor
            };
            getTransactionResponse.Response = responseBody;
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Sales.GetTransactionExtended", Logger.LoggingLevelType.Medium);
            return (getTransactionResponse);
        }

        public NewSaleWithExternalIDResponse NewSaleWithExternalID(NewSaleWithExternalIDRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Sales.NewSaleWithExternalID", Logger.LoggingLevelType.Medium);
            TopUpRequestInternal internalRequest = new TopUpRequestInternal()
            {
                SessionID=externalRequest.Request.UserId,
                Recipient = externalRequest.Request.Customer,
                MNODefinedID = externalRequest.Request.ExternalId,
                MNO = externalRequest.Request.IdProduct,
                HostTransRef = externalRequest.Request.ExternalId,
                DeviceType =_deciveTypeForNewSaleWithExternalID,
                Amount = externalRequest.Request.Amount
            };
            TopUpResponseInternal internalResponse = TopUpProvider.TopUpInternal(internalRequest);
            NewSaleWithExternalIDResponse externalResponse = new NewSaleWithExternalIDResponse();
            NewSaleWithExternalIDResponseBody externalResponseBody = new NewSaleWithExternalIDResponseBody()
            {
                Result=internalResponse.ResponseCode.ToString()=="0"?"true":"false",
                Message = internalResponse.ResponseCode.ToString() == "0" ? "Recarga Exitosa" : "Recarga Fallida",
                IdTransaction=internalResponse.TransactionID,
                SaleData=internalResponse.ResponseMessage
            };
            externalResponse.Response = externalResponseBody;
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Sales.NewSaleWithExternalID", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }
        public NewSaleWithExternalIDExtendedResponse NewSaleWithExternalIDExtended(NewSaleWithExternalIDExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Sales.NewSaleWithExternalID", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.AccessType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.AccessId
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            TopUpRequestInternal internalRequest = new TopUpRequestInternal()
            {
                SessionID = loginResponse.SessionID,
                Recipient = externalRequest.Request.Customer,
                MNODefinedID = externalRequest.Request.ExternalId,
                MNO = externalRequest.Request.IdProduct,
                HostTransRef = externalRequest.Request.ExternalId,
                DeviceType = _deciveTypeForNewSaleWithExternalID,
                Amount = externalRequest.Request.Amount
            };
            TopUpResponseInternal internalResponse = TopUpProvider.TopUpInternal(internalRequest);
            NewSaleWithExternalIDExtendedResponse externalResponse = new NewSaleWithExternalIDExtendedResponse()
            {
                Response = new NewSaleWithExternalIDExtendedResponseBody()
                {
                    Result = internalResponse.ResponseCode.ToString() == "0" ? "true" : "false",
                    Message = internalResponse.ResponseCode.ToString() == "0" ? "Recarga Exitosa" : "Recarga Fallida",
                    IdTransaction = internalResponse.TransactionID,
                    SaleData = internalResponse.ResponseMessage
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Sales.NewSaleWithExternalID", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public SaleStateByExternalIDResponse SaleStateByExternalID(SaleStateByExternalIDRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Sales.SaleStateByExternalID", Logger.LoggingLevelType.Medium);
            GetTransactionRequestInternal internalRequest = new GetTransactionRequestInternal()
            {
                SessionID=externalRequest.Request.UserID,
                DeviceType=_deciveTypeForNewSaleWithExternalID,
                ParameterValue=externalRequest.Request.ExternalID,
                ParameterType=GetTransactionRequestInternalParameterType.HostReference
            };
            GetTransactionResponseInternal internalResponse = GetTransactionInternal(internalRequest);
            SaleStateByExternalIDResponse externalResponse = new SaleStateByExternalIDResponse();
            SaleStateByExternalIDResponseBody externalResponseBody = new SaleStateByExternalIDResponseBody()
            {
                Amount=(int)internalResponse.Amount,
                Customer = internalResponse.Recipient,
                Date=internalResponse.TransactionDate,
                ReloadState = internalResponse.TransactionResult == 0 ? "Recarga Exitosa" : "Recarga Fallida",
                ReloadStateCode = internalResponse.TransactionResult.ToString(),
                IdTransaccion=internalResponse.TransactionID.ToString()
            };
            externalResponse.Response = externalResponseBody;
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Sales.SaleStateByExternalID", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }
        public SaleStateByExternalIDExtendedResponse SaleStateByExternalIDExtended(SaleStateByExternalIDExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Sales.SaleStateByExternalID", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.AccessType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.AccessId
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            GetTransactionRequestInternal internalRequest = new GetTransactionRequestInternal()
            {
                SessionID = loginResponse.SessionID,
                DeviceType = _deciveTypeForNewSaleWithExternalID,
                ParameterValue = externalRequest.Request.ExternalID,
                ParameterType = GetTransactionRequestInternalParameterType.HostReference
            };
            GetTransactionResponseInternal internalResponse = GetTransactionInternal(internalRequest);
            SaleStateByExternalIDExtendedResponse externalResponse = new SaleStateByExternalIDExtendedResponse()
            {
                Response = new SaleStateByExternalIDExtendedResponseBody()
                    {
                        Amount = (int)internalResponse.Amount,
                        Customer = internalResponse.Recipient,
                        Date = internalResponse.TransactionDate,
                        ReloadState = internalResponse.TransactionResult == 0 ? "Recarga Exitosa" : "Recarga Fallida",
                        ReloadStateCode = internalResponse.TransactionResult.ToString(),
                        IdTransaccion = internalResponse.TransactionID.ToString()
                    }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Sales.SaleStateByExternalID", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public BalanceResponse Balance(BalanceRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Sales.Balance", Logger.LoggingLevelType.Medium);
            BalanceRequestInternal internalRequest = new BalanceRequestInternal()

            {
                SessionID = externalRequest.Request.SessionID,
                DeviceType = externalRequest.Request.DeviceType
            };
            BalanceResponseInternal internalResponse = BalanceProvider.BalanceInternal(internalRequest);
            BalanceResponse externalResponse = new BalanceResponse();
            BalanceResponseBody externalResponseBody = new BalanceResponseBody()
            {
                ResponseCode = internalResponse.ResponseCode,
                ResponseMessage = internalResponse.ResponseMessage,
                TransactionID = internalResponse.TransactionID,
                Balance = internalResponse.StockBalance
            };
            externalResponse.Response = externalResponseBody;
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Sales.Balance", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }
        public BalanceExtendedResponse BalanceExtended(BalanceExtendedRequest balanceExtendedRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Sales.BalanceExtended", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = balanceExtendedRequest.Request.DeviceType,
                Password = balanceExtendedRequest.Request.Password,
                User = balanceExtendedRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);

            BalanceRequestInternal internalObject = new BalanceRequestInternal()
            {
                SessionID = loginResponse.SessionID,
                DeviceType = balanceExtendedRequest.Request.DeviceType
            };
            BalanceResponseInternal internalResponse = BalanceProvider.BalanceInternal(internalObject);
            BalanceExtendedResponse response = new BalanceExtendedResponse();
            BalanceExtendedResponseBody responseBody = new BalanceExtendedResponseBody()
            {
                ResponseCode = internalResponse.ResponseCode,
                ResponseMessage = internalResponse.ResponseMessage,
                TransactionID = internalResponse.TransactionID,
                Balance = internalResponse.StockBalance
            };
            response.Response = responseBody;
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Sales.BalanceExtended", Logger.LoggingLevelType.Medium);
            return (response);
        }

        #region Internal Methods

        internal GetTransactionResponseInternal GetTransactionInternal(GetTransactionRequestInternal getTransactionRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Sales.GetTransactionInternal", Logger.LoggingLevelType.Medium);
            GetTransactionResponseInternal responseInternal = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                queryTransactionResponse utibaQueryTransactionResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos Sales.GetTransactionInternal: " +
                    "SessionID={0}, DeviceType={1}, ParameterType={2}, ParameterValue={3}", getTransactionRequest.SessionID, getTransactionRequest.DeviceType,
                    getTransactionRequest.ParameterType, getTransactionRequest.ParameterValue), Logger.LoggingLevelType.Low);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados Sales.GetTransactionInternal: " +
                    "SessionID={0}, DeviceType={1}, ParameterType={2}, ParameterValue={3}", getTransactionRequest.SessionID, getTransactionRequest.DeviceType,
                    getTransactionRequest.ParameterType, getTransactionRequest.ParameterValue), Logger.LoggingLevelType.Low);
                    switch (getTransactionRequest.ParameterType)
                    {
                        case GetTransactionRequestInternalParameterType.HostReference:
                            utibaQueryTransactionResponse = utibaClient.queryTransaction(new queryTransaction()
                            {
                                queryTransactionRequest = new queryTransactionRequestType()
                                {
                                    sessionid = getTransactionRequest.SessionID,
                                    device_type = getTransactionRequest.DeviceType,
                                    hostTransRef = getTransactionRequest.ParameterValue
                                }
                            });
                            break;
                        case GetTransactionRequestInternalParameterType.OperatorReference:
                            utibaQueryTransactionResponse = utibaClient.queryTransaction(new queryTransaction()
                            {
                                queryTransactionRequest = new queryTransactionRequestType()
                                {
                                    sessionid = getTransactionRequest.SessionID,
                                    device_type = getTransactionRequest.DeviceType,
                                    targetMSISDN = getTransactionRequest.ParameterValue
                                }
                            });
                            break;
                        case GetTransactionRequestInternalParameterType.TransID:
                            utibaQueryTransactionResponse = utibaClient.queryTransaction(new queryTransaction() 
                            { 
                                queryTransactionRequest = new queryTransactionRequestType() 
                                { 
                                    sessionid = getTransactionRequest.SessionID, 
                                    device_type = getTransactionRequest.DeviceType, 
                                    ID = getTransactionRequest.ParameterValue
                                } 
                            });
                            break;
                    }
                }
                if (utibaQueryTransactionResponse != null)
                {
                    responseInternal = new GetTransactionResponseInternal()
                    {
                        Amount = utibaQueryTransactionResponse.queryTransactionReturn.amount,
                        Recipient = utibaQueryTransactionResponse.queryTransactionReturn.recipient,
                        ResponseCode = utibaQueryTransactionResponse.queryTransactionReturn.result,
                        ResponseMessage = utibaQueryTransactionResponse.queryTransactionReturn.result_message,
                        TransactionDate = FromEpochToLocalTime(utibaQueryTransactionResponse.queryTransactionReturn.date),
                        TransactionID = utibaQueryTransactionResponse.queryTransactionReturn.transid,
                        TransactionResult = utibaQueryTransactionResponse.queryTransactionReturn.trans_result,
                        TransactionType = utibaQueryTransactionResponse.queryTransactionReturn.transaction_type,
                        Initiator = utibaQueryTransactionResponse.queryTransactionReturn.initiator,
                        Debtor = utibaQueryTransactionResponse.queryTransactionReturn.debtor,
                        Creditor = utibaQueryTransactionResponse.queryTransactionReturn.creditor
                    };

                }
                Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido Sales.GetTransactionInternal: ResponseCode={0}, ResponseMessage={1}, TransactionID={2}, " +
                "Amount={3}, Recipient={4}, TransactionDate={5}, TransactionResult={6}", responseInternal.ResponseCode, responseInternal.ResponseMessage, responseInternal.TransactionID, responseInternal.Amount,
                responseInternal.Recipient, responseInternal.TransactionDate, responseInternal.TransactionResult), Logger.LoggingLevelType.Low);
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo Sales.GetTransaction, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                return (null);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Sales.GetTransactionInternal", Logger.LoggingLevelType.Medium);
            return (responseInternal);
        }

        #endregion
    }
}
