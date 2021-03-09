using Movilway.API.Service.ExtendedApi.DataContract.SOSIT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.SOSIT
{
    /// <summary>
    /// Implementacion de la interfaz de los servicios de los que dispone el modulo Solicitudes SOS IT
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SOSITApi : ISOSITApiSoap
    {
        #region Metodos Servicio
        AddSolicitudeResponse ISOSITApiSoap.AddSolicitude(AddSolicitudeRequest request)
        {
            return this.AddSolicitude(request);
        }


        GetSolicitudeResponse ISOSITApiSoap.GetSolicitude(GetSolicitudeRequest request)
        {
            return this.GetSolicitude(request);
        }

        ListConversationsResponse ISOSITApiSoap.ListConversations(ListConversationsRequest request)
        {
            return this.ListConversations(request);
        }

        ListSolicitudesResponse ISOSITApiSoap.ListSolicitudesByAgent(ListSolicitudesRequest request)
        {
            return this.ListSolicitudesByAgent(request);
        }
        #endregion


        #region Metodos privados que invocan la implementacion
        /// <summary>
        /// Creacion de una Solicitud en SOS IT
        /// </summary>
        /// <param name="request">Objeto que contiene el id asignado a la solicitud creada</param>
        /// <returns>Respuesta de la creacion</returns>
        private AddSolicitudeResponse AddSolicitude(AddSolicitudeRequest request)
        {
            return (new Provider.SOSIT.SolicitudeProvider()).AddSolicitude(request);
        }

        /// <summary>
        /// Consulta de una Solicitud en SOS IT dado el Id
        /// </summary>
        /// <param name="request">Objeto que contiene toda la informacion asignada a una solicitud</param>
        /// <returns>Respuesta de la Consulta</returns>
        private GetSolicitudeResponse GetSolicitude(GetSolicitudeRequest request)
        {
            return (new Provider.SOSIT.SolicitudeProvider()).GetSolicitude(request);
        }

        /// <summary>
        /// Lista las conversaciones asociadas a una solicitud registrada en SOS IT dado el ID de la solicitud
        /// </summary>
        /// <param name="request">Objeto que contiene toda la informacion asignada a una solicitud</param>
        /// <returns>Respuesta de la Consulta</returns>
        private ListConversationsResponse ListConversations(ListConversationsRequest request)
        {
            return (new Provider.SOSIT.SolicitudeProvider()).ListConversations(request);
        }


        /// <summary>
        /// Lista las solicitudes creadas en  SOS IT dada el id de la agencia
        /// </summary>
        /// <param name="request">Objeto que contiene toda la informacion asignada a una solicitud</param>
        /// <returns>Respuesta de la Consulta</returns>
        private ListSolicitudesResponse ListSolicitudesByAgent(ListSolicitudesRequest request)
        {
            return (new Provider.SOSIT.SolicitudeProvider()).ListSolicitudesByAgent(request);
        }
       
        #endregion

    }
}
