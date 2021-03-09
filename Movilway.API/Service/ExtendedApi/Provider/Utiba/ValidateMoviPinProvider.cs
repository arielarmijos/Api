using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Movilway.API.Utiba;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
//using Oracle.DataAccess.Client;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.ValidateMoviPin)]
    public class ValidateMoviPinProvider : IServiceProvider
    {
        private static readonly ILogger Logger = LoggerFactory.GetLogger(typeof(ValidateMoviPinProvider));

        public IMovilwayApiResponse PerformOperation(IMovilwayApiRequest requestObject)
        {
            var request = requestObject as ValidateMoviPinRequestBody;

            Logger.BeginLow(() => TagValue.New().Tag("Request").Value(request));

            var response = new ValidateMoviPinResponseBody
            {
                ResponseCode = 0,
                ResponseMessage = "Sus Movipins han sido validados correctamente",
                MoviPins = new MoviPins()
            };

            if (request != null)
                foreach (var details in request.MoviPins.Select(movipin => Utils.GetMoviPinDetails(movipin.Number)).Where(details => details!=null))
                {
                    response.MoviPins.Add(details);
                }

            Logger.CheckPointLow(() => TagValue.New().Tag("Response").Value(response));

            return (response);
        }
    }
}