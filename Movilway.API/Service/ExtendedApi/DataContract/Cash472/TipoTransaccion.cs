// <copyright file="TipoTransaccion.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Cash472
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Enumeración tipos de Transaccion
    /// </summary>
    public enum TipoTransaccion
    {
        /// <summary>
        /// Tipo Transaccion Cotizacion
        /// </summary>
        [EnumMember(Value = "Cotizacion")]
        Cotizacion,

        /// <summary>
        /// Tipo Transaccion Constitucion
        /// </summary>
        [EnumMember(Value = "Constitucion")]
        Constitucion,

        /// <summary>
        /// Tipo Transaccion Emisión
        /// </summary>
        [EnumMember(Value = "Emision")]
        Emision,

        /// <summary>
        /// Tipo Transaccion Reverso Emisión
        /// </summary>
        [EnumMember(Value = "ReversoEmision")]
        ReversoEmision,

        /// <summary>
        /// Tipo Transaccion Pago
        /// </summary>
        [EnumMember(Value = "Pago")]
        Pago,

        /// <summary>
        /// Tipo Transaccion Reverso Pago
        /// </summary>
        [EnumMember(Value = "ReversoPago")]
        ReversoPago,

        /// <summary>
        /// Tipo Transaccion Devolución incluido Flete
        /// </summary>
        [EnumMember(Value = "DevoluciónIncluidoFlete")]
        DevoluciónIncluidoFlete,

         /// <summary>
        /// Tipo Transaccion Devolución no incluido Flete
        /// </summary>
        [EnumMember(Value = "DevoluciónNoIncluidoFlete")]
        DevoluciónNoIncluidoFlete,

        /// <summary>
        /// Tipo Transaccion Sincronizacion Robot 472
        /// </summary>
        [EnumMember(Value = "SincronizacionRobot472")]
        SincronizacionRobot472
    }
}








