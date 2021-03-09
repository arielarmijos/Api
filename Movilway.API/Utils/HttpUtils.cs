using Movilway.API.Utils.Web;
using Movilway.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Movilway.API.Utils
{
    public class HttpUtils
    {

        /// <summary>
        /// UserAgent a envíar por omisión en las peticiones HTTP
        /// </summary>
        public const string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.3; WOW64; Trident/7.0; .NET4.0E; .NET4.0C; InfoPath.3)";

        /// <summary>
        /// Método GET
        /// </summary>
        public const string Get = "GET";

        /// <summary>
        /// Método POST
        /// </summary>
        public const string Post = "POST";

        /// <summary>
        /// ContentType para un POST
        /// </summary>
        public const string ContentTypeForm = "application/x-www-form-urlencoded";

        public const string ContentTypeFormUTF8 = "application/x-www-form-urlencoded; charset=UTF-8";
        public const string ContentTypeTEXT_XML_UTF8 = "text/xml;charset=UTF-8";

        /// <summary>
        /// ContentType para un POST
        /// </summary>
        public const string ContentTypeFormGwt = "text/x-gwt-rpc; charset=utf-8";

        /// <summary>
        /// Objeto para gestionar el log de acceso a los diferentes metodos
        /// </summary>
        private static readonly ILogger Logger;

        /// <summary>
        /// Initializes static members of the <see cref="HttpUtils" /> class
        /// </summary>
        static HttpUtils()
        {
            try
            {
                Logger = LoggerFactory.GetLogger(typeof(HttpUtils));
            }
            catch (Exception)
            {
                throw new Exception("No se ha podido iniciar el sistema de loggin");
            }
        }

        /// <summary>
        /// Construye un objeto <c>HttpWebRequest</c> a partir de una parametrización base
        /// </summary>
        /// <param name="request">Parametrización base del objeto a contruir</param>
        /// <returns>Un objeto <c>HttpWebRequest</c> que contiene la petición</returns>
        public static HttpWebRequest BuildHttpRequest(Request request)
        {
            string requestGetParams = string.Empty;
            if (request.GetParameters.Count > 0)
            {
                if (request.Url.Contains("?"))
                {
                    requestGetParams = request.Url.EndsWith("&") ? string.Empty : "&";
                }
                else
                {
                    requestGetParams = "?";
                }

                requestGetParams += CreatePostParamsString(request.GetParameters);
                request.GetParameters.Clear();
            }
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            HttpWebRequest ret = (HttpWebRequest)HttpWebRequest.Create(request.Url + requestGetParams);
            ret.UserAgent = string.IsNullOrEmpty(request.UserAgent) ? HttpUtils.DefaultUserAgent : request.UserAgent;
            ret.ContentType = request.ContentType;
            ret.Method = request.Method;
            ret.Referer = request.Referer;
            ret.KeepAlive = request.KeepAlive;
            ret.CookieContainer = request.CookieContainer;
            //ret.ProtocolVersion = HttpVersion.Version10;
            //ret.Connection = "Keep-Alive";


            int timeOut = 0;
            try
            {
                timeOut = Convert.ToInt32(ConfigurationManager.AppSettings["TimeOut"]??"0");
            }
            catch (Exception)
            {
            }

            ret.Timeout = (timeOut <= 0) ? 30000 : timeOut;

            if (request.AutomaticDecompression)
            {
                ret.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            if (!string.IsNullOrEmpty(request.Origin))
            {
                ret.Headers.Add("Origin", request.Origin);
            }

            if (!string.IsNullOrEmpty(request.XRequestedWith))
            {
                ret.Headers.Add("x-requested-with", request.XRequestedWith);
            }

            if (request.HandleAutomaticRedirect)
            {
                ret.AllowAutoRedirect = false;
            }

            if (request.Dnt)
            {
                ret.Headers.Add("DNT", "1");
            }

            if (request.ExtraHeaders.Count > 0)
            {
                foreach (string key in request.ExtraHeaders.Keys)
                {
                    ret.Headers.Add(key, request.ExtraHeaders[key]);
                }
                if (request.KeepHeaders == false)
                    request.ExtraHeaders.Clear();
            }

            if (!string.IsNullOrEmpty(request.Accept))
            {
                ret.Accept = request.Accept;
            }

            if (request.Expect100Continue.HasValue)
            {
                ret.ServicePoint.Expect100Continue = request.Expect100Continue.Value;
            }

            // Para evitar problemas de certificados SSL
            ret.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => { return true; };

            try
            {
                if (request.Method == HttpUtils.Post)
                {
                    byte[] parameters = null;
                    switch (request.ContentType)
                    {
                        case HttpUtils.ContentTypeForm:
                            parameters = Encoding.ASCII.GetBytes(CreatePostParamsString(request.PostParameters, request.PostParametersOrder));
                            request.PostParameters.Clear();
                            request.PostParametersOrder.Clear();
                            break;
                        case HttpUtils.ContentTypeFormUTF8:
                            parameters = Encoding.ASCII.GetBytes(CreatePostParamsString(request.PostParameters, request.PostParametersOrder));
                            request.PostParameters.Clear();
                            request.PostParametersOrder.Clear();
                            break;
                        case HttpUtils.ContentTypeFormGwt:
                            parameters = Encoding.ASCII.GetBytes(string.Join("|", request.PostParametersGwt));
                            request.PostParametersGwt.Clear();
                            break;
                        case HttpUtils.ContentTypeTEXT_XML_UTF8:
                            parameters = Encoding.UTF8.GetBytes(request.PostData);
                            request.PostData = string.Empty;
                            request.PostParametersOrder.Clear();
                            break;

                    }

                    ret.ContentLength = parameters.Length;
                    Stream requestStream = ret.GetRequestStream();
                    requestStream.Write(parameters, 0, parameters.Length);
                    requestStream.Close();
                }
            }
            catch (Exception exp)
            {
                Logger.ExceptionLow(() => TagValue.New()
                   .MethodName("BuildHttpRequest")
                   .Exception(exp));
            }

            return ret;
        }

        /// <summary>
        /// Obtiene la respuesta en base a la petición realizada por Http o Https
        /// </summary>
        /// <param name="request">Petición a realizar</param>
        /// <returns>Devuelve el contenido de la página</returns>
        public static string GetResponse(Request request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            string pageContent = string.Empty;

            HttpWebRequest baseRequest = HttpUtils.BuildHttpRequest(request);
            try
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)baseRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();

                if (!request.AutomaticDecompression)
                {
                    if (httpWebResponse.ContentEncoding.ToLower().Contains("gzip"))
                    {
                        responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                    }
                    else if (httpWebResponse.ContentEncoding.ToLower().Contains("deflate"))
                    {
                        responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);
                    }
                }

                StreamReader streamReader = new StreamReader(responseStream, Encoding.Default);
                pageContent = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                httpWebResponse.Close();

                if (request.HandleAutomaticRedirect && (
                    httpWebResponse.StatusCode == HttpStatusCode.Redirect
                    || httpWebResponse.StatusCode == HttpStatusCode.TemporaryRedirect
                    || (httpWebResponse.StatusCode == HttpStatusCode.Found
                        && httpWebResponse.Headers.AllKeys.Contains("Location"))))
                {
                    request.Url = httpWebResponse.Headers["Location"].Contains(request.Host) ? httpWebResponse.Headers["Location"]
                        : ((httpWebResponse.Headers["Location"].ToLower().StartsWith("http") || httpWebResponse.Headers["Location"].ToLower().StartsWith("https")) ? httpWebResponse.Headers["Location"] : request.Host + (string.IsNullOrEmpty(request.BasePath) ? string.Empty : request.BasePath) + httpWebResponse.Headers["Location"]);
                    request.Method = HttpUtils.Get;
                    request.ContentType = String.Empty;

                    //request.Host = "www1.sucursalelectronica.com";
                    pageContent = GetResponse(request);
                }
            }
            catch (Exception ex)
            {
                Logger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Exception(ex));
            }

            return pageContent;
        }

        /// <summary>
        /// Obtiene la respuesta en base al Response(Luego de ejecutar el Request) <b>ESTE MÉTODO NO EJECUTA HttpWebRequest.GetResponse().</b>
        /// </summary>
        /// <param name="request">Petición a realizar</param>
        /// <param name="baseRequest">Request  Base de la peticiòn realizada</param>
        /// <param name="httpWebResponse">Request de la peticiòn realizada</param>
        /// <returns>Devuelve el contenido de la página</returns>
        public static string GetResponse(Request request, HttpWebRequest baseRequest, HttpWebResponse httpWebResponse)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            string pageContent = string.Empty;

            // HttpWebRequest baseRequest = HttpUtils.BuildHttpRequest(request);
            try
            {
                //HttpWebResponse httpWebResponse = (HttpWebResponse)baseRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();

                if (!request.AutomaticDecompression)
                {
                    if (httpWebResponse.ContentEncoding.ToLower().Contains("gzip"))
                    {
                        responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                    }
                    else if (httpWebResponse.ContentEncoding.ToLower().Contains("deflate"))
                    {
                        responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);
                    }
                }

                StreamReader streamReader = new StreamReader(responseStream, Encoding.Default);
                pageContent = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                httpWebResponse.Close();

                if (request.HandleAutomaticRedirect && (
                    httpWebResponse.StatusCode == HttpStatusCode.Redirect
                    || httpWebResponse.StatusCode == HttpStatusCode.TemporaryRedirect
                    || (httpWebResponse.StatusCode == HttpStatusCode.Found
                        && httpWebResponse.Headers.AllKeys.Contains("Location"))))
                {
                    request.Url = httpWebResponse.Headers["Location"].Contains(request.Host) ? httpWebResponse.Headers["Location"]
                        : (request.Host + httpWebResponse.Headers["Location"]);
                    request.Method = HttpUtils.Get;
                    pageContent = GetResponse(request);
                }
            }
            catch (Exception ex)
            {
                Logger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Exception(ex));
            }

            return pageContent;
        }

        /// <summary>
        /// Obtiene una imagen a partir de una petición Web
        /// </summary>
        /// <param name="request">Petición a realizar</param>
        /// <returns>Devuelve la imagen en su formato de bytes, listo para ser escrito en un archivo</returns>
        public static byte[] GetImage(Request request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            HttpWebRequest baseRequest = HttpUtils.BuildHttpRequest(request);
            byte[] ret = null;

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)baseRequest.GetResponse())
                {
                    using (BinaryReader imageBinary = new BinaryReader(response.GetResponseStream()))
                    {
                        using (MemoryStream imageMemoryStream = new MemoryStream())
                        {
                            byte[] aux = imageBinary.ReadBytes(1024);
                            while (aux.Length > 0)
                            {
                                imageMemoryStream.Write(aux, 0, aux.Length);
                                aux = imageBinary.ReadBytes(1024);
                            }

                            ret = new byte[(int)imageMemoryStream.Length];
                            imageMemoryStream.Position = 0;
                            imageMemoryStream.Read(ret, 0, ret.Length);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Exception(ex));
            }

            return ret;
        }

        /// <summary>
        /// Crea el string de parametros a ser enviados en una petición POST
        /// </summary>
        /// <param name="parameters">Parámetros a ser enviados</param>
        /// <param name="order">Orden de los parámetros en el POST</param>
        /// <returns>Cadena parametrizada para enviar en la petición</returns>
        public static string CreatePostParamsString(Dictionary<string, string> parameters, List<string> order = null)
        {
            StringBuilder postParams = new StringBuilder(string.Empty);

            if (order != null && order.Count > 0)
            {
                foreach (string key in order)
                {
                    postParams.Append(UrlEncode(key) + "=" + (string.IsNullOrEmpty(parameters[key]) ? string.Empty : UrlEncode(parameters[key])) + "&");
                }
            }
            else
            {
                foreach (string key in parameters.Keys)
                {
                    postParams.Append(UrlEncode(key) + "=" + (string.IsNullOrEmpty(parameters[key]) ? string.Empty : UrlEncode(parameters[key])) + "&");
                }
            }

            return postParams.ToString().TrimEnd('&');
        }

        /// <summary>
        /// Codifica el valor del <c>string</c> a una URL valida
        /// </summary>
        /// <param name="url">URL a codificar</param>
        /// <returns>Un <c>string</c> que contiene el URL codificado</returns>
        public static string UrlEncode(string url)
        {
            return System.Text.RegularExpressions.Regex.Replace(HttpUtility.UrlEncode(url), "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());
        }

        /// <summary>
        /// Decodifica el valor del <c>string</c> a una URL valida
        /// </summary>
        /// <param name="url">URL a decodificar</param>
        /// <returns>Un <c>string</c> que contiene el URL decodificado</returns>
        public static string UrlDecode(string url)
        {
            return HttpUtility.UrlDecode(url);
        }

    }
}