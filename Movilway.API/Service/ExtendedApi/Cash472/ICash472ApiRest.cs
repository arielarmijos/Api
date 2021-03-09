// <copyright file="ICash472ApiRest.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.Cash472
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Threading.Tasks;

    using DataContract.Cash472;

    /// <summary>
    /// Interface para publicar los servicios de los que dispone el modulo CashIn/CashOut en su versión JSON
    /// </summary>
    [ServiceContract]
    [Description("Expone los métodos CashIn/CashOut por REST")]
    public interface ICash472ApiRest
    {
        /// <summary>
        /// Método dummy para probar el servicio
        /// </summary>
        /// <returns>Un <c>string</c> que contiene la versión de Cash API</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "GetCashApiVersion",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare),
        Description("Método dummy para probar el servicio.")]
        string GetCashApiVersion();

        /// <summary>
        /// Cotizador de giros
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la cotización</param>
        /// <returns>Respuesta de la cotización</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "Cotizar",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare),
        Description("Cotizador de giros.")]
        CotizarResponse Cotizar(CotizarRequest request);

        /// <summary>
        /// Nueva emisión de un giro
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la emisión del giro</param>
        /// <returns>Respuesta de la emisión del giro</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "Emitir",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare),
        Description("Nueva emisión de un giro.")]
        EmitirResponse Emitir(EmitirRequest request);

        /// <summary>
        /// Consulta de un giro
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la consulta</param>
        /// <returns>Respuesta de la consulta</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "Consulta",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare),
        Description("Consulta de un giro.")]
        ConsultaResponse Consulta(ConsultaRequest request);

        /// <summary>
        /// Consulta la información de un cliente
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la consulta</param>
        /// <returns>Respuesta de la consulta</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "ConsultaCliente",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare),
        Description("Consulta la información de un cliente.")]
        ConsultaClienteResponse ConsultaCliente(ConsultaClienteRequest request);

        /// <summary>
        /// Crea un nuevo cliente
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del cliente</param>
        /// <returns>Respuesta de la creación</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "NuevoCliente",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare),
        Description("Crea un nuevo cliente.")]
        NuevoClienteResponse NuevoCliente(NuevoClienteRequest request);

        /// <summary>
        /// Actualiza la información de un cliente
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del cliente</param>
        /// <returns>Respuesta de la actualización</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "ActualizarCliente",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare),
        Description("Actualiza la información de un cliente.")]
        ActualizarClienteResponse ActualizarCliente(ActualizarClienteRequest request);

        /// <summary>
        /// Realiza el proceso de pago de un giro
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del pago</param>
        /// <returns>Respuesta del pago</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "Pago",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare),
        Description("Realiza el proceso de pago de un giro.")]
        PagoResponse Pago(PagoRequest request);

        /// <summary>
        /// Realiza el proceso de reverso de pago de un giro
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del reverso de pago</param>
        /// <returns>Respuesta del reverso de pago</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "ReversoPago",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare),
        Description("Realiza el proceso de reverso de pago de un giro.")]
        ReversoPagoResponse ReversoPago(ReversoPagoRequest request);

        /// <summary>
        /// Consulta listado de Ciudades con su codigo DANE
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario </param>
        /// <returns>Respuesta de la consulta de las ciudades</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "GetCiudades",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare),
        Description("Consulta listado de Ciudades con su codigo DANE.")]
        ConsultaCiudadesResponse GetCiudades(ConsultaCiudadesRequest request);

        /// <summary>
        /// Realiza el proceso de notificación al oficial de cumplimento para un cliente en listas restrictivas
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la notificación</param>
        /// <returns>Respuesta de la notificación</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "NotificacionOficialCumplimiento",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare),
        Description("Realiza el proceso de notificación al oficial de cumplimento para un cliente en listas restrictivas.")]
        NotificacionOficialCumplimientoResponse NotificacionOficialCumplimiento(NotificacionOficialCumplimientoRequest request);

        /// <summary>
        /// Realiza el proceso de generación de factura
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del giro</param>
        /// <returns>Factura generada</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "GenerarFactura",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare),
        Description("Realiza el proceso de generación de factura.")]
        GenerarFacturaResponse GenerarFactura(GenerarFacturaRequest request);
    }
}
