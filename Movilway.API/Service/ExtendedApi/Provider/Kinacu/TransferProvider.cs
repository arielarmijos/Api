using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Data;
using Movilway.Logging;
using Movilway.API.KinacuWebService;
using Movilway.API.KinacuLogisticsWebService;
using System.Configuration;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.Transfer)]
    public class TransferProvider : AKinacuProvider
    {


        private static bool _secondRequestBandera = Boolean.TryParse(ConfigurationManager.AppSettings["SECOND_REQUEST_TransferProvider"], out _secondRequestBandera);
       

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(TransferProvider));
        protected override ILogger ProviderLogger { get { return logger; } }
        protected override TransactionType TransactionType { get { return TransactionType.transfer;} }



        public TransferProvider()
        {
            _delegateSecondExecution += SecondKinacuOperation;
        }

        protected IMovilwayApiResponse SecondKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            return new TransferResponseBody()
            {
                ResponseCode = 95,
                ResponseMessage = "ESTA INTENTANDO DUPLICAR UNA MISMA TRANSFERENCIA",
                TransactionID = 0,
                StockBalance = 0m
            };

        }

        protected override bool ValidateNumberOfExecution(IMovilwayApiRequest request)
        {
            return _secondRequestBandera;//request.DeviceType == cons.ACCESS_H2H || request.DeviceType == cons.ACCESS_POSWEB;
        }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {

            TransferRequestBody request = requestObject as TransferRequestBody;
            TransferResponseBody response = null;

            int sourceUserId = 0, recipientAgentId = 0;
            string accessReceiver = "";
            bool requestHasPDV = !String.IsNullOrEmpty(request.RecipientPdv);

            if (sessionID.Equals("0"))
            {
                response = new TransferResponseBody()
                {
                    ResponseCode = requestHasPDV ? 3 : 90,
                    ResponseMessage = requestHasPDV ? "ID Inactivo" : "error session",
                    TransactionID = 0,
                    StockBalance = 0m
                };
                if (requestHasPDV)
                    response.ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                return response;
            }

            try { sourceUserId = new IBank.Utils().GetUserId(request.AuthenticationData.Username); }
            catch (Exception) { }
            int recipientId = 0;
            bool recipientIsNumeric = int.TryParse(request.Recipient, out recipientId);

            if (!String.IsNullOrEmpty(request.RecipientPdv))
            {
                try
                {
                    recipientAgentId = new IBank.Utils().GetAgentIdByPdv(request.RecipientPdv);
                    accessReceiver = request.Recipient;
                }
                catch (Exception e)
                {
                    if (requestHasPDV)
                        throw e;
                    else
                        return new TransferResponseBody()
                        {
                            ResponseCode = 3,
                            ResponseMessage = "ID Inactivo",
                            TransactionID = 0,
                            StockBalance = 0m,
                            ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                        };
                }
            }
            else if (String.IsNullOrEmpty(request.RecipientAccessId))
            {
                try
                {
                    recipientAgentId = new IBank.Utils().GetAgentId(request.Recipient);
                    accessReceiver = request.Recipient;
                }
                catch (Exception) { }
            }
            else 
            {
                try
                {
                    //TODO PROBLEMA NAMESAPCE Movilway.API.Service.ExtendedApi.Provider.IBank.Utils().GetAgentId(request.RecipientAccessId);
                    recipientAgentId = new IBank.Utils().GetAgentId(request.RecipientAccessId);
                    accessReceiver = request.RecipientAccessId;
                }
                catch (Exception)
                {
                    if (requestHasPDV)
                        return new TransferResponseBody()
                        {
                            ResponseCode = 3,
                            ResponseMessage = "ID Inactivo",
                            TransactionID = 0,
                            StockBalance = 0m,
                            ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                        };
                    /*request.Recipient = request.RecipientAccessId;*/
                }
            }

            if (recipientAgentId != 0)
                request.Recipient = recipientAgentId.ToString();

            bool isChild = false;
            bool isNegativeAmount = false;
            bool acceptReverse = false;

            int sourceAgentId = new IBank.Utils().GetAgentId(request.AuthenticationData.Username);

            if (sourceAgentId == recipientAgentId)
            {
                response = new TransferResponseBody()
                {
                    Fee = 0,
                    ResponseCode = requestHasPDV ? 3 : 90,
                    ResponseMessage = requestHasPDV ? "ID Inactivo" : "Distribución de Saldo Fallida, agencia destino igual a agencia origen",
                    //ResponseCode = 90,
                    //ResponseMessage = "Distribución de Saldo Fallida, agencia destino igual a agencia origen",
                    TransactionID = 0
                };
                if (requestHasPDV)
                    response.ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                return response;
            }

            if (request.Amount < 0)
            {
                isNegativeAmount = true;
                acceptReverse = new IBank.Utils().CheckReverse(request.Recipient);
            }
            var getChildListResponse = new ServiceExecutionDelegator
                                            <GetChildListResponseBody, GetChildListRequestBody>().ResolveRequest(
                                                new GetChildListRequestBody()
                                                {
                                                    AuthenticationData = new AuthenticationData() {
                                                        Username = request.AuthenticationData.Username,
                                                        Password = request.AuthenticationData.Password
                                                    },
                                                    DeviceType = request.DeviceType,
                                                    Agent = request.AuthenticationData.Username,
                                                    Platform = "1"
                                                }, ApiTargetPlatform.Kinacu, ApiServiceName.GetChildList);

            if (getChildListResponse.ChildList != null && getChildListResponse.ChildList.Count(ch => ch.Agent == request.Recipient) > 0)
                isChild = true;

            if (isChild && !isNegativeAmount && !request.DBTransferIfChild)
            {
                // Acá resultó ser hijo y debe afectarse la cuenta corriente, así que procedo con la distribución a hijo vía WS

                LogisticsInterface logisticsWS = new LogisticsInterface();
                string message = "";

                logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[TransferProvider] [SEND-DATA] createProductDistributionParameters {UserId=" + sessionID +
                                            ",RetailerIdTo=" + decimal.Parse(request.Recipient) + ",IdProduct=0,Amount=" + int.Parse((request.Amount * 100).ToString("#")) + "}");

                var result = logisticsWS.CreateProductDistribution(int.Parse(sessionID), decimal.Parse(request.Recipient), 0, int.Parse((request.Amount * 100).ToString("#")), out message);

                var transactionId = new IBank.Utils().GetDistributionId(DateTime.Now, Convert.ToDecimal(sourceAgentId), decimal.Parse(request.Recipient), request.Amount);

                logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[TransferProvider] [RECV-DATA] createProductDistributionResult {response=" + result + ",message=" + message + "}");

                if (result)
                {
                    response = new TransferResponseBody()
                    {
                        Fee = 0,
                        ResponseCode = 0,
                        ResponseMessage = requestHasPDV ? "activado" : String.Format("Distribución de Saldo Exitosa\nFECHA: {0}\nVENDEDOR: {1}\nID PDV: {2}\nMONTO: {3}", DateTime.Now.ToString("dd/MM/yyyy"), request.AuthenticationData.Username, getChildListResponse.ChildList.Single(ch => ch.Agent == request.Recipient).Name, request.Amount.ToString("N2")), //message,
                        TransactionID = transactionId
                    };
                    if (requestHasPDV)
                        response.ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    response = new TransferResponseBody()
                    {
                        Fee = 0,
                        ResponseCode = requestHasPDV ? 3 : Utils.BuildResponseCode(result, message),
                        ResponseMessage = requestHasPDV ? "ID Inactivo" : (String.IsNullOrEmpty(message) ? "La distribución es fallida, por favor valide que ingreso el ID correcto del PDV Hijo o que tiene suficiente saldo." : message),
                        //ResponseCode = Utils.BuildResponseCode(result,message),
                        //ResponseMessage = String.IsNullOrEmpty(message) ? "La distribución es fallida, por favor valide que ingreso el ID correcto del PDV Hijo o que tiene suficiente saldo." : message, //message,
                        TransactionID = transactionId
                    };
                    if (requestHasPDV)
                        response.ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                }

            }
            else if ((isChild && !isNegativeAmount && request.DBTransferIfChild) || (!isChild && !isNegativeAmount) || (isChild && isNegativeAmount && acceptReverse))
            {
                // Acá resultó NO ser hijo(o hijo pero no se quiere afectar cta corriente), así que procedo con la distribución vía SP

                

                string autorization = "", messageOut = "", responseCode = "99";
                DateTime myNow = Utils.GetLocalTimeZone();

                logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[TransferProvider] [SEND-DATA] transferb2bParameters {agent_reciver=" + request.Recipient +
                                            ",cuenta=" + ConfigurationManager.AppSettings["TransferAccount"] + ",usr_id=" + sourceUserId + ",amount=" + double.Parse(request.Amount.ToString()) +
                                            ",reference_number=" + request.ExternalTransactionReference); // + ",date=" + myNow + "}");
                
                //Se valida que el maximo a transferir si es hijo no exceda el límite de crédito (Condición Proesa)
                if (isChild && !isNegativeAmount && request.DBTransferIfChild)
                {
                    //Ariel 2021-Ma-09 Comentado ponemos cero
                    decimal maxCredit = 0; //Utils.GetAgentMaxCreditAvailable(request.Recipient);
                    if (request.Amount > maxCredit) {
                        logger.ErrorHigh(base.LOG_PREFIX + "[TransferProvider] [SEND-DATA] transferb2bParameters El monto Excede el máximo de crédito disponible Amount:" + request.Amount + " Limite Credito: " + maxCredit);
                        return new TransferResponseBody()
                        {
                            ResponseCode = 99,
                            ResponseMessage = "EL MONTO EXCEDE EL LIMITE DE CREDITO CONFIGURADO",
                            TransactionID = 0,
                            StockBalance = 0m
                        };

                    }
                    
                    
                }


                //decimal newAmount = 0m;
                var distributionResponse = new IBank.Utils().transferb2b(request.Recipient, accessReceiver, ConfigurationManager.AppSettings["TransferAccount"], sourceUserId, double.Parse(request.Amount.ToString()), request.ExternalTransactionReference, myNow, (isChild && isNegativeAmount && acceptReverse), ref autorization, ref messageOut, ref responseCode); //, ref newAmount);

                logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[TransferProvider] [RECV-DATA] createProductDistributionResult {response=" + distributionResponse +
                                            ",autorization=" + autorization + ",message=" + messageOut + ",response_code=" + responseCode + "}");

                if (int.Parse(responseCode ?? "99") == 0)
                {
                    response = new TransferResponseBody()
                    {
                        Fee = 0,
                        ResponseCode = 0,
                        ResponseMessage = requestHasPDV ? "activado" : messageOut,
                        TransactionID = int.Parse(autorization)
                    };
                    if (requestHasPDV)
                        response.ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    response = new TransferResponseBody()
                    {
                        Fee = 0,
                        ResponseCode = requestHasPDV ? 3 : int.Parse(responseCode ?? "99"),
                        ResponseMessage = requestHasPDV ? "ID Inactivo" : messageOut,
                        TransactionID = String.IsNullOrEmpty(autorization) ? 99 : int.Parse(autorization)
                    };
                    if (requestHasPDV)
                        response.ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                }

            }
            else if (!isChild && isNegativeAmount)
            {
                // TODO NEW CONDITION TO MO - Done

                int[] branchsAllowed = Array.ConvertAll<string, int>(ConfigurationManager.AppSettings["ReversesBranchesAllowed"].Split(','), int.Parse);

                if (branchsAllowed.Contains(sourceAgentId))
                {
                    // TODO Check the debt_amount
                    var debtAmount = Utils.GetTotalDebt(request.Recipient);
                    logger.InfoLow("[MO] " + base.LOG_PREFIX + "[MOApiProvider] [RECV-DATA] totalDebtAmount {response=" + debtAmount + "}");

                    if (Math.Abs(request.Amount) <= debtAmount)
                    {
                        // Mismo código que arriba
                        string autorization = "", messageOut = "", responseCode = "99";
                        DateTime myNow = Utils.GetLocalTimeZone();

                        logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[TransferProvider] [SEND-DATA] transferb2bParameters {agent_reciver=" + request.Recipient +
                                                    ",cuenta=" + ConfigurationManager.AppSettings["TransferAccount"] + ",usr_id=" + sourceUserId + ",amount=" + double.Parse(request.Amount.ToString()) +
                                                    ",reference_number=" + request.ExternalTransactionReference + ",date=" + myNow + "}");

                        //decimal newAmount = 0m;
                        var distributionResponse = new IBank.Utils().transferb2b(request.Recipient, accessReceiver, ConfigurationManager.AppSettings["TransferAccount"], sourceUserId, double.Parse(request.Amount.ToString()), request.ExternalTransactionReference, myNow, (isChild && isNegativeAmount && acceptReverse), ref autorization, ref messageOut, ref responseCode); //, ref newAmount, partialCharge: true);

                        logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[TransferProvider] [RECV-DATA] createProductDistributionResult {response=" + distributionResponse +
                                                    ",autorization=" + autorization + ",message=" + messageOut + ",response_code=" + responseCode + "}");

                        if (int.Parse(responseCode ?? "99") == 0)
                        {
                            response = new TransferResponseBody()
                            {
                                Fee = 0,
                                ResponseCode = 0,
                                ResponseMessage = requestHasPDV ? "activado" : messageOut,
                                TransactionID = int.Parse(autorization)
                            };
                            if (requestHasPDV)
                                response.ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            response = new TransferResponseBody()
                            {
                                Fee = 0,
                                ResponseCode = requestHasPDV ? 3 : int.Parse(responseCode ?? "99"),
                                ResponseMessage = requestHasPDV ? "ID Inactivo" : messageOut,
                                TransactionID = String.IsNullOrEmpty(autorization) ? 99 : int.Parse(autorization)
                            };
                            if (requestHasPDV)
                                response.ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                            //if (!newAmount.Equals(request.Amount))
                            //    response.ChargedAmount = newAmount;
                        }
                        // End 
                    }
                    else
                    {
                        response = new TransferResponseBody()
                        {
                            Fee = 0,
                            ResponseCode = requestHasPDV ? 3 : 99,
                            ResponseMessage = requestHasPDV ? "ID Inactivo" : "El monto a debitar es superior a la deuda actual",
                            TransactionID = 99
                        };
                        if (requestHasPDV)
                            response.ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
                else
                {
                    response = new TransferResponseBody()
                    {
                        Fee = 0,
                        ResponseCode = requestHasPDV ? 3 : 99,
                        ResponseMessage = requestHasPDV ? "ID Inactivo" : "Debito de stock no permitido",
                        TransactionID = 99
                    };
                    if (requestHasPDV)
                        response.ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                }
                // Validate if the branch is MO (allowed from .config)
                // if true
                //      query debt amount
                //      if amount <= debt amount 
                //          transfer code (up)
                //      else 
                //          return error amount too high
                // else
                //      return error
            }
            else
            {
                response = new TransferResponseBody()
                {
                    Fee = 0,
                    ResponseCode = requestHasPDV ? 3 : 99,
                    ResponseMessage = requestHasPDV ? "ID Inactivo" : (isChild && isNegativeAmount && !acceptReverse ? "La agencia no acepta quitas automáticas" : "Error no identificado"),
                    TransactionID = 99
                };
                if (requestHasPDV)
                    response.ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            }
            try
            { 
                GetBalanceResponseBody balanceResponse = new ServiceExecutionDelegator<GetBalanceResponseBody, GetBalanceRequestBody>().ResolveRequest(new GetBalanceRequestBody()
                                                                {
                                                                    AuthenticationData = new AuthenticationData()
                                                                    {
                                                                        Username = request.AuthenticationData.Username,
                                                                        Password = request.AuthenticationData.Password
                                                                    },
                                                                    DeviceType = request.DeviceType
                                                                }, ApiTargetPlatform.Kinacu, ApiServiceName.GetBalance);
       
                if (response != null)
                {
                    response.StockBalance = balanceResponse.StockBalance.Value;
                }
            }
            catch (Exception) { }


            // Envío de SMS VE si lo tiene habilitado
            try
            {
                if (response.ResponseCode.Equals(0))
                {
                    var sendTransferSMS = ConfigurationManager.AppSettings["SendTransferSMS"];
                    if (!String.IsNullOrEmpty(sendTransferSMS) ? bool.Parse(sendTransferSMS) : false)
                    {
                        var mobilePhone = new IBank.Utils().GetAgentMobilePhone(int.Parse(request.Recipient));
                        var stockFinal = new IBank.Utils().GetAgentFinalStock(int.Parse(request.Recipient));
                        logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[TransferProvider] [SEND-SMS] phone " + mobilePhone);
                        if (mobilePhone.Length.Equals(int.Parse(ConfigurationManager.AppSettings["TelemoPhoneLength"])))
                        {
                            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[TransferProvider] [SEND-SMS] smsToCustomer begin");
                            Util.SMSTelemoDispatcher.SmsToCustomer(ConfigurationManager.AppSettings["TelemoClientId"],
                                ConfigurationManager.AppSettings["TelemoCustomerId"],
                                mobilePhone,
                                String.Format(ConfigurationManager.AppSettings["TelemoMessageTemplate"], request.Amount.ToString("C"), stockFinal, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.UtcNow.AddHours(new IBank.Utils().GetTimeZone()).ToString("HH:mm:ss")));
                            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[TransferProvider] [SEND-SMS] smsToCustomer end");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.ErrorHigh("[SMS] " + base.LOG_PREFIX + "[TransferProvider] [SEND-SMS] " + ex.Message + ", " + ex.StackTrace);
            }

            return (response);
        }
    }
}