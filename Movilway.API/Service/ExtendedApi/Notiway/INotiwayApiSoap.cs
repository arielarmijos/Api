using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using Movilway.API.Service.ExtendedApi.DataContract.Notiway;

namespace Movilway.API.Service.ExtendedApi.Notiway
{
    /// <summary>
    /// Interface para publicar los servicios de los que dispone Notiway
    /// </summary>
    [ServiceContract(Namespace = "http://api.movilway.net/schema/extendedNotiway")]
    [XmlSerializerFormat]
    [Description("Expone los metodos Notiway por SOAP")]
    public interface INotiwayApiSoap
    {
        /// <summary>
        /// Obtiene la lista de mensajes pendientes a ser enviados para el usuario que realiza la peticion
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario</param>
        /// <returns>Un objeto <c>GetNotiwayMessageListResponse</c> que contiene la lista de mensajes pendientes</returns>
        [OperationContract]
        [Description("Obtiene la lista de mensajes pendientes a ser enviados para el usuario que realiza la peticion.")]
        GetNotiwayNewsListResponse GetNotiwayNewsList(GetNotiwayNewsListRequest request);

        /// <summary>
        /// Notifica a Notiway que un mensaje fue leido
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario y el Id del mensaje</param>
        [OperationContract(IsOneWay = true)]
        [Description("Notifica a Notiway que un mensaje fue leido.")]
        void NotiwayNewsReadNotification(NotiwayNewsReadNotificationRequest request);

        /// <summary>
        /// Metodo dummy para probar el servicio
        /// </summary>
        /// <returns>Un <c>String</c> que contiene la version de Notiway</returns>
        [OperationContract]
        [Description("Metodo dummy para probar el servicio.")]
        String GetNotiwayVersion();
    }
}
