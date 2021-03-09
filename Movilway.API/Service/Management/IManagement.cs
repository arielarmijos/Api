using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Movilway.API.Service.Management.External;

namespace Movilway.API.Service.Management
{

    [ServiceContract(Namespace = "http://api.movilway.net")]
    public interface IManagement:ILoginServiceContract, ISessionServiceContract
    {
        [OperationContract]
        GetAgentByReferenceResponse GetAgentByReference(GetAgentByReferenceRequest externalRequest);
        [OperationContract]
        GetAgentByReferenceExtendedResponse GetAgentByReferenceExtended(GetAgentByReferenceExtendedRequest externalRequest);

        [OperationContract]
        GetParentListByReferenceIDResponse GetParentListByReferenceID(GetParentListByReferenceIDRequest externalRequest);
        [OperationContract]
        GetParentListByReferenceIDExtendedResponse GetParentListByReferenceIDExtended(GetParentListByReferenceIDExtendedRequest externalRequest);

        [OperationContract]
        GetChildListByReferenceResponse GetChildListByReference(GetChildListByReferenceRequest externalRequest);
        [OperationContract]
        GetChildListByReferenceExtendedResponse GetChildListByReferenceExtended(GetChildListByReferenceExtendedRequest externalRequest);

        [OperationContract]
        GetAgentGroupsResponse GetAgentGroups(GetAgentGroupsRequest externalRequest);
        [OperationContract]
        GetAgentGroupsExtendedResponse GetAgentGroupsExtended(GetAgentGroupsExtendedRequest externalRequest);

        [OperationContract]
        GetProductsResponse GetProducts(GetProductsRequest externalRequest);
        [OperationContract]
        GetProductsExtendedResponse GetProductsExtended(GetProductsExtendedRequest externalRequest);

        [OperationContract]
        GetAgentListResponse GetAgentList(GetAgentListRequest externalRequest);
        [OperationContract]
        GetAgentListExtendedResponse GetAgentListExtended(GetAgentListExtendedRequest externalRequest);

        [OperationContract]
        GetAllAgentGroupsResponse GetAllAgentGroups(GetAllAgentGroupsRequest externalRequest);
        [OperationContract]
        GetAllAgentGroupsExtendedResponse GetAllAgentGroupsExtended(GetAllAgentGroupsExtendedRequest externalRequest);

        [OperationContract]
        MapAgentToGroupResponse MapAgentToGroup(MapAgentToGroupRequest externalRequest);
        [OperationContract]
        MapAgentToGroupExtendedResponse MapAgentToGroupExtended(MapAgentToGroupExtendedRequest externalRequest);

        [OperationContract]
        UnMapAgentToGroupResponse UnMapAgentToGroup(UnMapAgentToGroupRequest externalRequest);
        [OperationContract]
        UnMapAgentToGroupExtendedResponse UnMapAgentToGroupExtended(UnMapAgentToGroupExtendedRequest externalRequest);

    }
}
