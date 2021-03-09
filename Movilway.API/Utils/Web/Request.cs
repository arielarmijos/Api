using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Movilway.API.Utils.Web
{
    public class Request
    {
        public String Host { get; set; }
        public String Iframe { get; set; }
        public String User { get; set; }
        public String Password { get; set; }
        public String UserId { get; set; }
        public String Url { get; set; }
        public String Method { get; set; }
        public String UserAgent { get; set; }
        public String ContentType { get; set; }
        public bool KeepAlive { get; set; }
        public String Referer { get; set; }
        public String Origin { get; set; }
        public CookieContainer CookieContainer { get; set; }
        public bool AutomaticDecompression { get; set; }
        public bool HandleAutomaticRedirect { get; set; }
        public bool IgnoreServerCertificateValidation { get; set; }
        public String XRequestedWith { get; set; }
        public bool Dnt { get; set; }
        public String Accept { get; set; }
        public bool? Expect100Continue { get; set; }
        public string BasePath { get; set; }
        public bool KeepHeaders { get; set; }

        public Dictionary<String, String> GetParameters { get; set; }
        public Dictionary<String, String> PostParameters { get; set; }
        public List<String> PostParametersOrder { get; set; }
        public List<String> PostParametersGwt { get; set; }
        public Dictionary<String, String> ExtraHeaders { get; set; }
        public string PostData { get; set; }

        public Protocol RequestProtocol { get; set; }
        //public SftpClient SftpClient { get; set; }

        public enum Type { Login, ConciliationReport, Logout, MainPage }
        public enum Protocol { Http, Https, Ftp, Sftp }

        // campos para provincial. //Ariel - Ronald
        public string Flujo { get; set; }
        public string Ventana { get; set; }
        public string sUrProvin { get; set; }
        public string sClProvin { get; set; }
        public string Session { get; set; }
        public string PageResponse { get; set; }
        public string Ultimo_Acceso { get; set; }
        public string Ultima_Hora { get; set; }
        public string ip { get; set; }
        public string tar { get; set; }
        public string tarjeta { get; set; }

        //Campos para BanescoT //Ariel - Ronald
        public string ViewState { get; set; }
        public string ViewStateGenerator { get; set; }
        public string BatUsuario { get; set; }
        public string ViewStateValidation { get; set; }
        //Ariel 2016-ENE-28 se agrega el campo cta al config  - Ronald
        public string cta { get; set; }

        public Request()
        {
            RequestProtocol = Request.Protocol.Https;
            CookieContainer = new CookieContainer();
            GetParameters = new Dictionary<String, String>();
            PostParameters = new Dictionary<String, String>();
            PostParametersOrder = new List<String>();
            PostParametersGwt = new List<String>();
            ExtraHeaders = new Dictionary<String, String>();
            UserAgent = String.Empty;
            Referer = String.Empty;
            ContentType = String.Empty;
           // SftpClient = null;
        }
        public Request(string url)
        {
            RequestProtocol = Request.Protocol.Https;
            CookieContainer = new CookieContainer();
            GetParameters = new Dictionary<String, String>();
            PostParameters = new Dictionary<String, String>();
            PostParametersOrder = new List<String>();
            PostParametersGwt = new List<String>();
            ExtraHeaders = new Dictionary<String, String>();
            UserAgent = String.Empty;
            Referer = String.Empty;
            ContentType = String.Empty;
            // SftpClient = null;
            this.Url = url;
        }
    }
}