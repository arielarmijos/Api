using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using Movilway.API.KinacuWebService;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using System.Text;
using System.Web.Script.Serialization;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform=ApiTargetPlatform.Kinacu, ServiceName=ApiServiceName.GetLoanToken)]
    public class GetLoanTokenProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetLoanTokenProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, string sessionID)
        {
            if (sessionID.Equals("0"))
                return new GetLoanTokenResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0
                };

            GetLoanTokenRequestBody request = requestObject as GetLoanTokenRequestBody;
            //string message;

            logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetLoanTokenProvider] [SEND-DATA] GetLoanTokenParameters {UserId=" + sessionID + "}");


            string id = Utils.GetAgentIdByAccessPosWeb(request.AuthenticationData.Username).ToString();

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage webResponse = new HttpResponseMessage();
            string data = "";
            Token token = new Token() { token = "nill" };

            logger.InfoLow(String.Concat("[MO] [SEND-DATA] agentId=", id));

            webResponse = client.PostAsync(String.Concat(ConfigurationManager.AppSettings["URL_MO"], "token"), new StringContent("{\"external_id\":" + id + "}", Encoding.UTF8, "application/json")).Result;

            logger.InfoLow(String.Concat("[MO] [RESPONSE-CODE] HttpStatusCode: ", (int)webResponse.StatusCode));

            JavaScriptSerializer JSserializer = new JavaScriptSerializer();

            GetLoanTokenResponseBody response;

            try
            {
                data = webResponse.Content.ReadAsStringAsync().Result;

                if (webResponse.IsSuccessStatusCode)
                {
                    token = JSserializer.Deserialize<Token>(data);

                    logger.InfoLow(String.Concat("[MO] [RESPONSE-DATA] Raw Data: ", data));
                    logger.InfoLow(String.Concat("[MO] [RESPONSE-DATA] token: ", token.token));

                    response = new GetLoanTokenResponseBody()
                    {
                        ResponseCode = 0,
                        ResponseMessage = "Exito",
                        TransactionID = new Random().Next(100000, 999999),
                        Token = token.token
                    };
                }
                else
                {
                    logger.InfoLow(String.Concat("[MO] [RESPONSE-DATA] ERROR Raw Data: ", data));
                    response = new GetLoanTokenResponseBody()
                    {
                        ResponseCode = 98,
                        ResponseMessage = "Error",
                        TransactionID = new Random().Next(100000, 999999),
                        Token = ""
                    };
                }
            }
            catch (Exception ex)
            {
                logger.ErrorLow(String.Concat(" [RECEIVED-DATA] Error recibiendo la data. ", ex.InnerException));
                response = new GetLoanTokenResponseBody()
                {
                    ResponseCode = 99,
                    ResponseMessage = "Excepcion",
                    TransactionID = new Random().Next(100000, 999999),
                    Token = ""
                };
            }

            //long balance = kinacuWS.GetAccountBalance(int.Parse(sessionID), out message);

            logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetLoanTokenProvider] [RECV-DATA] GetLoanTokenResult {response=Exito,message=ok,token=eyJjdXN0b21lcl9pZCI6MTU4fQ.DnrUTA.XHKdo08zgauO6JjwP-T2G6VjX6I}");


            return (response);
        }
    }

    public class Token
    {
        public string token { get; set; }
    }
}