using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.DataContract.SOSIT
{
    public enum EnumStatusSolicitude
    {
        /// <summary>
        //Abierta
        /// </summary>
        [EnumMember(Value = "Abierta")]
        Abierta,

        /// <summary>
        /// Cerrado
        /// </summary>
        [EnumMember(Value = "Cerrado")]
        Cerrado
    }
}