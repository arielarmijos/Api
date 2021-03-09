using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Movilway.API.Utiba;
using System.ServiceModel.Channels;
using System.Net;
using Movilway.API.Service.D2.External;
using Movilway.API.Service.D2.Internal;
using Movilway.API.Service.Internal;
using Movilway.API.Service.External;
using System.Configuration;
using System.Threading;

namespace Movilway.API.Service.D2
{
    [ServiceBehavior(Namespace = "http://api.movilway.net", ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class D2 : ApiServiceBase,  ID2
    {
        private int _defaultCouponType;
        private int _defaultWalletType;

        public D2()
        {
            _defaultCouponType = int.Parse(ConfigurationManager.AppSettings["DefaultCouponType"]);
            _defaultWalletType = int.Parse(ConfigurationManager.AppSettings["DefaultWalletType"]);
        }

        public LoginResponse Login(LoginRequest loginRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.Login", Logger.LoggingLevelType.Medium);
            Log(Logger.LogMessageType.Info, String.Format("Llamando a D2.Login con los parametros: User={0}, Password={1}, DeviceType={2}", loginRequest.Request.AccessId, loginRequest.Request.Password, loginRequest.Request.AccessType), Logger.LoggingLevelType.Low);
            return (AuthenticationProvider.Login(loginRequest));
            //Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Sales.Login", Logger.LoggingLevelType.Medium);
        }

        public GetSessionResponse GetSession(GetSessionRequest getSessionRequest)
        {
            Log(Logger.LogMessageType.Info, "ThreadID: " + Thread.CurrentThread.ManagedThreadId+"-", Logger.LoggingLevelType.Low);

            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.GetSession", Logger.LoggingLevelType.Medium);
            GetSessionResponse response = null;
            try
            {
                Log(Logger.LogMessageType.Info, String.Format("Llamando a AgentD2.Login con los parametros: User={0}, DeviceType={1}", getSessionRequest.Request.User, getSessionRequest.Request.DeviceType), Logger.LoggingLevelType.Low);
                LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(new LoginRequestInternal()
                {
                    DeviceType = getSessionRequest.Request.DeviceType,
                    Password = getSessionRequest.Request.Password,
                    User = getSessionRequest.Request.User
                });
                response = new GetSessionResponse()
                {
                    Response = new GetSessionResponseBody()
                    {
                        ResponseCode = loginResponse.ResponseCode,
                        ResponseMessage = loginResponse.ResponseMessage,
                        SessionID = loginResponse.SessionID,
                        TransactionID = loginResponse.TransactionID
                    }
                };
                Log(Logger.LogMessageType.Info, String.Format("Parametros de respuesta de AgentD2.Login: LoginResult={0}, Message={1} ", response.Response.ResponseCode, response.Response.ResponseMessage), Logger.LoggingLevelType.Low);

            }
            catch (Exception e)
            {
                Log(Logger.LogMessageType.Error, "Excepcion en el metodo D2.Login: " + e.ToString(), Logger.LoggingLevelType.Low);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.Login", Logger.LoggingLevelType.Medium);
            return (response);
        }

        public BuyResponse Buy(BuyRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.Buy", Logger.LoggingLevelType.Medium);
            BuyRequestInternal internalRequest = new BuyRequestInternal()
            {
                Amount = externalRequest.Request.Amount,
                DeviceType = externalRequest.Request.DeviceType,
                Recipient = externalRequest.Request.Recipient,
                SessionID = externalRequest.Request.SessionID,
                Target = externalRequest.Request.Target
            };
            BuyResponseInternal internalResponse=BuyInternal(internalRequest);
            BuyResponse externalResponse = new BuyResponse()
            {
                Response = new BuyResponseBody()
                {
                    ResponseCode=internalResponse.ResponseCode,
                    ResponseMessage=internalResponse.ResponseMessage,
                    TransactionID=internalResponse.TransactionID
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.Buy", Logger.LoggingLevelType.Medium);
            return(externalResponse);
        }

        public BuyExtendedResponse BuyExtended(BuyExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.Buy", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            BuyRequestInternal internalRequest = new BuyRequestInternal()
            {
                Amount = externalRequest.Request.Amount,
                DeviceType = externalRequest.Request.DeviceType,
                Recipient = externalRequest.Request.Recipient,
                SessionID = loginResponse.SessionID,
                Target = externalRequest.Request.Target
            };
            BuyResponseInternal internalResponse = BuyInternal(internalRequest);
            BuyExtendedResponse externalResponse = new BuyExtendedResponse()
            {
                Response = new BuyExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.Buy", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public SellResponse Sell(SellRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.Sell", Logger.LoggingLevelType.Medium);
            SellRequestInternal internalRequest = new SellRequestInternal()
            {
                Amount = externalRequest.Request.Amount,
                DeviceType = externalRequest.Request.DeviceType,
                Agent=externalRequest.Request.Agent,
                SessionID = externalRequest.Request.SessionID,
                Type=externalRequest.Request.Type
            };
            SellResponseInternal internalResponse = SellInternal(internalRequest);
            SellResponse externalResponse = new SellResponse()
            {
                Response = new SellResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.Sell", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public SellExtendedResponse SellExtended(SellExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.SellExtended", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            SellRequestInternal internalRequest = new SellRequestInternal()
            {
                Amount = externalRequest.Request.Amount,
                DeviceType = externalRequest.Request.DeviceType,
                Agent = externalRequest.Request.Agent,
                SessionID = loginResponse.SessionID,
                Type = externalRequest.Request.Type
            };
            SellResponseInternal internalResponse = SellInternal(internalRequest);
            SellExtendedResponse externalResponse = new SellExtendedResponse()
            {
                Response = new SellExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.SellExtended", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }



        public AccountPaymentExtendedResponse AccountPaymentExtended(AccountPaymentExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.AccountPayment", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            AccountPaymentRequestInternal internalRequest = new AccountPaymentRequestInternal()
            {
                Amount = externalRequest.Request.Amount.ToString(),
                DeviceType = externalRequest.Request.DeviceType,
                Agent = externalRequest.Request.Agent,
                SessionID = loginResponse.SessionID
            };
            AccountPaymentResponseInternal internalResponse = AccountPaymentInternal(internalRequest);
            AccountPaymentExtendedResponse externalResponse = new AccountPaymentExtendedResponse()
            {
                Response = new AccountPaymentExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.AccountPayment", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public AccountPaymentResponse AccountPayment(AccountPaymentRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.AccountPayment", Logger.LoggingLevelType.Medium);
            AccountPaymentRequestInternal internalRequest = new AccountPaymentRequestInternal()
            {
                Amount = externalRequest.Request.Amount.ToString(),
                DeviceType = externalRequest.Request.DeviceType,
                Agent = externalRequest.Request.Agent,
                SessionID = externalRequest.Request.SessionID,
            };
            AccountPaymentResponseInternal internalResponse = AccountPaymentInternal(internalRequest);
            AccountPaymentResponse externalResponse = new AccountPaymentResponse()
            {
                Response = new AccountPaymentResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.AccountPayment", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public CashInResponse CashIn(CashInRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.CashIn", Logger.LoggingLevelType.Medium);
            CashInRequestInternal internalRequest = new CashInRequestInternal()
            {
                Amount = externalRequest.Request.Amount,
                DeviceType = externalRequest.Request.DeviceType,
                Recipient = externalRequest.Request.Recipient,
                SessionID = externalRequest.Request.SessionID,
            };
            CashInResponseInternal internalResponse = CashInInternal(internalRequest);
            CashInResponse externalResponse = new CashInResponse()
            {
                Response = new CashInResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.CashIn", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public CashInExtendedResponse CashInExtended(CashInExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.CashIn", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            CashInRequestInternal internalRequest = new CashInRequestInternal()
            {
                Amount = externalRequest.Request.Amount,
                DeviceType = externalRequest.Request.DeviceType,
                Recipient = externalRequest.Request.Recipient,
                SessionID = loginResponse.SessionID,
            };
            CashInResponseInternal internalResponse = CashInInternal(internalRequest);
            CashInExtendedResponse externalResponse = new CashInExtendedResponse()
            {
                Response = new CashInExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.CashIn", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public CreateCouponResponse CreateCoupon(CreateCouponRequest createcouponRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.CreateCoupon", Logger.LoggingLevelType.Medium);
            CreateCouponRequestInternal internalObject = new CreateCouponRequestInternal()
            {
                Amount =  createcouponRequest.Request.Amount,
                CouponType=_defaultCouponType,
                WalletType=_defaultWalletType,
                DeviceType = createcouponRequest.Request.DeviceType,
                SessionID = createcouponRequest.Request.SessionID

            };
            CreateCouponResponseInternal internalResponse = CreateCouponInternal(internalObject);
            CreateCouponResponse response = new CreateCouponResponse();
            CreateCouponResponseBody responseBody = new CreateCouponResponseBody()
            {
                
                ResponseCode = internalResponse .ResponseCode,
                ResponseMessage = internalResponse.ResponseMessage,
                TransactionID = internalResponse.TransactionID,
                CouponID = internalResponse.CouponID,
                Fee = internalResponse.Fee,
                ResultNameSpace = internalResponse.ResultNameSpace//,
                //ScheduleID = internalResponse.ScheduleID,
                //TransExtReference = internalResponse.TransExtReference

            };
            response.Response = responseBody;
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.CreateCoupon", Logger.LoggingLevelType.Medium);
            return (response);
        }

        public CreateCouponWithRecipientResponse CreateCouponWithRecipient(CreateCouponWithRecipientRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.CreateCouponWithRecipient", Logger.LoggingLevelType.Medium);
            CreateCouponRequestInternal internalObject = new CreateCouponRequestInternal()
            {
                Amount = externalRequest.Request.Amount,
                CouponType = _defaultCouponType,
                WalletType = _defaultWalletType,
                DeviceType = externalRequest.Request.DeviceType,
                SessionID = externalRequest.Request.SessionID,
                Recipient=externalRequest.Request.Recipient
            };
            CreateCouponResponseInternal internalResponse = CreateCouponInternal(internalObject);
            CreateCouponWithRecipientResponse response = new CreateCouponWithRecipientResponse()
            {
                Response = new CreateCouponWithRecipientResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID,
                    CouponID = internalResponse.CouponID,
                    Fee = internalResponse.Fee,
                    ResultNameSpace = internalResponse.ResultNameSpace,
                    ScheduleID = internalResponse.ScheduleID,
                    TransExtReference = internalResponse.TransExtReference
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.CreateCouponWithRecipient", Logger.LoggingLevelType.Medium);
            return (response);
        }

        public CreateCouponWithRecipientExtendedResponse CreateCouponWithRecipientExtended(CreateCouponWithRecipientExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.CreateCouponWithRecipient", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            CreateCouponRequestInternal internalObject = new CreateCouponRequestInternal()
            {
                Amount = externalRequest.Request.Amount,
                CouponType = _defaultCouponType,
                WalletType = _defaultWalletType,
                DeviceType = externalRequest.Request.DeviceType,
                SessionID = loginResponse.SessionID,
                Recipient = externalRequest.Request.Recipient
            };
            CreateCouponResponseInternal internalResponse = CreateCouponInternal(internalObject);
            CreateCouponWithRecipientExtendedResponse response = new CreateCouponWithRecipientExtendedResponse()
            {
                Response = new CreateCouponWithRecipientExtendedResponseBody()
                    {
                        ResponseCode = internalResponse.ResponseCode,
                        ResponseMessage = internalResponse.ResponseMessage,
                        TransactionID = internalResponse.TransactionID,
                        CouponID = internalResponse.CouponID,
                        Fee = internalResponse.Fee,
                        ResultNameSpace = internalResponse.ResultNameSpace,
                        ScheduleID = internalResponse.ScheduleID,
                        TransExtReference = internalResponse.TransExtReference
                    }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.CreateCouponWithRecipient", Logger.LoggingLevelType.Medium);
            return (response);
        }

        public CreateCouponExtendedResponse CreateCouponExtended(CreateCouponExtendedRequest createcouponExtendedRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.CreateCouponExtended", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = createcouponExtendedRequest.Request.DeviceType,
                Password = createcouponExtendedRequest.Request.Password,
                User = createcouponExtendedRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);

            CreateCouponRequestInternal internalObject = new CreateCouponRequestInternal()
            {
                Amount = createcouponExtendedRequest.Request.Amount,
                CouponType = _defaultCouponType,
                WalletType = _defaultWalletType,
                DeviceType = createcouponExtendedRequest.Request.DeviceType,
                SessionID = loginResponse.SessionID

            };
            CreateCouponResponseInternal internalResponse = CreateCouponInternal(internalObject);
            CreateCouponExtendedResponse response = new CreateCouponExtendedResponse();
            CreateCouponExtendedResponseBody responseBody = new CreateCouponExtendedResponseBody()
            {

                ResponseCode = internalResponse.ResponseCode,
                ResponseMessage = internalResponse.ResponseMessage,
                TransactionID = internalResponse.TransactionID,
                CouponID = internalResponse.CouponID,
                Fee = internalResponse.Fee,
                ResultNameSpace = internalResponse.ResultNameSpace,
                ScheduleID = internalResponse.ScheduleID,
                TransExtReference = internalResponse.TransExtReference

            };
            response.Response = responseBody;
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.CreateCouponExtended", Logger.LoggingLevelType.Medium);
            return (response);
        }

        public CouponTransferResponse CouponTransfer(CouponTransferRequest coupontransferRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.CouponTransfer", Logger.LoggingLevelType.Medium);
            CouponTransferRequestInternal internalObject = new CouponTransferRequestInternal()
            {
                Amount = coupontransferRequest.Request.Amount,
                Type = coupontransferRequest.Request.Type,
                DeviceType = coupontransferRequest.Request.DeviceType,
                SessionID = coupontransferRequest.Request.SessionID,
                CouponID = coupontransferRequest.Request.CouponID

            };
            CouponTransferResponseInternal internalResponse = CouponTransferInternal(internalObject);
            CouponTransferResponse response = new CouponTransferResponse();
            CouponTransferResponseBody responseBody = new CouponTransferResponseBody()
            {

                ResponseCode = internalResponse.ResponseCode,
                ResponseMessage = internalResponse.ResponseMessage,
                TransactionID = internalResponse.TransactionID,
                Fee = internalResponse.Fee,
                ResultNameSpace = internalResponse.ResultNameSpace,
                ScheduleID = internalResponse.ScheduleID,
                TransExtReference = internalResponse.TransExtReference

            };
            response.Response = responseBody;
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.CouponTransfer", Logger.LoggingLevelType.Medium);
            return (response);
        }

        public CouponTransferExtendedResponse CouponTransferExtended(CouponTransferExtendedRequest coupontransferExtendedRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.CouponTransferExtended", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = coupontransferExtendedRequest.Request.DeviceType,
                Password = coupontransferExtendedRequest.Request.Password,
                User = coupontransferExtendedRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);

            CouponTransferRequestInternal internalObject = new CouponTransferRequestInternal()
            {
                Amount = coupontransferExtendedRequest.Request.Amount,
                Type = coupontransferExtendedRequest.Request.Type,
                DeviceType = coupontransferExtendedRequest.Request.DeviceType,
                SessionID = loginResponse.SessionID,
                CouponID = coupontransferExtendedRequest.Request.CouponID

            };
            CouponTransferResponseInternal internalResponse = CouponTransferInternal(internalObject);
            CouponTransferExtendedResponse response = new CouponTransferExtendedResponse();
            CouponTransferExtendedResponseBody responseBody = new CouponTransferExtendedResponseBody()
            {

                ResponseCode = internalResponse.ResponseCode,
                ResponseMessage = internalResponse.ResponseMessage,
                TransactionID = internalResponse.TransactionID,
                Fee = internalResponse.Fee,
                ResultNameSpace = internalResponse.ResultNameSpace,
                ScheduleID = internalResponse.ScheduleID,
                TransExtReference = internalResponse.TransExtReference

            };
            response.Response = responseBody;
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.CouponTransferExtended", Logger.LoggingLevelType.Medium);
            return (response);
        }

        public TransferResponse Transfer(TransferRequest transferRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.Transfer", Logger.LoggingLevelType.Medium);
            TransferRequestInternal internalObject = new TransferRequestInternal()
            {
                DeviceType = transferRequest.Request.DeviceType,
                Amount = transferRequest.Request.Amount,
                Recipient = transferRequest.Request.Recipient,
                SessionID = transferRequest.Request.SessionID
            };
            TransferResponseInternal internalResponse = TransferInternal(internalObject);
            TransferResponse response = new TransferResponse();
            TransferResponseBody responseBody = new TransferResponseBody()
            {
                ResponseCode = internalResponse.ResponseCode,
                ResponseMessage = internalResponse.ResponseMessage,
                TransactionID = internalResponse.TransactionID,
                Fee = internalResponse.Fee,
                ResultNameSpace = internalResponse.ResultNameSpace,
                ScheduleID = internalResponse.ScheduleID,
                TransExtReference = internalResponse.TransExtReference
            };
            response.Response = responseBody;
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.Transfer", Logger.LoggingLevelType.Medium);
            return (response);
        }

        public TransferExtendedResponse TransferExtended(TransferExtendedRequest transferExtendedRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.TransferExtended", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = transferExtendedRequest.Request.DeviceType,
                Password = transferExtendedRequest.Request.Password,
                User = transferExtendedRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);

            TransferRequestInternal internalObject = new TransferRequestInternal()
            {
                Amount = transferExtendedRequest.Request.Amount,
                DeviceType = transferExtendedRequest.Request.DeviceType,
                SessionID = loginResponse.SessionID,
                Recipient = transferExtendedRequest.Request.Recipient,
            };
            TransferResponseInternal internalResponse = TransferInternal(internalObject);
            TransferExtendedResponse response = new TransferExtendedResponse();
            TransferExtendedResponseBody responseBody = new TransferExtendedResponseBody()
            {
                ResponseCode = internalResponse.ResponseCode,
                ResponseMessage = internalResponse.ResponseMessage,
                TransactionID = internalResponse.TransactionID,
                Fee = internalResponse.Fee,
                ResultNameSpace = internalResponse.ResultNameSpace,
                ScheduleID = internalResponse.ScheduleID,
                TransExtReference = internalResponse.TransExtReference
            };
            response.Response = responseBody;
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.TransferExtended", Logger.LoggingLevelType.Medium);
            return (response);
        }

        public GetLastTransactionsResponse GetLastTransactions(GetLastTransactionsRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.GetLastTransactions", Logger.LoggingLevelType.Medium);
            GetLastTransactionsRequestInternal internalRequest = new GetLastTransactionsRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                SessionID = externalRequest.Request.SessionID,
                Agent=externalRequest.Request.Agent,
                Count=externalRequest.Request.Count
            };
            GetLastTransactionsResponseInternal internalResponse = GetLastTransactionsInternal(internalRequest);
            GetLastTransactionsResponse externalResponse = new GetLastTransactionsResponse()
            {
                Response = new GetLastTransactionsResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID,
                }
            };
            if (internalResponse.Transactions != null && internalResponse.Transactions.Count > 0)
            {
                externalResponse.Response.Transactions = new List<External.TransactionSummary>();
                foreach (Internal.TransactionSummaryInternal internalTran in internalResponse.Transactions)
                {
                    External.TransactionSummary externalTransaction = new External.TransactionSummary()
                    {
                        Amount=internalTran.Amount,
                        LastTimeModified=internalTran.LastTimeModified,
                        TransactionID=internalTran.TransactionID,
                        TransactionType=internalTran.TransactionType
                    };
                    if (internalTran.PartiesReferenceIDList != null && internalTran.PartiesReferenceIDList.Length > 0)
                    {
                        externalTransaction.PartiesReferenceIDList = new ArrayOfString();
                        foreach (String element in internalTran.PartiesReferenceIDList)
                        {
                            externalTransaction.PartiesReferenceIDList.Add(element);
                        }
                    }
                    externalResponse.Response.Transactions.Add(externalTransaction);
                }
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.GetLastTransactions", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public GetLastTransactionsExtendedResponse GetLastTransactionsExtended(GetLastTransactionsExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.GetLastTransactions", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            GetLastTransactionsRequestInternal internalRequest = new GetLastTransactionsRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                SessionID = loginResponse.SessionID,
                Agent = externalRequest.Request.Agent,
                Count = externalRequest.Request.Count
            };
            GetLastTransactionsResponseInternal internalResponse = GetLastTransactionsInternal(internalRequest);
            GetLastTransactionsExtendedResponse externalResponse = new GetLastTransactionsExtendedResponse()
            {
                Response = new GetLastTransactionsExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID,
                }
            };
            if (internalResponse.Transactions != null && internalResponse.Transactions.Count > 0)
            {
                externalResponse.Response.Transactions = new List<External.TransactionSummary>();
                foreach (Internal.TransactionSummaryInternal internalTran in internalResponse.Transactions)
                {
                    External.TransactionSummary externalTransaction = new External.TransactionSummary()
                    {
                        Amount = internalTran.Amount,
                        LastTimeModified = internalTran.LastTimeModified,
                        TransactionID = internalTran.TransactionID,
                        TransactionType = internalTran.TransactionType
                    };
                    if (internalTran.PartiesReferenceIDList != null && internalTran.PartiesReferenceIDList.Length > 0)
                    {
                        externalTransaction.PartiesReferenceIDList = new ArrayOfString();
                        foreach (String element in internalTran.PartiesReferenceIDList)
                        {
                            externalTransaction.PartiesReferenceIDList.Add(element);
                        }
                    }
                    externalResponse.Response.Transactions.Add(externalTransaction);
                }
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.GetLastTransactions", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public BalanceExtendedResponse BalanceExtended(BalanceExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.BalanceExtended", Logger.LoggingLevelType.Low);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            BalanceRequestInternal internalRequest = new BalanceRequestInternal()
            {
                SessionID = loginResponse.SessionID,
                DeviceType = externalRequest.Request.DeviceType
            };
            BalanceResponseInternal internalResponse = BalanceProvider.BalanceInternal(internalRequest);
            BalanceExtendedResponse externalResponse = new BalanceExtendedResponse()
            {
                Response = new BalanceExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID,


                    WalletBalance = internalResponse.WalletBalance,
                    StockBalance = internalResponse.StockBalance,
                    PointsBalance = internalResponse.PointsBalance,
                    DebtBalance = internalResponse.DebtBalance
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.Balance", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public BalanceResponse Balance(BalanceRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.Balance", Logger.LoggingLevelType.Low);
            BalanceRequestInternal internalRequest = new BalanceRequestInternal()
            {
                SessionID = externalRequest.Request.SessionID,
                DeviceType = externalRequest.Request.DeviceType
            };
            BalanceResponseInternal internalResponse = BalanceProvider.BalanceInternal(internalRequest);
            BalanceResponse externalResponse = new BalanceResponse()
            {
                Response = new BalanceResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID,


                    WalletBalance = internalResponse.WalletBalance,
                    StockBalance = internalResponse.StockBalance,
                    PointsBalance = internalResponse.PointsBalance,
                    DebtBalance = internalResponse.DebtBalance
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.Balance", Logger.LoggingLevelType.Medium);
            return(externalResponse);
        }
        
        public SummaryResponse Summary(SummaryRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.Summary", Logger.LoggingLevelType.Medium);
            SummaryRequestInternal internalRequest = new SummaryRequestInternal()
            {
                SessionID = externalRequest.Request.SessionID,
                DeviceType = externalRequest.Request.DeviceType,
                StartDate=externalRequest.Request.StartDate,
                EndDate=externalRequest.Request.EndDate,
                Target=externalRequest.Request.Target,
                WalletType=externalRequest.Request.WalletType
            };
            SummaryResponseInternal internalResponse = SummaryInternal(internalRequest);
            SummaryResponse externalResponse = new SummaryResponse()
            {
                Response = new SummaryResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID,

                    TotalAmount=internalResponse.TotalAmount,
                    TransactionCount=internalResponse.TransactionCount
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.Summary", Logger.LoggingLevelType.Medium);
            return(externalResponse);
        }

        public SummaryExtendedResponse SummaryExtended(SummaryExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.Summary", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            SummaryRequestInternal internalRequest = new SummaryRequestInternal()
            {
                SessionID = loginResponse.SessionID,
                DeviceType = externalRequest.Request.DeviceType,
                StartDate = externalRequest.Request.StartDate,
                EndDate = externalRequest.Request.EndDate,
                Target = externalRequest.Request.Target,
                WalletType = externalRequest.Request.WalletType
            };
            SummaryResponseInternal internalResponse = SummaryInternal(internalRequest);
            SummaryExtendedResponse externalResponse = new SummaryExtendedResponse()
            {
                Response = new SummaryExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID,

                    TotalAmount = internalResponse.TotalAmount,
                    TransactionCount = internalResponse.TransactionCount
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.Summary", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public BankListResponse BankList(BankListRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.BankList", Logger.LoggingLevelType.Medium);
            BankListRequestInternal internalRequest = new BankListRequestInternal()
            {
                SessionID = externalRequest.Request.SessionID,
                DeviceType = externalRequest.Request.DeviceType,
                AgentReference = externalRequest.Request.AgentReference
            };
            BankListResponseInternal internalResponse = BankListInternal(internalRequest);
            BankListResponse externalResponse = new BankListResponse()
            {
                Response = new BankListResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            if (internalResponse.Banks != null && internalResponse.Banks.Count > 0)
            {
                externalResponse.Response.BankList = new BankList();
                foreach(Internal.BankInfo bankInfo in internalResponse.Banks)
                {
                    externalResponse.Response.BankList.Add(new External.BankInfo()
                    {
                        Key=bankInfo.Key,
                        Description = bankInfo.Description
                    });
                }
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.BankList", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public BankListExtendedResponse BankListExtended(BankListExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.BankListExtended", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            BankListRequestInternal internalRequest = new BankListRequestInternal()
            {
                SessionID = loginResponse.SessionID,
                DeviceType = externalRequest.Request.DeviceType,
                AgentReference = externalRequest.Request.AgentReference
            };
            BankListResponseInternal internalResponse = BankListInternal(internalRequest);
            BankListExtendedResponse externalResponse = new BankListExtendedResponse()
            {
                Response = new BankListExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            if (internalResponse.Banks != null && internalResponse.Banks.Count > 0)
            {
                externalResponse.Response.BankList = new BankList();
                foreach(Internal.BankInfo bankInfo in internalResponse.Banks)
                {
                    externalResponse.Response.BankList.Add(new External.BankInfo()
                    {
                        Key=bankInfo.Key,
                        Description = bankInfo.Description
                    });
                }
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.BankListExtended", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        #region Internal Methods
        internal CreateCouponResponseInternal CreateCouponInternal(CreateCouponRequestInternal request)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.CreateCouponInternal", Logger.LoggingLevelType.Medium);
            CreateCouponResponseInternal createcouponResult = null; 
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos D2.CreateCouponInternal: SessionID={0}, DeviceType={1}, Amount={2}, WalletType={3}, CouponType={4}, Recipient={5}", request.SessionID, request.DeviceType, request.Amount, request.WalletType,request.CouponType, request.Recipient), Logger.LoggingLevelType.Low);
                    createcoupon utibaCreateCouponRequest = new Utiba.createcoupon()
                    {
                        createcouponRequest = new Utiba.createcouponRequestType()
                        {
                            sessionid = request.SessionID,
                            device_type = request.DeviceType,
                            amount = request.Amount,
                            amountSpecified = true,
                            wallet_type = request.WalletType,
                            wallet_typeSpecified = true,
                            coupon_typeSpecified = true,
                            coupon_type = request.CouponType,
                            reserve = true,
                            reserveSpecified = true,
                            wait = false,
                            expiry=180,
                            expirySpecified=true
                        }
                    };
                    if (request.Recipient != null)
                    {
                        utibaCreateCouponRequest.createcouponRequest.recipient = request.Recipient;
                    }
                    //Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados D2.CreateCouponInternal: SessionID={0}, DeviceType={1}, Amount={2}, WalletType={3}, CouponType={4}, Recipient={5}", request.SessionID, request.CouponType, request.DeviceType, request.Amount, request.WalletType, request.Recipient), Logger.LoggingLevelType.Low);
                    createcouponResponse myCreateCoupon = utibaClient.createcoupon(utibaCreateCouponRequest);
                    createcouponResult = new CreateCouponResponseInternal()
                    {
                        ResponseCode = myCreateCoupon.createcouponReturn.result,
                        ResponseMessage = myCreateCoupon.createcouponReturn.result_message,
                        TransactionID = myCreateCoupon.createcouponReturn.transid,
                        CouponID = myCreateCoupon.createcouponReturn.couponid,
                        Fee = myCreateCoupon.createcouponReturn.fee,
                        ResultNameSpace = myCreateCoupon.createcouponReturn.result_namespace,
                        ScheduleID = myCreateCoupon.createcouponReturn.schedule_id,
                        TransExtReference = myCreateCoupon.createcouponReturn.trans_ext_reference
                    };
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido D2.CreateCouponInternal: ResponseCode={0}, ResponseMessage={1}, TransactionID={2}, CouponID={3}, " +
                    "Fee={4}, ResultNameSpace={5}, ScheduleID={6}, TransExtReference={7}", createcouponResult.ResponseCode, createcouponResult.ResponseMessage, createcouponResult.TransactionID, createcouponResult.CouponID,
                    createcouponResult.Fee, createcouponResult.ResultNameSpace, createcouponResult.ScheduleID, createcouponResult.TransExtReference), Logger.LoggingLevelType.Low);

                    if (createcouponResult.ResponseCode != 0)
                        createcouponResult.SetResponseNamespace(ApiResponseInternal.ResponseNamespace.BAC);
                }
            }
            catch(Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo D2.CreateCouponInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                if (createcouponResult == null)
                    createcouponResult = new CreateCouponResponseInternal();
                createcouponResult.SetThrowedException(ex);
            }

            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.CreateCouponInternal", Logger.LoggingLevelType.Medium);
            return createcouponResult;
        }
            
        internal CouponTransferResponseInternal CouponTransferInternal(CouponTransferRequestInternal request)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.CouponTransferInternal", Logger.LoggingLevelType.Medium);
            CouponTransferResponseInternal coupontransferResult = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos D2.CouponTransferInternal: SessionID={0}, DeviceType={1}, Amount={2}, " +
                    "CouponId={3}, CouponType={4}", request.SessionID, request.DeviceType, request.Amount, request.CouponID, request.Type), Logger.LoggingLevelType.Low);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados D2.CouponTransferInternal: SessionID={0}, DeviceType={1}, Amount={2}, " +
                    "CouponId={3}, CouponType={4}", request.SessionID, request.DeviceType, request.Amount, request.CouponID, request.Type), Logger.LoggingLevelType.Low);
                    coupontransferResponse myCouponTransfer = utibaClient.coupontransfer(new Utiba.coupontransfer() { coupontransferRequestType = new coupontransferRequestType() { sessionid = request.SessionID, device_type = request.DeviceType, amount = request.Amount, couponid = request.CouponID, type = request.Type, typeSpecified = true } });
                    coupontransferResult = new CouponTransferResponseInternal()
                    {
                        ResponseCode = myCouponTransfer.coupontransferReturn.result,
                        ResponseMessage = myCouponTransfer.coupontransferReturn.result_message,
                        TransactionID = myCouponTransfer.coupontransferReturn.transid,
                        Fee = myCouponTransfer.coupontransferReturn.fee,
                        ResultNameSpace = myCouponTransfer.coupontransferReturn.result_namespace,
                        ScheduleID = myCouponTransfer.coupontransferReturn.schedule_id,
                        TransExtReference = myCouponTransfer.coupontransferReturn.trans_ext_reference
                    };
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido D2.CouponTransferInternal: ResponseCode={0}, ResponseMessage={1}, TransactionID={2}, " +
                    "Fee={3}, ResultNameSpace={4}, ScheduleID={5}, TransExtReference={6}", coupontransferResult.ResponseCode, coupontransferResult.ResponseMessage, coupontransferResult.TransactionID,
                    coupontransferResult.Fee, coupontransferResult.ResultNameSpace, coupontransferResult.ScheduleID, coupontransferResult.TransExtReference), Logger.LoggingLevelType.Low);

                    if (coupontransferResult.ResponseCode != 0)
                        coupontransferResult.SetResponseNamespace(ApiResponseInternal.ResponseNamespace.BAC);
                }
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo D2.CouponTransferInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                if (coupontransferResult == null)
                    coupontransferResult = new CouponTransferResponseInternal();
                coupontransferResult.SetThrowedException(ex);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.CouponTransferInternal", Logger.LoggingLevelType.Medium);
            return coupontransferResult;
        }

