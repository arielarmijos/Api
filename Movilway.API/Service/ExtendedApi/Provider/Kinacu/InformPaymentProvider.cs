using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using Movilway.API.KinacuWebService;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.InformPayment)]
    public class InformPaymentProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(InformPaymentProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new InformPaymentResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0
                };
            InformPaymentRequestBody request = requestObject as InformPaymentRequestBody;
            InformPaymentResponseBody response = null;

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[InformPaymentProvider] [SEND-DATA] informPaymentParameters {amount=" + request.Amount + ",targentAgentId=" + request.TargetAgentId + ",hasDeposit=" + request.HasDeposit +
                                            ",accountId=" + request.AccountId + ",transactionReference=" + request.TransactionReference + ",transactionDate=" + request.TransactionDate + ",sucursalNumber=" + request.SucursalNumber +
                                            ",sucursalName=" + request.SucursalName + ",comment=" + request.Comment + ",immediatelyDistribute=" + request.ImmediatelyDistribute + "}");

            string responseCode = "99", message = "error";

            bool result = false;

            int usrId = new IBank.Utils().GetUserId(request.AuthenticationData.Username);

            if (request.TargetAgentId > 0)
            {
                if (!IsValid(request))
                {
                    return new InformPaymentResponseBody()
                    {
                        ResponseCode = 80,
                        ResponseMessage = "Datos incompletos",
                        TransactionID = 0
                    };
                }

                string bankAccount = new IBank.Utils().GetBankNumber(request.AccountId);
                if (request.ImmediatelyDistribute)
                {
                    //result = new Movilway.API.Service.ExtendedApi.Provider.IBank.Utils().RegistroPago(decimal.Parse(request.TargetAgentId.ToString()), request.Amount, request.TransactionReference, request.TransactionDate, bankAccount, (request.HasDeposit ? "S" : "N"), ref responseCode, ref message, request.Comment, decimal.Parse(request.TransactionReference), DateTime.UtcNow.AddHours(new Movilway.API.Service.ExtendedApi.Provider.IBank.Utils().GetTimeZone()));

                    result = new IBank.Utils().RegistroDepositoAcreditaSaldo(decimal.Parse(request.TargetAgentId.ToString()), usrId, request.Amount, request.TransactionReference, request.TransactionDate, bankAccount, ref responseCode, ref message, request.Comment, DateTime.UtcNow.AddHours(new IBank.Utils().GetTimeZone()), request.SucursalNumber.ToString(), request.SucursalName);
                }
                else
                {
                    result = new IBank.Utils().RegistrarDeposito(request.TargetAgentId, usrId, request.Amount, request.TransactionReference, request.TransactionDate, bankAccount, request.Comment, ref responseCode, ref message, DateTime.UtcNow.AddHours(new IBank.Utils().GetTimeZone()), request.SucursalNumber.ToString(), request.SucursalName);
                }
            }
            else
            {

                //int ageId = new IBank.Utils().GetAgentId(request.AuthenticationData.Username);

                int prodcut = -1; String res = "";
                KinacuLogisticsWebService.LogisticsInterface logisticsInterface = new KinacuLogisticsWebService.LogisticsInterface();
                result = logisticsInterface.CreateProductRequest(Convert.ToInt32(sessionID), 0, (int)request.Amount * 100, out prodcut, out res);

                if (result)
                { responseCode = "00"; message = "TRANSACCION OK"; }
                else { responseCode = Movilway.API.Core.UtilResut.StrErrorCode(1); message = res; }
                //result = new IBank.Utils().RegistrarSolicitudProducto(ageId, request.Amount, request.TransactionDate, ref responseCode, ref message);  

            }

            response = new InformPaymentResponseBody()
            {
                ResponseCode = int.Parse(responseCode),
                ResponseMessage = message,
                TransactionID = 0
            };

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[InformPaymentProvider] [RECV-DATA] informPaymentResult {ResponseCode=" + responseCode + ",ResponseMessage=" + message + ",TransactionID=" + 0 + "}");

            return (response);
        }

        private bool IsValid(InformPaymentRequestBody request)
        {
            if (String.IsNullOrEmpty(request.SucursalName) || request.SucursalNumber < 0 || String.IsNullOrEmpty(request.Comment))
                return false;

            return true;
        }
    }
}