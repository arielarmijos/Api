using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract.SOSIT
{
    public class GetConversationRequest : ASecuredApiRequest
    {
        /// <summary>
        /// Gets or sets Workorderid
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 3)]
        public int Workorderid { get; set; }

        /// <summary>
        /// Gets or sets Conversationid
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 4)]
        public int Conversationid { get; set; }


        // <summary>
        /// Valida que todos los campos obligatorias no tengan su valor por omisión
        /// </summary>
        /// <returns><c>true</c> si todos los campos obligatorios existen, <c>false</c> en caso contrario</returns>
        public bool IsValidRequest()
        {
            bool ret = true;
            ret = Workorderid > 0 && Conversationid > 0 ? true : false;       
            return ret;
        }
    }
}