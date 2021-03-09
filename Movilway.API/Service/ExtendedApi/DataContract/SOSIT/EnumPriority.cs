using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.DataContract.SOSIT
{
    public enum EnumPriority
    {
        /// <summary>
        /// Alto
        /// </summary>
        [EnumMember(Value = "Alto")]
        Alto,

        /// <summary>
        /// Bajo
        /// </summary>
        [EnumMember(Value = "Bajo")]
        Bajo,

        /// <summary>
        /// Bajo
        /// </summary>
        [EnumMember(Value = "Critica")]
        Critica,

        /// <summary>
        /// Bajo
        /// </summary>
        [EnumMember(Value = "Medio")]
        Medio,

        /// <summary>
        /// Bajo
        /// </summary>
        [EnumMember(Value = "Normal")]
        Normal
    }
}