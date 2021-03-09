// <copyright file="Database.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Clase generica de utilidades con respecto a base de datos
    /// </summary>
    internal static class Database
    {
        /// <summary>
        /// Obtiene una nueva instancia de un objeto <c>SqlConnection</c>
        /// </summary>
        /// <param name="connectionString">Cadena de conexión a base de datos</param>
        /// <returns>La instancia del objeto <c>SqlConnection</c></returns>
        public static SqlConnection GetSqlConnectionInstance(string connectionString)
        {
            SqlConnection ret = new SqlConnection(connectionString);
            return ret;
        }

        /// <summary>
        /// Obtiene una nueva instancia de un objeto <c>SqlCommand</c>
        /// </summary>
        /// <param name="connection">Conexión a base de datos</param>
        /// <param name="commandType">Tipo de comando a ejecutar</param>
        /// <returns>La instancia del objeto <c>SqlCommand</c></returns>
        public static SqlCommand GetSqlCommandInstance(SqlConnection connection, CommandType commandType = CommandType.StoredProcedure)
        {
            SqlCommand ret = new SqlCommand();

            ret.Connection = connection;
            ret.CommandType = commandType;
            ret.CommandTimeout = connection.ConnectionTimeout;

            return ret;
        }

        /// <summary>
        /// Obtiene una conexión a la base de datos de Cash 472
        /// </summary>
        /// <returns>El objeto de conexión</returns>
        public static SqlConnection GetCash472DbConnection()
        {
            return GetSqlConnectionInstance(ConfigurationManager.ConnectionStrings["CASH472_DB"].ConnectionString);
        }

        /// <summary>
        /// Obtiene una conexión a la base de datos de Base
        /// </summary>
        /// <returns>El objeto de conexión</returns>
        public static SqlConnection GetBaseDbConnection()
        {
            return GetSqlConnectionInstance(ConfigurationManager.ConnectionStrings["BASEConnectionString"].ConnectionString);
        }

        /// <summary>
        /// Obtiene una conexión a la base de datos de Base
        /// </summary>
        /// <returns>El objeto de conexión</returns>
        public static SqlConnection GetKinacuDbConnection()
        {
            return GetSqlConnectionInstance(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
        }
    }
}
