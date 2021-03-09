// <copyright file="TipoCliente.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Cash472
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Enumeración tipos de cliente
    /// </summary>
    public enum TipoCliente
    {
        /// <summary>
        /// TC Emisor
        /// </summary>
        [EnumMember(Value = "Emisor")]
        Emisor,

        /// <summary>
        /// TC Receptor
        /// </summary>
        [EnumMember(Value = "Receptor")]
        Receptor
    }
}
