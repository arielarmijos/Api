using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System.Configuration;
using Movilway.API.Core;
using Movilway.Logging;
using Movilway.API.Core.Security;


namespace Movilway.API.Service.ExtendedApi.Provider
{


    public class ServiceExecutionDelegator<K, T>
        where T : IMovilwayApiRequest
        where K : IMovilwayApiResponse
    {

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(Object));

        //Inicializa el componente y lo ejecuta
        public K ResolveRequest(T request, ApiTargetPlatform targetPlatform, ApiServiceName serviceName)
        {
            //bandera para determinar si hubo error de la clase
            bool opok = false;
            try
            {


                if (ConfigurationManager.AppSettings["ProcessMigration"].ToLower() == "true")
                    if (request.AuthenticationData != null)
                        if (GetCustomDefaultPlatform(request.AuthenticationData.Username) == "1")
                            targetPlatform = ApiTargetPlatform.Kinacu;

                if (HttpContext.Current.Session != null)
                    if (HttpContext.Current.Session["LOG_PREFIX"] == null)
                        HttpContext.Current.Session.Add("LOG_PREFIX", "[" + new Random(DateTime.Now.Millisecond * 9).Next(100000000, 999999999) + "] [" + serviceName.ToString() + "] ");

                IServiceProvider serviceImpl = ServiceProviderFactory.GetServiceProvider(targetPlatform, serviceName);

                //se pudo ejecutar hasta este punto el codigo
                opok = true;

                return ((K)serviceImpl.PerformOperation(request));
            }
            catch (Exception ex)
            {
                K resultError = Reflection.FactoryObject<K>();
                resultError.ResponseCode = UtilResut.ResponseCode(ex);

                if (ex.Message.IndexOf("[API UNEXCEPTED ERROR]", StringComparison.Ordinal) < 0)
                    resultError.ResponseMessage = String.Concat("[API UNEXCEPTED ERROR]-[", ex.Message, "]");
                else
                    resultError.ResponseMessage = ex.Message;

                //si es un error interno del la clase se debe 
                if (!opok)
                    logger.Fatal(String.Concat(resultError.ResponseCode, "-", resultError.ResponseMessage, "-", ex.StackTrace), ex);

                return resultError;
            }
        }

        //Inicializa el componente y lo ejecuta
        public K ResolveRequest(T request, string platform, ApiServiceName serviceName)
        {
            //bandera para determinar si hubo error de la clase
            bool opok = false;
            try
            {

                //string defaultPlatform = ConfigurationManager.AppSettings["DefaultPlatform"];

                if (ConfigurationManager.AppSettings["ProcessMigration"].ToLower() == "true")
                {
                    string customDefault = GetCustomDefaultPlatform(request.AuthenticationData.Username);
                    if (customDefault != "0") platform = customDefault;
                }

                platform = (platform.ToLower().Equals("kinacu") ? "1" : (platform.ToLower().Equals("utiba") ? "2" : platform.ToLower()));

                if (HttpContext.Current.Session != null)
                    if (HttpContext.Current.Session["LOG_PREFIX"] == null)
                        HttpContext.Current.Session.Add("LOG_PREFIX", "[" + new Random(DateTime.Now.Millisecond * 9).Next(100000000, 999999999) + "] [" + serviceName.ToString() + "] ");

                IServiceProvider serviceImpl;
                if (platform.Equals("1"))
                    serviceImpl = ServiceProviderFactory.GetServiceProvider(ApiTargetPlatform.Kinacu, serviceName);
                else if (platform.Equals("2"))
                    serviceImpl = ServiceProviderFactory.GetServiceProvider(ApiTargetPlatform.Utiba, serviceName);
                else
                    throw new Exception("Problemas identificando el platform");
                //else
                //{
                //    if ((defaultPlatform ?? "0") == "2")
                //        serviceImpl = ServiceProviderFactory.GetServiceProvider(ApiTargetPlatform.Kinacu, serviceName);
                //    else
                //        serviceImpl = ServiceProviderFactory.GetServiceProvider(ApiTargetPlatform.Utiba, serviceName);
                //}

                //se pudo ejecutar hasta este punto el codigo
                opok = true;
                return ((K)serviceImpl.PerformOperation(request));
            }
            catch (Exception ex)
            {
                K resultError = Reflection.FactoryObject<K>();

                resultError.ResponseCode = UtilResut.ResponseCode(ex);
                if (ex.Message.IndexOf("[API UNEXCEPTED ERROR]", StringComparison.Ordinal) < 0)
                    resultError.ResponseMessage = String.Concat("[API UNEXCEPTED ERROR]-[", ex.Message, "]");
                else
                    resultError.ResponseMessage = ex.Message;

                if (!opok)
                    logger.Fatal(String.Concat(resultError.ResponseCode, "-", resultError.ResponseMessage, "-", ex.StackTrace), ex);

                return resultError;
            }
        }

