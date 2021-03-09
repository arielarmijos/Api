using Movilway.API.KinacuWebService;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.Security)]
    public class SecurityProvider: AKinacuProvider
    {

        private Logging.ILogger logger = Logging.LoggerFactory.GetLogger(typeof(SecurityProvider)); 
        protected override Logging.ILogger ProviderLogger
        {
            get {
                return logger;
            }
        }

        public override DataContract.Common.IMovilwayApiResponse PerformKinacuOperation(DataContract.Common.IMovilwayApiRequest requestObject, KinacuWebService.SaleInterface kinacuWS, string sessionID)
        {

            // throw new NotImplementedException();
             
             SecurityResponseBody response = new SecurityResponseBody() ;
            try 
            {

              response =   dummyKinacuImplementation(requestObject, kinacuWS, sessionID);
           
               
            }
            catch (Exception ex)
            {

                response.ResponseCode = 500;
                response.ResponseMessage = ex.Message;//_GenericError;
                response.TransactionID = 0;

                string mensaje = String.Concat("[API] " + base.LOG_PREFIX + "[UpdateAgentProvider] ", ". Exception: ", ex.Message, ". ", ex.StackTrace);
                logger.ErrorLow(mensaje);

            }
            return response;
        }

        private SecurityResponseBody dummyKinacuImplementation(IMovilwayApiRequest request, SaleInterface kinacuWS, string sessionID)
        {

            string result = "";



            logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetSessionProvider] [SEND-DATA] loginParameters {accessId=" + request.AuthenticationData.Username + ",password=******,accessType=" + request.DeviceType + "}");

            int newSessionResponse = kinacuWS.Login(request.AuthenticationData.Username, request.AuthenticationData.Password, request.DeviceType, out result);

            logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetSessionProvider] [RECV-DATA] loginResult {response=" + newSessionResponse + ",result=" + result + "}");

            SecurityResponseBody response = new SecurityResponseBody()
            {
                ResponseCode = newSessionResponse != 0 ? 0 : GetResponseCode(result),
                TransactionID = 0,
                SessionID = newSessionResponse.ToString()
            };

            if (newSessionResponse == 0)
                response.ResponseMessage = result;


            return response;

        }

        private int GetResponseCode(string result)
        {
            int responseCode = 99;

            if (result.Contains('-'))
                if (int.TryParse(result.Split('-')[0], out responseCode))
                {
                    //if (responseCode == 1013)
                    //responseCode = 10133;
                    if (responseCode == 1017)
                        responseCode = 1013;
                }
                else
                    responseCode = 99;

            return responseCode;
        }
    }
}