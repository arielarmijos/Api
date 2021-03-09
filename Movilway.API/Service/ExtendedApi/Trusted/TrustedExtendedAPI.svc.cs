using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.Provider;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System.Reflection;
using System.Diagnostics;

namespace Movilway.API.Service.ExtendedApi.Trusted
{
    [ServiceBehavior(Namespace = "http://api.movilway.net/schema/extended",
                     ConcurrencyMode = ConcurrencyMode.Multiple,
                     InstanceContextMode = InstanceContextMode.Single)]
    public class TrustedExtendedAPI : Public.ExtendedAPI, ITrustedExtendedAPI
    {
        public BlockAgentResponse BlockAgent(BlockAgentRequest request)
        {
            return WrapResponse<BlockAgentResponse, BlockAgentResponseBody>(new ServiceExecutionDelegator<BlockAgentResponseBody, BlockAgentRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.BlockAgent));
        }

        public GetCityListResponse GetCityList(GetCityListRequest request)
        {
            return WrapResponse<GetCityListResponse, GetCityListResponseBody>(new ServiceExecutionDelegator<GetCityListResponseBody, GetCityListRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.GetCityList));
        }

        public GetProvinceListResponse GetProvinceList(GetProvinceListRequest request)
        {
            return WrapResponse<GetProvinceListResponse, GetProvinceListResponseBody>(new ServiceExecutionDelegator<GetProvinceListResponseBody, GetProvinceListRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.GetProvinceList));
        }


       

        public RegisterAgentResponse RegisterAgent(RegisterAgentRequest request)
        {
            return WrapResponse<RegisterAgentResponse, RegisterAgentResponseBody>(new ServiceExecutionDelegator<RegisterAgentResponseBody, RegisterAgentRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.RegisterAgent));
        }

        public RegisterAgentBulkResponse RegisterAgentBulk(RegisterAgentBulkRequest request)
        {
            return WrapResponse<RegisterAgentBulkResponse, RegisterAgentBulkResponseBody>(new ServiceExecutionDelegator<RegisterAgentBulkResponseBody, RegisterAgentBulkRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.RegisterAgentBulk));
        }

        public UnBlockAgentResponse UnBlockAgent(UnBlockAgentRequest request)
        {
            return WrapResponse<UnBlockAgentResponse, UnBlockAgentResponseBody>(new ServiceExecutionDelegator<UnBlockAgentResponseBody, UnBlockAgentRequestBody>().ResolveRequest(request.Request,
                ApiTargetPlatform.Utiba, ApiServiceName.UnBlockAgent));
        }


        // MOCKS:

        public CompleteBuyStockResponse CompleteBuyStock(CompleteBuyStockRequest request)
        {
            return new CompleteBuyStockResponse
                       {
                           Response = new CompleteBuyStockResponseBody
                                          {
                                              ResponseCode = 0,
                                              Fee = 0,
                                              ResponseMessage = "Mensaje de Respuesta",
                                              TransactionID = 1234567
                                          }
                       };
        }

        public ValidateDepositResponse ValidateDeposit(ValidateDepositRequest request)
        {
            return WrapResponse<ValidateDepositResponse, ValidateDepositResponseBody>(
              new ServiceExecutionDelegator<ValidateDepositResponseBody, ValidateDepositRequestBody>().ResolveRequest(request.Request, ApiTargetPlatform.Kinacu, ApiServiceName.ValidateDeposit));

            //return new ValidateDepositResponse
            //           {
            //               Response = new ValidateDepositResponseBody
            //                              {
            //                                  ResponseCode = 0,
            //                                  TransactionID = 1234321,
            //                                  ResponseMessage = "Mensaje de Respuesta"
            //                              }
            //           };
        }
    }
}
