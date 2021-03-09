using Movilway.API.Service.ExtendedApi.DataContract.Payment;
using Movilway.API.Service.ExtendedApi.Provider.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.Payment
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Payment" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Payment.svc or Payment.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PaymentApi : IPaymentApi
    {
        public GetBanklistResponse GetPGBanklist(GetBankListRequest bankListRequest) {

            //return new GetPGBanklistResponse { BankList = new List<Bank> { new Bank { Id = "1", Name = "Bancolombia" }, new Bank { Id = "2", Name = "Davivienda" }, new Bank { Id = "3", Name = "Caja Social" } }, ResponseCode = 0, ResponseMessage="Respuesta" };
            return new PaymentProvider().GetBankList(bankListRequest);
        }

        public IniciarPagoResponse InitPayment(IniciarPagoRequest initPaymentRequest) {

            return new PaymentProvider().InitPayment(initPaymentRequest);
        }

        public ConfirmaRespuestaPagoResponse ConfirmPayment(ConfirmaRespuestaPagoRequest confirmPaymentRequest) {
            return new PaymentProvider().ConfirmPayment(confirmPaymentRequest);
        }


        public CrearTransaccionResponse CreateTransaction(CrearTransaccionRequest createTransactionRequest)
        {

            return new PaymentProvider().CrearTransaccion(createTransactionRequest);
            
        }

        public ActualizarTransaccionResponse UpdateTransaction(ActualizarTransaccionRequest actualizarTransaccionRequest) {

            return new PaymentProvider().ActualizarTransaccion(actualizarTransaccionRequest);
        }

        public GetPSEBankResponse GetPSEBank(GetPSEBankRequest obtenerBancoPSEequest)
        {
            return new PaymentProvider().ObtenerBancoPSE(obtenerBancoPSEequest);
        }
    }
}
