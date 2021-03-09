// <copyright file="ConsultaCliente.cs" company="Movilway">
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
    /// Implementación método ConsultaCliente
    /// </summary>
    internal partial class CashProvider : AGenericPlatformAuthentication
    {
        /// <summary>
        /// Consulta de un cliente
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la consulta</param>
        /// <returns>Información del cliente en caso de que exista</returns>
        public ConsultaClienteResponse ConsultaCliente(ConsultaClienteRequest request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.LogRequest(request);

            ConsultaClienteResponse response = new ConsultaClienteResponse();
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

            // Validar en listas restrictivas
            ErrorMessagesMnemonics errorMessageRestrictiva = ErrorMessagesMnemonics.None;
            int ocurrenciaListas = this.GetInfoClienteListasRestrictivas(sessionId, request.TipoIdentificacion, request.NumeroIdentificacion, out errorMessageRestrictiva);

            // Obtener informacion del cliente en BD
            ErrorMessagesMnemonics errorMessageCliente = ErrorMessagesMnemonics.None;
            DwhModel.Cliente infoCliente = this.GetInfoCliente(sessionId, request.TipoIdentificacion, request.NumeroIdentificacion, out errorMessageCliente);

            if (errorMessageRestrictiva != ErrorMessagesMnemonics.None && errorMessageCliente == ErrorMessagesMnemonics.None)
            {
                // Cliente en listas restrictivas y enrolado
                if (request.FechaExpedicion != null && request.FechaExpedicion.HasValue)
                {
                    if (infoCliente.FechaExpedicion == null && !infoCliente.FechaExpedicion.HasValue)
                    {
                        // Cliente enrolado sin Fecha de expedicion
                        this.SetResponseErrorCode(response, ErrorMessagesMnemonics.ClientWithoutDateEspeditionLists);
                        response.Cliente = new DataContract.Cash472.Cliente();
                        this.EstablecerValoresCliente(response.Cliente, infoCliente);
                    }
                    else if (request.FechaExpedicion.Value.ToString("yyyy-MM-dd").Equals(infoCliente.FechaExpedicion.Value.ToString("yyyy-MM-dd")))
                    {
                        // Cliente enrolado con la misma fecha de expedicion
                        this.SetResponseErrorCode(response, ErrorMessagesMnemonics.ClientInRestrictiveListsAndDatabase);
                        response.Cliente = new DataContract.Cash472.Cliente();
                        this.EstablecerValoresCliente(response.Cliente, infoCliente);
                    }
                    else
                    {
                        // Cliente enrolado con otra fecha de expedicion
                        this.SetResponseErrorCode(response, ErrorMessagesMnemonics.ClientWithAnotherDateEspeditionLists);
                    }
                }
                else
                {
                    // Cliente enrolado no importa la fecha de expedicion 
                    this.SetResponseErrorCode(response, ErrorMessagesMnemonics.ClientInRestrictiveListsAndDatabase);
                    response.Cliente = new DataContract.Cash472.Cliente();
                    this.EstablecerValoresCliente(response.Cliente, infoCliente);
                }
            }
            else if (errorMessageRestrictiva != ErrorMessagesMnemonics.None && errorMessageCliente != ErrorMessagesMnemonics.None)
            {
                // Cliente en listas restrictivas y no enrolado.
                this.SetResponseErrorCode(response, ErrorMessagesMnemonics.ClientInRestrictiveListsAndNotInDatabase);
            }
            else if (errorMessageRestrictiva == ErrorMessagesMnemonics.None && errorMessageCliente != ErrorMessagesMnemonics.None)
            {
                // Cliente No esta en listas restrictivas y no esta enrolado.
                this.SetResponseErrorCode(response, ErrorMessagesMnemonics.ClientNotInRestrictiveListsAndNotInDatabase);
            }
            else if (errorMessageRestrictiva == ErrorMessagesMnemonics.None && errorMessageCliente == ErrorMessagesMnemonics.None)
            {
                // Cliente No esta en listas restrictivas y enrolado.
                if (request.FechaExpedicion != null && request.FechaExpedicion.HasValue)
                {
                    if (infoCliente.FechaExpedicion == null && !infoCliente.FechaExpedicion.HasValue)
                    {
                        // Cliente enrolado con  fecha de expedicion null
                        this.SetResponseErrorCode(response, ErrorMessagesMnemonics.ClientWithoutDateEspeditionNoLists);
                        response.Cliente = new DataContract.Cash472.Cliente();
                        this.EstablecerValoresCliente(response.Cliente, infoCliente);
                    }
                    else if (request.FechaExpedicion.Value.ToString("yyyy-MM-dd").Equals(infoCliente.FechaExpedicion.Value.ToString("yyyy-MM-dd")))
                    {
                        // Cliente enrolado con la misma fecha de expedicion
                        this.SetResponseErrorCode(response, ErrorMessagesMnemonics.ClientNotInRestrictiveListsAndInDatabase);
                        response.Cliente = new DataContract.Cash472.Cliente();
                        this.EstablecerValoresCliente(response.Cliente, infoCliente);
                    }
                    else
                    {
                        // Cliente enrolado con otra fecha de expedicion
                        this.SetResponseErrorCode(response, ErrorMessagesMnemonics.ClientWithAnotherDateEspeditionNoLists);
                    }
                }
                else
                {
                    // Cliente enrolado no importa la fecha de expedicion 
                    this.SetResponseErrorCode(response, ErrorMessagesMnemonics.ClientNotInRestrictiveListsAndInDatabase);
                    response.Cliente = new DataContract.Cash472.Cliente();
                    this.EstablecerValoresCliente(response.Cliente, infoCliente);
                }
            }
            else
            {
                this.SetResponseErrorCode(response, ErrorMessagesMnemonics.ClientNotInRestrictiveListsAndNotInDatabase);
            }

            this.LogResponse(response);
            return response;
        }

        /// <summary>
        /// Obtiene la información de un cliente dado
        /// </summary>
        /// <param name="sessionId">Session ID que será escrito en los logs</param>
        /// <param name="tipoIdentificacion">Tipo de identificación del cliente</param>
        /// <param name="numeroIdentificacion">Número de identificación del cliente</param>
        /// <param name="returnCode">Codigo de error en caso de que algo falle (-1 = OK, >-1 = Error)</param>
        /// <param name="connection">Objeto de conexión a base de datos</param>
        /// <returns>Un objeto <c>DwhModel.Cliente</c> que contiene la información del cliente</returns>
        private DwhModel.Cliente GetInfoCliente(string sessionId, TipoIdentificacion tipoIdentificacion, string numeroIdentificacion, out ErrorMessagesMnemonics returnCode, SqlConnection connection = null)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            returnCode = ErrorMessagesMnemonics.None;
            DwhModel.Cliente ret = null;

            try
            {
                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Ejecutando query ..."));

                long tipo = Cash472.CashProvider.ObtenerCodigoTipoIdentificacion(tipoIdentificacion);
                Dictionary<string, object> queryParams = new Dictionary<string, object>()
                {
                    { "@TipoIdentificacionId", tipo },
                    { "@NumeroIdentificacion", numeroIdentificacion }
                };

                if (connection == null)
                {
                    using (connection = Utils.Database.GetCash472DbConnection())
                    {
                        connection.Open();
                        ret = Utils.Dwh<DwhModel.Cliente>.ExecuteSingle(
                        connection,
                        Queries.Cash.GetInfoCliente,
                        queryParams,
                        null);
                    }
                }
                else
                {
                    ret = Utils.Dwh<DwhModel.Cliente>.ExecuteSingle(
                    connection,
                    Queries.Cash.GetInfoCliente,
                    queryParams,
                    null);
                }

                if (ret == null || ret.Id == 0)
                {
                    returnCode = ErrorMessagesMnemonics.UnableToFindUserInLocalDatabase;
                    ret = null;
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
                ret = null;
            }

            return ret;
        }

        /// <summary>
        /// Obtiene información de la agencia.
        /// </summary>
        /// <param name="sessionId"> Session ID que será escrito en los logs </param>
        /// <param name="access"> Acces login del usuario </param>
        /// <returns> agencia a la que pertenece el usuario. </returns>
        private DwhModel.Agencia GetInfoAgencia(string sessionId, string access)
        {
            Dictionary<string, object> queryParams = new Dictionary<string, object>()
            {
                { "@login", access }
            };

            DwhModel.Agencia agencia = null;

            using (SqlConnection connection = Utils.Database.GetSqlConnectionInstance(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString))
            {
                connection.Open();

                agencia = Utils.Dwh<DwhModel.Agencia>.ExecuteSingle(
                    connection,
                    Utils.Queries.Kinacu.GetInfoAgenteByLogin,
                    queryParams,
                    null);
            }

            if (agencia == null)
            {
                return null;
            }

            DwhModel.Agencia agenciaLocal = null;

            using (SqlConnection connection = Utils.Database.GetCash472DbConnection())
            {
                connection.Open();
                agenciaLocal = Utils.Dwh<DwhModel.Agencia>.ExecuteSingle(
                    connection,
                    Queries.Cash.InfoAgencia,
                    new Dictionary<string, object>()
                    {
                        { "@Id", agencia.BranchId }
                    },
                    null);
            }

            if (agenciaLocal == null || string.IsNullOrEmpty(agenciaLocal.Ciudad))
            {
                return null;
            }

            agencia.Ciudad = agenciaLocal.Ciudad;
            agencia.Id = agenciaLocal.Id;
            agencia.Habilitado = agenciaLocal.Habilitado;

            return agencia;
        }

        /// <summary>
        /// Obtiene la información de un cliente en listas restrictivas
        /// </summary>
        /// <param name="sessionId">Session ID que será escrito en los logs</param>
        /// <param name="tipoIdentificacion">Tipo de identificación del cliente</param>
        /// <param name="numeroIdentificacion">Número de identificación del cliente</param>
        /// <param name="returnCode">Codigo de error en caso de que algo falle (0 = OK, >0 = Error)</param>
        /// <param name="connection">Objeto de conexión a base de datos</param>
        /// <returns>Entero que indica las veces en que el cliente esta en listas restrictivas</returns>
        private int GetInfoClienteListasRestrictivas(string sessionId, TipoIdentificacion tipoIdentificacion, string numeroIdentificacion, out ErrorMessagesMnemonics returnCode, SqlConnection connection = null)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            returnCode = ErrorMessagesMnemonics.None;
            int ret = 0;

            try
            {
                this.ProviderLogger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Ejecutando query ..."));

                long tipo = Cash472.CashProvider.ObtenerCodigoTipoIdentificacion(tipoIdentificacion);
                Dictionary<string, object> queryParams = new Dictionary<string, object>()
                {
                    { "@TipoIdentificacionId", tipo },
                    { "@NumeroIdentificacion", numeroIdentificacion }
                };

                if (connection == null)
                {
                    using (connection = Utils.Database.GetCash472DbConnection())
                    {
                        connection.Open();
                        ret = (int)Utils.Dwh<int>.ExecuteScalar(
                        connection,
                        Queries.Cash.GetOcurrenciasClienteListasRestrictivas,
                        queryParams,
                        null);
                    }
                }
                else
                {
                    ret = (int)Utils.Dwh<int>.ExecuteScalar(
                    connection,
                    Queries.Cash.GetOcurrenciasClienteListasRestrictivas,
                    queryParams,
                    null);
                }

                if (ret > 0)
                {
                    returnCode = ErrorMessagesMnemonics.InvalidUser;
                }

                this.ProviderLogger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Query ejecutado, número ocurrencias: " + ret.ToString()));
            }
            catch (Exception ex)
            {
                this.ProviderLogger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Error ejecutando query")
                    .Exception(ex));
                returnCode = ErrorMessagesMnemonics.InternalDatabaseError;
                ret = -1;
            }

            return ret;
        }
    }
}
