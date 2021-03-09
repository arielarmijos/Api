using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.DataContract.SOSIT
{
    public enum EnumServiceCategory
    {
        /// <summary>
        /// Active Directory
        /// </summary>
        [EnumMember(Value = "Active Directory")]
        ActiveDirectory,

        /// <summary>
        /// Aplicaciones Intelligence
        /// </summary>
        [EnumMember(Value = "Aplicaciones Intelligence")]
        AplicacionesIntelligence,

        /// <summary>
        /// C-Channel
        /// </summary>
        [EnumMember(Value = "C-Channel")]
        CChannel,

        /// <summary>
        /// C-Learning
        /// </summary>
        [EnumMember(Value = "C-Learning")]
        CLearning,

        /// <summary>
        /// Compras Generales
        /// </summary>
        [EnumMember(Value = "Compras Generales")]
        ComprasGenerales,

        /// <summary>
        /// Comunicaciones
        /// </summary>
        [EnumMember(Value = "Comunicaciones")]
        Comunicaciones,

        /// <summary>
        /// Correo Electronico
        /// </summary>
        [EnumMember(Value = "Correo Electronico")]
        CorreoElectronico,

        /// <summary>
        /// Equipos y Accesorios
        /// </summary>
        [EnumMember(Value = "Equipos y Accesorios")]
        EquiposAccesorios,

        /// <summary>
        /// Intranet
        /// </summary>
        [EnumMember(Value = "Intranet")]
        Intranet,

        /// <summary>
        /// JDA
        /// </summary>
        [EnumMember(Value = "JDA")]
        JDA,

        /// <summary>
        /// Logistica Internacional
        /// </summary>
        [EnumMember(Value = "Logistica Internacional")]
        LogisticaInternacional,

        /// <summary>
        /// Lync
        /// </summary>
        [EnumMember(Value = "Lync")]
        Lync,

        /// <summary>
        /// MS Project Server
        /// </summary>
        [EnumMember(Value = "MS Project Server")]
        MSProjectServer,

        /// <summary>
        /// OP-Movilway
        /// </summary>
        [EnumMember(Value = "OP-Movilway")]
        OPMovilway,

        /// <summary>
        /// Pia Global
        /// </summary>
        [EnumMember(Value = "Pia Global")]
        PiaGlobal,

        /// <summary>
        /// Pia Guatemala
        /// </summary>
        [EnumMember(Value = "Pia Guatemala")]
        PiaGuatemala,

        /// <summary>
        /// Pia Panama
        /// </summary>
        [EnumMember(Value = "Pia Panama")]
        PiaPanama,

        /// <summary>
        /// Sap B1
        /// </summary>
        [EnumMember(Value = "Sap B1")]
        SapB1,

        /// <summary>
        /// Sap B1 Celistics
        /// </summary>
        [EnumMember(Value = "Sap B1 Celistics")]
        SapB1Celistics,

        /// <summary>
        /// Sap B1 Movilway
        /// </summary>
        [EnumMember(Value = "Sap B1 Movilway")]
        SapB1Movilway,

        /// <summary>
        /// Seguridad Corporativa
        /// </summary>
        [EnumMember(Value = "Seguridad Corporativa")]
        SeguridadCorporativa,

        /// <summary>
        /// Servicio de Telefonia Avaya
        /// </summary>
        [EnumMember(Value = "Servicio de Telefonia Avaya")]
        ServicioTelefoniaAvaya,

        /// <summary>
        /// Servicio de Red
        /// </summary>
        [EnumMember(Value = "Servicio de Red")]
        ServicioRed,

        /// <summary>
        /// Sharepoint
        /// </summary>
        [EnumMember(Value = "Sharepoint")]
        Sharepoint,

        /// <summary>
        /// Soporte de Aplicaciones
        /// </summary>
        [EnumMember(Value = "Soporte de Aplicaciones")]
        SoporteAplicaciones,

        /// <summary>
        /// TI-Infraestructura
        /// </summary>
        [EnumMember(Value = "TI-Infraestructura")]
        TIInfraestructura,

        /// <summary>
        /// TI-Servicios C.P.S
        /// </summary>
        [EnumMember(Value = "TI-Servicios C.P.S")]
        TIServiciosCPS,   

        /// <summary>
        /// Bajo
        /// </summary>
        [EnumMember(Value = "TMS-Brasil")]
        TMSBrasil
    }
}