using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using Movilway.API.KinacuWebService;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net;
using System.Diagnostics;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.Logging;
using Movilway.API.Data;
using System.Configuration;
using System.Reflection;
using Movilway.API.Core.Security;
using Movilway.API.KinacuManagementWebService;
using Movilway.API.Core;
using Movilway.Cache;
using Movilway.API.Service.ExtendedApi.Security;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    public abstract class AKinacuProvider : IServiceProvider, ISecureServiceProvider
    {
        private const int MAX_SEED_REQUEST = 10000;
        private static volatile int Count = 1;

        private static int SeedRequest
        {
            get { return Count + MAX_SEED_REQUEST; }
        }


        private static void AddRequest()
        {
            Count = Count + 1 % MAX_SEED_REQUEST;

        }
        private const string CACHE_REQUEST_NAME = "REQUEST";

        private static ICache _cacheRequest = Cache.CacheFactory.FactoryCache(CACHE_REQUEST_NAME);
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(AKinacuProvider));
        protected abstract ILogger ProviderLogger { get; }
        protected virtual TransactionType TransactionType
        {
            get { throw new NotImplementedException("The property <TransactionType> must be implemented for providers for Financial Transactions"); }
        }
        protected string LOG_PREFIX { get; set; }






        private ICache _cache = null;


        protected Func<IMovilwayApiRequest, SaleInterface, String, IMovilwayApiResponse> _delegateSecondExecution;



        public AKinacuProvider()
        {
            _cache = Cache.CacheFactory.GetSingleInstaceCacheSession;
        }

        /// <summary>
        /// Inicializa el prefix del provider si y solo si la variable no esta inicializada
        /// </summary>
        private void InitPrefix(IMovilwayApiRequest request = null)
        {
            // PREFIX
            if (String.IsNullOrEmpty(LOG_PREFIX))
            {
                try
                {
                    int bandera = 3;
                    //se se toma el hascode de la peticion se repetiria en teoria el codigo para las mismas peticiones al mismo tiempo
                    try
                    {
                        bandera = HttpContext.Current.Session.SessionID.GetHashCode();
                    }
                    catch (Exception ex) {
                        logger.InfoLow(String.Concat("[API] ", LOG_PREFIX , "[KinacuProvider] [INPUT] ERROR SESSION ID ",ex.GetType().Name));
                    }//request != null && request.AuthenticationData != null ? request.AuthenticationData.GetHashCode() : 3;


                    LOG_PREFIX =String.Concat( HttpContext.Current.Session["LOG_PREFIX"].ToString() , "[" , new Random(DateTime.Now.Millisecond * bandera * SeedRequest).Next(100000000, 999999999) , "] ");
                   // AddRequest();
                }
                catch (Exception)
                {
                    LOG_PREFIX = "";
                }
            }
        }

        /// <summary>
        /// Imprime los datos de autenticacion del log
        /// </summary>
        /// <param name="request"></param>
        private void PrintLoginValues(IMovilwayApiRequest request)
        {
            try
            {
                logger.InfoLow("[API] " + LOG_PREFIX + "[KinacuProvider] [INPUT] UserInfo {IP=" + HttpContext.Current.Request.UserHostAddress + ",Username=" + request.AuthenticationData.Username + "} " + request.ToString());
            }
            catch (Exception)
            {
                logger.InfoLow("[API] " + LOG_PREFIX + "[KinacuProvider] [INPUT] UserInfo {IP=" + "NULL" + ",Username=" + request.AuthenticationData.Username + "} " + request.ToString());
            }
        }

        /// <summary>
        /// Validacion privada
        /// </summary>
        /// <param name="request"></param>
        private void SecureValidation(IMovilwayApiRequest request)
        {



            switch (request.DeviceType)
            {

                case cons.ACCESS_H2H:
                    if (!NetWorkSecurity.IpIsWithinRangeH2H(HttpContext.Current.Request.UserHostAddress))
                    {
                        string message = String.Concat("[LA IP NO ESTA REGISTRADA EN EL RANGO PERMITIDO H2H (", HttpContext.Current.Request.UserHostAddress, ")]");
                        logger.ErrorHigh(() => TagValue.New().Message("[ERROR DE PERMISOS H2H]").Message(message).Tag("[PARA ACCESO]").Value(request.AuthenticationData));
                        throw new Exception(message);
                    }

                    break;

            }





        }


        /// <summary>
        /// Ejecuta la operacion validando los constrains de seguridad
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IMovilwayApiResponse PerformSecureOperation(IMovilwayApiRequest request)
        {

            IMovilwayApiResponse response = null;
            //DATOS DE AUTENTICACION
            try
            {
                InitPrefix();

                PrintLoginValues(request);



                //Validaciones de seguridad teniendo en cuenta los datos de la peticion
                SecureValidation(request);



                SecureAccessValidation(request);

                //
                String sessionID = null;
                if (!(request is GetSessionRequestBody))
                    sessionID = GetSessionID(request);


                SaleInterface kinacuWS = new SaleInterface();
                //
                // CONDIGURACION INTERFAZ
                int timeOutSeconds = int.Parse(ConfigurationManager.AppSettings["DefaultTimeout"]);
                kinacuWS.Timeout = timeOutSeconds * 1000;


                response = PerformKinacuOperation(request, kinacuWS, sessionID);
                #region secondrequest
                //if (!ValidateNumberOfExecution(request))
                //    response = PerformKinacuOperation(request, kinacuWS, sessionID);
                //else
                //{


                //        if (_delegateSecondExecution == null)
                //            throw new Exception("NO SE HA DEFINIDO EL MANEJADOR PARA LA SEGUNDA PETICION");

                //        bool isNew = false;

                //        Func<int> callback = delegate()
                //        {
                //            logger.InfoLow("[API] " + LOG_PREFIX + "[KinacuProvider] [DOUBLE REQUEST] [CALL BACK] [IS NEW]");
                //            isNew = true;
                //            return 1;
                //        };

                //        int hascode = request.GetHashCode();
                //        logger.InfoLow("[API] " + LOG_PREFIX + "[KinacuProvider] [DOUBLE REQUEST VALIDATION] [" + hascode + "] "+_cacheRequest.ToString());
                //    //Action<object,object> oncache= delegate(object k , object v)
                //    //    {
                //    //        logger.InfoLow("[API] " + LOG_PREFIX + "[KinacuProvider] [DOUBLE REQUEST] [CALL BACK] [IS DOUBLE]");
                //    //        isNew = false;
                //    //    };


                //         _cacheRequest.GetValue<int>(hascode, callback);//, oncache);




                //        if (isNew)
                //        {
                //               response = PerformKinacuOperation(request, kinacuWS, sessionID);
                //        }
                //        else
                //        {
                //            logger.InfoLow("[API] " + LOG_PREFIX + "[KinacuProvider] [SecondExecution]");
                //            response = _delegateSecondExecution (request, kinacuWS, sessionID);
                //        }   



                //}
                #endregion


                if (bool.Parse(ConfigurationManager.AppSettings["LogoffAuto"] ?? "true"))
                {
                    logger.InfoLow("[KIN] " + this.LOG_PREFIX + "[LogOffProvider] [SEND-DATA] logoffParameters {userid=" + sessionID + "}");
                    kinacuWS.LogOff(int.Parse(sessionID));
                    logger.InfoLow("[KIN] " + this.LOG_PREFIX + "[LogOffProvider] [RECV-DATA] logoffResult {nothing}");
                }

                logger.InfoLow("[API] " + LOG_PREFIX + "[KinacuProvider] [OUTPUT] " + response.ToString());
            }
            catch (Exception e)
            {

                logger.ErrorLow("[API] " + LOG_PREFIX + "[KinacuProvider] [EXCEPTION] Exception trying to serve KINACU Operation {message=" + e.Message + ",stackTrace=" + e.StackTrace + "}");

                //SE DELEGA EL MANEJO DE LA EXPECION A QUIEN LO INVOCA GENERALMENTE SERVICEEXECUTIONDELEGATOR
                throw;

                #region intento de respuesta anterior

                //Type a = this.GetType().GetMethod("PerformOperation").ReturnType;
                //response = new AGenericApiResponse() { ResponseCode = 99, ResponseMessage = e.Message, TransactionID = 0 };
                //Type name = MethodBase.GetCurrentMethod().GetType();
                //ProviderLogger.ExceptionLow(() => TagValue.New().Message("Exception trying to serve KINACU Operation").Exception(e));
                #endregion
            }

            return response;
        }

        private void SecureAccessValidation(IMovilwayApiRequest request)
        {
            //pasar a un metodo estatico desacoplar mejor
            if (ApiConfiguration.IS_AVAILABLE_SECURITY)
                if (request is ASecuredFinancialApiRequest)
                {

                    //SecureAccessValidator validator = new SecureAccessValidator(this.GetType().Name);

                    //validator.IsValidAccess(request);



                }
        }

        /// <summary>
        /// Inicia la ejecucion del metodo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IMovilwayApiResponse PerformOperation(IMovilwayApiRequest request)
        {
            IMovilwayApiResponse response = null;


            //DATOS DE AUTENTICACION
            try
            {
                #region antes
                // PREFIX
                //try
                //{
                //    LOG_PREFIX = HttpContext.Current.Session["LOG_PREFIX"].ToString() + "[" + new Random(DateTime.Now.Millisecond * 3).Next(100000000, 999999999) + "] ";
                //} 
                //catch (Exception)
                //{
                //    LOG_PREFIX = "";
                //}
                #endregion
                InitPrefix();

                #region log data
                //try 
                //{
                //    logger.InfoLow("[API] " + LOG_PREFIX + "[KinacuProvider] [INPUT] UserInfo {IP=" + HttpContext.Current.Request.UserHostAddress + ",Username=" + request.AuthenticationData.Username + "} " + request.ToString());
                //}
                //catch (Exception)
                //{
                //    logger.InfoLow("[API] " + LOG_PREFIX + "[KinacuProvider] [INPUT] UserInfo {IP=" + "NULL" + ",Username=" + request.AuthenticationData.Username + "} " + request.ToString());
                //}
                #endregion
                PrintLoginValues(request);

                #region antes
                //if (request.DeviceType == cons.ACCESS_H2H)
                //{
                //    if (!NetWorkSecurity.IpIsWithinRangeH2H(HttpContext.Current.Request.UserHostAddress))
                //    {


                //        string message = String.Concat("[LA IP NO ESTA REGISTRADA EN EL RANGO PERMITIDO H2H (", HttpContext.Current.Request.UserHostAddress, ")]");
                //        logger.ErrorHigh(() => TagValue.New().Message("[ERROR DE PERMISOS H2H]").Message(message).Tag("[PARA ACCESO]").Value(request.AuthenticationData));
                //        throw new Exception(message);
                //    }
                //}
                #endregion
                //si no se activa la optimizacion de seguridad 
                //siempre ejecuta las validaciones de seguridad
                //if (!ApiConfiguration.API_SECURE_OPTIMIZATION)
                    SecureValidation(request);


                //la restriccion se mantiene con el fin  de que se pueda volver a validar el tiempo del session ID
                String sessionID = null;
                if (!(request is GetSessionRequestBody))
                    sessionID = GetSessionID(request);


                SaleInterface kinacuWS = new SaleInterface();
                // CONDIGURACION INTERFAZ
                int timeOutSeconds = int.Parse(ConfigurationManager.AppSettings["DefaultTimeout"]);
                kinacuWS.Timeout = timeOutSeconds * 1000;

                response = PerformKinacuOperation(request, kinacuWS, sessionID);
                // no deberia lanzar second request deberia solo se responsabilidad 
                #region secondrequest
                //if (!ValidateNumberOfExecution(request))
                //    response = PerformKinacuOperation(request, kinacuWS, sessionID);
                //else
                //{


                //        if (_delegateSecondExecution == null)
                //            throw new Exception("NO SE HA DEFINIDO EL MANEJADOR PARA LA SEGUNDA PETICION");

                //        bool isNew = false;

                //        Func<int> callback = delegate()
                //        {
                //            logger.InfoLow("[API] " + LOG_PREFIX + "[KinacuProvider] [DOUBLE REQUEST] [CALL BACK] [IS NEW]");
                //            isNew = true;
                //            return 1;
                //        };

                //        int hascode = request.GetHashCode();
                //        logger.InfoLow("[API] " + LOG_PREFIX + "[KinacuProvider] [DOUBLE REQUEST VALIDATION] [" + hascode + "] "+_cacheRequest.ToString());
                //    //Action<object,object> oncache= delegate(object k , object v)
                //    //    {
                //    //        logger.InfoLow("[API] " + LOG_PREFIX + "[KinacuProvider] [DOUBLE REQUEST] [CALL BACK] [IS DOUBLE]");
                //    //        isNew = false;
                //    //    };


                //         _cacheRequest.GetValue<int>(hascode, callback);//, oncache);




                //        if (isNew)
                //        {
                //               response = PerformKinacuOperation(request, kinacuWS, sessionID);
                //        }
                //        else
                //        {
                //            logger.InfoLow("[API] " + LOG_PREFIX + "[KinacuProvider] [SecondExecution]");
                //            response = _delegateSecondExecution (request, kinacuWS, sessionID);
                //        }   



                //}
                #endregion

                if (!(request is GetSessionRequestBody))
                {
                    if (bool.Parse(ConfigurationManager.AppSettings["LogoffAuto"] ?? "true"))
                    {
                        logger.InfoLow("[KIN] " + this.LOG_PREFIX + "[LogOffProvider] [SEND-DATA] logoffParameters {userid=" + sessionID + "}");
                        kinacuWS.LogOff(int.Parse(sessionID));
                        logger.InfoLow("[KIN] " + this.LOG_PREFIX + "[LogOffProvider] [RECV-DATA] logoffResult {nothing}");
                    }
                }
                logger.InfoLow("[API] " + LOG_PREFIX + "[KinacuProvider] [OUTPUT] " + response.ToString());
            }
            catch (Exception e)
            {

                logger.ErrorLow("[API] " + LOG_PREFIX + "[KinacuProvider] [EXCEPTION] Exception trying to serve KINACU Operation {message=" + e.Message + ",stackTrace=" + e.StackTrace + "}");
                //SE DELEGA EL MANEJO DE LA EXPECION A QUIEN LO INVOCA GENERALMENTE SERVICEEXECUTIONDELEGATOR
                throw;

                #region intento de respuesta anterior
                //Type a = this.GetType().GetMethod("PerformOperation").ReturnType;
                //response = new AGenericApiResponse() { ResponseCode = 99, ResponseMessage = e.Message, TransactionID = 0 };
                //Type name = MethodBase.GetCurrentMethod().GetType();
                //ProviderLogger.ExceptionLow(() => TagValue.New().Message("Exception trying to serve KINACU Operation").Exception(e));
                #endregion
            }

            return response;
        }
        protected virtual bool ValidateNumberOfExecution(IMovilwayApiRequest request)
        {
            return false;
        }
        private String GetSessionID(IMovilwayApiRequest request)
        {


            if (request is TopUpRequestBody)
            {


                try
                {


                    //No hacer resolveRquestsi no ResoveRquestScure
                    //TODO CACHE NO VALIDO NECESITO CACHE POR SESSION 
                    Func<string> callback = delegate()
                    {

                        //logger.InfoLow(String.Concat("[API] ", LOG_PREFIX, " EJECUTANDO CALLBACK DE CACHE ", request.AuthenticationData.Username));
                        logger.InfoLow(String.Concat("[API] ", LOG_PREFIX, " Session Not Found in Cache ", request.AuthenticationData.Username));


                        //try
                        //{
                        //    var copyrequest = Reflection.FactoryObject<ASecuredApiRequest>(request.GetType());
                        //    copyrequest.AuthenticationData = new AuthenticationData();
                        //    copyrequest.AuthenticationData.Username = request.AuthenticationData.Username;
                        //    copyrequest.AuthenticationData.Password = request.AuthenticationData.Password;
                        //    copyrequest.AuthenticationData.SessionID = request.AuthenticationData.SessionID;
                        //}
                        //catch (Exception ex)
                        //{

                        //}

                        return GetSessionIDEL(request);

                    };

                    Action<Object, Object> accion = delegate(Object key2, Object val)
                    {
                        logger.InfoLow(String.Concat("[API] ", LOG_PREFIX, " Session Found in Cache [", key2, "]"));
                    };



                    String llave = String.Concat(request.AuthenticationData.Username, " ", request.AuthenticationData.Password);//String.Concat(cons.CACHE_SESSION_PREFIX, request.AuthenticationData.Username);  //String.Concat(request.AuthenticationData.Username," " ,request.AuthenticationData.Password);// "llaveunica";//debeser el tokken
                    //HttpContext.Current.Session.SessionID;

                    string ID = _cache.GetValue<String>(llave, callback, accion);
                    return ID;

                }
                catch (Exception ex)
                {
                    //DESACTIVAR CACHE
                    logger.ErrorHigh(String.Concat("[API] ", LOG_PREFIX, "[KinacuProvider] [EXCEPTION] [", ex.GetType().Name.ToUpper(), "] ACCEDIENDO AL CACHE VALIDANDO SECURITY {message=", ex.Message, ",stackTrace=", ex.StackTrace, "}"));
                    throw;
                }

            }
            else
                return GetSessionIDEL(request, LOG_PREFIX);


            //IMPLEMENTACION ANTIGUA
            #region Implementacion antigua
            //string username = request.AuthenticationData.Username, platform = request.Platform ?? ConfigurationManager.AppSettings["DefaultPlatform"];
            //var cacheObj = new LoginDataCache();

            //if (request != null)
            //{
            //    //ObjectCache cache = MemoryCache.Default;
            //    //var cacheKey = username + platform;
            //    //
            //    //ProviderLogger.InfoLow("*** Chequeo de cache ***");
            //    //foreach (var item in cache)
            //    //    ProviderLogger.InfoLow(item.Key + ": " + item.Value);
            //    //ProviderLogger.InfoLow("************************");

            //    if (!String.IsNullOrEmpty(request.AuthenticationData.SessionID))
            //    {
            //        int userId;
            //        long timeOut;
            //        string userName, userLastName, userAddress, message;
            //        ManagementInterface managementWS = new ManagementInterface();

            //        if (managementWS.GetUserInfo(int.Parse(request.AuthenticationData.SessionID), out userId, out userName, out userLastName, out userAddress, out timeOut, out message))
            //        {
            //            return (request.AuthenticationData.SessionID);
            //        }
            //        else
            //        {

            //            if (!string.IsNullOrEmpty(request.AuthenticationData.Username) &&
            //                !string.IsNullOrEmpty(request.AuthenticationData.Password))
            //            {
            //                var getSessionResponse = new ServiceExecutionDelegator
            //           <GetSessionResponseBody, GetSessionRequestBody>().ResolveRequest(
            //               new GetSessionRequestBody()
            //               {
            //                   Username = request.AuthenticationData.Username,
            //                   Password = request.AuthenticationData.Password,
            //                   DeviceType = request.DeviceType
            //               }, ApiTargetPlatform.Kinacu, ApiServiceName.GetSession);


            //                if (getSessionResponse.ResponseCode == 0 &&
            //                      !string.IsNullOrEmpty(getSessionResponse.SessionID) &&
            //                     !getSessionResponse.SessionID.Equals("0"))
            //                    return getSessionResponse.SessionID;
            //                else
            //                    throw new Exception(getSessionResponse.ResponseMessage);
            //            }


            //            //SI NO REALIZO RETURN ENTONCES LANZA LA EXCEPCION
            //            throw new Exception("DATOS DE AUTENTICACION INVALIDOS");

            //        }

            //    }
            //    else
            //    {
            //        //if (cache.Contains(cacheKey))
            //        //    cacheObj = (LoginDataCache)cache.Get(cacheKey);
            //        //else
            //        //{
            //        //TODO CACHE
            //        var getSessionResponse = new ServiceExecutionDelegator
            //            <GetSessionResponseBody, GetSessionRequestBody>().ResolveRequest(
            //                new GetSessionRequestBody()
            //                {
            //                    Username = request.AuthenticationData.Username,
            //                    Password = request.AuthenticationData.Password,
            //                    DeviceType = request.DeviceType
            //                }, ApiTargetPlatform.Kinacu, ApiServiceName.GetSession);




            //        if (getSessionResponse.ResponseCode == 0 &&
            //             !string.IsNullOrEmpty(getSessionResponse.SessionID) &&
            //            !getSessionResponse.SessionID.Equals("0"))
            //            return getSessionResponse.SessionID;
            //        else
            //            throw new Exception(getSessionResponse.ResponseMessage);


            //        //SI NO REALIZO RETURN ENTONCES LANZA LA EXCEPCION
            //        throw new Exception("DATOS DE AUTENTICACION INVALIDOS");

            //        /*var newCacheObject = new LoginDataCache()
            //        {
            //            UserName = username,
            //            Platform = platform,
            //            Token = getSessionResponse.SessionID
            //        };

            //        // Store data in the cache
            //        var cacheMinutes = ConfigurationManager.AppSettings["UtibaSessionTTL"] ?? "10";
            //        var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddSeconds(Convert.ToInt32(cacheMinutes) * 1000) };
            //        //cache.Add(cacheKey, newCacheObject, cacheItemPolicy);
            //        ProviderLogger.InfoHigh("cacheKey: " + cacheKey + " - newCacheObject: {" + newCacheObject.UserName + "," + newCacheObject.Platform + "," + newCacheObject.Token + "} - cacheItemPolicy: " + cacheItemPolicy.AbsoluteExpiration);

            //        cacheObj = newCacheObject;*/
            //        //}
            //    }
            //}
            //return "error";
            #endregion
        }




        private static String GetSessionIDEL(IMovilwayApiRequest request, String LOG_PREFIX = null)
        {

            if (request != null)
            {
                //SE COMENTA LA VALIDACION DEL TOKEN DE KIANCU
                //var sessionRequest = new GetSessionRequestBody()
                //{
                //    Username = request.AuthenticationData.Username,
                //    Password = request.AuthenticationData.Password,
                //    DeviceType = request.DeviceType
                //};


                //if (!String.IsNullOrEmpty(request.AuthenticationData.SessionID))
                //{
                //    int userId;
                //    long timeOut;
                //    string userName, userLastName, userAddress, message;
                //    ManagementInterface managementWS = new ManagementInterface();

                //    //validacion de la session ID de Kinacu
                //    if (managementWS.GetUserInfo(int.Parse(request.AuthenticationData.SessionID), out userId, out userName, out userLastName, out userAddress, out timeOut, out message))
                //    {
                //        return (request.AuthenticationData.SessionID);
                //    }
                //    else
                //    {
                //        //solicita nueva mente un SessionId
                //        if (!string.IsNullOrEmpty(request.AuthenticationData.Username) &&
                //            !string.IsNullOrEmpty(request.AuthenticationData.Password))
                //        {
                //            var getSessionResponse = new ServiceExecutionDelegator
                //       <GetSessionResponseBody, GetSessionRequestBody>().ResolveRequest(
                //                //new GetSessionRequestBody()
                //                //    {
                //                //        Username = request.AuthenticationData.Username,
                //                //        Password = request.AuthenticationData.Password,
                //                //        DeviceType = request.DeviceType
                //                //    }
                //           sessionRequest
                //           , ApiTargetPlatform.Kinacu, ApiServiceName.GetSession);


                //            if (getSessionResponse.ResponseCode == 0 &&
                //                   !string.IsNullOrEmpty(getSessionResponse.SessionID) &&
                //                  !getSessionResponse.SessionID.Equals("0"))
                //                return getSessionResponse.SessionID;
                //            else
                //                throw new Exception(getSessionResponse.ResponseMessage);

                //        }



                //        throw new Exception("DATOS DE AUTENTICACION INVALIDOS");

                //    }

                //}
                //else
                //{

                //CONDICION INICIAL PARA VALIDAR CREDENCIALES
                if (String.IsNullOrEmpty(request.AuthenticationData.SessionID))
                {
                    var sessionRequest = new GetSessionRequestBody()
                    {
                        Username = request.AuthenticationData.Username,
                        Password = request.AuthenticationData.Password,
                        DeviceType = request.DeviceType
                    };

                    var getSessionResponse = new ServiceExecutionDelegator
                        <GetSessionResponseBody, GetSessionRequestBody>().ResolveRequest(
                        //new GetSessionRequestBody()
                        //    {
                        //        Username = request.AuthenticationData.Username,
                        //        Password = request.AuthenticationData.Password,
                        //        DeviceType = request.DeviceType
                        //    }
                           sessionRequest
                           , ApiTargetPlatform.Kinacu, ApiServiceName.GetSession);

                    if ((getSessionResponse.ResponseCode == 0 ||
                        getSessionResponse.ResponseCode == 1013) &&
                         !string.IsNullOrEmpty(getSessionResponse.SessionID) &&
                                        !getSessionResponse.SessionID.Equals("0"))
                        return getSessionResponse.SessionID;
                    else
                        throw new Exception(getSessionResponse.ResponseMessage);

                }
                else
                {
                    var _log_prefix = String.IsNullOrEmpty(LOG_PREFIX) ? "[LOG PREFIX DOESN'T HAVE VALUE]" : LOG_PREFIX;
                    logger.InfoLow(String.Concat("[API] ", _log_prefix, " SESSION ID IS DEFINED IN REQUEST"));
                    return request.AuthenticationData.SessionID;
                }

                //    }
            }
            return "error";


        }

        public abstract IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID);
    }
}