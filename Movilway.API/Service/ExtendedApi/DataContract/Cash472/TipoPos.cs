// <copyright file="TipoPos.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Cash472
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Enumeración tipos de POS
    /// </summary>
    public enum TipoPos
    {
        /// <summary>
        /// POS PosWeb
        /// </summary>
        [EnumMember(Value = "PosWeb")]
        PosWeb,

        /// <summary>
        /// POS App
        /// </summary>
        [EnumMember(Value = "App")]
        App,

        /// <summary>
        /// POS Mobiprint3
        /// </summary>
        [EnumMember(Value = "Mobiprint3")]
        Mobiprint3
    }
}
