// <copyright file="NuevoCliente.cs" company="Movilway">
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
    /// Implementación método NuevoCliente
    /// </summary>
    internal partial class CashProvider : AGenericPlatformAuthentication
    {
        /// <summary>
        /// Crea un nuevo cliente
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del cliente</param>
        /// <returns>Respuesta de la creación</returns>
        public NuevoClienteResponse NuevoCliente(NuevoClienteRequest request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.LogRequest(request);

            NuevoClienteResponse response = new NuevoClienteResponse();
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
            if ((this.errorMessage != ErrorMessagesMnemonics.None && this.errorMessage != ErrorMessagesMnemonics.UnableToFindUserInLocalDatabase)
                || infoCliente != null)
            {
                this.errorMessage = infoCliente != null ? ErrorMessagesMnemonics.UserAlreadyExistsInLocalDatabase : this.errorMessage;
                this.SetResponseErrorCode(response, this.errorMessage);
                this.LogResponse(response);
                return response;
            }

            int id = this.InsertNuevoCliente(sessionId, request.Cliente, out this.errorMessage);
            if (this.errorMessage != ErrorMessagesMnemonics.None || id == 0)
            {
                this.errorMessage = id == 0 ? ErrorMessagesMnemonics.InternalDatabaseErrorCreatingUser : this.errorMessage;
                this.SetResponseErrorCode(response, this.errorMessage);
                this.LogResponse(response);
                return response;
            }

            response.ResponseCode = 0;
            response.Id = id;
            this.LogResponse(response);
            return response;
        }

        /// <summary>
        /// Inserta un nuevo cliente en base de datos
        /// </summary>
        /// <param name="sessionId">Session ID que será escrito en los logs</param>
        /// <param name="cliente">Información básica del cliente</param>
        /// <param name="returnCode">Codigo de error en caso de que algo falle (-1 = OK, >-1 = Error)</param>
        /// <param name="connection">Objeto de conexión a base de datos</param>
        /// <returns>Un <c>int</c> que contiene el ID del cliente creado, cero en caso de falla</returns>
        private int InsertNuevoCliente(string sessionId, DataContract.Cash472.Cliente cliente, out ErrorMessagesMnemonics returnCode, SqlConnection connection = null)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            returnCode = ErrorMessagesMnemonics.None;
            int ret = 0;

            try
            {
                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Ejecutando query ..."));

                long tipo = Cash472.CashProvider.ObtenerCodigoTipoIdentificacion(cliente.TipoIdentificacion);
                Dictionary<string, object> queryParams = new Dictionary<string, object>()
                {
                    { "@TipoIdentificacionId", tipo },
                    { "@NumeroIdentificacion", cliente.NumeroIdentificacion.Trim() },
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

                if (connection == null)
                {
                    using (connection = Utils.Database.GetCash472DbConnection())
                    {
                        connection.Open();
                        ret = (int)Utils.Dwh<int>.ExecuteScalar(
                        connection,
                        Queries.Cash.InsertCliente,
                        queryParams,
                        null);
                    }
                }
                else
                {
                    ret = (int)Utils.Dwh<int>.ExecuteScalar(
                    connection,
                    Queries.Cash.InsertCliente,
                    queryParams,
                    null);
                }

                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Query ejecutado"));
            }
            catch (Exception ex)
            {
                this.ProviderLogger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Error ejecutando query")
                    .Exception(ex));
                returnCode = ErrorMessagesMnemonics.InternalDatabaseError;
                ret = 0;
            }

            return ret;
        }
    }
}
