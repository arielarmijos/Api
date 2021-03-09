using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract.Payment;
using Movilway.API.Service.ExtendedApi.Provider.Payment.Model;
using Movilway.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.Payment
{
    internal partial class PaymentProvider
    {

        public CrearTransaccionResponse CrearTransaccion(CrearTransaccionRequest crearTransactionRequest) {

            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                   .Message("Started"));

            CrearTransaccionResponse response = new CrearTransaccionResponse();

            string sessionId = this.GetSessionId(crearTransactionRequest, response, out this.errorMessage);

            this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                   .Message("[" + sessionId + "] " + "Creando Transaccion"));
            if (this.errorMessage != ErrorMessagesMnemonics.None)
            {
                this.LogResponse(response);
                return response;
            }

            try
            {
                int id = new TransactionDB().GetTransaction(crearTransactionRequest);
                this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                   .Message("[" + sessionId + "] " + "Creada ").Tag("id").Value(id));
                return new CrearTransaccionResponse { TransactionID = id, ResponseMessage = "Ok", ResponseCode = 0 };
            }
            catch (Exception e)
            {
                this.ProviderLogger.ExceptionHigh(() => TagValue.New().MethodName(methodName).Exception(e));
                return new CrearTransaccionResponse { TransactionID = 0, ResponseMessage = e.Message, ResponseCode = 99 };
            }
            finally {
                this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                       .Message("[" + sessionId + "] End" ));
            }
        }
    }
}