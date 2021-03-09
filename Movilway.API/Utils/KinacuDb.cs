// <copyright file="Database.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Web;

    using Logging;
    using Models.Kinacu;

    /// <summary>
    /// Clase generica de utilidades con respecto a base de datos de Kinacu
    /// </summary>
    internal class KinacuDb
    {
        /// <summary>
        /// Objeto conexion db
        /// </summary>
        public SqlConnection db;

        /// <summary>
        /// Objeto para realizar transacciones
        /// </summary>
        public SqlTransaction databaseTransaction;

        /// <summary>
        /// Objeto para gestionar el log de acceso a los diferentes metodos
        /// </summary>
        private static readonly ILogger Logger;

        /// <summary>
        /// Initializes static members of the <see cref="KinacuDb" /> class.
        /// </summary>
        static KinacuDb()
        {
            try
            {
                Logger = LoggerFactory.GetLogger(typeof(KinacuDb));
            }
            catch (Exception)
            {
                throw new Exception("No se ha podido iniciar el sistema de loggin");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinacuDb" /> class.
        /// </summary>
        public KinacuDb()
        {
            this.db = null;
            this.databaseTransaction = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinacuDb" /> class.
        /// </summary>
        /// <param name="db">Database connection</param>
        /// <param name="databaseTransaction">Database transaction</param>
        public KinacuDb(SqlConnection db, SqlTransaction databaseTransaction = null) : base()
        {
            this.db = db;
            this.databaseTransaction = databaseTransaction;
        }

        /// <summary>
        /// Actualiza la secuencia para el parametro dado
        /// </summary>
        /// <param name="idsecuencia">Tipo de secuencia a actualizar</param>
        /// <returns>ID de la secuencia generada</returns>
        public int UpdateSecuencia(string idsecuencia)
        {
            int ret = 0;
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

            try
            {
                ret = Convert.ToInt32(Dwh<int>.ExecuteScalar(
                    this.db,
                    Queries.Kinacu.UpdateSequence,
                    new Dictionary<string, object>()
                    {
                        { "@paramValue", idsecuencia }
                    },
                    this.databaseTransaction));
            }
            catch (Exception ex)
            {
                Logger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Exception(ex));
            }

            return ret;
        }

        /// <summary>
        /// Inserta un movimiento de auditoria en las tablas de Kinacu
        /// </summary>
        /// <param name="idUsuario">ID usuario</param>
        /// <param name="idUsuarioSup">ID usuario superior</param>
        /// <param name="comentario">Comentario auditoria</param>
        /// <param name="codigo">Código que relaciona la auditoria con el registro</param>
        /// <param name="dominio">Tipo de auditoria</param>
        /// <param name="subdominio">Subtipo de auditoria</param>
        /// <param name="code">Codigo error de retorno</param>
        /// <returns>ID del registro de auditoria creado</returns>
        public int InsertarMovimientoAuditoria(int idUsuario, int? idUsuarioSup, string comentario, int codigo, string dominio, string subdominio)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

            int ret = 0;

            int idMovimiento = this.UpdateSecuencia(Core.cons.SecuenciaAuditoria);
            if (idMovimiento == 0)
            {
                Logger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName));
                return ret;
            }

            Logger.CheckPointHigh(() => TagValue.New().MethodName(methodName).Message("INSERT INTO KcrTransaccion Start"));

            try
            {
                object usrIdSuperior = DBNull.Value;
                if (idUsuarioSup.HasValue)
                {
                    usrIdSuperior = idUsuarioSup.Value;
                }

                Dwh<int>.ExecuteNonQuery(
                    this.db,
                    Queries.Kinacu.InsertAuditoria,
                    new Dictionary<string, object>()
                    {
                        { "@traId", idMovimiento },
                        { "@usrId", idUsuario },
                        { "@traComentario", comentario },
                        { "@usrIdSuperior", usrIdSuperior },
                        { "@iCodigo", codigo },
                        { "@traDominio", dominio },
                        { "@traSubdominio", subdominio }
                    },
                    this.databaseTransaction);
                ret = idMovimiento;
            }
            catch (Exception ex)
            {
                Logger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Exception(ex));
            }

            Logger.CheckPointHigh(() => TagValue.New().MethodName(methodName).Message("INSERT INTO KcrTransaccion End"));
            return ret;
        }

        /// <summary>
        /// Obtiene la información del depósito pendiente de BD
        /// </summary>
        /// <param name="id">ID del depósito</param>
        /// <returns>Un objeto <c>InfoDeposito</c> que contiene la información del depósito</returns>
        public InfoDeposito GetInfoDepositoPendiente(int id)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

            InfoDeposito ret = null;

            try
            {
                ret = Dwh<InfoDeposito>.ExecuteSingle(
                    this.db,
                    Queries.Kinacu.GetInfoDeposito,
                    new Dictionary<string, object>()
                    {
                        { "@id", id },
                        { "@estado", "PE" }
                    },
                    this.databaseTransaction);
            }
            catch (Exception ex)
            {
                Logger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Exception(ex));
            }

            if (ret.Id == 0)
            {
                ret = null;
            }

            return ret;
        }

        /// <summary>
        /// Obtiene la información de una cuenta corriente
        /// </summary>
        /// <param name="agenciaId">ID de la agencia</param>
        /// <returns>Un objeto <c>InfoCuentaCorriente</c> que contiene la información de la cuenta corriente</returns>
        public InfoCuentaCorriente GetInfoCuentaCorriente(int agenciaId)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

            InfoCuentaCorriente ret = null;

            try
            {
                ret = Dwh<InfoCuentaCorriente>.ExecuteSingle(
                    this.db,
                    Queries.Kinacu.GetInfoCuentaCorriente,
                    new Dictionary<string, object>()
                    {
                        { "@id", agenciaId }
                    },
                    this.databaseTransaction);
            }
            catch (Exception ex)
            {
                Logger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Exception(ex));
            }

            if (ret.Id == 0)
            {
                ret = null;
            }

            return ret;
        }

        /// <summary>
        /// Obtiene la información del depósito pendiente de BD
        /// </summary>
        /// <param name="id">ID del depósito</param>
        /// <returns>Un objeto <c>InfoDeposito</c> que contiene la información del depósito</returns>
        public InfoAgente GetInfoAgente(int? id, string login = null)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

            InfoAgente ret = null;

            try
            {
                string query = string.Empty;
                object idParam = DBNull.Value;
                object loginParam = DBNull.Value;

                if (id != null && id.HasValue)
                {
                    query = Queries.Kinacu.GetInfoAgenteById;
                    idParam = id.Value;
                }
                else
                {
                    query = Queries.Kinacu.GetInfoAgenteByLogin;
                }

                if (!string.IsNullOrEmpty(login))
                {
                    loginParam = login;
                }

                ret = Dwh<InfoAgente>.ExecuteSingle(
                    this.db,
                    query,
                    new Dictionary<string, object>()
                    {
                        { "@id", idParam },
                        { "@login", loginParam }
                    },
                    this.databaseTransaction);
            }
            catch (Exception ex)
            {
                Logger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Exception(ex));
            }

            if (ret.BranchId == 0)
            {
                ret = null;
            }

            return ret;
        }

        /// <summary>
        /// Crea un movimiento de cuenta corriente con los datos especificados
        /// </summary>
        /// <param name="idAgencia">Id de la agencia</param>
        /// <param name="monto">Monto del movimiento</param>
        /// <param name="comentario">Comentario de creación</param>
        /// <param name="asignacionProducto">Es una asignación sí/no</param>
        /// <param name="ttrId">ID tipo de movimiento cuenta corriente</param>
        /// <returns>Un <c>int</c> que contiene el ID de la transacción creada en <c>KfnCuentaCorrienteMovimiento</c></returns>
        public int CrearMovimientoCuentaCorriente(int idAgencia, decimal monto, string comentario, bool asignacionProducto, int ttrId = 1000)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

            int ret = 0;

            InfoCuentaCorriente cuentaCorriente = this.GetInfoCuentaCorriente(idAgencia);
            if (cuentaCorriente == null)
            {
                return ret;
            }

            try
            {
                Dictionary<string, object> qp = new Dictionary<string, object>()
                {
                    { "@ctaId", cuentaCorriente.Id },
                    { "@ageId", idAgencia },
                    { "@ctaSaldo", cuentaCorriente.Saldo },
                    { "@depMonto", monto },
                    { "@limiteCredito", cuentaCorriente.LimiteCredito }
                };

                if (Dwh<int>.ExecuteNonQuery(this.db, asignacionProducto ? Queries.Kinacu.UpdateCuentaCorrienteRestaLimiteCredito : Queries.Kinacu.UpdateCuentaCorriente, qp, this.databaseTransaction) == 0)
                {
                    Logger.ErrorHigh(() => TagValue.New()
                        .MethodName(methodName)
                        .Message("Parametros")
                        .Tag("CtaId").Value(cuentaCorriente.Id)
                        .Tag("AgeId").Value(idAgencia)
                        .Tag("CtaSaldo").Value(cuentaCorriente.Saldo.ToString(CultureInfo.InvariantCulture))
                        .Tag("Monto").Value(monto.ToString(CultureInfo.InvariantCulture))
                        .Tag("LimiteCredito").Value(cuentaCorriente.LimiteCredito.ToString(CultureInfo.InvariantCulture)));
                    return ret;
                }
            }
            catch (Exception ex)
            {
                Logger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Exception(ex));
                return ret;
            }

            int idMovimientoCtaCorriente = this.UpdateSecuencia(Core.cons.SecuenciaMovimientoCuentaCorriente);
            if (idMovimientoCtaCorriente == 0)
            {
                Logger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("NO SE PUDO LEER LA SECUENCIA DEL CTACTEMOVIMIENTO"));
                return ret;
            }

            Logger.CheckPointHigh(() => TagValue.New().MethodName(methodName).Message("INSERT INTO KfnCuentaCorrienteMovimiento Start"));

            try
            {
                if (asignacionProducto)
                {
                    cuentaCorriente = this.GetInfoCuentaCorriente(idAgencia);
                }

                if (Dwh<int>.ExecuteNonQuery(
                    this.db,
                    Queries.Kinacu.InsertMovimientoCuentaCorriente,
                    new Dictionary<string, object>()
                    {
                        { "@traId", idMovimientoCtaCorriente },
                        { "@ctaId", cuentaCorriente.Id },
                        { "@ctaSaldo", asignacionProducto ? (cuentaCorriente.Saldo + monto) : cuentaCorriente.Saldo },
                        { "@depMonto", asignacionProducto ? (monto * -1) : monto },
                        { "@sComentario", comentario },
                        { "@ttrId", ttrId }
                    },
                    this.databaseTransaction) == 0)
                {
                    throw new Exception();
                }

                ret = idMovimientoCtaCorriente;
            }
            catch (Exception ex)
            {
                Logger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Exception(ex));
                return ret;
            }

            Logger.CheckPointHigh(() => TagValue.New().MethodName(methodName).Message("INSERT INTO KfnCuentaCorrienteMovimiento End"));
            return ret;
        }
    }
}
