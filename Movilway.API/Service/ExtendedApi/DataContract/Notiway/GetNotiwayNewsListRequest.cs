using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract.Notiway
{
    /// <summary>
    /// Clase que determina la estructura de para una peticion de 
    /// lista de mensajes de Notiway
    /// </summary>
    public class GetNotiwayNewsListRequest : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 3)]
        public bool OnlyUnread { set; get; }

        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 4)]
        public bool MarkAsReaded { set; get; }
    }
}
