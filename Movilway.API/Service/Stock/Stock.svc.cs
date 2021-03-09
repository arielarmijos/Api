using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Movilway.API.Service.External;
using Movilway.API.Service.Stock.Internal;
using Movilway.API.Utiba;
using System.ServiceModel.Channels;
using System.Net;
using Movilway.API.Service.Stock.External;
using Movilway.API.Service.Internal;

namespace Movilway.API.Service.Stock
{
    [ServiceBehavior(Namespace="http://api.movilway.net", ConcurrencyMode=ConcurrencyMode.Multiple, InstanceContextMode=InstanceContextMode.Single)]
    public class Stock : ApiServiceBase, IStock
    {
        public LoginResponse Login(LoginRequest loginRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Stock.Login", Logger.LoggingLevelType.Medium);
            LoginResponse response = null;
            try
            {
                Log(Logger.LogMessageType.Info, String.Format("Llamando a Stock.Login con los parametros: User={0}, Password={1}, DeviceType={2}", loginRequest.Request.AccessId, loginRequest.Request.Password, loginRequest.Request.AccessType), Logger.LoggingLevelType.Low);
                response = AuthenticationProvider.Login(loginRequest);
                Log(Logger.LogMessageType.Info, String.Format("Parametros de respuesta de Stock.Login: LoginResult={0}, Message={1}", response.Response.LoginResult, response.Response.Message), Logger.LoggingLevelType.Low);

            }
            catch (Exception e)
            {
                Log(Logger.LogMessageType.Error, "Excepcion en el metodo Stock.Login: " + e.ToString(), Logger.LoggingLevelType.Low);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Stock.Login", Logger.LoggingLevelType.Medium);
            return (response);
        }

        //LoginRequestInternal loginRequest = new LoginRequestInternal()
        //{
        //    DeviceType = externalRequest.Request.DeviceType,
        //    Password = externalRequest.Request.Password,
        //    User = externalRequest.Request.Username
        //};
        //LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);

        public PayStockResponse PayStock(PayStockRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Stock.PayStock", Logger.LoggingLevelType.Medium);
            PayStockRequestInternal internalRequest = new PayStockRequestInternal()
            {
                SessionID = externalRequest.Request.SessionID,
                Amount = externalRequest.Request.Amount,
                DeviceType = externalRequest.Request.DeviceType,
                Bank = externalRequest.Request.Bank,
                Account = externalRequest.Request.Account,
                Voucher = externalRequest.Request.Voucher
            };
            PayStockResponseInternal internalResponse = PayStockInternal(internalRequest);
            PayStockResponse externalResponse = new PayStockResponse()
            {
                Response = new PayStockResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID,
                    Fee = internalResponse.Fee
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Stock.PayStock", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }
        public PayStockExtendedResponse PayStockExtended(PayStockExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Stock.PayStock", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            PayStockRequestInternal internalRequest = new PayStockRequestInternal()
            {
                SessionID = loginResponse.SessionID,
                Amount = externalRequest.Request.Amount,
                DeviceType = externalRequest.Request.DeviceType,
                Bank = externalRequest.Request.Bank,
                Account = externalRequest.Request.Account,
                Voucher = externalRequest.Request.Voucher
            };
            PayStockResponseInternal internalResponse = PayStockInternal(internalRequest);
            PayStockExtendedResponse externalResponse = new PayStockExtendedResponse()
            {
                Response = new PayStockExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID,
                    Fee = internalResponse.Fee
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Stock.PayStock", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public BuyStockResponse BuyStock(BuyStockRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Stock.BuyStock", Logger.LoggingLevelType.Medium);
            BuyStockRequestInternal internalRequest = new BuyStockRequestInternal()
            {
                SessionID=externalRequest.Request.SessionID,
                Amount = externalRequest.Request.Amount,
                DeviceType = externalRequest.Request.DeviceType
            };
            BuyStockResponseInternal internalResponse = BuyStockInternal(internalRequest);
            BuyStockResponse externalResponse = new BuyStockResponse()
            {
                Response = new BuyStockResponseBody()
                {
                    ResponseCode=internalResponse.ResponseCode,
                    ResponseMessage=internalResponse.ResponseMessage,
                    TransactionID=internalResponse.TransactionID
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Stock.BuyStock", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }
        public BuyStockExtendedResponse BuyStockExtended(BuyStockExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Stock.BuyStock", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            BuyStockRequestInternal internalRequest = new BuyStockRequestInternal()
            {
                SessionID = loginResponse.SessionID,
                Amount = externalRequest.Request.Amount,
                DeviceType = externalRequest.Request.DeviceType
            };
            BuyStockResponseInternal internalResponse = BuyStockInternal(internalRequest);
            BuyStockExtendedResponse externalResponse = new BuyStockExtendedResponse()
            {
                Response = new BuyStockExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Stock.BuyStock", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public TopUpResponse TopUp(TopUpRequest topUpRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Stock.TopUp", Logger.LoggingLevelType.Medium);
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
                TransactionID = internalResponse.TransactionID,
                HostTransRef = internalResponse.HostTransRef,
                Fee = internalResponse.Fee,
                BalanceStock = internalResponse.BalanceStock
            };
            response.Response = responseBody;
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Stock.TopUp", Logger.LoggingLevelType.Medium);
            return (response);
        }
        public TopUpExtendedResponse TopUpExtended(TopUpExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Stock.TopUp", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            TopUpRequestInternal internalObject = new TopUpRequestInternal()
            {
                Amount = externalRequest.Request.Amount,
                DeviceType = externalRequest.Request.DeviceType,
                HostTransRef = externalRequest.Request.HostTransRef,
                MNO = externalRequest.Request.MNO,
                MNODefinedID = externalRequest.Request.MNODefinedID,
                Recipient = externalRequest.Request.Recipient,
                SessionID = loginResponse.SessionID
            };
            TopUpResponseInternal internalResponse = TopUpProvider.TopUpInternal(internalObject);
            TopUpExtendedResponse response = new TopUpExtendedResponse()
            {
                Response = new TopUpExtendedResponseBody()
                    {
                        ResponseCode = internalResponse.ResponseCode,
                        ResponseMessage = internalResponse.ResponseMessage,
                        TransactionID = internalResponse.TransactionID,
                        HostTransRef = internalResponse.HostTransRef,
                        Fee = internalResponse.Fee,
                        BalanceStock = internalResponse.BalanceStock
                    }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Stock.TopUp", Logger.LoggingLevelType.Medium);
            return (response);
        }

        public TransferStockResponse TransferStock(TransferStockRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Stock.TransferStock", Logger.LoggingLevelType.Medium);
            TransferStockRequestInternal internalRequest = new TransferStockRequestInternal()
            {
                Agent = externalRequest.Request.Agent,
                Amount = externalRequest.Request.Amount,
                DeviceType = externalRequest.Request.DeviceType,
                SessionID = externalRequest.Request.SessionID
            };
            TransferStockResponseInternal internalResponse = TransferStockInternal(internalRequest);
            TransferStockResponse externalResponse = new TransferStockResponse()
            {
                Response = new TransferStockResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID,
                    Fee = internalResponse.Fee
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Stock.TransferStock", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }
        public TransferStockExtendedResponse TransferStockExtended(TransferStockExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Stock.TransferStock", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            TransferStockRequestInternal internalRequest = new TransferStockRequestInternal()
            {
                Agent = externalRequest.Request.Agent,
                Amount = externalRequest.Request.Amount,
                DeviceType = externalRequest.Request.DeviceType,
                SessionID = loginResponse.SessionID
            };
            TransferStockResponseInternal internalResponse = TransferStockInternal(internalRequest);
            TransferStockExtendedResponse externalResponse = new TransferStockExtendedResponse()
            {
                Response = new TransferStockExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID,
                    Fee = internalResponse.Fee
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Stock.TransferStock", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        #region Internal Methods

        private PayStockResponseInternal PayStockInternal(PayStockRequestInternal internalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Stock.PayStockInternal", Logger.LoggingLevelType.Medium);
            PayStockResponseInternal internalResponse = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                payStockResponse utibaPayStockResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos Stock.PayStockInternal: " +
                    "SessionID={0}, DeviceType={1}, Amount={2}, Bank={3}, Account={4}, Voucher={5}", internalRequest.SessionID, internalRequest.DeviceType,
                    internalRequest.Amount, internalRequest.Bank, internalRequest.Account, internalRequest.Voucher), Logger.LoggingLevelType.Low);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados Stock.PayStockInternal: " +
                    "SessionID={0}, DeviceType={1}, Amount={2}, Bank={3}, Account={4}, Voucher={5}", internalRequest.SessionID, internalRequest.DeviceType,
                    internalRequest.Amount, internalRequest.Bank, internalRequest.Account, internalRequest.Voucher), Logger.LoggingLevelType.Low);
                    utibaPayStockResponse = utibaClient.payStock(new payStock()
                    {
                        payStockRequest = new payStockRequestType()
                        {
                            sessionid = internalRequest.SessionID,
                            device_type = internalRequest.DeviceType,
                            wait = false,
                            waitSpecified = true,
                            amount = internalRequest.Amount,
                            details = " Banco: " +  internalRequest.Bank + 
                            " Cuenta: " +  internalRequest.Account + 
                            " N°Depósito: " +  internalRequest.Voucher
                        }
                    });

                }
                if (utibaPayStockResponse != null)
                {
                    internalResponse = new PayStockResponseInternal()
                    {
                        ResponseCode = utibaPayStockResponse.payStockReturn.result,
                        ResponseMessage = utibaPayStockResponse.payStockReturn.result_message,
                        Fee = utibaPayStockResponse.payStockReturn.fee,
                        TransactionID = utibaPayStockResponse.payStockReturn.transid
                    };
                }
                Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido Stock.PayStockInternal: ResponseCode={0}, ResponseMessage={1}, TransactionID={2}, " +
                "Fee={3}", internalResponse.ResponseCode, internalResponse.ResponseMessage, internalResponse.TransactionID,
                internalResponse.Fee), Logger.LoggingLevelType.Low);
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo Stock.PayStockInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                return (null);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Stock.PayStockInternal", Logger.LoggingLevelType.Medium);
            return (internalResponse);
        }

        private BuyStockResponseInternal BuyStockInternal(BuyStockRequestInternal internalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Stock.BuyStockInternal", Logger.LoggingLevelType.Medium);
            BuyStockResponseInternal internalResponse = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                buyStockResponse utibaBuyStockResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos Stock.BuyStockInternal: " +
                    "SessionID={0}, DeviceType={1}, Amount={2}", internalRequest.SessionID, internalRequest.DeviceType, internalRequest.Amount), Logger.LoggingLevelType.Low);

                    utibaBuyStockResponse = utibaClient.buyStock(new buyStock()
                    {
                        buyStockRequest = new buyStockRequestType()
                        {
                            sessionid=internalRequest.SessionID,
                            amount=internalRequest.Amount,
                            device_type=internalRequest.DeviceType
                        }
                    });
                    
                }
                if (utibaBuyStockResponse != null)
                {
                    internalResponse = new BuyStockResponseInternal()
                    {
                        ResponseCode=utibaBuyStockResponse.buyStockReturn.result,
                        ResponseMessage = utibaBuyStockResponse.buyStockReturn.result_message,
                        Fee = utibaBuyStockResponse.buyStockReturn.fee,
                        TransactionID = utibaBuyStockResponse.buyStockReturn.transid
                    };
                }
                Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido Stock.BuyStockInternal: ResponseCode={0}, ResponseMessage={1}, TransactionID={2}, " +
                "Fee={3}", internalResponse.ResponseCode, internalResponse.ResponseMessage, internalResponse.TransactionID, internalResponse.Fee), Logger.LoggingLevelType.Low);
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo Stock.BuyStockInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                return (null);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Stock.BuyStockInternal", Logger.LoggingLevelType.Medium);
            return (internalResponse);
        }

        private TransferStockResponseInternal TransferStockInternal(TransferStockRequestInternal internalRequest)
        {
            TransferStockResponseInternal internalResponse = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                transferStockResponse utibaTransferStockResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);

                    utibaTransferStockResponse = utibaClient.transferStock(new transferStock()
                    {
                        transferStockRequest = new transferStockRequestType()
                        {
                            sessionid = internalRequest.SessionID,
                            amount = internalRequest.Amount.ToString(),
                            device_type = internalRequest.DeviceType,
                            to=internalRequest.Agent
                        }
                    });

                }
                if (utibaTransferStockResponse != null)
                {
                    internalResponse = new TransferStockResponseInternal()
                    {
                        ResponseCode = utibaTransferStockResponse.transferStockReturn.result,
                        ResponseMessage = utibaTransferStockResponse.transferStockReturn.result_message,
                        Fee = utibaTransferStockResponse.transferStockReturn.fee,
                        TransactionID = utibaTransferStockResponse.transferStockReturn.transid
                    };
                }
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo Stock.TransferStockInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                return (null);
            }
            return (internalResponse);
        }


        #endregion 
    }
}
