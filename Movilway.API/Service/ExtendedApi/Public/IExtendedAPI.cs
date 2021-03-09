using System.ServiceModel;
using Movilway.API.Service.ExtendedApi.DataContract;

namespace Movilway.API.Service.ExtendedApi.Public
{
    [ServiceContract(Namespace = "http://api.movilway.net/schema/extended")]
    public interface IExtendedAPI
    {
        [OperationContract(Name = "BuyStock")]
        BuyStockResponse BuyStock(BuyStockRequest request);

        [OperationContract(Name = "CashIn")]
        CashInResponse CashIn(CashInRequest request);

        [OperationContract(Name = "CashOut")]
        CashOutResponse CashOut(CashOutRequest request);

        [OperationContract(Name = "ChangePin")]
        ChangePinResponse ChangePin(ChangePinRequest request);

        [OperationContract(Name = "CreateMoviPin")]
        CreateMoviPinResponse CreateMoviPin(CreateMoviPinRequest request);

        [OperationContract(Name = "ExternalTransfer")]
        TransferResponse ExternalTransfer(TransferRequest request);

        [OperationContract(Name = "GetAgentInfo")]
        GetAgentInfoResponse GetAgentInfo(GetAgentInfoRequest request);

        [OperationContract(Name = "GetBalance")]
        GetBalanceResponse GetBalance(GetBalanceRequest request);

        [OperationContract(Name = "GetLoanToken")]
        GetLoanTokenResponse GetLoanToken(GetLoanTokenRequest request);

        [OperationContract(Name = "GetBankList")]
        GetBankListResponse GetBankList(GetBankListRequest request);

        [OperationContract(Name = "GetBankListApp")]
        GetBankListAppResponse GetBankListApp(GetBankListAppRequest request);

        [OperationContract(Name = "GetChildList")]
        GetChildListResponse GetChildList(GetChildListRequest request);

      
        [OperationContract(Name = "GetAgentDistributionList")]
        GetAgentDistributionListResponse GetAgentDistributionList(GetAgentDistributionListRequest request);

        [OperationContract(Name = "GetExternalBalance")]
        GetExternalBalanceResponse GetExternalBalance(GetExternalBalanceRequest request);

        [OperationContract(Name = "GetLastTransactions")]
        GetLastTransactionsResponse GetLastTransactions(GetLastTransactionsRequest request);

        [OperationContract(Name = "GetParentList")]
        GetParentListResponse GetParentList(GetParentListRequest request);

        [OperationContract(Name = "GetProductList")]
        GetProductListResponse GetProductList(GetProductListRequest request);
        /*
         [OperationContract(Name = "GetPinUsers")]
         GetPinUsersResponse GetPinUsers(GetPinUsersRequest request);*/

        [OperationContract(Name = "GetSession")]
        GetSessionResponse GetSession(GetSessionRequest request);

        [OperationContract(Name = "GetScore")]
        GetScoreResponse GetScore(GetScoreRequest request);

        [OperationContract(Name = "GetTransaction")]
        GetTransactionResponse GetTransaction(GetTransactionRequest request);

        [OperationContract(Name = "GetTransactionsReport")]
        GetTransactionsReportResponse GetTransactionsReport(GetTransactionsReportRequest request);

        [OperationContract(Name = "GetSalesSummary")]
        GetSalesSummaryResponse GetSalesSummary(GetSalesSummaryRequest request);

        [OperationContract(Name = "GetSalesSummaryReport")]
        GetSalesSummaryResponse GetSalesSummaryReport(GetSalesSummaryReportRequest request);

        [OperationContract(Name = "MoviPayment")]
        MoviPaymentResponse MoviPayment(MoviPaymentRequest request);

        [OperationContract(Name = "PayStock")]
        PayStockResponse PayStock(PayStockRequest request);

        [OperationContract(Name = "ProcessExternalTransaction")]
        ProcessExternalTransactionResponse ProcessExternalTransaction(ProcessExternalTransactionRequest request);

        [OperationContract(Name = "RegisterDebtPayment")]
        RegisterDebtPaymentResponse RegisterDebtPayment(RegisterDebtPaymentRequest request);

        [OperationContract(Name = "RequestStock")]
        RequestStockResponse RequestStock(RequestStockRequest request);

        [OperationContract(Name = "Sell")]
        SellResponse Sell(SellRequest request);

        [OperationContract(Name = "TopUp")]
        TopUpResponse TopUp(TopUpRequest request);

