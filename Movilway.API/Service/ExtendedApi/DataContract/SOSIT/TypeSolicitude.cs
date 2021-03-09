using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.DataContract.SOSIT
{
    /// <summary>
    /// Enumeración tipos de categorias
    /// </summary>
    public enum TypeSolicitude
    {

        /// <summary>
        /// Acceso a aplicaciones
        /// </summary>
        [EnumMember(Value = "Acceso a aplicaciones")]
        AccesoAplicaciones,

        /// <summary>
        /// Consulta (pregunte como)
        /// </summary>
        [EnumMember(Value = "Consulta (pregunte como)")]
        Consulta,

        /// <summary>
        /// Consulta (pregunte como)
        /// </summary>
        [EnumMember(Value = "Cuentas de usuarios / Licencias")]
        CuentasUsuariosLicencias,

        /// <summary>
        /// Incidente (Algo deja de funcionar)
        /// </summary>
        [EnumMember(Value = "Incidente (Algo deja de funcionar)")]
        Incidente,

        /// <summary>
        /// Requerimiento Menor
        /// </summary>
        [EnumMember(Value = "Requerimiento Menor")]
        RequerimientoMenor

    }
}