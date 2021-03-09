using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Movilway.API.Service.ExtendedApi.DataContract;

namespace Movilway.API.Service.ExtendedApi.Trusted
{
    [ServiceContract(Namespace = "http://api.movilway.net/schema/extended")]
    public interface ITrustedExtendedAPI : Public.IExtendedAPI
    {
        [OperationContract(Name = "BlockAgent")]
        BlockAgentResponse BlockAgent(BlockAgentRequest request);

        [OperationContract(Name = "CompleteBuyStock")]
        CompleteBuyStockResponse CompleteBuyStock(CompleteBuyStockRequest request);

        [OperationContract(Name = "GetProviceList")]
        GetProvinceListResponse GetProvinceList(GetProvinceListRequest request);

        [OperationContract(Name = "GetCityList")]
        GetCityListResponse GetCityList(GetCityListRequest request);

        [OperationContract(Name = "RegisterAgent")]
        RegisterAgentResponse RegisterAgent(RegisterAgentRequest request);

        [OperationContract(Name = "RegisterAgentBulk")]
        RegisterAgentBulkResponse RegisterAgentBulk(RegisterAgentBulkRequest request);

        [OperationContract(Name = "UnBlockAgent")]
        UnBlockAgentResponse UnBlockAgent(UnBlockAgentRequest request);

        [OperationContract(Name = "ValidateDeposit")]
        ValidateDepositResponse ValidateDeposit(ValidateDepositRequest request);
    }
}
