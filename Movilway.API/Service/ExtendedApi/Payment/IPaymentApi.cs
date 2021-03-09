// <copyright file="IPaymentApi.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.Payment
{
    using Movilway.API.Service.ExtendedApi.DataContract.Payment;
    using System;
    using System.Collections.Generic;
    using System.EnterpriseServices;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Text;
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPayment" in both code and config file together.
    [ServiceContract(Namespace = "http://api.movilway.net/schema/extendedpayment")]
    [XmlSerializerFormat]
    public interface IPaymentApi
    {
        [OperationContract]
        [Description("Obtiene lista de bancos.")]
        GetBanklistResponse GetPGBanklist(GetBankListRequest bankListRequest);

        [OperationContract]
        [Description("Inicia Proceso de Pago.")]
        IniciarPagoResponse InitPayment(IniciarPagoRequest initPaymentRequest);

        [OperationContract]
        [Description("Confirma el pago.")]
        ConfirmaRespuestaPagoResponse ConfirmPayment(ConfirmaRespuestaPagoRequest confirmPaymentRequest);

        [OperationContract]
        [Description("Crear Transacción en BD")]
        CrearTransaccionResponse CreateTransaction(CrearTransaccionRequest createTransactionRequest);

        [OperationContract]
        [Description("Actualiza estado de la transacción en BD")]
        ActualizarTransaccionResponse UpdateTransaction(ActualizarTransaccionRequest actualizarTransaccionRequest);

        [OperationContract]
        [Description("Obtiene el Banco, y numero de cuenta del usuario en Kinacu")]
        GetPSEBankResponse GetPSEBank(GetPSEBankRequest obtenerBancoPSEequest);

    }
}
