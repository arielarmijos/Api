// <copyright file="TipoIdentificacion.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Cash472
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Enumeración tipos de documento
    /// </summary>
    public enum TipoIdentificacion
    {
        /// <summary>
        /// TI CedulaCiudadania
        /// </summary>
        [EnumMember(Value = "CedulaCiudadania")]
        CedulaCiudadania,

        /// <summary>
        /// TI TarjetaExtranjeria
        /// </summary>
        [EnumMember(Value = "TarjetaExtranjeria")]
        TarjetaExtranjeria,

        /// <summary>
        /// TI CedulaExtranjeria
        /// </summary>
        [EnumMember(Value = "CedulaExtranjeria")]
        CedulaExtranjeria,

        /// <summary>
        /// TI Nit
        /// </summary>
        [EnumMember(Value = "Nit")]
        Nit,

        /// <summary>
        /// TI Pasaporte
        /// </summary>
        [EnumMember(Value = "Pasaporte")]
        Pasaporte,

        /// <summary>
        /// TI DocumentoExtranjero
        /// </summary>
        [EnumMember(Value = "DocumentoExtranjero")]
        DocumentoExtranjero,

        /// <summary>
        /// TI Otro
        /// </summary>
        [EnumMember(Value = "Otro")]
        Otro
    }
}
