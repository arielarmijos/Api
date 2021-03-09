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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.ProcessDistribution)]
    public class ProcessDistributionProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(ProcessDistributionProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new ProcessDistributionResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0
                };
            
            ProcessDistributionRequestBody request = requestObject as ProcessDistributionRequestBody;
            ProcessDistributionResponseBody response = null;

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[ProcessDistributionProvider] [SEND-DATA] processDistributionParameters {comment=" + request.Comment + ",immediatelyDistribute=" + request.ImmediatelyDistribute + ",isApproved=" + request.IsApproved + ",distributionId=" + request.DistributionId + "}");

            string responseCode = "99", message = "error";

            DistributionSummary deposit;
            var isProductRequest = request.AccountId == 0;
            var result = false;

            int usrId = new IBank.Utils().GetUserId(request.AuthenticationData.Username);

            if (isProductRequest)
            {
                deposit = new IBank.Utils().GetSolicitudProducto(request.DistributionId);
                if (request.IsApproved)
                {
                    logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[ProcessDistributionProvider] isProductRequest y isApproved");
                    //deposit.HasDeposit = false;
                    result = new IBank.Utils().ProcesaSolicitudProducto(deposit.OriginalDistributionID, usrId, deposit.TargetAgentID, deposit.Amount, DateTime.UtcNow.AddHours(new IBank.Utils().GetTimeZone()), request.Comment, ref responseCode, ref message);
                }
                else
                {
                    logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[ProcessDistributionProvider] isProductRequest y not isApproved");
                    result = new IBank.Utils().RechazaSolicitudProducto(deposit.OriginalDistributionID, deposit.TargetAgentID, usrId, DateTime.UtcNow.AddHours(new IBank.Utils().GetTimeZone()), request.Comment, ref responseCode, ref message);
                }
            }
            else
            {
                deposit = new IBank.Utils().GetSolicitud(request.DistributionId);

                deposit.HasDeposit = true;
                
                //TODO escenario prueba dos registro pago
               // deposit.HasDeposit = false;
                //

                if (request.IsApproved)
                {
                    if (request.ImmediatelyDistribute)
                    {
                        logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[ProcessDistributionProvider] not isProductRequest y isApproved y immediatelyDist");
                        result = new IBank.Utils().RegistroPago(deposit.TargetAgentID, usrId, deposit.Amount, deposit.ReferenceNumber, deposit.DepositDate, deposit.AccountNumber, deposit.HasDeposit ? "S" : "N", ref responseCode, ref message, request.Comment, request.DistributionId, DateTime.UtcNow.AddHours(new IBank.Utils().GetTimeZone()));
                    }
                    else
                    {
                        logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[ProcessDistributionProvider] not isProductRequest y isApproved y not immediatelyDist");
                        result = new IBank.Utils().ProcesaAvisoDeposito(deposit.OriginalDistributionID, deposit.TargetAgentID, usrId, deposit.Amount, deposit.ReferenceNumber, deposit.DepositDate, deposit.AccountNumber, request.Comment, ref responseCode, ref message);
                    }
                    //result = new Movilway.API.Service.ExtendedApi.Provider.IBank.Utils().RegistrarDeposito(deposit.TargetAgentID, deposit.Amount, deposit.ReferenceNumber, deposit.DepositDate, deposit.AccountNumber, request.Comment, ref responseCode, ref message, DateTime.UtcNow.AddHours(new Movilway.API.Service.ExtendedApi.Provider.IBank.Utils().GetTimeZone()), "", "");
                }
                else
                {
                    logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[ProcessDistributionProvider] not isProductRequest y not isApproved"); 
                    result = new IBank.Utils().RechazaAvisoDeposito(deposit.OriginalDistributionID, deposit.TargetAgentID, usrId, DateTime.UtcNow.AddHours(new IBank.Utils().GetTimeZone()), request.Comment, ref responseCode, ref message);
                }
            }

            response = new ProcessDistributionResponseBody()
            {
                ResponseCode = int.Parse(responseCode),
                ResponseMessage = message,
                TransactionID = 0
            };

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[ProcessDistributionProvider] [RECV-DATA] processDistributionResult {response={" + responseCode + "," + message + "}}");

            return (response);
        }
    }
}