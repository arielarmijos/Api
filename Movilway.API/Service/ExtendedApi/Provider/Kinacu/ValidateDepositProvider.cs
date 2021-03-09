using Movilway.API.Core;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.ValidateDeposit)]
    public class ValidateDepositProvider : AKinacuProvider
    {

        private Logging.ILogger logger = Logging.LoggerFactory.GetLogger(typeof(ValidateDepositProvider));
        protected override Logging.ILogger ProviderLogger
        {
            get
            {
                return logger;
            }
        }

        public override DataContract.Common.IMovilwayApiResponse PerformKinacuOperation(DataContract.Common.IMovilwayApiRequest requestObject, KinacuWebService.SaleInterface kinacuWS, string sessionID)
        {

            // throw new NotImplementedException();
            ValidateDepositRequestBody request = (ValidateDepositRequestBody)requestObject;
            ValidateDepositResponseBody response = new ValidateDepositResponseBody();

            try
            {
                //OK
                response.ResponseCode = 0;
                response.ResponseMessage = "OK";
                response.DepositResult = Utils.GetDepositStatus(request.Date.ToString("yyyyMMdd"), request.Amount, request.TransactionReference, request.BankName);
                response.TransactionID = new Random().Next(100000, 999999);
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;//"ERROR INESPERADO";
                response.DepositResult = "EX";
                response.TransactionID = 0;

                string mensaje = String.Concat("[API] ", base.LOG_PREFIX, "[ValidateDepositProvider] ", ". Exception: ", ex.Message, ". ", ex.StackTrace);
                logger.ErrorLow(mensaje);
            }
            return response;
        }
    }
}