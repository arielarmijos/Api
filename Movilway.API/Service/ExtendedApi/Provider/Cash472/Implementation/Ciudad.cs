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
        /// Realiza la consulta de las ciudades con su codigo DANE
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario </param>
        /// <returns>Listado de las ciudades</returns>
        public ConsultaCiudadesResponse GetCiudades(ConsultaCiudadesRequest request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.LogRequest(request);

            ConsultaCiudadesResponse response = new ConsultaCiudadesResponse();
            string sessionId = this.GetSessionId(request, response, out this.errorMessage);
            if (this.errorMessage != ErrorMessagesMnemonics.None)
            {
                this.LogResponse(response);
                return response;
            }

            List<Cash472.DwhModel.Ciudad> Ciudades = this.GetListCities(sessionId, out this.errorMessage);
            if (this.errorMessage != ErrorMessagesMnemonics.None)
            {
                this.SetResponseErrorCode(response, this.errorMessage);
            }
            else
            {
                response.Ciudades = new List<DataContract.Cash472.Ciudad>();
                foreach (Cash472.DwhModel.Ciudad ciudadtemp in Ciudades)
                {
                    DataContract.Cash472.Ciudad temp = new DataContract.Cash472.Ciudad();
                    this.EstablecerValoresCiudad(temp, ciudadtemp);
                    response.Ciudades.Add(temp);
                }
                response.Quantity = response.Ciudades.Count;
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
        private List<Cash472.DwhModel.Ciudad> GetListCities(string sessionId, out ErrorMessagesMnemonics returnCode, SqlConnection connection = null)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            returnCode = ErrorMessagesMnemonics.None;
            List<Cash472.DwhModel.Ciudad> listCities = new List<DwhModel.Ciudad>();

            try
            {
                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Ejecutando query ..."));

                Dictionary<string, object> queryParams = null;
                if (connection == null)
                {
                    using (connection = Utils.Database.GetCash472DbConnection())
                    {
                        connection.Open();
                        listCities = Utils.Dwh<Cash472.DwhModel.Ciudad>.ExecuteReader(
                        connection,
                        Queries.Cash.ListCities,
                        queryParams,
                        null);
                    }
                }
                else
                {
                    listCities = Utils.Dwh<Cash472.DwhModel.Ciudad>.ExecuteReader(
                        connection,
                        Queries.Cash.ListCities,
                        queryParams,
                        null);
                }

                if (listCities == null || listCities.Count == 0)
                {
                    returnCode = ErrorMessagesMnemonics.ErrorGetCities;
                    listCities = null;
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
                listCities = null;
            }

            return listCities;
        }


        /// <summary>
        /// Establece los valores del objeto especificado a partir del cliente en base de datos
        /// </summary>
        /// <param name="cliente">Cliente con la información básica</param>
        /// <param name="clientedwh">Cliente base de datos</param>
        private void EstablecerValoresCiudad(DataContract.Cash472.Ciudad ciudad, Cash472.DwhModel.Ciudad ciudadesdwh)
        {
            if (ciudad != null && ciudadesdwh != null)
            {
                ciudad.CodigoDANE = ciudadesdwh.CodDANE;
                ciudad.Nombre = ciudadesdwh.Nombre;
                ciudad.CodPostal = ciudadesdwh.CodPostal != null ? ciudadesdwh.CodPostal : " ";
            }
        }
    }
}