        [OperationContract(Name = "Transfer")]
        TransferResponse Transfer(TransferRequest request);

        [OperationContract(Name = "TransferStock")]
        TransferStockResponse TransferStock(TransferStockRequest request);

        [OperationContract(Name = "InformPayment")]
        InformPaymentResponse InformPayment(InformPaymentRequest request);

        [OperationContract(Name = "GetPendingDistributions")]
        GetPendingDistributionsResponse GetPendingDistributions(GetPendingDistributionsRequest request);

        [OperationContract(Name = "QueryPayment")]
        QueryPaymentResponse QueryPayment(QueryPaymentRequest request);


        [OperationContract(Name = "ProcessDistribution")]
        ProcessDistributionResponse ProcessDistribution(ProcessDistributionRequest request);

        [OperationContract(Name = "GetLastDistributions")]
        GetLastDistributionsResponse GetLastDistributions(GetLastDistributionsRequest request);

        [OperationContract(Name = "ChangeBranchStatus")]
        ChangeBranchStatusResponse ChangeBranchStatus(ChangeBranchStatusRequest request);

        //[OperationContract(Name = "Notiway")]
        //NotiwayResponse Notiway(NotiwayRequest request);

        //metodos de seguridad
        [OperationContract(Name = "GetTrustedDevices")]
        GetTrustedDevicesResponse GetTrustedDevices(GetTrustedDevicesRequest request);

        [OperationContract(Name = "DeleteTrustedDevice")]
        DeleteTrustedDeviceResponse DeleteTrustedRequest(DeleteTrustedDeviceRequest request);

        [OperationContract(Name = "SetStateTrustedDevice")]
        SetStateTrustedDeviceResponse SetStateTrustedDevice(SetStateTrustedDeviceRequest request);


        [OperationContract(Name = "AddTrustedDevice")]
        AddTrustedDeviceResponse AddTrustedDevice(AddTrustedDeviceRequest request);

        [OperationContract(Name = "ValidateDevice")]
        ValidateDeviceResponse ValidateDevice(ValidateDeviceRequest request);

        //GetTypeAccess
        
        //ultimos accessos
        [OperationContract(Name = "GetLatestAccessList")]
        GetLatestAccessResponse GetLatestAccess(GetLatestAccessRequest request);
        //

        /// <summary>
        /// Obtiene las distribuciones realizadas por una agencia a su red
        /// </summary>
        /// <param name="request">Filtros de busqueda</param>
        /// <returns>Resultado de la busqueda</returns>
        [OperationContract(Name = "GetDistributionsMadeByAgent")]
        GetDistributionsMadeByAgentResponse GetDistributionsMadeByAgent(GetDistributionsMadeByAgentRequest request);
        
        [OperationContract(Name = "GetAgentClosingDetails")]
        GetAgentClosingDetailsResponse GetAgentClosingDetails(GetAgentClosingDetailsRequest request);

        [OperationContract(Name = "SetAgentClose")]
        SetAgentCloseResponse SetAgentClose(SetAgentCloseRequest request);
       
        [OperationContract(Name = "GetAgentDistributions")]
        GetAgentDistributionsResponse GetAgentDistributions(GetAgentDistributionsRequest request);
       
        [OperationContract(Name = "GetAgentClosings")]
        GetAgentClosingsResponse GetAgentClosings(GetAgentClosingsRequest request);

        [OperationContract(Name = "GetNewAgentsReport")]
        GetNewAgentsReportResponse GetNewAgentsReport(GetNewAgentsReportRequest request);

        [OperationContract(Name = "GetBlockedAgenciesByInactivity")]
        GetBlockedAgenciesByInactivityReportResponse GetBlockedAgenciesByInactivity(GetBlockedAgenciesByInactivityReportRequest request);

        [OperationContract(Name = "GetDailyCutReport")]
        GetDailyCutReportResponse GetDailyCutReport(GetDailyCutReportRequest request);

        [OperationContract(Name = "MonthlyReport")]
        MonthlyReportResponse MonthlyReport(MonthlyReportRequest request);

        [OperationContract(Name = "GetUserDistributions")]
        GetDistributionsMadeByUserResponse GetUserDistributions(GetDistributionsMadeByUserRequest request);

        [OperationContract(Name = "GetCuts")]
        GetCutsResponse GetCuts(GetCutsRequest request);

        [OperationContract(Name = "GetCutStockReport")]
        GetCutStockResponse GetCutStock(GetCutStockRequest request);

    }
}
