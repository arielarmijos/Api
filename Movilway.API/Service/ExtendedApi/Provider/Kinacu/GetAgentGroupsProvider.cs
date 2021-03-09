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
   [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetAgentGroups)]
    public class GetAgentGroupsProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetAgentGroupsProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new GetAgentGroupsResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0,
                    GroupList = new GroupList()
                };

            GetAgentGroupsRequestBody request = requestObject as GetAgentGroupsRequestBody;
            GetAgentGroupsResponseBody response = null;

            ManagementInterface managementWS = new ManagementInterface();
         
           KinacuManagementWebService.PrivilegeInfo[] privileges;
            String message;

            bool result = managementWS.GetPrivileges(int.Parse(sessionID), null, out privileges, out message);

            if (result)
            {
                response = new GetAgentGroupsResponseBody()
                {
                    ResponseCode = 0,
                    ResponseMessage = "exito",
                    TransactionID = 0
                };

                if (privileges.Count() > 0)
                {
                    response.GroupList = new GroupList();
                    foreach (KinacuManagementWebService.PrivilegeInfo privilege in privileges)
                    {
                        response.GroupList.Add(new GroupInfo()
                        {
                            GroupID = long.Parse(privilege.Id.ToString()),
                            Name = privilege.Name,
                            Category = privilege.Type
                            //Type = privilege.Type
                        });
                    }
                }
            }
            else
            {
                return new GetAgentGroupsResponseBody()
                {
                    ResponseCode = 99,
                    ResponseMessage = message,
                    TransactionID = 0
                };
            }
            return (response);
        }
    }
}