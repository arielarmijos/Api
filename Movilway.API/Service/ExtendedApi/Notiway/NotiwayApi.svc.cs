using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Movilway.API.Service.ExtendedApi.DataContract.Notiway;
using Movilway.API.Service.ExtendedApi.Provider.Notiway;
using System.ServiceModel.Activation;
using System.Configuration;

namespace Movilway.API.Service.ExtendedApi.Notiway
{
    /// <summary>
    /// Implementacion de la interfaz de los servicios de los que dispone Notiway
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class NotiwayApi : INotiwayApiSoap, INotiwayApiRest
    {
        #region soap
        /// <summary>
        /// Obtiene la lista de mensajes pendientes a ser enviados para el usuario que realiza la peticion
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario</param>
        /// <returns>Un objeto <c>GetNotiwayMessageListResponse</c> que contiene la lista de mensajes pendientes</returns>
        GetNotiwayNewsListResponse INotiwayApiSoap.GetNotiwayNewsList(GetNotiwayNewsListRequest request)
        {
            return _getNotiwayNewsList(request);
        }

        /// <summary>
        /// Notifica a Notiway que un mensaje fue leido
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario y el Id del mensaje</param>
        void INotiwayApiSoap.NotiwayNewsReadNotification(NotiwayNewsReadNotificationRequest request)
        {
            _notiwayNewsReadNotification(request);
        }

        /// <summary>
        /// Metodo dummy para probar el servicio
        /// </summary>
        /// <returns>Un <c>String</c> que contiene la version de Notiway</returns>
        String INotiwayApiSoap.GetNotiwayVersion()
        {
            return _getNotiwayVersion();
        }
        #endregion

        #region rest
        /// <summary>
        /// Obtiene la lista de mensajes pendientes a ser enviados para el usuario que realiza la peticion
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario</param>
        /// <returns>Un objeto <c>GetNotiwayMessageListResponse</c> que contiene la lista de mensajes pendientes</returns>
        GetNotiwayNewsListResponse INotiwayApiRest.GetNotiwayNewsListRest(GetNotiwayNewsListRequest request)
        {
            return _getNotiwayNewsList(request);
        }

        /// <summary>
        /// Notifica a Notiway que un mensaje fue leido
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario y el Id del mensaje</param>
        void INotiwayApiRest.NotiwayNewsReadNotificationRest(NotiwayNewsReadNotificationRequest request)
        {
            _notiwayNewsReadNotification(request);
        }

        /// <summary>
        /// Metodo dummy para probar el servicio
        /// </summary>
        /// <returns>Un <c>String</c> que contiene la version de Notiway</returns>
        String INotiwayApiRest.GetNotiwayVersionRest()
        {
            return _getNotiwayVersion();
        }
        #endregion

        #region methodImplementation
        /// <summary>
        /// Obtiene la lista de mensajes pendientes a ser enviados para el usuario que realiza la peticion
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario</param>
        /// <returns>Un objeto <c>GetNotiwayMessageListResponse</c> que contiene la lista de mensajes pendientes</returns>
        private GetNotiwayNewsListResponse _getNotiwayNewsList(GetNotiwayNewsListRequest request)
        {
            bool notiwayEnabled = true;
            try
            {
                notiwayEnabled = Boolean.Parse(ConfigurationManager.AppSettings["NotiwayEnabled"]);
            }
            catch (Exception) { }

            GetNotiwayNewsListResponse response = new GetNotiwayNewsListResponse();
            if (notiwayEnabled)
            {
                response = (new NotiwayProvider()).GetNotiwayNewsList(request);
            }
            else
            {
                response.ResponseCode = 9999;
                response.ResponseMessage = "Notiway disabled";
            }

            return response;
        }

        /// <summary>
        /// Notifica a Notiway que un mensaje fue leido
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario y el Id del mensaje</param>
        private void _notiwayNewsReadNotification(NotiwayNewsReadNotificationRequest request)
        {
            (new NotiwayProvider()).NotiwayNewsReadNotification(request);
        }

        /// <summary>
        /// Metodo dummy para probar el servicio
        /// </summary>
        /// <returns>Un <c>String</c> que contiene la version de Notiway</returns>
        private String _getNotiwayVersion()
        {
            return "0.0.1";
        }
        #endregion
    }
}
