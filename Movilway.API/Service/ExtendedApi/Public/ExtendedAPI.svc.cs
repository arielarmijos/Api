using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.Provider;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System.Configuration;

namespace Movilway.API.Service.ExtendedApi.Public
{
    [ServiceBehavior(Namespace = "http://api.movilway.net/schema/extended",
                     ConcurrencyMode = ConcurrencyMode.Multiple,
                     InstanceContextMode = InstanceContextMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Allowed)]
    public class ExtendedAPI : IExtendedAPI
    {
        public TWrapper WrapResponse<TWrapper, TWrapped>(IMovilwayApiResponse response)
            where TWrapped : IMovilwayApiResponse
        {
            var wrappedResponse = Activator.CreateInstance<TWrapper>();

            var movilwayApiResponseWrapper = wrappedResponse as IMovilwayApiResponseWrapper<TWrapped>;
            if (movilwayApiResponseWrapper != null)
                movilwayApiResponseWrapper.Response = (TWrapped)response;
            return (wrappedResponse);
        }

        public BuyStockResponse BuyStock(BuyStockRequest request)
        {
            return WrapResponse<BuyStockResponse, BuyStockResponseBody>(
                new ServiceExecutionDelegator<BuyStockResponseBody, BuyStockRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.BuyStock));
        }

