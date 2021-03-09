using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.Provider;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;

namespace Movilway.API.Service.ExtendedApi.Private
{
    [ServiceBehavior(Namespace = "http://api.movilway.net/schema/extended",
                     ConcurrencyMode = ConcurrencyMode.Multiple,
                     InstanceContextMode = InstanceContextMode.Single)]
    public class PrivateExtendedAPI : Trusted.TrustedExtendedAPI, IPrivateExtendedAPI
    {
        public ConsolidateMoviPinResponse ConsolidateMoviPin(ConsolidateMoviPinRequest request)
        {
            return WrapResponse<ConsolidateMoviPinResponse, ConsolidateMoviPinResponseBody>(new ServiceExecutionDelegator<ConsolidateMoviPinResponseBody, ConsolidateMoviPinRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.ConsolidateMoviPin));
        }

        public RegenerateMoviPinResponse RegenerateMoviPin(RegenerateMoviPinRequest request)
        {
            return WrapResponse<RegenerateMoviPinResponse, RegenerateMoviPinResponseBody>(new ServiceExecutionDelegator<RegenerateMoviPinResponseBody, RegenerateMoviPinRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.RegenerateMoviPin));
        }

        public CreateAgentResponse CreateAgent(CreateAgentRequest request)
        {
            return WrapResponse<CreateAgentResponse, CreateAgentResponseBody>(
                new ServiceExecutionDelegator<CreateAgentResponseBody, CreateAgentRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.CreateAgent));
        }

        public GetChildListResponse GetChildListById(GetChildListByIdRequest request)
        {
            return WrapResponse<GetChildListResponse, GetChildListResponseBody>(
                new ServiceExecutionDelegator<GetChildListResponseBody, GetChildListRequestByIdBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetChildListById));
        }



        public CreateAgentResponse UpdateAgent(CreateAgentRequest request)
        {
            return WrapResponse<CreateAgentResponse, CreateAgentResponseBody>(
                new ServiceExecutionDelegator<CreateAgentResponseBody, CreateAgentRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.UpdateAgent));
        }




        public GetAgentResponse GetAgentEdit(GetAgentRequest request)
        {
            return WrapResponse<GetAgentResponse, GetAgentResponseBody>(new ServiceExecutionDelegator<GetAgentResponseBody, GetAgentRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetAgent));
        }




        public GetAgentGroupsResponse GetAgentGroups(GetAgentGroupsRequest request)
        {
            return WrapResponse<GetAgentGroupsResponse, GetAgentGroupsResponseBody>(
                new ServiceExecutionDelegator<GetAgentGroupsResponseBody, GetAgentGroupsRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetAgentGroups));
        }

        public GetParametersResponse GetParameters(GetParametersRequest request)
        {
            return WrapResponse<GetParametersResponse, GetParametersResponseBody>(new ServiceExecutionDelegator<GetParametersResponseBody, GetParametersRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Kinacu, ApiServiceName.GetParameters));
        }

        public GetRolesResponse GetRoles(GetRolesRequest request)
        {
            return WrapResponse<GetRolesResponse, GetRolesResponseBody>(new ServiceExecutionDelegator<GetRolesResponseBody, GetRolesRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Kinacu, ApiServiceName.GetRoles));
        }

        public GetGroupListResponse GetGroupList(GetGroupListRequest request)
        {
            return WrapResponse<GetGroupListResponse, GetGroupListResponseBody>(new ServiceExecutionDelegator<GetGroupListResponseBody, GetGroupListRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.GetGroupList));
        }

        public GetTodayMoviPaymentsResponse GetTodayMoviPayments(GetTodayMoviPaymentsRequest request)
        {
            return WrapResponse<GetTodayMoviPaymentsResponse, GetTodayMoviPaymentsResponseBody>(new ServiceExecutionDelegator<GetTodayMoviPaymentsResponseBody, GetTodayMoviPaymentsRequestBody>().ResolveRequest(request.Request,
               ApiTargetPlatform.Utiba, ApiServiceName.GetTodayMoviPayments));
        }

        public GetTransactionsInRangeResponse GetTransactionsInRange(GetTransactionsInRangeRequest request)
        {
            return WrapResponse<GetTransactionsInRangeResponse, GetTransactionsInRangeResponseBody>(new ServiceExecutionDelegator<GetTransactionsInRangeResponseBody, GetTransactionsInRangeRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.GetTransactionsInRange));
        }




        public MapAgentToGroupResponse MapAgentToGroup(MapAgentToGroupRequest request)
        {
            return WrapResponse<MapAgentToGroupResponse, MapAgentToGroupResponseBody>(new ServiceExecutionDelegator<MapAgentToGroupResponseBody, MapAgentToGroupRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.MapAgentToGroup));
        }

        public UnMapAgentToGroupResponse UnMapAgentToGroup(UnMapAgentToGroupRequest request)
        {
            return WrapResponse<UnMapAgentToGroupResponse, UnMapAgentToGroupResponseBody>(new ServiceExecutionDelegator<UnMapAgentToGroupResponseBody, UnMapAgentToGroupRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.UnMapAgentToGroup));
        }

        public ValidateMoviPinResponse ValidateMoviPin(ValidateMoviPinRequest request)
        {
            return WrapResponse<ValidateMoviPinResponse, ValidateMoviPinResponseBody>(new ServiceExecutionDelegator<ValidateMoviPinResponseBody, ValidateMoviPinRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.ValidateMoviPin));
        }

        public String GetAPIVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }



        public LoginAvailableResponse LoginAvaiable(LoginAvailableRequest request)
        {
            return WrapResponse<LoginAvailableResponse, LoginAvailableResponseBody>(
                new ServiceExecutionDelegator<LoginAvailableResponseBody, LoginAvailableRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.LoginAvailable));
        }






        public GetTypeAccessResponse GetTypeAccess(GetValuesRequest request)
        {
            return WrapResponse<GetTypeAccessResponse, GetTypeAccessResponseBody>(
                 new ServiceExecutionDelegator<GetTypeAccessResponseBody, GetValuesRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetTypeAccess));
        }



        /**/
        public GetPinUsersResponse GetPinUsers(GetPinUsersRequest request)
        {
            return WrapResponse<GetPinUsersResponse, GetPinUsersResponseBody>(new ServiceExecutionDelegator<GetPinUsersResponseBody, GetPinUsersRequestBody>()
                 .ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetPinUsers));

        }

        public GetUsersAgentResponse GetUsersAgent(GetUsersAgentRequest request)
        {
            return WrapResponse<GetUsersAgentResponse, GetUsersAgentResponseBody>(new ServiceExecutionDelegator<GetUsersAgentResponseBody, GetUsersAgentRequestBody>()
                 .ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetUsersAgent));

        }
        public SetUserStatusResponse SetUserStatus(SetUserStatusRequest request)
        {
            return WrapResponse<SetUserStatusResponse, SetUserStatusResponseBody>(new ServiceExecutionDelegator<SetUserStatusResponseBody, SetUserStatusRequestBody>()
                 .ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.SetUserStatus));

        }
        public ChangePasswordResponse ChangePassword(ChangePasswodRequest request)
        {
            return WrapResponse<ChangePasswordResponse, ChangePasswordResponseBody>(new ServiceExecutionDelegator<ChangePasswordResponseBody, ChangePasswodRequestBody>()
                    .ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.ChangePassword));
        }

        public CreditNoteResponse CreditNote(CreditNoteRequest request)
        {
            return WrapResponse<CreditNoteResponse, CreditNoteResponseBody>(new ServiceExecutionDelegator<CreditNoteResponseBody, CreditNoteRequestBody>()
                    .ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.CreditNote));
        }


        public GetSalesSummaryDashBoardResponse GetSalesSummaryDashBoard(GetSalesSummaryDashBoardRequest request)
        {
            return WrapResponse<GetSalesSummaryDashBoardResponse, GetSalesSummaryDashBoardResponseBody>(
                new ServiceExecutionDelegator<GetSalesSummaryDashBoardResponseBody, GetSalesSummaryDashBoardRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetSalesSummaryDashBoard));
        }

        public GetPurchasesSummaryDashBoardResponse GetPurchasesSummaryDashBoard(GetPurchasesSummaryDashBoardRequest request)
        {
            return WrapResponse<GetPurchasesSummaryDashBoardResponse, GetPurchasesSummaryDashBoardResponseBody>(
                new ServiceExecutionDelegator<GetPurchasesSummaryDashBoardResponseBody, GetPurchasesSummaryDashBoardRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetPurchasesSummaryDashBoard));
        }


        public UpdateContactInfoAgentResponse UpdateContactInfoAgent(UpdateContactInfoAgentRequest request)
        {
            return WrapResponse<UpdateContactInfoAgentResponse, UpdateContactInfoAgentResponseBody>(
                new ServiceExecutionDelegator<UpdateContactInfoAgentResponseBody, UpdateContactInfoAgentRequestBody>().ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.UpdateContactInfoAgent));
        }

        public DebitNoteResponse DebitNote(DebitNoteRequest request)
        {
            return WrapResponse<DebitNoteResponse, DebitNoteResponseBody>(new ServiceExecutionDelegator<DebitNoteResponseBody, DebitNoteRequestBody>()
                    .ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.DebitNote));
        }

        public TransferCommissionResponse TransferCommission(TransferCommissionRequest request)
        {
            return WrapResponse<TransferCommissionResponse, TransferCommissionResponseBody>(new ServiceExecutionDelegator<TransferCommissionResponseBody, TransferCommissionRequestBody>()
                    .ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.TransferCommission));
        }

       
        public GetAuditoryUsersReportResponse GetAuditoryUsersReport(GetAuditoryUsersReportRequest request)
        {
            return WrapResponse<GetAuditoryUsersReportResponse, GetAuditoryUsersReportResponsetBody>(new ServiceExecutionDelegator<GetAuditoryUsersReportResponsetBody, GetAuditoryUsersReportRequestBody>()
                    .ResolveRequest(request.Request, (request.Request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"]), ApiServiceName.GetAuditoryUsersReport));
        }

        public String Test()
        {
            string client_id = "client_id_data";
            string mobile = "mobile_data";
            string message = "message_data";
            string customer_id = "customer_id";
            var result = String.Format("{0} -- {1}", Movilway.API.Util.SMSTelemoDispatcher.Send(client_id, mobile, message), Movilway.API.Util.SMSTelemoDispatcher.SmsToCustomer(client_id, customer_id, mobile, message));


            return result;
        }

     


        //private static readonly Movilway.Logging.ILogger logger = Logging.LoggerFactory.GetLogger(typeof(Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils));
        //private static void WriteLine(string message)
        //{

        //    logger.InfoHigh(message);
        //}

        //public DebitNoteResponse Test(DebitNoteRequest request)
        //{
        //    string key = "";

        //    DebitNoteResponse result = new DebitNoteResponse();
        //    result.Response = new DebitNoteResponseBody();
        //    result.Response.ResponseCode = 99;
        //    result.Response.ResponseMessage = "";
        //    try
        //    {
        //        System.Web.HttpContext.Current.Session.Add("LOG_PREFIX", System.Threading.Thread.CurrentThread.ManagedThreadId*10+"-"+DateTime.Now.ToString("yyyyMMddHHmmss"));



        //        Dictionary<String, Action> methods = new Dictionary<String, Action>() {

        //        {"SalesSummary", ()=>{        WriteLine("SalesSummary Result = "+  Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.SalesSummary("4", DateTime.Now)); } },
        //        { "SalesSummaryByAgent",()=>{
        //            decimal initialAmount, finalAmount;
        //           WriteLine("SalesSummaryByAgent Result = "+   Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.SalesSummaryByAgent("4", DateTime.Now, out  initialAmount, out  finalAmount)); }

        //        },

        //         { "GetRoles",()=>{


        //            WriteLine("GetRoles Result = "+  Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetRoles("usrservices")); }

        //        },


        //          { "GetParentAgent",()=>{


        //            WriteLine("GetParentAgent Result = "+  Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetParentAgent("usrservices")); }

        //        },


        //        { "GetAgentEmail",()=>{


        //            WriteLine("GetAgentEmail Result = "+  Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetParentAgent("usrservices")); }

        //        },

        //       { "GetUserId",()=>{


        //            WriteLine("GetUserId Result = "+  Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetUserId("usrservices")); }

        //        },

        //         { "GetAgentByAccess",()=>{


        //            WriteLine("GetAgentByAccess Result = "+  Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetAgentByAccess("usrservices")); }

        //        },
        //        { "GetUserId[ agentReference, acceso]",()=>{


        //            WriteLine("GetUserId[ agentReference, acceso] Result = "+  Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetUserId("usrservices",12)); }

        //        },

        //        { "GetAgentInfoById",()=>{


        //            WriteLine("GetAgentInfoById = "+  Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetAgentInfoById("4")); }

        //        },

        //        { "GetAgentInfo",()=>{


        //            WriteLine("GetAgentInfo = "+  Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetAgentInfo("usrservices")); }

        //        },
        //         { "GetValidatedEmailByLogin",()=>{


        //            WriteLine("GetValidatedEmailByLogin = "+  Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetValidatedEmailByLogin("usrservices")); }

        //        },
        //        { "GetValidatedEmailById",()=>{


        //            WriteLine("GetValidatedEmailById = "+  Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetValidatedEmailById("usrservices")); }

        //        },
        //        //{ "GetScore",()=>{


        //        //    WriteLine("GetScore = "+  Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetScore("4")); }

        //        //},

        //         { "GetAgentDistributionList[agentReference,  agentChildReference,  cutInfo]",()=>{

        //             //DeviceType=12,Agent=4,Platform=1,AgentChild=,CutInfo=true ||  request.Agent, request.AgentChild, request.CutInfo
        //            WriteLine("GetAgentDistributionList[agentReference 4 ,  agentChildReference ,  cutInfo true] = "+  Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetAgentDistributionList("4","","true")); }

        //        },

        //        { "GetAgentDistributionList[agentReference 4,  agentChildReference 5,  cutInfo 1]",()=>{

        //             //DeviceType=12,Agent=4,Platform=1,AgentChild=,CutInfo=true ||  request.Agent, request.AgentChild, request.CutInfo
        //            WriteLine("GetAgentDistributionList[agentReference 4,  agentChildReference 5,  cutInfo 1] = "+  Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetAgentDistributionList("4","5","true")); }

        //        },

        //        { "GetAgentCheckingAccountBalance ",()=>{


        //            WriteLine("GetAgentCheckingAccountBalance  = "+  Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetAgentCheckingAccountBalance("usrservices")); }

        //        },


        //        { "GetAgentExtendedValues ",()=>{


        //            WriteLine("GetAgentExtendedValues  = "+  Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetAgentExtendedValues("4,5,6,7,8,9")); }

        //        },

        //          { "GetAgentName ",()=>{


        //            WriteLine("GetAgentName  = "+  Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetAgentName("usrservices")); }

        //        },


        //    };





        //        foreach (var method in methods)
        //        {

        //            key = method.Key;
        //            method.Value();
        //        }

        //        result.Response.ResponseCode = 0;
        //        result.Response.ResponseMessage = "All Test have run";

        //    }
        //    catch (Exception ex)
        //    {
        //        var message = "Method " + key + " ; " + ex.Message + " || " + ex.GetType().FullName + " || " + ex.StackTrace;
        //        WriteLine(message);


        //        result.Response.ResponseCode = 0;
        //        result.Response.ResponseMessage = "Error "+ message;
        //    }

        //    return result;
        //}

    }
}