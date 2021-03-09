// <copyright file="Consulta.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.Provider.Cash472
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using DataContract.Cash472;
    using DataContract.Common;
    using Movilway.Logging;
    using Movilway.API.Core.Security;

    /// <summary>
    /// Implementación método ConsultaGiros
    /// </summary>
    internal partial class CashProvider : AGenericPlatformAuthentication
    {
        /// <summary>
        /// Consulta de giros
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la consulta</param>
        /// <returns>Respuesta de la consulta</returns>
        public ConsultaResponse Consulta(ConsultaRequest request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.LogRequest(request);

            ConsultaResponse response = new ConsultaResponse();

            string sessionId = this.GetSessionId(request, response, out this.errorMessage);
            if (this.errorMessage != ErrorMessagesMnemonics.None)
            {
                this.LogResponse(response);
                return response;
            }

            if (!request.IsValidRequest())
            {
                this.SetResponseErrorCode(response, ErrorMessagesMnemonics.InvalidRequiredFields);
                this.LogResponse(response);
                return response;
            }

            MultiPay472.Service1SoapClient client = this.GetSoapClient();
            string endpointName = "ConsultaGiros";
            try
            {
                MultiPay472.ConsultaGiro peticion = new MultiPay472.ConsultaGiro();
                peticion.NitRed = this.multipayNitRed;
                peticion.CodigoTerminal = this.multipayTerminal;
                peticion.CodigoTransaccion = this.GenerarCodigoTransaccion(sessionId);

                peticion.CodigoPuntoVenta = request.Pdv;
                peticion.TipoCliente = Cash472.CashProvider.ObtenerCodigoTipoCliente(request.TipoCliente);
                peticion.OIdentificacionCliente = new MultiPay472.IdentificacionCliente();
                peticion.OIdentificacionCliente.TipoIdentificacion = Cash472.CashProvider.ObtenerCodigoTipoIdentificacion(request.TipoIdentificacion);
                peticion.OIdentificacionCliente.NumeroIdentificacion = request.NumeroIdentificacion;

                if (!string.IsNullOrEmpty(request.Pin))
                {
                    peticion.PIN = Multipay472TripleDes.Encrypt(this.multipayTripleDesKey, request.Pin); 
                }

                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Llamando servicio \"" + endpointName + "\" ..."));

                MultiPay472.RespuestaConsultaGiro[] resp = client.ConsultaGiros(peticion, this.multipayUsuario);
                MultiPay472.RespuestaConsultaGiro primero = resp != null && resp.Length > 0 ? resp[0] : null;

                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Respuesta servicio \"" + endpointName + "\"")
                    .Tag("Respuesta").Value(resp != null && primero != null ? resp[0].CodigoRespuesta : "NULL"));

                if (resp != null && primero != null && primero.CodigoRespuesta == CashProvider.CodigoRespuestaExitoso)
                {
                    response.ResponseCode = 0;
                    response.Giros = new List<Giro>();
                    foreach (MultiPay472.RespuestaConsultaGiro it in resp)
                    {
                        DataContract.Cash472.Giro giro = new DataContract.Cash472.Giro();
                        this.EstablecerValoresGiro(giro, it);
                        response.Giros.Add(giro);
                    }

                    response.Quantity = response.Giros.Count;
                }
                else
                {
                    if (resp == null || primero == null)
                    {
                        this.errorMessage = ErrorMessagesMnemonics.WebServiceDoesNotRespond;
                        response.ResponseCode = (int)this.errorMessage;
                        response.ResponseMessage = this.errorMessage.ToDescription();
                    }
                    else
                    {
                        response.ResponseMessage = CashProvider.ObtenerMensajeCodigoRespuesta(primero.CodigoRespuesta);
                    }
                }
            }
            catch (Exception ex)
            {
                this.ProviderLogger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Error llamando servicio \"" + endpointName + "\"")
                    .Exception(ex));
            }

            this.LogResponse(response);
            return response;
        }

        /// <summary>
        /// Establece los valores del objeto especificado (origen o destino) a partir del cliente especificado
        /// </summary>
        /// <param name="cliente">Cliente con la información básica</param>
        /// <param name="origen">Objeto origen</param>
        /// <param name="destino">Objeto destino</param>
        private void EstablecerValoresCliente(DataContract.Cash472.Cliente cliente, MultiPay472.Originador origen, MultiPay472.Destinatario destino)
        {
            if (origen != null)
            {
                cliente.TipoIdentificacion = Cash472.CashProvider.ObtenerTipoIdentificacion(origen.OIdentificacionCliente.TipoIdentificacion);
                cliente.NumeroIdentificacion = origen.OIdentificacionCliente.NumeroIdentificacion;
                cliente.PrimerNombre = origen.PrimerNombre;
                cliente.PrimerApellido = origen.PrimerApellido;
                cliente.CiudadDomicilio = origen.CodigoDaneCiudadDomicilio;
                cliente.Telefono = origen.NumeroTelefono;

                if (!string.IsNullOrEmpty(origen.FechaExpedicion))
                {
                    cliente.FechaExpedicion = Cash472.CashProvider.ObtenerFechaDesdeString(origen.FechaExpedicion);
                }

                if (origen.NumeroCelular != null && origen.NumeroCelular.HasValue)
                {
                    cliente.Celular = origen.NumeroCelular.Value;
                }

                if (!string.IsNullOrEmpty(origen.SegundoNombre))
                {
                    cliente.SegundoNombre = origen.SegundoNombre;
                }

                if (!string.IsNullOrEmpty(origen.SegundoApellido))
                {
                    cliente.SegundoApellido = origen.SegundoApellido;
                }

                if (!string.IsNullOrEmpty(origen.Direccion))
                {
                    cliente.Direccion = origen.Direccion;
                }
            }

            if (destino != null)
            {
                cliente.TipoIdentificacion = Cash472.CashProvider.ObtenerTipoIdentificacion(destino.OIdentificacionCliente.TipoIdentificacion);
                cliente.NumeroIdentificacion = destino.OIdentificacionCliente.NumeroIdentificacion;
                cliente.PrimerNombre = destino.PrimerNombre;
                cliente.PrimerApellido = destino.PrimerApellido;
                cliente.CiudadDomicilio = destino.CodigoDaneCiudadDomicilio;
                cliente.Telefono = destino.NumeroTelefono;

                if (destino.NumeroCelular != null && destino.NumeroCelular.HasValue)
                {
                    cliente.Celular = destino.NumeroCelular.Value;
                }

                if (!string.IsNullOrEmpty(destino.SegundoNombre))
                {
                    cliente.SegundoNombre = destino.SegundoNombre;
                }

                if (!string.IsNullOrEmpty(destino.SegundoApellido))
                {
                    cliente.SegundoApellido = destino.SegundoApellido;
                }

                if (!string.IsNullOrEmpty(destino.Direccion))
                {
                    cliente.Direccion = destino.Direccion;
                }
            }
        }

        /// <summary>
        /// Establece los valores del objeto especificado a partir del cliente en base de datos
        /// </summary>
        /// <param name="cliente">Cliente con la información básica</param>
        /// <param name="clientedwh">Cliente base de datos</param>
        private void EstablecerValoresCliente(DataContract.Cash472.Cliente cliente, Cash472.DwhModel.Cliente clientedwh)
        {
            if (cliente != null && clientedwh != null)
            {
                cliente.TipoIdentificacion = Cash472.CashProvider.ObtenerTipoIdentificacion(clientedwh.TipoIdentificacionId);
                cliente.NumeroIdentificacion = clientedwh.NumeroIdentificacion;
                cliente.PrimerNombre = clientedwh.PrimerNombre;
                cliente.PrimerApellido = clientedwh.PrimerApellido;
                cliente.CiudadDomicilio = clientedwh.Ciudad;

                cliente.SegundoNombre = !string.IsNullOrEmpty(clientedwh.SegundoNombre) ? clientedwh.SegundoNombre : null;
                cliente.SegundoApellido = !string.IsNullOrEmpty(clientedwh.SegundoApellido) ? clientedwh.SegundoApellido : null;
                cliente.Direccion = !string.IsNullOrEmpty(clientedwh.Direccion) ? clientedwh.Direccion : null;

                if (clientedwh.FechaExpedicion != null && clientedwh.FechaExpedicion.HasValue)
                {
                    cliente.FechaExpedicion = clientedwh.FechaExpedicion.Value;
                }

                if (clientedwh.Telefono != null && clientedwh.Telefono.HasValue)
                {
                    cliente.Telefono = clientedwh.Telefono.Value;
                }

                if (clientedwh.Celular != null && clientedwh.Celular.HasValue)
                {
                    cliente.Celular = clientedwh.Celular.Value;
                }
            }
        }

        /// <summary>
        /// Establece los valores del objeto giro a partir de un objeto de 472
        /// </summary>
        /// <param name="giro">Giro con la información básica</param>
        /// <param name="original">Giro proveniente de Cash472</param>
        private void EstablecerValoresGiro(DataContract.Cash472.Giro giro, MultiPay472.RespuestaConsultaGiro original)
        {
            if (giro != null && original != null)
            {
                giro.CodigoTransaccion = original.CodigoTransaccion;
                giro.Id = original.OGiro.IdGiro;
                giro.TotalRecibido = original.OGiro.ValorRecibidoTotal;
                giro.TotalAEntregar = original.OGiro.Valor;
                giro.Flete = original.OGiro.Flete;
                giro.IncluyeFlete = original.OGiro.IncluyeFlete;
                giro.Fecha = Cash472.CashProvider.ObtenerFechaDesdeString(original.OGiro.FechaConstitucion);
                giro.Emisor = new Cliente();
                giro.Receptor = new Cliente();
                this.EstablecerValoresCliente(giro.Emisor, original.Originador, null);
                this.EstablecerValoresCliente(giro.Receptor, null, original.Destinatario);
            }
        }
    }
}
