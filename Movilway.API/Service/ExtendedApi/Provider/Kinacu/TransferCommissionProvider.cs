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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.TransferCommission)]
    public class TransferCommissionProvider : AKinacuProvider
    {




        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(TransferProvider));
        protected override ILogger ProviderLogger { get { return logger; } }
        protected override TransactionType TransactionType { get { return TransactionType.transfer; } }



        public TransferCommissionProvider()
        {
            _delegateSecondExecution += SecondKinacuOperation;
        }

        protected IMovilwayApiResponse SecondKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            return new TransferCommissionResponseBody()
            {
                ResponseCode = 95,
                ResponseMessage = "ESTA INTENTANDO DUPLICAR UNA MISMA TRANSFERENCIA",
                TransactionID = 0,
                StockBalance = 0m
            };

        }


        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {

            TransferCommissionRequestBody request = requestObject as TransferCommissionRequestBody;
            TransferCommissionResponseBody response = null;

            string rootagency = ConfigurationManager.AppSettings["rootagency"] ?? "4";

            int sourceUserId = 0, recipientAgentId = 0;
            string accessReceiver = "";
            bool requestHasPDV = !String.IsNullOrEmpty(request.RecipientPdv);


            //LA AGENCIA RAIZ PUEDE TRANSFERIR UNA COMISION AL PDV
            //LA AGENCIA RAIZ BUSCA EL PADRE Y HACE QUE EL PADRE TRANSFIERA LA COMISION

            if (sessionID.Equals("0"))
            {
                response = new TransferCommissionResponseBody()
                {
                    ResponseCode = requestHasPDV ? 3 : 90,
                    ResponseMessage = requestHasPDV ? "ID Inactivo" : "error session",
                    TransactionID = 0,
                    StockBalance = 0m
                };
                
                    response.ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                return response;
            }

            try
            {
                sourceUserId = new IBank.Utils().GetUserId(request.AuthenticationData.Username);

            }
            catch (Exception)
            {

                response = new TransferCommissionResponseBody()
                {
                    ResponseCode = 99,
                    ResponseMessage = "ERROR OBTENIENDO USUARIO ID",
                    TransactionID = 0,
                    StockBalance = 0m
                };

                return response;

            }


            int recipientId = 0;
            bool recipientIsNumeric = int.TryParse(request.Recipient, out recipientId);


            if (!recipientIsNumeric && String.IsNullOrEmpty(request.RecipientPdv))
            {
                response = new TransferCommissionResponseBody()
                {
                    ResponseCode = 99,
                    ResponseMessage = "RecipientPdv NO PUEDE SER VACIO",
                    TransactionID = 0,
                    StockBalance = 0m
                };

                return response;
            }


            if (recipientIsNumeric)
                recipientAgentId = recipientId;

            if (!recipientIsNumeric)
            {

                try
                {
                    recipientAgentId = new IBank.Utils().GetAgentIdByPdv(request.RecipientPdv);
                    accessReceiver = request.Recipient;
                }
                catch (Exception ex)
                {
                    //if (requestHasPDV)
                    //    throw e;
                    //else
                        return new TransferCommissionResponseBody()
                        {
                            ResponseCode = 3,
                            ResponseMessage = "PDV Inactivo",
                            TransactionID = 0,
                            StockBalance = 0m,
                            ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                        };
                }

            }

            if (recipientAgentId != 0)
                request.Recipient = recipientAgentId.ToString();



            int sourceAgentId = new IBank.Utils().GetAgentId(request.AuthenticationData.Username);

            bool isChild = false;


            IBank.Utils.TransferInfo info = new IBank.Utils.TransferInfo();

            info.OverrideRequestCode = request.Code > 0? request.Code: 501;
            info.OverrideReverseCode = 205;

            logger.InfoHigh(LOG_PREFIX + " get agent data ");
            //Ariel 2021-Ma-09 Comentado 
           // var agentinfo = Utils.GetAgentData(base.LOG_PREFIX, recipientAgentId);
            logger.InfoHigh(LOG_PREFIX + " agent data ok");


            switch (request.TransferRequestType)
            {
                case TransferRequestType.FromParentToBranch:



                    if (sourceAgentId.ToString() == rootagency)
                    {
                        isChild = true;

                        // seleccionar agencia padre y usuario


                        //Ariel 2021-Ma-09 Comentado asignamos 0
                        info.AgentParent = 0; // Convert.ToInt32(agentinfo["age_id_sup"]);
                        info.UserTransacctionId = sourceUserId;

                        // info.AgentParent = (int) agentinfo.AgentID;


                    }
                    else
                    {
                        // validar si la agencia es padre

                        //Ariel 2021-Ma-09 Comentado asignamos 0
                        info.AgentParent = 0;// Convert.ToInt32(agentinfo["age_id_sup"]);
                        info.UserTransacctionId = sourceUserId;

                        if (info.AgentParent != sourceAgentId)
                        {
                            response = new TransferCommissionResponseBody()
                            {
                                Fee = 0,
                                ResponseCode = 99,
                                ResponseMessage = "No puede hacer asginacion de comision a una agencia que no es hija directa.",
                                //ResponseCode = 90,
                                //ResponseMessage = "Distribución de Saldo Fallida, agencia destino igual a agencia origen",
                                TransactionID = 0
                            };
                            return response;
                        }

                    }

                    break;


            }




            if (sourceAgentId == recipientAgentId)
            {
                response = new TransferCommissionResponseBody()
                {
                    Fee = 0,
                    // ResponseCode = requestHasPDV ? 3 : 90,
                    // ResponseMessage = requestHasPDV ? "ID Inactivo" : "Distribución de Saldo Fallida, agencia destino igual a agencia origen",
                    ResponseCode = 90,
                    ResponseMessage = "Comision Fallida, agencia destino igual a agencia origen",


                    //ResponseCode = 90,
                    //ResponseMessage = "Distribución de Saldo Fallida, agencia destino igual a agencia origen",
                    TransactionID = 0
                };
               // if (requestHasPDV)
                    response.ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                return response;
            }



            bool isNegativeAmount = false;
            bool acceptReverse = false;


            if (request.Amount < 0)
            {
                isNegativeAmount = true;
                acceptReverse = new IBank.Utils().CheckReverse(request.Recipient);
            }



            if ((isChild && !isNegativeAmount && request.DBTransferIfChild) || (!isChild && !isNegativeAmount) || (isChild && isNegativeAmount && acceptReverse))
            {
                // Acá resultó NO ser hijo(o hijo pero no se quiere afectar cta corriente), así que procedo con la distribución vía SP



                string autorization = "", messageOut = "", responseCode = "99";
                DateTime myNow = Utils.GetLocalTimeZone();

                logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[TransferProvider] [SEND-DATA] transferb2bCommissionParameters {info="+ info.ToString()+ "} {agent_reciver=" + request.Recipient +
                                            ",cuenta=" + ConfigurationManager.AppSettings["TransferAccount"] + ",usr_id=" + sourceUserId + ",amount=" + double.Parse(request.Amount.ToString()) +
                                            ",reference_number=" + request.ExternalTransactionReference); // + ",date=" + myNow + "}");

                //Se valida que el maximo a transferir si es hijo no exceda el límite de crédito (Condición Proesa)
                if (isChild && !isNegativeAmount && request.DBTransferIfChild)
                {
                    //Ariel 2021-Ma-09 Comentado  devolvemos 0
                    decimal maxCredit = 0;//Utils.GetAgentMaxCreditAvailable(request.Recipient);
                    if (request.Amount > maxCredit)
                    {
                        logger.ErrorHigh(base.LOG_PREFIX + "[TransferProvider] [SEND-DATA] transferb2bCommission El monto Excede el máximo de crédito disponible Amount:" + request.Amount + " Limite Credito: " + maxCredit);
                        return new TransferCommissionResponseBody()
                        {
                            ResponseCode = 99,
                            ResponseMessage = "EL MONTO EXCEDE EL LIMITE DE CREDITO CONFIGURADO",
                            TransactionID = 0,
                            StockBalance = 0m
                        };

                    }


                }

              
                //decimal newAmount = 0m;
                var distributionResponse = new IBank.Utils().transferb2bCommission(request.Recipient, accessReceiver, ConfigurationManager.AppSettings["TransferAccount"], sourceUserId, double.Parse(request.Amount.ToString()), request.ExternalTransactionReference, myNow, (isChild && isNegativeAmount && acceptReverse), ref autorization, ref messageOut, ref responseCode, info); //, ref newAmount);

                logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[transferb2bCommission] [RECV-DATA] createCommissionResult {response=" + distributionResponse +
                                            ",autorization=" + autorization + ",message=" + messageOut + ",response_code=" + responseCode + "}");

                if (int.Parse(responseCode ?? "99") == 0)
                {
                    response = new TransferCommissionResponseBody()
                    {
                        Fee = 0,
                        ResponseCode = 0,
                        //ResponseMessage = requestHasPDV ? "activado" : messageOut,
                        ResponseMessage =  messageOut,
                        TransactionID = int.Parse(autorization)
                    };
                   // if (requestHasPDV)
                        response.ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    response = new TransferCommissionResponseBody()
                    {
                        Fee = 0,
                        ResponseCode = requestHasPDV ? 3 : int.Parse(responseCode ?? "99"),
                        ResponseMessage = requestHasPDV ? "ID Inactivo" : messageOut,
                        TransactionID = String.IsNullOrEmpty(autorization) ? 99 : int.Parse(autorization)
                    };
                   // if (requestHasPDV)
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
                            response = new TransferCommissionResponseBody()
                            {
                                Fee = 0,
                                ResponseCode = 0,
                                ResponseMessage = requestHasPDV ? "activado" : messageOut,
                                TransactionID = int.Parse(autorization)
                            };
                            //if (requestHasPDV)
                                response.ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            response = new TransferCommissionResponseBody()
                            {
                                Fee = 0,
                                ResponseCode = requestHasPDV ? 3 : int.Parse(responseCode ?? "99"),
                                ResponseMessage = requestHasPDV ? "ID Inactivo" : messageOut,
                                TransactionID = String.IsNullOrEmpty(autorization) ? 99 : int.Parse(autorization)
                            };
                            //if (requestHasPDV)
                                response.ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                            //if (!newAmount.Equals(request.Amount))
                            //    response.ChargedAmount = newAmount;
                        }
                        // End 
                    }
                    else
                    {
                        response = new TransferCommissionResponseBody()
                        {
                            Fee = 0,
                            ResponseCode = requestHasPDV ? 3 : 99,
                            ResponseMessage = requestHasPDV ? "ID Inactivo" : "El monto a debitar es superior a la deuda actual",
                            TransactionID = 99
                        };
                       // if (requestHasPDV)
                            response.ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
                else
                {
                    response = new TransferCommissionResponseBody()
                    {
                        Fee = 0,
                        ResponseCode = requestHasPDV ? 3 : 99,
                        ResponseMessage = requestHasPDV ? "ID Inactivo" : "Debito de stock no permitido",
                        TransactionID = 99
                    };
                    //if (requestHasPDV)
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
                response = new TransferCommissionResponseBody()
                {
                    Fee = 0,
                    ResponseCode = requestHasPDV ? 3 : 99,
                    ResponseMessage = requestHasPDV ? "ID Inactivo" : (isChild && isNegativeAmount && !acceptReverse ? "La agencia no acepta quitas automáticas" : "Error no identificado"),
                    TransactionID = 99
                };
                //if (requestHasPDV)
                    response.ResponseDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            }


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