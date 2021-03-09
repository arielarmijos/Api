using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using Movilway.API.KinacuWebService;
using Movilway.API.KinacuManagementWebService;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.ChangeBranchStatus)]
    public class ChangeBranchStatusProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(ChangeBranchStatusProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new ChangeBranchStatusResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0
                };


            try
            {


                ChangeBranchStatusRequestBody request = requestObject as ChangeBranchStatusRequestBody;
                ChangeBranchStatusResponseBody response = null;
                
                /*
                var getChildListResponse = new ServiceExecutionDelegator
                                                <GetChildListResponseBody, GetChildListRequestBody>().ResolveRequest(
                                                    new GetChildListRequestBody()
                                                    {
                                                        AuthenticationData = new AuthenticationData()
                                                        {
                                                            Username = request.AuthenticationData.Username,
                                                            Password = request.AuthenticationData.Password
                                                        },
                                                        DeviceType = request.DeviceType,
                                                        Agent = request.AuthenticationData.Username,
                                                        Platform = "1"
                                                    }, ApiTargetPlatform.Kinacu, ApiServiceName.GetChildList);*/

                var isChild = Utils.IsChildAgent(request.AuthenticationData.Username, Convert.ToInt32(request.Agent)); 
                /*
                if (getChildListResponse.ChildList != null && getChildListResponse.ChildList.Count(ch => ch.Agent == request.Agent) > 0)
                    isChild = true;
                */


                if (isChild)
                {
                    logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[ChangeBranchStatusProvider] [SEND-DATA] changeBranchStatusParameters {agent=" + request.Agent + "}");

                    //Ariel 2021-Ma-09 Comentado asignamos true
                    var result = true; //Utils.ChangeBranchStatus(request.Agent,request.Cascade, request.AuthenticationData.Username);
                    logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[ChangeBranchStatusProvider] [RECV-DATA] changeBranchStatusResult {response={" + result + "}}");

                    if (result){

                       
                         response = new ChangeBranchStatusResponseBody
                         {
                             ResponseCode = 0,
                             ResponseMessage = "Exito",
                             TransactionID = new Random().Next(10000000, 99999999)
                         };
                    }
                      
                    else
                        response = new ChangeBranchStatusResponseBody
                        {
                            ResponseCode = 99,
                            ResponseMessage = "El agente no existe"
                        };


                    //registrar en auditoria
                   
                   
                }
                else
                {
                    response = new ChangeBranchStatusResponseBody
                    {
                        ResponseCode = 99,
                        ResponseMessage = "El agente no puede ser modificado"
                    };
                }

                return (response);
            }
            catch (Exception ex)
            {
                return new ChangeBranchStatusResponseBody()
                {
                    ResponseCode = 500,
                    ResponseMessage = string.Concat("ERROR INESPERADO ",ex.Message),
                    TransactionID = 0
                };
            }
        }
    }
}