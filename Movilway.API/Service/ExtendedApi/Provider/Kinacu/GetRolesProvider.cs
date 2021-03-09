using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using Movilway.API.KinacuWebService;
using Movilway.API.KinacuLogisticsWebService;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetRoles)]
    public class GetRolesProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetRolesProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new GetRolesResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0,
                    RolList = new RolList()
                };

            GetRolesRequestBody request = requestObject as GetRolesRequestBody;
            GetRolesResponseBody response = null;

            LogisticsInterface logisticsWS = new LogisticsInterface();

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[GetRolesProvider] [SEND-DATA] getRolesParameters {agentReference=" + request.Agent + "}");

            response = new GetRolesResponseBody()
            {
                ResponseCode = 0,
                ResponseMessage = "OK",
                RolList = Utils.GetRoles(request.Agent),
                TransactionID = 0
            };

            StringBuilder sb = new StringBuilder();
            foreach (Rol rol in response.RolList)
                sb.Append("Rol={RolId=" + rol.RolId + ",RolName=" + rol.RolName + "},");
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[GetRolesProvider] [RECV-DATA] getRolesResult {response={" + sb.ToString() + "}}");

            return (response);
        }
    }
}