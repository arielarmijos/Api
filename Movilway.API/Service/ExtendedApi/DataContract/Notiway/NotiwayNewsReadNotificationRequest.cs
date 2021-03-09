using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract.Notiway
{
    /// <summary>
    /// Clase que determina la estructura de para una notificacion de lectura
    /// de un mensaje de Notiway, solo necesita las credenciales del usuario y
    /// el id del mensaje
    /// </summary>
    public class NotiwayNewsReadNotificationRequest : ASecuredApiRequest
    {
        /// <summary>
        /// ID del mensaje que fue leido por el usuario
        /// </summary>
        [Loggable]
        [DataMember(Order = 0, IsRequired = true)]
        public int NewsId { set; get; }

        /// <summary>
        /// ID del schedule asociado al mensaje
        /// </summary>
        [Loggable]
        [DataMember(Order = 1, IsRequired = true)]
        public int ScheduleId { set; get; }
    }
}