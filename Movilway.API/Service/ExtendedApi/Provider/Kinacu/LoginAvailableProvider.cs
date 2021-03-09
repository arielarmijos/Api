using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;


namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.LoginAvailable)]
    public class LoginAvailableProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(LoginAvailableProvider));
        protected override ILogger ProviderLogger { get { return logger; } }


        public override DataContract.Common.IMovilwayApiResponse PerformKinacuOperation(DataContract.Common.IMovilwayApiRequest requestObject, KinacuWebService.SaleInterface kinacuWS, string sessionID)
        {
            LoginAvailableResponseBody response = new LoginAvailableResponseBody();
            if (sessionID.Equals("0"))
            {

                response.ResponseCode = 90;
                response.ResponseMessage = "SESIÓN INVALIDA";
                return response;
            }


            LoginAvailableRequestBody request = requestObject as LoginAvailableRequestBody;

            bool result = false;

            try
            {
                if (request.AgenteId.HasValue && request.AgenteId.Value > -1m)
                    result = Utils.LoginDispobible(request.Login, request.AgenteId.Value);
                else
                    result = Utils.LoginDispobible(request.Login);

                if (result)
                {
                    response.ResponseCode = 0;
                    response.ResponseMessage = "LOGIN ESTA DISPONIBLE";
                }
                else
                {
                    response.ResponseCode = 1;
                    response.ResponseMessage = "LOGIN NO ESTA DISPONIBLE";
                }


            }
            catch (Exception ex)
            {

                response.ResponseCode = 2;
                response.ResponseMessage = String.Concat("ERROR INESPERADO. Exception:", ex.Message, " - ", ex.StackTrace);
            }


            return response;

        }
    }
}