using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Movilway.API.Service.D2.External;

namespace Movilway.API.Service.D2
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ID2" in both code and config file together.
    [ServiceContract(Namespace = "http://api.movilway.net")]
    public interface ID2: ISessionServiceContract, ILoginServiceContract
    {
        
        [OperationContract]
        CreateCouponResponse CreateCoupon(CreateCouponRequest externalRequest);
        [OperationContract]
        CreateCouponExtendedResponse CreateCouponExtended(CreateCouponExtendedRequest externalRequest);


        [OperationContract]
        CreateCouponWithRecipientResponse CreateCouponWithRecipient(CreateCouponWithRecipientRequest externalRequest);
        [OperationContract]
        CreateCouponWithRecipientExtendedResponse CreateCouponWithRecipientExtended(CreateCouponWithRecipientExtendedRequest externalRequest);

        [OperationContract]
        CouponTransferResponse CouponTransfer(CouponTransferRequest externalRequest);
        [OperationContract]
        CouponTransferExtendedResponse CouponTransferExtended(CouponTransferExtendedRequest externalRequest);


        [OperationContract]
        BuyResponse Buy(BuyRequest externalRequest);
        [OperationContract]
        BuyExtendedResponse BuyExtended(BuyExtendedRequest externalRequest);
        
        
        [OperationContract]
        SellResponse Sell(SellRequest externalRequest);
        [OperationContract]
        SellExtendedResponse SellExtended(SellExtendedRequest externalRequest);

        [OperationContract]
        TransferResponse Transfer(TransferRequest transferRequest);
        [OperationContract]
        TransferExtendedResponse TransferExtended(TransferExtendedRequest transferRequest);

        [OperationContract]
        AccountPaymentResponse AccountPayment(AccountPaymentRequest externalRequest);
        [OperationContract]
        AccountPaymentExtendedResponse AccountPaymentExtended(AccountPaymentExtendedRequest externalRequest);

        [OperationContract]
        CashInResponse CashIn(CashInRequest externalRequest);
        [OperationContract]
        CashInExtendedResponse CashInExtended(CashInExtendedRequest externalRequest);

        [OperationContract]
        GetLastTransactionsResponse GetLastTransactions(GetLastTransactionsRequest externalRequest);
        [OperationContract]
        GetLastTransactionsExtendedResponse GetLastTransactionsExtended(GetLastTransactionsExtendedRequest externalRequest);

        [OperationContract]
        BalanceResponse Balance(BalanceRequest externalRequest);
        [OperationContract]
        BalanceExtendedResponse BalanceExtended(BalanceExtendedRequest externalRequest);

        [OperationContract]
        SummaryResponse Summary(SummaryRequest externalRequest);
        [OperationContract]
        SummaryExtendedResponse SummaryExtended(SummaryExtendedRequest externalRequest);

        [OperationContract]
        BankListResponse BankList(BankListRequest externalRequest);
        [OperationContract]
        BankListExtendedResponse BankListExtended(BankListExtendedRequest externalRequest);

        [OperationContract]
        GetTransactionsInRangeResponse GetTransactionsInRange(GetTransactionsInRangeRequest externalRequest);
    }
}