        private BuyResponseInternal BuyInternal(BuyRequestInternal internalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.BuyInternal", Logger.LoggingLevelType.Medium);
            BuyResponseInternal internalResponse = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                buyResponse utibaBuyResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos D2.BuyInternal: SessionID={0}, DeviceType={1}, Amount={2}, " +
                    "Recipient={3}, Target={4}", internalRequest.SessionID, internalRequest.DeviceType, internalRequest.Amount, internalRequest.Recipient, internalRequest.Target), Logger.LoggingLevelType.Low);
                    //Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados D2.BuyInternal: SessionID={0}, DeviceType={1}, Amount={2}, " +
                    //"Recipient={3}, Target={4}, Type={5}", internalRequest.SessionID, internalRequest.DeviceType, internalRequest.Amount, internalRequest.Recipient, internalRequest.Target,"1"), Logger.LoggingLevelType.Low);
                    utibaBuyResponse = utibaClient.buy(new buy()
                    {
                        buyRequest = new buyRequestType()
                        {
                            sessionid = internalRequest.SessionID,
                            device_type = internalRequest.DeviceType,
                            amount=internalRequest.Amount,
                            target=internalRequest.Target,
                            recipient=internalRequest.Recipient,
                            type=1,
                            typeSpecified=true
                        }
                    });
                    internalResponse = new BuyResponseInternal()
                    {
                        ResponseCode = utibaBuyResponse.buyReturn.result,
                        ResponseMessage = utibaBuyResponse.buyReturn.result_message,
                        TransactionID = utibaBuyResponse.buyReturn.transid
                    };
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido D2.BuyInternal: ResponseCode={0}, ResponseMessage={1}, TransactionID={2}"
                    , internalResponse.ResponseCode, internalResponse.ResponseMessage, internalResponse.TransactionID), Logger.LoggingLevelType.Low);

                    if (internalResponse.ResponseCode != 0)
                        internalResponse.SetResponseNamespace(ApiResponseInternal.ResponseNamespace.BAC);
                }
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo D2.BuyInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                if (internalResponse == null)
                    internalResponse = new BuyResponseInternal();
                internalResponse.SetThrowedException(ex);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.BuyInternal", Logger.LoggingLevelType.Medium);
            return (internalResponse);
        }
        
        internal TransferResponseInternal TransferInternal(TransferRequestInternal request)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.TransferInternal", Logger.LoggingLevelType.Medium);
            TransferResponseInternal transferResult = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos D2.TransferInternal: SessionID={0}, DeviceType={1}, Amount={2}, " +
                    "Recipient={3}", request.SessionID, request.DeviceType, request.Amount, request.Recipient), Logger.LoggingLevelType.Low);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados D2.TransferInternal: SessionID={0}, DeviceType={1}, Amount={2}, " +
                    "Recipient={3}", request.SessionID, request.DeviceType, request.Amount, request.Recipient), Logger.LoggingLevelType.Low);
                    transferResponse myTransfer = utibaClient.transfer(new Utiba.transfer() { transferRequest = new Utiba.transferRequestType() { sessionid = request.SessionID, device_type = request.DeviceType, amount = request.Amount, to = request.Recipient, suppress_confirm = true, suppress_confirmSpecified = true } });
                    transferResult = new TransferResponseInternal()
                    {
                        ResponseCode = myTransfer.transferReturn.result,
                        ResponseMessage = myTransfer.transferReturn.result_message,
                        TransactionID = myTransfer.transferReturn.transid,
                        Fee = myTransfer.transferReturn.fee,
                        ResultNameSpace = myTransfer.transferReturn.result_namespace,
                        ScheduleID = myTransfer.transferReturn.schedule_id,
                        TransExtReference = myTransfer.transferReturn.trans_ext_reference
                    };
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido D2.TransferInternal: ResponseCode={0}, ResponseMessage={1}, TransactionID={2}, " +
                    "Fee={3},  ResultNameSpace={4}, ScheduleID ={5}", transferResult.ResponseCode, transferResult.ResponseMessage, transferResult.TransactionID, transferResult.Fee,
                    transferResult.ResultNameSpace, transferResult.ScheduleID, transferResult.TransExtReference), Logger.LoggingLevelType.Low);

                    if (transferResult.ResponseCode != 0)
                        transferResult.SetResponseNamespace(ApiResponseInternal.ResponseNamespace.BAC);
                }
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo D2.TransferInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                if (transferResult == null)
                    transferResult = new TransferResponseInternal();
                transferResult.SetThrowedException(ex);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.TransferInternal", Logger.LoggingLevelType.Medium);
            return transferResult;
        }

        private SellResponseInternal SellInternal(SellRequestInternal internalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.SellInternal", Logger.LoggingLevelType.Medium);
            SellResponseInternal internalResponse = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                utibaClient.InnerChannel.OperationTimeout = new TimeSpan(0, 5, 0);
                sellResponse utibaSellResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos D2.SellInternal: SessionID={0}, DeviceType={1}, Amount={2}, " +
                    "To={3}", internalRequest.SessionID, internalRequest.DeviceType, internalRequest.Amount, internalRequest.Agent), Logger.LoggingLevelType.Low);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados D2.SellInternal: SessionID={0}, DeviceType={1}, Amount={2}, " +
                    "To={3}, Type={4}", internalRequest.SessionID, internalRequest.DeviceType, internalRequest.Amount, internalRequest.Agent, "1"), Logger.LoggingLevelType.Low);
                    utibaSellResponse = utibaClient.sell(new sell()
                    {
                        sellRequest = new sellRequestType()
                        {
                            sessionid = internalRequest.SessionID,
                            device_type = internalRequest.DeviceType,
                            amount = internalRequest.Amount,
                            to=internalRequest.Agent,
                            wait=true,
                            waitSpecified=true,
                            type = 1,
                            typeSpecified = true
                        }
                    });
                    internalResponse = new SellResponseInternal()
                    {
                        ResponseCode = utibaSellResponse.sellReturn.result,
                        ResponseMessage = utibaSellResponse.sellReturn.result_message,
                        TransactionID = utibaSellResponse.sellReturn.transid
                    };
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido D2.SellInternal: ResponseCode={0}, ResponseMessage={1}, TransactionID={2}"
                    , internalResponse.ResponseCode, internalResponse.ResponseMessage, internalResponse.TransactionID), Logger.LoggingLevelType.Low);

                    if (internalResponse.ResponseCode != 0)
                        internalResponse.SetResponseNamespace(ApiResponseInternal.ResponseNamespace.BAC);
                }
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo D2.SellInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                if (internalResponse == null)
                    internalResponse = new SellResponseInternal();
                internalResponse.SetThrowedException(ex);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.SellInternal", Logger.LoggingLevelType.Medium);
            return (internalResponse);
        }

        private AccountPaymentResponseInternal AccountPaymentInternal(AccountPaymentRequestInternal internalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.AccountPaymentInternal", Logger.LoggingLevelType.Medium);
            AccountPaymentResponseInternal internalResponse = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                accountPaymentResponse utibaAccountPaymentResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos D2.AccountPaymentInternal: SessionID={0}, DeviceType={1}, Amount={2}, " +
                    "To={3}", internalRequest.SessionID, internalRequest.DeviceType, internalRequest.Amount, internalRequest.Agent), Logger.LoggingLevelType.Low);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados D2.AccountPaymentInternal: SessionID={0}, DeviceType={1}, Amount={2}, " +
                    "To={3}, Type={4}", internalRequest.SessionID, internalRequest.DeviceType, internalRequest.Amount, internalRequest.Agent, "1"), Logger.LoggingLevelType.Low);
                    utibaAccountPaymentResponse = utibaClient.accountPayment(new accountPayment()
                    {
                        accountPaymentRequest = new accountPaymentRequestType()
                        {
                            sessionid = internalRequest.SessionID,
                            device_type = internalRequest.DeviceType,
                            amount = internalRequest.Amount,
                            to = internalRequest.Agent
                        }
                    });
                    internalResponse = new AccountPaymentResponseInternal()
                    {
                        ResponseCode = utibaAccountPaymentResponse.accountPaymentReturn.result,
                        ResponseMessage = utibaAccountPaymentResponse.accountPaymentReturn.result_message,
                        TransactionID = utibaAccountPaymentResponse.accountPaymentReturn.transid
                    };
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido D2.AccountPaymentInternal: ResponseCode={0}, ResponseMessage={1}, TransactionID={2}"
                    , internalResponse.ResponseCode, internalResponse.ResponseMessage, internalResponse.TransactionID), Logger.LoggingLevelType.Low);

                    if (internalResponse.ResponseCode != 0)
                        internalResponse.SetResponseNamespace(ApiResponseInternal.ResponseNamespace.BAC);
                }
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo D2.AccountPaymentInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                if (internalResponse == null)
                    internalResponse = new AccountPaymentResponseInternal();
                internalResponse.SetThrowedException(ex);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.AccountPaymentInternal", Logger.LoggingLevelType.Medium);
            return (internalResponse);
        }

        private CashInResponseInternal CashInInternal(CashInRequestInternal internalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.CashInInternal", Logger.LoggingLevelType.Medium);
            CashInResponseInternal internalResponse = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                cashinResponse utibaCashInResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos D2.CashInInternal: SessionID={0}, DeviceType={1}, Amount={2}, " +
                    "Recipient={3}", internalRequest.SessionID, internalRequest.DeviceType, internalRequest.Amount, internalRequest.Recipient), Logger.LoggingLevelType.Low);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados D2.CashInInternal: SessionID={0}, DeviceType={1}, Amount={2}, " +
                    "Recipient={3}", internalRequest.SessionID, internalRequest.DeviceType, internalRequest.Amount, internalRequest.Recipient), Logger.LoggingLevelType.Low);
                    utibaCashInResponse = utibaClient.cashin(new cashin()
                    {
                        cashinRequest = new cashinRequestType()
                        {
                            sessionid = internalRequest.SessionID,
                            device_type = internalRequest.DeviceType,
                            amount = internalRequest.Amount,
                            to = internalRequest.Recipient,
                            suppress_confirm=true,
                            suppress_confirmSpecified=true
                        }
                    });
                    internalResponse = new CashInResponseInternal()
                    {
                        ResponseCode = utibaCashInResponse.cashinReturn.result,
                        ResponseMessage = utibaCashInResponse.cashinReturn.result_message,
                        TransactionID = utibaCashInResponse.cashinReturn.transid
                    };
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido D2.CashInInternal: ResponseCode={0}, ResponseMessage={1}, TransactionID={2}"
                    , internalResponse.ResponseCode, internalResponse.ResponseMessage, internalResponse.TransactionID), Logger.LoggingLevelType.Low);

                    if (internalResponse.ResponseCode != 0)
                        internalResponse.SetResponseNamespace(ApiResponseInternal.ResponseNamespace.BAC);
                }
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo D2.CashInInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                if (internalResponse == null)
                    internalResponse = new CashInResponseInternal();
                internalResponse.SetThrowedException(ex);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.CashInInternal", Logger.LoggingLevelType.Medium);
            return (internalResponse);
        }

        private GetLastTransactionsResponseInternal GetLastTransactionsInternal(GetLastTransactionsRequestInternal internalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.GetLastTransactionsInternal", Logger.LoggingLevelType.Medium);
            GetLastTransactionsResponseInternal internalResponse = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                lastTransactionsResponse utibaGetLastTransactionsResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos D2.GetLastTransactionsInternal: SessionID={0}, DeviceType={1}, Agent={2}, " +
                    "transCount={3}", internalRequest.SessionID, internalRequest.DeviceType, internalRequest.Agent, internalRequest.Count), Logger.LoggingLevelType.Low);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados D2.GetLastTransactionsInternal: SessionID={0}, DeviceType={1}, Agent={2}, " +
                    "transCount={3}", internalRequest.SessionID, internalRequest.DeviceType, internalRequest.Agent, internalRequest.Count), Logger.LoggingLevelType.Low);
                    utibaGetLastTransactionsResponse = utibaClient.lastTransactions(new lastTransactionsRequest()
                    {
                        lastTransactionsRequestType = new lastTransactionsRequestType()
                        {
                            sessionid = internalRequest.SessionID,
                            device_type = internalRequest.DeviceType,
                            agent=internalRequest.Agent,
                            transCount = internalRequest.Count
                        }
                    });

                    internalResponse = new GetLastTransactionsResponseInternal()
                    {
                        ResponseCode = utibaGetLastTransactionsResponse.lastTransactionsReturn.result,
                        ResponseMessage = utibaGetLastTransactionsResponse.lastTransactionsReturn.result_namespace,
                        TransactionID = utibaGetLastTransactionsResponse.lastTransactionsReturn.transid
                    };
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido D2.GetLastTransactionsInternal: ResponseCode={0}, ResponseMessage={1}, TransactionID={2}"
                    , internalResponse.ResponseCode, internalResponse.ResponseMessage, internalResponse.TransactionID), Logger.LoggingLevelType.Low);

                    if (utibaGetLastTransactionsResponse.lastTransactionsReturn.transactionsList != null &&
                        utibaGetLastTransactionsResponse.lastTransactionsReturn.transactionsList.Length > 0)
                    {
                        internalResponse.Transactions = new List<TransactionSummaryInternal>();
                        foreach (Utiba.TransactionSummary transaction in utibaGetLastTransactionsResponse.lastTransactionsReturn.transactionsList)
                        {
                            Internal.TransactionSummaryInternal currentTransactionSummary = new Internal.TransactionSummaryInternal()
                            {
                                Amount = transaction.amount,
                                LastTimeModified = transaction.lastModified,
                                PartiesReferenceIDList = transaction.partiesReferenceIdList,
                                TransactionID = transaction.transactionId,
                                TransactionType = transaction.transactionType
                            };
                            internalResponse.Transactions.Add(currentTransactionSummary);
                        }
                    }

                    if (internalResponse.ResponseCode != 0)
                        internalResponse.SetResponseNamespace(ApiResponseInternal.ResponseNamespace.BAC);
                }
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo D2.GetLastTransactions, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                if (internalResponse == null)
                    internalResponse = new GetLastTransactionsResponseInternal();
                internalResponse.SetThrowedException(ex);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.GetLastTransactions", Logger.LoggingLevelType.Medium);
            return (internalResponse);
        }

        private SummaryResponseInternal SummaryInternal(SummaryRequestInternal internalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.SummaryInternal", Logger.LoggingLevelType.Medium);
            SummaryResponseInternal internalResponse = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                salesResponse utibaSalesResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos D2.SummaryInternal: SessionID={0}, DeviceType={1}, Target={2}, " +
                    "WalletType={3}, Start_offset={4}, End_offset={5}", internalRequest.SessionID, internalRequest.DeviceType, internalRequest.Target, internalRequest.WalletType, internalRequest.StartDate, internalRequest.EndDate), Logger.LoggingLevelType.Low);
                    int startEpochTime = FromDateTimeToEpoch(internalRequest.StartDate);
                    int endEpochTime = FromDateTimeToEpoch(internalRequest.EndDate);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados D2.SummaryInternal: SessionID={0}, DeviceType={1}, Target={2}, " +
                    "WalletType={3}, Start_offset={4}, End_offset={5}", internalRequest.SessionID, internalRequest.DeviceType, internalRequest.Target, internalRequest.WalletType, internalRequest.StartDate, internalRequest.EndDate), Logger.LoggingLevelType.Low);
                    utibaSalesResponse = utibaClient.sales(new sales()
                    {
                        salesRequest = new salesRequestType()
                        {
                            sessionid = internalRequest.SessionID,
                            device_type = internalRequest.DeviceType,
                            start=(startEpochTime*1000L),     //long.Parse(startEpochTime.ToString() + "000"),
                            startSpecified=true,
                            end = (endEpochTime * 1000L), //long.Parse(endEpochTime.ToString() + "000"),
                            endSpecified=true,
                            type=internalRequest.WalletType,
                            target=internalRequest.Target
                        }
                    });
                    internalResponse = new SummaryResponseInternal()
                    {
                        ResponseCode = utibaSalesResponse.salesReturn.result,
                        ResponseMessage = utibaSalesResponse.salesReturn.result_message,
                        TransactionID = utibaSalesResponse.salesReturn.transid,

                        TransactionCount=utibaSalesResponse.salesReturn.count,
                        TotalAmount=utibaSalesResponse.salesReturn.sum
                    };
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido D2.SummaryInternal: ResponseCode={0}, ResponseMessage={1}, TransactionID={2}" +
                    ", TransactionCount={3}, TotalAmount={4}", internalResponse.ResponseCode, internalResponse.ResponseMessage, internalResponse.TransactionID, internalResponse.TransactionCount, internalResponse.TotalAmount), Logger.LoggingLevelType.Low);
                    if (internalResponse.ResponseCode != 0)
                        internalResponse.SetResponseNamespace(ApiResponseInternal.ResponseNamespace.BAC);
                }
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo D2.SummaryInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                if (internalResponse == null)
                    internalResponse = new SummaryResponseInternal();
                internalResponse.SetThrowedException(ex);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.BankListResponseInternal", Logger.LoggingLevelType.Medium);
            return (internalResponse);
        }

        private BankListResponseInternal BankListInternal(BankListRequestInternal internalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método D2.BankListResponseInternal", Logger.LoggingLevelType.Medium);
            BankListResponseInternal internalResponse = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                getBankListResponse utibaGetBankListResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    //Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos D2.SummaryInternal: SessionID={0}, DeviceType={1}, Target={2}, " +
                    //"WalletType={3}, Start_offset={4}, End_offset={5}", internalRequest.SessionID, internalRequest.DeviceType, internalRequest.Target, internalRequest.WalletType, internalRequest.StartDate, internalRequest.EndDate), Logger.LoggingLevelType.Low);
                    //int startEpochTime = FromDateTimeToEpoch(internalRequest.StartDate);
                    //int endEpochTime = FromDateTimeToEpoch(internalRequest.EndDate);
                    //Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados D2.SummaryInternal: SessionID={0}, DeviceType={1}, Target={2}, " +
                    //"WalletType={3}, Start_offset={4}, End_offset={5}", internalRequest.SessionID, internalRequest.DeviceType, internalRequest.Target, internalRequest.WalletType, internalRequest.StartDate, internalRequest.EndDate), Logger.LoggingLevelType.Low);
                    utibaGetBankListResponse = utibaClient.getBankList(new getBankList()
                    {
                        getBankListRequest = new getBankListRequestType()
                        {
                            sessionid = internalRequest.SessionID,
                            device_type = internalRequest.DeviceType,
                            agent_reference=internalRequest.AgentReference
                        }
                    });
                    internalResponse = new BankListResponseInternal()
                    {
                        ResponseCode = utibaGetBankListResponse.getBankListReturn.result,
                        ResponseMessage = utibaGetBankListResponse.getBankListReturn.result_message,
                        TransactionID = utibaGetBankListResponse.getBankListReturn.transid
                    };
                    if (utibaGetBankListResponse.getBankListReturn.banks.Length > 0)
                    {
                        internalResponse.Banks = new List<Internal.BankInfo>();
                        foreach (KeyValuePair1 kvp in utibaGetBankListResponse.getBankListReturn.banks)
                        {
                            internalResponse.Banks.Add(new Internal.BankInfo()
                            {
                                Key = int.Parse(kvp.key),
                                Description = kvp.value
                            });
                        }
                    }
                    if (internalResponse.ResponseCode != 0)
                        internalResponse.SetResponseNamespace(ApiResponseInternal.ResponseNamespace.BAC);
                }
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo D2.SummaryInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                if (internalResponse == null)
                    internalResponse = new BankListResponseInternal();
                internalResponse.SetThrowedException(ex);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método D2.BankListResponseInternal", Logger.LoggingLevelType.Medium);
            return (internalResponse);
        }

        
        public GetTransactionsInRangeResponse GetTransactionsInRange(GetTransactionsInRangeRequest externalRequest)
        {
            return (null);
        }

        #endregion
    }
}
