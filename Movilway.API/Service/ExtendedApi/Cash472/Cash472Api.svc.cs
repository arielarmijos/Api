// <copyright file="Cash472Api.svc.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.Cash472
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.Text;

    using DataContract.Cash472;

    /// <summary>
    /// Implementacion de la interfaz de los servicios de los que dispone el modulo CashIn/CashOut
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Cash472Api : ICash472ApiSoap, ICash472ApiRest
    {
        /// <summary>
        /// Método dummy para probar el servicio
        /// </summary>
        /// <returns>Un <c>string</c> que contiene la versión de Cash API</returns>
        string ICash472ApiSoap.GetCashApiVersion()
        {
            return this.GetCashApiVersion();
        }

        /// <summary>
        /// Método dummy para probar el servicio
        /// </summary>
        /// <returns>Un <c>string</c> que contiene la versión de Cash API</returns>
        string ICash472ApiRest.GetCashApiVersion()
        {
            return this.GetCashApiVersion();
        }

        /// <summary>
        /// Cotizador de giros
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la cotización</param>
        /// <returns>Respuesta de la cotización</returns>
        CotizarResponse ICash472ApiSoap.Cotizar(CotizarRequest request)
        {
            return this.Cotizar(request);
        }

        /// <summary>
        /// Cotizador de giros
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la cotización</param>
        /// <returns>Respuesta de la cotización</returns>
        CotizarResponse ICash472ApiRest.Cotizar(CotizarRequest request)
        {
            return this.Cotizar(request);
        }

        /// <summary>
        /// Nueva emisión de un giro
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la emisión del giro</param>
        /// <returns>Respuesta de la emisión del giro</returns>
        EmitirResponse ICash472ApiSoap.Emitir(EmitirRequest request)
        {
            return this.Emitir(request);
        }

        /// <summary>
        /// Nueva emisión de un giro
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la emisión del giro</param>
        /// <returns>Respuesta de la emisión del giro</returns>
        EmitirResponse ICash472ApiRest.Emitir(EmitirRequest request)
        {
            return this.Emitir(request);
        }

        /// <summary>
        /// Consulta de un giro
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la consulta</param>
        /// <returns>Respuesta de la consulta</returns>
        ConsultaResponse ICash472ApiSoap.Consulta(ConsultaRequest request)
        {
            return this.Consulta(request);
        }

        /// <summary>
        /// Consulta de un giro
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la consulta</param>
        /// <returns>Respuesta de la consulta</returns>
        ConsultaResponse ICash472ApiRest.Consulta(ConsultaRequest request)
        {
            return this.Consulta(request);
        }

        /// <summary>
        /// Consulta la información de un cliente
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la consulta</param>
        /// <returns>Respuesta de la consulta</returns>
        ConsultaClienteResponse ICash472ApiSoap.ConsultaCliente(ConsultaClienteRequest request)
        {
            return this.ConsultaCliente(request);
        }

        /// <summary>
        /// Consulta la información de un cliente
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la consulta</param>
        /// <returns>Respuesta de la consulta</returns>
        ConsultaClienteResponse ICash472ApiRest.ConsultaCliente(ConsultaClienteRequest request)
        {
            return this.ConsultaCliente(request);
        }

        /// <summary>
        /// Crea un nuevo cliente
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del cliente</param>
        /// <returns>Respuesta de la creación</returns>
        NuevoClienteResponse ICash472ApiSoap.NuevoCliente(NuevoClienteRequest request)
        {
            return this.NuevoCliente(request);
        }

        /// <summary>
        /// Crea un nuevo cliente
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del cliente</param>
        /// <returns>Respuesta de la creación</returns>
        NuevoClienteResponse ICash472ApiRest.NuevoCliente(NuevoClienteRequest request)
        {
            return this.NuevoCliente(request);
        }

        /// <summary>
        /// Actualiza la información de un cliente
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del cliente</param>
        /// <returns>Respuesta de la actualización</returns>
        ActualizarClienteResponse ICash472ApiSoap.ActualizarCliente(ActualizarClienteRequest request)
        {
            return this.ActualizarCliente(request);
        }

        /// <summary>
        /// Actualiza la información de un cliente
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del cliente</param>
        /// <returns>Respuesta de la actualización</returns>
        ActualizarClienteResponse ICash472ApiRest.ActualizarCliente(ActualizarClienteRequest request)
        {
            return this.ActualizarCliente(request);
        }

        /// <summary>
        /// Realiza el proceso de pago de un giro
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del pago</param>
        /// <returns>Respuesta del pago</returns>
        PagoResponse ICash472ApiSoap.Pago(PagoRequest request)
        {
            return this.Pago(request);
        }

        /// <summary>
        /// Realiza el proceso de pago de un giro
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del pago</param>
        /// <returns>Respuesta del pago</returns>
        PagoResponse ICash472ApiRest.Pago(PagoRequest request)
        {
            return this.Pago(request);
        }

        /// <summary>
        /// Realiza el proceso de reverso de pago de un giro
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del reverso de pago</param>
        /// <returns>Respuesta del reverso de pago</returns>
        ReversoPagoResponse ICash472ApiSoap.ReversoPago(ReversoPagoRequest request)
        {
            return this.ReversoPago(request);
        }

        /// <summary>
        /// Realiza el proceso de reverso de pago de un giro
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del reverso de pago</param>
        /// <returns>Respuesta del reverso de pago</returns>
        ReversoPagoResponse ICash472ApiRest.ReversoPago(ReversoPagoRequest request)
        {
            return this.ReversoPago(request);
        }

        /// <summary>
        /// Consulta listado de Ciudades con su codigo DANE
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario </param>
        /// <returns>Respuesta de la consulta de las ciudades</returns>
        ConsultaCiudadesResponse ICash472ApiSoap.GetCiudades(ConsultaCiudadesRequest request)
        {
            return this.GetCiudades(request);
        }

        /// <summary>
        /// Consulta listado de Ciudades con su codigo DANE
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario </param>
        /// <returns>Respuesta de la consulta de las ciudades</returns>
        ConsultaCiudadesResponse ICash472ApiRest.GetCiudades(ConsultaCiudadesRequest request)
        {
            return this.GetCiudades(request);
        }

        /// <summary>
        /// Realiza el proceso de notificación al oficial de cumplimento para un cliente en listas restrictivas
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la notificación</param>
        /// <returns>Respuesta de la notificación</returns>
        NotificacionOficialCumplimientoResponse ICash472ApiSoap.NotificacionOficialCumplimiento(NotificacionOficialCumplimientoRequest request)
        {
            return this.NotificacionOficialCumplimiento(request);
        }

        /// <summary>
        /// Realiza el proceso de notificación al oficial de cumplimento para un cliente en listas restrictivas
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la notificación</param>
        /// <returns>Respuesta de la notificación</returns>
        NotificacionOficialCumplimientoResponse ICash472ApiRest.NotificacionOficialCumplimiento(NotificacionOficialCumplimientoRequest request)
        {
            return this.NotificacionOficialCumplimiento(request);
        }

        /// <summary>
        /// Realiza el proceso de generación de factura
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del giro</param>
        /// <returns>Factura generada</returns>
        GenerarFacturaResponse ICash472ApiSoap.GenerarFactura(GenerarFacturaRequest request)
        {
            return this.GenerarFactura(request);
        }

        /// <summary>
        /// Realiza el proceso de generación de factura
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del giro</param>
        /// <returns>Factura generada</returns>
        GenerarFacturaResponse ICash472ApiRest.GenerarFactura(GenerarFacturaRequest request)
        {
            return this.GenerarFactura(request);
        }

        /// <summary>
        /// Método dummy para probar el servicio
        /// </summary>
        /// <returns>Un <c>string</c> que contiene la versión de Cash API</returns>
        private string GetCashApiVersion()
        {
            // return Core.Security.Multipay472TripleDes.Decrypt("0123456789ABCDEFFEDCBA9876543210", "687AA9F15515C2DB");
            return "0.0.1";
        }

        /// <summary>
        /// Cotizador de giros
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la cotización</param>
        /// <returns>Respuesta de la cotización</returns>
        private CotizarResponse Cotizar(CotizarRequest request)
        {
            return (new Provider.Cash472.CashProvider()).Cotizar(request);
        }

        /// <summary>
        /// Nueva emisión de un giro
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la emisión del giro</param>
        /// <returns>Respuesta de la emisión del giro</returns>
        private EmitirResponse Emitir(EmitirRequest request)
        {
            return (new Provider.Cash472.CashProvider()).Emitir(request);
        }

        /// <summary>
        /// Consulta de un giro
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la consulta</param>
        /// <returns>Respuesta de la consulta</returns>
        private ConsultaResponse Consulta(ConsultaRequest request)
        {
            return (new Provider.Cash472.CashProvider()).Consulta(request);
        }

        /// <summary>
        /// Consulta la información de un cliente
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la consulta</param>
        /// <returns>Respuesta de la consulta</returns>
        private ConsultaClienteResponse ConsultaCliente(ConsultaClienteRequest request)
        {
            return (new Provider.Cash472.CashProvider()).ConsultaCliente(request);
        }

        /// <summary>
        /// Crea un nuevo cliente
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del cliente</param>
        /// <returns>Respuesta de la creación</returns>
        private NuevoClienteResponse NuevoCliente(NuevoClienteRequest request)
        {
            return (new Provider.Cash472.CashProvider()).NuevoCliente(request);
        }

        /// <summary>
        /// Actualiza la información de un cliente
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del cliente</param>
        /// <returns>Respuesta de la actualización</returns>
        private ActualizarClienteResponse ActualizarCliente(ActualizarClienteRequest request)
        {
            return (new Provider.Cash472.CashProvider()).ActualizarCliente(request);
        }

        /// <summary>
        /// Realiza el proceso de pago de un giro
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del pago</param>
        /// <returns>Respuesta del pago</returns>
        private PagoResponse Pago(PagoRequest request)
        {
            return (new Provider.Cash472.CashProvider()).Pago(request);
        }

        /// <summary>
        /// Realiza el proceso de reverso de pago de un giro
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del reverso de pago</param>
        /// <returns>Respuesta del reverso de pago</returns>
        private ReversoPagoResponse ReversoPago(ReversoPagoRequest request)
        {
            return (new Provider.Cash472.CashProvider()).ReversoPago(request);
        }

        /// <summary>
        /// Consulta listado de Ciudades con su codigo DANE
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario </param>
        /// <returns>Respuesta de la consulta de las ciudades</returns>
        private ConsultaCiudadesResponse GetCiudades(ConsultaCiudadesRequest request)
        {
            return (new Provider.Cash472.CashProvider()).GetCiudades(request);
        }

        /// <summary>
        /// Realiza el proceso de notificación al oficial de cumplimento para un cliente en listas restrictivas
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la notificación</param>
        /// <returns>Respuesta de la notificación</returns>
        private NotificacionOficialCumplimientoResponse NotificacionOficialCumplimiento(NotificacionOficialCumplimientoRequest request)
        {
            return (new Provider.Cash472.CashProvider()).NotificacionOficialCumplimiento(request);
        }

        /// <summary>
        /// Realiza el proceso de generación de factura
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del giro</param>
        /// <returns>Factura generada</returns>
        private GenerarFacturaResponse GenerarFactura(GenerarFacturaRequest request)
        {
            return (new Provider.Cash472.CashProvider()).GenerarFactura(request);
        }
    }
}