        public CashInResponse CashIn(CashInRequest request)
        {
            //TODO SECURE EXECUTION NO SE UTILIZA
            return WrapResponse<CashInResponse, CashInResponseBody>(new ServiceExecutionDelegator<CashInResponseBody, CashInRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.CashIn));
        }

        public CashOutResponse CashOut(CashOutRequest request)
        {
            //TODO SECURE EXECUTION NO SE UTILIZA
            return WrapResponse<CashOutResponse, CashOutResponseBody>(new ServiceExecutionDelegator<CashOutResponseBody, CashOutRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.CashOut));
        }

        public ChangePinResponse ChangePin(ChangePinRequest request)
        {
            return WrapResponse<ChangePinResponse, ChangePinResponseBody>(
                new ServiceExecutionDelegator<ChangePinResponseBody, ChangePinRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.ChangePin));
        }

        public CreateMoviPinResponse CreateMoviPin(CreateMoviPinRequest request)
        {
            //TODO SECURE EXECUTION NO SE UTILIZA
            return WrapResponse<CreateMoviPinResponse, CreateMoviPinResponseBody>(new ServiceExecutionDelegator<CreateMoviPinResponseBody, CreateMoviPinRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.CreateMoviPin));
        }

        public TransferResponse ExternalTransfer(TransferRequest request)
        {
            //TODO SECURE EXECUTION PROBRAR ANTES Y DESPUES
            return WrapResponse<TransferResponse, TransferResponseBody>(
                new ServiceExecutionDelegator<TransferResponseBody, TransferRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.ExternalTransfer));


           // return WrapResponse<TransferResponse, TransferResponseBody>(
           //     new ServiceExecutionDelegator<TransferResponseBody, TransferRequestBody>().ResolveRequestService(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.ExternalTransfer));
        }

        public GetAgentInfoResponse GetAgentInfo(GetAgentInfoRequest request)
        {
            return WrapResponse<GetAgentInfoResponse, GetAgentInfoResponseBody>(
                new ServiceExecutionDelegator<GetAgentInfoResponseBody, GetAgentInfoRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetAgentInfo));
        }

        public GetBalanceResponse GetBalance(GetBalanceRequest request)
        {
            return WrapResponse<GetBalanceResponse, GetBalanceResponseBody>(
                new ServiceExecutionDelegator<GetBalanceResponseBody, GetBalanceRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetBalance));
        }

        public GetLoanTokenResponse GetLoanToken(GetLoanTokenRequest request)
        {
            return WrapResponse<GetLoanTokenResponse, GetLoanTokenResponseBody>(
                new ServiceExecutionDelegator<GetLoanTokenResponseBody, GetLoanTokenRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetLoanToken));
        }

        public GetBankListResponse GetBankList(GetBankListRequest request)
        {
            return WrapResponse<GetBankListResponse, GetBankListResponseBody>(
                new ServiceExecutionDelegator<GetBankListResponseBody, GetBankListRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetBankList));
        }

        public GetBankListAppResponse GetBankListApp(GetBankListAppRequest request)
        {
            return WrapResponse<GetBankListAppResponse, GetBankListAppResponseBody>(
                new ServiceExecutionDelegator<GetBankListAppResponseBody, GetBankListAppRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetBankListApp));
        }

        public GetChildListResponse GetChildList(GetChildListRequest request)
        {
            return WrapResponse<GetChildListResponse, GetChildListResponseBody>(
                new ServiceExecutionDelegator<GetChildListResponseBody, GetChildListRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetChildList));
        }


     


        public GetAgentDistributionListResponse GetAgentDistributionList(GetAgentDistributionListRequest request)
        {
            return WrapResponse<GetAgentDistributionListResponse, GetAgentDistributionListResponseBody>(
                new ServiceExecutionDelegator<GetAgentDistributionListResponseBody, GetAgentDistributionListRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetAgentDistributionList));
        }

        public InformPaymentResponse InformPayment(InformPaymentRequest request)
        {
            return WrapResponse<InformPaymentResponse, InformPaymentResponseBody>(
                new ServiceExecutionDelegator<InformPaymentResponseBody, InformPaymentRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.InformPayment));
        }

        public GetPendingDistributionsResponse GetPendingDistributions(GetPendingDistributionsRequest request)
        {
            return WrapResponse<GetPendingDistributionsResponse, GetPendingDistributionsResponseBody>(
                new ServiceExecutionDelegator<GetPendingDistributionsResponseBody, GetPendingDistributionsRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetPendingDistributions));
        }

        public ProcessDistributionResponse ProcessDistribution(ProcessDistributionRequest request)
        {
            return WrapResponse<ProcessDistributionResponse, ProcessDistributionResponseBody>(
                new ServiceExecutionDelegator<ProcessDistributionResponseBody, ProcessDistributionRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.ProcessDistribution));
        }

        public GetLastDistributionsResponse GetLastDistributions(GetLastDistributionsRequest request)
        {
            return WrapResponse<GetLastDistributionsResponse, GetLastDistributionsResponseBody>(
                new ServiceExecutionDelegator<GetLastDistributionsResponseBody, GetLastDistributionsRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetLastDistributions));
        }

        public GetExternalBalanceResponse GetExternalBalance(GetExternalBalanceRequest request)
        {
            return WrapResponse<GetExternalBalanceResponse, GetExternalBalanceResponseBody>(new ServiceExecutionDelegator<GetExternalBalanceResponseBody, GetExternalBalanceRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.iBank, ApiServiceName.GetExternalBalance));
        }

        public GetLastTransactionsResponse GetLastTransactions(GetLastTransactionsRequest request)
        {
            return WrapResponse<GetLastTransactionsResponse, GetLastTransactionsResponseBody>(
                new ServiceExecutionDelegator<GetLastTransactionsResponseBody, GetLastTransactionsRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetLastTransactions));
        }
        
        public GetTransactionsReportResponse GetTransactionsReport(GetTransactionsReportRequest request)
        {
            return WrapResponse<GetTransactionsReportResponse, GetTransactionsReportResponseBody>(
              new ServiceExecutionDelegator<GetTransactionsReportResponseBody, GetTransactionsReportRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetTransactionsReport));
        }
        
        public GetAgentClosingDetailsResponse GetAgentClosingDetails(GetAgentClosingDetailsRequest request)
        {
            return WrapResponse<GetAgentClosingDetailsResponse, GetAgentClosingDetailsResponseBody>(
              new ServiceExecutionDelegator<GetAgentClosingDetailsResponseBody, GetAgentClosingDetailsRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetAgentClosingDetails));
        }
        
        public GetAgentClosingsResponse GetAgentClosings(GetAgentClosingsRequest request)
        {
            return WrapResponse<GetAgentClosingsResponse, GetAgentClosingsResponseBody>(
              new ServiceExecutionDelegator<GetAgentClosingsResponseBody, GetAgentClosingsRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetAgentClosings));
        }

        public GetNewAgentsReportResponse GetNewAgentsReport(GetNewAgentsReportRequest request)
        {
            return WrapResponse<GetNewAgentsReportResponse, GetNewAgentsReportResponseBody>(
              new ServiceExecutionDelegator<GetNewAgentsReportResponseBody, GetNewAgentsReportRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetNewAgentsReport));
        }

        public GetBlockedAgenciesByInactivityReportResponse GetBlockedAgenciesByInactivity(GetBlockedAgenciesByInactivityReportRequest request)
        {
            return WrapResponse<GetBlockedAgenciesByInactivityReportResponse, GetBlockedAgenciesByInactivityResponseBody>(
              new ServiceExecutionDelegator<GetBlockedAgenciesByInactivityResponseBody, GetBlockedAgenciesByInactivityRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetBlockedAgenciesByInactivityReport));
        }

        public GetDailyCutReportResponse GetDailyCutReport(GetDailyCutReportRequest request)
        {
            return WrapResponse<GetDailyCutReportResponse, GetDailyCutReportResponseBody>(
              new ServiceExecutionDelegator<GetDailyCutReportResponseBody, GetDailyCutReportRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetDailyCutReport));
        }

        public SetAgentCloseResponse SetAgentClose(SetAgentCloseRequest request)
        {
            return WrapResponse<SetAgentCloseResponse, SetAgentCloseResponseBody>(
              new ServiceExecutionDelegator<SetAgentCloseResponseBody, SetAgentCloseRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.SetAgentClose));
        }

        public GetAgentDistributionsResponse GetAgentDistributions(GetAgentDistributionsRequest request)
        {
            return WrapResponse<GetAgentDistributionsResponse, GetAgentDistributionsResponseBody>(
              new ServiceExecutionDelegator<GetAgentDistributionsResponseBody, GetAgentDistributionsRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetAgentDistributions));
        }

        public GetParentListResponse GetParentList(GetParentListRequest request)
        {
            return WrapResponse<GetParentListResponse, GetParentListResponseBody>(
                new ServiceExecutionDelegator<GetParentListResponseBody, GetParentListRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetParentList));
        }

        public GetProductListResponse GetProductList(GetProductListRequest request)
        {
            return WrapResponse<GetProductListResponse, GetProductListResponseBody>(
                new ServiceExecutionDelegator<GetProductListResponseBody, GetProductListRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetProductList));
        }

        public GetSessionResponse GetSession(GetSessionRequest request)
        {
            return WrapResponse<GetSessionResponse, GetSessionResponseBody>(
                new ServiceExecutionDelegator<GetSessionResponseBody, GetSessionRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetSession));
        }

        public GetScoreResponse GetScore(GetScoreRequest request)
        {
            return WrapResponse<GetScoreResponse, GetScoreResponseBody>(
                new ServiceExecutionDelegator<GetScoreResponseBody, GetScoreRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetScore));
        }

        public GetTransactionResponse GetTransaction(GetTransactionRequest request)
        {
            return WrapResponse<GetTransactionResponse, GetTransactionResponseBody>(
                new ServiceExecutionDelegator<GetTransactionResponseBody, GetTransactionRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetTransaction));
        }

        public GetSalesSummaryResponse GetSalesSummary(GetSalesSummaryRequest request)
        {
            return WrapResponse<GetSalesSummaryResponse, GetSalesSummaryResponseBody>(
                new ServiceExecutionDelegator<GetSalesSummaryResponseBody, GetSalesSummaryRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetSalesSummary));
        }

        public GetSalesSummaryResponse GetSalesSummaryReport(GetSalesSummaryReportRequest request)
        {
            return WrapResponse<GetSalesSummaryResponse, GetSalesSummaryResponseBody>(
                new ServiceExecutionDelegator<GetSalesSummaryResponseBody, GetSalesSummaryReportRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetSalesSummaryReport));
        }

        public MoviPaymentResponse MoviPayment(MoviPaymentRequest request)
        {
            //TODO SECURE EXECUTION NO SE UTILIZA
            return WrapResponse<MoviPaymentResponse, MoviPaymentResponseBody>(new ServiceExecutionDelegator<MoviPaymentResponseBody, MoviPaymentRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.MoviPayment));
        }

        public PayStockResponse PayStock(PayStockRequest request)
        {
            return WrapResponse<PayStockResponse, PayStockResponseBody>(new ServiceExecutionDelegator<PayStockResponseBody, PayStockRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.PayStock));
        }

        public ProcessExternalTransactionResponse ProcessExternalTransaction(ProcessExternalTransactionRequest request)
        {
            //TODO SECURE EXECUTION NO SE UTILIZA
            return WrapResponse<ProcessExternalTransactionResponse, ProcessExternalTransactionResponseBody>(new ServiceExecutionDelegator<ProcessExternalTransactionResponseBody, ProcessExternalTransactionRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.iBank, ApiServiceName.ProcessExternalTransaction));
        }

        public RegisterDebtPaymentResponse RegisterDebtPayment(RegisterDebtPaymentRequest request)
        {
            return WrapResponse<RegisterDebtPaymentResponse, RegisterDebtPaymentResponseBody>(new ServiceExecutionDelegator<RegisterDebtPaymentResponseBody, RegisterDebtPaymentRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.RegisterDebtPayment));
        }

        public RequestStockResponse RequestStock(RequestStockRequest request)
        {
            return WrapResponse<RequestStockResponse, RequestStockResponseBody>(new ServiceExecutionDelegator<RequestStockResponseBody, RequestStockRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.RequestStock));
        }

        public SellResponse Sell(SellRequest request)
        {
            //TODO SECURE EXECUTION NO SE UTILIZA
            return WrapResponse<SellResponse, SellResponseBody>(new ServiceExecutionDelegator<SellResponseBody, SellRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.Sell));
        }
       
        public QueryPaymentResponse QueryPayment(QueryPaymentRequest request)
        {
            return WrapResponse<QueryPaymentResponse, QueryPaymentResponseBody>(
              new ServiceExecutionDelegator<QueryPaymentResponseBody, QueryPaymentRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.QueryPayment));

            //return WrapResponse<TopUpResponse, TopUpResponseBody>(
            //  new ServiceExecutionDelegator<TopUpResponseBody, TopUpRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.TopUp));
        }


        public TopUpResponse TopUp(TopUpRequest request)
        {
            return WrapResponse<TopUpResponse, TopUpResponseBody>(
              new ServiceExecutionDelegator<TopUpResponseBody, TopUpRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.TopUp));

             // return WrapResponse<TopUpResponse, TopUpResponseBody>(
            //  new ServiceExecutionDelegator<TopUpResponseBody, TopUpRequestBody>().ResolveRequestService(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.TopUp));
        }

        public TransferResponse Transfer(TransferRequest request)
        {


            return WrapResponse<TransferResponse, TransferResponseBody>(
                new ServiceExecutionDelegator<TransferResponseBody, TransferRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.Transfer));

           // return WrapResponse<TransferResponse, TransferResponseBody>(
            //    new ServiceExecutionDelegator<TransferResponseBody, TransferRequestBody>().ResolveRequestService(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.Transfer)); ;
        }

        public TransferStockResponse TransferStock(TransferStockRequest request)
        {
            //TODO SECURE EXECUTION NO SE UTILIZA
            return WrapResponse<TransferStockResponse, TransferStockResponseBody>(new ServiceExecutionDelegator<TransferStockResponseBody, TransferStockRequestBody>().ResolveRequest(request.Request,
                 ApiTargetPlatform.Utiba, ApiServiceName.TransferStock));
        }

        public ChangeBranchStatusResponse ChangeBranchStatus(ChangeBranchStatusRequest request)
        {
            return WrapResponse<ChangeBranchStatusResponse, ChangeBranchStatusResponseBody>(
                new ServiceExecutionDelegator<ChangeBranchStatusResponseBody, ChangeBranchStatusRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.ChangeBranchStatus));
        }

        //public NotiwayResponse Notiway(NotiwayRequest request)
        //{
        //    return WrapResponse<NotiwayResponse, NotiwayResponseBody>(new ServiceExecutionDelegator<NotiwayResponseBody, NotiwayRequestBody>().ResolveRequest(request.Request,
        //         (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.Notiway));
        //}




        //TODO SEGURIDAD DEL API DUDA SI DEBE IR A UN NIVEL MAS ABAJO
        public GetTrustedDevicesResponse GetTrustedDevices(GetTrustedDevicesRequest request)
        {
            return WrapResponse<GetTrustedDevicesResponse, GetTrustedDevicesResponseBody>(new ServiceExecutionDelegator<GetTrustedDevicesResponseBody, GetTrustedDevicesRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetTrustedDevices));
   
        }

        public DeleteTrustedDeviceResponse DeleteTrustedRequest(DeleteTrustedDeviceRequest request)
        {
         return WrapResponse<DeleteTrustedDeviceResponse,DeleteTrustedDeviceResponseBody>(   new ServiceExecutionDelegator<DeleteTrustedDeviceResponseBody,DeleteTrustedDeviceRequestBody>().ResolveRequest(request.Request,(request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.DeleteTrustedDevice));
        }

        public SetStateTrustedDeviceResponse SetStateTrustedDevice(SetStateTrustedDeviceRequest request)
        {
            return WrapResponse<SetStateTrustedDeviceResponse, SetStateTrustedDeviceResponseBody>(new ServiceExecutionDelegator<SetStateTrustedDeviceResponseBody, SetStateTrustedDeviceRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.SetStateTrustedDevice));
        }

        public GetLatestAccessResponse GetLatestAccess(GetLatestAccessRequest request)
        {
            return WrapResponse<GetLatestAccessResponse, GetLatestAccessResponseBody>(new ServiceExecutionDelegator<GetLatestAccessResponseBody, GetLatestAccessRequestBody>().ResolveRequest(request.Request,
               (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetLatestAccess));
        }


        public AddTrustedDeviceResponse AddTrustedDevice(AddTrustedDeviceRequest request)
        {
            //no se valida tokken solo user y pass
            //return WrapResponse<AddTrustedDeviceResponse, AddTrustedDeviceResponseBody>(new SecureServiceExecutionDelegator<AddTrustedDeviceResponseBody, AddTrustedDeviceRequestBody>(ApiSecurityMode.USER)
            //    .ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.AddTrustedDevice));

            return WrapResponse<AddTrustedDeviceResponse, AddTrustedDeviceResponseBody>(new ServiceExecutionDelegator<AddTrustedDeviceResponseBody, AddTrustedDeviceRequestBody>()
              .ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.AddTrustedDevice));
        }

        public ValidateDeviceResponse ValidateDevice(ValidateDeviceRequest request)
        {
            //no se valida tokken solo user y pass
            //throw new NotImplementedException();
            //return WrapResponse<ValidateDeviceResponse, ValidateDeviceResponseBody>(new SecureServiceExecutionDelegator<ValidateDeviceResponseBody, ValidateDeviceRequestBody>(ApiSecurityMode.USER).ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.ValidateDevice));

            return WrapResponse<ValidateDeviceResponse, ValidateDeviceResponseBody>(new ServiceExecutionDelegator<ValidateDeviceResponseBody, ValidateDeviceRequestBody>()
             .ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.ValidateDevice));
        }


        /*public ValidateDeviceResponse ValidateDevice(ValidateDeviceRequest request)
        {
            //no se valida tokken solo user y pass
            //throw new NotImplementedException();
            //return WrapResponse<ValidateDeviceResponse, ValidateDeviceResponseBody>(new SecureServiceExecutionDelegator<ValidateDeviceResponseBody, ValidateDeviceRequestBody>(ApiSecurityMode.USER).ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.ValidateDevice));

            return WrapResponse<ValidateDeviceResponse, ValidateDeviceResponseBody>(new ServiceExecutionDelegator<ValidateDeviceResponseBody, ValidateDeviceRequestBody>()
             .ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.ValidateDevice));
        }*/

        //

        /*
                public GetPinUsersResponse GetPinUsers(GetPinUsersRequest request) {
                    return WrapResponse<GetPinUsersResponse, GetPinUsersResponseBody>(new ServiceExecutionDelegator<GetPinUsersResponseBody, GetPinUsersRequestBody>()
                         .ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetPinUsers));

                }*/


        /// <summary>
        /// Obtiene las distribuciones realizadas por una agencia a su red
        /// </summary>
        /// <param name="request">Filtros de busqueda</param>
        /// <returns>Resultado de la busqueda</returns>
        public GetDistributionsMadeByAgentResponse GetDistributionsMadeByAgent(GetDistributionsMadeByAgentRequest request)
        {
            return WrapResponse<GetDistributionsMadeByAgentResponse, GetDistributionsMadeByAgentResponseBody>(new ServiceExecutionDelegator<GetDistributionsMadeByAgentResponseBody, GetDistributionsMadeByAgentRequestBody>()
             .ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetDistributionsMadeByAgent));
        }




        /// <summary>
        /// Registra la solicutd de reporte en base de datos
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public MonthlyReportResponse MonthlyReport(MonthlyReportRequest request)
        {
            return WrapResponse<MonthlyReportResponse, MonthlyReportResponseBody>(new ServiceExecutionDelegator<MonthlyReportResponseBody, MonthlyReportRequestBody>()
               .ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.MonthlyReport));
        }


        public GetDistributionsMadeByUserResponse GetUserDistributions(GetDistributionsMadeByUserRequest request)
        {
            return WrapResponse<GetDistributionsMadeByUserResponse, GetDistributionsMadeByUserResponseBody>(new ServiceExecutionDelegator<GetDistributionsMadeByUserResponseBody, GetDistributionsMadeByUserRequestBody>()
              .ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetUserDistributionList));
        }

        public GetCutStockResponse GetCutStock(GetCutStockRequest request)
        {
            return WrapResponse<GetCutStockResponse, GetCutStockResponseBody>(new ServiceExecutionDelegator<GetCutStockResponseBody, GetCutStockRequestBody>()
               .ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetCutStock));
        }

        public GetCutsResponse GetCuts(GetCutsRequest request)
        {
            return WrapResponse<GetCutsResponse, GetCutsResponseBody>(new ServiceExecutionDelegator<GetCutsResponseBody, GetCutsRequestBody>()
              .ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetCutsProvider));
        }
    }
}
