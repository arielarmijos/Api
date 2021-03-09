using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using Movilway.API.KinacuWebService;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetParentList)]
    public class GetParentListProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetParentListProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new GetParentListResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0,
                    ParentList = new ParentList()
                };

            GetParentListRequestBody request = requestObject as GetParentListRequestBody;
            GetParentListResponseBody response = null;

            response = new GetParentListResponseBody()
                {
                    ResponseCode = 0,
                    ResponseMessage = "exito",
                    TransactionID = 0
                };

            string parentAgentInfo = Kinacu.Utils.GetParentAgent(request.AuthenticationData.Username);

            response.ParentList = new ParentList();

            response.ParentList.Add(new BasicAgentInfo()
            {
                Agent = parentAgentInfo.Split('-')[0],
                Name = parentAgentInfo.Split('-')[1]
            });
            
            return (response);
        }
    }
}