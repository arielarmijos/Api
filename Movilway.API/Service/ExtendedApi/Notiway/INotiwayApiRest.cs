using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract.Notiway;

namespace Movilway.API.Service.ExtendedApi.Notiway
{
    /// <summary>
    /// Interface para publicar los servicios de los que dispone Notiway en su version JSON
    /// </summary>
    [ServiceContract]
    [Description("Expone los metodos Notiway por REST")]
    public interface INotiwayApiRest
    {
        /// <summary>
        /// Obtiene la lista de mensajes pendientes a ser enviados para el usuario que realiza la peticion
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario</param>
        /// <returns>Un objeto <c>GetNotiwayMessageListResponse</c> que contiene la lista de mensajes pendientes</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "GetNotiwayNewsListRest",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped),
        Description("Obtiene la lista de mensajes pendientes a ser enviados para el usuario que realiza la peticion.")]
        GetNotiwayNewsListResponse GetNotiwayNewsListRest(GetNotiwayNewsListRequest request);

        /// <summary>
        /// Notifica a Notiway que un mensaje fue leido
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario y el Id del mensaje</param>
        [OperationContract(IsOneWay = true)]
        [WebInvoke(Method = "POST",
                   UriTemplate = "NotiwayNewsReadNotificationRest",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped),
        Description("Notifica a Notiway que un mensaje fue leido.")]
        void NotiwayNewsReadNotificationRest(NotiwayNewsReadNotificationRequest request);

        /// <summary>
        /// Metodo dummy para probar el servicio
        /// </summary>
        /// <returns>Un <c>String</c> que contiene la version de Notiway</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "GetNotiwayVersionRest",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped),
        Description("Metodo dummy para probar el servicio.")]
        String GetNotiwayVersionRest();
    }
}