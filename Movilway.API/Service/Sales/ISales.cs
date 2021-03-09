using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Movilway.API.Service.Sales.External;
using Movilway.API.Service.External;

namespace Movilway.API.Service.Sales
{
    [ServiceContract(Namespace = "http://api.movilway.net")]
    public interface ISales:ISessionServiceContract, ILoginServiceContract
    {
        [OperationContract]
        TopUpResponse TopUp(TopUpRequest externalRequest);
        [OperationContract]
        TopUpExtendedResponse TopUpExtended(TopUpExtendedRequest externalRequest);

        [OperationContract]
        GetTransactionResponse GetTransaction(GetTransactionRequest externalRequest);
        [OperationContract]
        GetTransactionExtendedResponse GetTransactionExtended(GetTransactionExtendedRequest externalRequest);

        [OperationContract]
        NewSaleWithExternalIDResponse NewSaleWithExternalID(NewSaleWithExternalIDRequest externalRequest);
        [OperationContract]
        NewSaleWithExternalIDExtendedResponse NewSaleWithExternalIDExtended(NewSaleWithExternalIDExtendedRequest externalRequest);

        [OperationContract]
        SaleStateByExternalIDResponse SaleStateByExternalID(SaleStateByExternalIDRequest externalRequest);
        [OperationContract]
        SaleStateByExternalIDExtendedResponse SaleStateByExternalIDExtended(SaleStateByExternalIDExtendedRequest externalRequest);

        [OperationContract]
        BalanceResponse Balance(BalanceRequest externalRequest);
        [OperationContract]
        BalanceExtendedResponse BalanceExtended(BalanceExtendedRequest balanceExtendedRequest);

    }
}