        //Crea el componente que implmente una ejecucion segura y la invoca
        private K ResolveSecureRequest(T request, ApiTargetPlatform targetPlatform, ApiServiceName serviceName)
        {
            //bandera para determinar si hubo error de la clase
            bool opok = false;
            try
            {


                if (ConfigurationManager.AppSettings["ProcessMigration"].ToLower() == "true")
                    if (request.AuthenticationData != null)
                        if (GetCustomDefaultPlatform(request.AuthenticationData.Username) == "1")
                            targetPlatform = ApiTargetPlatform.Kinacu;

                if (HttpContext.Current.Session != null)
                    if (HttpContext.Current.Session["LOG_PREFIX"] == null)
                        HttpContext.Current.Session.Add("LOG_PREFIX", "[" + new Random(DateTime.Now.Millisecond * 9).Next(100000000, 999999999) + "] [" + serviceName.ToString() + "] ");

                ISecureServiceProvider serviceImpl = ServiceProviderFactory.GetServiceProviderSecure(targetPlatform, serviceName);

                //se pudo ejecutar hasta este punto el codigo
                opok = true;

                return ((K)serviceImpl.PerformSecureOperation(request));
            }

            catch (Exception ex)
            {
                K resultError = Reflection.FactoryObject<K>();
                resultError.ResponseCode = UtilResut.ResponseCode(ex);

                if (ex.Message.IndexOf("[API UNEXCEPTED ERROR]", StringComparison.Ordinal) < 0)
                    resultError.ResponseMessage = String.Concat("[API UNEXCEPTED ERROR]-[", ex.Message, "]");
                else
                    resultError.ResponseMessage = ex.Message;

                //si es un error interno del la clase se debe 
                if (!opok)
                    logger.Fatal(String.Concat(resultError.ResponseCode, "-", resultError.ResponseMessage, "-", ex.StackTrace), ex);

                return resultError;
            }
        }

