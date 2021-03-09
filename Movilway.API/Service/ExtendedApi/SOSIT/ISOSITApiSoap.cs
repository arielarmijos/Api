using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ComponentModel;
using Movilway.API.Service.ExtendedApi.DataContract.SOSIT;

namespace Movilway.API.Service.ExtendedApi.SOSIT
{
    /// <summary>
    /// Interface para publicar los servicios de los que dispone el modulo SOS IT -- HelpDesk Celistics
    /// </summary>
    [ServiceContract(Namespace = "http://api.movilway.net/schema/extendedsosit")]
    [XmlSerializerFormat]
    [Description("Expone los métodos de SOS IT por SOAP")]
    public interface ISOSITApiSoap
    {
        [OperationContract]
        [Description("Crea una solicitude en SOS IT ")]
        AddSolicitudeResponse AddSolicitude(AddSolicitudeRequest request);

        [OperationContract]
        [Description("Consulta una solicitud de SOS IT dado su ID")]
        GetSolicitudeResponse GetSolicitude(GetSolicitudeRequest request);

        [OperationContract]
        [Description("Lista las conversaciones asociadas a una solicitud registrada en SOS IT dado el ID de la solicitud")]
        ListConversationsResponse ListConversations(ListConversationsRequest request);

        [OperationContract]
        [Description("Lista las solicitudes creadas en  SOS IT dada el id de la agencia")]
        ListSolicitudesResponse ListSolicitudesByAgent(ListSolicitudesRequest request);
    }
}
