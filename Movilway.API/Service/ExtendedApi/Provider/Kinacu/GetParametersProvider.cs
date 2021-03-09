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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetParameters)]
    public class GetParametersProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetParametersProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {

            

            if (sessionID.Equals("0"))
                return new GetParametersResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0,
                    ParametersInfo = new ParameterList()
                };

            GetParametersRequestBody request = requestObject as GetParametersRequestBody;
            GetParametersResponseBody response = null;

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[GetParametersProvider] [SEND-DATA] getParametersParameters {}");

            ParameterList parameterList = Utils.GetParameters();

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[GetParametersProvider] [RECV-DATA] getParametersResult {response={" + DataContract.Utils.logFormat(parameterList) + "}}");

            response = new GetParametersResponseBody()
            {
                ResponseCode = 0,
                ResponseMessage = "exito",
                TransactionID = 0,
                ParametersInfo = parameterList
            };

            return (response);
        }
    }
}