        //Crea el componente que implmente una ejecucion segura y la invoca
        private K ResolveSecureRequest(T request, string platform, ApiServiceName serviceName)
        {
            //bandera para determinar si hubo error de la clase
            bool opok = false;
            try
            {

                //string defaultPlatform = ConfigurationManager.AppSettings["DefaultPlatform"];

                if (ConfigurationManager.AppSettings["ProcessMigration"].ToLower() == "true")
                {
                    string customDefault = GetCustomDefaultPlatform(request.AuthenticationData.Username);
                    if (customDefault != "0") platform = customDefault;
                }

                platform = (platform.ToLower().Equals("kinacu") ? "1" : (platform.ToLower().Equals("utiba") ? "2" : platform.ToLower()));

                if (HttpContext.Current.Session != null)
                    if (HttpContext.Current.Session["LOG_PREFIX"] == null)
                        HttpContext.Current.Session.Add("LOG_PREFIX", "[" + new Random(DateTime.Now.Millisecond * 9).Next(100000000, 999999999) + "] [" + serviceName.ToString() + "] ");

                ISecureServiceProvider serviceImpl;
                if (platform.Equals("1"))
                    serviceImpl = ServiceProviderFactory.GetServiceProviderSecure(ApiTargetPlatform.Kinacu, serviceName);
                else if (platform.Equals("2"))
                    serviceImpl = ServiceProviderFactory.GetServiceProviderSecure(ApiTargetPlatform.Utiba, serviceName);
                else
                    throw new Exception("Problemas identificando el platform");
                //else
                //{
                //    if ((defaultPlatform ?? "0") == "2")
                //        serviceImpl = ServiceProviderFactory.GetServiceProvider(ApiTargetPlatform.Kinacu, serviceName);
                //    else
                //        serviceImpl = ServiceProviderFactory.GetServiceProvider(ApiTargetPlatform.Utiba, serviceName);
                //}

                //se pudo ejecutar hasta este punto el codigo
                opok = true;



                return ((K)serviceImpl.PerformSecureOperation(request));
            }
            catch (Exception ex)
            {
                K resultError = Reflection.FactoryObject<K>();

                resultError.ResponseCode = UtilResut.ResponseCode(ex);
                if (ex.Message.IndexOf("[API UNEXCEPTED ERROR]", StringComparison.Ordinal) < 0)
                    resultError.ResponseMessage = String.Concat("[API UNEXCEPTED ERROR]-[", ex.Message, "]");
                else
                    resultError.ResponseMessage = ex.Message;

                if (!opok)
                    logger.Fatal(String.Concat(resultError.ResponseCode, "-", resultError.ResponseMessage, "-", ex.StackTrace), ex);

                return resultError;
            }
        }

        /// <summary>
        /// Ejecuta la validacion del cokie monster ,  en caso de ser correcta
        /// continua con el despliegue del componente
        /// </summary>
        /// <param name="request"></param>
        /// <param name="platform"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public K ResolveRequestService(T request, string platform, ApiServiceName serviceName)
        {
            // llamar a la validacion de seguridad del api 


            //


            String cadena = "This is an object in session";
            HttpContext.Current.Session.Add("MYOBJECT", cadena);

            K result = default(K);


            if (ApiConfiguration.API_SECURE_OPTIMIZATION)
                result = ResolveSecureRequest(request, platform, serviceName);
            else
                result = ResolveRequest(request, platform, serviceName);

            HttpContext.Current.Session.Clear();

            return result;
        }

        /// <summary>
        /// Ejecuta la validacion del cokie monster ,  en caso de ser correcta
        /// continua con el despliegue del componente
        /// </summary>
        /// <param name="request"></param>
        /// <param name="targetPlatform"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public K ResolveRequestService(T request, ApiTargetPlatform targetPlatform, ApiServiceName serviceName)
        {
            // llamar a la validacion de seguridad del api 
            //
            if (ApiConfiguration.API_SECURE_OPTIMIZATION)
                return ResolveSecureRequest(request, targetPlatform, serviceName);
            else
                return ResolveRequest(request, targetPlatform, serviceName);
        }

        public string GetCustomDefaultPlatform(String agentReference)
        {
            var sqlConnection = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["BASEConnectionString"].ConnectionString);
            string countryId = ConfigurationManager.AppSettings["CountryID"], platformId = ConfigurationManager.AppSettings["DefaultPlatform"], result = "";
            try
            {
                var query = String.Format("SELECT cast(isnull(sum(newplatformid),0) as varchar) FROM [DefaultPlatform] where countryid={0} and platformid={1} and username='{2}' and wasmigrated=1 and ismigrationactive=1", countryId, platformId, agentReference);

                sqlConnection.Open();

                var command = new System.Data.SqlClient.SqlCommand(query, sqlConnection);
                result = (string)command.ExecuteScalar();
            }
            catch (Exception)
            {
                result = "0";
            }
            finally
            {
                sqlConnection.Close();
            }
            return result;
        }




    }
}