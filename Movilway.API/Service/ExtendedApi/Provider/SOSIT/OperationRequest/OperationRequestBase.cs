using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract.SOSIT;
using Movilway.Logging;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Collections.Generic;

namespace Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest
{
    /// <summary>
    /// Clase abstracta para la importación de el llamado a las diferentes operaciones del API SOS IT
    /// </summary>
    abstract public class OperationRequestBase
    {
        /// <summary>
        /// Objeto para gestionar el log de acceso a los diferentes metodos
        /// </summary>
        private static ILogger _logger;

        /// <summary>
        /// Constructor con cada uno de sus parametros
        /// </summary>        
        public OperationRequestBase()
        {
            _logger = LoggerFactory.GetLogger(typeof(OperationRequestBase));
        }

        /// <summary>
        /// Consume o Llama la operacion indicada por parametro y que es consumida del API SOS IT
        /// </summary>
        /// <param name="ParamUrlApi">Url del api que se debe consumir</param>
        /// <param name="ParamTechnicianKey">Llave del tecnico asociado</param>
        /// <param name="ParamFormat">Formato de la informacion enviada</param>
        /// <param name="ParamTimeOutApi">Tiempo maximo de espera de una respuesta</param>
        /// <param name="ParamOperation">Operación</param>
        /// <param name="ParamWorkorderid">Id de la Solicitud de la cual se desea efectuar alguna operacion</param>
        /// <param name="ParamDataInputXML">XML con la informacion requerida para efectuar la operacion</param>
        public AGenericApiResponse CallOperation(string ParamUrlApi, string ParamTechnicianKey, string ParamTimeOutApi, Request.EnumOperation ParamOperation, int ParamWorkorderid, int ParamDetailId,int BranchId = -1, string CountryAcronym = null, XmlDocument ParamDataInputXML = null)
        {
            AGenericApiResponse returnData = new AGenericApiResponse();
            string _methodFulName = String.Format("{0}.{1}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);

            Request Request = new Request();
            string UrlApi = ParamUrlApi;
            string TechnicianKey = ParamTechnicianKey;
            string TimeOutApi = ParamTimeOutApi;
            Request.EnumOperation Operation = ParamOperation;
            int Workorderid = ParamWorkorderid;
            XmlDocument DataInputXML = ParamDataInputXML;

            string _pageContent = "";

            try
            {
                _logger.CheckPointHigh(() => TagValue.New()
                    .MethodName(_methodFulName)
                    .Message("INICIA ::> Llamado al API  SOS IT")
                    .Tag("ParamUrlApi").Value(UrlApi)
                    .Tag("ParamTechnicianKey").Value(TechnicianKey)
                    .Tag("ParamOperation").Value(Operation.ToString())
                );

                try
                {
                    ConsumeService(DataInputXML, Workorderid, ParamDetailId, Request, TechnicianKey, UrlApi, TimeOutApi, out _pageContent); //se tiene la respuesta HTTP 

                    if (!String.IsNullOrEmpty(_pageContent))
                    {
                        returnData = CreateResponseObject(_pageContent, BranchId, CountryAcronym); //se procesa la información    
                    }
                    else
                    {
                        _logger.InfoLow(() => TagValue.New()
                            .MethodName(_methodFulName)
                            .Message("No se pudo cargar la respuesta una vez se llama el API variable _pageContent es NULL")
                            .Tag("ParamUrlApi").Value(UrlApi)
                            .Tag("ParamTechnicianKey").Value(TechnicianKey)
                            .Tag("ParamOperation").Value(Operation.ToString())
                        );

                        returnData.ResponseCode = (int)ErrorMessagesMnemonics.SOSITWsError;
                        returnData.ResponseMessage = ErrorMessagesMnemonics.SOSITWsError.ToDescription();
                    }
                }
                catch (Exception ex)
                {
                    _logger.ExceptionLow(() => TagValue.New()
                        .Exception(ex)
                        .MethodName(_methodFulName)
                        .Tag("ParamUrlApi").Value(UrlApi)
                        .Tag("ParamTechnicianKey").Value(TechnicianKey)
                        .Tag("ParamOperation").Value(Operation.ToString())
                    );

                    returnData.ResponseCode = (int)ErrorMessagesMnemonics.SOSITWsError;
                    returnData.ResponseMessage = ErrorMessagesMnemonics.SOSITWsError.ToDescription();
                }
            }
            catch (Exception ex)
            {
                _logger.ExceptionLow(() => TagValue.New()
                        .Exception(ex)
                        .MethodName(_methodFulName)
                        .Tag("ParamUrlApi").Value(UrlApi)
                        .Tag("ParamTechnicianKey").Value(TechnicianKey)
                        .Tag("ParamOperation").Value(Operation.ToString())
                    );
                returnData.ResponseCode = (int)ErrorMessagesMnemonics.SOSITWsError;
                returnData.ResponseMessage = ErrorMessagesMnemonics.SOSITWsError.ToDescription();
            }

            _logger.CheckPointHigh(() => TagValue.New()
                    .MethodName(_methodFulName)
                    .Message("FINALIZA ::> Llamado al API  SOS IT")
                    .Tag("ParamUrlApi").Value(UrlApi)
                    .Tag("ParamTechnicianKey").Value(TechnicianKey)
                    .Tag("ParamOperation").Value(Operation.ToString())
                );

            return returnData;
        }

        /// <summary>
        /// Consume el API SOS IT 
        /// </summary>
        protected bool ConsumeService(XmlDocument ParamXMLData, int ParamWorkorderid, int ParamDetailId, Request Request, string TechnicianKey, string ParamUrlApi, string ParamTimeOutApi, out string PageContent)
        {
            bool ISOK = true;
            string _methodFulName = String.Format("{0}.{1}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
            _logger.CheckPointHigh(() => TagValue.New()
                .MethodName(_methodFulName)
                .Message("Start")
            );

            Request = getRequest(ParamXMLData, ParamWorkorderid, ParamDetailId, Request, TechnicianKey, ParamUrlApi);

            PageContent = getResponse(Request, ParamTimeOutApi);

            _logger.CheckPointHigh(() => TagValue.New()
                .MethodName(_methodFulName)
                .Message("End")

            );
            return ISOK;

        }


        /// <summary>
        /// Obtiene la respuesta en base a la petición realizada
        /// </summary>
        /// <param name="Request">Petición</param>
        /// <returns>Devuelve el contenido de la página</returns>
        protected string getResponse(Request Request, string ParamTimeOut)
        {
            HttpWebRequest _request = (HttpWebRequest)HttpWebRequest.Create(Request.Url);

            _request.Method = Request.Method;
            _request.KeepAlive = Request.KeepAlive;
            _request.ContentType = Request.ContentType;
            _request.Timeout = Convert.ToInt32(ParamTimeOut);
            _request.ContentLength = 0;

            HttpWebResponse _httpWebResponse = (HttpWebResponse)_request.GetResponse();
            Stream _responseStream = _httpWebResponse.GetResponseStream();

            StreamReader _streamReader = new StreamReader(_responseStream, Encoding.Default);
            string _pageContent = _streamReader.ReadToEnd();

            _streamReader.Close();
            _responseStream.Close();
            _httpWebResponse.Close();

            return _pageContent;
        }

        /// <summary>
        /// Codifica el valor del String a una URL valida
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected String UrlEncode(String url)
        {
            return System.Text.RegularExpressions.Regex.Replace(HttpUtility.UrlEncode(url), "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());
        }
            

        /// <summary>
        /// Crea el response del API Movilway dependiendo de la operacion 
        /// </summary>
        protected abstract AGenericApiResponse CreateResponseObject(string PageContent, int idCountry = -1, string CountryAcronym = null);

        /// <summary>
        /// Obtiene los datos de la petición configurados en el archivo según la operacion
        /// </summary>
        /// <param name="operation">Operacion</param>
        /// <returns>Un objeto <see cref="Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest.Request">Movilway.API.Service.ExtendedApi.Provider.SOSIT</see> que contiene la petición deseada según el tipo </returns>
        protected abstract Request getRequest(XmlDocument ParamXMLData, int ParamWorkorderid, int detailId, Request Request, string ParamTechnicianKey, string ParamUrl);


    }
}