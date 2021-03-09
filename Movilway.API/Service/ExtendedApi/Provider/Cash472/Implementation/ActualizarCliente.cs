// <copyright file="ActualizarCliente.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.Provider.Cash472
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using DataContract.Cash472;
    using DataContract.Common;
    using Movilway.Logging;

    /// <summary>
    /// Implementación método ActualizarCliente
    /// </summary>
    internal partial class CashProvider : AGenericPlatformAuthentication
    {
        /// <summary>
        /// Actualiza un cliente
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del cliente</param>
        /// <returns>Respuesta de la actualización</returns>
        public ActualizarClienteResponse ActualizarCliente(ActualizarClienteRequest request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.LogRequest(request);

            ActualizarClienteResponse response = new ActualizarClienteResponse();
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

            DwhModel.Cliente infoCliente = this.GetInfoCliente(sessionId, request.Cliente.TipoIdentificacion, request.Cliente.NumeroIdentificacion.Trim(), out this.errorMessage);
            if (this.errorMessage != ErrorMessagesMnemonics.None)
            {
                this.SetResponseErrorCode(response, this.errorMessage);
                this.LogResponse(response);
                return response;
            }

            bool ok = this.ActualizarCliente(sessionId, infoCliente.Id, request.Cliente, out this.errorMessage);
            if (this.errorMessage != ErrorMessagesMnemonics.None || !ok)
            {
                this.errorMessage = !ok ? ErrorMessagesMnemonics.InternalDatabaseErrorUpdatingUser : this.errorMessage;
                this.SetResponseErrorCode(response, this.errorMessage);
                this.LogResponse(response);
                return response;
            }

            response.ResponseCode = 0;
            this.LogResponse(response);
            return response;
        }

        /// <summary>
        /// Inserta un nuevo cliente en base de datos
        /// </summary>
        /// <param name="sessionId">Session ID que será escrito en los logs</param>
        /// <param name="idCliente">ID en base de datos del cliente a actualizar</param>
        /// <param name="cliente">Información básica del cliente</param>
        /// <param name="returnCode">Codigo de error en caso de que algo falle (-1 = OK, >-1 = Error)</param>
        /// <param name="connection">Objeto de conexión a base de datos</param>
        /// <returns>Un <c>int</c> que contiene el ID del cliente creado, cero en caso de falla</returns>
        private bool ActualizarCliente(string sessionId, int idCliente, DataContract.Cash472.Cliente cliente, out ErrorMessagesMnemonics returnCode, SqlConnection connection = null)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            returnCode = ErrorMessagesMnemonics.None;
            bool ret = false;

            try
            {
                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Ejecutando query ..."));

                long tipo = Cash472.CashProvider.ObtenerCodigoTipoIdentificacion(cliente.TipoIdentificacion);
                Dictionary<string, object> queryParams = new Dictionary<string, object>()
                {
                    { "@ClienteId", idCliente },
                    { "@FechaExpedicion", (cliente.FechaExpedicion != null && cliente.FechaExpedicion.HasValue) ? (object)cliente.FechaExpedicion.Value.ToString("yyyy-MM-dd") : DBNull.Value },
                    { "@RazonSocial", DBNull.Value },
                    { "@PrimerNombre", cliente.PrimerNombre.Trim() },
                    { "@SegundoNombre", !string.IsNullOrEmpty(cliente.SegundoNombre) ? (object)cliente.SegundoNombre : DBNull.Value },
                    { "@PrimerApellido", cliente.PrimerApellido.Trim() },
                    { "@SegundoApellido", !string.IsNullOrEmpty(cliente.SegundoApellido) ? (object)cliente.SegundoApellido : DBNull.Value },
                    { "@Ciudad", cliente.CiudadDomicilio },
                    { "@Direccion", !string.IsNullOrEmpty(cliente.Direccion) ? (object)cliente.Direccion : DBNull.Value },
                    { "@Telefono", cliente.Telefono },
                    { "@Celular", (cliente.Celular != null && cliente.Celular.HasValue) ? (object)cliente.Celular.Value : DBNull.Value }
                };

                int modicados = 0;
                if (connection == null)
                {
                    using (connection = Utils.Database.GetCash472DbConnection())
                    {
                        connection.Open();
                        modicados = Utils.Dwh<int>.ExecuteNonQuery(
                        connection,
                        Queries.Cash.UpdateCliente,
                        queryParams,
                        null);
                    }
                }
                else
                {
                    modicados = Utils.Dwh<int>.ExecuteNonQuery(
                    connection,
                    Queries.Cash.UpdateCliente,
                    queryParams,
                    null);
                }

                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Query ejecutado"));
                ret = modicados == 1;
            }
            catch (Exception ex)
            {
                this.ProviderLogger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Error ejecutando query")
                    .Exception(ex));
                returnCode = ErrorMessagesMnemonics.InternalDatabaseError;
                ret = false;
            }

            return ret;
        }
    }
}
