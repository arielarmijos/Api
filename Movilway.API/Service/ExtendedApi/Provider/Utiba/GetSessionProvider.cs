using Movilway.Logging;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Utiba;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.GetSession)]
    public class GetSessionProvider:AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetSessionProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            GetSessionRequestBody request = requestObject as GetSessionRequestBody;
            GetSessionResponseBody response = null;

            createsessionResponse newSessionResponse = utibaClientProxy.createsession(new createsession());
            String PIN = Utils.GenerateHash(newSessionResponse.createsessionReturn.sessionid, request.Username, request.Password);

            logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetSessionProvider] [SEND-DATA] loginRequest {sessionid=" + newSessionResponse.createsessionReturn.sessionid + ",device_type=" + request.DeviceType + ",initiator=" + request.Username + ",pin=" + PIN + "}");

            loginResponse loginResponse = utibaClientProxy.login(new login()
            {
                loginRequest = new loginRequestType()
                    {
                        sessionid = newSessionResponse.createsessionReturn.sessionid,
                        device_type = request.DeviceType,
                        initiator = request.Username,
                        pin = PIN
                    }
            });

            logger.InfoLow("[UTI] " + base.LOG_PREFIX + "[GetSessionProvider] [RECV-DATA] loginResponse {result=" + loginResponse.loginReturn.result + ",result_namespace=" + loginResponse.loginReturn.result_namespace + "}");

            response = new GetSessionResponseBody()
            {
                ResponseCode = Utils.BuildResponseCode(loginResponse.loginReturn.result,loginResponse.loginReturn.result_namespace),
                ResponseMessage = loginResponse.loginReturn.result_message,
                TransactionID = loginResponse.loginReturn.transid,
                SessionID = newSessionResponse.createsessionReturn.sessionid
            };

            if (response.ResponseCode == 0)
            {

                if (ConfigurationManager.AppSettings["ProcessMigration"].ToLower() == "true")
                {
                    //logger.InfoHigh("Comienza la migración del usuario: " + request.AuthenticationData.Username);
                    bool migrateAgent = MigrateAgent(request.AuthenticationData.Username);
                    int deviceType = GetDeviceType(request.AuthenticationData.Username);
                    if (migrateAgent)
                    {
                        // Cambio de password Kinacu
                        var changePinResponse = new ServiceExecutionDelegator<ChangePinResponseBody, ChangePinRequestBody>().ResolveRequest(
                                                            new ChangePinRequestBody()
                                                            {
                                                                AuthenticationData = new AuthenticationData()
                                                                {
                                                                    Username = request.AuthenticationData.Username,
                                                                    Password = ConfigurationManager.AppSettings["StandardOldPin"]
                                                                },
                                                                DeviceType = deviceType, /*int.Parse(ConfigurationManager.AppSettings["StandardNewDeviceType"]),*/
                                                                Agent = request.AuthenticationData.Username,
                                                                OldPin = ConfigurationManager.AppSettings["StandardOldPin"],
                                                                NewPin = request.AuthenticationData.Password
                                                            }, ApiTargetPlatform.Kinacu, ApiServiceName.ChangePin);

                        // Login con Kinacu - NOT NOW - La proxima vez que entre va por Kinacu de una

                        //logger.InfoHigh("Resultado del cambio de clave del usuario: " + changePinResponse.ResponseCode);

                        // Save in DB
                        if (changePinResponse.ResponseCode == 0)
                        {
                            //logger.InfoHigh("Se migró exitosamente la clave del usuario: " + request.AuthenticationData.Username);
                            SaveAgentMigrated(request.AuthenticationData.Username);
                        }
                    }
                }

                // REPG2013 - esto queda deshabilitado por ahora

                // For speeding up, I'm caching the session value:
                //-var couchBaseClient = new CouchbaseClient();
                //-couchBaseClient.Store(StoreMode.Set, Utils.GetSessionForUserKey(request.Username, request.Password), response.SessionID, new TimeSpan(0, UtibaUtils.SessionTTL, 0));
                // Also caching the 
                //-couchBaseClient.Store(StoreMode.Set, Utils.GetUserForSessionKey(response.SessionID), request.Username, new TimeSpan(0, UtibaUtils.SessionTTL, 0));
            }
            return (response);
        }

        private bool MigrateAgent(String agentReference)
        {
            var sqlConnection = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["BASEConnectionString"].ConnectionString);
            string countryId = ConfigurationManager.AppSettings["CountryID"], platformId = ConfigurationManager.AppSettings["DefaultPlatform"];
            bool result = false;
            try
            {
                var query = String.Format("SELECT CASE count(1) WHEN 0 THEN 'False' WHEN 1 THEN 'True' END FROM [DefaultPlatform] where countryid={0} and platformid={1} and username='{2}' and wasmigrated is null", countryId, platformId, agentReference);

                sqlConnection.Open();

                var command = new System.Data.SqlClient.SqlCommand(query, sqlConnection);
                result = bool.Parse((string)command.ExecuteScalar());
            }
            catch (Exception)
            {
                result = false;
            }
            finally
            {
                sqlConnection.Close();
            }
            return result;
        }

        private int GetDeviceType(String agentReference)
        {
            var sqlConnection = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["BASEConnectionString"].ConnectionString);
            string countryId = ConfigurationManager.AppSettings["CountryID"], platformId = ConfigurationManager.AppSettings["DefaultPlatform"];
            int result = 0;
            try
            {
                var query = String.Format("SELECT DeviceType FROM [DefaultPlatform] where countryid={0} and platformid={1} and username='{2}' and wasmigrated is null", countryId, platformId, agentReference);

                sqlConnection.Open();

                var command = new System.Data.SqlClient.SqlCommand(query, sqlConnection);
                result = (int)command.ExecuteScalar();
            }
            catch (Exception)
            {
                result = -1;
            }
            finally
            {
                sqlConnection.Close();
            }
            return result;
        }

        private void SaveAgentMigrated(String agentReference)
        {
            var sqlConnection = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["BASEConnectionString"].ConnectionString);
            string countryId = ConfigurationManager.AppSettings["CountryID"], platformId = ConfigurationManager.AppSettings["DefaultPlatform"];

            try
            {
                var query = String.Format("update [DefaultPlatform] set wasmigrated=1,newplatformid=1,datemigrated=getdate() where countryid={0} and platformid={1} and username='{2}' and wasmigrated is null", countryId, platformId, agentReference);

                sqlConnection.Open();

                var command = new System.Data.SqlClient.SqlCommand(query, sqlConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}