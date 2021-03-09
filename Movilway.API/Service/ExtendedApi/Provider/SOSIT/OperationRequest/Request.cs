using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;

namespace Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest
{
    public class Request
    {
        public string Host { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string UserId { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string UserAgent { get; set; }
        public string ContentType { get; set; }
        public bool KeepAlive { get; set; }
        public string Referer { get; set; }
        public string Origin { get; set; }
        public CookieContainer CookieContainer { get; set; }
        public bool AutomaticDecompression { get; set; }
        public string XRequestedWith { get; set; }


        public string Accept { get; set; }
        public string AcceptLanguage { get; set; }
        public string AcceptEncoding { get; set; }
        public string Connection { get; set; }
        public string Flujo { get; set; }
        public string Ventana { get; set; }

        public string ViewState { get; set; }
        public string ViewStateValidation { get; set; }
        public string Ultimo_Acceso { get; set; }
        public string Ultima_Hora { get; set; }
        public string Session { get; set; }
        public string ip { get; set; }
        public string tarjeta { get; set; }
        public string tar { get; set; }
        public string sUrProvin { get; set; }
        public string sClProvin { get; set; }
        public string PageResponse { get; set; }
        public string BatUsuario { get; set; }

        public string ViewStateGenerator { get; set; }
        public string CacheControl { get; set; }
        public string ProtocolVersion { get; set; }
        public int ConnectionLimit { get; set; }



        public string PostData { get; set; }
        public Dictionary<string, string> PostParameters { get; set; }

        public enum EnumOperation { ADD_REQUEST, GET_REQUESTS, GET_REQUEST, GET_ALL_CONVERSATIONS, GET_CONVERSATION, GET_NOTIFICATIONS, GET_NOTIFICATION }

        public Request()
        {
            CookieContainer = new CookieContainer();
            PostParameters = new Dictionary<string, string>();
        }
    }
}