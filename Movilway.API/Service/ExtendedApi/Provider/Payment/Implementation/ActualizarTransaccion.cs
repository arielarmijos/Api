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
        public ActualizarTransaccionResponse ActualizarTransaccion(ActualizarTransaccionRequest actualizarTransaccion) {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                   .Message("Started"));

            ActualizarTransaccionResponse response = new ActualizarTransaccionResponse();

            string sessionId = this.GetSessionId(actualizarTransaccion, response, out this.errorMessage);

            this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                   .Message("[" + sessionId + "] " + "Actualizando transaccion. " +
                        String.Concat(
                            "CodigoTransaccionExterno: ", actualizarTransaccion.CodigoTransaccionExterno,
                            ", CodigoTransaccion: ", actualizarTransaccion.CodigoTransaccion,
                            ", Estado: ", actualizarTransaccion.Estado,
                            ", Mensaje: ", actualizarTransaccion.Mensaje
                        )
                    ));
            if (this.errorMessage != ErrorMessagesMnemonics.None)
            {
                this.LogResponse(response);
                return response;
            }


            try
            {
                int r = new TransactionDB().UpdateTransaction(actualizarTransaccion);

                

                return new ActualizarTransaccionResponse { ResponseMessage = "Ok", ResponseCode = 0, TransactionID = r };

            }
            catch (Exception e)
            {
                this.ProviderLogger.ExceptionHigh(() => TagValue.New().MethodName(methodName).Message("[" + sessionId + "]")
                    .Exception(e));
                return new ActualizarTransaccionResponse { ResponseMessage = e.Message };
            }
            finally {
                this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                       .Message("[" + sessionId + "] " + "Fin Actualización"));
            }
            
        }
    }
}