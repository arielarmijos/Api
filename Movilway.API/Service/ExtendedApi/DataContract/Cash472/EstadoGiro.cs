// <copyright file="EstadoGiro.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Cash472
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Enumeración Estados del Giro
    /// </summary>
    public enum EstadoGiro
    {
        /// <summary>
        /// Estado Emitido
        /// </summary>
        [EnumMember(Value = "Emitido")]
        Emitido,

        /// <summary>
        /// Estado Enmendado
        /// </summary>
        [EnumMember(Value = "Enmendado")]
        Enmendado,

        /// <summary>
        /// Estado Anulado
        /// </summary>
        [EnumMember(Value = "Anulado")]
        Anulado,

        /// <summary>
        /// Estado Pagado
        /// </summary>
        [EnumMember(Value = "Pagado")]
        Pagado,

        /// <summary>
        /// Estado SolicitudAnulacion
        /// </summary>
        [EnumMember(Value = "SolicitudAnulacion")]
        SolicitudAnulacion,

        /// <summary>
        /// Estado SolicitudEmendacion
        /// </summary>
        [EnumMember(Value = "SolicitudEmendacion")]
        SolicitudEmendacion,

        /// <summary>
        /// Estado Inactivo
        /// </summary>
        [EnumMember(Value = "Inactivo")]
        Inactivo,

        /// <summary>
        /// Estado Reembolsable
        /// </summary>
        [EnumMember(Value = "Reembolsable")]
        Reembolsable,

        /// <summary>
        /// Estado Consitucion
        /// </summary>
        [EnumMember(Value = "Consitucion")]
        Consitucion,
                
        /// <summary>
        /// Estado Reversado Emisón
        /// </summary>
        [EnumMember(Value = "ReversadoEmision")]
        ReversadoEmision,

        /// <summary>
        /// Estado Devuelto con flete
        /// </summary>
        [EnumMember(Value = "DevueltoConFlete")]
        DevueltoConFlete,

        /// <summary>
        /// Estado Devuelto sin flete
        /// </summary>
        [EnumMember(Value = "DevueltoSinFlete")]
        DevueltoSinFlete,

        /// <summary>
        /// Estado Solicitud Devolucion
        /// </summary>
        [EnumMember(Value = "SolicitudDevolucion")]
        SolicitudDevolucion,

        /// <summary>
        /// Estado PagoDevuelto
        /// </summary>
        [EnumMember(Value = "PagoDevuelto")]
        PagoDevuelto,

        /// <summary>
        /// Estado GiroRedServi
        /// </summary>
        [EnumMember(Value = "GiroRedServi")]
        GiroRedServi,

        /// <summary>
        /// Estado Inactivo 90 días
        /// </summary>
        [EnumMember(Value = "Inactivo90Dias")]
        Inactivo90Dias,

        /// <summary>
        /// Estado Reembolsable 90 días 
        /// </summary>
        [EnumMember(Value = "Reembolsable90Dias")]
        Reembolsable90Dias,

        /// <summary>
        /// Estado En proceso- se asigna cuando se registra y aun no
        /// se a efectuado transacciones sobre el giro
        /// </summary>
        [EnumMember(Value = "EnProceso")]
        EnProceso,

        /// <summary>
        /// Estado Error Procesando - se asigna cuando se presenta un error en el envio de transacciones al 472 o en el protocolo
        /// </summary>
        [EnumMember(Value = "ErrorProcesando")]
        ErrorProcesando 
    }
}










