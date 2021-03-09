using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Movilway.API.Service.ExtendedApi.DataContract;

namespace Movilway.API.Service.ExtendedApi.Private
{
    [ServiceContract(Namespace = "http://api.movilway.net/schema/extended")]
    public interface IPrivateExtendedAPI : Trusted.ITrustedExtendedAPI
    {
        [OperationContract(Name = "ConsolidateMoviPin")]
        ConsolidateMoviPinResponse ConsolidateMoviPin(ConsolidateMoviPinRequest request);

        [OperationContract(Name = "CreateAgent")]
        CreateAgentResponse CreateAgent(CreateAgentRequest request);

        [OperationContract(Name = "UpdateAgent")]
        CreateAgentResponse UpdateAgent(CreateAgentRequest request);

        [OperationContract(Name = "LoginAvailable")]
        LoginAvailableResponse LoginAvaiable(LoginAvailableRequest request);

        [OperationContract(Name = "GetAgentEdit")]
        GetAgentResponse GetAgentEdit(GetAgentRequest request);

        [OperationContract(Name = "RegenerateMoviPin")]
        RegenerateMoviPinResponse RegenerateMoviPin(RegenerateMoviPinRequest request);

        [OperationContract(Name = "GetAgentGroups")]
        GetAgentGroupsResponse GetAgentGroups(GetAgentGroupsRequest request);

        [OperationContract(Name = "GetGroupList")]
        GetGroupListResponse GetGroupList(GetGroupListRequest request);

        [OperationContract(Name = "GetParameters")]
        GetParametersResponse GetParameters(GetParametersRequest request);

        [OperationContract(Name = "GetRoles")]
        GetRolesResponse GetRoles(GetRolesRequest request);

        [OperationContract(Name = "GetTodayMoviPayments")]
        GetTodayMoviPaymentsResponse GetTodayMoviPayments(GetTodayMoviPaymentsRequest request);

        [OperationContract(Name = "GetTransactionsInRange")]
        GetTransactionsInRangeResponse GetTransactionsInRange(GetTransactionsInRangeRequest request);

        [OperationContract(Name = "MapAgentToGroup")]
        MapAgentToGroupResponse MapAgentToGroup(MapAgentToGroupRequest request);

        [OperationContract(Name = "UnMapAgentToGroup")]
        UnMapAgentToGroupResponse UnMapAgentToGroup(UnMapAgentToGroupRequest request);

        [OperationContract(Name = "ValidateMoviPin")]
        ValidateMoviPinResponse ValidateMoviPin(ValidateMoviPinRequest request);

        [OperationContract(Name = "GetAPIVersion")]
        String GetAPIVersion();


        [OperationContract(Name = "GetChildListById")]
        GetChildListResponse GetChildListById(GetChildListByIdRequest request);


        [OperationContract(Name = "GetTypeAccess")]
        GetTypeAccessResponse GetTypeAccess(GetValuesRequest request);



        [OperationContract(Name = "GetPinUsers")]
        GetPinUsersResponse GetPinUsers(GetPinUsersRequest request);



        [OperationContract(Name = "GetUsersAgent")]
        GetUsersAgentResponse GetUsersAgent(GetUsersAgentRequest request);

        [OperationContract(Name = "SetUserStatus")]
        SetUserStatusResponse SetUserStatus(SetUserStatusRequest request);

        [OperationContract(Name = "ChangePassword")]
        ChangePasswordResponse ChangePassword(ChangePasswodRequest request);

        [OperationContract(Name = "CreditNote")]
        CreditNoteResponse CreditNote(CreditNoteRequest request);

        //[OperationContract(Name = "Test")]
        //String Test();
        /// <summary>
        /// Obtiene por dia las ventas efectuadas por una agencia en un determinado rango de fechas
        /// </summary>
        /// <param name="request">Filtros de busqueda</param>
        /// <returns>Resultado de la busqueda</returns>
        [OperationContract(Name = "GetSalesSummaryDashBoard")]
        GetSalesSummaryDashBoardResponse GetSalesSummaryDashBoard(GetSalesSummaryDashBoardRequest request);


        [OperationContract(Name = "GetPurchasesSummaryDashBoard")]
        GetPurchasesSummaryDashBoardResponse GetPurchasesSummaryDashBoard(GetPurchasesSummaryDashBoardRequest request);

        [OperationContract(Name = "UpdateContactInfoAgent")]
        UpdateContactInfoAgentResponse UpdateContactInfoAgent(UpdateContactInfoAgentRequest request);

        [OperationContract(Name = "DebitNote")]
        DebitNoteResponse DebitNote(DebitNoteRequest request);

        //[OperationContract(Name = "Test")]
        //DebitNoteResponse Test(DebitNoteRequest request);

        [OperationContract(Name = "TransferCommission")]
        TransferCommissionResponse TransferCommission(TransferCommissionRequest request);

        [OperationContract(Name = "GetAuditoryUsersReport")]
        GetAuditoryUsersReportResponse GetAuditoryUsersReport(GetAuditoryUsersReportRequest request);
    }
}
