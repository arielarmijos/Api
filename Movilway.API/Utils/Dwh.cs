// <copyright file="Dwh.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    using Movilway.Logging;

    /// <summary>
    /// DWH Helper
    /// </summary>
    /// <typeparam name="T">Type of the value to be returned</typeparam>
    internal class Dwh<T> where T : new()
    {
        /// <summary>
        /// Objeto para gestionar el log de acceso a los diferentes metodos
        /// </summary>
        private static readonly ILogger Logger;

        /// <summary>
        /// Initializes static members of the <see cref="Dwh" /> class.
        /// </summary>
        static Dwh()
        {
            try
            {
                Logger = LoggerFactory.GetLogger(typeof(Dwh<T>));
            }
            catch (Exception)
            {
                throw new Exception("No se ha podido iniciar el sistema de loggin");
            }
        }

        /// <summary>
        /// Ejecuta la consulta indicada
        /// </summary>
        /// <param name="connection">Objeto de conexión a base de datos</param>
        /// <param name="query"><c>string</c> que contiene la consulta</param>
        /// <param name="parameters">Parametros para ser agregados al comando de consulta (si la consulta así lo requiere)</param>
        /// <param name="transaction">Objeto de transacción en base de datos</param>
        /// <returns>Número de registros afectados</returns>
        internal static int ExecuteNonQuery(SqlConnection connection, string query, Dictionary<string, object> parameters = null, SqlTransaction transaction = null)
        {
            int ret = 0;

            SqlCommand cmd = new SqlCommand(query, connection);
            AddQueryParams(cmd, parameters);
            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }

            cmd.CommandTimeout = connection.ConnectionTimeout;

            try
            {
                Auditoria(cmd);
                ret = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ret;
        }

        /// <summary>
        /// Ejecuta la consulta indicada y retorna un tipo de dato primitivo
        /// </summary>
        /// <param name="connection">Objeto de conexión a base de datos</param>
        /// <param name="query"><c>string</c> que contiene la consulta</param>
        /// <param name="parameters">Parametros para ser agregados al comando de consulta (si la consulta así lo requiere)</param>
        /// <param name="transaction">Objeto de transacción en base de datos</param>
        /// <returns><c>object</c> primitivo que contiene el resultado de la consulta</returns>
        internal static object ExecuteScalar(SqlConnection connection, string query, Dictionary<string, object> parameters = null, SqlTransaction transaction = null)
        {
            object ret = null;

            SqlCommand cmd = new SqlCommand(query, connection);
            AddQueryParams(cmd, parameters);
            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }

            cmd.CommandTimeout = connection.ConnectionTimeout;

            try
            {
                Auditoria(cmd);
                ret = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ret;
        }

        /// <summary>
        /// Ejecuta la consulta indicada y retorna un tipo de dato complejo
        /// </summary>
        /// <param name="connection">Objeto de conexión a base de datos</param>
        /// <param name="query"><c>string</c> que contiene la consulta</param>
        /// <param name="parameters">Parametros para ser agregados al comando de consulta (si la consulta así lo requiere)</param>
        /// <param name="transaction">Objeto de transacción en base de datos</param>
        /// <returns>Objeto <c>T</c> que contiene el resultado de la consulta</returns>
        internal static T ExecuteSingle(SqlConnection connection, string query, Dictionary<string, object> parameters = null, SqlTransaction transaction = null)
        {
            SqlCommand cmd = new SqlCommand(query, connection);
            AddQueryParams(cmd, parameters);
            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }

            T result = new T();
            cmd.CommandTimeout = connection.ConnectionTimeout;

            SqlDataReader queryResult = null;
            try
            {
                Auditoria(cmd);
                queryResult = cmd.ExecuteReader();
                if (queryResult.HasRows)
                {
                    queryResult.Read();
                    result = GetObject(queryResult);
                }
            }
            catch (SqlException orex)
            {
                throw orex;
            }
            catch (Exception ex)
            {
                Logger.ErrorLow("Funcion ExecuteSingle: " + ex.Message + ": " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                if (queryResult != null)
                {
                    queryResult.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// Ejecuta la consulta indicada y retorna una lista con un tipo de dato complejo
        /// </summary>
        /// <param name="connection">Objeto de conexión a base de datos</param>
        /// <param name="query"><c>string</c> que contiene la consulta</param>
        /// <param name="parameters">Parametros para ser agregados al comando de consulta (si la consulta así lo requiere)</param>
        /// <param name="transaction">Objeto de transacción en base de datos</param>
        /// <returns>Una lista que contiene objetos <c>T</c> que contiene el resultado de la consulta</returns>
        internal static List<T> ExecuteReader(SqlConnection connection, string query, Dictionary<string, object> parameters = null, SqlTransaction transaction = null)
        {
            SqlCommand cmd = new SqlCommand(query, connection);
            AddQueryParams(cmd, parameters);
            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }

            List<T> result = new List<T>();
            cmd.CommandTimeout = connection.ConnectionTimeout;

            SqlDataReader queryResult = null;
            try
            {
                Auditoria(cmd);
                queryResult = cmd.ExecuteReader();
                while (queryResult.Read())
                {
                    result.Add(GetObject(queryResult));
                }
            }
            catch (SqlException orex)
            {
                throw orex;
            }
            catch (Exception ex)
            {
                Logger.ErrorLow("Funcion ExecuteReader: " + ex.Message + ": " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                if (queryResult != null)
                {
                    queryResult.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// Adiciona los parámetros de una consulta al comando proporcionado
        /// </summary>
        /// <param name="cmd">Comando al cuál se agregaran los parámetros</param>
        /// <param name="parameters">Parametros a agregar, nombre => valor</param>
        private static void AddQueryParams(SqlCommand cmd, Dictionary<string, object> parameters)
        {
            if (cmd != null && parameters != null && parameters.Count > 0)
            {
                foreach (KeyValuePair<string, object> param in parameters)
                {
                    SqlParameter qparam = new SqlParameter()
                    {
                        ParameterName = param.Key,
                        Value = param.Value
                    };
                    cmd.Parameters.Add(qparam);
                }
            }
        }

        /// <summary>
        /// Escribe en el archivo de log la excepción indicada
        /// </summary>
        /// <param name="ex">Excepción a ser escrita</param>
        /// <param name="query">Consulta que generó la excepción</param>
        private static void LogException(Exception ex, string query)
        {
            if (ex != null && Logger != null)
            {
                Logger.InfoLow(Environment.NewLine + "========== Begin Error SQL ==========");
                Logger.ExceptionLow("Exception: " + Environment.NewLine + ex.Message);
                Logger.ExceptionLow("InnerException: " + ((ex.InnerException == null || ex.InnerException.Message == null) ? string.Empty : (Environment.NewLine + ex.InnerException.Message)));
                Logger.ExceptionLow("StackTrace: " + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                Logger.ExceptionLow("Query/SQL: " + Environment.NewLine + query);
                Logger.InfoLow(Environment.NewLine + "========== End Error SQL ==========" + Environment.NewLine);
            }
        }

        /// <summary>
        /// Registra la auditoria si esta activada
        /// </summary>
        /// <param name="cmd">Comando que se va a auditar</param>
        private static void Auditoria(SqlCommand cmd)
        {
        }

        /// <summary>
        /// Crea un nuevo objeto <c>T</c> a partir del resultado de una consulta
        /// </summary>
        /// <param name="reader">Resultado de la consulta</param>
        /// <returns>Un objecto <c>T</c> que contiene el resultado de la consulta</returns>
        private static T GetObject(SqlDataReader reader)
        {
            T objeto = new T();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                SetValue(reader.GetName(i), reader.GetValue(i), ref objeto);
            }

            return objeto;
        }

        /// <summary>
        /// Establece el valor de un objeto de acuerdo al tipo de dato
        /// </summary>
        /// <param name="fieldName">Nombre del objeto</param>
        /// <param name="fieldValue">Valor del objeto</param>
        /// <param name="objeto">Objeto que contendrá el valor leído</param>
        private static void SetValue(string fieldName, object fieldValue, ref T objeto)
        {
            if (!string.IsNullOrEmpty(fieldName))
            {
                if (fieldValue.GetType().Name.Equals("DBNull"))
                {
                    fieldValue = null;
                }

                PropertyInfo pi = typeof(T).GetProperty(fieldName);
                if (pi != null)
                {
                    try
                    {
                        SetValue(pi, fieldValue, ref objeto);
                    }
                    catch (Exception ex)
                    {
                        Logger.ErrorLow("Funcion privada SetValue: " + ex.Message + ": " + ex.StackTrace);
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Establece el valor de un objeto de acuerdo al tipo de dato
        /// </summary>
        /// <param name="pi">Información de la propiedad</param>
        /// <param name="fieldValue">Valor del objeto</param>
        /// <param name="objeto">Objeto que contendrá el valor leído</param>
        private static void SetValue(PropertyInfo pi, object fieldValue, ref T objeto)
        {
            if (fieldValue == null || fieldValue.GetType() == null)
            {
                pi.SetValue(objeto, null, null);
            }
            else if (pi.PropertyType == typeof(int))
            {
                pi.SetValue(objeto, Convert.ToInt32(fieldValue.ToString()), null);
            }
            else if (pi.PropertyType == typeof(string))
            {
                pi.SetValue(objeto, Convert.ToString(fieldValue.ToString()), null);
            }
            else if (pi.PropertyType == typeof(DateTime))
            {
                string fieldValueType = fieldValue.GetType().FullName;
                switch (fieldValueType)
                {
                    case "System.String":
                        pi.SetValue(objeto, DateTime.ParseExact(fieldValue.ToString(), "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), null);
                        break;
                    case "System.DateTime":
                        pi.SetValue(objeto, fieldValue, null);
                        break;
                    default:
                        pi.SetValue(objeto, DateTime.ParseExact(fieldValue.ToString(), "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), null);
                        break;
                }
            }
            else if (pi.PropertyType == typeof(DateTimeOffset))
            {
                string fieldValueType = fieldValue.GetType().FullName;
                switch (fieldValueType)
                {
                    case "System.String":
                        pi.SetValue(objeto, DateTimeOffset.ParseExact(fieldValue.ToString(), "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), null);
                        break;
                    case "System.DateTimeOffset":
                        pi.SetValue(objeto, fieldValue, null);
                        break;
                    default:
                        pi.SetValue(objeto, DateTimeOffset.ParseExact(fieldValue.ToString(), "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), null);
                        break;
                }
            }
            else if (pi.PropertyType == typeof(decimal))
            {
                pi.SetValue(objeto, Convert.ToDecimal(fieldValue.ToString()), null);
            }
            else if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (Nullable.GetUnderlyingType(pi.PropertyType) == typeof(int))
                {
                    pi.SetValue(objeto, (int?)Convert.ToInt32(fieldValue.ToString()), null);
                }
                else if (Nullable.GetUnderlyingType(pi.PropertyType) == typeof(long))
                {
                    pi.SetValue(objeto, (long?)Convert.ToInt64(fieldValue.ToString()), null);
                }
                else if (Nullable.GetUnderlyingType(pi.PropertyType) == typeof(decimal))
                {
                    pi.SetValue(objeto, (decimal?)Convert.ToDecimal(fieldValue.ToString()), null);
                }
                else if (Nullable.GetUnderlyingType(pi.PropertyType) == typeof(DateTime))
                {
                    string fieldValueType = fieldValue.GetType().FullName;
                    switch (fieldValueType)
                    {
                        case "System.String":
                            pi.SetValue(objeto, (DateTime?)DateTime.ParseExact(fieldValue.ToString(), "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), null);
                            break;
                        case "System.DateTime":
                            pi.SetValue(objeto, (DateTime?)((DateTime)fieldValue), null);
                            break;
                        default:
                            pi.SetValue(objeto, (DateTime?)DateTime.ParseExact(fieldValue.ToString(), "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), null);
                            break;
                    }
                }
                else if (Nullable.GetUnderlyingType(pi.PropertyType) == typeof(DateTimeOffset))
                {
                    string fieldValueType = fieldValue.GetType().FullName;
                    switch (fieldValueType)
                    {
                        case "System.String":
                            pi.SetValue(objeto, (DateTimeOffset?)DateTimeOffset.ParseExact(fieldValue.ToString(), "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), null);
                            break;
                        case "System.DateTimeOffset":
                            pi.SetValue(objeto, (DateTimeOffset?)((DateTimeOffset)fieldValue), null);
                            break;
                        default:
                            pi.SetValue(objeto, (DateTimeOffset?)DateTimeOffset.ParseExact(fieldValue.ToString(), "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), null);
                            break;
                    }
                }
            }
            else if (pi.PropertyType == typeof(double))
            {
                pi.SetValue(objeto, Convert.ToDouble(fieldValue.ToString()), null);
            }
            else if (pi.PropertyType == typeof(long))
            {
                pi.SetValue(objeto, Convert.ToInt64(fieldValue.ToString()), null);
            }
            else if (pi.PropertyType == typeof(short))
            {
                pi.SetValue(objeto, Convert.ToInt16(fieldValue.ToString()), null);
            }
            else if (pi.PropertyType == typeof(byte))
            {
                pi.SetValue(objeto, Convert.ToByte(fieldValue.ToString()), null);
            }
            else if (pi.PropertyType == typeof(float))
            {
                pi.SetValue(objeto, (float)Convert.ToDouble(fieldValue.ToString()), null);
            }
            else
            {
                pi.SetValue(objeto, fieldValue, null);
            }
        }
    }
}
