using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Web;
using Movilway.API.Core;

namespace Movilway.API.Service.ExtendedApi.Provider.IBank
{
 

    public class ExecuteDistributionResponse
    {
        public string responseCode { get; set; }
        public string Message { get; set; }
        public decimal newBalance { get; set; }
        public string authorizationNumber { get; set; }
    }

    public class Utils
    {
        private string LOG_PREFIX = HttpContext.Current.Session["LOG_PREFIX"].ToString();
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(Utils));




        //protected override ILogger ProviderLogger { get { return logger; } }
        /// <summary>
        /// realiza un traslado b2b
        /// </summary>
        /// <param name="affectCtaCte">Indica si la operacion es un reversion con las respectivas reglas</param>
        /// <param name="amount">si es una reversion el monto es negativo</param>
        /// <returns></returns>
        public bool transferb2b(string agent_reciver, string accessReceiver, string cuenta, int usr_id, double amount, string reference_number, DateTime date, bool affectCtaCte, ref string autorization, ref string Message, ref string Response_Code) //, ref decimal newAmount, bool? partialCharge = false)
        {
            //destomn
          
            //se mapearan en un anonymous type de tipo
         
         
            //agente que recibe la transferencia
            decimal age_id = decimal.Parse(agent_reciver);
            string age_estado = string.Empty;
            string AgenteDestino = string.Empty;
            string UsuarioDestino = string.Empty;

           //agente que realiza la recarga
           // decimal IdAgenteSolicitante = 0;
            decimal Agente = 0;
            string AgenteOrigen = string.Empty;
     
            //cuenta para realizar la transferencia
            decimal cubId = 0;
            string cubNumero = string.Empty;

            //cuenta corriente para recibir la transferencia
            decimal ctaSaldo= 0m;
            decimal ctaId = 0m;

             int dateDiff = 0;

            decimal IdBodegaOrigen = 0;
            decimal IdBodegaDestino = 0; 
            
            int traIdReferencia;
            int sec_number_audit;
            int sec_number_trans;
            decimal ctaIdCorriente = 0;
            int sec_number_ctacte_mov;

            decimal ult_sol_pro;
            int sec_number;
            decimal ult_env_pro;

            decimal StockIdOrigen;
            decimal StockIdDestino;

            decimal sec_number_auditoria = 0m;
            int sec_number_aut;
            int sec_numbercta_movi;

            //CANTIDAD DEL CLIENTE ACTUALIZADA
            decimal stkCantidad;

            //CANTIDAD EN STOCK PARA VALIDAR
            decimal saldoOrigen = 0;
            decimal saldoDestino = 0;
            
            // varaible auxiliar para operaciones boleanas
            bool bandera = false;

            //variable auxiliar para concatenar el mensaje de error
            //cunado se utiliza TagValue
            string  _m = "";

            string sentencia = string.Empty;

            autorization = "";

            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            SqlConnection mySqlConnection = new SqlConnection(strConnString);
            SqlCommand mySqlCommand = new SqlCommand();
            SqlDataReader reader = null;
            SqlTransaction tran = null;

            try
            {
                mySqlConnection.Open();
                tran = mySqlConnection.BeginTransaction();
                mySqlCommand.Transaction = tran;
                mySqlCommand.Connection = mySqlConnection;


                try
                {
                    //mySqlCommand.CommandText = "SELECT  CAST(par_valor as int) FROM [Parametro] WITH(NOLOCK) WHERE par_id='TimeDifference'";
                    mySqlCommand.CommandText = "SELECT  CAST(par_valor as int) FROM [Parametro] WITH(NOLOCK) WHERE par_id = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "TimeDifference");
                    dateDiff = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Message = "ERROR AL SELECCIONAR PARAMETRO TimeDifference";
                    Response_Code = "01";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                ///DATOS DEL AGENTE DESTINO LEER DATOS POR READER

                Response_Code = "02";
                try
                {
                    #region proceso anterior
                    /*
                try
                {
                    // mySqlCommand.CommandText = "SELECT age_nombre FROM agente WITH(NOLOCK) WHERE age_id = " + age_id + "";
                    mySqlCommand.CommandText = "SELECT age_nombre FROM agente WITH(NOLOCK) WHERE age_id = @age_id ";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    AgenteDestino = mySqlCommand.ExecuteScalar().ToString();

                    if (AgenteDestino.Length == 0)
                    {
                        Message = "PDV NO SE ENCONTRO O AGENCIA NO PARTICIPA";
                        logger.ErrorLow(String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message, "."));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Message = "ERROR CONSULTADO EL NOMBRE DEL AGENTE";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { age_id = age_id }));
                    tran.Rollback();
                    return false;
                }

                Response_Code = "03";
                try
                {
                    //mySqlCommand.CommandText = "SELECT age_estado FROM agente WITH(NOLOCK) WHERE age_id = " + age_id + "";
                    mySqlCommand.CommandText = "SELECT age_estado FROM agente WITH(NOLOCK) WHERE age_id = @age_id ";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    age_estado = mySqlCommand.ExecuteScalar().ToString();

                    if (age_estado == "SU" || age_estado == "DE")
                    {
                        Message = "AGENCIA DESTINO SUSPENDIDA O INACTIVA;" + AgenteDestino;
                   
                        logger.ErrorLow(String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message, "."));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Response_Code = "03";
                    Message = "ERROR CONSULTANDO ESTADO DE AGENCIA";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { AgenteDestino = AgenteDestino }));
                    tran.Rollback();
                    return false;
                }


                //Obtenemos el nombre del usuario al que se le hace la transferencia

                //se incluye en los datos del agente que recibe la transferencia por lo que dado que antes se  realiza la validacion por id o por login u 
                if (!String.IsNullOrEmpty(accessReceiver))
                {
                    try
                    {
           
                        //mySqlCommand.CommandText = "SELECT u.[usr_nombre] FROM Acceso ac with (NOLOCK) JOIN Usuario u with (NOLOCK) on u.usr_id=ac.usr_id WHERE ac.acc_login='" + accessReceiver + "'";
                        mySqlCommand.CommandText = "SELECT u.[usr_nombre] FROM Acceso ac with (NOLOCK) JOIN Usuario u with (NOLOCK) on u.usr_id=ac.usr_id WHERE ac.acc_login = @accessReceiver";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@accessReceiver", accessReceiver);
                        UsuarioDestino = mySqlCommand.ExecuteScalar().ToString();
                    }
                    catch (Exception ex)
                    {
                        Response_Code = "06";
                        Message = "";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { accessReceiver = accessReceiver }));
                        tran.Rollback();
                        return false;
                    }
                }*/
                    #endregion
                    
                    if (!String.IsNullOrEmpty(accessReceiver))
                    {
                        bandera = true;
                        //query que incluye el nombre del usuario destinatario
                        mySqlCommand.CommandText = "SELECT  ag.[age_nombre], ag.age_estado,  u.[usr_nombre] FROM Acceso ac with (NOLOCK) JOIN Usuario u with (NOLOCK) on u.usr_id=ac.usr_id JOIN [dbo].[Agente] ag with (NOLOCK)  on u.usr_id=ag.usr_id   WHERE ag.[age_id] =  @age_id ";
                    }
                    else
                    {
                        //query sin el nombre del usuario
                        mySqlCommand.CommandText = "SELECT ag.[age_nombre], ag.age_estado FROM [dbo].[Agente] ag  WHERE ag.[age_id] =  @age_id ";
                    }

                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    using (reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                     
                             AgenteDestino  = reader["age_nombre"].ToString();
                            age_estado  =  reader["age_estado"].ToString();   
                            UsuarioDestino = bandera ?reader["usr_nombre"].ToString():"";                      
                           
                        }
                        else
                        {
                            Message = "NO SE ENCONTRO DATOS DEL AGENTE DESTINO CON EL ID INDICADO";
                          
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            if (reader != null && !reader.IsClosed)
                            { reader.Close(); }
                            tran.Rollback();
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Message = "ERROR CONSULTADO DATOS DEL AGENTE DESTINO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                ///


                ///

                //Validamos el saldo del pdv que va a realizar el pago

                ///DATOS DEL AGENTE QUE REALIZA LA TRANSFERENCIA
                Response_Code = "03";
                try
                {
                    #region proceso anterior
                        /*
                    try
                    {
                        //mySqlCommand.CommandText = "SELECT TOP 1 a.age_id  FROM [Agente] a WITH(NOLOCK) JOIN [Usuario] u WITH(NOLOCK) on a.age_id=u.age_id    WHERE u.usr_id in (" + usr_id + ")";
                        mySqlCommand.CommandText = "SELECT a.age_id  FROM [Agente] a WITH(NOLOCK) JOIN [Usuario] u WITH(NOLOCK) on a.age_id=u.age_id WHERE u.usr_id=@usr_id";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                        Agente = (decimal)mySqlCommand.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        Response_Code = "04";
                        Message = "";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { usr_id = usr_id }));
                        tran.Rollback();
                        return false;
                    }

                    // Obtenemos el nombre de la agencia origen
                    try
                    {
                        mySqlCommand.CommandText = "SELECT a.age_nombre FROM [Agente] a WITH(NOLOCK) JOIN [Usuario] u WITH(NOLOCK) on a.age_id=u.age_id WHERE u.usr_id=@usr_id";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                        AgenteOrigen = mySqlCommand.ExecuteScalar().ToString();
                    }
                    catch (Exception ex)
                    {
                        Response_Code = "05";
                        Message = "";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { usr_id = usr_id }));
                        tran.Rollback();
                        return false;
                    }*/
                    #endregion

                    mySqlCommand.CommandText = "SELECT a.age_id, a.age_nombre  FROM [Agente] a WITH(NOLOCK) JOIN [Usuario] u WITH(NOLOCK) on a.age_id=u.age_id WHERE u.usr_id=@usr_id";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                    using (reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                               // AgOrigenData = new AgentData()
                                //{
                                  //  age_id = (int)reader["age_id"],
                            Agente = (decimal)reader["age_id"];
                            AgenteOrigen= reader["age_nombre"].ToString();
                                //};
                        }
                        else
                        {
                            Message = "NO SE ENCONTRO DATOS DEL AGENTE ORIGEN CON EL ID INDICADO";

                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            tran.Rollback();
                            return false;
                        }
                    }
                
                }
                catch (Exception ex)
                {
                    Message = "ERROR CONSULTADO DATOS DEL AGENTE ORIGEN";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                ///

                // RP - OJO con esto, debemos utilizar un if para determinar que saldo es o no insuficiente
                // ARV si el saldo es mayor a cero se esta realizando una transferencia padre hijo
                //if (amount > 0)
                {
                    Response_Code = "04";
                    try
                    {
                        //mySqlCommand.CommandText = "SELECT stkCantidad FROM [KlgStockAgenteProducto] WITH (NOLOCK) WHERE ageId =" + Agente + " AND prdId = 0";

                        mySqlCommand.CommandText = "SELECT stkCantidad FROM [KlgStockAgenteProducto] WITH (NOLOCK) WHERE ageId = @age_id  AND prdId = @prdId";
                        mySqlCommand.Parameters.Clear();
                        // si no es reversion se verifica la cantidad del solicitante, si no se verifica del destino
                        mySqlCommand.Parameters.AddWithValue("@age_id", amount > 0 ? Agente : age_id);
                        mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                        saldoOrigen = (decimal)mySqlCommand.ExecuteScalar();

                        //verificacion para hacer la el trasaldo 
                        if (amount > 0) { 
                            if ((double)saldoOrigen < amount) // && !(partialCharge ?? false))
                            {
                                
                                Message = "NO HAY SALDO SUFICIENTE PARA HACER EL TRASLADO";
                                logger.ErrorLow(() => TagValue.New().Message("NO HAY SALDO SUFICIENTE PARA HACER EL TRASLADO").Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                tran.Rollback();
                                return false;
                            }
                        }
                        //verificacion para hacer la Reversión
                        else
                        {
                            if ((double)saldoOrigen <Math.Abs(amount)) // && !(partialCharge ?? false))
                            {
                                Message = "NO HAY SALDO SUFICIENTE PARA HACER LA REVERSION";

                                logger.ErrorLow(() => TagValue.New().Message("NO HAY SALDO SUFICIENTE PARA HACER LA REVERSION").Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                tran.Rollback();
                                return false;
                            }

                        }
                    }
                    catch (Exception ex)
                    {

                        Message = "ERROR VERFICANDO SALDO DE INVENTARIO DEL AGENTE ORIGEN";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                }
/*
                else if( amount < 0)
                {
                    // si esl saldo es negatvo se esta realizando una devolucion se valida el stock destino
                    Response_Code = "05";
                    try
                    {
                        //mySqlCommand.CommandText = "SELECT stkCantidad FROM [KlgStockAgenteProducto] WITH (NOLOCK) WHERE ageId =" + Agente + " AND prdId = 0";

                        mySqlCommand.CommandText = "SELECT stkCantidad FROM [KlgStockAgenteProducto] WITH (NOLOCK) WHERE ageId = @age_id  AND prdId = @prdId";
                        mySqlCommand.Parameters.Clear();
                        ///TODO2
                        mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                        mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                        saldoDestino = (decimal)mySqlCommand.ExecuteScalar();

                        if ((double)saldoDestino < Math.Abs( amount))
                        {

                            logger.ErrorLow(() => TagValue.New().Message("NO HAY SALDO SUFICIENTE PARA HACER LA REVERSION").Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                            tran.Rollback();
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = "ERROR VERFICANDO SALDO DE INVENTARIO DEL AGENTE DESTINO";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                }*/
                //validacion de stock destino

                //ED: no se debe afectar cuenta corriente en estas distribuciones padre a el que sea.
                if (affectCtaCte)
                {
                    ///DATOS CUENTA CORRIENTE MOVIL WAY

                    #region procesoanterior
                    /*
                    try
                    {
                        //mySqlCommand.CommandText = "SELECT cubId    FROM dbo.KfnCuentaBanco WITH(ROWLOCK)  WHERE cubNumero = '" + cuenta + "'";
                        mySqlCommand.CommandText = "SELECT cubId    FROM dbo.KfnCuentaBanco WITH(ROWLOCK)  WHERE cubNumero = @cuenta";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@cuenta", cuenta);
                        cubId = (decimal)mySqlCommand.ExecuteScalar();

                        //mySqlCommand.CommandText = "SELECT cubNumero    FROM dbo.KfnCuentaBanco WITH(ROWLOCK)  WHERE cubNumero = '" + cuenta + "'";

                        mySqlCommand.CommandText = "SELECT cubNumero FROM dbo.KfnCuentaBanco WITH(ROWLOCK) WHERE cubNumero = @cuenta";
                        //los mismos parametros
   
                        cubNumero = mySqlCommand.ExecuteScalar().ToString();

                        if (cubNumero.Trim() != cuenta.Trim())
                        {
                            throw new Exception("CUENTA NO EXISTE");
                        }
                    }
                    catch (Exception ex)
                    {
                        Response_Code = "08";
                        Message = "ERROR BSUCANDO CUENTA";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { cuenta = cuenta }));
                        tran.Rollback();
                        return false;
                    }
                     * */
                    #endregion
                    Response_Code = "06";
                    try
                    {
                        //mySqlCommand.CommandText = "SELECT cubId    FROM dbo.KfnCuentaBanco WITH(ROWLOCK)  WHERE cubNumero = '" + cuenta + "'";
                        mySqlCommand.CommandText = "SELECT cubId  , cubNumero  FROM dbo.KfnCuentaBanco WITH(ROWLOCK)  WHERE cubNumero = @cuenta";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@cuenta", cuenta);


                        using (reader = mySqlCommand.ExecuteReader())
                        {
                            if (reader.HasRows && reader.Read())
                            {
                               cubId = Convert.ToInt32( reader["cubId"]);
                               cubNumero = reader["cubNumero"].ToString();
                            }
                            else
                            {
                                Message = "NO SE ENCONTRO DATOS DE LA CUENTA DE BANCO";
                                _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                                logger.ErrorLow(()=> TagValue.New().Message(_m).Tag("[INPUT]").Value(new {cuenta = cuenta}));
                                if (reader != null && !reader.IsClosed)
                                { reader.Close(); }                                  
                                tran.Rollback();
                                return false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                     
                        Message = "ERROR BSUCANDO DATOS CUENTA BANCO";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { cuenta = cuenta }));                         
                        tran.Rollback();
                        return false;
                    }
                 
                    ///

                  
                    #region proceso anterior
                    /*
                    try
                    {
                        //mySqlCommand.CommandText = "SELECT  ctaId    FROM KfnCuentaCorriente  WITH(ROWLOCK) WHERE ageId =  " + age_id;

                        mySqlCommand.CommandText = "SELECT  ctaId  FROM KfnCuentaCorriente  WITH(ROWLOCK) WHERE ageId = @age_id";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                        ctaId = (decimal)mySqlCommand.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        Response_Code = "09";
                        Message = "";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { age_id = age_id }));
                        tran.Rollback();
                        return false;
                    }

                    try
                    {
                        // mySqlCommand.CommandText = "SELECT  ctaSaldo      FROM KfnCuentaCorriente  WITH(ROWLOCK) WHERE ageId =  " + age_id;

                        mySqlCommand.CommandText = "SELECT  ctaSaldo      FROM KfnCuentaCorriente  WITH(ROWLOCK) WHERE ageId = @age_id";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@ageId", age_id);
                        ctaSaldo = (decimal)mySqlCommand.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        Response_Code = "10";
                        Message = "";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { age_id = age_id }));
                        tran.Rollback();
                        return false;
                    }
                     * */
                    #endregion
                    ///cuenta origen
                    Response_Code = "07";
                    try
                    {
                        // mySqlCommand.CommandText = "SELECT  ctaSaldo      FROM KfnCuentaCorriente  WITH(ROWLOCK) WHERE ageId =  " + age_id;

                        mySqlCommand.CommandText = "SELECT ctaId, ctaSaldo FROM KfnCuentaCorriente  WITH(ROWLOCK) WHERE ageId = @age_id";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@age_id",age_id);

                        using (reader = mySqlCommand.ExecuteReader())
                        {
                            if (reader.HasRows && reader.Read())
                            {

                               ctaId = Convert.ToInt32(reader["ctaId"]);
                              ctaSaldo =Convert.ToDecimal( reader["ctaSaldo"]);
                            }
                            else
                            {
                                Message = "NO SE ENCONTRO DATOS DEL AGENTE ORIGEN CON EL ID INDICADO";
                                _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                                logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(age_id));
                                tran.Rollback();
                                return false;
                            }
                        }
                      
                    }
                    catch (Exception ex)
                    {
                        Message = "ERROR SELECCIONANDO DATOS DE CUENTA CORRIENTE";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(age_id));
                        tran.Rollback();
                        return false;
                    }
                 
                
               
                    ///
                }
                /*FIN ED*/

                ///Proceso de distribucion 
               
                // RP - OJO, este query no lo ejecutemos de nuevo, copiemos los valores previamente consultados
                //QUITAR QUERY COLOCAR DATOS DE OBJETOS MAPEADOS
                /*
                try
                {
                    mySqlCommand.CommandText = "SELECT a.age_id  FROM [Agente] a WITH(NOLOCK) JOIN [Usuario] u WITH(NOLOCK) on a.age_id=u.age_id    WHERE u.usr_id = @usr_id ";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                    IdAgenteSolicitante = (decimal)mySqlCommand.ExecuteScalar();

                    IdBodegaOrigen = IdAgenteSolicitante;
                    IdBodegaDestino = age_id;
                }
                catch (Exception ex)
                {
                    Response_Code = "11";
                    Message = "";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { usr_id = usr_id }));
                    tran.Rollback();
                    return false;
                }
                 * */

                IdBodegaOrigen = Agente;
                IdBodegaDestino = age_id;
                ///

                #region proceso comentariado
                //ED: No se requiere depósito en estos casos
                /*
                
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "DEPOSITO");
                mySqlCommand.ExecuteNonQuery();

                
                mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = 'DEPOSITO'";
                traIdReferencia = (int)mySqlCommand.ExecuteScalar();

                
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                mySqlCommand.ExecuteNonQuery();

                
                mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                sec_number_audit = (int)mySqlCommand.ExecuteScalar();

                
                sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                             "VALUES (" + sec_number_audit.ToString() + "," + usr_id.ToString() + ", null, 'Ingreso de Deposito por el valor : " + amount.ToString() + "'" + ", GETDATE() ," + traIdReferencia.ToString() + ", 'FINANCIERO', 'AVISODEPOSITO')";
                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();

                
                sentencia = "INSERT INTO KfnDeposito (depId,ageIdOrigen,cubId,depComprobante,depFechaComprobante,depFecha,depMonto,depEstado,depComentario,depComentarioProcesamiento,ccmNumeroTransaccion,usrId,processingUsrId)" +
                             "VALUES ( " + traIdReferencia.ToString() + "," + age_id.ToString() + "," + cubId.ToString() + "," + reference_number + ",'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ", getdate()," + amount.ToString().Replace(",", ".") + ", 'PE', 'Deposito enviado desde API-Movilway monto: " + amount.ToString() + "'" + ", null, null," + usr_id.ToString() + ", null)";
                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();

                
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                mySqlCommand.ExecuteNonQuery();

                
                mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                sec_number_trans = (int)mySqlCommand.ExecuteScalar();

                //Leemos la cuenta corriente y su valor
                
                mySqlCommand.CommandText = "SELECT   ctaId  FROM KfnCuentaCorriente  WITH (UPDLOCK, ROWLOCK) 	 WHERE ageId = " + age_id;
                ctaIdCorriente = (decimal)mySqlCommand.ExecuteScalar();

                
                mySqlCommand.CommandText = "SELECT ctaSaldo FROM KfnCuentaCorriente  WITH (UPDLOCK, ROWLOCK) 	WHERE ageId = " + age_id;
                ctaSaldo = (decimal)mySqlCommand.ExecuteScalar();

                
                mySqlCommand.CommandText = "UPDATE KfnCuentaCorriente WITH(ROWLOCK)  SET ctaSaldo = ctaSaldo + " + amount.ToString().Replace(",", ".") + "  WHERE ctaId = " + ctaId.ToString() + " AND ageId = " + age_id.ToString() + "AND ctaSaldo =" + ctaSaldo.ToString().Replace(",", ".");
                mySqlCommand.ExecuteNonQuery();

                
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                mySqlCommand.ExecuteNonQuery();

                
                mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                sec_number_ctacte_mov = (int)mySqlCommand.ExecuteScalar();

                
                sentencia = "INSERT INTO KfnCuentaCorrienteMovimiento (ccmId,ctaId,ccmFecha,ccmImporte  ,ccmSaldo,ccmDetalle,ccmNumeroTransaccion,ttrId)" +
                            "VALUES ( " + sec_number_ctacte_mov.ToString() + "," + ctaId.ToString() + ", GETDATE()" + "," + amount.ToString().Replace(",", ".") + "," + ctaSaldo.ToString().Replace(",", ".") + ", 'Deposito enviado desde API-Movilway  por el monto: " + amount.ToString().Replace(",", ".") + "'" + "," + sec_number_trans + ", 1000)";

                
                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();

                
                sentencia = "UPDATE KfnDeposito SET  depEstado = 'AU', depComentarioProcesamiento = 'Deposito enviado desde API-Movilway por el Monto: " + @amount.ToString().Replace(",", ".") + "'" +
                            ",ccmNumeroTransaccion =" + sec_number_trans + ", processingUsrId =" + usr_id + " WHERE  depId =" + traIdReferencia;

                
                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();

                
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                mySqlCommand.ExecuteNonQuery();

                
                mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                sec_number_audit = (int)mySqlCommand.ExecuteScalar();

                
                sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                            "VALUES ( " + sec_number_audit + "," + usr_id + ", null, 'Deposito enviado desde API-Movilway por el monto " + amount + "'" + "," +
                            "GETDATE(), " + traIdReferencia.ToString() + ", 'FINANCIERO', 'AUTORIZACION')";
                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();
                */
                #endregion

                Response_Code = "08";
                try
                {
                   // mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                    mySqlCommand.CommandText = "UPDATE [secuencia] WITH(ROWLOCK) SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR LA SECUENCIA SOLICITUDPRODUCTO";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                        tran.Rollback();
                        return false;
                    }
                      
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR LA SECUENCIA SOLICITUDPRODUCTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                ///
                Response_Code = "09";
                try
                {
                   // mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                    mySqlCommand.CommandText = "SELECT sec_number FROM [secuencia] WITH(NOLOCK) WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                   // ult_sol_pro = (int)mySqlCommand.ExecuteScalar();
                    sec_number =  (int)mySqlCommand.ExecuteScalar();
                    ult_sol_pro = sec_number;
                }
                catch (Exception ex)
                {
                
                    Message = "ERROR AL SELECCIONAR LA SECUENCIA DE SOLICITUDPRODUCTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }


                #region codigo comentariado
                /*no tiene sentido volver a ejecutar dado el bloqueo al momento de actualizar el valor de la tabla
                
                // RP - OJO, No volver a consultar a la DB y copiar el valor ya consultado
                try
                {
                    mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                    mySqlCommand.Parameters.Clear();
                    sec_number = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "14";
                    Message = "";
                    
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                 
                 */
                #endregion
                ///

                ///
                /* inserto una solicitud y la coloco en estado pendiente */
                Response_Code = "10";
                try
                {
                    //CONSTANTES EN CODIGO
                    sentencia = "INSERT INTO [KlgSolicitudProducto] WITH(ROWLOCK) (sprId,usrId,ageIdSolicitante,ageIdDestinatario,prvIdDestinatario,sprEstado,sprFecha,sprFechaAprobacion,sprImporteSolicitud,sltId,ageIdBodegaOrigen,ageIdBodegaDestino) " +
                                    "VALUES (@ult_sol_pro,@usr_id,@IdAgenteSolicitante,@age_id, @prvIdDestinatario,@sprEstado,@date,@date,@amount,@sltId,@IdBodegaOrigen,@IdBodegaDestino)";
                
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    //entre condiciones diferencia entre el parametro sprImporteSolicitudsltId
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro",ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@usr_id",usr_id);
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteSolicitante", IdBodegaOrigen);
                    mySqlCommand.Parameters.AddWithValue("@age_id", IdBodegaDestino);
                    mySqlCommand.Parameters.AddWithValue("@prvIdDestinatario",DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@sprEstado","AU");
                    mySqlCommand.Parameters.AddWithValue("@date",date);
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaOrigen",  IdBodegaOrigen);
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaDestino", IdBodegaDestino);

                    if (amount >= 0)
                    {
                        mySqlCommand.Parameters.AddWithValue("@amount", amount);
                        mySqlCommand.Parameters.AddWithValue("@sltId",  202);
                    }
                    else
                    {
                        mySqlCommand.Parameters.AddWithValue("@amount", amount * -1);
                        mySqlCommand.Parameters.AddWithValue("@sltId",  205);
                    }
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Message = "ERROR AL CREAR SOLICITUD PRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                ///

                ///
                Response_Code = "11";
                try
                {
                    //sentencia = "INSERT INTO [KlgSolicitudProductoItem] WITH(ROWLOCK) (sprId,prdId,spiCantidadSolicitada,spiCantidadAutorizada,spiPrecioUnitario,spiEstado) VALUES (" + ult_sol_pro + ",0," + amount + "," + amount + ", 1.0000,'PE')";
                    //CONSTANTES EN CODIGO
                    sentencia = "INSERT INTO [KlgSolicitudProductoItem] WITH(ROWLOCK) (sprId,prdId,spiCantidadSolicitada,spiCantidadAutorizada,spiPrecioUnitario,spiEstado)  VALUES (@ult_sol_pro ,@prdId,@amount ,@amount , @spiPrecioUnitario,@spiEstado)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro",ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@spiPrecioUnitario", 1.0000);
                    mySqlCommand.Parameters.AddWithValue("@spiEstado", "AU");
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL CREAR SOLICITUD PRODUCTO ITEM";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { ult_sol_pro = ult_sol_pro, amount = amount }));
                    tran.Rollback();
                    return false;
                }
         
            
                // RP - OJO, en vez de hacer este update mejor insertar como AU en el insert previo .. dejar el update comentado
                #region codigo comentariado
                /*
                try
                {
                    // sentencia = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK) SET spiEstado = 'AU', spiCantidadAutorizada = " + amount + "WHERE sprId =" + ult_sol_pro + " AND prdId = 0 AND spiEstado = 'PE'";
                    sentencia = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK) SET spiEstado = 'AU', spiCantidadAutorizada = @amount WHERE sprId = @ult_sol_pro  AND prdId = 0 AND spiEstado = 'PE'";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                        throw new Exception("NO SE PUDO ACTUALIZAR SOLICITUD PRODUCTO IT");
                }
                catch (Exception ex)
                {
                    Response_Code = "17";
                    Message = "";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { ult_sol_pro = ult_sol_pro, amount = amount }));
                    tran.Rollback();
                    return false;
                }*/
                #endregion

                ///
                Response_Code = "12";
                try
                {
                    //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "ENVIOPRODUCTO");
                    mySqlCommand.CommandText = "UPDATE [secuencia] WITH(ROWLOCK) SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "ENVIOPRODUCTO");
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR SECUENCIA ENVIOPRODUCTO";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                        tran.Rollback();
                        return false;

                    }
                       
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR LA SECUENCIA ENVIO PRODUCTO";
                    logger.ErrorLow( String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }


                Response_Code = "13";
                try
                {
                   // mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "ENVIOPRODUCTO");
                    mySqlCommand.CommandText = "SELECT sec_number FROM [secuencia] WITH(NOLOCK) WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "ENVIOPRODUCTO");
                    ult_env_pro = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL SELECCIONAR LA SECUENCIA ENVIO PRODUCTO";
                    logger.ErrorLow( String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                
                //sentencia = "INSERT INTO [KlgEnvio] WITH(ROWLOCK) (envId,envEstado,envFechaEnvio,envFechaRecepcion,envNumeroRemito,envNumeroFactura,envObservaciones,ageIdBodegaOrigen,ageIdBodegaDestino) " +"VALUES (" + ult_env_pro + ",'DE','" + date.ToString("yyyy-MM-dd HH:mm:ss") + "',NULL,'','','Envio de productos Virtuales en linea'," + IdBodegaOrigen + "," + IdBodegaDestino + ")";
                Response_Code = "14";
                try
                {
                    sentencia = "INSERT INTO [KlgEnvio] WITH(ROWLOCK) (envId,envEstado,envFechaEnvio,envFechaRecepcion,envNumeroRemito,envNumeroFactura,envObservaciones,ageIdBodegaOrigen,ageIdBodegaDestino) VALUES (@ult_env_pro ,@envEstado,@date,@envFechaRecepcion,@envNumeroRemito,@envNumeroFactura,@envObservaciones,@IdBodegaOrigen , @IdBodegaDestino )";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro",ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@envEstado", "DE");
                    mySqlCommand.Parameters.AddWithValue("@date",date);
                    mySqlCommand.Parameters.AddWithValue("@envFechaRecepcion",DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@envNumeroRemito", reference_number);
                    mySqlCommand.Parameters.AddWithValue("@envNumeroFactura","");
                    mySqlCommand.Parameters.AddWithValue("@envObservaciones", "Envio de productos Virtuales en linea");
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaOrigen",IdBodegaOrigen);
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaDestino", IdBodegaDestino);                 
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL CREAR UN ENVIO";

                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { amount = amount, ult_sol_pro = ult_sol_pro }));
                    tran.Rollback();
                    return false;
                }
            
                try
                {
                    //sentencia = "INSERT INTO [KlgSolicitudProductoEnvio] WITH(ROWLOCK) (sprId,envId) VALUES (" + ult_sol_pro + "," + ult_env_pro + ")";
                    sentencia = "INSERT INTO [KlgSolicitudProductoEnvio] WITH(ROWLOCK) (sprId,envId) VALUES (@ult_sol_pro ,@ult_env_pro )";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = "15";
                    Message = "ERROR AL CREAR SOLICITUD PRODUCTO ENVIO";

                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;

                }
             
                try
                {

                    //sentencia = "INSERT INTO [KlgEnvioItem] WITH(ROWLOCK) (envId,prdId,eitCantidad,eitEstado) VALUES (" + ult_env_pro + "," + 0 + "," + amount.ToString().Replace(",", ".") + ",'DE')";
                    sentencia = "INSERT INTO [KlgEnvioItem] WITH(ROWLOCK) (envId,prdId,eitCantidad,eitEstado) VALUES (@ult_env_pro , @prdId ,@amount ,@eitEstado)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("prdId", 0);
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@eitEstado", "DE");
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = "16";
                    Message = "ERROR AL CREAR ENVIO ITEM";

                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;

                }
                
                // RP - OJO no tocar este, dejarlo así
                Response_Code = "17";
                try
                {
                    //mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = 'DE' WHERE sprId =" + ult_sol_pro + " AND prdId = 0 AND piEstado = 'AU'";
                    mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = @spiEstadoN WHERE sprId = @ult_sol_pro AND prdId = @prdId AND spiEstado = @spiEstado";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@spiEstadoN", "DE");
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    mySqlCommand.Parameters.AddWithValue("@spiEstado", "AU");
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR EL ESTADO DE LA SOLICITUD DE PRODUCTO";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { ult_sol_pro = ult_sol_pro }));
                        tran.Rollback();
                        return false;
                  
                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL CREAR SOLICITUD PRODUCTO IT";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { ult_sol_pro = ult_sol_pro }));
                    tran.Rollback();
                    return false;

                }

                try
                {

                    //mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId =" + IdAgenteSolicitante + " AND prdId IN (0)";
                    mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId = @IdAgenteSolicitante AND prdId = @prdId";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteSolicitante", Agente);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    StockIdOrigen = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "18";
                    Message = "ERROR AL SELECCIONAR STOCK AGENTE PRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { }));
                    tran.Rollback();
                    return false;

                }

                //
                //decimal _newAmount = 0m;
                Response_Code = "19";
                try
                {
                    //mySqlCommand.CommandText = " UPDATE [KlgStockAgenteProducto] WITH(ROWLOCK) SET  stkCantidad = stkCantidad + - " + amount + " WHERE  stkId = " + StockIdOrigen + " AND ageId = " + IdAgenteSolicitante + "and stkCantidad + - " + amount + ">= 0";
                    mySqlCommand.CommandText = " UPDATE [KlgStockAgenteProducto] WITH(ROWLOCK) SET  stkCantidad = stkCantidad + - @amount  WHERE  stkId = @StockIdOrigen AND ageId = @IdAgenteSolicitante and stkCantidad + - @amount >= 0";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@StockIdOrigen", StockIdOrigen);
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteSolicitante", Agente);
                 
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR EL STOCK AGENTE PRODUCTO";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                  
                    Message = "ERROR ALA ACTUALIZAR STOCK AGENTE PRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { amount = amount, StockIdOrigen = StockIdOrigen, IdAgenteSolicitante = Agente }));
                    tran.Rollback();
                    return false;
                }

                Response_Code = "20";
                try
                {
                    //mySqlCommand.CommandText = " UPDATE [KlgEnvio] WITH(ROWLOCK) SET envEstado = 'RE', envFechaRecepcion = GETDATE() WHERE envId = " + ult_env_pro + " AND envEstado = 'DE'";
                    mySqlCommand.CommandText = " UPDATE [KlgEnvio] WITH(ROWLOCK) SET envEstado = @newestado, envFechaRecepcion = dateadd(minute,@dateDiff ,getdate()) WHERE envId = @ult_env_pro AND envEstado = @oldestado";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@dateDiff", dateDiff);
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@oldestado", "DE");
                    mySqlCommand.Parameters.AddWithValue("@newestado", "RE");
                 
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR EL ESTADO DEL  ENVIO";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value( this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    
                    Message = "ERROR AL ACTUALIZAR ENVIO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                Response_Code = "21";
                try
                {
                    //mySqlCommand.CommandText = "UPDATE [KlgEnvioItem] WITH(ROWLOCK) SET eitEstado = 'RE'  WHERE envId = " + ult_env_pro + " AND prdId = 0 AND eitEstado = 'DE'";
                    mySqlCommand.CommandText = "UPDATE [KlgEnvioItem] WITH(ROWLOCK) SET eitEstado = @newestado  WHERE envId = @ult_env_pro  AND prdId = 0 AND eitEstado = @oldestado ";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@oldestado", "DE");
                    mySqlCommand.Parameters.AddWithValue("@newestado", "RE");
                 
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR EL ESTADO DE ENVIO IT";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { ult_sol_pro = ult_sol_pro }));
                        tran.Rollback();
                        return false;
                    }


                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR ENVIO ITEM";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                Response_Code = "22";
                try
                {
                    //mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = 'RE'  WHERE sprId = " + ult_sol_pro + "  AND prdId = 0";
                    mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = @newestado  WHERE sprId = @ult_sol_pro  AND prdId = @prdId";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@newestado", "RE");
                    mySqlCommand.Parameters.AddWithValue("@prdId",0);
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                 
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR EL ESTADO SOLICITUD PRODUCTO IT";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }

                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR SOLICITUD PRODUCTO ITEM";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { ult_sol_pro = ult_sol_pro}));
                    tran.Rollback();
                    return false;
                }
          
                try
                {
                    //mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId =" +age_id + " AND prdId IN (0)";
                    mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId = @age_id  AND prdId = @prdId";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    StockIdDestino = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "23";
                    Message = "ERROR AL SELECCIONAR DATOS STOCK AGENTE PRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                Response_Code = "24";
                try
                {
                    //mySqlCommand.CommandText = "UPDATE KlgStockAgenteProducto SET  stkCantidad = stkCantidad + " + amount + "  WHERE  stkId =" + StockIdDestino + " AND ageId =" + age_id + " and stkCantidad + " + amount + ">= 0 ";
                    mySqlCommand.CommandText = "UPDATE KlgStockAgenteProducto SET  stkCantidad = stkCantidad + @amount  WHERE  stkId = @StockIdDestino AND ageId = @age_id and stkCantidad +  @amount >= 0 ";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@StockIdDestino", StockIdDestino);
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);

                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        //logger.InfoLow(() => TagValue.New().Message(partialCharge.ToString()).Message("[partialCharge]"));

                        //if ((bool)partialCharge)
                        //{
                        //    //Al parecer no hay saldo suficiente, así que consultamos cuánto podemos cobrar
                        //    mySqlCommand.CommandText = "SELECT stkCantidad FROM [KlgStockAgenteProducto] WHERE stkId = @StockIdDestino AND ageId = @age_id";
                        //    mySqlCommand.Parameters.Clear();
                        //    mySqlCommand.Parameters.AddWithValue("@StockIdDestino", StockIdDestino);
                        //    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                        //    _newAmount = (decimal)mySqlCommand.ExecuteScalar();
                        //
                        //    if (_newAmount > 0)
                        //    {
                        //        //Mando a actualizar el stock según el monto que tiene
                        //        mySqlCommand.CommandText = " UPDATE [KlgStockAgenteProducto] WITH(ROWLOCK) SET  stkCantidad = stkCantidad + - @amount  WHERE  stkId = @StockIdOrigen AND ageId = @IdAgenteSolicitante and stkCantidad + - @amount >= 0";
                        //        mySqlCommand.Parameters.Clear();
                        //        mySqlCommand.Parameters.AddWithValue("@amount", _newAmount);
                        //        mySqlCommand.Parameters.AddWithValue("@StockIdOrigen", StockIdOrigen);
                        //        mySqlCommand.Parameters.AddWithValue("@IdAgenteSolicitante", Agente);
                        //
                        //        if (mySqlCommand.ExecuteNonQuery() == 0)
                        //        {
                        //            newAmount = 0m;
                        //            Message = "NO SE PUDO ACTUALIZAR LA CANTDAD DEL STOCK DESTINO AJUSTADO";
                        //            _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        //            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        //            tran.Rollback();
                        //            return false;
                        //        }
                        //        else
                        //        {
                        //            newAmount = _newAmount;
                        //        }
                        //    }
                        //}
                        //else
                        //{
                            Message = "NO SE PUDO ACTUALIZAR LA CANTDAD DEL STOCK DESTINO";
                            _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                            tran.Rollback();
                            return false;
                        //}
                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR STOCK AGENTE PRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                Response_Code = "25";
                try
                {
                    //mySqlCommand.CommandText = " UPDATE KlgSolicitudProducto WITH(ROWLOCK) SET sprEstado = 'CE' WHERE sprId = " + ult_sol_pro + "  AND sprEstado = 'AU' AND ( ageIdSolicitante =" + age_id + " OR ageIdDestinatario =" + age_id + ")";
                    mySqlCommand.CommandText = " UPDATE KlgSolicitudProducto WITH(ROWLOCK) SET sprEstado = @newestado WHERE sprId = @ult_sol_pro  AND sprEstado = @oldestado AND ( ageIdSolicitante = @age_id  OR ageIdDestinatario = @age_id )";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    mySqlCommand.Parameters.AddWithValue("@oldestado", "AU");
                    mySqlCommand.Parameters.AddWithValue("@newestado", "CE");
                
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR EL ESTADO DE SOLICITUD PRODUCTO";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;

                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR SOLICITUD PRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                //Fin de proceso de Acreditacion  

                // Inicio el proceso para registar en cta cte
                Response_Code = "26";
                try
                {
                    //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");

                   
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR LA SECUENCIA AUDITORIA";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;

                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR SECUENCIA DE AUDITORIA";
                 
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                
                try
                {
                    //mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    sec_number_auditoria = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "27";
                    Message = "ERROR AL SELECCIONAR SECUENCIA AUDITORIA";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                // RP - OJO revisar en otros métodos si la DISTRIBUCION y ENVIOPRODUCTO se mapeaban a un parametro para usar el mismo nombre
                try
                {

                    //sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                    //            "VALUES ( " + sec_number_auditoria + ", " + usr_id + ", null, 'Despacho de productos nro." + ult_env_pro + "'" + " , GETDATE()," + ult_env_pro + ",'DISTRIBUCION', 'ENVIOPRODUCTO')";
                    sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio) VALUES ( @sec_number_auditoria , @usr_id , null, @traComentario, dateadd(minute,@dateDiff,getdate()),@ult_env_pro ,@traDominio, @traSubdominio)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@sec_number_auditoria", sec_number_auditoria);
                    mySqlCommand.Parameters.AddWithValue("@usr_id" , usr_id);
                    mySqlCommand.Parameters.AddWithValue("@traComentario", "Despacho de productos nro." + ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@dateDiff",dateDiff);
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@traDominio", "DISTRIBUCION");
                    mySqlCommand.Parameters.AddWithValue("@traSubdominio", "ENVIOPRODUCTO");
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = "28";
                    Message = "ERROR AL CREAR KCR TRANSACCION";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                //ED:no se necesita nuevamente afectar cta ctre
                // --Volvemos a leer el saldo de la cta cte. 
                if (affectCtaCte)
                {
                    /*
                    try 
                    { 
                        //mySqlCommand.CommandText = "SELECT  ctaId   FROM KfnCuentaCorriente  WITH (NOLOCK) WHERE ageId =" + age_id;
                        mySqlCommand.CommandText = "SELECT  ctaId   FROM KfnCuentaCorriente  WITH (NOLOCK) WHERE ageId =@age_id";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                        ctaId = (decimal)mySqlCommand.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        Response_Code = "29";
                        Message = "ERROR AL SELECCIONAR KFN CUENTACORRIENTE";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }

                    try
                    {
                        //mySqlCommand.CommandText = "SELECT  ctaSaldo   FROM KfnCuentaCorriente  WITH (NOLOCK) WHERE ageId =" + age_id;
                        mySqlCommand.CommandText = "SELECT  ctaSaldo   FROM KfnCuentaCorriente  WITH (NOLOCK) WHERE ageId = @age_id";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                        ctaSaldo = (decimal)mySqlCommand.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        Response_Code = "30";
                        Message = "ERROR AL SELECCIONAR KFN CUENTACORRIENTE";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                     * */

                    Response_Code = "31";
                    try
                    {
                        //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                        mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                 
                        if (mySqlCommand.ExecuteNonQuery() == 0)
                        {
                            Message = "NO SE PUDO ACTUALIZAR LA SECUENCIA TRANSACCION";
                         
                            logger.ErrorLow( String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            tran.Rollback();
                            return false;

                        }
                     }
                    catch (Exception ex)
                    {

                        Message = "ERROR AL ACTUALIZAR LA SECUENCIA TRANSACCION";
                        logger.ErrorLow(String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message, ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }
                    Response_Code = "32";
                    try
                    {
                        //mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                        mySqlCommand.CommandText = "SELECT sec_number_aut = sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                        sec_number_aut = (int)mySqlCommand.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {

                        Message = "ERROR AL SELECCIONAR LA SECUENCUA TRANSACCION";
                      
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }

                    // RP ABR 2015 - No es necesario validar límites de cta cte ni montos mayores a cero porque acá el monto es negativo, por lo tanto siempre se sumará
                    Response_Code = "33";
                    try
                    {
                        //mySqlCommand.CommandText = "UPDATE KfnCuentaCorriente WITH(ROWLOCK) SET ctaSaldo = ctaSaldo + - " + amount + " WHERE ctaId =" + ctaId + " AND ageId = " + age_id + " AND ctaSaldo =" + ctaSaldo + " AND ( (ctaSaldo + - " + amount + " >= 0) OR (ABS(ctaSaldo + - " + amount + ") <= 600) )";
                        mySqlCommand.CommandText = "UPDATE KfnCuentaCorriente WITH(ROWLOCK) SET ctaSaldo = ctaSaldo + - @amount  WHERE ctaId = @ctaId  AND ageId = @age_id AND ctaSaldo = @ctaSaldo";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@amount", amount);
                        mySqlCommand.Parameters.AddWithValue("@ctaId", ctaId);
                        mySqlCommand.Parameters.AddWithValue("@age_id", age_id); 
                        mySqlCommand.Parameters.AddWithValue("@ctaSaldo", ctaSaldo);
                        if (mySqlCommand.ExecuteNonQuery() == 0)
                        {
                            Message = "NO SE PUDO ACTUALIZAR EL SALDO DE LA CUENTA CORRIENTE";
                            _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                            tran.Rollback();
                            return false;

                        }
                    }
                    catch (Exception ex)
                    {

                        Message = "ERROR AL ACTUALIZAR KFN CUENTACORRIENTE";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                    
                    try
                    {

                        //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                        mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                    
                        if (mySqlCommand.ExecuteNonQuery() == 0)
                        {
                            Message = "NO SE PUDO ACTUALIZAR LA SECUENCIA CTACTEMOVIMIENTO";
                            _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                            tran.Rollback();
                            return false;

                        }
                    }
                    catch (Exception ex)
                    {
                        Response_Code = "34";
                        Message = "ERROR AL ACTUALIZAR SECUENCIA CTACTEMOVIMIENTO";
                  
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }

                    try
                    {
                        //mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = 'CTACTEMOVIMIENTO'";
                        mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                        sec_numbercta_movi = (int)mySqlCommand.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        Response_Code = "35";
                        Message = "ERROR AL SELECCONAR SECUENCIA CTA CTEMOVIMIENTO";

                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }
                    
                    try
                    {
                        sentencia = "INSERT INTO KfnCuentaCorrienteMovimiento (ccmId,ctaId,ccmFecha,ccmImporte  ,ccmSaldo,ccmDetalle,ccmNumeroTransaccion,ttrId) VALUES ( @sec_numbercta_movi ,@ctaId ,GETDATE()  ,@amount ,@ctaSaldo,@ccmDetalle,@sec_number_aut , @ttrId)";
                        mySqlCommand.CommandText = sentencia;
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@sec_numbercta_movi", sec_numbercta_movi);
                        mySqlCommand.Parameters.AddWithValue("@ctaId", ctaId);
                        mySqlCommand.Parameters.AddWithValue("@amount", amount);
                        mySqlCommand.Parameters.AddWithValue("@ctaSaldo", ctaSaldo);
                        mySqlCommand.Parameters.AddWithValue("@ccmDetalle", "Asignacion de productos No.: " + ult_sol_pro);
                        mySqlCommand.Parameters.AddWithValue("@sec_number_aut", sec_number_aut);
                        mySqlCommand.Parameters.AddWithValue("ttrId", 2000); 
                   
                        mySqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Response_Code = "36";
                        Message = "ERROR AL CREAR MOVIMIENTO CUENTA CORRIENTE";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { sec_numbercta_movi = sec_numbercta_movi, ctaId = ctaId, amount = amount, ctaSaldo = ctaSaldo, sec_number_aut = sec_number_aut }));
                        tran.Rollback();
                        return false;
                    }
                    Response_Code = "37";
                    try{

                            //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                            mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                            mySqlCommand.Parameters.Clear();
                            mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                          
                            if (mySqlCommand.ExecuteNonQuery() == 0)
                            {
                                Message = "NO SE PUDO ACTUALIZAR LA SECUENCIA AUDITORIA";
                                logger.ErrorLow(String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                                tran.Rollback();
                                return false;

                            }
                    }
                    catch (Exception ex)
                    {

                        Message = "ERROR AL ACTUALIZAR SECUENCIA AUDITORIA";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }
                    
                    try
                    {

                        //mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                        mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                        sec_number_auditoria = (int)mySqlCommand.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        Response_Code = "38";
                         Message = "ERROR AL SELECCIONAR SECUENCIA AUDITORIA";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { }));
                        tran.Rollback();
                        return false;
                    }
                    
                    try
                    {
                        //sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" + "VALUES (" + sec_number_auditoria + "," + usr_id + ", null, 'Asignacion de productos',  GETDATE()," + ult_sol_pro + ", 'DISTRIBUCION', 'SOLICITUDPRODUCTO')";

                        sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio) VALUES (@sec_number_auditoria ,@usr_id , null,@traComentario ,  GETDATE(), @ult_sol_pro ,@traDominio, @traSubdominio)";

                        mySqlCommand.CommandText = sentencia;
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@sec_number_auditoria", sec_number_auditoria);
                        mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                        mySqlCommand.Parameters.AddWithValue("@traComentario","Asignacion de productos");
                        mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                        mySqlCommand.Parameters.AddWithValue("@traDominio", "DISTRIBUCION");
                        mySqlCommand.Parameters.AddWithValue("@traSubdominio", "SOLICITUDPRODUCTO");
                        mySqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Response_Code = "39";
                        Message = "ERROR AL CREAR KCR TRANSACCION";
                         _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                         logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                    
                }
                // Fin del proceso para registar en cta cte
                /*************************/

                try
                {

                    //	 Devolvemos el nuevo saldo al cliente
                
                    //mySqlCommand.CommandText = " SELECT  stkCantidad = stkCantidad FROM   [KlgStockAgenteProducto] WITH (NOLOCK)  WHERE  ageId = " + age_id + " AND  prdId = 0";
                    mySqlCommand.CommandText = " SELECT  stkCantidad FROM   [KlgStockAgenteProducto] WITH (NOLOCK)  WHERE  ageId = @age_id  AND  prdId = @prdId";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    stkCantidad = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                        Response_Code = "40";
                        Message = "ERROR AL SELECCIONAR DATOS DEL STOCK";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                }


                autorization = ult_sol_pro.ToString();
                Response_Code = "00";
                Message = "Ticket de Transferencia;;;Status: Aprobado;Num Transaccion: " + sec_number_auditoria + ";;Monto: " + amount + ";; agencia Origen: " + AgenteOrigen + ";" + (String.IsNullOrEmpty(UsuarioDestino) ? "" : ";Usuario Destino: " + UsuarioDestino) + ";agencia Destino: " + AgenteDestino;
                tran.Commit();
                return true;

            }
            catch (Exception ex)
            {
                Response_Code = "47";
                Message = "Ticket de Transferencia;;;Status: Error;Num Transaccion: " + sec_number_auditoria + ";;Monto: " + amount + ";; agencia Origen: " + AgenteOrigen + ";" + (String.IsNullOrEmpty(UsuarioDestino) ? "" : ";Usuario Destino: " + UsuarioDestino) + ";agencia Destino: " + AgenteDestino;
                //NO HAY ROLLBACK
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                tran.Rollback();
                return false;

            }
            finally
            {
                mySqlConnection.Close();
            }
        }


        public class TransferInfo {

            public int AgentParent { get; set; }
            public int UserTransacctionId { get; set; }

            public int OverrideRequestCode { get; set; }
            public int OverrideReverseCode { get; set; }

            public override string ToString()
            {
                return "AgentParent="+AgentParent+",UserTransacctionId="+ UserTransacctionId + ",OverrideRequestCode="+ OverrideRequestCode + ",OverrideReverseCode="+OverrideReverseCode;
            }
        }

        //protected override ILogger ProviderLogger { get { return logger; } }
        /// <summary>
        /// realiza un traslado b2b
        /// </summary>
        /// <param name="affectCtaCte">Indica si la operacion es un reversion con las respectivas reglas</param>
        /// <param name="amount">si es una reversion el monto es negativo</param>
        /// <returns></returns>
        public bool transferb2bCommission(string agent_reciver, string accessReceiver, string cuenta, int usr_id, double amount, string reference_number, DateTime date, bool affectCtaCte, ref string autorization, ref string Message, ref string Response_Code, TransferInfo transferInfo = null) //, ref decimal newAmount, bool? partialCharge = false)
        {
            //destomn

            //se mapearan en un anonymous type de tipo


            if (transferInfo != null && transferInfo.UserTransacctionId > 0) {

                usr_id = transferInfo.UserTransacctionId;
            }

            //agente que recibe la transferencia
            decimal _agent_reciver = decimal.Parse(agent_reciver);
            string age_estado = string.Empty;
            string AgenteDestino = string.Empty;
            string UsuarioDestino = string.Empty;

            //agente que realiza la recarga
            // decimal IdAgenteSolicitante = 0;
            decimal Agente = 0;
            string AgenteOrigen = string.Empty;

            //cuenta para realizar la transferencia
            decimal cubId = 0;
            string cubNumero = string.Empty;

            //cuenta corriente para recibir la transferencia
            decimal ctaSaldo = 0m;
            decimal ctaId = 0m;

            int dateDiff = 0;

            decimal IdBodegaOrigen = 0;
            decimal IdBodegaDestino = 0;

            int traIdReferencia;
            int sec_number_audit;
            int sec_number_trans;
            decimal ctaIdCorriente = 0;
            int sec_number_ctacte_mov;

            decimal ult_sol_pro;
            int sec_number;
            decimal ult_env_pro;

            decimal StockIdOrigen;
            decimal StockIdDestino;

            decimal sec_number_auditoria = 0m;
            int sec_number_aut;
            int sec_numbercta_movi;

            //CANTIDAD DEL CLIENTE ACTUALIZADA
            decimal stkCantidad;

            //CANTIDAD EN STOCK PARA VALIDAR
            decimal saldoOrigen = 0;
            decimal saldoDestino = 0;

            // varaible auxiliar para operaciones boleanas
            bool bandera = false;

            //variable auxiliar para concatenar el mensaje de error
            //cunado se utiliza TagValue
            string _m = "";

            string sentencia = string.Empty;

            autorization = "";

            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            SqlConnection mySqlConnection = new SqlConnection(strConnString);
            SqlCommand mySqlCommand = new SqlCommand();
            SqlDataReader reader = null;
            SqlTransaction tran = null;

            try
            {
                mySqlConnection.Open();
                tran = mySqlConnection.BeginTransaction();
                mySqlCommand.Transaction = tran;
                mySqlCommand.Connection = mySqlConnection;


                try
                {
                    //mySqlCommand.CommandText = "SELECT  CAST(par_valor as int) FROM [Parametro] WITH(NOLOCK) WHERE par_id='TimeDifference'";
                    mySqlCommand.CommandText = "SELECT  CAST(par_valor as int) FROM [Parametro] WITH(NOLOCK) WHERE par_id = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "TimeDifference");
                    dateDiff = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Message = "ERROR AL SELECCIONAR PARAMETRO TimeDifference";
                    Response_Code = "01";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                ///DATOS DEL AGENTE DESTINO LEER DATOS POR READER

                Response_Code = "02";
                try
                {
              

                  
                  
                    //query sin el nombre del usuario
                    mySqlCommand.CommandText = "SELECT ag.[age_nombre], ag.age_estado FROM [dbo].[Agente] ag  WHERE ag.[age_id] =  @age_id ";
                  

                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", _agent_reciver);
                    using (reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {

                            AgenteDestino = reader["age_nombre"].ToString();
                            age_estado = reader["age_estado"].ToString();
                            UsuarioDestino = bandera ? reader["usr_nombre"].ToString() : "";

                        }
                        else
                        {
                            Message = "NO SE ENCONTRO DATOS DEL AGENTE DESTINO CON EL ID INDICADO";

                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            if (reader != null && !reader.IsClosed)
                            { reader.Close(); }
                            tran.Rollback();
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Message = "ERROR CONSULTADO DATOS DEL AGENTE DESTINO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                ///


                ///

                //Validamos el saldo del pdv que va a realizar el pago

                ///DATOS DEL AGENTE QUE REALIZA LA TRANSFERENCIA
                Response_Code = "03";
                try
                {
                    // si se sobre escribio la informacion de la agente se debe buscar po agencia
    
                    if (transferInfo == null || transferInfo.AgentParent <=0 )
                    {
                        mySqlCommand.CommandText = "SELECT a.age_id, a.age_nombre  FROM [Agente] a WITH(NOLOCK) JOIN [Usuario] u WITH(NOLOCK) on a.age_id=u.age_id WHERE u.usr_id=@usr_id";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                    }
                    else {

                     
                        mySqlCommand.CommandText = "SELECT  a.age_id, a.age_nombre FROM [dbo].[Agente] a  WHERE a.age_id =  @age_id ";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@age_id", transferInfo.AgentParent);

                    }
                  


                    using (reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            // AgOrigenData = new AgentData()
                            //{
                            //  age_id = (int)reader["age_id"],
                            Agente = (decimal)reader["age_id"];
                            AgenteOrigen = reader["age_nombre"].ToString();
                             
                            //};
                        }
                        else
                        {
                            Message = "NO SE ENCONTRO DATOS DEL AGENTE ORIGEN CON EL ID INDICADO";

                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            tran.Rollback();
                            return false;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Message = "ERROR CONSULTADO DATOS DEL AGENTE ORIGEN";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                ///

                // RP - OJO con esto, debemos utilizar un if para determinar que saldo es o no insuficiente
                // ARV si el saldo es mayor a cero se esta realizando una transferencia padre hijo
                //if (amount > 0)
                {
                    Response_Code = "04";
                    try
                    {
                        //mySqlCommand.CommandText = "SELECT stkCantidad FROM [KlgStockAgenteProducto] WITH (NOLOCK) WHERE ageId =" + Agente + " AND prdId = 0";

                        mySqlCommand.CommandText = "SELECT stkCantidad FROM [KlgStockAgenteProducto] WITH (NOLOCK) WHERE ageId = @age_id  AND prdId = @prdId";
                        mySqlCommand.Parameters.Clear();
                        // si no es reversion se verifica la cantidad del solicitante, si no se verifica del destino
                        mySqlCommand.Parameters.AddWithValue("@age_id", amount > 0 ? Agente : _agent_reciver);
                        mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                        saldoOrigen = (decimal)mySqlCommand.ExecuteScalar();

                        //verificacion para hacer la el trasaldo 
                        if (amount > 0)
                        {
                            if ((double)saldoOrigen < amount) // && !(partialCharge ?? false))
                            {

                                Message = "NO HAY SALDO SUFICIENTE PARA HACER EL TRASLADO";
                                logger.ErrorLow(() => TagValue.New().Message("NO HAY SALDO SUFICIENTE PARA HACER EL TRASLADO").Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                tran.Rollback();
                                return false;
                            }
                        }
                        //verificacion para hacer la Reversión
                        else
                        {
                            if ((double)saldoOrigen < Math.Abs(amount)) // && !(partialCharge ?? false))
                            {
                                Message = "NO HAY SALDO SUFICIENTE PARA HACER LA REVERSION";

                                logger.ErrorLow(() => TagValue.New().Message("NO HAY SALDO SUFICIENTE PARA HACER LA REVERSION").Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                tran.Rollback();
                                return false;
                            }

                        }
                    }
                    catch (Exception ex)
                    {

                        Message = "ERROR VERFICANDO SALDO DE INVENTARIO DEL AGENTE ORIGEN";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                }
               
                //ED: no se debe afectar cuenta corriente en estas distribuciones padre a el que sea.
                if (affectCtaCte)
                {
                    ///DATOS CUENTA CORRIENTE MOVIL WAY

                   
                    Response_Code = "06";
                    try
                    {
                        //mySqlCommand.CommandText = "SELECT cubId    FROM dbo.KfnCuentaBanco WITH(ROWLOCK)  WHERE cubNumero = '" + cuenta + "'";
                        mySqlCommand.CommandText = "SELECT cubId  , cubNumero  FROM dbo.KfnCuentaBanco WITH(ROWLOCK)  WHERE cubNumero = @cuenta";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@cuenta", cuenta);


                        using (reader = mySqlCommand.ExecuteReader())
                        {
                            if (reader.HasRows && reader.Read())
                            {
                                cubId = Convert.ToInt32(reader["cubId"]);
                                cubNumero = reader["cubNumero"].ToString();
                            }
                            else
                            {
                                Message = "NO SE ENCONTRO DATOS DE LA CUENTA DE BANCO";
                                _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                                logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(new { cuenta = cuenta }));
                                if (reader != null && !reader.IsClosed)
                                { reader.Close(); }
                                tran.Rollback();
                                return false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        Message = "ERROR BSUCANDO DATOS CUENTA BANCO";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { cuenta = cuenta }));
                        tran.Rollback();
                        return false;
                    }

                    ///

                    ///cuenta origen
                    Response_Code = "07";
                    try
                    {
                        // mySqlCommand.CommandText = "SELECT  ctaSaldo      FROM KfnCuentaCorriente  WITH(ROWLOCK) WHERE ageId =  " + age_id;

                        mySqlCommand.CommandText = "SELECT ctaId, ctaSaldo FROM KfnCuentaCorriente  WITH(ROWLOCK) WHERE ageId = @age_id";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@age_id", _agent_reciver);

                        using (reader = mySqlCommand.ExecuteReader())
                        {
                            if (reader.HasRows && reader.Read())
                            {

                                ctaId = Convert.ToInt32(reader["ctaId"]);
                                ctaSaldo = Convert.ToDecimal(reader["ctaSaldo"]);
                            }
                            else
                            {
                                Message = "NO SE ENCONTRO DATOS DEL AGENTE ORIGEN CON EL ID INDICADO";
                                _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                                logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(_agent_reciver));
                                tran.Rollback();
                                return false;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Message = "ERROR SELECCIONANDO DATOS DE CUENTA CORRIENTE";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(_agent_reciver));
                        tran.Rollback();
                        return false;
                    }



                    ///
                }
                /*FIN ED*/

                ///Proceso de distribucion 

                // RP - OJO, este query no lo ejecutemos de nuevo, copiemos los valores previamente consultados
                //QUITAR QUERY COLOCAR DATOS DE OBJETOS MAPEADOS
              

                IdBodegaOrigen = Agente;
                IdBodegaDestino = _agent_reciver;
                ///

                

                Response_Code = "08";
                try
                {
                    // mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                    mySqlCommand.CommandText = "UPDATE [secuencia] WITH(ROWLOCK) SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR LA SECUENCIA SOLICITUDPRODUCTO";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                        tran.Rollback();
                        return false;
                    }

                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR LA SECUENCIA SOLICITUDPRODUCTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                ///
                Response_Code = "09";
                try
                {
                    // mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                    mySqlCommand.CommandText = "SELECT sec_number FROM [secuencia] WITH(NOLOCK) WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                    // ult_sol_pro = (int)mySqlCommand.ExecuteScalar();
                    sec_number = (int)mySqlCommand.ExecuteScalar();
                    ult_sol_pro = sec_number;
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL SELECCIONAR LA SECUENCIA DE SOLICITUDPRODUCTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }


                ///

                ///
                /* inserto una solicitud y la coloco en estado pendiente */
                Response_Code = "10";
                try
                {
                    //CONSTANTES EN CODIGO
                    sentencia = "INSERT INTO [KlgSolicitudProducto] WITH(ROWLOCK) (sprId,usrId,ageIdSolicitante,ageIdDestinatario,prvIdDestinatario,sprEstado,sprFecha,sprFechaAprobacion,sprImporteSolicitud,sltId,ageIdBodegaOrigen,ageIdBodegaDestino) " +
                                    "VALUES (@ult_sol_pro,@usr_id,@IdAgenteSolicitante,@age_id, @prvIdDestinatario,@sprEstado,@date,@date,@amount,@sltId,@IdBodegaOrigen,@IdBodegaDestino)";

                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    //entre condiciones diferencia entre el parametro sprImporteSolicitudsltId
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteSolicitante", IdBodegaOrigen);
                    mySqlCommand.Parameters.AddWithValue("@age_id", IdBodegaDestino);
                    mySqlCommand.Parameters.AddWithValue("@prvIdDestinatario", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@sprEstado", "AU");
                    mySqlCommand.Parameters.AddWithValue("@date", date);
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaOrigen", IdBodegaOrigen);
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaDestino", IdBodegaDestino);

                    if (amount >= 0)
                    {
                        mySqlCommand.Parameters.AddWithValue("@amount", amount);
                        mySqlCommand.Parameters.AddWithValue("@sltId", transferInfo != null && transferInfo.OverrideRequestCode > 0 ? transferInfo.OverrideRequestCode :  202);
                    }
                    else
                    {
                        mySqlCommand.Parameters.AddWithValue("@amount",  amount * -1);
                        mySqlCommand.Parameters.AddWithValue("@sltId", transferInfo != null && transferInfo.OverrideReverseCode > 0 ? transferInfo.OverrideReverseCode :  205);
                    }
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Message = "ERROR AL CREAR SOLICITUD PRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                ///

                ///
                Response_Code = "11";
                try
                {
                    //sentencia = "INSERT INTO [KlgSolicitudProductoItem] WITH(ROWLOCK) (sprId,prdId,spiCantidadSolicitada,spiCantidadAutorizada,spiPrecioUnitario,spiEstado) VALUES (" + ult_sol_pro + ",0," + amount + "," + amount + ", 1.0000,'PE')";
                    //CONSTANTES EN CODIGO
                    sentencia = "INSERT INTO [KlgSolicitudProductoItem] WITH(ROWLOCK) (sprId,prdId,spiCantidadSolicitada,spiCantidadAutorizada,spiPrecioUnitario,spiEstado)  VALUES (@ult_sol_pro ,@prdId,@amount ,@amount , @spiPrecioUnitario,@spiEstado)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@spiPrecioUnitario", 1.0000);
                    mySqlCommand.Parameters.AddWithValue("@spiEstado", "AU");
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL CREAR SOLICITUD PRODUCTO ITEM";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { ult_sol_pro = ult_sol_pro, amount = amount }));
                    tran.Rollback();
                    return false;
                }


                // RP - OJO, en vez de hacer este update mejor insertar como AU en el insert previo .. dejar el update comentado
                #region codigo comentariado
                /*
                try
                {
                    // sentencia = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK) SET spiEstado = 'AU', spiCantidadAutorizada = " + amount + "WHERE sprId =" + ult_sol_pro + " AND prdId = 0 AND spiEstado = 'PE'";
                    sentencia = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK) SET spiEstado = 'AU', spiCantidadAutorizada = @amount WHERE sprId = @ult_sol_pro  AND prdId = 0 AND spiEstado = 'PE'";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                        throw new Exception("NO SE PUDO ACTUALIZAR SOLICITUD PRODUCTO IT");
                }
                catch (Exception ex)
                {
                    Response_Code = "17";
                    Message = "";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { ult_sol_pro = ult_sol_pro, amount = amount }));
                    tran.Rollback();
                    return false;
                }*/
                #endregion

                ///
                Response_Code = "12";
                try
                {
                    //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "ENVIOPRODUCTO");
                    mySqlCommand.CommandText = "UPDATE [secuencia] WITH(ROWLOCK) SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "ENVIOPRODUCTO");
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR SECUENCIA ENVIOPRODUCTO";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                        tran.Rollback();
                        return false;

                    }

                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR LA SECUENCIA ENVIO PRODUCTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }


                Response_Code = "13";
                try
                {
                    // mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "ENVIOPRODUCTO");
                    mySqlCommand.CommandText = "SELECT sec_number FROM [secuencia] WITH(NOLOCK) WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "ENVIOPRODUCTO");
                    ult_env_pro = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL SELECCIONAR LA SECUENCIA ENVIO PRODUCTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                //sentencia = "INSERT INTO [KlgEnvio] WITH(ROWLOCK) (envId,envEstado,envFechaEnvio,envFechaRecepcion,envNumeroRemito,envNumeroFactura,envObservaciones,ageIdBodegaOrigen,ageIdBodegaDestino) " +"VALUES (" + ult_env_pro + ",'DE','" + date.ToString("yyyy-MM-dd HH:mm:ss") + "',NULL,'','','Envio de productos Virtuales en linea'," + IdBodegaOrigen + "," + IdBodegaDestino + ")";
                Response_Code = "14";
                try
                {
                    sentencia = "INSERT INTO [KlgEnvio] WITH(ROWLOCK) (envId,envEstado,envFechaEnvio,envFechaRecepcion,envNumeroRemito,envNumeroFactura,envObservaciones,ageIdBodegaOrigen,ageIdBodegaDestino) VALUES (@ult_env_pro ,@envEstado,@date,@envFechaRecepcion,@envNumeroRemito,@envNumeroFactura,@envObservaciones,@IdBodegaOrigen , @IdBodegaDestino )";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@envEstado", "DE");
                    mySqlCommand.Parameters.AddWithValue("@date", date);
                    mySqlCommand.Parameters.AddWithValue("@envFechaRecepcion", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@envNumeroRemito", reference_number);
                    mySqlCommand.Parameters.AddWithValue("@envNumeroFactura", "");
                    mySqlCommand.Parameters.AddWithValue("@envObservaciones", "Envio de productos Virtuales en linea");
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaOrigen", IdBodegaOrigen);
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaDestino", IdBodegaDestino);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL CREAR UN ENVIO";

                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { amount = amount, ult_sol_pro = ult_sol_pro }));
                    tran.Rollback();
                    return false;
                }

                try
                {
                    //sentencia = "INSERT INTO [KlgSolicitudProductoEnvio] WITH(ROWLOCK) (sprId,envId) VALUES (" + ult_sol_pro + "," + ult_env_pro + ")";
                    sentencia = "INSERT INTO [KlgSolicitudProductoEnvio] WITH(ROWLOCK) (sprId,envId) VALUES (@ult_sol_pro ,@ult_env_pro )";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = "15";
                    Message = "ERROR AL CREAR SOLICITUD PRODUCTO ENVIO";

                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;

                }

                try
                {

                    //sentencia = "INSERT INTO [KlgEnvioItem] WITH(ROWLOCK) (envId,prdId,eitCantidad,eitEstado) VALUES (" + ult_env_pro + "," + 0 + "," + amount.ToString().Replace(",", ".") + ",'DE')";
                    sentencia = "INSERT INTO [KlgEnvioItem] WITH(ROWLOCK) (envId,prdId,eitCantidad,eitEstado) VALUES (@ult_env_pro , @prdId ,@amount ,@eitEstado)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("prdId", 0);
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@eitEstado", "DE");
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = "16";
                    Message = "ERROR AL CREAR ENVIO ITEM";

                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;

                }

                // RP - OJO no tocar este, dejarlo así
                Response_Code = "17";
                try
                {
                    //mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = 'DE' WHERE sprId =" + ult_sol_pro + " AND prdId = 0 AND piEstado = 'AU'";
                    mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = @spiEstadoN WHERE sprId = @ult_sol_pro AND prdId = @prdId AND spiEstado = @spiEstado";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@spiEstadoN", "DE");
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    mySqlCommand.Parameters.AddWithValue("@spiEstado", "AU");
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR EL ESTADO DE LA SOLICITUD DE PRODUCTO";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { ult_sol_pro = ult_sol_pro }));
                        tran.Rollback();
                        return false;

                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL CREAR SOLICITUD PRODUCTO IT";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { ult_sol_pro = ult_sol_pro }));
                    tran.Rollback();
                    return false;

                }

                try
                {

                    //mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId =" + IdAgenteSolicitante + " AND prdId IN (0)";
                    mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId = @IdAgenteSolicitante AND prdId = @prdId";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteSolicitante", Agente);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    StockIdOrigen = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "18";
                    Message = "ERROR AL SELECCIONAR STOCK AGENTE PRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { }));
                    tran.Rollback();
                    return false;

                }

                //
                //decimal _newAmount = 0m;
                Response_Code = "19";
                try
                {
                    //mySqlCommand.CommandText = " UPDATE [KlgStockAgenteProducto] WITH(ROWLOCK) SET  stkCantidad = stkCantidad + - " + amount + " WHERE  stkId = " + StockIdOrigen + " AND ageId = " + IdAgenteSolicitante + "and stkCantidad + - " + amount + ">= 0";
                    mySqlCommand.CommandText = " UPDATE [KlgStockAgenteProducto] WITH(ROWLOCK) SET  stkCantidad = stkCantidad + - @amount  WHERE  stkId = @StockIdOrigen AND ageId = @IdAgenteSolicitante and stkCantidad + - @amount >= 0";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@StockIdOrigen", StockIdOrigen);
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteSolicitante", Agente);

                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR EL STOCK AGENTE PRODUCTO";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR ALA ACTUALIZAR STOCK AGENTE PRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { amount = amount, StockIdOrigen = StockIdOrigen, IdAgenteSolicitante = Agente }));
                    tran.Rollback();
                    return false;
                }

                Response_Code = "20";
                try
                {
                    //mySqlCommand.CommandText = " UPDATE [KlgEnvio] WITH(ROWLOCK) SET envEstado = 'RE', envFechaRecepcion = GETDATE() WHERE envId = " + ult_env_pro + " AND envEstado = 'DE'";
                    mySqlCommand.CommandText = " UPDATE [KlgEnvio] WITH(ROWLOCK) SET envEstado = @newestado, envFechaRecepcion = dateadd(minute,@dateDiff ,getdate()) WHERE envId = @ult_env_pro AND envEstado = @oldestado";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@dateDiff", dateDiff);
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@oldestado", "DE");
                    mySqlCommand.Parameters.AddWithValue("@newestado", "RE");

                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR EL ESTADO DEL  ENVIO";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR ENVIO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                Response_Code = "21";
                try
                {
                    //mySqlCommand.CommandText = "UPDATE [KlgEnvioItem] WITH(ROWLOCK) SET eitEstado = 'RE'  WHERE envId = " + ult_env_pro + " AND prdId = 0 AND eitEstado = 'DE'";
                    mySqlCommand.CommandText = "UPDATE [KlgEnvioItem] WITH(ROWLOCK) SET eitEstado = @newestado  WHERE envId = @ult_env_pro  AND prdId = 0 AND eitEstado = @oldestado ";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@oldestado", "DE");
                    mySqlCommand.Parameters.AddWithValue("@newestado", "RE");

                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR EL ESTADO DE ENVIO IT";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { ult_sol_pro = ult_sol_pro }));
                        tran.Rollback();
                        return false;
                    }


                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR ENVIO ITEM";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                Response_Code = "22";
                try
                {
                    //mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = 'RE'  WHERE sprId = " + ult_sol_pro + "  AND prdId = 0";
                    mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = @newestado  WHERE sprId = @ult_sol_pro  AND prdId = @prdId";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@newestado", "RE");
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);

                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR EL ESTADO SOLICITUD PRODUCTO IT";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }

                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR SOLICITUD PRODUCTO ITEM";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { ult_sol_pro = ult_sol_pro }));
                    tran.Rollback();
                    return false;
                }

                try
                {
                    //mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId =" +age_id + " AND prdId IN (0)";
                    mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId = @age_id  AND prdId = @prdId";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", _agent_reciver);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    StockIdDestino = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "23";
                    Message = "ERROR AL SELECCIONAR DATOS STOCK AGENTE PRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                Response_Code = "24";
                try
                {
                    //mySqlCommand.CommandText = "UPDATE KlgStockAgenteProducto SET  stkCantidad = stkCantidad + " + amount + "  WHERE  stkId =" + StockIdDestino + " AND ageId =" + age_id + " and stkCantidad + " + amount + ">= 0 ";
                    mySqlCommand.CommandText = "UPDATE KlgStockAgenteProducto SET  stkCantidad = stkCantidad + @amount  WHERE  stkId = @StockIdDestino AND ageId = @age_id and stkCantidad +  @amount >= 0 ";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@StockIdDestino", StockIdDestino);
                    mySqlCommand.Parameters.AddWithValue("@age_id", _agent_reciver);

                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                       
                        Message = "NO SE PUDO ACTUALIZAR LA CANTDAD DEL STOCK DESTINO";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                        
                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR STOCK AGENTE PRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                Response_Code = "25";
                try
                {
                    //mySqlCommand.CommandText = " UPDATE KlgSolicitudProducto WITH(ROWLOCK) SET sprEstado = 'CE' WHERE sprId = " + ult_sol_pro + "  AND sprEstado = 'AU' AND ( ageIdSolicitante =" + age_id + " OR ageIdDestinatario =" + age_id + ")";
                    mySqlCommand.CommandText = " UPDATE KlgSolicitudProducto WITH(ROWLOCK) SET sprEstado = @newestado WHERE sprId = @ult_sol_pro  AND sprEstado = @oldestado AND ( ageIdSolicitante = @age_id  OR ageIdDestinatario = @age_id )";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@age_id", _agent_reciver);
                    mySqlCommand.Parameters.AddWithValue("@oldestado", "AU");
                    mySqlCommand.Parameters.AddWithValue("@newestado", "CE");

                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR EL ESTADO DE SOLICITUD PRODUCTO";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;

                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR SOLICITUD PRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                //Fin de proceso de Acreditacion  

                // Inicio el proceso para registar en cta cte
                Response_Code = "26";
                try
                {
                    //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");


                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR LA SECUENCIA AUDITORIA";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;

                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR SECUENCIA DE AUDITORIA";

                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                try
                {
                    //mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    sec_number_auditoria = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "27";
                    Message = "ERROR AL SELECCIONAR SECUENCIA AUDITORIA";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                // RP - OJO revisar en otros métodos si la DISTRIBUCION y ENVIOPRODUCTO se mapeaban a un parametro para usar el mismo nombre
                try
                {

                    //sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                    //            "VALUES ( " + sec_number_auditoria + ", " + usr_id + ", null, 'Despacho de productos nro." + ult_env_pro + "'" + " , GETDATE()," + ult_env_pro + ",'DISTRIBUCION', 'ENVIOPRODUCTO')";
                    sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio) VALUES ( @sec_number_auditoria , @usr_id , null, @traComentario, dateadd(minute,@dateDiff,getdate()),@ult_env_pro ,@traDominio, @traSubdominio)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@sec_number_auditoria", sec_number_auditoria);
                    mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@traComentario", "Despacho de productos nro." + ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@dateDiff", dateDiff);
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@traDominio", "DISTRIBUCION");
                    mySqlCommand.Parameters.AddWithValue("@traSubdominio", "ENVIOPRODUCTO");
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = "28";
                    Message = "ERROR AL CREAR KCR TRANSACCION";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                //ED:no se necesita nuevamente afectar cta ctre
                // --Volvemos a leer el saldo de la cta cte. 
                if (affectCtaCte)
                {
                   

                    Response_Code = "31";
                    try
                    {
                        //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                        mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");

                        if (mySqlCommand.ExecuteNonQuery() == 0)
                        {
                            Message = "NO SE PUDO ACTUALIZAR LA SECUENCIA TRANSACCION";

                            logger.ErrorLow(String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            tran.Rollback();
                            return false;

                        }
                    }
                    catch (Exception ex)
                    {

                        Message = "ERROR AL ACTUALIZAR LA SECUENCIA TRANSACCION";
                        logger.ErrorLow(String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message, ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }
                    Response_Code = "32";
                    try
                    {
                        //mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                        mySqlCommand.CommandText = "SELECT sec_number_aut = sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                        sec_number_aut = (int)mySqlCommand.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {

                        Message = "ERROR AL SELECCIONAR LA SECUENCUA TRANSACCION";

                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }

                    // RP ABR 2015 - No es necesario validar límites de cta cte ni montos mayores a cero porque acá el monto es negativo, por lo tanto siempre se sumará
                    Response_Code = "33";
                    try
                    {
                        //mySqlCommand.CommandText = "UPDATE KfnCuentaCorriente WITH(ROWLOCK) SET ctaSaldo = ctaSaldo + - " + amount + " WHERE ctaId =" + ctaId + " AND ageId = " + age_id + " AND ctaSaldo =" + ctaSaldo + " AND ( (ctaSaldo + - " + amount + " >= 0) OR (ABS(ctaSaldo + - " + amount + ") <= 600) )";
                        mySqlCommand.CommandText = "UPDATE KfnCuentaCorriente WITH(ROWLOCK) SET ctaSaldo = ctaSaldo + - @amount  WHERE ctaId = @ctaId  AND ageId = @age_id AND ctaSaldo = @ctaSaldo";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@amount", amount);
                        mySqlCommand.Parameters.AddWithValue("@ctaId", ctaId);
                        mySqlCommand.Parameters.AddWithValue("@age_id", _agent_reciver);
                        mySqlCommand.Parameters.AddWithValue("@ctaSaldo", ctaSaldo);
                        if (mySqlCommand.ExecuteNonQuery() == 0)
                        {
                            Message = "NO SE PUDO ACTUALIZAR EL SALDO DE LA CUENTA CORRIENTE";
                            _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                            tran.Rollback();
                            return false;

                        }
                    }
                    catch (Exception ex)
                    {

                        Message = "ERROR AL ACTUALIZAR KFN CUENTACORRIENTE";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }

                    try
                    {

                        //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                        mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");

                        if (mySqlCommand.ExecuteNonQuery() == 0)
                        {
                            Message = "NO SE PUDO ACTUALIZAR LA SECUENCIA CTACTEMOVIMIENTO";
                            _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                            tran.Rollback();
                            return false;

                        }
                    }
                    catch (Exception ex)
                    {
                        Response_Code = "34";
                        Message = "ERROR AL ACTUALIZAR SECUENCIA CTACTEMOVIMIENTO";

                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }

                    try
                    {
                        //mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = 'CTACTEMOVIMIENTO'";
                        mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                        sec_numbercta_movi = (int)mySqlCommand.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        Response_Code = "35";
                        Message = "ERROR AL SELECCONAR SECUENCIA CTA CTEMOVIMIENTO";

                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }

                    try
                    {
                        sentencia = "INSERT INTO KfnCuentaCorrienteMovimiento (ccmId,ctaId,ccmFecha,ccmImporte  ,ccmSaldo,ccmDetalle,ccmNumeroTransaccion,ttrId) VALUES ( @sec_numbercta_movi ,@ctaId ,GETDATE()  ,@amount ,@ctaSaldo,@ccmDetalle,@sec_number_aut , @ttrId)";
                        mySqlCommand.CommandText = sentencia;
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@sec_numbercta_movi", sec_numbercta_movi);
                        mySqlCommand.Parameters.AddWithValue("@ctaId", ctaId);
                        mySqlCommand.Parameters.AddWithValue("@amount", amount);
                        mySqlCommand.Parameters.AddWithValue("@ctaSaldo", ctaSaldo);
                        mySqlCommand.Parameters.AddWithValue("@ccmDetalle", "Asignacion de productos No.: " + ult_sol_pro);
                        mySqlCommand.Parameters.AddWithValue("@sec_number_aut", sec_number_aut);
                        mySqlCommand.Parameters.AddWithValue("ttrId", 2000);

                        mySqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Response_Code = "36";
                        Message = "ERROR AL CREAR MOVIMIENTO CUENTA CORRIENTE";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { sec_numbercta_movi = sec_numbercta_movi, ctaId = ctaId, amount = amount, ctaSaldo = ctaSaldo, sec_number_aut = sec_number_aut }));
                        tran.Rollback();
                        return false;
                    }
                    Response_Code = "37";
                    try
                    {

                        //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                        mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");

                        if (mySqlCommand.ExecuteNonQuery() == 0)
                        {
                            Message = "NO SE PUDO ACTUALIZAR LA SECUENCIA AUDITORIA";
                            logger.ErrorLow(String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            tran.Rollback();
                            return false;

                        }
                    }
                    catch (Exception ex)
                    {

                        Message = "ERROR AL ACTUALIZAR SECUENCIA AUDITORIA";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }

                    try
                    {

                        //mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                        mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                        sec_number_auditoria = (int)mySqlCommand.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        Response_Code = "38";
                        Message = "ERROR AL SELECCIONAR SECUENCIA AUDITORIA";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { }));
                        tran.Rollback();
                        return false;
                    }

                    try
                    {
                        //sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" + "VALUES (" + sec_number_auditoria + "," + usr_id + ", null, 'Asignacion de productos',  GETDATE()," + ult_sol_pro + ", 'DISTRIBUCION', 'SOLICITUDPRODUCTO')";

                        sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio) VALUES (@sec_number_auditoria ,@usr_id , null,@traComentario ,  GETDATE(), @ult_sol_pro ,@traDominio, @traSubdominio)";

                        mySqlCommand.CommandText = sentencia;
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@sec_number_auditoria", sec_number_auditoria);
                        mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                        mySqlCommand.Parameters.AddWithValue("@traComentario", "Asignacion de productos");
                        mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                        mySqlCommand.Parameters.AddWithValue("@traDominio", "DISTRIBUCION");
                        mySqlCommand.Parameters.AddWithValue("@traSubdominio", "SOLICITUDPRODUCTO");
                        mySqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Response_Code = "39";
                        Message = "ERROR AL CREAR KCR TRANSACCION";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }

                }
                // Fin del proceso para registar en cta cte
                /*************************/

                try
                {

                    //	 Devolvemos el nuevo saldo al cliente

                    //mySqlCommand.CommandText = " SELECT  stkCantidad = stkCantidad FROM   [KlgStockAgenteProducto] WITH (NOLOCK)  WHERE  ageId = " + age_id + " AND  prdId = 0";
                    mySqlCommand.CommandText = " SELECT  stkCantidad FROM   [KlgStockAgenteProducto] WITH (NOLOCK)  WHERE  ageId = @age_id  AND  prdId = @prdId";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", _agent_reciver);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    stkCantidad = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "40";
                    Message = "ERROR AL SELECCIONAR DATOS DEL STOCK";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }


                autorization = ult_sol_pro.ToString();
                Response_Code = "00";
                Message = "Ticket de Transferencia;;;Status: Aprobado;Num Transaccion: " + sec_number_auditoria + ";;Monto: " + amount + ";; agencia Origen: " + AgenteOrigen + ";" + (String.IsNullOrEmpty(UsuarioDestino) ? "" : ";Usuario Destino: " + UsuarioDestino) + ";agencia Destino: " + AgenteDestino;
                tran.Commit();
                return true;

            }
            catch (Exception ex)
            {
                Response_Code = "47";
                Message = "Ticket de Transferencia;;;Status: Error;Num Transaccion: " + sec_number_auditoria + ";;Monto: " + amount + ";; agencia Origen: " + AgenteOrigen + ";" + (String.IsNullOrEmpty(UsuarioDestino) ? "" : ";Usuario Destino: " + UsuarioDestino) + ";agencia Destino: " + AgenteDestino;
                //NO HAY ROLLBACK
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                tran.Rollback();
                return false;

            }
            finally
            {
                mySqlConnection.Close();
            }
        }


        public bool Externaltransfer(decimal age_id, string cuenta, int usr_id, double amount, string reference_number, DateTime date, ref string autorization, ref string Message, ref string Response_Code)
        {

            // decimal age_id;
            string age_estado = string.Empty;
            decimal cubId = 0;
            string cubNumero = string.Empty;
            decimal ctaSaldo;
            decimal ctaId;
            decimal IdAgenteSolicitante = 0;
            decimal IdBodegaOrigen = 0;
            decimal IdBodegaDestino = 0;
            decimal Agente = 0;
            int traIdReferencia;
            int sec_number_audit;
            int sec_number_trans;
            decimal ctaIdCorriente = 0;
            int sec_number_ctacte_mov;
            decimal ult_sol_pro;
            int sec_number;
            decimal ult_env_pro;
            decimal StockIdOrigen;
            decimal StockIdDestino;
            decimal sec_number_auditoria;
            int sec_number_aut;
            int sec_numbercta_movi;
            decimal stkCantidad;
            decimal saldo = 0;
            string AgenteNombre = string.Empty;
            int dateDiff = 0;

            string sentencia = string.Empty;


            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            SqlConnection mySqlConnection = new SqlConnection(strConnString);
            SqlCommand mySqlCommand = new SqlCommand();


            SqlTransaction tran = null;

            try
            {
                mySqlConnection.Open();
                tran = mySqlConnection.BeginTransaction();
                mySqlCommand.Transaction = tran;
                mySqlCommand.Connection = mySqlConnection;

                
                mySqlCommand.CommandText = "SELECT CAST(par_valor as int) FROM [Parametro] WITH(NOLOCK) WHERE par_id='TimeDifference'";
                dateDiff = (int)mySqlCommand.ExecuteScalar();

                try
                {

                    
                    mySqlCommand.CommandText = "SELECT  age_nombre FROM  agente  WITH(NOLOCK)  WHERE    age_id 	= " + age_id + "";
                    AgenteNombre = mySqlCommand.ExecuteScalar().ToString();


                    if (AgenteNombre.Length == 0)
                    {
                        Message = "PDV NO EXISTE O AGENCIA NO PARTICIPA";
                        Response_Code = "01";
                        autorization = "";
                        return false;
                    }

                }
                catch (Exception ex)
                {
                    logger.ErrorLow(ex.Message + ". " + ex.StackTrace);
                    Message = "PDV NO EXISTE O AGENCIA NO PARTICIPA";
                    Response_Code = "01";
                    autorization = "";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    return false;
                }



                try
                {
                    
                    mySqlCommand.CommandText = "SELECT  age_estado FROM  agente  WITH(NOLOCK)  WHERE   age_id 	=  " + age_id + "";
                    age_estado = mySqlCommand.ExecuteScalar().ToString();

                    if (age_estado == "SU" || age_estado == "DE")
                    {
                        Response_Code = "08";
                        Message = "AGENCIA DESTINO SUSPENDIDA O INACTIVA;" + AgenteNombre;
                        return false;

                    }
                }
                catch (Exception ex)
                {
                    logger.ErrorLow(ex.Message + ". " + ex.StackTrace);
                    Response_Code = "08";
                    Message = "AGENCIA DESTINO SUSPENDIDA O INACTIVA;" + AgenteNombre;
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    return false;

                }




                //Validamos el saldo del pdv que va a realizar el pago

                
                //mySqlCommand.CommandText = "SELECT age_id  FROM [Agente] WITH(NOLOCK)   WHERE usr_id = " + usr_id;
                mySqlCommand.CommandText = "SELECT TOP 1 a.age_id  FROM [Agente] a WITH(NOLOCK) JOIN [Usuario] u WITH(NOLOCK) on a.age_id=u.age_id    WHERE u.usr_id in (" + usr_id + ")";
                Agente = (decimal)mySqlCommand.ExecuteScalar();


                
                mySqlCommand.CommandText = "SELECT   stkCantidad  FROM   [KlgStockAgenteProducto] WITH (NOLOCK)  WHERE  ageId =" + Agente + "  AND  prdId = 0";
                saldo = (decimal)mySqlCommand.ExecuteScalar();

                if ((double)saldo < amount)
                {
                    Response_Code = "02";
                    Message = "SALDO INSUFICIENTE";
                    return false;

                }



                try
                {
                    
                    mySqlCommand.CommandText = "SELECT cubId    FROM dbo.KfnCuentaBanco WITH(ROWLOCK)  WHERE cubNumero = '" + cuenta + "'";
                    cubId = (decimal)mySqlCommand.ExecuteScalar();


                    
                    mySqlCommand.CommandText = "SELECT cubNumero    FROM dbo.KfnCuentaBanco WITH(ROWLOCK)  WHERE cubNumero = '" + cuenta + "'";
                    cubNumero = mySqlCommand.ExecuteScalar().ToString();

                    if (cubNumero.Trim() != cuenta.Trim())
                    {
                        Response_Code = "09";
                        Message = "CUENTA NO EXISTE";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                   
                    Response_Code = "09";
                    Message = "CUENTA NO EXISTE";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    return false;
                }




                
                mySqlCommand.CommandText = "SELECT  ctaId    FROM KfnCuentaCorriente  WITH(ROWLOCK) WHERE ageId =  " + age_id;
                ctaId = (decimal)mySqlCommand.ExecuteScalar();

                
                mySqlCommand.CommandText = "SELECT  ctaSaldo      FROM KfnCuentaCorriente  WITH(ROWLOCK) WHERE ageId =  " + age_id;
                ctaSaldo = (decimal)mySqlCommand.ExecuteScalar();


                //Proceso de distribucion 

                
                //mySqlCommand.CommandText = "SELECT age_id  FROM [Agente] WITH(NOLOCK)   WHERE usr_id = " + usr_id;
                mySqlCommand.CommandText = "SELECT TOP 1 a.age_id  FROM [Agente] a WITH(NOLOCK) JOIN [Usuario] u WITH(NOLOCK) on a.age_id=u.age_id    WHERE u.usr_id in (" + usr_id + ")";
                IdAgenteSolicitante = (decimal)mySqlCommand.ExecuteScalar();

                //IdBodegaOrigen = IdAgenteSolicitante;
                //IdBodegaDestino = age_id;

                IdBodegaOrigen =Agente;
                IdBodegaDestino = age_id;

                //correccion de secuencias 
                //se hace Parameters.Clear() por definicion
                
               // mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "DEPOSITO");
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "DEPOSITO");
                mySqlCommand.ExecuteNonQuery();

               
                
                //mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = 'DEPOSITO'";
                mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "DEPOSITO");
                traIdReferencia = (int)mySqlCommand.ExecuteScalar();


                
               // mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName  =  @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA"); 
                mySqlCommand.ExecuteNonQuery();


                
                //mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA"); 
                sec_number_audit = (int)mySqlCommand.ExecuteScalar();


                
                sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                             "VALUES (" + sec_number_audit.ToString() + "," + usr_id.ToString() + ", null, 'Ingreso de Deposito por el valor : " + amount.ToString() + "'" + ", GETDATE() ," + traIdReferencia.ToString() + ", 'FINANCIERO', 'AVISODEPOSITO')";
                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                
                sentencia = "INSERT INTO KfnDeposito (depId,ageIdOrigen,cubId,depComprobante,depFechaComprobante,depFecha,depMonto,depEstado,depComentario,depComentarioProcesamiento,ccmNumeroTransaccion,usrId,processingUsrId)" +
                             "VALUES ( " + traIdReferencia.ToString() + "," + age_id.ToString() + "," + cubId.ToString() + "," + reference_number + ",'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ", getdate()," + amount.ToString().Replace(",", ".") + ", 'PE', 'Deposito enviado desde API-Movilway monto: " + amount.ToString() + "'" + ", null, null," + usr_id.ToString() + ", null)";
                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                
                //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION"); 
                mySqlCommand.ExecuteNonQuery();


                
                //mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION"); 
                sec_number_trans = (int)mySqlCommand.ExecuteScalar();


                //Leemos la cuenta corriente y su valor

                
                mySqlCommand.CommandText = "SELECT   ctaId  FROM KfnCuentaCorriente  WITH (UPDLOCK, ROWLOCK) 	 WHERE ageId = " + age_id;
                ctaIdCorriente = (decimal)mySqlCommand.ExecuteScalar();


                
                mySqlCommand.CommandText = "SELECT ctaSaldo FROM KfnCuentaCorriente  WITH (UPDLOCK, ROWLOCK) 	WHERE ageId = " + age_id;
                ctaSaldo = (decimal)mySqlCommand.ExecuteScalar();


                
                mySqlCommand.CommandText = "UPDATE KfnCuentaCorriente WITH(ROWLOCK)  SET ctaSaldo = ctaSaldo + " + amount.ToString().Replace(",", ".") + "  WHERE ctaId = " + ctaId.ToString() + " AND ageId = " + age_id.ToString() + "AND ctaSaldo =" + ctaSaldo.ToString().Replace(",", ".");
                mySqlCommand.ExecuteNonQuery();


                
                //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");+
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =   @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                mySqlCommand.ExecuteNonQuery();


                
                //mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");+
                mySqlCommand.CommandText = "SELECT sec_number_ctacte_mov = sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                sec_number_ctacte_mov = (int)mySqlCommand.ExecuteScalar();

                
                sentencia = "INSERT INTO KfnCuentaCorrienteMovimiento (ccmId,ctaId,ccmFecha,ccmImporte  ,ccmSaldo,ccmDetalle,ccmNumeroTransaccion,ttrId)" +
                            "VALUES ( " + sec_number_ctacte_mov.ToString() + "," + ctaId.ToString() + ", GETDATE()" + "," + amount.ToString().Replace(",", ".") + "," + ctaSaldo.ToString().Replace(",", ".") + ", 'Deposito enviado desde API-Movilway  por el monto: " + amount.ToString().Replace(",", ".") + "'" + "," + sec_number_trans + ", 1000)";

                
                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                
                sentencia = "UPDATE KfnDeposito SET  depEstado = 'AU', depComentarioProcesamiento = 'Deposito enviado desde API-Movilway por el Monto: " + @amount.ToString().Replace(",", ".") + "'" +
                            ",ccmNumeroTransaccion =" + sec_number_trans + ", processingUsrId =" + usr_id + " WHERE  depId =" + traIdReferencia;

                
                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                
                //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                mySqlCommand.ExecuteNonQuery();


                
                //mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                mySqlCommand.CommandText = "SELECT sec_number_audit = sec_number FROM secuencia WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                sec_number_audit = (int)mySqlCommand.ExecuteScalar();


                
                sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                            "VALUES ( " + sec_number_audit + "," + usr_id + ", null, 'Deposito enviado desde API-Movilway por el monto " + amount + "'" + "," +
                            "GETDATE(), " + traIdReferencia.ToString() + ", 'FINANCIERO', 'AUTORIZACION')";
                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                
                //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                mySqlCommand.CommandText = "UPDATE [secuencia] WITH(ROWLOCK) SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                mySqlCommand.ExecuteNonQuery();


                
               // mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                mySqlCommand.CommandText = "SELECT sec_number FROM [secuencia] WITH(NOLOCK) WHERE sec_objectName =   @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                ult_sol_pro = (int)mySqlCommand.ExecuteScalar();


                
               // mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                 mySqlCommand.CommandText = "SELECT sec_number FROM [secuencia] WITH(NOLOCK) WHERE sec_objectName =   @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                sec_number = (int)mySqlCommand.ExecuteScalar();


                /* inserto una solicitud y la coloco en estado pendiente */


                
                sentencia = "INSERT INTO [KlgSolicitudProducto] WITH(ROWLOCK) (sprId,usrId,ageIdSolicitante,ageIdDestinatario,prvIdDestinatario,sprEstado,sprFecha,sprFechaAprobacion,sprImporteSolicitud,sltId,ageIdBodegaOrigen,ageIdBodegaDestino) " +
                            "VALUES (" + ult_sol_pro.ToString() + "," + usr_id.ToString() + "," + IdAgenteSolicitante.ToString() + "," + age_id.ToString() + ", NULL,'AU','" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ",'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + amount.ToString().Replace(",", ".") + ",202," + IdBodegaOrigen.ToString() + "," + IdBodegaDestino.ToString() + ")";

                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                
                sentencia = "INSERT INTO [KlgSolicitudProductoItem] WITH(ROWLOCK) (sprId,prdId,spiCantidadSolicitada,spiCantidadAutorizada,spiPrecioUnitario,spiEstado) " +
                            "VALUES (" + ult_sol_pro + ",0," + amount + "," + amount + ", 1.0000,'PE')";

                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                
                sentencia = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK) SET spiEstado = 'AU', spiCantidadAutorizada = " + amount +
                            "WHERE sprId =" + ult_sol_pro + " AND prdId = 0 AND spiEstado = 'PE'";

                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                
                //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "ENVIOPRODUCTO");
                mySqlCommand.CommandText = "UPDATE [secuencia] WITH(ROWLOCK) SET sec_number = sec_number + 1 WHERE sec_objectName =   @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "ENVIOPRODUCTO");
                mySqlCommand.ExecuteNonQuery();



                mySqlCommand.CommandText = "SELECT sec_number FROM [secuencia] WITH(NOLOCK) WHERE sec_objectName =   @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "ENVIOPRODUCTO");
                ult_env_pro = (int)mySqlCommand.ExecuteScalar();

                
                sentencia = "INSERT INTO [KlgEnvio] WITH(ROWLOCK) (envId,envEstado,envFechaEnvio,envFechaRecepcion,envNumeroRemito,envNumeroFactura,envObservaciones,ageIdBodegaOrigen,ageIdBodegaDestino) " +
                            "VALUES (" + ult_env_pro + ",'DE','" + date.ToString("yyyy-MM-dd HH:mm:ss") + "',NULL,'','','Envio de productos Virtuales en linea'," + IdBodegaOrigen + "," + IdBodegaDestino + ")";

                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();



                
                sentencia = "INSERT INTO [KlgSolicitudProductoEnvio] WITH(ROWLOCK) (sprId,envId)  " +
                            "VALUES (" + ult_sol_pro + "," + ult_env_pro + ")";

                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                
                sentencia = "INSERT INTO [KlgEnvioItem] WITH(ROWLOCK) (envId,prdId,eitCantidad,eitEstado)" +
                            "VALUES (" + ult_env_pro + "," + 0 + "," + amount.ToString().Replace(",", ".") + ",'DE')";

                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = 'DE' WHERE sprId =" + ult_sol_pro + " AND prdId = 0 AND spiEstado = 'AU'";
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId =" + IdAgenteSolicitante + " AND prdId IN (0)";
                StockIdOrigen = (decimal)mySqlCommand.ExecuteScalar();



                
                mySqlCommand.CommandText = " UPDATE [KlgStockAgenteProducto] WITH(ROWLOCK) SET  stkCantidad = stkCantidad + - " + amount + " WHERE  stkId = " + StockIdOrigen + " AND ageId = " + IdAgenteSolicitante + "and stkCantidad + - " + amount + ">= 0";
                mySqlCommand.ExecuteNonQuery();


                
                //mySqlCommand.CommandText = " UPDATE [KlgEnvio] WITH(ROWLOCK) SET envEstado = 'RE', envFechaRecepcion = GETDATE() WHERE envId = " + ult_env_pro + " AND envEstado = 'DE'";
                mySqlCommand.CommandText = " UPDATE [KlgEnvio] WITH(ROWLOCK) SET envEstado = 'RE', envFechaRecepcion = dateadd(minute," + dateDiff + ",getdate()) WHERE envId = " + ult_env_pro + " AND envEstado = 'DE'";
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = "UPDATE [KlgEnvioItem] WITH(ROWLOCK) SET eitEstado = 'RE'  WHERE envId = " + ult_env_pro + " AND prdId = 0 AND eitEstado = 'DE'";
                mySqlCommand.ExecuteNonQuery();



                
                mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = 'RE'  WHERE sprId = " + ult_sol_pro + "  AND prdId = 0";
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId =" + age_id + " AND prdId IN (0)";
                StockIdDestino = (decimal)mySqlCommand.ExecuteScalar();



                
                mySqlCommand.CommandText = "UPDATE KlgStockAgenteProducto SET  stkCantidad = stkCantidad + " + amount + "  WHERE  stkId =" + StockIdDestino + " AND ageId =" + age_id + " and stkCantidad + " + amount + ">= 0 ";
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = " UPDATE KlgSolicitudProducto WITH(ROWLOCK) SET sprEstado = 'CE' WHERE sprId = " + ult_sol_pro + "  AND sprEstado = 'AU' AND ( ageIdSolicitante =" + age_id + " OR ageIdDestinatario =" + age_id + ")";
                mySqlCommand.ExecuteNonQuery();


                //Fin de proceso de Acreditacion  


                // {Inicio el proceso para registar en cta cte


                
                //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA"); 
                mySqlCommand.ExecuteNonQuery();



                mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA"); 
                sec_number_auditoria = (int)mySqlCommand.ExecuteScalar();


                
                sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                            "VALUES ( " + sec_number_auditoria + ", " + usr_id + ", null, 'Despacho de productos nro." + ult_env_pro + "'" + " , GETDATE()," + ult_env_pro + ",'DISTRIBUCION', 'ENVIOPRODUCTO')";

                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                // --Volvemos a leer el saldo de la cta cte. 

                
                mySqlCommand.CommandText = "SELECT  ctaId   FROM KfnCuentaCorriente  WITH (NOLOCK) WHERE ageId =" + age_id;
                ctaId = (decimal)mySqlCommand.ExecuteScalar();



                
                mySqlCommand.CommandText = "SELECT  ctaSaldo   FROM KfnCuentaCorriente  WITH (NOLOCK) WHERE ageId =" + age_id;
                ctaSaldo = (decimal)mySqlCommand.ExecuteScalar();


                
               // mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION"); 
                mySqlCommand.ExecuteNonQuery();



                mySqlCommand.CommandText = "SELECT sec_number_aut = sec_number FROM secuencia WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION"); 
                sec_number_aut = (int)mySqlCommand.ExecuteScalar();

                
                mySqlCommand.CommandText = "UPDATE KfnCuentaCorriente WITH(ROWLOCK) SET ctaSaldo = ctaSaldo + - " + amount + " WHERE ctaId =" + ctaId + " AND ageId = " + age_id + " AND ctaSaldo =" + ctaSaldo + " AND ( (ctaSaldo + - " + amount + " >= 0) OR (ABS(ctaSaldo + - " + amount + ") <= 600) )";
                mySqlCommand.ExecuteNonQuery();


                
                //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO"); 
                mySqlCommand.ExecuteNonQuery();


                mySqlCommand.CommandText = "SELECT   sec_number FROM secuencia WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO"); 
                sec_numbercta_movi = (int)mySqlCommand.ExecuteScalar();


                
                sentencia = "INSERT INTO KfnCuentaCorrienteMovimiento (ccmId,ctaId,ccmFecha,ccmImporte  ,ccmSaldo,ccmDetalle,ccmNumeroTransaccion,ttrId)" +
                            "VALUES ( " + sec_numbercta_movi + "," + ctaId + ",GETDATE()  ," + -amount + "," + ctaSaldo.ToString() + ",'Asignacion de productos No.: " + ult_sol_pro.ToString() + "'" + "," + sec_number_aut.ToString() + ", 2000)";

                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                
               // mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                mySqlCommand.CommandText = " Update secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =   @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA"); 
                mySqlCommand.ExecuteNonQuery();





                mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA"); 
                sec_number_auditoria = (int)mySqlCommand.ExecuteScalar();



                
                sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                            "VALUES (" + sec_number_auditoria + "," + usr_id + ", null, 'Asignacion de productos',  GETDATE()," + ult_sol_pro + ", 'DISTRIBUCION', 'SOLICITUDPRODUCTO')";

                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();

                // }Fin del proceso para registar en cta cte


                //	 Devolvemos el nuevo saldo al cliente

                
                mySqlCommand.CommandText = " SELECT  stkCantidad = stkCantidad FROM   [KlgStockAgenteProducto] WITH (NOLOCK)  WHERE  ageId = " + age_id + " AND  prdId = 0  ";
                stkCantidad = (decimal)mySqlCommand.ExecuteScalar();

                autorization = ult_sol_pro.ToString();
                Response_Code = "00";
                Message = "TRANSACCION OK;" + AgenteNombre;
                tran.Commit();
                return true;

            }
            catch (Exception ex)
            {
                
                autorization = "";
                Response_Code = "01";
                Message = "ERROR AL TRANSFERIR SALDO";
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                return false;

            }
            finally { mySqlConnection.Close(); }
        }

        public ExecuteDistributionResponse ExecuteDistribution(string id_net, string referenceNumber, DateTime dateTime, string agentReceiver, decimal amount, string account)
        {
            logger.InfoHigh("NewFlag 1");

            logger.InfoHigh("id_net " + id_net + " - refNumber " + referenceNumber + " - dateTime " + dateTime + " - agentReceiver " + agentReceiver + " - amount " + amount + " - account " + account);

            logger.InfoHigh("NewFlag 2");

            SqlCommand mySqlCommand = new SqlCommand();

            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            SqlConnection cnn = new SqlConnection(strConnString);
            mySqlCommand.CommandTimeout = 300;
            SqlCommand cmd = new SqlCommand("Generate_BuyStock", cnn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@msg", SqlDbType.VarChar)).Value = "0100";
            cmd.Parameters.Add(new SqlParameter("@id_net", SqlDbType.VarChar)).Value = id_net;
            cmd.Parameters.Add(new SqlParameter("@reference_number", SqlDbType.VarChar)).Value = referenceNumber;
            cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = dateTime;
            cmd.Parameters.Add(new SqlParameter("@agent_reciver", SqlDbType.VarChar)).Value = agentReceiver;
            cmd.Parameters.Add(new SqlParameter("@amount", SqlDbType.Decimal)).Value = amount;
            cmd.Parameters.Add(new SqlParameter("@cuenta", SqlDbType.VarChar)).Value = account;

            SqlParameter prmMessage = new SqlParameter("@o_Message", SqlDbType.VarChar, -1);
            prmMessage.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(prmMessage);

            SqlParameter prmResponseCode = new SqlParameter("@o_Response_Code", SqlDbType.VarChar, -1);
            prmResponseCode.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(prmResponseCode);

            SqlParameter prmNewBalance = new SqlParameter("@o_new_balance", SqlDbType.Float);
            prmNewBalance.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(prmNewBalance);

            SqlParameter prmNameAgent = new SqlParameter("@o_name_agent", SqlDbType.VarChar, -1);
            prmNameAgent.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(prmNameAgent);

            SqlParameter prmAutorization = new SqlParameter("@o_autorization", SqlDbType.VarChar, -1);
            prmAutorization.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(prmAutorization);

            ExecuteDistributionResponse response = new ExecuteDistributionResponse();
            try
            {
                cnn.Open();
                cmd.ExecuteNonQuery();
                //response. = cmd.Parameters["@o_name_agent"].Value.ToString();
                response.responseCode = cmd.Parameters["@o_Response_Code"].Value.ToString();
                response.Message = cmd.Parameters["@o_Message"].Value.ToString();
                response.authorizationNumber = cmd.Parameters["@o_autorization"].Value.ToString();
                response.newBalance = decimal.Parse(cmd.Parameters["@o_new_balance"].Value.ToString());
            }
            catch (Exception ex)
            {
                ////Logger.LogInfo("SendRegisterDeposit " + oRequest.reference_number + ": " + "Error inesperado al ejecutar  Generate_BuyStock . Error recibido: " + ex.Message, LoggingLevelType.High);
                response.Message = "ERROR DE BASE DE DATOS";
                response.responseCode = "98";
                response.authorizationNumber = "0";
                response.newBalance = 0;
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", response.responseCode, "-", response.Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                //oResponse.Message = "ERROR DE BASE DE DATOS";  // para el log ex.Message.ToString();
                //oResponse.Response_Code = "98";
                //oResponse.Result = false;
                //return oResponse;
            }
            finally { cnn.Close(); }

            return response;
        }

        public string GetAgentPdv(string agentReference)
        {
#if DEBUG
            return "134";
#endif
            string agentPdv = "";

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@"SELECT age_pdv FROM Agente ag with (NOLOCK) 
                            JOIN Usuario u with (NOLOCK) on u.age_id=ag.age_id
                            JOIN Acceso ac with (NOLOCK) on ac.usr_id=u.usr_id
                            WHERE ac.acc_login='{0}' "), agentReference);

                //logger.InfoHigh(query);
                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                agentPdv = (string)command.ExecuteScalar();
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el resúmen en la base de datos de Kinacu"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return (agentPdv);
        }

        public int GetUserId(string agentReference)
        {
            int userId = 0;

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@"SELECT CAST(ac.usr_id as varchar)
                            FROM Acceso ac with (NOLOCK) 
                            WHERE ac.acc_login='{0}' "), agentReference);

                //logger.InfoHigh(query);
                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                userId = int.Parse((string)command.ExecuteScalar());
            }
            catch (Exception e)
            {
                //logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el UserId de una agencia en la base de datos de Kinacu"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return (userId);
        }

        public int GetAgentId(string agentReference)
        {
            int agentId = 0;

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@"SELECT CAST(u.age_id as varchar)
                            FROM Acceso ac with (NOLOCK)
                            JOIN Usuario u with (NOLOCK) on u.usr_id=ac.usr_id
                            WHERE ac.acc_login='{0}' "), agentReference);

                //logger.InfoHigh(query);
                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                agentId = int.Parse((string)command.ExecuteScalar());
            }
            catch (Exception e)
            {
                //logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el AgentId de una agencia en la base de datos de Kinacu"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return (agentId);
        }

        internal int GetAgentIdByPdv(string recipientPdv)
        {
            int agentId = 0;

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@"SELECT count(1)
                            FROM Agente with (NOLOCK)
                            WHERE age_estado='AC' and [age_pdv]='{0}' "), recipientPdv);

                var query2 =
                    String.Format(
                        (@"SELECT age_id
                            FROM Agente with (NOLOCK)
                            WHERE age_estado='AC' and [age_pdv]='{0}' "), recipientPdv);

                //logger.InfoHigh(query);
                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);

                var countRecords = (int)command.ExecuteScalar();

                if (countRecords == 1)
                {
                    command = new SqlCommand(query2, sqlConnection);
                    agentId = int.Parse(command.ExecuteScalar().ToString());
                }
                else
                    throw new Exception("Se hallaron múltiples agencias con el mismo PDV " + recipientPdv);
            }
            catch (Exception e)
            {
                //logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el AgentId de una agencia en la base de datos de Kinacu"));
                throw e;
            }
            finally
            {
                sqlConnection.Close();
            }
            return (agentId);
        }

        public string GetAgentMobilePhone(int agentId)
        {
            string mobilePhone;

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@"SELECT CAST(ag.age_cel as varchar)
                            FROM Agente ag with (NOLOCK)
                            WHERE ag.age_id={0} "), agentId);

                //logger.InfoHigh(query);
                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                mobilePhone = (string)command.ExecuteScalar();
            }
            catch (Exception e)
            {
                //logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el AgentId de una agencia en la base de datos de Kinacu"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return mobilePhone.Contains('-') ? mobilePhone.Split('-')[0] : mobilePhone;
        }

        public decimal GetAgentFinalStock(int agentId)
        {
            decimal stockFinal;

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@"SELECT stkCantidad
                            FROM [KlgStockAgenteProducto] with (NOLOCK) 
                            where ageidpropietario={0}"), agentId);

                //logger.InfoHigh(query);
                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                stockFinal = (decimal)command.ExecuteScalar();
            }
            catch (Exception e)
            {
                //logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el AgentId de una agencia en la base de datos de Kinacu"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return stockFinal;
        }

        public bool CheckReverse(string agentReference)
        {
            bool acceptReverse = false;

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@" SELECT [ataValor]
                              FROM [KcrAtributoAgencia]
                              WHERE ageId = {0}
                              and attId = 'QuitaAutomatica'"), agentReference);

                //logger.InfoHigh(query);
                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                acceptReverse = bool.Parse((string)command.ExecuteScalar());
            }
            catch (Exception e)
            {
                //logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el AgentId de una agencia en la base de datos de Kinacu"));
                return false;
            }
            finally
            {
                sqlConnection.Close();
            }
            return (acceptReverse);
        }

        public bool RegistroPago(decimal age_id, int usr_id, decimal amount, string reference_number, DateTime date, string cuenta, string condeposito, ref string Response_Code, ref string Message, string Comentario, decimal depId, DateTime fecha_aprobacion)
        {
            string age_estado = string.Empty;
            decimal cubId = 0;
            string cubNumero = string.Empty;
            decimal ctaSaldo;
            decimal ctaId;
            string AgenteNombre = string.Empty;
            string sentencia = string.Empty;
            string age_comisionadeposito = string.Empty;
            decimal age_montocomision = 0;
            decimal LimiteCredito = 0;
            decimal credito = 0;
            string sLimiteCredito = string.Empty;
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            logger.InfoLow(() => TagValue.New().Message("RegistroPago"));
            SqlConnection mySqlConnection = new SqlConnection(strConnString);
            SqlDataReader reader = null;
            try
            {
                mySqlConnection.Open();
                SqlCommand mySqlCommand = new SqlCommand();
                mySqlCommand.Connection = mySqlConnection;
                try
                {
                   //mySqlCommand.CommandText = "SELECT age_estado FROM agente WITH(NOLOCK) WHERE age_id='" + age_id + "'";
                    mySqlCommand.CommandText = "SELECT age_estado,age_montocomision,age_comisionadeposito FROM agente WITH(NOLOCK) WHERE age_id=@age_id";
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    //age_estado = mySqlCommand.ExecuteScalar().ToString();

                    using (reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            age_estado = reader["age_estado"].ToString();
                            age_montocomision = Convert.ToDecimal( reader["age_montocomision"]);
                            age_comisionadeposito = reader["age_comisionadeposito"].ToString();
                        }
                        else
                        {
                            Message = "NO SE PUDIERON LEER LOS DATOS DE LA CUENTA";
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            return false;
                        }
                    }

                    if (age_estado == "SU" || age_estado == "DE")
                    {
                        Response_Code = "01";
                        Message = "AGENCIA DESTINO SUSPENDIDA O INACTIVA " + AgenteNombre;
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Response_Code = "02";
                    Message = "AGENCIA DESTINO SUSPENDIDA O INACTIVA " + AgenteNombre;
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    return false;
                }

                /*
                try
                {
                    //mySqlCommand.CommandText = "SELECT  age_montocomision FROM  agente  WITH(NOLOCK)  WHERE    age_id 	= '" + age_id + "'";
                    mySqlCommand.CommandText = "SELECT age_montocomision FROM agente WITH(NOLOCK) WHERE age_id=@age_id ";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    age_montocomision = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "03";
                    Message = "MONTO DE COMISIÓN NO EXISTE";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    return false;
                }


                try
                {
			        //mySqlCommand.CommandText = "SELECT  age_comisionadeposito FROM  agente  WITH(NOLOCK)  WHERE    age_id 	= '" + age_id + "'";
                    mySqlCommand.CommandText = "SELECT age_comisionadeposito FROM  agente  WITH(NOLOCK)  WHERE age_id=@age_id";
                    mySqlCommand.Parameters.Clear();
				    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    age_comisionadeposito = mySqlCommand.ExecuteScalar().ToString();
                }
                catch (Exception ex)
                {

                    Response_Code = "04";
                    Message = "COMISION EN DEPOSITO NO EXISTE";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    return false;
                }
                 */ 

                Response_Code = "05";

                try
                {
                    //mySqlCommand.CommandText = "SELECT cubId    FROM dbo.KfnCuentaBanco WITH(ROWLOCK)  WHERE cubNumero = '" + cuenta + "' and ageId=0";
                    
                    bool checkaccountsinroot  = String.IsNullOrEmpty(ConfigurationManager.AppSettings["CheckAccountsInRoot"]) ? true : bool.Parse(ConfigurationManager.AppSettings["CheckAccountsInRoot"]);

                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@cuenta", cuenta);

                    if (checkaccountsinroot)
                    {
                        mySqlCommand.CommandText = "SELECT cubId ,cubNumero   FROM dbo.KfnCuentaBanco WITH(NOLOCK)  WHERE cubNumero = @cuenta  and ageId = @age_id";
                        mySqlCommand.Parameters.AddWithValue("@age_id", cons.AGENTE_CONCENTRADOR);
                    }
                    else
                    {
                        mySqlCommand.CommandText = "SELECT cubId ,cubNumero   FROM dbo.KfnCuentaBanco WITH(NOLOCK)  WHERE cubNumero = @cuenta  and ageId = (SELECT age_id_sup FROM dbo.Agente with(NOLOCK) WHERE age_id=@age_id)";
                         mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    }
                    
                    using (reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            cubId = Convert.ToInt32(reader["cubId"]); 
                            cubNumero = reader["cubNumero"].ToString();
                        }
                        else
                        {
                            Message = "NO SE PUDIERON LEER LOS DATOS DE LA CUENTA";
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            return false;
                        }
                    }
                    //cubId = (decimal)mySqlCommand.ExecuteScalar();


                    
                    // mySqlCommand.CommandText = "SELECT cubNumero    FROM dbo.KfnCuentaBanco WITH(ROWLOCK)  WHERE cubNumero = '" + cuenta + "' and ageId=0";
                    /*   mySqlCommand.CommandText = "SELECT  cubNumero FROM dbo.KfnCuentaBanco WITH (ROWLOCK)  WHERE cubNumero = @cuenta and ageId=0";
                       mySqlCommand.Parameters.Clear();
                       mySqlCommand.Parameters.AddWithValue("@cuenta", cuenta);
                       cubNumero = mySqlCommand.ExecuteScalar().ToString();
                     */

                    //sequita validacion por que no aporta
                    /*
                    if (cubNumero.Trim() != cuenta.Trim())
                    {
                     
                        //LA CUENTA BANCARIA ESTA PARAMETRIZADA POR LA CONFIGURACION DE LA APLICACION
                        Message = "LOS DATOS DE LA CUENTA BANCARIA NO CONCUERDAN";//"CUENTA NO EXISTE";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                     
                        return false;
                    }
                     * */

                }
                catch (Exception ex)
                {
                    Response_Code = "06";
                    Message = "ERROR BUSCANDO LOS DATOS DE LA CUENTA BANCARIA";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    return false;
                }


                try
                {
               
                    //mySqlCommand.CommandText = "SELECT  ctaId FROM KfnCuentaCorriente  WITH(ROWLOCK)  WHERE    ageid 	= '" + age_id + "'";
                    mySqlCommand.CommandText = " SELECT  ctaId,ctaSaldo FROM KfnCuentaCorriente  WITH (ROWLOCK) WHERE  ageid=@age_id ";
                    mySqlCommand.Parameters.Clear();
					mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                   // ctaId = (decimal)mySqlCommand.ExecuteScalar();
                    using (reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            ctaId = (decimal)reader["ctaId"];
                            ctaSaldo = (decimal)reader["ctaSaldo"];
                        }
                        else
                        {
                            Message = "NO SE PUDIERON LEER LOS DATOS DE LA CUENTA CORRIENTE";
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response_Code = "07";
                    Message = "ERROR SELECCIONADO LOS DATOS DE CUENTA CTA. CTE.";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    return false;
                }
                /*
                try
                {
               
                    
                   // mySqlCommand.CommandText = "SELECT  ctaSaldo FROM KfnCuentaCorriente  WITH(ROWLOCK)   WHERE    ageid 	= '" + age_id + "'";
                    mySqlCommand.CommandText = " SELECT  ctaSaldo FROM KfnCuentaCorriente  WITH (ROWLOCK) WHERE ageid=@age_id";
                    mySqlCommand.Parameters.Clear();
					mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    ctaSaldo = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "08";
                    Message = "SALDO EN CTA. CTE. NO EXISTE";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    return false;
                }
                */


                //try
                //{
                //    
                //    //mySqlCommand.CommandText = "SELECT  convert(int,usr_id_modificacion) FROM  agente  WITH(NOLOCK)  WHERE   age_id 	=  '" + age_id + "'";
                //    mySqlCommand.CommandText = "SELECT  convert(int,usr_id) FROM  agente  WITH(NOLOCK)  WHERE   age_id 	=  '" + age_id + "'";
                //    usr_id = (int)mySqlCommand.ExecuteScalar();
                //}
                //catch (Exception ex)
                //{
                //    Response_Code = "10";
                //    Message = "USUARIO NO REGISTRADO ";
                //    return false;
                //}
       

                if (condeposito == "S")
                {

                    try
                    {

                        SqlTransaction tran = null;
                        tran = mySqlConnection.BeginTransaction();
                       
                        if (Acreditacion(age_id, amount, usr_id, reference_number, date, cuenta, condeposito, ctaId, cubId, ref Response_Code, ref  Message, Comentario, depId, fecha_aprobacion,mySqlConnection,tran))
                        {
                            if ((age_comisionadeposito == "S") && (Response_Code == "00"))
                            {
                                if (LiquidaComisionKinacu(age_id, usr_id, amount, fecha_aprobacion, age_montocomision, ref Response_Code, ref Message, mySqlConnection, tran))
                                {
                                    tran.Commit();
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                tran.Commit();
                                return true;
                            }
                        }
                        else // Si el proceso de acreditacion falla
                        {
                            return false;

                        }
                        
                    }
                    catch(Exception ex){
                        Response_Code = "09";
                        Message = "ERROR INESPERADO ";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        return false;
                    }
                }
                else // Si no tiene deposito la trx
                {
                    
                    //mySqlCommand.CommandText = "SELECT  ataValor FROM dbo.KcrAtributoAgencia  WITH(ROWLOCK)   WHERE    ageid 	= '" + age_id + "' and attId='LimiteCredito'";
                    try
                    {
                        mySqlCommand.CommandText = "SELECT  ataValor FROM dbo.KcrAtributoAgencia  WITH(ROWLOCK)   WHERE    ageid 	=  @age_id  and attId=@attId";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                        mySqlCommand.Parameters.AddWithValue("@attId", "LimiteCredito");
                        sLimiteCredito = mySqlCommand.ExecuteScalar().ToString();
                        LimiteCredito = Convert.ToDecimal(sLimiteCredito);
                        credito = LimiteCredito + ctaSaldo;
                    }
                    catch (Exception ex)
                    {
                        Response_Code = "11";
                        Message = "ERROR AL SELECCIONAR LIMITE VALORE DE KCRATRIBUTOAGENCIA";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        return false;
                    }

                    if (amount <= credito)
                    {
                        
                        try
                        {   
                            SqlTransaction tran = null;
                            tran = mySqlConnection.BeginTransaction();

                            if (AcreditacionCredito(age_id, amount, usr_id, reference_number, date, cuenta, condeposito, ref Response_Code, ref  Message, fecha_aprobacion, mySqlConnection, tran))
                            {
                                if ((age_comisionadeposito == "S") && (Response_Code == "00"))
                                {
                                    if (LiquidaComisionKinacu(age_id, usr_id, amount, date, age_montocomision, ref Response_Code, ref Message, mySqlConnection, tran))
                                    {
                                        tran.Commit();
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    tran.Commit();
                                    return true;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        catch(Exception ex){
                            Response_Code = "11";
                            Message = "ERROR INESPERADO ";
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                            return false;
                        }
                    }
                    else
                    {
                        Response_Code = "12";
                        Message = "LA AGENCIA NO POSEE LIMITE DE CREDITO PARA ESTA TRANSACCION";
                        return false;
                    }
                }
          
           

            }

            catch (Exception ex)
            {
                Response_Code = "13";
                Message = "ERROR AL TRANSFERIR SALDO";
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                return false;
            }
            finally { mySqlConnection.Close(); }
        }

    
       /// <summary>
        /// Metodo para ser llamado desde el api, para registrar deposito y distribuir saldo
       /// </summary>
       /// <param name="age_id"></param>
       /// <param name="usr_id"></param>
       /// <param name="amount"></param>
       /// <param name="reference_number"></param>
       /// <param name="date"></param>
       /// <param name="cuenta"></param>
       /// <param name="Response_Code"></param>
       /// <param name="Message"></param>
       /// <param name="Comentario"></param>
       /// <param name="fecha_aprobacion"></param>
       /// <param name="numero_sucursal"></param>
       /// <param name="nombre_sucursal"></param>
       /// <returns>true si pudo realizar la operacion false de lo contrario</returns>
        public bool RegistroDepositoAcreditaSaldo(decimal age_id, int usr_id, decimal amount, string reference_number, DateTime date, string cuenta, ref string Response_Code, ref string Message, string Comentario, DateTime fecha_aprobacion, string numero_sucursal, string nombre_sucursal)
        {
            string age_estado = string.Empty;
            string age_comisionadeposito = string.Empty;
            decimal age_montocomision = 0;


            decimal cubId = 0;
            string cubNumero = string.Empty;
            decimal ctaId;
            string AgenteNombre = string.Empty;
            string sentencia = string.Empty;
            decimal age_sup = 0;
         
            string sLimiteCredito = string.Empty;
            //int usr_id;
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
       
            logger.InfoLow(() => TagValue.New().Tag("[OP]").Value("RegistroDepositoAcreditaSaldo"));
            //variable axiliar para el manejo del codigo de error
            int ResponseCode = 01;
            object escalaraux = null;

            //VARIABLES PARA VALIDAR LA COMISIONES
            decimal PorcentajeCom = 0m;//= PorcentajeCom / 100;
            decimal MontoComision = 0m;// MontoSolicitud* PorcentajeCom;


            try
            {
                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {


                    SqlCommand mySqlCommand = new SqlCommand();
                    mySqlConnection.Open();
                    mySqlCommand.Connection = mySqlConnection;


                    //label de error
                    ResponseCode = 2;
                    try
                    {

                        //   mySqlCommand.CommandText = "SELECT  age_estado FROM  agente  WITH(NOLOCK)  WHERE   age_id 	=  '" + age_id + "'";

                        //CADA VALORE SE LEIA EN UNA PETICION
                        mySqlCommand.CommandText = "SELECT  age_estado,age_montocomision,age_comisionadeposito,age_id_sup FROM  agente  WITH(NOLOCK)  WHERE   age_id 	= @age_id ";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@age_id", age_id);

                        using (var reader = mySqlCommand.ExecuteReader())
                        {
                            if(reader.HasRows && reader.Read()){
                                age_estado = reader["age_estado"].ToString();
                                age_montocomision = Convert.ToDecimal( reader["age_montocomision"]);
                                age_comisionadeposito = reader["age_comisionadeposito"].ToString();
                                age_sup = Convert.ToDecimal(reader["age_id_sup"]);
                            }
                            else
                            {

                                Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                                Message = string.Concat("NO SE ENCONTRARON DATOS DEL AGENTE ", age_id);
                                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                                return false;
                            }
                          
                        }
                       

                        if (age_estado.Equals("SU") || age_estado.Equals("DE"))
                        {
                            Response_Code = UtilResut.StrBussErrorCode(ResponseCode); //"02";
                            Message = String.Concat("AGENCIA DESTINO SUSPENDIDA O INACTIVA ", age_id);
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            return false;
                        }
                       
                    }
                    catch (Exception ex)
                    {
                        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                        Message = string.Concat("ERROR INESPERADO SELECCIONADO DATOS DE AGENCIA");
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        return false;
                    }

                  

                    ResponseCode = 3;
                    try
                    {
                        /*
                        mySqlCommand.CommandText = "SELECT cubId    FROM dbo.KfnCuentaBanco WITH(ROWLOCK)  WHERE cubNumero = '" + cuenta + "' and ageId=0";
                        cubId = (decimal)mySqlCommand.ExecuteScalar();
                        mySqlCommand.CommandText = "SELECT cubNumero    FROM dbo.KfnCuentaBanco WITH(ROWLOCK)  WHERE cubNumero = '" + cuenta + "' and ageId=0";
                        cubNumero = mySqlCommand.ExecuteScalar().ToString();*/

                        bool checkAccountsInRoot = true;
                        checkAccountsInRoot = String.IsNullOrEmpty(ConfigurationManager.AppSettings["CheckAccountsInRoot"]) ? true : bool.Parse(ConfigurationManager.AppSettings["CheckAccountsInRoot"]);

                        if (checkAccountsInRoot)
                            mySqlCommand.CommandText = "SELECT cubId ,cubNumero   FROM dbo.KfnCuentaBanco WITH(NOLOCK)  WHERE cubNumero = @cuenta  and ageId = @age_id";
                        else
                            mySqlCommand.CommandText = "SELECT cubId ,cubNumero   FROM dbo.KfnCuentaBanco WITH(NOLOCK)  WHERE cubNumero = @cuenta  and ageId = (SELECT age_id_sup FROM dbo.Agente with(NOLOCK) WHERE age_id=@age_id)";

                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@cuenta", cuenta);

                        if (checkAccountsInRoot)
                            mySqlCommand.Parameters.AddWithValue("@age_id", cons.AGENTE_CONCENTRADOR);
                        else
                            mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                        
                        using (var reader = mySqlCommand.ExecuteReader())
                        {
                            if (reader.HasRows && reader.Read())
                            {
                                cubId = (decimal)reader["cubId"];
                                cubNumero = reader["cubNumero"].ToString();
                            }
                            else
                            {
                                Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                                Message = string.Concat("NO SE ENCONTRARON DATOS DEL AGENTE CONCENTRADOR");
                                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                                return false;
                            }
                        }

                        if (cubNumero.Trim() != cuenta.Trim())
                        {
                            Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                            Message = "LA CUENTA INGRESADA NO COINCIDE CON LA CUENTA DEL AGENTE CONCENTRADOR";
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, " ", Message));
                            return false;
                        }

                    }
                    catch (Exception ex)
                    {
                        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                        Message = "ERROR INESPERADO SELECCIONADO DATOS CUENTA AGENTE CONCENTRADOR";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        return false;
                    }


                    ResponseCode = 4;
                    try
                    {
                        /*
                        mySqlCommand.CommandText = "SELECT  ctaId FROM KfnCuentaCorriente  WITH(ROWLOCK)  WHERE    ageid 	= '" + age_id + "'";
                        ctaId = (decimal)mySqlCommand.ExecuteScalar();
                         * */

                        mySqlCommand.CommandText = "SELECT  ctaId FROM KfnCuentaCorriente WITH(NOLOCK)  WHERE  ageid = @age_id ";
                        mySqlCommand.Parameters.Clear();
          
                        mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                        ctaId = (decimal)mySqlCommand.ExecuteScalar();

                    }
                    catch (Exception ex)
                    {
                        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                        Message = "ERROR INESPERADO SELECCIONADO CUENTA CTA. CTE. NO EXISTE";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        return false;
                    }



                    //try
                    //{
                    //    
                    //    //mySqlCommand.CommandText = "SELECT  convert(int,usr_id_modificacion) FROM  agente  WITH(NOLOCK)  WHERE   age_id 	=  '" + age_id + "'";
                    //    mySqlCommand.CommandText = "SELECT  convert(int,usr_id) FROM  agente  WITH(NOLOCK)  WHERE   age_id 	=  '" + age_id + "'";
                    //    usr_id = (int)mySqlCommand.ExecuteScalar();
                    //}
                    //catch (Exception ex)
                    //{
                    //    Response_Code = "10";
                    //    Message = "USUARIO NO REGISTRADO ";
                    //    return false;
                    //}


                    //VALIDACION SALDO PADRE 
                    ResponseCode = 5;
                    try
                    {
                        //SELECCIONA AGENTE SUPERIOD DEL AGENTE
                        //mySqlCommand.CommandText = "SELECT age_id_sup  FROM [Agente] WITH(NOLOCK)   WHERE age_id = " + age_id;
                        mySqlCommand.CommandText = @"SELECT stkId FROM [KlgStockAgenteProducto] WITH(NOLOCK) WHERE prdId = @prdId  AND ageId = @age_sup and stkCantidad + - @amount >= 0";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@prdId", cons.MULTI_PRODUCTO);
                        mySqlCommand.Parameters.AddWithValue("@age_sup", age_sup);
                        mySqlCommand.Parameters.AddWithValue("@amount", amount);
                        escalaraux = mySqlCommand.ExecuteScalar();
                        if (escalaraux == null)
                        {
                        
                            Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                            Message = "LA AGENCIA SUPERIOR NO TIENE SUFICIENTE SALDO PARA REALIZAR LA OPERACION";
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                    
                        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                        Message = "ERROR INESPERADO VALIDANDO SALDO AGENTE PADRE";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        return false;

                    }



                    //VALIDACION SALDO HIJO
                    ResponseCode = 6;
                    try
                    {
                        //SELECCIONA AGENTE SUPERIOD DEL AGENTE
                        //mySqlCommand.CommandText = "SELECT age_id_sup  FROM [Agente] WITH(NOLOCK)   WHERE age_id = " + age_id;
                        mySqlCommand.CommandText = "SELECT stkId FROM [KlgStockAgenteProducto] WITH(NOLOCK)  WHERE prdId = @prdId  AND ageId = @age_id and stkCantidad + @amount >= 0";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@prdId", cons.MULTI_PRODUCTO);
                        mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                        mySqlCommand.Parameters.AddWithValue("@amount", amount);
                        escalaraux = mySqlCommand.ExecuteScalar();
                        if (escalaraux == null)
                        {
                        
                            Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                            Message = "LA AGENCIA NO TIENE UN SALDO VALIDO PARA REALIZAR LA OPERACION (SALDO NEGATIVO)";
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                 
                        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                        Message = "ERROR INESPERADO VALIDANDO SALDO AGENCIA";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        return false;

                    }



                    if (age_comisionadeposito.Equals("S") && age_montocomision > 0m)
                    {

                        //validacion de comisiones
                        PorcentajeCom = age_montocomision / 100;
                        //MONTO + MONTO DE COMISION
                        MontoComision = amount + (amount * PorcentajeCom);

                        //VALIDACION SALDO PADRE PARA COMISION
                        ResponseCode = 7;
                        try
                        {
                            //SELECCIONA AGENTE SUPERIOD DEL AGENTE
                            //mySqlCommand.CommandText = "SELECT age_id_sup  FROM [Agente] WITH(NOLOCK)   WHERE age_id = " + age_id;
                            //   UPDATE  [dbo].[KlgStockAgenteProducto] WITH(ROWLOCK) SET  stkCantidad = stkCantidad - @MontoComision WHERE  stkId = @StockIdOrigen  AND ageId = @IdAgenteSolicitante and stkCantidad - @MontoComision >= 0 
                            mySqlCommand.CommandText = @"SELECT stkId FROM [KlgStockAgenteProducto] WITH(NOLOCK) WHERE  prdId = @prdId  AND ageId = @age_sup AND stkCantidad - @MontoComision >= 0";
                            mySqlCommand.Parameters.Clear();
                            mySqlCommand.Parameters.AddWithValue("@prdId", cons.MULTI_PRODUCTO);
                            mySqlCommand.Parameters.AddWithValue("@age_sup", age_sup);
                            mySqlCommand.Parameters.AddWithValue("@MontoComision", MontoComision);
                            escalaraux = mySqlCommand.ExecuteScalar();
                            if (escalaraux == null)
                            {

                                Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                                Message = "LA AGENCIA SUPERIOR NO TIENE SUFICIENTE SALDO PARA REALIZAR LA OPERACION E INCLUIR LA COMISION";
                                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {

                            Response_Code = UtilResut.StrErrorCode(ResponseCode);
                            Message = "ERROR INESPERADO VALIDANDO SALDO AGENTE SUPERIOR COMISION";
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                            return false;

                        }

                        ResponseCode = 8;
                        try
                        {
                            //SELECCIONA AGENTE SUPERIOD DEL AGENTE
                            //mySqlCommand.CommandText = "SELECT age_id_sup  FROM [Agente] WITH(NOLOCK)   WHERE age_id = " + age_id;
                            //   UPDATE  [dbo].[KlgStockAgenteProducto] WITH(ROWLOCK) SET  stkCantidad = stkCantidad - @MontoComision WHERE  stkId = @StockIdOrigen  AND ageId = @IdAgenteSolicitante and stkCantidad - @MontoComision >= 0 
                            mySqlCommand.CommandText = @"SELECT stkId FROM [KlgStockAgenteProducto] WITH(NOLOCK) WHERE  prdId = @prdId  AND ageId = @age_Id and stkCantidad + @MontoComision >= 0";
                            mySqlCommand.Parameters.Clear();
                            mySqlCommand.Parameters.AddWithValue("@prdId", cons.MULTI_PRODUCTO);
                            mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                            mySqlCommand.Parameters.AddWithValue("@MontoComision", MontoComision);
                            escalaraux = mySqlCommand.ExecuteScalar();
                            if (escalaraux == null)
                            {

                                Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                                Message = "LA AGENCIA NO TIENE SUFICIENTE SALDO VALIDO PARA INCLUIR LA COMISION (SALDO NEGATIVO)";
                                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {

                            Response_Code = UtilResut.StrErrorCode(ResponseCode);
                            Message = "ERROR INESPERADO VALIDANDO SALDO AGENTE COMISION";
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                            return false;

                        }

                    }


                    //se comienza la transaccion
                    SqlTransaction tran = mySqlConnection.BeginTransaction();

                    if (AcreditacionRegistroDeposito(age_id, amount, usr_id, reference_number, date, cuenta, ctaId, cubId, ref Response_Code, ref  Message, Comentario, fecha_aprobacion, numero_sucursal, nombre_sucursal,mySqlConnection, tran))
                    {
                        if ((age_comisionadeposito.Equals("S")) && age_montocomision > 0m && (Response_Code.Equals("00")))
                        {
                            if (LiquidaComisionKinacu(age_id, usr_id, amount, fecha_aprobacion, age_montocomision, ref Response_Code, ref Message,mySqlConnection, tran))
                            {
                                tran.Commit();
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            tran.Commit();
                            return true;
                        }
                    }
                    else // Si el proceso de acreditacion falla
                    {
                        return false;

                    }
                }

               

            }
            catch (SqlException ex)
            {
           
                Response_Code = UtilResut.StrDbErrorCode(ResponseCode);
                Message = "SQLEXCEPTION INESPERADO AL REGISTRAR DEPOSITO Y TRANSFERIR SALDO";
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                return false;
            }
            catch (Exception ex)
            {
                Response_Code = UtilResut.StrErrorCode(ResponseCode);
                Message = "ERROR INESPERADO AL REGISTRAR DEPOSITO Y TRANSFERIR SALDO";
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                
                return false;
            }
            
            //finally { mySqlConnection.Close(); }
        }

        
        /// <summary>
        /// Metodo interno que es llamado por RegistroDepositoAcreditaSaldo para registar el deposito y distribuir saldo
        /// 
        /// Puede recibir la conexion y la transaccion que se esta utilizando
        /// </summary>
        /// <param name="age_id"></param>
        /// <param name="amount"></param>
        /// <param name="usr_id"></param>
        /// <param name="reference_number"></param>
        /// <param name="date"></param>
        /// <param name="cuenta"></param>
        /// <param name="ctaId"></param>
        /// <param name="cubId"></param>
        /// <param name="Response_Code"></param>
        /// <param name="Message"></param>
        /// <param name="Comentario"></param>
        /// <param name="fecha_aprobacion"></param>
        /// <param name="numero_sucursal"></param>
        /// <param name="nombre_sucursal"></param>
        /// <param name="_mySqlConnection">Conexion abierta utilizada en al transaccion a la base de datos TRAN_DB</param>
        /// <param name="_tran">Transaccion inicializada</param>
        /// <returns></returns>
        public bool AcreditacionRegistroDeposito(decimal age_id, decimal amount, int usr_id, string reference_number, DateTime date, string cuenta, decimal ctaId, decimal cubId, ref string Response_Code, ref string Message, string Comentario, DateTime fecha_aprobacion, string numero_sucursal, string nombre_sucursal, SqlConnection _mySqlConnection = null, SqlTransaction _tran = null)
        {

            logger.InfoLow(() => TagValue.New().Tag("[OP]").Value("AcreditacionRegistroDeposito"));
            string age_estado = string.Empty;

            //representa el agente padre del agente objetivo
            decimal IdAgenteSolicitante = 0;
            
            string age_comisionadeposito = string.Empty;
  
            string cubNumero = string.Empty;
            decimal ctaSaldo = 0m;
     

            decimal IdBodegaOrigen = 0;
            decimal IdBodegaDestino = 0;

            //autonumerico nuevo deposito
            int traIdReferencia = 0;
            //autonumerico auditoria
            int sec_number_audit = 0;
            //autonumerico number trans
            int sec_number_trans = 0;
            decimal ctaIdCorriente = 0;
            int sec_number_ctacte_mov = 0;
            decimal ult_sol_pro;
            int sec_number;
            decimal ult_env_pro = 0;

            decimal StockIdOrigen = 0m;
            decimal StockIdDestino = 0m;

            string AgenteNombre = string.Empty;
            string sentencia = string.Empty;
            string _men = "";

            int sec_number_aut = 0;
            int sec_number_auditoria  = 0;
            int sec_numbercta_movi = 0;


            //variable auxiliar para la eruqyeta de error
            int ResponseCode = 1;
            //registros modificados en un update
            int resupdate = 0; 


            SqlConnection mySqlConnection = null;
            SqlTransaction tran = null;
            SqlCommand mySqlCommand = new SqlCommand();
            SqlDataReader reader = null;
            //indica si se debe hacer commit a la transaccion 
            //y si se debe cerrar la conexion
            //dependiendo si se pasa o no una conexion y transaccion como parametro
        
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            //Indica si el metodo debe liberar todos los recursos
            bool commitTransaction = false;

            object escalaraux = null;
            try
            {

                if (_mySqlConnection == null)
                {
                    //si no hay conexion se inicializa
                    commitTransaction = true;
                    mySqlConnection = new SqlConnection(strConnString);
                    mySqlConnection.Open();
                    tran = mySqlConnection.BeginTransaction();
                }
                else if (_mySqlConnection != null && _tran != null)
                {
                    //
                    mySqlConnection = _mySqlConnection;
                    tran = _tran;
                }
                else
                {
                    throw new Exception("Parámetros de conexión incorrectos");
                }

     
                mySqlCommand.Transaction = tran;
                mySqlCommand.Connection = mySqlConnection;


                //Proceso de distribucion 
                ResponseCode = 2;
                try
                {
                    //SELECCIONA AGENTE SUPERIOD DEL AGENTE
                    //mySqlCommand.CommandText = "SELECT age_id_sup  FROM [Agente] WITH(NOLOCK)   WHERE age_id = " + age_id;
                    //TODO + SELECT
                    mySqlCommand.CommandText = "SELECT age_id_sup  FROM [Agente] WITH(NOLOCK)   WHERE age_id = @ge_id";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ge_id", age_id);
                    IdAgenteSolicitante = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO SELECCIONANDO AGENTE PADRE";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;

                }


                IdBodegaOrigen = IdAgenteSolicitante;
                IdBodegaDestino = age_id;
                ResponseCode = 3;
                // Inicio Registro de Deposito
                try
                {
            
       
                
                    traIdReferencia = UpdateSecuence("DEPOSITO", mySqlCommand);
                }
                catch (ApiException ex)
                {
                  
                    Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                    Message = ex.Message;
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                   
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO ACTUALIZANDO SECUENCIA DEPOSITO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                
                //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                
              
                ResponseCode = 4;
                try
                {
                    //REALIZA SECUENCIA DE AUDITORIA SIN HINTS
                
                    //CAMBIO
                    sec_number_audit = UpdateSecuence("AUDITORIA", mySqlCommand);
                }
                catch (ApiException ex)
                {
                   
                    Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                    Message = ex.Message;
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                    
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO ACTUALIZANDO SECUENCIA AUDITORIA";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
 
 
                ResponseCode = 5;
                try
                {


                    //CREA KcrTransaccion
                    //sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                    //             "VALUES (" + sec_number_audit.ToString() + "," + usr_id.ToString() + ", null, 'Ingreso de Deposito por el valor : " + usr_id.ToString() + "'" + ", GETDATE() ," + traIdReferencia.ToString() + ", 'FINANCIERO', 'AVISODEPOSITO')";
                    sentencia = @"INSERT INTO KcrTransaccion WITH(ROWLOCK) (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)
                               VALUES 
                                (@traId,@usrId, @usrIdSuperior, @traComentario,GETDATE(),@traIdReferencia,@traDominio,@traSubdominio)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@traId", sec_number_audit);
                    mySqlCommand.Parameters.AddWithValue("@usrId", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@usrIdSuperior", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@traComentario", String.Concat("Ingreso de Deposito por el valor : " , usr_id));
                    mySqlCommand.Parameters.AddWithValue("@traIdReferencia",  traIdReferencia);
                    mySqlCommand.Parameters.AddWithValue("@traDominio", "FINANCIERO");
                    mySqlCommand.Parameters.AddWithValue("@traSubdominio", "AVISODEPOSITO");
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                   
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO CREANDO LA KCR TRANSACCION DEL DEPOSITO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }


                ResponseCode = 6;
                try
                {
                    //CREA KfnDeposito
                    //sentencia = "INSERT INTO KfnDeposito (depId,ageIdOrigen,cubId,depComprobante,depFechaComprobante,depFecha,depMonto,depEstado,depComentario,depComentarioProcesamiento,ccmNumeroTransaccion,usrId,processingUsrId)" +
                    //             "VALUES ( " + traIdReferencia.ToString() + "," + age_id.ToString() + "," + cubId.ToString() + "," + reference_number + ",'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ",'" + fecha_aprobacion.ToString("yyyy-MM-dd HH:mm:ss") + "'," + amount.ToString().Replace(",", ".") + ", 'PE', 'Deposito enviado desde API-Movilway por el Monto: " + amount.ToString() + "'" + ", null, null," + usr_id.ToString() + ", null)";
                    sentencia = @"INSERT INTO KfnDeposito WITH(ROWLOCK) (depId,ageIdOrigen,cubId,depComprobante,depFechaComprobante,depFecha,depMonto,depEstado,depComentario,depComentarioProcesamiento,ccmNumeroTransaccion,usrId,processingUsrId,depNumeroTerminal ,depNombreTerminal )
                        VALUES 
                    ( @traIdReferencia,@age_id,@cubId,@reference_number,@date, @fecha_aprobacion,@amount,@depEstado, @depComentario , @depComentarioProcesamiento ,@ccmNumeroTransaccion ,@usrId, @processingUsrId,@depNumeroTerminal,@depNombreTerminal)";
          
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@traIdReferencia", traIdReferencia);
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    mySqlCommand.Parameters.AddWithValue("@cubId", cubId);
                    mySqlCommand.Parameters.AddWithValue("@reference_number", reference_number);
                    mySqlCommand.Parameters.AddWithValue("@date", date.ToString("yyyyMMdd HH:mm:ss"));
                    mySqlCommand.Parameters.AddWithValue("@fecha_aprobacion", fecha_aprobacion.ToString("yyyyMMdd HH:mm:ss"));
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@depEstado", "PE");
                    mySqlCommand.Parameters.AddWithValue("@depComentario", String.Concat("Deposito enviado desde API-Movilway por el Monto: ", amount.ToString(CultureInfo.InvariantCulture)));
                    mySqlCommand.Parameters.AddWithValue("@depComentarioProcesamiento", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@ccmNumeroTransaccion", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@usrId", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@processingUsrId", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@depNumeroTerminal", numero_sucursal);
                    mySqlCommand.Parameters.AddWithValue("@depNombreTerminal", nombre_sucursal);
              
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
          
                    Response_Code = UtilResut.StrDbErrorCode(ResponseCode);
                    
                    if (ex.Class == 16)
                    {

                        _men  = ex.Message;
                        Message = _men;
                        if (ex.Message.IndexOf("UI_MESSAGE_END") > 0)
                        {
                            var resultado = Message.Split(new string[] { "UI_MESSAGE_START", "UI_MESSAGE_END" }, StringSplitOptions.RemoveEmptyEntries);
                            //MESANJE PARA EL CLIENTE
                            if(resultado.Length > 0)
                            Message = resultado[0];
                        }
                            //"ERROR INESPERADO CREANDO EL DEPOSITO";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", _men, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    }
                    else
                    {
                
                        Message = "ERROR INESPERADO CREANDO EL DEPOSITO SQLEXCEPTION";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    }
                    tran.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                 
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO CREANDO EL DEPOSITO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

        

                
            
                ResponseCode = 8;
                try
                {
                    sec_number_trans = UpdateSecuence("TRANSACCION", mySqlCommand);
                }
                catch (ApiException ex)
                {
                    
                    Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                    Message = ex.Message;
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO ACTUALIZANDO SECUENCIA TRANSACCION";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
 


                //Leemos la cuenta corriente y su valor

                  ResponseCode = 9;
                  try
                  {

                      //SELECCIONA DATOS DEL KfnCuentaCorriente DADO EL AGENTE LOGEADO
                      //mySqlCommand.CommandText = "SELECT   ctaId  FROM KfnCuentaCorriente  WITH (UPDLOCK, ROWLOCK) 	 WHERE ageId = " + age_id.ToString();
                      //ctaIdCorriente = (decimal)mySqlCommand.ExecuteScalar();
                      //mySqlCommand.CommandText = "SELECT ctaSaldo FROM KfnCuentaCorriente  WITH (UPDLOCK, ROWLOCK) 	WHERE ageId = " + age_id.ToString();
                      //ctaSaldo = (decimal)mySqlCommand.ExecuteScalar();
                      //TODO + SELECT
                      mySqlCommand.CommandText = "SELECT   ctaId ,ctaSaldo FROM KfnCuentaCorriente  WITH (UPDLOCK, ROWLOCK)  WHERE ageId = @ageId ";
                      mySqlCommand.Parameters.Clear();
                      mySqlCommand.Parameters.AddWithValue("@ageId", age_id);

                      using( reader = mySqlCommand.ExecuteReader())
                      {
                          if (reader.HasRows && reader.Read())
                          {
                              ctaIdCorriente = (decimal)reader["ctaId"];
                              ctaSaldo = (decimal)reader["ctaSaldo"];
                          }
                          else
                          {
                            
                              Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                              Message = "NO HAY DATOS DE CUENTA CORRIENTE PARA EL AGENTE";
                              logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                              tran.Rollback();
                              return false;
                          }
                      }

                  }
                  catch (Exception ex)
                  {
                    
                      Response_Code = UtilResut.StrErrorCode(ResponseCode);
                      Message = "ERROR INESPERADO SELECCIONADO KFN CUENTA CORRIENTE";
                      logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                      tran.Rollback();
                      return false;
                  }

                  ResponseCode = 10;
                  try
                  {
                      //ACTUALIZA SALDOS EN KfnCuentaCorriente (SUMA)

                      mySqlCommand.CommandText = @"UPDATE KfnCuentaCorriente WITH(ROWLOCK)  SET ctaSaldo = ctaSaldo + @amount 
                        WHERE ctaId = @taId AND ageId = @ageId AND ctaSaldo = @ctaSaldo";
                      mySqlCommand.Parameters.Clear();
                      mySqlCommand.Parameters.AddWithValue("@amount", amount.ToString(CultureInfo.InvariantCulture));
                      mySqlCommand.Parameters.AddWithValue("@taId", ctaId);
                      mySqlCommand.Parameters.AddWithValue("@ageId", age_id);
                      mySqlCommand.Parameters.AddWithValue("@ctaSaldo", ctaSaldo.ToString(CultureInfo.InvariantCulture));

                      resupdate = mySqlCommand.ExecuteNonQuery();
                      if (resupdate == 0)
                      {
                       
                          Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                          Message = "NO SE PUDO ACTUALIZAR SALDO DE CUENTA CORRIENTE";
                          logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                          tran.Rollback();
                          return false;
                      }
                  }
                  catch (Exception ex)
                  {
                
                      Response_Code = UtilResut.StrErrorCode(ResponseCode);
                      Message = "ERROR ACTUALIZANDO SALDO DE CUENTA CORRIENTE";
                      logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                      tran.Rollback();
                      return false;
                  }


                
            
                ResponseCode = 11;
                try
                {
                    //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE 
                    sec_number_ctacte_mov = UpdateSecuence("CTACTEMOVIMIENTO",  mySqlCommand);
                }
                catch (ApiException ex)
                {
                 
                    Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                    Message = ex.Message;
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
	                return false;
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO ACTUALIZANDO SECUENCIA CTACTEMOVIMIENTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                ResponseCode = 12;
                try
                {
                    //     sentencia = "INSERT INTO KfnCuentaCorrienteMovimiento (ccmId,ctaId,ccmFecha,ccmImporte  ,ccmSaldo,ccmDetalle,ccmNumeroTransaccion,ttrId)" + "VALUES ( " + sec_number_ctacte_mov.ToString() + "," + ctaId.ToString() + ", GETDATE()" + "," + amount.ToString().Replace(",", ".") + "," + ctaSaldo.ToString().Replace(",", ".") + ", 'Deposito enviado desde API-Movilway  por el monto: " + amount.ToString().Replace(",", ".") + "'" + "," + sec_number_trans + ", 1000)";


                    //CREA REGISTRO KfnCuentaCorrienteMovimiento
                    sentencia = @"INSERT INTO KfnCuentaCorrienteMovimiento WITH(ROWLOCK)(ccmId,ctaId,ccmFecha,ccmImporte  ,ccmSaldo,ccmDetalle,ccmNumeroTransaccion,ttrId)
                                   VALUES (@sec_number_ctacte_mov ,@ctaId, GETDATE(),@amount,@ctaSaldo, @ccmDetalle ,@sec_number_trans , @ttrId)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@sec_number_ctacte_mov", sec_number_ctacte_mov);
                    mySqlCommand.Parameters.AddWithValue("@ctaId", ctaId);

                    mySqlCommand.Parameters.AddWithValue("@amount", amount.ToString(CultureInfo.InvariantCulture));
                    mySqlCommand.Parameters.AddWithValue("@ctaSaldo", ctaSaldo.ToString(CultureInfo.InvariantCulture));
                    mySqlCommand.Parameters.AddWithValue("@ccmDetalle", string.Concat( "Deposito enviado desde API-Movilway por el Monto: " , amount.ToString(CultureInfo.InvariantCulture)));

                    mySqlCommand.Parameters.AddWithValue("@sec_number_trans", sec_number_trans);
                    mySqlCommand.Parameters.AddWithValue("@ttrId", 1000);
                    mySqlCommand.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                   
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESERADO CREANDO CUENTA CORRIENTE MOVIMIENTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }



                ResponseCode = 13;
                try
                {


                    //ACTUALIZA KfnDeposito A  AUTORIZADO Y COMENTARIO

                    //sentencia = "UPDATE KfnDeposito SET  depEstado = 'AU', depComentarioProcesamiento = 'Deposito enviado desde API-Movilway por el Monto: " + @amount.ToString().Replace(",", ".") + "'" +
                    //            ",ccmNumeroTransaccion =" + sec_number_trans + ", processingUsrId =" + usr_id + " WHERE  depId =" + traIdReferencia;
                    sentencia = @"  
                                    UPDATE KfnDeposito  WITH(ROWLOCK)
                                    SET  depEstado = @depEstado,
                                    depComentarioProcesamiento = @depComentarioProcesamiento,
                                    ccmNumeroTransaccion = @sec_number_trans,
                                    processingUsrId = @usr_id ,
                                    depComentario= @Comentario 
                                    WHERE  depId = @traIdReferencia";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@depEstado", "AU");
                    mySqlCommand.Parameters.AddWithValue("@depComentarioProcesamiento", String.Concat("Deposito enviado desde API-Movilway por el Monto: " , amount.ToString(CultureInfo.InvariantCulture)));
                    mySqlCommand.Parameters.AddWithValue("@sec_number_trans", sec_number_trans);
                    mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@Comentario", Comentario);
                    mySqlCommand.Parameters.AddWithValue("@traIdReferencia", traIdReferencia);
               
                    resupdate =  mySqlCommand.ExecuteNonQuery();
                    if (resupdate == 0)
                    {
                   
                        Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                        Message = "NO SE PUDO ACTUALIZAR ESTADO DE DEPOSITO";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                        tran.Rollback();
                        return false;
                    }

                }
                catch (Exception ex)
                {
               
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO ACTUALIZANDO DEPOSITO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                

                  ResponseCode = 14;
                  try
                  {
                      sec_number_audit = UpdateSecuence("AUDITORIA", mySqlCommand);// (int)
                  }
                  catch (ApiException ex)
                  {
             
                      Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                      Message = ex.Message;
                      logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                      tran.Rollback();
                      return false;
                  }
                  catch (Exception ex)
                  {
                
                      Response_Code = UtilResut.StrErrorCode(ResponseCode);
                      Message = "ERROR INESPERADO ACTUALIZANDO AUDITORIA";
                      logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                      tran.Rollback();
                      return false;
                  }

                  ResponseCode = 15;
                  try
                  {
                      //CREA KcrTransaccion
                      //sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                      //      "VALUES ( " + sec_number_audit + "," + usr_id + ", null, 'Deposito enviado desde API-Movilway por el monto " + amount + "'" + "," +
                      //      "GETDATE(), " + traIdReferencia.ToString() + ", 'FINANCIERO', 'AUTORIZACION')";
                      sentencia = @"INSERT INTO KcrTransaccion WITH(ROWLOCK)(traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)
                                                    VALUES  (@traId,@usrId,@usrIdSuperior,@traComentario,GETDATE(),@traIdReferencia,@traDominio,@traSubdominio)";

                      mySqlCommand.CommandText = sentencia;
                      mySqlCommand.Parameters.Clear();
                      mySqlCommand.Parameters.AddWithValue("@traId", sec_number_audit );
                      mySqlCommand.Parameters.AddWithValue("@usrId", usr_id);
                      mySqlCommand.Parameters.AddWithValue("@usrIdSuperior", DBNull.Value);
                      mySqlCommand.Parameters.AddWithValue("@traComentario",String.Concat( "Deposito enviado desde API-Movilway por el monto " , amount.ToString(CultureInfo.InvariantCulture)));
                      mySqlCommand.Parameters.AddWithValue("@traIdReferencia",traIdReferencia);
                      mySqlCommand.Parameters.AddWithValue("@traDominio", "FINANCIERO");
                      mySqlCommand.Parameters.AddWithValue("@traSubdominio", " AUTORIZACION");

                      mySqlCommand.ExecuteNonQuery();
                  }
                  catch (Exception ex)
                  {
                   
                      Response_Code = UtilResut.StrErrorCode(ResponseCode);
                      Message = "ERROR INESPERADO CREANDO KRC TRANSACCION DEPOSITO";
                      logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                      tran.Rollback();
                      return false;
                  }

                // Fin Registro de Deposito



                //Inicio Proceso de Acreditacion 


                  ResponseCode = 16;
                  try
                  {
                 

                    ult_sol_pro = UpdateSecuence("SOLICITUDPRODUCTO", mySqlCommand);// (int)mySqlCommand.ExecuteScalar();
                    ult_sol_pro = (int)mySqlCommand.ExecuteScalar();
                    sec_number = (int)ult_sol_pro;
                  }
                  catch (ApiException ex)
                  {
                     
                      Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                      Message = ex.Message;
                      logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                      tran.Rollback();
                      return false;
                  }
                  catch (Exception ex)
                  {
                   
                      Response_Code = UtilResut.StrErrorCode(ResponseCode);
                      Message = "ERROR INESPERADO  ACTUALIZANDO SECUENCIA SOLICITUDPRODUCTO";
                      logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                      tran.Rollback();
                      return false;
                  }

                /* inserto una solicitud y la coloco en estado pendiente */


                //CREA SOLICITUD PRODUCTO
                  ResponseCode = 17;
                  try
                  {
                      //sentencia = "INSERT INTO [KlgSolicitudProducto] WITH(ROWLOCK) (sprId,usrId,ageIdSolicitante,ageIdDestinatario,prvIdDestinatario,sprEstado,sprFecha,sprFechaAprobacion,sprImporteSolicitud,sltId,ageIdBodegaOrigen,ageIdBodegaDestino) " +
                      //            "VALUES (" + ult_sol_pro.ToString() + "," + usr_id.ToString() + "," + IdAgenteSolicitante.ToString() + "," + age_id.ToString() + ", NULL,'AU','" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ",'" + fecha_aprobacion.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + amount.ToString().Replace(",", ".") + ",202," + IdBodegaOrigen.ToString() + "," + IdBodegaDestino.ToString() + ")";
                      sentencia = @"
                                    INSERT INTO [KlgSolicitudProducto] WITH(ROWLOCK)        (sprId,usrId,ageIdSolicitante,ageIdDestinatario,prvIdDestinatario,sprEstado,sprFecha,sprFechaAprobacion,sprImporteSolicitud,sltId,ageIdBodegaOrigen,ageIdBodegaDestino) 
                                VALUES (@sprId,@usrId,@ageIdSolicitante,@ageIdDestinatario,@prvIdDestinatario,@sprEstado,@sprFecha,@sprFechaAprobacion,@sprImporteSolicitud,@sltId,@ageIdBodegaOrigen,@ageIdBodegaDestino)";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@sprId",ult_sol_pro);
                        mySqlCommand.Parameters.AddWithValue("@usrId",usr_id);
                        mySqlCommand.Parameters.AddWithValue("@ageIdSolicitante",IdAgenteSolicitante);
                        mySqlCommand.Parameters.AddWithValue("@ageIdDestinatario",age_id);
                        mySqlCommand.Parameters.AddWithValue("@prvIdDestinatario",DBNull.Value);
                        mySqlCommand.Parameters.AddWithValue("@sprEstado","AU");
                        mySqlCommand.Parameters.AddWithValue("@sprFecha",date);
                        mySqlCommand.Parameters.AddWithValue("@sprFechaAprobacion",fecha_aprobacion);
                        mySqlCommand.Parameters.AddWithValue("@sprImporteSolicitud",amount);
                        mySqlCommand.Parameters.AddWithValue("@sltId","202");
                        mySqlCommand.Parameters.AddWithValue("@ageIdBodegaOrigen",IdBodegaOrigen);
                        mySqlCommand.Parameters.AddWithValue("@ageIdBodegaDestino",IdBodegaDestino);

                          mySqlCommand.CommandText = sentencia;
                          mySqlCommand.ExecuteNonQuery();
                  }
                  catch (Exception ex)
                  {
                      Response_Code = UtilResut.StrErrorCode(ResponseCode);
                      Message = "ERROR INESPERADO CREANDO KLG SOLICITUD PRODUCTO ";
                      logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                      tran.Rollback();
                      return false;
                  }

                  ResponseCode = 18;
           
                  try
                  {
                      //TODO COMENTARIO ANTERIOR
                      //--
                      //Se actualiza el campo usrIdInitiator en caso de existir
                      //--
                      //ACTUALIZA SOLICITUD PRODUCTO USUARIOINITIATOR
                        sentencia = @"
                        UPDATE [KlgSolicitudProducto] WITH(ROWLOCK) SET usrIdInitiator = @usr_id
                        WHERE sprId = @ult_sol_pro";
                        mySqlCommand.CommandText = sentencia;
                        mySqlCommand.Parameters.AddWithValue("@usr_id",usr_id);
                        mySqlCommand.Parameters.AddWithValue("@ult_sol_pro",ult_sol_pro);
                     
                        resupdate =   mySqlCommand.ExecuteNonQuery();
                        if (resupdate == 0)
                        {

                            Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                            Message = "NO SE ACTUALIZARON LOS CAMPOS DE NUMERO TERMINAL Y NOMBRE TERMINAL";
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            //dada la notacion anterior no se termina el proceso
                            //return false;
                        }
                  
                  }
                  catch (Exception ex)
                  {
                  
                      //registrar si no exsiste el campo
                      // problema de cambio de versiones de kinacu
                      Response_Code = UtilResut.StrErrorCode(ResponseCode);
                      Message = "ERROR INESPERADO  ACTUALIZANDO USRIDINITIATOR KLG SOLICITUD PRODUCTO";
                      logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                      //tran.Rollback();
                  }


                   ResponseCode = 19;
                //Se actualiza el campo usrIdInitiator en caso de existir
                   try
                   {
                       //CREA ITEMDEL SOLICITUD DE PRODUCTO CERO ESTADO 'PE'
                       //sentencia = "INSERT INTO [KlgSolicitudProductoItem] WITH(ROWLOCK) (sprId,prdId,spiCantidadSolicitada,spiCantidadAutorizada,spiPrecioUnitario,spiEstado) " +
                       //            "VALUES (" + ult_sol_pro + ",0," + amount + "," + amount + ", 1.0000,'PE')";
                       sentencia = @"
                            INSERT INTO [KlgSolicitudProductoItem] WITH(ROWLOCK)
                            (sprId,prdId,spiCantidadSolicitada,spiCantidadAutorizada,spiPrecioUnitario,spiEstado)
                            VALUES
                            (@sprId,@prdId,@spiCantidadSolicitada,@spiCantidadAutorizada,@spiPrecioUnitario,@spiEstado)";
                       mySqlCommand.Parameters.Clear();
                       mySqlCommand.Parameters.AddWithValue("@sprId", ult_sol_pro );
                       mySqlCommand.Parameters.AddWithValue("@prdId", cons.MULTI_PRODUCTO);
                       mySqlCommand.Parameters.AddWithValue("@spiCantidadSolicitada",  amount );
                       mySqlCommand.Parameters.AddWithValue("@spiCantidadAutorizada", amount );
                       mySqlCommand.Parameters.AddWithValue("@spiPrecioUnitario", 1m);//1.0000
                       mySqlCommand.Parameters.AddWithValue("@spiEstado", "PE");


                       mySqlCommand.CommandText = sentencia;
                       mySqlCommand.ExecuteNonQuery();
                   }
                   catch (Exception ex)
                   {
                    
                       Response_Code = UtilResut.StrErrorCode(ResponseCode);
                       Message = "ERROR INESPERADO  CREANDO KLG SOLICTUD PRODUCTO ITEM";
                       logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                       tran.Rollback();
                       return false;
                   }


                   ResponseCode = 20;
                   try
                   {
                       //  sentencia = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK) SET spiEstado = 'AU', spiCantidadAutorizada = @amount WHERE sprId = @ult_sol_pro  AND prdId = 0 AND spiEstado = 'PE'";
                       sentencia = @"
                                    UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK) 
                                    SET spiEstado = @spiEstado,
                                    spiCantidadAutorizada = @amount 
                                    WHERE sprId = @ult_sol_pro  AND prdId = @prdId AND spiEstado = @oldSpiEstado";
                    
                       mySqlCommand.CommandText = sentencia;
                       mySqlCommand.Parameters.Clear();
                       mySqlCommand.Parameters.AddWithValue("@spiEstado", "AU");
                       mySqlCommand.Parameters.AddWithValue("@amount", amount);
                       mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                       mySqlCommand.Parameters.AddWithValue("@prdId", cons.MULTI_PRODUCTO);
                       mySqlCommand.Parameters.AddWithValue("@oldSpiEstado", "PE");
                       resupdate = mySqlCommand.ExecuteNonQuery();
                       if (resupdate == 0)
                       {
                     
                           Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                           Message = "NO SE PUDO ACTUALIZAR ESTADO Y CANTIDAD AUTORIZADA KLG SOLICTUD PRODUCTO ITEM ";
                           logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                           //dada la notacion anterior no se termina el proceso
                           tran.Rollback();
                           return false;
                       }
                   }
                   catch (Exception ex)
                   {
                     
                       Response_Code = UtilResut.StrErrorCode(ResponseCode);
                       Message = "ERROR NO SE PUDO ACTUALIZAR KLG SOLICTUD PRODUCTO ITEM";
                       logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                       tran.Rollback();
                       return false;
                   }



                   ResponseCode = 21;
                   try
                   {
                     
                       ult_env_pro = UpdateSecuence("ENVIOPRODUCTO",  mySqlCommand);// (int)mySqlCommand.ExecuteScalar();
                
                   }
                   catch (ApiException ex)
                   {
                 
                       Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                       Message = ex.Message;
                       logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                       tran.Rollback();
                       return false;
                   }
                   catch (Exception ex)
                   {
                   
                       Response_Code = UtilResut.StrErrorCode(ResponseCode);
                       Message = "ERROR ACTUALIZANDO SECUENCIA ENVIOPRODUCTO";
                       logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                       tran.Rollback();
                       return false;
                   }

                   ResponseCode = 22;
                   try
                   {
                       //CREA KlgEnvio ESTAPO 'DE'
                       //sentencia = "INSERT INTO [KlgEnvio] WITH(ROWLOCK) (envId,envEstado,envFechaEnvio,envFechaRecepcion,envNumeroRemito,envNumeroFactura,envObservaciones,ageIdBodegaOrigen,ageIdBodegaDestino) " +
                       //            "VALUES (" + ult_env_pro + ",'DE','" + date.ToString("yyyy-MM-dd HH:mm:ss") + "',NULL,'','" + traIdReferencia.ToString() + "','Envio de productos Virtuales en linea'," + IdBodegaOrigen + "," + IdBodegaDestino + ")";
                        sentencia = @" INSERT INTO [KlgEnvio] WITH (ROWLOCK)            (envId,envEstado,envFechaEnvio,envFechaRecepcion,envNumeroRemito,envNumeroFactura,envObservaciones,ageIdBodegaOrigen,ageIdBodegaDestino)
                            VALUES   (@envId,@envEstado,@envFechaEnvio,@envFechaRecepcion,@envNumeroRemito,@envNumeroFactura,@envObservaciones,@ageIdBodegaOrigen,@ageIdBodegaDestino)";
                        mySqlCommand.CommandText = sentencia;
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@envId",ult_env_pro);
                        mySqlCommand.Parameters.AddWithValue("@envEstado","DE");
                        mySqlCommand.Parameters.AddWithValue("@envFechaEnvio",date);
                        mySqlCommand.Parameters.AddWithValue("@envFechaRecepcion", DBNull.Value);
                        mySqlCommand.Parameters.AddWithValue("@envNumeroRemito","");
                        mySqlCommand.Parameters.AddWithValue("@envNumeroFactura",traIdReferencia);
                        mySqlCommand.Parameters.AddWithValue("@envObservaciones","Envio de productos Virtuales en linea");
                        mySqlCommand.Parameters.AddWithValue("@ageIdBodegaOrigen",IdBodegaOrigen);
                        mySqlCommand.Parameters.AddWithValue("@ageIdBodegaDestino",IdBodegaDestino );
                       
                        mySqlCommand.ExecuteNonQuery();
                   }
                   catch (Exception ex)
                   {
                     
                       Response_Code = UtilResut.StrErrorCode(ResponseCode);
                       Message = "ERROR AL CREAR KLG ENVIO";
                       logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                       tran.Rollback();
                       return false;
                   }
             


                   ResponseCode = 23;
                   try
                   {

                       //CREA KlgSolicitudProductoEnvio
                       //sentencia = "INSERT INTO [KlgSolicitudProductoEnvio] WITH(ROWLOCK) (sprId,envId)  " +
                       // "VALUES (" + ult_sol_pro + "," + ult_env_pro + ")";
                       sentencia = @"INSERT INTO [KlgSolicitudProductoEnvio] WITH(ROWLOCK) (sprId,envId)
                                    VALUES(@sprId,@envId)";
                       mySqlCommand.CommandText = sentencia;
                       mySqlCommand.Parameters.Clear();
                       mySqlCommand.Parameters.AddWithValue("@sprId", ult_sol_pro);
                       mySqlCommand.Parameters.AddWithValue("@envId",ult_env_pro );

                       mySqlCommand.ExecuteNonQuery();
                   }
                   catch (Exception ex)
                   {
                      
                       Response_Code = UtilResut.StrErrorCode(ResponseCode);
                       Message = "ERROR AL CREAR KLG SOLICITUD PRODUCTO ENVIO";
                       logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                       tran.Rollback();
                       return false;
                   }
            
                
                   ResponseCode = 24;
                   try
                   {

                        //CREA KlgEnvioItem
                        //sentencia = "INSERT INTO [KlgEnvioItem] WITH(ROWLOCK) (envId,prdId,eitCantidad,eitEstado)" +
                        //            "VALUES (" + ult_env_pro + "," + 0 + "," + amount.ToString().Replace(",", ".") + ",'DE')";
                           sentencia = @"INSERT INTO [KlgEnvioItem] WITH(ROWLOCK) (envId,prdId,eitCantidad,eitEstado)
                                        VALUES (@envId,@prdId,@eitCantidad,@eitEstado)";
                        mySqlCommand.CommandText = sentencia;
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@envId", ult_env_pro);
                        mySqlCommand.Parameters.AddWithValue("@prdId", cons.MULTI_PRODUCTO);
                        mySqlCommand.Parameters.AddWithValue("@eitCantidad", amount);
                        mySqlCommand.Parameters.AddWithValue("@eitEstado", "DE");

                        mySqlCommand.ExecuteNonQuery();
                   }
                   catch (Exception ex)
                   {
                       
                       Response_Code = UtilResut.StrErrorCode(ResponseCode);
                       Message = "ERROR AL CREAR KLG SOLICITUD PRODUCTO ENVIO";
                       logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                       tran.Rollback();
                       return false;
                   }


                   ResponseCode = 25;
                   try
                   {
                       //ACTUALIZA LA SOLCITUD KlgSolicitudProductoIte ESTADO 'AU'
                       //mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = 'DE' WHERE sprId =" + ult_sol_pro + " AND prdId = 0 AND spiEstado = 'AU'";
                       mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = @spiEstado WHERE sprId = @ult_sol_pro  AND prdId = @prdId AND spiEstado = @oldSpiEstado";
                       mySqlCommand.Parameters.Clear();
                       mySqlCommand.Parameters.AddWithValue("@spiEstado", "DE");
                       mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                       mySqlCommand.Parameters.AddWithValue("@prdId", cons.MULTI_PRODUCTO);
                       mySqlCommand.Parameters.AddWithValue("@oldSpiEstado", "AU");
                       resupdate = mySqlCommand.ExecuteNonQuery();
                       if (resupdate == 0)
                       {
                        
                           Response_Code = UtilResut.StrErrorCode(ResponseCode);
                           Message = "NO SE PUDO ACTUALIZAR KLG SOLICITUD PRODUCTO ITEM";
                           logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                           tran.Rollback();
                           return false;
                       }
                   }
                   catch (Exception ex)
                   {
                  
                       Response_Code = UtilResut.StrErrorCode(ResponseCode);
                       Message = "ERROR AL CREAR KLG SOLICITUD PRODUCTO ITEM";
                       logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                       tran.Rollback();
                       return false;
                   }
                

                    ResponseCode = 26;
                    try
                    {

                        //mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId =" + IdAgenteSolicitante + " AND prdId IN (0)";
                        mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId = @IdAgenteSolicitante  AND prdId  = @prdId";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@IdAgenteSolicitante", IdAgenteSolicitante);
                        mySqlCommand.Parameters.AddWithValue("@prdId ", cons.MULTI_PRODUCTO);
                        StockIdOrigen = (decimal)mySqlCommand.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                    
                        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                        Message = "ERROR AL SELECCIONAR ID KLG STOCK AGENTE PRODUCTO";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }


                    ResponseCode = 27;
                    try
                    {
                        //REALIZA ACTUALIZACION DE SALDO DAO EL AGENTESOLICITANTE  SI Y SOLO SI TIENE EL SALDO SUFICIENTE
                        //TODO NO HAY VERIFICACION DEL UPDATE
                        //mySqlCommand.CommandText = " UPDATE [KlgStockAgenteProducto] WITH(ROWLOCK) SET  stkCantidad = stkCantidad + - " + amount + " WHERE  stkId = " + StockIdOrigen + " AND ageId = " + IdAgenteSolicitante + "and stkCantidad + - " + amount + ">= 0";
                        mySqlCommand.CommandText = @"UPDATE [KlgStockAgenteProducto] WITH(ROWLOCK) SET  stkCantidad = stkCantidad + - @amount  WHERE  stkId = @StockIdOrigen  AND ageId = @IdAgenteSolicitante and stkCantidad + - @amount >= 0";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@amount", amount);
                        mySqlCommand.Parameters.AddWithValue("@StockIdOrigen", StockIdOrigen);
                        mySqlCommand.Parameters.AddWithValue("@IdAgenteSolicitante", IdAgenteSolicitante);
                        resupdate = mySqlCommand.ExecuteNonQuery();
                        if (resupdate == 0)
                        {
                        
                            Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                            Message = "NO SE PUDO ACTUALIZAR STOCK AGENTE PRODUCTO SUPERIOR, VERIFIQUE SALDO";
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            tran.Rollback();
                            return false;
                        }
 
                    }
                    catch (Exception ex)
                    {
                      
                        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                        Message = "ERROR AL ACTUALIZAR KLG STOCK AGENTE PRODUCTO SUPERIOR";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }
             

                
                    ResponseCode = 28;
                    try
                    {
                        // ACTUALIZA KlgEnvio CON ESTADO 'DE'
                        //mySqlCommand.CommandText = @" UPDATE [KlgEnvio] WITH(ROWLOCK) SET envEstado = 'RE', envFechaRecepcion = GETDATE() WHERE envId = " + ult_env_pro + " AND envEstado = 'DE'";
                        mySqlCommand.CommandText = @" UPDATE [KlgEnvio] WITH(ROWLOCK) SET envEstado = @envEstado, envFechaRecepcion = GETDATE() WHERE envId =  @ult_env_pro AND envEstado = @oldEnvEstado";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@envEstado", "RE");
                        mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                        mySqlCommand.Parameters.AddWithValue("@oldEnvEstado", "DE");
                        resupdate = mySqlCommand.ExecuteNonQuery();
                        if (resupdate == 0)
                        {
                         
                            Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                            Message = "NO SE PUDO ACTUALIZAR ESTADO KLG ENVIO";
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            tran.Rollback();
                            return false;
                        }
 
                    }
                    catch (Exception ex)
                    {
                   
                        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                        Message = "ERROR AL ACTUALIZAR KLG ENVIO";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }

                
                    ResponseCode = 29;
                    try
                    {
                        //REALIZA ENVIO ITEM LO PASA A ESTADO 'RE'
                        mySqlCommand.CommandText = "UPDATE [KlgEnvioItem] WITH(ROWLOCK) SET eitEstado = @eitEstado   WHERE envId = @ult_env_pro AND prdId = @prdId AND eitEstado = @oldEitEstado ";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@eitEstado", "RE");
                        mySqlCommand.Parameters.AddWithValue("@ult_env_pro",ult_env_pro);
                        mySqlCommand.Parameters.AddWithValue("@prdId", cons.MULTI_PRODUCTO);
                        mySqlCommand.Parameters.AddWithValue("@oldEitEstado", "DE");
                        resupdate = mySqlCommand.ExecuteNonQuery();
                        if (resupdate == 0)
                        {
                        
                            Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                            Message = "NO SE PUDO ACTUALIZAR ESTADO KLG ENVIO ITEM";
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            tran.Rollback();
                            return false;
                        }
 
                    }
                    catch (Exception ex)
                    {
                      
                        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                        Message = "ERROR AL ACTUALIZAR KLG ENVIO ITEM";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }


                    ResponseCode = 30;
                    try
                    {

                          //CAMBIA ESTADO A KlgSolicitudProductoItem RE
                          //mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = 'RE'  WHERE sprId = " + ult_sol_pro + "  AND prdId = 0";
                          mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = @spiEstado  WHERE sprId = @ult_sol_pro  AND prdId = @prdId";
                          mySqlCommand.Parameters.Clear();
                          mySqlCommand.Parameters.AddWithValue("@spiEstado", "RE");
                          mySqlCommand.Parameters.AddWithValue("@ult_sol_pro",ult_sol_pro);
                          mySqlCommand.Parameters.AddWithValue("@prdId", cons.MULTI_PRODUCTO);
                          resupdate = mySqlCommand.ExecuteNonQuery();
                          if (resupdate == 0)
                          {
                            
                              Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                              Message = "NO SE ACTUALIZAR EL ESTADO DE KLG SOLICITUD PRODUCTO ITEM";
                              logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                              tran.Rollback();
                              return false;
                          }
                    }
                    catch (Exception ex)
                    {
                     
                          Response_Code = UtilResut.StrErrorCode(ResponseCode);
                          Message = "ERROR AL ACTUALIZAR KLG SOLICITUD PRODUCTO ITEM";
                          logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                          tran.Rollback();
                          return false;
                    }

                    ResponseCode = 31;
                    try
                    {
                        //SELECCIONA EL STOCK DEL AGENTE QUE REALIZA EL DEPOSITO
                        //mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId =" + age_id + " AND prdId IN (0)";
                        mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId = @age_id AND prdId = @prdId";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@age_id",age_id);
                        mySqlCommand.Parameters.AddWithValue("@prdId", cons.MULTI_PRODUCTO);
                        StockIdDestino = (decimal)mySqlCommand.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                       
                        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                        Message = "ERROR AL SELECCIONAR ID KLG STOCK AGENTE PRODUCTO";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }

                     ResponseCode = 32;
                     try
                     {
                         //ACTUALIZA EL STOCK DEL AGENTE QUE REALIZA EL DEPOSITO
                         //TODO NO HAY VALIDACION DE OPERACION CORRECTA
                         //mySqlCommand.CommandText = "UPDATE KlgStockAgenteProducto SET  stkCantidad = stkCantidad + " + amount + "  WHERE  stkId =" + StockIdDestino + " AND ageId =" + age_id + " and stkCantidad + " + amount + ">= 0 ";
                         mySqlCommand.CommandText = "UPDATE KlgStockAgenteProducto WITH(ROWLOCK) SET  stkCantidad = stkCantidad + @amount   WHERE  stkId = @StockIdDestino AND ageId = @age_id and stkCantidad + @amount >= 0 ";
                          mySqlCommand.Parameters.Clear();
                          mySqlCommand.Parameters.AddWithValue("@amount", amount);
                          mySqlCommand.Parameters.AddWithValue("@StockIdDestino", StockIdDestino);
                          mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    
                       
                          resupdate = mySqlCommand.ExecuteNonQuery();
                          if (resupdate == 0)
                          {
                          
                              Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                              Message = "NO SE PUDO ACTUALIZAR KLG STOCK AGENTE PRODUCTO DEL AGENTE, VERIFIQUE EL SALDO (SALDO NEGATIVO)";
                              logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                              tran.Rollback();
                              return false;
                          }
                     }
                     catch (Exception ex)
                     {
                      
                         Response_Code = UtilResut.StrErrorCode(ResponseCode);
                         Message = "ERROR INESPERADO ACTUALIZAR KLG STOCK AGENTE PRODUCTO DEL AGENTE";
                         logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                         tran.Rollback();
                         return false;
                     }

                    ResponseCode = 33;
                    try
                    {
                          //CAMBIA ESTADO A KlgSolicitudProductoItem RE
                          //mySqlCommand.CommandText = " UPDATE KlgSolicitudProducto WITH(ROWLOCK) SET sprEstado = 'CE' WHERE sprId = " + ult_sol_pro + "  AND sprEstado = 'AU' AND ( ageIdSolicitante =" + age_id.ToString() + " OR ageIdDestinatario =" + age_id.ToString() + ")";
                          //TODO SE ACTUALIZA EL ESTADO DEL KLG SOLICITUD PRODUCTO, TENENDO ENCUENT LA CREACION INICIAL
                          mySqlCommand.CommandText = " UPDATE KlgSolicitudProducto WITH(ROWLOCK) SET sprEstado =  @sprEstado WHERE sprId = @ult_sol_pro AND sprEstado =  @oldSprEstado AND  ageIdDestinatario =  @age_id  ";
                          mySqlCommand.Parameters.Clear();
                          mySqlCommand.Parameters.AddWithValue("@sprEstado", "CE");
                          mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                          mySqlCommand.Parameters.AddWithValue("@oldSprEstado", "AU");
                          mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                          resupdate = mySqlCommand.ExecuteNonQuery();
                          if (resupdate == 0)
                          {
                            
                              Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                              Message = "NO SE PUEDE ACTUALIZAR ESTADO KLG SOLICITUD PRODUCTO";
                              logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                              tran.Rollback();
                              return false;
                          }
                    }
                    catch (Exception ex)
                    {
                    
                          Response_Code = UtilResut.StrErrorCode(ResponseCode);
                          Message = "ERROR AL ACTUALIZAR SOLICITUD PRODUCTO";
                          logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                          tran.Rollback();
                          return false;
                    }



                //Fin de proceso de Acreditacion  


                // {Inicio el proceso para registar en cta cte
                
                    ResponseCode = 34;
                    try
                    {
            

                        sec_number_auditoria = UpdateSecuence("AUDITORIA", mySqlCommand);
                    }
                    catch (ApiException ex)
                    {
                     
                        Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                        Message = ex.Message;
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }
                    catch (Exception ex)
                    {
                       
                        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                        Message = "ERROR INESPERADO ACTUALIZANDO SECUENCIA AUDITORIA";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }

                    ResponseCode = 35;
                    try
                    {


                        //CREA KcrTransaccion CON TESPECTO A LA AUDITORIA
                        //sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                        //            "VALUES ( " + sec_number_auditoria + ", " + usr_id + ", null, 'Despacho de productos nro." + ult_env_pro + "'" + " ," + "'" + fecha_aprobacion.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + ult_env_pro + ",'DISTRIBUCION', 'ENVIOPRODUCTO')";

                        sentencia = @"INSERT INTO KcrTransaccion WITH(ROWLOCK)(traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)
                                         VALUES(@traId,@usrId,@usrIdSuperior,@traComentario,@traFecha,@traIdReferencia,@traDominio,@traSubdominio)";
                        mySqlCommand.CommandText = sentencia;
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@traId",  sec_number_auditoria );
                        mySqlCommand.Parameters.AddWithValue("@usrId",usr_id );
                        mySqlCommand.Parameters.AddWithValue("@usrIdSuperior", DBNull.Value);
                        mySqlCommand.Parameters.AddWithValue("@traComentario", String.Concat("Despacho de productos nro. ", ult_env_pro) );
                        mySqlCommand.Parameters.AddWithValue("@traFecha",  fecha_aprobacion);
                        mySqlCommand.Parameters.AddWithValue("@traIdReferencia", ult_env_pro );
                        mySqlCommand.Parameters.AddWithValue("@traDominio", "DISTRIBUCION");
                        mySqlCommand.Parameters.AddWithValue("@traSubdominio", "ENVIOPRODUCTO");

                        mySqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                     
                        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                        Message = "ERROR INESPERADO CREANDO KCR TRANSACCION ENVIO PRODUCTO";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                            
                    }

                    ResponseCode = 36;
                    try
                    {
                        // --Volvemos a leer el saldo de la cta cte. 
                        //LEE EL SALDO NUEVO
                        //mySqlCommand.CommandText = "SELECT  ctaId   FROM KfnCuentaCorriente  WITH (NOLOCK) WHERE ageId =" + age_id;
                        //mySqlCommand.CommandText = "SELECT  ctaSaldo   FROM KfnCuentaCorriente  WITH (NOLOCK) WHERE ageId =" + age_id;
                        //ctaSaldo = (decimal)mySqlCommand.ExecuteScalar();


                        mySqlCommand.CommandText = "SELECT ctaId,  ctaSaldo   FROM KfnCuentaCorriente  WITH (NOLOCK) WHERE ageId = @age_id";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                        using (reader = mySqlCommand.ExecuteReader())
                        {
                            if (reader.HasRows && reader.Read())
                            {
                                ctaIdCorriente = (decimal)reader["ctaId"];
                                ctaSaldo = (decimal)reader["ctaSaldo"];
                            }
                            else
                            {
                         
                                Response_Code = UtilResut.StrErrorCode(ResponseCode);
                                Message = "NO HAY DATOS DE CUENTA CORRIENTE";
                                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                                tran.Rollback();
                                return false;
                            }
                        }
                     

                    }
                    catch (Exception ex)
                    {
                      
                        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                        Message = "ERROR SELECCIONADO KFN CUENTA CORRIENTE";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }



                    ResponseCode = 37;
                    try
                    {
                     

                        sec_number_aut = UpdateSecuence("TRANSACCION", mySqlCommand);

                    }
                    catch (ApiException ex)
                    {
                     
                        Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                        Message = ex.Message;
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }
                    catch (Exception ex)
                    {
                   
                        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                        Message = "ERROR INESPERADO ACTUALIZANDO SECUENCIA TRANSACCION";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }

                    ResponseCode = 38;
                    try
                    {

                        //NO SE TENIA ENCUENTA EL RANGO DE QUE ESTA COMENTARIADO EN EL QUERY ORIGINAL
                        //mySqlCommand.CommandText = "UPDATE KfnCuentaCorriente WITH(ROWLOCK) SET ctaSaldo = ctaSaldo + - " + amount + " WHERE ctaId =" + ctaId + " AND ageId = " + age_id + " AND ctaSaldo =" + ctaSaldo; //+ " AND ( (ctaSaldo + - " + amount + " >= 0) OR (ABS(ctaSaldo + - " + amount + ") <= 600) )";
                        mySqlCommand.CommandText = @"UPDATE KfnCuentaCorriente WITH(ROWLOCK) SET ctaSaldo = ctaSaldo + - @amount WHERE ctaId =  @ctaId  AND ageId = @age_id AND ctaSaldo =  @ctaSaldo" ; 
                      
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@amount", amount);
                        mySqlCommand.Parameters.AddWithValue("@ctaId", ctaId);
                        mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                        mySqlCommand.Parameters.AddWithValue("@ctaSaldo", ctaSaldo);
                        resupdate = mySqlCommand.ExecuteNonQuery();
                        if (resupdate == 0)
                        {
                           
                            Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                            Message = "NO SE PUDO ACTUALIZAR EL SALDO CORRIENTE (RESTA)";
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            tran.Rollback();
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                     
                        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                        Message = "ERROR INESPERADO ACTUALIZANDO CUENTA CORRIENTE SALDO (RESTA)";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }

                    ResponseCode = 39;
                    try
                    {
                        sec_numbercta_movi = UpdateSecuence("CTACTEMOVIMIENTO", mySqlCommand);//(int)mySqlCommand.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                      
                        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                        Message = "ERROR INESPERADO ACTUALIZANDO CUENTA CORRIENTE SALDO (RESTA)";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }


                    ResponseCode = 40;
                    try
                    {
                        //SE CREA
                        //sentencia = "INSERT INTO KfnCuentaCorrienteMovimiento (ccmId,ctaId,ccmFecha,ccmImporte  ,ccmSaldo,ccmDetalle,ccmNumeroTransaccion,ttrId)" +
                        //            "VALUES ( " + sec_numbercta_movi + "," + ctaId + "," + "'" + fecha_aprobacion.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + -amount + "," + ctaSaldo.ToString() + ",'Asignacion de productos No.: " + ult_sol_pro.ToString() + "'" + "," + sec_number_aut.ToString() + ", 2000)";

                        sentencia = @"INSERT INTO KfnCuentaCorrienteMovimiento WITH(ROWLOCK)(ccmId,ctaId,ccmFecha,ccmImporte  ,ccmSaldo,ccmDetalle,ccmNumeroTransaccion,ttrId)
                        VALUES (@ccmId,@ctaId,@ccmFecha, -@ccmImporte  ,@ccmSaldo,@ccmDetalle,@ccmNumeroTransaccion,@ttrId)";
                        mySqlCommand.CommandText = sentencia;
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@ccmId", sec_numbercta_movi );
                        mySqlCommand.Parameters.AddWithValue("@ctaId",ctaId );
                        mySqlCommand.Parameters.AddWithValue("@ccmFecha", fecha_aprobacion );
                        mySqlCommand.Parameters.AddWithValue("@ccmImporte",amount );
                        mySqlCommand.Parameters.AddWithValue("@ccmSaldo", ctaSaldo);
                        mySqlCommand.Parameters.AddWithValue("@ccmDetalle",String.Concat("Asignacion de productos No : ", ult_sol_pro));
                        mySqlCommand.Parameters.AddWithValue("@ccmNumeroTransaccion",sec_number_aut);
                        mySqlCommand.Parameters.AddWithValue("@ttrId","2000");
                        mySqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                      
                        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                        Message = "ERROR INESPERADO CREANDO LA KFN CUENTA CORRIENTE MOVIMIENTO (RESTA)";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }


                    ResponseCode = 41;
                    try
                    {
                        sec_number_auditoria = UpdateSecuence("AUDITORIA", mySqlCommand);
                    }
                    catch (ApiException ex)
                    {
                    
                        Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                        Message = ex.Message;
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }
                    catch (Exception ex)
                    {
                     
                        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                        Message = "ERROR INESPERADO ACTUALIZANDO SECUENCIA AUDITORIA" ;
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;
                    }

                    ResponseCode = 42;
                    try
                    {


                        //CREA KcrTransaccion
                        //sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                        //            "VALUES (" + sec_number_auditoria + "," + usr_id + ", null, 'Asignacion de productos'," + "'" + fecha_aprobacion.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + ult_sol_pro + ", 'DISTRIBUCION', 'SOLICITUDPRODUCTO')";
                        sentencia = @"INSERT INTO KcrTransaccion WITH(ROWLOCK)  (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio) 
                                      VALUES                                      (@traId,@usrId,@usrIdSuperior,@traComentario,@traFecha,@traIdReferencia,@traDominio,@traSubdominio)";
                        mySqlCommand.CommandText = sentencia;
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@traId", sec_number_auditoria );
                        mySqlCommand.Parameters.AddWithValue("@usrId", usr_id );
                        mySqlCommand.Parameters.AddWithValue("@usrIdSuperior", DBNull.Value);
                        mySqlCommand.Parameters.AddWithValue("@traComentario", "Asignacion de productos");
                        mySqlCommand.Parameters.AddWithValue("@traFecha", fecha_aprobacion);
                        mySqlCommand.Parameters.AddWithValue("@traIdReferencia",ult_sol_pro );
                        mySqlCommand.Parameters.AddWithValue("@traDominio", "DISTRIBUCION");
                        mySqlCommand.Parameters.AddWithValue("@traSubdominio", "SOLICITUDPRODUCTO");

                        mySqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                       
                        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                        Message = "ERROR INESPERADO CREANDO KCR TRANSACCION SOL PRODUCTO";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                        tran.Rollback();
                        return false;

                    }
                // }Fin del proceso para registar en cta cte




                Response_Code = "00";
                Message = "TRANSACCION OK";
                if(commitTransaction)
                tran.Commit();
                return true;

            }
            catch (Exception ex)
            {
                //Excepcion inesperada al momento de abrir la conexion
                #region RollBackPreventivo
                try
                {
                    //rollback preventivo
                    //solo en caso de que la transaccion se ha valida
                    //y no se este capturando algun error
                    if (tran != null)
                        tran.Rollback();
                }
                catch (Exception exin)
                {
                    logger.InfoLow(String.Concat("[QRY] NO SE REALIZO ROLLBACK PREVENTIVO"));
                }
                #endregion

                Response_Code = "01";
                Message = "ERROR AL TRANSFERIR SALDO";

                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                
                return false;

            }
            finally {
                if (commitTransaction)
                mySqlConnection.Close(); 
            }
        }

        /// <summary>
        /// Actualiza y retorna la secuencia de audotiria, bajo el contexto transaccinal indicado, si se pasa como parametro un SqlCommand 
        /// con una conexion abierta y una transaccion indicada
        /// </summary>
        /// <param name="secuencia">Secuencia en base de datos</param>
        /// <param name="withHints">Query Con hints de bloqueo</param>
        /// <param name="_command">Commando e</param>
        /// <returns></returns>
        private int  UpdateSecuence(string secuencia,SqlCommand _command= null)
        {
            //proceso anterior esto se realizaba por metodo
            //ejemplo la secuencia es TRANSACCION
            //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");

            //REALIZA SECUENCIA DE TRASANCCION
            //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName  =  @paramValue";
            //mySqlCommand.Parameters.Clear();
            //mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
            //mySqlCommand.ExecuteNonQuery();
            //mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName  =  @paramValue";
            //mySqlCommand.Parameters.Clear();
            //mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
            //sec_number_trans = (int)mySqlCommand.ExecuteScalar();

            var isintransaction = _command != null;
            var mySqlCommand = isintransaction ? _command : new SqlCommand();

            string messageException = String.Concat("NO SE PUDO ACTUALIZAR LA SECUENCIA ",secuencia);

            string UPDATEQUERY = //withHints?
                 "UPDATE secuencia  WITH (ROWLOCK) SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                //:"UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";

            string SELECTQUERY = "SELECT sec_number FROM secuencia WITH(NOLOCK) WHERE sec_objectName  = @paramValue";

            mySqlCommand.CommandText = UPDATEQUERY;
            mySqlCommand.Parameters.Clear();
            mySqlCommand.Parameters.AddWithValue("@paramValue", secuencia); //"DEPOSITO");

            //SI NO SE ESTA EN UN CONTEXTO TRANSACCIONAL
            if (!isintransaction)
            {
           
                 string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;

                 using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                 {
                     mySqlConnection.Open();
                     mySqlCommand.Connection = mySqlConnection;

                     int result = mySqlCommand.ExecuteNonQuery();
                     if (result == 0)
                         throw new ApiException(messageException);

                     mySqlCommand.CommandText = SELECTQUERY;
                     return (int)mySqlCommand.ExecuteScalar();
                 }

               
            }
            else
            {
                int result = mySqlCommand.ExecuteNonQuery();
                if (result == 0)
                    throw new ApiException(messageException);

                mySqlCommand.CommandText = SELECTQUERY;
                return (int)mySqlCommand.ExecuteScalar();
            }
     
          
        }

        public bool ProcesaAvisoDeposito(decimal DepositId, decimal age_id, int usr_id, decimal amount, string reference_number, DateTime date, string cuenta, string comentario, ref string Response_Code, ref string Message)
        {
            string age_estado = string.Empty;
            string cubNumero = string.Empty;
            decimal ctaSaldo;
            int sec_number_audit;
            int sec_number_trans;
            int sec_number_ctacte_mov;
            string AgenteNombre = string.Empty;
            string sentencia = string.Empty;
            //int usr_id;
            decimal cubId = 0;
            decimal ctaId = 0;
            string age_comisionadeposito = string.Empty;

            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            SqlConnection mySqlConnection = new SqlConnection(strConnString);
            SqlCommand mySqlCommand = new SqlCommand();

            SqlTransaction tran = null;

            try
            {
                mySqlConnection.Open();
                tran = mySqlConnection.BeginTransaction();
                mySqlCommand.Transaction = tran;
                mySqlCommand.Connection = mySqlConnection;



                //try
                //{
                //    
                //    //mySqlCommand.CommandText = "SELECT  convert(int,usr_id_modificacion) FROM  agente  WITH(NOLOCK)  WHERE   age_id 	=  '" + age_id + "'";
                //    mySqlCommand.CommandText = "SELECT  convert(int,usr_id) FROM  agente  WITH(NOLOCK)  WHERE   age_id 	=  '" + age_id + "'";
                //    usr_id = (int)mySqlCommand.ExecuteScalar();
                //}
                //catch (Exception ex)
                //{
                //    Response_Code = "10";
                //    Message = "USUARIO NO REGISTRADO ";
                //    return false;
                //}




                try
                {
                    
                    mySqlCommand.CommandText = "SELECT cubId    FROM dbo.KfnCuentaBanco WITH(ROWLOCK)  WHERE cubNumero = '" + cuenta + "' and ageId=0";
                    cubId = (decimal)mySqlCommand.ExecuteScalar();



                    
                    mySqlCommand.CommandText = "SELECT cubNumero    FROM dbo.KfnCuentaBanco WITH(ROWLOCK)  WHERE cubNumero = '" + cuenta + "' and ageId=0";
                    cubNumero = mySqlCommand.ExecuteScalar().ToString();

                    if (cubNumero.Trim() != cuenta.Trim())
                    {
                        Response_Code = "06";
                        Message = "CUENTA NO EXISTE";
                        return false;
                    }

                }
                catch (Exception ex)
                {
                    Response_Code = "06";
                    Message = "CUENTA NO EXISTE";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    return false;
                }


                try
                {
                    
                    mySqlCommand.CommandText = "SELECT  ctaId FROM KfnCuentaCorriente  WITH(ROWLOCK)  WHERE    ageid 	= '" + age_id + "'";
                    ctaId = (decimal)mySqlCommand.ExecuteScalar();

                }
                catch (Exception ex)
                {
                    Response_Code = "07";
                    Message = "CUENTA CTA. CTE. NO EXISTE";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    return false;
                }




                
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                sec_number_trans = (int)mySqlCommand.ExecuteScalar();


                //Leemos la cuenta corriente y su valor



                
                mySqlCommand.CommandText = "SELECT ctaSaldo FROM KfnCuentaCorriente  WITH (UPDLOCK, ROWLOCK) 	WHERE ageId = " + age_id.ToString();
                ctaSaldo = (decimal)mySqlCommand.ExecuteScalar();


                
                mySqlCommand.CommandText = "UPDATE KfnCuentaCorriente WITH(ROWLOCK)  SET ctaSaldo = ctaSaldo + " + amount.ToString().Replace(",", ".") + "  WHERE ctaId = " + ctaId.ToString() + " AND ageId = " + age_id + "AND ctaSaldo =" + ctaSaldo.ToString().Replace(",", ".");
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; 
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; 
                mySqlCommand.Parameters.Clear(); 
                mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                sec_number_ctacte_mov = (int)mySqlCommand.ExecuteScalar();

                
                sentencia = "INSERT INTO KfnCuentaCorrienteMovimiento (ccmId,ctaId,ccmFecha,ccmImporte  ,ccmSaldo,ccmDetalle,ccmNumeroTransaccion,ttrId)" +
                            "VALUES ( " + sec_number_ctacte_mov.ToString() + "," + ctaId.ToString() + ", GETDATE()" + "," + amount.ToString().Replace(",", ".") + "," + ctaSaldo.ToString().Replace(",", ".") + ", 'Registro de deposito enviado desde API-Movilway por el Monto: " + amount.ToString().Replace(",", ".") + "'" + "," + sec_number_trans + ", 1000)";

                
                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                
                sentencia = "UPDATE KfnDeposito SET  depEstado = 'AU', depComentarioProcesamiento = 'Deposito enviado desde API-Movilway por el Monto: " + amount.ToString().Replace(",", ".") + "'" +
                            ",ccmNumeroTransaccion =" + sec_number_trans + ", processingUsrId =" + usr_id + " ,depComentario = " + "'" + comentario + "'" + "  WHERE  depId =" + DepositId;

                
                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; 
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear(); 
                mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                sec_number_audit = (int)mySqlCommand.ExecuteScalar();


                
                sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                            "VALUES ( " + sec_number_audit + "," + usr_id + ", null, 'Deposito enviado desde API-Movilway por el monto " + amount + "'" + "," +
                            "GETDATE(), " + DepositId.ToString() + ", 'FINANCIERO', 'AUTORIZACION')";
                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();

                // Fin Registro de Deposito



                Response_Code = "00";
                Message = "TRANSACCION OK";
                tran.Commit();
                return true;

            }
            catch (Exception ex)
            {
                Response_Code = "01";
                Message = "ERROR AL REGISTAR DEPOSITO";
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                return false;

            }
            finally { mySqlConnection.Close(); }
        }

        /// <summary>
    ///Metodo para ser llamado desde el api, para los avisos de deposito que no afectan el saldo, solo registra el deposito y afecta la cta.cte.  
    /// </summary>
    /// <param name="age_id"></param>
    /// <param name="usr_id"></param>
    /// <param name="amount"></param>
    /// <param name="reference_number"></param>
    /// <param name="date"></param>
    /// <param name="cuenta"></param>
    /// <param name="comentario"></param>
    /// <param name="Response_Code"></param>
    /// <param name="Message"></param>
    /// <param name="fecha_aprobacion"></param>
    /// <param name="numero_sucursal"></param>
    /// <param name="nombre_sucursal"></param>
    /// <returns></returns>
        public bool RegistrarDeposito(decimal age_id, int usr_id, decimal amount, string reference_number, DateTime date, string cuenta, string comentario, ref string Response_Code, ref string Message, DateTime fecha_aprobacion, string numero_sucursal, string nombre_sucursal)
        {
            string age_estado = string.Empty;
            string cubNumero = string.Empty;
            decimal ctaSaldo;
            int sec_number_audit = 0;
            int sec_number_trans = 0;
            int sec_number_ctacte_mov;
            string AgenteNombre = string.Empty;
            string sentencia = string.Empty;
            //int usr_id;
            decimal cubId = 0;
            decimal ctaId = 0;
            decimal DepositId = 0;
            int sec_number_audit_fin = 0;
            string age_comisionadeposito = string.Empty;
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            SqlConnection mySqlConnection = new SqlConnection(strConnString);
            SqlCommand mySqlCommand = new SqlCommand();
            SqlDataReader reader = null;
            SqlTransaction tran = null;

            int ResponseCode = 1;

            int resultupdate = 0;
            try
            {
                mySqlConnection.Open();
                tran = mySqlConnection.BeginTransaction();
                mySqlCommand.Transaction = tran;
                mySqlCommand.Connection = mySqlConnection;



                //try
                //{
                //    
                //    //mySqlCommand.CommandText = "SELECT  convert(int,usr_id_modificacion) FROM  agente  WITH(NOLOCK)  WHERE   age_id 	=  '" + age_id + "'";
                //    mySqlCommand.CommandText = "SELECT  convert(int,usr_id) FROM  agente  WITH(NOLOCK)  WHERE   age_id 	=  '" + age_id + "'";
                //    usr_id = (int)mySqlCommand.ExecuteScalar();
                //}
                //catch (Exception ex)
                //{
                //    Response_Code = "10";
                //    Message = "USUARIO NO REGISTRADO ";
                //    return false;
                //}


                ResponseCode = 2;
                try
                {

                    //mySqlCommand.CommandText = @"SELECT cubId     
                    //                             FROM dbo.KfnCuentaBanco WITH(NOLOCK)  
                    //                             WHERE cubNumero = @cuenta  and ageId=@ageId";
                    //mySqlCommand.Parameters.AddWithValue("@cuenta", cuenta);
                    //mySqlCommand.Parameters.AddWithValue("@ageId", cons.AGENTE_CONCENTRADOR);


                    bool checkAccountsInRoot = true;
                    checkAccountsInRoot = String.IsNullOrEmpty(ConfigurationManager.AppSettings["CheckAccountsInRoot"]) ? true : bool.Parse(ConfigurationManager.AppSettings["CheckAccountsInRoot"]);

                    if (checkAccountsInRoot)
                        mySqlCommand.CommandText = "SELECT cubId,cubNumero FROM dbo.KfnCuentaBanco WITH(NOLOCK) WHERE cubNumero = @cuenta  and ageId = @age_id";
                    else
                        mySqlCommand.CommandText = "SELECT cubId,cubNumero FROM dbo.KfnCuentaBanco WITH(NOLOCK) WHERE cubNumero = @cuenta  and ageId = (SELECT age_id_sup FROM dbo.Agente with(NOLOCK) WHERE age_id=@age_id)";

                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@cuenta", cuenta);

                    if (checkAccountsInRoot)
                        mySqlCommand.Parameters.AddWithValue("@age_id", cons.AGENTE_CONCENTRADOR);
                    else
                        mySqlCommand.Parameters.AddWithValue("@age_id", age_id);

                    cubId = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                 
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR SELECCIONANDO CUENTA NO EXISTE";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                ResponseCode = 3;
                try
                {

                  

                    DepositId = UpdateSecuence("DEPOSITO", mySqlCommand);
                }
                catch (ApiException ex)
                {
               
                    Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                    Message = ex.Message;
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO ACTUALIZANDO SECUENCIA";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                ResponseCode = 4;
                try
                {

                    sec_number_audit = UpdateSecuence("AUDITORIA", mySqlCommand);
                }
                catch (ApiException ex)
                {
              
                    Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                    Message = ex.Message;
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO ACTUALIZANDO SECUENCIA AUDITORIA";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }


                ResponseCode = 5;
                try
                {
                    //sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                    //            "VALUES ( " + sec_number_audit + "," + usr_id + ", null, " + "'" + comentario + " por el monto " + amount.ToString() + "'" + "," + "'" +
                    //            date.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ", " + DepositId.ToString() + ", 'FINANCIERO', 'AVISODEPOSITO')";

                    sentencia = "INSERT INTO KcrTransaccion WITH(ROWLOCK)  (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio) VALUES(@traId,@usrId,@usrIdSuperior,@traComentario,@traFecha,@traIdReferencia,@traDominio,@traSubdominio)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@traId", sec_number_audit );
                    mySqlCommand.Parameters.AddWithValue("@usrId",usr_id);
                    mySqlCommand.Parameters.AddWithValue("@usrIdSuperior",DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@traComentario", String.Concat(comentario , " por el monto " , amount.ToString(CultureInfo.InvariantCulture)));
                    mySqlCommand.Parameters.AddWithValue("@traFecha", date);
                    mySqlCommand.Parameters.AddWithValue("@traIdReferencia",  DepositId);
                    mySqlCommand.Parameters.AddWithValue("@traDominio", "FINANCIERO");
                    mySqlCommand.Parameters.AddWithValue("@traSubdominio", "AVISODEPOSITO");
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
              
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO CREANDO KCR TRANSACCION DEPOSITO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                ResponseCode = 6;
                try
                {


                    //sentencia = "INSERT INTO KfnDeposito (depId,ageIdOrigen,cubId,depComprobante,depFechaComprobante,depFecha,depMonto,depEstado,depComentario,depComentarioProcesamiento,ccmNumeroTransaccion,usrId,processingUsrId)" +
                    //            "VALUES ( " + DepositId + "," + age_id + "," + cubId + ", " + "'" + reference_number + "'" + "," + "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + fecha_aprobacion.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ", " + amount + "," + "'PE'" + "," + "'" + comentario + "'" + ", 'Registro de deposito enviado desde API-Movilway por el Monto: " + amount.ToString().Replace(",", ".") + "'" + ",null ," + usr_id + ", null)";


                    sentencia = @"INSERT INTO KfnDeposito WITH(ROWLOCK)  (depId,ageIdOrigen,cubId,depComprobante,depFechaComprobante,depFecha,depMonto,depEstado,depComentario,depComentarioProcesamiento,ccmNumeroTransaccion,usrId,processingUsrId,depNumeroTerminal,depNombreTerminal)
                    VALUES(@depId,@ageIdOrigen,@cubId,@depComprobante,@depFechaComprobante,@depFecha,@depMonto,@depEstado,@depComentario,@depComentarioProcesamiento,@ccmNumeroTransaccion,@usrId,@processingUsrId,@depNumeroTerminal,@depNombreTerminal)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@depId",DepositId );
                    mySqlCommand.Parameters.AddWithValue("@ageIdOrigen",age_id);
                    mySqlCommand.Parameters.AddWithValue("@cubId", cubId );
                    mySqlCommand.Parameters.AddWithValue("@depComprobante", reference_number);
                    mySqlCommand.Parameters.AddWithValue("@depFechaComprobante",  date);
                    mySqlCommand.Parameters.AddWithValue("@depFecha",  fecha_aprobacion);
                    mySqlCommand.Parameters.AddWithValue("@depMonto",  amount );
                    mySqlCommand.Parameters.AddWithValue("@depEstado",  "PE" );
                    mySqlCommand.Parameters.AddWithValue("@depComentario",  comentario);
                    mySqlCommand.Parameters.AddWithValue("@depComentarioProcesamiento",String.Concat( "Registro de deposito enviado desde API-Movilway por el Monto: " , amount.ToString(CultureInfo.InvariantCulture) ));
                    mySqlCommand.Parameters.AddWithValue("@ccmNumeroTransaccion", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@usrId",  usr_id );
                    mySqlCommand.Parameters.AddWithValue("@processingUsrId", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@depNumeroTerminal", numero_sucursal);
                    mySqlCommand.Parameters.AddWithValue("@depNombreTerminal", nombre_sucursal);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                 
                    if (ex.Class == 16)
                    {
                        Response_Code = UtilResut.StrDbErrorCode(ResponseCode);
                        Message = ex.Message;
                        if (ex.Message.IndexOf("UI_MESSAGE_END") > 0)
                        {
                            var resultado = Message.Split(new string[] { "UI_MESSAGE_START", "UI_MESSAGE_END" }, StringSplitOptions.RemoveEmptyEntries);
                            Message = resultado[0];
                        }
                        //"ERROR INESPERADO CREANDO EL DEPOSITO";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    }
                    else
                    {
                        Response_Code = UtilResut.StrDbErrorCode(ResponseCode);
                        Message = "ERROR INESPERADO CREANDO EL DEPOSITO SQLEXCEPTION";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    }
                    tran.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                  
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO CREANDO KFN DEPOSITO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                //ResponseCode = 7;
                //try
                //{
                //    //TODO UPDATE + 
                //    //sentencia = "UPDATE KfnDeposito  SET  depNumeroTerminal =" + "'" + numero_sucursal + "'" + ",depNombreTerminal=" + "'" + nombre_sucursal + "'" + " WHERE depId = " + DepositId;
                //    sentencia = "UPDATE KfnDeposito  SET  depNumeroTerminal = @numero_sucursal ,depNombreTerminal = @nombre_sucursal WHERE depId = @DepositId ";

                //    mySqlCommand.Parameters.Clear();
                //    mySqlCommand.Parameters.AddWithValue("@numero_sucursal", numero_sucursal);
                //    mySqlCommand.Parameters.AddWithValue("@nombre_sucursal", nombre_sucursal);
                //    mySqlCommand.Parameters.AddWithValue("@DepositId", DepositId);
                //    mySqlCommand.CommandText = sentencia;
                //    resultupdate =  mySqlCommand.ExecuteNonQuery();
                //    if (resultupdate == 0) 
                //    {
                  
                //        Response_Code = UtilResut.StrErrorCode(ResponseCode);
                //        Message = "ERROR INESPERADO ACTUALIZA KFN DEPOSITO";
                //        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                //        tran.Rollback();
                //        return false;
                //    }
                //}
                //catch (Exception ex)
                //{
                   
                //    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                //    Message = "ERROR INESPERADO ACTUALIZA KFN DEPOSITO";
                //    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                //    tran.Rollback();
                //    return false;
                 
                //}

                ResponseCode = 8;
                try
                {

                    sec_number_trans = UpdateSecuence("TRANSACCION", mySqlCommand);
                }
                catch (ApiException ex)
                {
            
                    Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                    Message = ex.Message;
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO ACTUALIZANDO SECUENCIA TRANSACCION";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }



                //Leemos la cuenta corriente y su valor
                ResponseCode = 9;
                try
                {
                    //mySqlCommand.CommandText = "SELECT  ctaId FROM KfnCuentaCorriente  WITH(ROWLOCK)  WHERE    ageid 	= '" + age_id + "'";


                    //mySqlCommand.CommandText = "SELECT ctaSaldo FROM KfnCuentaCorriente  WITH (UPDLOCK, ROWLOCK) 	WHERE ageId = " + age_id.ToString();
                    //ctaSaldo = (decimal)mySqlCommand.ExecuteScalar();

                    mySqlCommand.CommandText = "SELECT  ctaId,ctaSaldo FROM KfnCuentaCorriente  WITH(ROWLOCK)  WHERE ageid = @age_id";
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    using (reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            ctaId = (decimal)reader["ctaId"];
                            ctaSaldo = (decimal)reader["ctaSaldo"];
                        }
                        else
                        {
                      
                            Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                            Message = String.Concat("NO SE ENCONTRO DATOS DE CUENTA CTA. CTE. PARA AGENTE ",age_id);
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            tran.Rollback();
                            return false;
                        }
                    }
                 

                }
                catch (Exception ex)
                {
              
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO CUENTA CTA. CTE. NO EXISTE";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                ResponseCode = 10;
                try
                {
                    //mySqlCommand.CommandText = "UPDATE KfnCuentaCorriente WITH(ROWLOCK)  SET ctaSaldo = ctaSaldo + " + amount.ToString().Replace(",", ".") + "  WHERE ctaId = " + ctaId.ToString() + " AND ageId = " + age_id + "AND ctaSaldo =" + ctaSaldo.ToString().Replace(",", ".");
                    //TODO NO SE ESTA VALIDANDO QUE EL SALDO QUEDE VALIDO
                    mySqlCommand.CommandText = "UPDATE KfnCuentaCorriente WITH(ROWLOCK)  SET ctaSaldo = ctaSaldo + @amount  WHERE ctaId = @ctaId  AND ageId = @age_id AND ctaSaldo = @ctaSaldo";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@ctaId", ctaId);
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    mySqlCommand.Parameters.AddWithValue("@ctaSaldo", ctaSaldo);
                    resultupdate =  mySqlCommand.ExecuteNonQuery();
                    if (resultupdate == 0)
                    {
                      
                        Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                        Message = "NO SE PUDO ACTUALIZAR SALDO KFN CUENTA CORRIENTE";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (SqlException ex)
                {
                    Response_Code = UtilResut.StrDbErrorCode(ResponseCode);
                    Message = String.Concat("ERROR ACTUALIZANDO KFN CUENTA CORRIENTE ","[", ex.Message,"]");
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
           
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO ACTUALIZANDO KFN CUENTA CORRIENTE";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                ResponseCode = 11;
                try
                {
                   
                    sec_number_ctacte_mov = UpdateSecuence("CTACTEMOVIMIENTO", mySqlCommand);
                }
                catch (ApiException ex)
                {
    
                    Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                    Message = ex.Message;
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                   
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO ACTUALIZANDO SECUENCIA CTACTEMOVIMIENTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                ResponseCode = 12;
                try
                {

                    //sentencia = "INSERT INTO KfnCuentaCorrienteMovimiento (ccmId,ctaId,ccmFecha,ccmImporte  ,ccmSaldo,ccmDetalle,ccmNumeroTransaccion,ttrId)" +
                    //            "VALUES ( " + sec_number_ctacte_mov.ToString() + "," + ctaId.ToString() + "," + "'" + fecha_aprobacion.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + amount.ToString().Replace(",", ".") + "," + ctaSaldo.ToString().Replace(",", ".") + ", 'Registro de deposito enviado desde API-Movilway por el Monto: " + amount.ToString().Replace(",", ".") + "'" + "," + sec_number_trans + ", 1000)";
                    sentencia = @"
                                INSERT INTO KfnCuentaCorrienteMovimiento  WITH(ROWLOCK)        (ccmId,ctaId,ccmFecha,ccmImporte  ,ccmSaldo,ccmDetalle,ccmNumeroTransaccion,ttrId)
                                VALUES
                               (@ccmId,@ctaId,@ccmFecha,@ccmImporte  ,@ccmSaldo,@ccmDetalle,@ccmNumeroTransaccion,@ttrId)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
    
                    mySqlCommand.Parameters.AddWithValue("@ccmId", sec_number_ctacte_mov.ToString() );
                    mySqlCommand.Parameters.AddWithValue("@ctaId",ctaId);
                    mySqlCommand.Parameters.AddWithValue("@ccmFecha", fecha_aprobacion);
                    mySqlCommand.Parameters.AddWithValue("@ccmImporte", amount);
                    mySqlCommand.Parameters.AddWithValue("@ccmSaldo",ctaSaldo);
                    mySqlCommand.Parameters.AddWithValue("@ccmDetalle", String.Concat("Registro de deposito enviado desde API-Movilway por el Monto: " , amount.ToString(CultureInfo.InvariantCulture)));
                    mySqlCommand.Parameters.AddWithValue("@ccmNumeroTransaccion", sec_number_trans );
                    mySqlCommand.Parameters.AddWithValue("@ttrId", " 1000");
               
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO CREANDO KFN CUENTA CORRIENTE MOVIMIENTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                ResponseCode = 13;
                try
                {
                    //sentencia = "UPDATE KfnDeposito SET  depEstado = 'AU', depComentarioProcesamiento = 'Deposito enviado desde API-Movilway por el Monto: " + amount.ToString().Replace(",", ".") + "'" +
                    //      ",ccmNumeroTransaccion =" + sec_number_trans + ", processingUsrId =" + usr_id + " ,depComentario = " + "'" + comentario + "'" + "  WHERE  depId =" + DepositId;
                    sentencia = @"
                                    UPDATE KfnDeposito WITH(ROWLOCK)
                                    SET     depEstado = @depEstado , 
                                            depComentarioProcesamiento = @depComentarioProcesamiento ,
                                            ccmNumeroTransaccion = @sec_number_trans ,
                                            processingUsrId = @usr_id ,
                                            depComentario = @comentario  
                                            WHERE  depId = @DepositId ";


                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@depEstado", "AU");
                    mySqlCommand.Parameters.AddWithValue("@depComentarioProcesamiento", String.Concat("Deposito enviado desde API-Movilway por el Monto: ", amount.ToString(CultureInfo.InvariantCulture)) );
                    mySqlCommand.Parameters.AddWithValue("@sec_number_trans", sec_number_trans);
                    mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@comentario", comentario);
                    mySqlCommand.Parameters.AddWithValue("@DepositId", DepositId);
                    resultupdate =  mySqlCommand.ExecuteNonQuery();
                    if (resultupdate == 0)
                    {
                  
                        Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                        Message = "NO SE PUDO ACTUALIZAR KFN DEPOSITO";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
              
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO ACTUALIZANDO KFN DEPOSITO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
              




                ResponseCode = 14;
                try
                {
            
                    sec_number_audit_fin = UpdateSecuence("AUDITORIA", mySqlCommand);
                }
                catch (ApiException ex)
                {
          
                    Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                    Message = ex.Message;
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
               
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO ACTUALIZANDO SECUENCIA AUDITORIA";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                ResponseCode = 15;
                try
                {
                    //sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                    //    "VALUES ( " + sec_number_audit_fin + "," + usr_id + ", null, " + "'" + comentario + " por el monto " + amount.ToString() + "'" + "," + "'" +
                    //    date.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ", " + DepositId.ToString() + ", 'FINANCIERO', 'AUTORIZACION')";
                    sentencia = @"INSERT INTO KcrTransaccion WITH(ROWLOCK)
                                (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)
                                VALUES 
                                (@traId,@usrId,@usrIdSuperior,@traComentario,@traFecha,@traIdReferencia,@traDominio,@traSubdominio)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@traId", sec_number_audit_fin );
                    mySqlCommand.Parameters.AddWithValue("@usrId", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@usrIdSuperior", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@traComentario", String.Concat( comentario ," por el monto " , amount.ToString(CultureInfo.InvariantCulture)));
                    mySqlCommand.Parameters.AddWithValue("@traFecha", date);//.ToString(yyyy-MM-dd HH:mm:ss) +  + "
                    mySqlCommand.Parameters.AddWithValue("@traIdReferencia", DepositId);
                    mySqlCommand.Parameters.AddWithValue("@traDominio", "FINANCIERO");
                    mySqlCommand.Parameters.AddWithValue("@traSubdominio", "AUTORIZACION");
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
             
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO KRC TRANSACCION";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                
            

                // Fin Registro de Deposito

                Response_Code = "00";
                Message = "TRANSACCION OK";
                tran.Commit();
                return true;

            }
            catch (Exception ex)
            {
                Response_Code = "01";
                Message = "ERROR AL REGISTAR DEPOSITO";
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                return false;

            }
            finally { mySqlConnection.Close(); }
        }

        /// <summary>
        /// Realiza una solicitud de producto
        /// </summary>
        /// <param name="age_id"></param>
        /// <param name="amount"></param>
        /// <param name="date"></param>
        /// <param name="Response_Code"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public bool RegistrarSolicitudProducto(decimal age_id, decimal amount, DateTime date, ref string Response_Code, ref string Message)
        {
            logger.InfoLow(() => TagValue.New().Tag("[OP]").Value("RegistrarSolicitudProducto"));
            string age_estado = string.Empty;
            string cubNumero = string.Empty;
            decimal ctaSaldo;
            decimal IdBodegaOrigen = 0;
            decimal IdBodegaDestino = 0;
            int sec_number_audit= 0;
            decimal ult_sol_pro = 0m;
            string AgenteNombre = string.Empty;
            string sentencia = string.Empty;
            int usr_id;
            decimal LimiteCredito = 0;
            decimal credito = 0;
            string sLimiteCredito = string.Empty;
            string age_comisionadeposito = string.Empty;

            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            SqlConnection mySqlConnection = new SqlConnection(strConnString);
            SqlCommand mySqlCommand = new SqlCommand();

            SqlTransaction tran = null;
            //variable auxiliar para resultadod e update
            int resupdate = 0;
            //variable para ir acumulando los errores
            int ResponseCode = 1;
            try
            {
                mySqlConnection.Open();
                tran = mySqlConnection.BeginTransaction();
                mySqlCommand.Transaction = tran;
                mySqlCommand.Connection = mySqlConnection;

                ResponseCode = 2;
                try
                {
                    //SELECCIONA EL USUARIO TIPO WEB DEL AGENTE
                    //mySqlCommand.CommandText = "SELECT  usr_id FROM  agente  WITH(NOLOCK)  WHERE   age_id 	=  '" + age_id + "'";
                    mySqlCommand.CommandText = "SELECT  usr_id FROM  agente  WITH(NOLOCK)  WHERE   age_id 	=  @age_id";
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    usr_id = Convert.ToInt32(mySqlCommand.ExecuteScalar().ToString());
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);//"10";
                    Message = "USUARIO NO REGISTRADO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                ResponseCode = 3;
                try
                {
                    //SELECCIONA EL SALDO DEL AGENTE KfnCuentaCorriente
                    // mySqlCommand.CommandText = "SELECT  ctaSaldo FROM KfnCuentaCorriente  WITH(ROWLOCK)   WHERE    ageid 	= '" + age_id + "'";
                    mySqlCommand.CommandText = "SELECT  ctaSaldo FROM KfnCuentaCorriente  WITH(ROWLOCK)   WHERE    ageid 	= @age_id ";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("age_id", age_id);
                    ctaSaldo = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO SELECCIONADO SALDO EN CTA. CTE. NO EXISTE";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                ResponseCode = 4;
                try
                {
                    //DETERMINA EL CREDITO DEL QUE DISPONE EL AGENTE
                    //mySqlCommand.CommandText = "SELECT  ataValor FROM dbo.KcrAtributoAgencia  WITH(ROWLOCK)   WHERE    ageid 	= '" + age_id + "' and attId='LimiteCredito'";
                    mySqlCommand.CommandText = "SELECT  ataValor FROM dbo.KcrAtributoAgencia  WITH(ROWLOCK)   WHERE    ageid 	=  @age_id  and attId = @attId ";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    mySqlCommand.Parameters.AddWithValue("@attId", "LimiteCredito");
                    sLimiteCredito = mySqlCommand.ExecuteScalar().ToString();
                    LimiteCredito = Convert.ToDecimal(sLimiteCredito);
                    credito = LimiteCredito + ctaSaldo;

                    //SI EL MONTO SUPERA EL CREDITO ENTONCES INFORMA EL ERROR
                    if (amount > credito)
                    {
                      
                        Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                        Message = "LA AGENCIA NO POSEE LIMITE DE CREDITO PARA ESTA TRANSACCION";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO SELECCIONADO SALDO EN CTA. CTE. NO EXISTE";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                ResponseCode = 5;
                try
                {
                    //mySqlCommand.CommandText = "SELECT age_id_sup  FROM [Agente] WITH(NOLOCK)   WHERE age_id = @age_id";
                    //TODO ES CORRECTO QUE EL EL DESTINO SE HA MI AGENTE PADRE
                    mySqlCommand.CommandText = "SELECT age_id_sup  FROM [Agente] WITH(NOLOCK)   WHERE age_id = @age_id";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    IdBodegaDestino = (decimal)mySqlCommand.ExecuteScalar();
                    IdBodegaOrigen = age_id;
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO AL SELECCIONAR EL AGENTE SUPERIOR";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }


                ResponseCode = 6;
                try
                {
                 
                    ult_sol_pro = UpdateSecuence("SOLICITUDPRODUCTO", mySqlCommand);

                }
                catch (ApiException ex)
                {
                    Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                    Message = ex.Message;
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO ACTUALIZANDO SECUENCIA";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                /* inserto una solicitud y la coloco en estado pendiente */
                ResponseCode = 7;
                try
                {
                    //sentencia = "INSERT INTO [KlgSolicitudProducto] WITH(ROWLOCK) (sprId,usrId,ageIdSolicitante,ageIdDestinatario,prvIdDestinatario,sprEstado,sprFecha,sprFechaAprobacion,sprImporteSolicitud,sltId,ageIdBodegaOrigen,ageIdBodegaDestino) " +
                    //            "VALUES (" + ult_sol_pro.ToString() + "," + usr_id.ToString() + "," + IdBodegaOrigen.ToString() + "," + IdBodegaDestino.ToString() + ", NULL,'PE','" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ",null," + amount.ToString().Replace(",", ".") + ",201," + IdBodegaOrigen.ToString() + "," + IdBodegaDestino.ToString() + ")";

                    sentencia = @"INSERT INTO [KlgSolicitudProducto] WITH(ROWLOCK) (sprId,usrId,ageIdSolicitante,ageIdDestinatario,prvIdDestinatario,sprEstado,sprFecha,sprFechaAprobacion,sprImporteSolicitud,sltId,ageIdBodegaOrigen,ageIdBodegaDestino) 
                        VALUES  (@sprId,@usrId,@ageIdSolicitante,@ageIdDestinatario,@prvIdDestinatario,@sprEstado,@sprFecha,@sprFechaAprobacion,@sprImporteSolicitud,@sltId,@ageIdBodegaOrigen,@ageIdBodegaDestino)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@sprId", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@usrId", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@ageIdSolicitante", IdBodegaOrigen);
                    mySqlCommand.Parameters.AddWithValue("@ageIdDestinatario", IdBodegaDestino);
                    mySqlCommand.Parameters.AddWithValue("@prvIdDestinatario", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@sprEstado", "PE");
                    mySqlCommand.Parameters.AddWithValue("@sprFecha", date);
                    mySqlCommand.Parameters.AddWithValue("@sprFechaAprobacion", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@sprImporteSolicitud", amount);
                    mySqlCommand.Parameters.AddWithValue("@sltId", "201");
                    mySqlCommand.Parameters.AddWithValue("@ageIdBodegaOrigen", IdBodegaDestino);
                    mySqlCommand.Parameters.AddWithValue("@ageIdBodegaDestino", IdBodegaOrigen);

                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR AL CREAR KLG SOLICITUD PRODUCTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                //Se actualiza el campo usrIdInitiator en caso de existir
                ResponseCode = 8;
                try
                {

                    //sentencia = "UPDATE [KlgSolicitudProducto] WITH(ROWLOCK) SET usrIdInitiator = " + usr_id.ToString() + " WHERE sprId =" + ult_sol_pro;
                    sentencia = "UPDATE [KlgSolicitudProducto] WITH(ROWLOCK) SET usrIdInitiator = @usr_id  WHERE sprId = @ult_sol_pro";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    resupdate = mySqlCommand.ExecuteNonQuery();
                    if (resupdate == 0)
                    {
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, "NO SE ACTUALIZARON CAMPOS PARA KLG SOLICITUD PRODUCTO"));
                    }
                }
                catch (Exception ex)
                {
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                }

                ResponseCode = 9;
                try
                {
                    //REALIZA LA SOLICITUD DEL ITEM
                    //sentencia = "INSERT INTO [KlgSolicitudProductoItem] WITH(ROWLOCK) (sprId,prdId,spiCantidadSolicitada,spiCantidadAutorizada,spiPrecioUnitario,spiEstado) " +
                    //            "VALUES (" + ult_sol_pro + ",0," + amount + "," + amount + ", 1.0000,'PE')";

                    sentencia = @"INSERT INTO [KlgSolicitudProductoItem] WITH(ROWLOCK) (sprId,prdId,spiCantidadSolicitada,spiCantidadAutorizada,spiPrecioUnitario,spiEstado)
                        VALUES
                        (@sprId,@prdId,@spiCantidadSolicitada,@spiCantidadAutorizada,@spiPrecioUnitario,@spiEstado)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@sprId", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@prdId", cons.MULTI_PRODUCTO);
                    mySqlCommand.Parameters.AddWithValue("@spiCantidadSolicitada", amount);
                    mySqlCommand.Parameters.AddWithValue("@spiCantidadAutorizada", amount);
                    mySqlCommand.Parameters.AddWithValue("@spiPrecioUnitario", 1m);//" 1.0000");
                    mySqlCommand.Parameters.AddWithValue("@spiEstado", "PE");

                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR AL CREAR KLG SOLICITUD PRODUCTO ITEM";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                ResponseCode = 10;
                try
                {

                   
                    sec_number_audit = UpdateSecuence("AUDITORIA", mySqlCommand);
                }
                catch (ApiException ex)
                {
                    Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                    Message = ex.Message;
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR INESPERADO ACTUALIZANDO SECUENCIA AUDITORIA";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                ResponseCode = 11;
                try
                {
                    //REGISTRA LA SOLICITUD EN KcrTransaccion
                    //sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                    //             "VALUES (" + sec_number_audit.ToString() + "," + usr_id.ToString() + ", null, 'Creación de la solicitud nro. :" + ult_sol_pro.ToString() + "'" + "," + "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'" + " ," + ult_sol_pro.ToString() + ", 'DISTRIBUCION', 'SOLICITUDPRODUCTO')";

                    sentencia = @"INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)
                        VALUES
                        (@traId,@usrId,@usrIdSuperior,@traComentario,@traFecha,@traIdReferencia,@traDominio,@traSubdominio)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@traId", sec_number_audit);
                    mySqlCommand.Parameters.AddWithValue("@usrId", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@usrIdSuperior", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@traComentario", String.Concat("Creación de la solicitud nro. :", ult_sol_pro));
                    mySqlCommand.Parameters.AddWithValue("@traFecha", date);
                    mySqlCommand.Parameters.AddWithValue("@traIdReferencia", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@traDominio", "DISTRIBUCION");
                    mySqlCommand.Parameters.AddWithValue("@traSubdominio", "SOLICITUDPRODUCTO");

                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR CREANDO KCR TRANSACCION SOLICITUD PRODUCTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    return false;
                }

                // Fin Registro de Deposito
                Response_Code = "00";
                Message = "TRANSACCION OK";
                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Response_Code = "01";
                Message = "ERROR AL REGISTAR LA SOLICITUD DE PRODUCTO";
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                return false;
            }
            finally { mySqlConnection.Close(); }
        }

     /// <summary>
    ///Metodo interno realiza la distribucion de saldo de los avisos de depositos, con respaldo de deposito, registra el pago y afecta la cta cte.
     /// <param name="_mySqlConnection">conexion que se esta utilizando, de lo contrario es null </param>
     /// <param name="_mySqlCommand">transaccon asociada a la conexion que se esta utilizando</param>
     /// <param name="_tran"></param>
        /// <returns>true si pudo realizar la la distribucion de saldo de los avisos de depositos</returns>
        public bool Acreditacion(decimal age_id, decimal amount, int usr_id, string reference_number, DateTime date, string cuenta, string condeposito, decimal ctaId, decimal cubId, ref string Response_Code, ref string Message, string Comentario, decimal depId, DateTime fecha_aprobacion, SqlConnection _mySqlConnection = null, SqlTransaction _tran = null)
        {
            string age_estado = string.Empty;
            string cubNumero = string.Empty;
            decimal ctaSaldo;
            decimal IdAgenteSolicitante = 0;
            decimal IdBodegaOrigen = 0;
            decimal IdBodegaDestino = 0;          
            int sec_number_audit;
            int sec_number_trans;
            decimal ctaIdCorriente = 0;
            int sec_number_ctacte_mov;
            decimal ult_sol_pro;
   
            decimal ult_env_pro;
            decimal StockIdOrigen;
            decimal StockIdDestino;
            decimal sec_number_auditoria;
            int sec_number_aut;
            int sec_numbercta_movi;
            string AgenteNombre = string.Empty;
            string sentencia = string.Empty;
            string age_comisionadeposito = string.Empty;
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;

            //variable auxiliar para concatenar los errores en lambdas
            string _m="";

            SqlConnection mySqlConnection = null;
            SqlTransaction tran = null;
            SqlCommand mySqlCommand = new SqlCommand();
            SqlDataReader reader = null;
            //indica si la transaccion debe hacer commit
            bool committransaction = false ;
            try
            {
                if (_mySqlConnection == null){
                    committransaction = true;
                    mySqlConnection = new SqlConnection(strConnString);
                    mySqlConnection.Open();
                    tran = mySqlConnection.BeginTransaction();
                }
                else if(_mySqlConnection != null && _tran !=null) {
                    committransaction = false;
                    mySqlConnection = _mySqlConnection;
                    tran = _tran;
                }
                else
                {
                    throw new Exception("Parámetros de conexión incorrectos");
                }

                mySqlCommand.Transaction = tran;
                mySqlCommand.Connection = mySqlConnection;
               
                #region inicializacion anterior
                /*    
                SqlConnection mySqlConnection = new SqlConnection(strConnString);
                SqlCommand mySqlCommand = new SqlCommand();
                SqlTransaction tran = null;

                try
                {
                mySqlConnection.Open();
                tran = mySqlConnection.BeginTransaction();
                mySqlCommand.Transaction = tran;
                mySqlCommand.Connection = mySqlConnection;
                */
                #endregion
                //Proceso de distribucion 

                try
                {
                    //mySqlCommand.CommandText = "SELECT age_id_sup  FROM [Agente] WITH(NOLOCK)   WHERE age_id = " + age_id;
                    mySqlCommand.CommandText = "SELECT  age_id_sup FROM [Agente] WITH(NOLOCK) WHERE age_id=@age_id";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    IdAgenteSolicitante = (decimal)mySqlCommand.ExecuteScalar();
                    IdBodegaOrigen = IdAgenteSolicitante;
                    IdBodegaDestino = age_id;
                }
                catch (Exception ex)
                {
                    Response_Code = "01";
                    Message = string.Empty;
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }




                // Inicio Registro de Deposito
                Response_Code = "02";
                try
                {
                    //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                    mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                    if (mySqlCommand.ExecuteNonQuery() == 0) { 
              
                        Message = "NO SE PUDO ACTUALIZAR LA SECUENCIA DE TRANSACCION";
                      
                        logger.ErrorLow(String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                        tran.Rollback();
                        return false;
                  
                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR LA SECUENCIA DE TRANSACCION";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
               
                
                try
                {
				    // mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                    mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                    sec_number_trans = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "03";
                    Message = "ERROR AL SELECCIONAR LA SECUENCIA TRANSACCION";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                //Leemos la cuenta corriente y su valor

                //se cambia a una sola lectura
               try
               {
			       // mySqlCommand.CommandText = "SELECT   ctaId  FROM KfnCuentaCorriente  WITH (UPDLOCK, ROWLOCK) 	 WHERE ageId = " + age_id.ToString();
                   mySqlCommand.CommandText = "SELECT ctaId,ctaSaldo FROM KfnCuentaCorriente  WITH(UPDLOCK, ROWLOCK) WHERE ageId=@age_id";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    //ctaIdCorriente = (decimal)mySqlCommand.ExecuteScalar();
                    using (reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            ctaIdCorriente = (decimal)reader["ctaId"];
                            ctaSaldo = (decimal)reader["ctaSaldo"];
                        }
                        else
                        {
                            Message = "NO SE PUDIERON LEER LOS DATOS DE LA CUENTA CORRIENTE";
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                            return false;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Response_Code = "04";
                    Message = "";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                /* 
                //se reemplaza por una sola lectura
                try
                {
                    //mySqlCommand.CommandText = "SELECT ctaSaldo FROM KfnCuentaCorriente  WITH (UPDLOCK, ROWLOCK) 	WHERE ageId = " + age_id.ToString();
                    mySqlCommand.CommandText = "SELECT ctaSaldo FROM KfnCuentaCorriente  WITH(UPDLOCK, ROWLOCK) WHERE ageId = @age_id";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    ctaSaldo = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "05";
                    Message = "";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }*/
              ///
               Response_Code = "06";
                try
                {
               
                   //mySqlCommand.CommandText = "UPDATE  KfnCuentaCorriente WITH(ROWLOCK)  SET ctaSaldo = ctaSaldo + " + amount.ToString().Replace(",", ".") + "  WHERE ctaId = " + ctaId.ToString() + " AND ageId = " + age_id + "AND ctaSaldo =" + ctaSaldo.ToString().Replace(",", ".");
                    mySqlCommand.CommandText = "UPDATE KfnCuentaCorriente WITH(ROWLOCK)  SET ctaSaldo =  ctaSaldo +  @amount  WHERE ctaId = @ctaId AND ageId = @age_id AND ctaSaldo = @ctaSaldo";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@ctaId", ctaId);
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    mySqlCommand.Parameters.AddWithValue("@ctaSaldo", ctaSaldo);
                    if(mySqlCommand.ExecuteNonQuery() == 0)
                     {    
                         Message = "NO SE PUDO ACTUALIZAR EL SALDO EN CUENTA CORRIENTE";
                     _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                     logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                     tran.Rollback();
                     return false;

                     }

                }
                catch (Exception ex)
                {
                    Message = "ERROR AL ACTUALIZAR SALDO EN CUENTA CORRIENTE";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                Response_Code = "07";
                try
                {
				    //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                    mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");

                    if(mySqlCommand.ExecuteNonQuery()== 0){
                        Message = "NO SE PUDO ACTUALIZAR SECUENCIA EN CUENTA MOVIMIENTO";
                        logger.ErrorLow(String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message));
                        tran.Rollback();
                        return false;
                    }

                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR SECUENCIA EN CUENTA MOVIMIENTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

               
                try
                {
				    //mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                    mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                    sec_number_ctacte_mov = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "08";
                    Message = "ERROR AL SELECCIONAR LA SECUENCIA CTACTEMOVIMIENTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
               
                try
                {
				//sentencia = "INSERT INTO KfnCuentaCorrienteMovimiento (ccmId,ctaId,ccmFecha,ccmImporte  ,ccmSaldo,ccmDetalle,ccmNumeroTransaccion,ttrId) VALUES ( " + sec_number_ctacte_mov.ToString() + "," + ctaId.ToString() + ", GETDATE()" + "," + amount.ToString().Replace(",", ".") + "," + ctaSaldo.ToString().Replace(",", ".") + ", 'Deposito enviado desde API-Movilway por el Monto: " + amount.ToString().Replace(",", ".") + "'" + "," + sec_number_trans + ", 1000)";
                    sentencia = "INSERT INTO KfnCuentaCorrienteMovimiento (ccmId,ctaId,ccmFecha,ccmImporte  ,ccmSaldo,ccmDetalle,ccmNumeroTransaccion,ttrId) VALUES ( @ccmId,@ctaId, GETDATE(),@ccmImporte,@ccmSaldo, @ccmDetalle,@ccmNumeroTransaccion, @ttrId)";
                             
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ccmId", sec_number_ctacte_mov);
                    mySqlCommand.Parameters.AddWithValue("@ctaId", ctaId);
                    mySqlCommand.Parameters.AddWithValue("@ccmImporte", amount);
                    mySqlCommand.Parameters.AddWithValue("@ccmSaldo", ctaSaldo);
                    mySqlCommand.Parameters.AddWithValue("@ccmDetalle",String.Concat( "Deposito enviado desde API-Movilway por el Monto: " , amount.ToString(CultureInfo.InvariantCulture)));
                    mySqlCommand.Parameters.AddWithValue("@ccmNumeroTransaccion", sec_number_trans);
                    mySqlCommand.Parameters.AddWithValue("@ttrId", 1000);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = "09";
                    Message = "ERROR AL CREAR CUENTA CORRIENTE MOVIMIENTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                Response_Code = "10";
                try
                {
                    //sentencia = "UPDATE KfnDeposito SET  depEstado = 'AU', depComentarioProcesamiento = 'Deposito enviado desde API-Movilway por el Monto: " + amount.ToString().Replace(",", ".") + "'",ccmNumeroTransaccion =" + sec_number_trans + ", processingUsrId =" + usr_id + ", depComentario= " + "'" + Comentario + "'" + " WHERE  depId =" + depId;
                    sentencia = "UPDATE  KfnDeposito SET  depEstado = @newdepestado, depComentarioProcesamiento = @comentarioprocesamiento  ,ccmNumeroTransaccion = @sec_number_trans , processingUsrId = @usr_id , depComentario= @Comentario  WHERE  depId=@depId";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@comentarioprocesamiento", "Deposito enviado desde API-Movilway por el Monto: " + amount.ToString(CultureInfo.InvariantCulture));
                    mySqlCommand.Parameters.AddWithValue("@sec_number_trans", sec_number_trans);
                    mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@Comentario", Comentario);
                    mySqlCommand.Parameters.AddWithValue("@depId", depId);
                    mySqlCommand.Parameters.AddWithValue("@newdepestado", "AU");
                    if (mySqlCommand.ExecuteNonQuery() == 0) { 
                        Message = "NO SE PUDO REALIZAR OPERACION DE DEPOSITO";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
               
                    Message = "ERROR AL ACTUALIZAR CAMBIAR DE ESTADO DEPOSITO";

                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }


                Response_Code = "11";
                try
                {
                    // mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    mySqlCommand.CommandText = "UPDATE  secuencia SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        //throw new Exception("NO SE PUDO CAMBIAR LA SECUENCIA DE AUDITORIA");
                        Message = "NO SE PUDO ACTUALIZAR LA SECUENCIA";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR LA SECUENCIA DE AUDITORIA";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                
                try
                {
				    // mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    sec_number_audit = (int)mySqlCommand.ExecuteScalar();

                }
                catch (Exception ex)
                {
                    Response_Code = "12";
                    Message = "ERROR AL SELECCIONAR LA SECUENCIA AUDITORIA";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                try
                {
				    //sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio) VALUES ( " + sec_number_audit + "," + usr_id + ", null, 'Deposito enviado desde API-Movilway por el monto " + amount + "'" + ", GETDATE(), " + depId.ToString() + ", 'FINANCIERO', 'AUTORIZACION')";
                    sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio) VALUES ( @sec_number_audit ,@usr_id , null, @traComentario ,GETDATE(), @depId,@traDominio,@traSubdominio)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@sec_number_audit", sec_number_audit);
                    mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@traComentario", "Deposito enviado desde API-Movilway por el monto " + amount );
                    mySqlCommand.Parameters.AddWithValue("@depId", depId);
                    mySqlCommand.Parameters.AddWithValue("@traDominio", "FINANCIERO");
                    mySqlCommand.Parameters.AddWithValue("@traSubdominio", "AUTORIZACION");
                    mySqlCommand.ExecuteNonQuery();
                
                }
                catch (Exception ex)
                {
                    Response_Code = "13";
                    Message = "ERROR AL CREAR  KCRTRANSACCION ";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                // Fin Registro de Deposito



                //Inicio Proceso de Acreditacion 
                Response_Code = "14";
                try
                {
                    //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                    mySqlCommand.CommandText = "UPDATE  [secuencia] WITH(ROWLOCK) SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR LA SECUENCIA DE SOLICITUDPRODUCTO";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;

                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR LA SECUENCIA DE SOLICITUDPRODUCTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                try
                {
                    //mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                    mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WITH(NOLOCK) WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                    ult_sol_pro = (int)mySqlCommand.ExecuteScalar();
                    //repeticion
                    //sec_number = (int)ult_sol_pro;
                }
                catch (Exception ex)
                {
                    Response_Code = "15";
                    Message = "ERROR AL SELECCIONAL LA SECUENCIA SOLICITUDPRODUCTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                //  SE QUITA POR REPETICION la variable sec_number  no se utilizaba
                /*
                try
                {
                    mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WITH(NOLOCK) WHERE sec_objectName =  @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                    sec_number = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "16";
                    Message = "ERROR AL SELECCIONAR LA SECUENCIA SOLICITUDPRODUCTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                 * */
              
                /* inserto una solicitud y la coloco en estado pendiente */
                try 
                { 
				     /*"
                     //cambio
                     INSERT INTO [KlgSolicitudProducto] WITH(ROWLOCK) (sprId,usrId,ageIdSolicitante,ageIdDestinatario,prvIdDestinatario,sprEstado,sprFecha,sprFechaAprobacion,sprImporteSolicitud,sltId,ageIdBodegaOrigen,ageIdBodegaDestino) VALUES (@ult_sol_pro,@usr_id,@IdAgenteSolicitante , @age_id , NULL,'AU','" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ",'" + fecha_aprobacion.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ",@amount ,202, @IdBodegaOrigen  , @IdBodegaDestino)";*/

                    sentencia = "INSERT INTO [KlgSolicitudProducto] WITH(ROWLOCK) (sprId,usrId,ageIdSolicitante,ageIdDestinatario,prvIdDestinatario,sprEstado,sprFecha,sprFechaAprobacion,sprImporteSolicitud,sltId,ageIdBodegaOrigen,ageIdBodegaDestino) VALUES (@ult_sol_pro,@usr_id,@IdAgenteSolicitante , @age_id , @prvIdDestinatario,@sprEstado,@date, @fecha_aprobacion,@amount ,@sltId, @IdBodegaOrigen  , @IdBodegaDestino)";                
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteSolicitante", IdAgenteSolicitante);
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    mySqlCommand.Parameters.AddWithValue("@prvIdDestinatario", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@sprEstado", "AU");
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@sltId", 202);
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaOrigen", IdBodegaOrigen);
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaDestino", IdBodegaDestino);
                    mySqlCommand.Parameters.AddWithValue("@date", date);
                    mySqlCommand.Parameters.AddWithValue("@fecha_aprobacion",fecha_aprobacion);
             
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = "17";
                    Message = "ERROR AL CREAR KLGSOLICITUDPRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                Response_Code = "18";
                //Se actualiza el campo usrIdInitiator en caso de existir
                try
                {
                    //sentencia = "UPDATE [KlgSolicitudProducto] WITH(ROWLOCK) SET usrIdInitiator = " + usr_id.ToString() + " WHERE sprId =" + ult_sol_pro;
                    sentencia = "UPDATE  [KlgSolicitudProducto] WITH(ROWLOCK) SET usrIdInitiator = @usr_id WHERE sprId = @ult_sol_pro";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR EL CAMPO usrIdInitiator";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                      //  tran.Rollback();
                      //  return false;

                    }
                  
                
                }
                catch (Exception ex)
                {
                  
                    Message = "ERROR AL MODIFICAR EL USUARIO INITIATOR EN SOLICITUD PRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                  //  tran.Rollback();
                  //  return false;
                }
             
                try
                {

                       //sentencia = "INSERT INTO [KlgSolicitudProductoItem] WITH(ROWLOCK) (sprId,prdId,spiCantidadSolicitada,spiCantidadAutorizada,spiPrecioUnitario,spiEstado)  VALUES (" +ult_sol_pro + ",0," + amount + "," + amount + ", 1.0000,'PE')";
                   sentencia = "INSERT INTO [KlgSolicitudProductoItem] WITH(ROWLOCK) (sprId,prdId,spiCantidadSolicitada,spiCantidadAutorizada,spiPrecioUnitario,spiEstado)  VALUES (@ult_sol_pro ,@prdId, @amount , @amount , @spiPrecioUnitario,@spiEstado)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@spiPrecioUnitario", 1m);//"1.0000");
                    mySqlCommand.Parameters.AddWithValue("@spiEstado", "PE");
                    mySqlCommand.ExecuteNonQuery();

                 }
                 catch (Exception ex)
                 {
                    Response_Code = "19";
                    Message = "ERROR AL CREAR KLGSOLICITUDPRODUCTOITEM";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                 }

               Response_Code = "20";
                try
                {
				    //sentencia = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK) SET spiEstado = 'AU', spiCantidadAutorizada = " + amount +" WHERE sprId =" + ult_sol_pro + " AND prdId = 0 AND spiEstado = 'PE'";
                    sentencia = "UPDATE  [KlgSolicitudProductoItem] WITH(ROWLOCK) SET spiEstado = @newestado, spiCantidadAutorizada = @amount WHERE sprId = @ult_sol_pro  AND prdId = @prdId AND spiEstado = @oldestado";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@newestado", "AU");
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    mySqlCommand.Parameters.AddWithValue("@oldestado", "PE");
                    if (mySqlCommand.ExecuteNonQuery() == 0) {
                        Message = "NO SE PUDO MODIFICAR LA SOLICITUD PRODUCTO IT";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                 }
                catch (Exception ex)
                {
                   
                    Message = "ERROR AL ACTUALIZAR LA SOLICITUD PRODUCTO IT";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                Response_Code = "21";
                try
                {
               
                   mySqlCommand.CommandText = "UPDATE  [secuencia] WITH(ROWLOCK) SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue ";
					mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue","ENVIOPRODUCTO");

                    if(mySqlCommand.ExecuteNonQuery()== 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR SECUENCIA ENVIOPRODUCTO";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
             
                    Message = "ERROR AL ACTUALIZAR SECUENCIA ENVIOPRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                try
                {
                    mySqlCommand.CommandText = "SELECT  sec_number FROM [secuencia] WITH(NOLOCK) WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "ENVIOPRODUCTO");
                    ult_env_pro = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "22";
                    Message = "ERROR AL SELECCIONAR LA SECUENCIA ENVIOPRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;

       
                }

               
                try
                {
                /*sentencia = "INSERT INTO [KlgEnvio] WITH(ROWLOCK) (envId,envEstado,envFechaEnvio,envFechaRecepcion,envNumeroRemito,envNumeroFactura,envObservaciones,ageIdBodegaOrigen,ageIdBodegaDestino) " +
                            "VALUES (" +ult_env_pro + ",'DE','" + date.ToString("yyyy-MM-dd HH:mm:ss") + "',NULL,'','" + depId + "','Envio de productos Virtuales en linea'," + IdBodegaOrigen + "," + IdBodegaDestino + ")";*/

                    sentencia = "INSERT INTO [KlgEnvio] WITH(ROWLOCK) (envId,envEstado,envFechaEnvio,envFechaRecepcion,envNumeroRemito,envNumeroFactura,envObservaciones,ageIdBodegaOrigen,ageIdBodegaDestino)  VALUES (@ult_env_pro ,@envEstado,@date,@envFechaRecepcion,@envNumeroRemito,@depId,@envObservaciones, @IdBodegaOrigen ,@IdBodegaDestino )";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@envEstado", "DE");
                    mySqlCommand.Parameters.AddWithValue("@date", date);
                    mySqlCommand.Parameters.AddWithValue("@envFechaRecepcion", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@envNumeroRemito","");
                    mySqlCommand.Parameters.AddWithValue("@depId", depId);
                    mySqlCommand.Parameters.AddWithValue("@envObservaciones", "Envio de productos Virtuales en linea");
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaOrigen", IdBodegaOrigen);
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaDestino", IdBodegaDestino);
                  
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = "23";
                    Message = "ERROR AL CREAR KLGENVIO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
              
                }


                try
                {
				   /* sentencia = "INSERT INTO [KlgSolicitudProductoEnvio] WITH(ROWLOCK) (sprId,envId)  " +
                            "VALUES (" + ult_sol_pro + "," + ult_env_pro + ")";*/
                    sentencia = "INSERT INTO [KlgSolicitudProductoEnvio] WITH(ROWLOCK) (sprId,envId) VALUES (@ult_sol_pro ,@ult_env_pro )";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = "24";
                    Message = "ERROR AL CREAR KLGSOLICITUDPRODUCTOENVIO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

              
                try
                {
                   /*sentencia = "INSERT INTO [KlgEnvioItem] WITH(ROWLOCK) (envId,prdId,eitCantidad,eitEstado)" +
                                "VALUES (" + ult_env_pro + "," + 0 + "," + amount.ToString().Replace(",", ".") + ",'DE')";*/
                    sentencia = "INSERT INTO [KlgEnvioItem] WITH(ROWLOCK) (envId,prdId,eitCantidad,eitEstado) VALUES (@ult_env_pro ,@prdId ,@amount,@eitEstado)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    mySqlCommand.Parameters.AddWithValue("@amount",amount);
                    mySqlCommand.Parameters.AddWithValue("@eitEstado", "DE");
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = "25";
                    Message = "ERROR AL CREAR KLGENVIOITEM";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;

                }

                Response_Code = "26";
                try
                {
				   // mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = 'DE' WHERE sprId =" + ult_sol_pro + " AND prdId = 0 AND spiEstado = 'AU'";
                    mySqlCommand.CommandText = "UPDATE  [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = @newestado WHERE sprId = @ult_sol_pro  AND prdId = @prdId AND spiEstado = @oldestado";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@newestado", "DE");
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    mySqlCommand.Parameters.AddWithValue("@oldestado", "AU");

                    if (mySqlCommand.ExecuteNonQuery() == 0) { 
                        Message ="NO SE PUDO ACTUALIZAR LA SOLICITUD PRODUCTO IT";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }

                }
                catch (Exception ex)
                {
                
                    Message = "ERROR AL ACTUALIZAR LA SOLICITUD PRODUCTO IT";
                   _m =  String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                   logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }


                try 
                { 
				//mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId =" + IdAgenteSolicitante + " AND prdId IN (0)";
                    mySqlCommand.CommandText = "SELECT   stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId = @IdAgenteSolicitante  AND prdId = @prdId";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteSolicitante", IdAgenteSolicitante);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    StockIdOrigen = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "27";
                    Message = "ERROR AL SELECCIONAR KLGSTOCKAGENTEPRODUCTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                Response_Code = "28";

                try
                {
				// mySqlCommand.CommandText = " UPDATE [KlgStockAgenteProducto] WITH(ROWLOCK) SET  stkCantidad = stkCantidad + - " + amount + " WHERE  stkId = " + StockIdOrigen + " AND ageId = " + IdAgenteSolicitante + "and stkCantidad + - " + amount + ">= 0";
                    //resta en el stock
                    mySqlCommand.CommandText = " UPDATE [KlgStockAgenteProducto] WITH(ROWLOCK) SET  stkCantidad = stkCantidad + -  @amount  WHERE  stkId = @StockIdOrigen AND ageId = @IdAgenteSolicitante and stkCantidad + -@amount >= 0";
                    mySqlCommand.Parameters.Clear();
					mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@StockIdOrigen", StockIdOrigen);
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteSolicitante", IdAgenteSolicitante);

                    if (mySqlCommand.ExecuteNonQuery() == 0) { 
                       Message =  "NO SE PUDO MODIFICAR EL STOCK AGENTE PRODUCTO";
                       _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                       logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                       tran.Rollback();
                       return false;

                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL MODIFICAR EL STOCK AGENTE PRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                Response_Code = "29";
                try
                {
				    // mySqlCommand.CommandText = " UPDATE [KlgEnvio] WITH(ROWLOCK) SET envEstado = 'RE', envFechaRecepcion = GETDATE() WHERE envId = " + ult_env_pro + " AND envEstado = 'DE'";
                    mySqlCommand.CommandText = " UPDATE [KlgEnvio] WITH(ROWLOCK) SET envEstado = @newestado, envFechaRecepcion = GETDATE() WHERE envId =  @ult_env_pro  AND envEstado = @oldestado";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@newestado", "RE");
                    mySqlCommand.Parameters.AddWithValue("@oldestado", "DE");
                    if (mySqlCommand.ExecuteNonQuery() == 0) {
                        Message = "NO SE PUDO ACTUALIZAR KLGENVIO";
                       _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                       logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                       tran.Rollback();
                       return false;
                    }
                 }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR KLGENVIO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                Response_Code = "30";
                try
                {
				    //mySqlCommand.CommandText = "UPDATE [KlgEnvioItem] WITH(ROWLOCK) SET eitEstado = 'RE'  WHERE envId = " + ult_env_pro + " AND prdId = 0 AND eitEstado = 'DE'";
                    mySqlCommand.CommandText = "UPDATE  [KlgEnvioItem] WITH(ROWLOCK) SET eitEstado = @newestado  WHERE envId = @ult_env_pro  AND prdId = @prdId AND eitEstado = @oldestado";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@newestado", "RE");
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    mySqlCommand.Parameters.AddWithValue("@oldestado", "DE");
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR KLGENVIOITEM";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                     
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR KLGENVIOITEM";
                   // logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                Response_Code = "31";
                try
                {
				    //mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = 'RE'  WHERE sprId = " + ult_sol_pro + "  AND prdId = 0";
                    mySqlCommand.CommandText = "UPDATE  [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = @newestado  WHERE sprId =  @ult_sol_pro   AND prdId = @prdId";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@newestado", "RE");
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    if (mySqlCommand.ExecuteNonQuery() == 0) {
                        Message =  "NO SE PUDO ACTUALIZAR KLGSOLICITUDPRODUCTOITEM";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR KLGSOLICITUDPRODUCTOITEM";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                try
                {
				    //mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId =" + age_id + " AND prdId IN (0)";
                    mySqlCommand.CommandText = "SELECT   stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId=@age_id  AND prdId  = @prdId";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    StockIdDestino = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "32";
                    Message = "";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                Response_Code = "33";
                try
                {
				    //mySqlCommand.CommandText = "UPDATE KlgStockAgenteProducto SET  stkCantidad = stkCantidad + " + amount + "  WHERE  stkId =" + StockIdDestino + " AND ageId =" + age_id + " and stkCantidad + " + amount + ">= 0 ";
                    //TODO DUDA SI EL AMOUNT ES NEGATIVO
                    mySqlCommand.CommandText = "UPDATE  KlgStockAgenteProducto SET  stkCantidad = stkCantidad + @amount   WHERE  stkId = @StockIdDestino  AND ageId = @age_id and stkCantidad + @amount >= 0 ";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@StockIdDestino", StockIdDestino);
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
					mySqlCommand.Parameters.AddWithValue("@amount",amount);
                    if (mySqlCommand.ExecuteNonQuery() == 0) { 
                       Message = "NO SE PUDO ACTUALIZAR CANTIDAD DE PRODUCTO PARA EL AGENTE DESTINO";
                       _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                       logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                       tran.Rollback();
                       return false;
                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR CANTIDAD DE PRODUCTO PARA EL AGENTE DESTINO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                Response_Code = "34";
                try 
                { 
				    //mySqlCommand.CommandText = " UPDATE KlgSolicitudProducto WITH(ROWLOCK) SET sprEstado = 'CE' WHERE sprId = " + ult_sol_pro + "  AND sprEstado = 'AU' AND ( ageIdSolicitante =" + age_id.ToString() + " OR ageIdDestinatario =" + age_id.ToString() + ")";
                    //CAMBIO CON RESPECTO A LA CREACION DE LA SOLICITUD DEL PRODUCTO SE QUITA UNA CONDICION EN EL PRODUCTO
                    mySqlCommand.CommandText = " UPDATE KlgSolicitudProducto WITH(ROWLOCK) SET sprEstado = @newestado WHERE sprId = @ult_sol_pro   AND sprEstado = @oldestado AND (ageIdDestinatario = @age_id)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@newestado", "CE");
                    mySqlCommand.Parameters.AddWithValue("@oldestado", "AU");
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);

                    if (mySqlCommand.ExecuteNonQuery() == 0) {
                        Message = "NO SE PUDO ACTUALIZAR KLGSOLICITUDPRODUCTO";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR KLGSOLICITUDPRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { ult_sol_pro = ult_sol_pro, age_id = age_id}));
                    tran.Rollback();
                    return false;
                }

                //Fin de proceso de Acreditacion  




                // {Inicio el proceso para registar en cta cte

                Response_Code = "35";
                try
                {
                    mySqlCommand.CommandText = "UPDATE  secuencia SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue" ,"AUDITORIA");
                       if (mySqlCommand.ExecuteNonQuery() == 0) { 
                            Message="NO SE PUDO ACTUALIZAR SECUENCIA DE AUDITORIA";
                            _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                            tran.Rollback();
                            return false;
                       }
                }
                catch (Exception ex)
                {
                 
                    Message = "ERROR AL ACTUALIZAR SECUENCIA DE AUDITORIA";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                try
                {
                    mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    sec_number_auditoria = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "36";
                    Message = "ERROR AL SELECCIONAR LA SECUENCIA AUDITORIA";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                try
                {
				    /* sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                            "VALUES ( " + sec_number_auditoria + ", " + usr_id + ", null, 'Despacho de productos nro." + ult_env_pro + "'" + " , GETDATE()," + ult_env_pro + ",'DISTRIBUCION', 'ENVIOPRODUCTO')";*/
                    sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                                "VALUES ( @sec_number_auditoria , @usr_id, @usrIdSuperior, @traComentario, GETDATE(),@ult_env_pro ,@traDominio, @traSubdominio)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@sec_number_auditoria", sec_number_auditoria);
                    mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@usrIdSuperior", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@traComentario", "Despacho de productos nro." + ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@traDominio", "DISTRIBUCION");
                    mySqlCommand.Parameters.AddWithValue("@traSubdominio", "ENVIOPRODUCTO");

					
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = "37";
                    Message = "ERROR AL CREAR KCRTRANSACCION";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

              
                Response_Code = "38";
                try
                {
                    // --Volvemos a leer el saldo de la cta cte. 
                    //POR QUE SE HABIA AHUMENTADO EL SALADO

                    // mySqlCommand.CommandText = "SELECT   ctaId   FROM KfnCuentaCorriente  WITH (NOLOCK) WHERE ageId =" +age_id;
                    //DUDA REPETIR LECTIRA POR CONCURRENCIA VALIDAR CUENTACORRIENTE AGENTE (SE VALIDA NO HAY AGENTES CON MAS DE UNA CUENTA )
                    mySqlCommand.CommandText = "SELECT   ctaId ,ctaSaldo  FROM KfnCuentaCorriente  WITH (NOLOCK) WHERE ageId = @age_id";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id",age_id);
                    using (reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            ctaId = (decimal)reader["ctaId"];//mySqlCommand.ExecuteScalar();
                            ctaSaldo = (decimal)reader["ctaSaldo"];//mySqlCommand.ExecuteScalar();
                        }
                        else
                        {
                            Message = "NO SE PUDO ACTUALIZAR EL ESTADO DE LA SOLICITUD DE PRODUCTO";
                            _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                            tran.Rollback();
                            return false;
                        }

                    }
                }
                catch (Exception ex)
                {
                   
                    Message = "ERROR AL SELECCIONAR DATOS DE LA CUENTA CORRIENTE";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                /*
                try
                {
				   //mySqlCommand.CommandText = "SELECT  ctaSaldo   FROM KfnCuentaCorriente  WITH (NOLOCK) WHERE ageId =" + age_id;
                    mySqlCommand.CommandText = "SELECT   ctaSaldo   FROM KfnCuentaCorriente  WITH (NOLOCK) WHERE ageId = @age_id";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id",age_id);
                    ctaSaldo = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "39";
                    Message = "";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                 * */
                Response_Code = "40";
                try
                {
                    mySqlCommand.CommandText = "UPDATE  secuencia SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                    if (mySqlCommand.ExecuteNonQuery() == 0) {
                       Message = "NO SE PUDO ACTUALIZAR SECUENCIA DE TRANSACCION";
                       _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                       logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                       tran.Rollback();
                       return false;
                  
                    }
                }
                catch (Exception ex)
                {
                 
                    Message = "ERROR AL ACTUALIZAR LA SECUENCIA DE TRANSACCION";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                try
                {
                    mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                    sec_number_aut = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "41";
                    Message = "ERROR SELECCIONAR LA SECUENCIA DE TRANSACCION";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                Response_Code = "42";
                try
                {
				    //mySqlCommand.CommandText = "UPDATE KfnCuentaCorriente WITH(ROWLOCK) SET ctaSaldo = ctaSaldo + - " + amount + " WHERE ctaId =" + ctaId + " AND ageId = " + age_id + " AND ctaSaldo =" + ctaSaldo + " AND ( (ctaSaldo + - " + amount + " >= 0) OR (ABS(ctaSaldo + - " + amount + ") <= 600) )";
                    //mySqlCommand.CommandText = "UPDATE  KfnCuentaCorriente WITH(ROWLOCK) SET ctaSaldo = ctaSaldo + -@amount  WHERE ctaId = @ctaId  AND ageId = @age_id AND ctaSaldo = @ctaSaldo  AND ( (ctaSaldo + - @amount  >= 0) OR (ABS(ctaSaldo + -@amount ) <= 600) )";

                    mySqlCommand.CommandText = "UPDATE KfnCuentaCorriente WITH(ROWLOCK) SET ctaSaldo = ctaSaldo + - @amount  WHERE ctaId = @ctaId  AND ageId = @age_id AND ctaSaldo = @ctaSaldo";

                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@ctaId", ctaId);
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    mySqlCommand.Parameters.AddWithValue("@ctaSaldo", ctaSaldo);

                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR KFNCUENTACORRIENTE";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                  
                    }
                }
                catch (Exception ex)
                {
                    Response_Code = "42";
                    Message = "ERROR AL ACTUALIZAR KFNCUENTACORRIENTE";
                  //  logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                Response_Code = "43";
                try
                {
                    mySqlCommand.CommandText = "UPDATE  secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =@paramValue ";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");

                    if(mySqlCommand.ExecuteNonQuery()== 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR LA SECUENCIA CTACTEMOVIMIENTO";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                
                    Message = "ERROR AL ACTUALIZAR LA SECUENCIA CTACTEMOVIMIENTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                
                try
                {
                    //        mySqlCommand.CommandText = "SELECT   sec_numbercta_movi = sec_number FROM secuencia WHERE sec_objectName =  @paramValue";
                    mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue ";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                    sec_numbercta_movi = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "44";
                    Message = "ERROR AL SELECCIONAR ";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                try
                {
                    /*sentencia = "INSERT INTO KfnCuentaCorrienteMovimiento (ccmId,ctaId,ccmFecha,ccmImporte  ,ccmSaldo,ccmDetalle,ccmNumeroTransaccion,ttrId) " +
                                " VALUES ( " + sec_numbercta_movi + "," + ctaId + "," + "'" + fecha_aprobacion.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + -amount + "," + ctaSaldo.ToString() + ",'Asignacion de productos No.: " + ult_sol_pro.ToString() + "'" + "," + sec_number_aut.ToString() + ", 2000)";*/
                    sentencia = "INSERT INTO KfnCuentaCorrienteMovimiento (ccmId,ctaId,ccmFecha,ccmImporte  ,ccmSaldo,ccmDetalle,ccmNumeroTransaccion,ttrId) VALUES ( @sec_numbercta_movi , @ctaId , @fecha_aprobacion ,@amount , @ctaSaldo ,@detalle, @sec_number_aut , @ttrId)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@sec_numbercta_movi", sec_numbercta_movi);
                    mySqlCommand.Parameters.AddWithValue("@ctaId", ctaId);
                    mySqlCommand.Parameters.AddWithValue("@ctaSaldo", ctaSaldo);
                    mySqlCommand.Parameters.AddWithValue("@sec_number_aut", sec_number_aut);
                    mySqlCommand.Parameters.AddWithValue("@amount", -amount);
                    mySqlCommand.Parameters.AddWithValue("@detalle", "Asignacion de productos No.: " + ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@fecha_aprobacion",  fecha_aprobacion);
                    mySqlCommand.Parameters.AddWithValue("@ttrId", 2000);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = "45";
                    Message = "ERRRO AL CREAR KFNCUENTACORRIENTEMOVIMIENTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }


                Response_Code = "46";
                try
                {
                    mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue ";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    if (mySqlCommand.ExecuteNonQuery() == 0) {
                       Message = "NO SE PUDO ACTUALIZAR LA SECUENCIA AUDITORIA";
                       _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                       logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                       tran.Rollback();
                       return false;
                    }
                }
                catch (Exception ex)
                {
                 
                    Message = "ERROR AL ACTUALIZAR LA SECUENCIA AUDITORIA";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                

                try
                {
                    mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName =  @paramValue ";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    sec_number_auditoria = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "47";
                    Message = "ERROR AL SELECCIONAR LA SECUENCIA AUDITORIA";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                try
                {
				/* sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                            "VALUES (" + sec_number_auditoria + "," + usr_id + ", null, 'Asignacion de productos',  GETDATE()," + ult_sol_pro + ", 'DISTRIBUCION', 'SOLICITUDPRODUCTO')";*/
                    sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                                "VALUES (@sec_number_auditoria, @usr_id,@usrIdSuperior,@traComentario, GETDATE(),@ult_sol_pro,@traDominio,@traSubdominio)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@sec_number_auditoria", sec_number_auditoria);
                    mySqlCommand.Parameters.AddWithValue("@usrIdSuperior", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@traComentario", "Asignacion de productos");
                    mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@traDominio","DISTRIBUCION");
                    mySqlCommand.Parameters.AddWithValue("@traSubdominio", "SOLICITUDPRODUCTO");
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = "48";
                    Message = "";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                // }Fin del proceso para registar en cta cte



                Response_Code = "00";
                Message = "TRANSACCION OK";
                if (committransaction)
                tran.Commit();
                return true;

            }
            catch (Exception ex)
            {
                Response_Code = "49";
                Message = "ERROR AL TRANSFERIR SALDO";

                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));

                return false;

            }
            finally { if (committransaction) mySqlConnection.Close(); }
        }

        public bool AcreditacionCredito(decimal age_id, decimal amount, int usr_id, string reference_number, DateTime date, string cuenta, string condeposito, ref string Response_Code, ref string Message, DateTime fecha_aprobacion,SqlConnection _mySqlConnection = null, SqlTransaction _tran = null)
        {


            string age_estado = string.Empty;
            string cubNumero = string.Empty;
            decimal ctaSaldo;
            decimal ctaId;
            decimal IdAgenteSolicitante = 0;
            decimal IdBodegaOrigen = 0;
            decimal IdBodegaDestino = 0;
            decimal ult_sol_pro = 0m;
            int sec_number;
            decimal ult_env_pro;
            decimal StockIdOrigen;
            decimal StockIdDestino;
            decimal sec_number_auditoria;
            int sec_number_aut;
            int sec_numbercta_movi;
            string AgenteNombre = string.Empty;
            string sentencia = string.Empty;
            string age_comisionadeposito = string.Empty;
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;

            //variable auxiliar para concatener el error del sistema
            string _m = null;

            SqlConnection mySqlConnection = null;
            SqlTransaction tran = null;
            SqlCommand mySqlCommand = new SqlCommand();
            SqlDataReader reader = null;
            //indica si la transaccion debe hacer commit
            bool committransaction = false;
            try
            {
                if (_mySqlConnection == null){

                    committransaction = true;
                    mySqlConnection = new SqlConnection(strConnString);
                    mySqlConnection.Open();
                    tran = mySqlConnection.BeginTransaction();
                }
                else if (_mySqlConnection != null && _tran != null) {

                    mySqlConnection = _mySqlConnection;
                    tran = _tran;
                }
                else
                {
                    throw new Exception("Parámetros de conexión incorrectos");
                }

                mySqlCommand.Transaction = tran;
                mySqlCommand.Connection = mySqlConnection;


                //Inicio Proceso de Acreditacion 
                try
                {
                    //mySqlCommand.CommandText = "SELECT age_id_sup  FROM [Agente] WITH(NOLOCK)   WHERE age_id = " + age_id;
                    mySqlCommand.CommandText = "SELECT age_id_sup  FROM [Agente] WITH(NOLOCK)   WHERE age_id = @age_id";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    IdAgenteSolicitante = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "01";
                    Message = "ERROR AL SELECCIONAR EL AGENTE PADRE";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                IdBodegaOrigen = IdAgenteSolicitante;
                IdBodegaDestino = age_id;

                Response_Code = "02";
                try
                {
                
                  //  mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                        mySqlCommand.CommandText = "UPDATE [secuencia] WITH(ROWLOCK) SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");

                    if (mySqlCommand.ExecuteNonQuery() == 0) { 
                        Message = "NO SE PUDO ACTUALIZAR SECUENCIA SOLICITUD PRODUCTO";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                  
                    }
                }
                catch (Exception ex)
                {
                  
                    Message = "ERROR AL ACTUALIZAR SECUENCIA SOLICITUD PRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
              
                }

                try
                {
                    mySqlCommand.CommandText = "SELECT sec_number FROM [secuencia] WITH(NOLOCK) WHERE sec_objectName = @paramValue";// 'SOLICITUDPRODUCTO'";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                    ult_sol_pro = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "03";
                    Message = "ERROR AL SELECCIONAR LA SECUENCIA SOLICITUDPRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                }

                //TODO DEJO LA SELECCION DEL LA SECUENCIA POR LA CONCURRENCIA
                try
                {
                   // mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                    mySqlCommand.CommandText = "SELECT sec_number FROM [secuencia] WITH(NOLOCK) WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "SOLICITUDPRODUCTO");
                    sec_number = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "04";
                    Message = "ERROR AL SELECCIONAR LA SECUENCIA SOLICITUDPRODUCTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                /* inserto una solicitud y la coloco en estado pendiente */

                //se podria hacer solo una
                try
                {
                    /*sentencia = "INSERT INTO [KlgSolicitudProducto] WITH(ROWLOCK) (sprId,usrId,ageIdSolicitante,ageIdDestinatario,prvIdDestinatario,sprEstado,sprFecha,sprFechaAprobacion,sprImporteSolicitud,sltId,ageIdBodegaOrigen,ageIdBodegaDestino) " +
                                "VALUES (" + ult_sol_pro.ToString() + "," + usr_id.ToString() + "," + IdAgenteSolicitante.ToString() + "," + age_id.ToString() + ", NULL,'AU','" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ",'" + fecha_aprobacion.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + amount.ToString().Replace(",", ".") + ",202," + IdBodegaOrigen.ToString() + "," + IdBodegaDestino.ToString() + ")";*/

                    sentencia = "INSERT INTO [KlgSolicitudProducto] WITH(ROWLOCK) (sprId,usrId,ageIdSolicitante,ageIdDestinatario,prvIdDestinatario,sprEstado,sprFecha,sprFechaAprobacion,sprImporteSolicitud,sltId,ageIdBodegaOrigen,ageIdBodegaDestino) VALUES ( @ult_sol_pro, @usr_id,@IdAgenteSolicitante,@age_id, @prvIdDestinatario,@sprEstado,@date,@fecha_aprobacion,@amount ,@sltId,@IdBodegaOrigen, @IdBodegaDestino)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteSolicitante", IdAgenteSolicitante);
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    mySqlCommand.Parameters.AddWithValue("@prvIdDestinatario", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@sprEstado", "AU");
                    mySqlCommand.Parameters.AddWithValue("@date", date);
                    mySqlCommand.Parameters.AddWithValue("@fecha_aprobacion",fecha_aprobacion);
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@sltId", 202);
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaOrigen", IdBodegaOrigen);
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaDestino", IdBodegaDestino);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = "05";
                    Message = "ERROR AL CREAR KLGSOLICITUDPRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                Response_Code = "06";
               
                try
                {
                    //Se actualiza el campo usrIdInitiator en caso de existir
					//sentencia = "UPDATE [KlgSolicitudProducto] WITH(ROWLOCK) SET usrIdInitiator = " + usr_id.ToString() + " WHERE sprId =" + ult_sol_pro;
                    sentencia = "UPDATE [KlgSolicitudProducto] WITH(ROWLOCK) SET usrIdInitiator = @usr_id WHERE sprId = @ult_sol_pro";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR KLGSOLICITUDPRODUCTO USRIDINITIATOR";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        //tran.Rollback();
                        //return false;
                    }
                }
                catch (Exception ex)
                {
                
                    Message = "ERROR AL ACTUALIZAR KLGSOLICITUDPRODUCTO";
                     _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                     logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                     //tran.Rollback();
                     //return false;
                }

                try
                {
                    //sentencia = "INSERT INTO [KlgSolicitudProductoItem] WITH(ROWLOCK) (sprId,prdId,spiCantidadSolicitada,spiCantidadAutorizada,spiPrecioUnitario,spiEstado) VALUES (" + ult_sol_pro + ",0," + amount + "," + amount + ", 1.0000,'PE')";
                    sentencia = string.Concat("INSERT INTO [KlgSolicitudProductoItem] WITH(ROWLOCK) (sprId,prdId,spiCantidadSolicitada,spiCantidadAutorizada,spiPrecioUnitario,spiEstado) " ,
                                "VALUES (@ult_sol_pro,@prdId,@amount,@amount,@spiPrecioUnitario,@spiEstado)");
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@spiPrecioUnitario", 1m);//"1.0000");
                    mySqlCommand.Parameters.AddWithValue("@spiEstado", "AU"); //"PE");
                    mySqlCommand.ExecuteNonQuery();
                 }
                catch (Exception ex)
                {
                    Response_Code = "07";
                    Message = "ERROR AL CREAR KLGSOLICITUDPRODUCTOIT";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                /*
                // SE QUITA Y SE INSERTA EN EL ESTADO AU (query anterior) Y spiCantidadAutorizada no tiene un cambio significativo
                try
                {
				   
					//sentencia = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK) SET spiEstado = 'AU', spiCantidadAutorizada = " + amount + "WHERE prId =" + ult_sol_pro + " AND prdId = 0 AND spiEstado = 'PE'";
					
                    sentencia = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK) SET spiEstado = 'AU', spiCantidadAutorizada =  @amount WHERE sprId = @ult_sol_pro  AND prdId = 0 AND spiEstado = 'PE'";

                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    if( mySqlCommand.ExecuteNonQuery()== 0)
                       throw new Exception("NO SE PUDO ACTUALIZAR ESTADO Y CANTIDAD SOLICITUD PRODUCTO IT ");
                }
                catch (Exception ex)
                {
                    Response_Code = "08";
                    Message = string.Empty;
                  //  logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                   string _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                   logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { amount = amount, ult_sol_pro = ult_sol_pro }));
                    tran.Rollback();
                    return false;
                }
                */

                Response_Code = "09";
                try
                {
                    mySqlCommand.CommandText = "UPDATE [secuencia] WITH(ROWLOCK) SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";//'ENVIOPRODUCTO'";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "ENVIOPRODUCTO");
                    if (mySqlCommand.ExecuteNonQuery() == 0) { 
                      Message = "NO SE PUDO ACTUALIZAR SECUENCIA DE ENVIO PRODUCTO";
                      _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                      logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                      tran.Rollback();
                      return false;
                    }
                 }
                catch (Exception ex)
                {
                    Message = "ERROR AL ACTUALIZAR SECUENCIA DE ENVIO PRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                  
                }


                try
                {
                    mySqlCommand.CommandText = "SELECT sec_number FROM [secuencia] WITH(NOLOCK) WHERE sec_objectName = @paramValue ";//'ENVIOPRODUCTO'";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "ENVIOPRODUCTO");
                    ult_env_pro = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "10";
                    Message = "ERROR AL SELECCIONAR LA SECUENCIA";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                try
                {
                   /* sentencia = "INSERT INTO [KlgEnvio] WITH(ROWLOCK) (envId,envEstado,envFechaEnvio,envFechaRecepcion,envNumeroRemito,envNumeroFactura,envObservaciones,ageIdBodegaOrigen,ageIdBodegaDestino) " +
                                "VALUES (" + ult_env_pro + ",'DE','" + date.ToString("yyyy-MM-dd HH:mm:ss") + "',NULL,'','','Envio de productos Virtuales en linea'," + IdBodegaOrigen + "," + IdBodegaDestino + ")";
                    */
                    sentencia = "INSERT INTO [KlgEnvio] WITH(ROWLOCK) (envId,envEstado,envFechaEnvio,envFechaRecepcion,envNumeroRemito,envNumeroFactura,envObservaciones,ageIdBodegaOrigen,ageIdBodegaDestino) VALUES (@ult_env_pro ,@envEstado,@date,@envFechaRecepcion,@envNumeroRemito,@envNumeroFactura,@envObservaciones,@IdBodegaOrigen ,@IdBodegaDestino )";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@envEstado", "DE");
                    mySqlCommand.Parameters.AddWithValue("@date", date);
                    mySqlCommand.Parameters.AddWithValue("@envFechaRecepcion", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@envNumeroRemito",string.Empty);
                    mySqlCommand.Parameters.AddWithValue("@envNumeroFactura", string.Empty);
                    mySqlCommand.Parameters.AddWithValue("@envObservaciones", "Envio de productos Virtuales en linea");
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaOrigen", IdBodegaOrigen);
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaDestino", IdBodegaDestino);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = "11";
                    Message = "ERROR AL CREAR KLGENVIO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                try
                {
                    /*sentencia = "INSERT INTO [KlgSolicitudProductoEnvio] WITH(ROWLOCK) (sprId,envId)  " +
                            "VALUES (" + ult_sol_pro + "," + ult_env_pro + ")";*/
                    sentencia = string.Concat("INSERT INTO [KlgSolicitudProductoEnvio] WITH(ROWLOCK) (sprId,envId)  " ,
                                "VALUES (@ult_sol_pro , @ult_env_pro )");

                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Response_Code = "12";
                     Message = "ERROR AL CREAR KLGSOLICITUDPRODUCTOENVIO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }


                try
                {
                     /*  sentencia = "INSERT INTO [KlgEnvioItem] WITH(ROWLOCK) (envId,prdId,eitCantidad,eitEstado)" +
                            "VALUES (" + ult_env_pro + "," + 0 + "," + amount.ToString().Replace(",", ".") + ",'DE')";*/

                      sentencia = "INSERT INTO [KlgEnvioItem] WITH(ROWLOCK) (envId,prdId,eitCantidad,eitEstado) VALUES (@ult_env_pro , @prdId ,@amount ,@eitEstado)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@eitEstado", "DE");
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = "13";
                    Message = "ERROR AL CREAR KLGENVIOITEM";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                 
                }
                Response_Code = "14";
                try
                {
                    //mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = 'DE' WHERE sprId =" + ult_sol_pro + " AND prdId = 0 AND spiEstado = 'AU'";
                    mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = @newestado WHERE sprId = @ult_sol_pro AND prdId = @prdId AND spiEstado = @oldestado";
                    mySqlCommand.Parameters.Clear();
                 
                    mySqlCommand.Parameters.AddWithValue("@newestado", "DE");
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    mySqlCommand.Parameters.AddWithValue("@oldestado", "AU");
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                         Message = "NO SE PUDO ACTUALIZAR ESTADO SOLICITUD PRODUCTO IT";
                         _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                         logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                         tran.Rollback();
                         return false;
                    }
                }
                catch (Exception ex)
                {
                    
                    Message = "ERROR AL ACTUALIZAR ESTADO SOLICITUD PRODUCTO IT";
                  //  logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                     _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new {  ult_sol_pro = ult_sol_pro }));
                    tran.Rollback();
                    return false;
                }

                try
                {
                    //mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId =" + IdAgenteSolicitante + " AND prdId IN (0)";
                    mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId =@IdAgenteSolicitante AND prdId = @prdId";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteSolicitante", IdAgenteSolicitante);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    StockIdOrigen = (decimal)mySqlCommand.ExecuteScalar();

                }
                catch (Exception ex)
                {
                    Response_Code = "15";
                    Message = "ERROR AL SELECCIONAR DATOS DEL STOCK AGENTE PRODUCTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                Response_Code = "16";
                try
                {
                    // mySqlCommand.CommandText = " UPDATE [KlgStockAgenteProducto] WITH(ROWLOCK) SET  stkCantidad = stkCantidad + - " + amount + " WHERE  stkId = " + StockIdOrigen + " AND ageId = " + IdAgenteSolicitante + "and stkCantidad + - " + amount + ">= 0";
                    mySqlCommand.CommandText = " UPDATE [KlgStockAgenteProducto] WITH(ROWLOCK) SET  stkCantidad = stkCantidad + - @amount  WHERE  stkId =  @StockIdOrigen  AND ageId = @IdAgenteSolicitante and stkCantidad + - @amount >= 0";
                    mySqlCommand.Parameters.Clear();
                  
                    mySqlCommand.Parameters.AddWithValue("@StockIdOrigen", StockIdOrigen);
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteSolicitante", IdAgenteSolicitante);
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    if (mySqlCommand.ExecuteNonQuery() == 0) { 
                       Message ="NO SE PUDO ACTUALIZAR CANTIDAD STOCK AGENTE PRODUCTO";
                       _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                       logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                       tran.Rollback();
                       return false;

                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR CANTIDAD STOCK AGENTE PRODUCTO";
                 //   logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }


                Response_Code = "17";
                try
                {
				    //mySqlCommand.CommandText = " UPDATE [KlgEnvio] WITH(ROWLOCK) SET envEstado = 'RE', envFechaRecepcion = GETDATE() WHERE envId = " + ult_env_pro + " AND envEstado = 'DE'";
                    mySqlCommand.CommandText = " UPDATE [KlgEnvio] WITH(ROWLOCK) SET envEstado = @newestado, envFechaRecepcion = GETDATE() WHERE envId = @ult_env_pro AND envEstado = @oldestado";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@newestado", "RE");
                    mySqlCommand.Parameters.AddWithValue("@oldestado", "DE");
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR KLGENVIO";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                   
                     Message = "ERROR AL ACTUALIZAR KLGENVIO";
                     _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                     logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                     tran.Rollback();
                     return false;
                }


                Response_Code = "18";
                try
                {
                  //  mySqlCommand.CommandText = "UPDATE [KlgEnvioItem] WITH(ROWLOCK) SET eitEstado = 'RE'  WHERE envId = @ult_env_pro  AND prdId = 0 AND eitEstado = 'DE'";
                    mySqlCommand.CommandText = "UPDATE [KlgEnvioItem] WITH(ROWLOCK) SET eitEstado = @newestado  WHERE envId = @ult_env_pro  AND prdId = @prdId AND eitEstado = @oldestado";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@newestado", "RE");
                    mySqlCommand.Parameters.AddWithValue("@oldestado", "DE");
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR ESTADO DE KLGENVIOITEM";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR ESTADO DE KLGENVIOITEM";
                     _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                     logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                     tran.Rollback();
                     return false;
                }

                Response_Code = "19";
                try
                {

                    //mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = 'RE'  WHERE sprId = @ult_sol_pro   AND prdId = 0";
                    mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = @newestado  WHERE sprId = @ult_sol_pro   AND prdId = @prdId";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@newestado", "RE");
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    if (mySqlCommand.ExecuteNonQuery() == 0) { 
                        Message = "NO SE PUDO ACTUALIZAR ESTADO SOLICITUD PRODUCTO IT";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR ESTADO SOLICITUD PRODUCTO IT";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }


                try
                {
				    //
                    mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId =@age_id  AND prdId = @prdId";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    StockIdDestino = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                     Response_Code = "20";
                     Message = "ERROR AL SELECCIONAR DATOS DE KLGSTOCKAGENTEPRODUCTO";

                     _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                     logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                     tran.Rollback();
                     return false;
                }

                Response_Code = "21";
                try
                {

                     /* mySqlCommand.CommandText = "UPDATE KlgStockAgenteProducto SET  stkCantidad = stkCantidad + " + amount + "  WHERE  stkId =" + StockIdDestino + " AND ageId =" + age_id + " and stkCantidad + " + amount + ">= 0 ";*/
                     mySqlCommand.CommandText = "UPDATE KlgStockAgenteProducto SET  stkCantidad = stkCantidad + @amount   WHERE  stkId = @StockIdDestino  AND ageId = @age_id  and stkCantidad + @amount >= 0 ";
                     mySqlCommand.Parameters.Clear();
                     mySqlCommand.Parameters.AddWithValue("@amount", amount);
                     mySqlCommand.Parameters.AddWithValue("@StockIdDestino", StockIdDestino);
                     mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                     if (mySqlCommand.ExecuteNonQuery() == 0) 
                     { 
                        Message = "NO SE PUDO ACTUALIZAR AGENTE PROUCTO STOCK";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                     }
                }
                catch (Exception ex)
                {
                     Message = Message = "ERROR AL ACTUALIZAR AGENTE PROUCTO STOCK";
                     _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                     logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                     tran.Rollback();
                     return false;
                }


                Response_Code = "22";
                try
                {
                     /*mySqlCommand.CommandText = " UPDATE KlgSolicitudProducto WITH(ROWLOCK) SET sprEstado = 'CE' WHERE sprId = " + ult_sol_pro + "  AND sprEstado = 'AU' AND ( ageIdSolicitante =" + age_id.ToString() + " OR ageIdDestinatario =" + age_id.ToString() + ")";*/
                    //se quita el or
                    mySqlCommand.CommandText = " UPDATE KlgSolicitudProducto WITH(ROWLOCK) SET sprEstado = @newestado WHERE sprId = @ult_sol_pro   AND sprEstado =  @oldestado AND  (  ageIdDestinatario = @age_id )";
                     mySqlCommand.Parameters.Clear();
                     mySqlCommand.Parameters.AddWithValue("@newestado", "CE");
                     mySqlCommand.Parameters.AddWithValue("@oldestado", "AU");
                     mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                     mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                     if (mySqlCommand.ExecuteNonQuery() == 0)
                     {
                        Message = "NO SE PUDO ACTUALIZAR SOLICITUD PRODUCTO";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                     }
                        
   
                }
                catch (Exception ex)
                {
              
                     Message = "ERROR AL ACTUALIZAR SOLICITUD PRODUCTO";
                   //  logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));

                     _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                     logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                     tran.Rollback();
                     return false;
                }

                //Fin de proceso de Acreditacion  

                // {Inicio el proceso para registar en cta cte

                Response_Code = "23";
                try
                {

                   // mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    if (mySqlCommand.ExecuteNonQuery() == 0) {
                        Message = "NO SE PUDO ACTUALIZAR SECUENCIA DE AUDITORIA";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }

                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR SECUENCIA DE AUDITORIA";
                     _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                     logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                     tran.Rollback();
                     return false;
                }

                try
                {
                
                   // mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    sec_number_auditoria = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                     Response_Code = "24";
                      Message = "ERROR AL SELECCIONAR SECUENCIA AUDITORIA";
                     logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                     tran.Rollback();
                     return false;
                }


                 try
                 {

                    /* sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                                 "VALUES ( " + sec_number_auditoria + ", " + usr_id + ", null, 'Despacho de productos nro." + ult_env_pro + "'" + " , GETDATE()," + ult_env_pro + ",'DISTRIBUCION', 'ENVIOPRODUCTO')";
                         */
                     sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio) VALUES ( @sec_number_auditoria ,  @usr_id , @usrIdSuperior, @traComentario , GETDATE(),@ult_env_pro ,@traDominio, @traSubdominio)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@sec_number_auditoria", sec_number_auditoria);
                    mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@usrIdSuperior", DBNull.Value); 
                    mySqlCommand.Parameters.AddWithValue("@traComentario", "Despacho de productos nro." + ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@traDominio", "DISTRIBUCION");
                    mySqlCommand.Parameters.AddWithValue("@traSubdominio", "ENVIOPRODUCTO");
                    mySqlCommand.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Response_Code = "25";
                    Message = "ERROR AL CREAR KCRTRANSACCION";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                // Volvemos a leer el saldo de la cta cte. 
                ///
                try
                {
				    // mySqlCommand.CommandText = "SELECT  ctaId   FROM KfnCuentaCorriente  WITH (NOLOCK) WHERE ageId =" + age_id;
                    mySqlCommand.CommandText = "SELECT  ctaId , ctaSaldo  FROM KfnCuentaCorriente  WITH (NOLOCK) WHERE ageId = @age_id";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                                        //ctaId = (decimal)mySqlCommand.ExecuteScalar();
                    using (reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            ctaId = (decimal)reader["ctaId"];
                            ctaSaldo = (decimal)reader["ctaSaldo"];
                        }
                        else
                        {
                            Message = "NO SE PUDO SELECCIONAR DATOS DE LA CUENTA CORRIENTE";
                            _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                            tran.Rollback();
                            return false;
                        }
                    }


                }
                catch (Exception ex)
                {
                    Response_Code = "26";
                    Message = "ERROR AL SELECCIONAR DATOS DE LA CUENTA CORRIENTE";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                /*
                try
                {
				  // mySqlCommand.CommandText = "SELECT  ctaSaldo   FROM KfnCuentaCorriente  WITH (NOLOCK) WHERE ageId =" + age_id;
                    mySqlCommand.CommandText = "SELECT  ctaSaldo   FROM KfnCuentaCorriente  WITH (NOLOCK) WHERE ageId = @age_id";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                    ctaSaldo = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "27";
                    Message = string.Empty;
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                 */
                ///



                try
                {
                    //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                    mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR SECUENCIA TRANSACCION";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Response_Code = "28";
                    Message = "ERROR AL ACTUALIZAR SECUENCIA TRANSACCION";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                try
                {
                    //mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                    mySqlCommand.CommandText = "SELECT sec_number_aut = sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                    sec_number_aut = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "29";
                    Message = "ERROR AL SELECCIONAR SECUENCIA TRANSACCION";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                
                try
                {
                    /*
                mySqlCommand.CommandText = "UPDATE KfnCuentaCorriente WITH(ROWLOCK) SET ctaSaldo = ctaSaldo + - " + amount + " WHERE ctaId =" + ctaId + " AND ageId = " + age_id + " AND ctaSaldo =" + ctaSaldo + " AND ( (ctaSaldo + - " + amount + " >= 0) OR (ABS(ctaSaldo + - " + amount + ") <= 600) )";
                    */
                    //SE REALIZA RESTA DE SALDO  
                      mySqlCommand.CommandText = "UPDATE KfnCuentaCorriente WITH(ROWLOCK) SET ctaSaldo = ctaSaldo + -@amount  WHERE ctaId = @ctaId  AND ageId = @age_id  AND ctaSaldo = @ctaSaldo  AND ( (ctaSaldo + - @amount  >= 0) OR (ABS(ctaSaldo + - @amount ) <= 600) )";
                      mySqlCommand.Parameters.Clear();
                      mySqlCommand.Parameters.AddWithValue("@amount", amount);
                      mySqlCommand.Parameters.AddWithValue("@ctaId", ctaId);
                      mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                      mySqlCommand.Parameters.AddWithValue("@ctaSaldo", ctaSaldo);
                      if (mySqlCommand.ExecuteNonQuery() == 0) { 
                          Message ="NO SE PUDO ACTUALIZAR SALDO CUENTA CORRIENTE";
                          _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                          logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                          tran.Rollback();
                          return false;

                      }
                }
                catch (Exception ex) {
                    Response_Code = "30";
                    Message = "ERROR AL ACTUALIZAR SASLDO CUENTA CORRIENTE";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                Response_Code = "31";
                try
                {
                     // mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                     mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                     mySqlCommand.Parameters.Clear();
                     mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                     if (mySqlCommand.ExecuteNonQuery() == 0) 
                     { 
                         Message = "NO SE PUDO ACTUALIZAR SECUENCIA CUENTA MOVIMIENTO";
                         _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                         logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                         tran.Rollback();
                         return false;
                     }
                }
                catch (Exception ex){

                  
                    Message = "ERROR AL ACTUALIZAR SECUENCIA CUENTA MOVIMIENTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;

                }
                
                try
                 {
                     mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                    sec_numbercta_movi = (int)mySqlCommand.ExecuteScalar();
                  
                  
                 }
                catch (Exception ex)
                {
                    Response_Code = "32";
                    Message = "ERROR AL SELECCIONAR SECUENCIA CTACTEMOVIMIENTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                
                try
                {
                /*
                sentencia = "INSERT INTO KfnCuentaCorrienteMovimiento (ccmId,ctaId,ccmFecha,ccmImporte  ,ccmSaldo,ccmDetalle,ccmNumeroTransaccion,ttrId)" +
                            "VALUES ( " + sec_numbercta_movi + "," + ctaId + ",GETDATE()  ," + -amount + "," + ctaSaldo.ToString() + ",'Asignacion de productos No.: " + ult_sol_pro.ToString() + "'" + "," + sec_number_aut.ToString() + ", 2000)";*/
                    //CREA EL MOVIMIENTO EN LA CUENTA CORRIENTE
                    sentencia = "INSERT INTO KfnCuentaCorrienteMovimiento (ccmId,ctaId,ccmFecha,ccmImporte  ,ccmSaldo,ccmDetalle,ccmNumeroTransaccion,ttrId) VALUES ( @sec_numbercta_movi ,@ctaId ,GETDATE()  , -@amount , @ctaSaldo,@ccmDetalle,@sec_number_aut, @ttrId)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@sec_numbercta_movi", sec_numbercta_movi);
                    mySqlCommand.Parameters.AddWithValue("@ctaId", ctaId);
                    mySqlCommand.Parameters.AddWithValue("@amount", amount);
                    mySqlCommand.Parameters.AddWithValue("@ctaSaldo", ctaSaldo);
                    mySqlCommand.Parameters.AddWithValue("@ccmDetalle" , "Asignacion de productos No.: " + ult_sol_pro.ToString(CultureInfo.InvariantCulture));
                    mySqlCommand.Parameters.AddWithValue("@sec_number_aut", sec_number_aut);
                    mySqlCommand.Parameters.AddWithValue("@ttrId", 2000);
            
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = "33";
                    Message = "ERROR AL CREAR EL MOVIMIENTO KFNCUENTACORRIENTEMOVIMIENTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                   
                }

                Response_Code = "34";
                try 
                {
                    //mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    if (mySqlCommand.ExecuteNonQuery() == 0){
                        Message = "NO SE PUDO ACTUALIZAR SECUENCIA AUDITORIA";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
   
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR LA SECUENCIA AUDITORIA";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }


                try 
                {
                   // mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; mySqlCommand.Parameters.Clear(); mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    sec_number_auditoria = (int)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "35";
                    Message = "ERROR AL SELECCIONAR LA SECUENCIA AUDITORIA";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }



                try
                {

                    /*sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                            "VALUES (" + sec_number_auditoria + "," + usr_id + ", null, 'Asignacion de productos',  GETDATE()," + ult_sol_pro + ", 'DISTRIBUCION', 'SOLICITUDPRODUCTO')";*/
                    sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio) VALUES (@sec_number_auditoria ,@usr_id , @usrIdSuperior, @traComentario,  GETDATE(),@ult_sol_pro ,@traDominio, @traSubdominio)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@sec_number_auditoria", sec_number_auditoria);
                    mySqlCommand.Parameters.AddWithValue("@usr_id", usr_id);
                    mySqlCommand.Parameters.AddWithValue("@usrIdSuperior", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@traComentario", "Asignacion de productos");
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@traDominio", "DISTRIBUCION");
                    mySqlCommand.Parameters.AddWithValue("@traSubdominio", "SOLICITUDPRODUCTO");
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = "36";
                    Message = string.Empty;
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }
                // }Fin del proceso para registar en cta cte

                Response_Code = "00";
                Message = "TRANSACCION OK";
                if(committransaction)
                tran.Commit();
                return true;

            }
            catch (Exception ex)
            {
                Response_Code = "37";
                Message = "ERROR AL REALIZAR LA ACREDITACION";
                tran.Rollback();
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                return false;
            }
            finally { if( committransaction )mySqlConnection.Close(); }
        }
        /// <summary>
        /// Liquida la comision al agente destinatario
        /// </summary>
        /// <param name="IdAgenteDestinatario"></param>
        /// <param name="UsrId"></param>
        /// <param name="MontoSolicitud"></param>
        /// <param name="Ahora"></param>
        /// <param name="PorcentajeCom"></param>
        /// <param name="Response_Code"></param>
        /// <param name="Message"></param>
        /// <param name="_mySqlConnection"></param>
        /// <param name="_tran"></param>
        /// <returns></returns>
        public bool LiquidaComisionKinacu(decimal IdAgenteDestinatario, int UsrId, decimal MontoSolicitud, DateTime Ahora, decimal PorcentajeCom, ref string Response_Code, ref string Message, SqlConnection _mySqlConnection = null, SqlTransaction _tran = null)
        {
            logger.InfoLow(() => TagValue.New().Tag("[OP]").Value("LiquidaComisionKinacu"));
            decimal IdUsuario;
            decimal IdAgenteSolicitante = 0;

            decimal SolicitudId;
            decimal IdBodegaOrigen;
            decimal IdBodegaDestino;
            decimal StockIdOrigen;
            decimal StockIdDestina;
            decimal ult_sol_pro;
            decimal ult_env_pro;
            decimal MontoComision;
            string sentencia;


            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;

            SqlConnection mySqlConnection = null;
            SqlTransaction tran = null;
            SqlCommand mySqlCommand = new SqlCommand();
            SqlDataReader reader = null;
            //indica si se debe hacer commit a la transaccion 
            //y si se debe cerrar la conexion
            //dependiendo si se pasa o no una conexion y transaccion como parametro
            bool committransaction = false;
            // variable auxiliar para concatener mensajes de error
            string _m;
            //Variable auxiliar para el codigo de respuesta
            int ResponseCode = 1;
            //variable auxiliar para registrar resultado de los updates
            int resupdate = 0;
            try
            {

                if (_mySqlConnection == null)
                {
                    committransaction = true;
                    mySqlConnection = new SqlConnection(strConnString);
                    mySqlConnection.Open();
                    tran = mySqlConnection.BeginTransaction();
                }
                else if (_mySqlConnection != null && _tran != null)
                {
                    mySqlConnection = _mySqlConnection;
                    tran = _tran;
                }
                else
                {
                    throw new Exception("Parámetros de conexión incorrectos");
                }



                mySqlCommand.Transaction = tran;
                mySqlCommand.Connection = mySqlConnection;

                /* 501 = Distribución de credito por comisiones */
                SolicitudId = cons.SOLICITUD_COMISIONES;


                /* El  monto viene de la Cuenta Corriente y es negativo por eso lo multiplico por -1 */
                // MontoSolicitud = MontoSolicitud * (-1);

               // Response_Code = "01";
                ResponseCode = 2;
                try
                {
				    //mySqlCommand.CommandText = "SELECT age_id_sup  FROM [Agente] WITH(NOLOCK)   WHERE age_id = " + IdAgenteDestinatario;
                    mySqlCommand.CommandText = "SELECT  age_id_sup,usr_id   FROM [Agente] WITH(NOLOCK)   WHERE age_id = @IdAgenteDestinatario";
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteDestinatario", IdAgenteDestinatario);
                    //IdAgenteSolicitante = (decimal)mySqlCommand.ExecuteScalar();
                    using (reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            IdAgenteSolicitante = (decimal)reader["age_id_sup"];
                            IdUsuario = (decimal)reader["usr_id"];
                        }
                        else
                        {
                            Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                            Message = "NO SE ENCONTRARON DATOS DEL AGENTE";
                            _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                            tran.Rollback();
                            return false;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR AL AL SELECCIONAR DATOS DEL AGENTE";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                //SE LEEN LOS DATOS EN UN READER
        
                IdBodegaOrigen = IdAgenteSolicitante;//IdUsuario;
                IdBodegaDestino = IdAgenteDestinatario;


                PorcentajeCom = PorcentajeCom / 100;
                MontoComision = MontoSolicitud * PorcentajeCom;


                ResponseCode = 3;
                /* Termina de calcular el monto por el  Porcentaje y lo deja en @montoComision */
                try
                {
                   //solicitud producto
                    ult_sol_pro = UpdateSecuence("SOLICITUDPRODUCTO", mySqlCommand);
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);//"04";
                    Message = "ERROR AL ACTUALIZAR LA SECUENCIA SOLICITUDPRODUCTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return false;
                }

                ResponseCode = 4;
                /* inserto una solicitud y la coloco en estado pendiente */
                try
                {
                    /*    sentencia = "INSERT INTO [dbo].[KlgSolicitudProducto] WITH(ROWLOCK)	(sprId,usrId,ageIdSolicitante,ageIdDestinatario,prvIdDestinatario,sprEstado,sprFecha,sprFechaAprobacion,sprImporteSolicitud,sltId,ageIdBodegaOrigen,ageIdBodegaDestino) " +
                                    "VALUES (" +ult_sol_pro + "," + IdUsuario + "," + IdAgenteSolicitante + "," + IdAgenteDestinatario + ",NULL" + ",'AU'," + "'" + Ahora + "'" + "," + "'" + Ahora + "'" + "," + MontoComision + "," + SolicitudId "," + IdBodegaOrigen + "," + IdBodegaDestino + ")";*/
                    sentencia = "INSERT INTO [dbo].[KlgSolicitudProducto] WITH(ROWLOCK)	(sprId,usrId,ageIdSolicitante,ageIdDestinatario,prvIdDestinatario,sprEstado,sprFecha,sprFechaAprobacion,sprImporteSolicitud,sltId,ageIdBodegaOrigen,ageIdBodegaDestino) VALUES (@ult_sol_pro ,@IdUsuario , @IdAgenteSolicitante , @IdAgenteDestinatario ,@prvIdDestinatario,@sprEstado,@Ahora,@Ahora ,@MontoComision , @SolicitudId , @IdBodegaOrigen , @IdBodegaDestino )";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteSolicitante", IdAgenteSolicitante);
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteDestinatario", IdAgenteDestinatario);
                    mySqlCommand.Parameters.AddWithValue("@prvIdDestinatario",DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@sprEstado", "AU");
                    mySqlCommand.Parameters.AddWithValue("@Ahora", Ahora);
                    mySqlCommand.Parameters.AddWithValue("@MontoComision", MontoComision);
                    mySqlCommand.Parameters.AddWithValue("@SolicitudId", SolicitudId);
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaOrigen", IdBodegaOrigen);
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaDestino", IdBodegaDestino);
                    mySqlCommand.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR AL CREAR KLG SOLICITUD PRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }


                //Se actualiza el campo usrIdInitiator en caso de existir
                ResponseCode = 5;
                try
                {
                    sentencia = "UPDATE  [KlgSolicitudProducto] WITH(ROWLOCK) SET usrIdInitiator =  @IdUsuario WHERE sprId =@ult_sol_pro";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);

                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        Message = "NO SE PUDO ACTUALIZAR USRIDINITIATOR DE SOLICITUD DE PRODUCTO";
                       _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                       logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                       //tran.Rollback();
                       //return false;
                    }
                        
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR AL ACTUALIZAR USUARIO DE SOLICITUD DE PRODUCTO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    //tran.Rollback();
                    //return false;
                }



                ResponseCode = 6;
                try 
                { 
				   /*    sentencia = "INSERT INTO [dbo].[KlgSolicitudProductoItem] WITH(ROWLOCK) (sprId,prdId,spiCantidadSolicitada,spiCantidadAutorizada,spiPrecioUnitario,spiEstado)  " +
                            "VALUES	( " + ult_sol_pro + ",0" + "," + MontoComision + "," + MontoComision + ",1.0000,'PE')";*/
                    sentencia = "INSERT INTO [dbo].[KlgSolicitudProductoItem] WITH(ROWLOCK) (sprId,prdId,spiCantidadSolicitada,spiCantidadAutorizada,spiPrecioUnitario,spiEstado) VALUES	( @ult_sol_pro ,@prdId ,@MontoComision , @MontoComision ,@spiPrecioUnitario,@spiEstado)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    mySqlCommand.Parameters.AddWithValue("@MontoComision", MontoComision);
                    mySqlCommand.Parameters.AddWithValue("@spiPrecioUnitario", 1m); //"1.0000");
                    mySqlCommand.Parameters.AddWithValue("@spiEstado", "PE");
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR AL CREAR KLG SOLICITUD PRODUCTO ITEM";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }


                ResponseCode = 7;
                try
                {
				    // mySqlCommand.CommandText = "UPDATE [dbo].[KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = 'AU', spiCantidadAutorizada = " + MontoComision + " WHERE sprId = " + ult_sol_pro + " AND prdId = 0 AND spiEstado = 'PE'";
                    mySqlCommand.CommandText = "UPDATE  [dbo].[KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = @newestado, spiCantidadAutorizada = @MontoComision  WHERE sprId = @ult_sol_pro AND prdId = @prdId AND spiEstado = @oldestado";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@newestado", "AU");
                    mySqlCommand.Parameters.AddWithValue("@MontoComision", MontoComision);
                    mySqlCommand.Parameters.AddWithValue("@prdId ", 0);
                    mySqlCommand.Parameters.AddWithValue("@oldestado", "PE");
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR AL ACTUALIZAR KLG SOLICITUD PRODUCTO ITEM";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                ResponseCode = 8;
                try
                {
				
                    ult_env_pro = UpdateSecuence("ENVIOPRODUCTO", mySqlCommand);
                }
                catch (Exception ex)
                {
                    Response_Code = "10";
                    Message = "ERROR AL ACTUALIZAR LA SECUENCIA ENVIOPRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                ResponseCode = 9;
                try
                {     
				    /*  sentencia = "INSERT INTO [dbo].[KlgEnvio] WITH(ROWLOCK) (envId,envEstado,envFechaEnvio,envFechaRecepcion,envNumeroRemito,envNumeroFactura,envObservaciones,ageIdBodegaOrigen,ageIdBodegaDestino) " +
                            "VALUES	(" + ult_env_pro + ",'DE'," + "'" + Ahora + "'" + ",NULL,'','','Envio prd x Comisión Prepaga' " + "," + IdBodegaOrigen + "," + IdBodegaDestino + ")";*/
                    sentencia = "INSERT INTO [dbo].[KlgEnvio] WITH(ROWLOCK) (envId,envEstado,envFechaEnvio,envFechaRecepcion,envNumeroRemito,envNumeroFactura,envObservaciones,ageIdBodegaOrigen,ageIdBodegaDestino) VALUES	(@ult_env_pro ,@envEstado,@Ahora,@envFechaRecepcion,@envNumeroRemito,@envNumeroFactura,@envObservaciones , @IdBodegaOrigen ,@IdBodegaDestino )";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro",ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@envEstado", "DE");
                    mySqlCommand.Parameters.AddWithValue("@Ahora",Ahora);
                    mySqlCommand.Parameters.AddWithValue("@envFechaRecepcion",DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@envNumeroRemito", "");
                    mySqlCommand.Parameters.AddWithValue("@envNumeroFactura", "");
                    mySqlCommand.Parameters.AddWithValue("@envObservaciones", "Envio prd x Comisión Prepaga");
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaOrigen",IdBodegaOrigen);
                    mySqlCommand.Parameters.AddWithValue("@IdBodegaDestino", IdBodegaDestino);
                    mySqlCommand.ExecuteNonQuery();
                 }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR AL CREAR KLG ENVIO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }


                ResponseCode = 10;
                try
                {
				    /*sentencia = "INSERT INTO [dbo].[KlgSolicitudProductoEnvio] WITH(ROWLOCK) (sprId,envId) " +
                            "VALUES	(" + ult_sol_pro + "," + ult_env_pro + ")";*/
                    sentencia = "INSERT INTO [dbo].[KlgSolicitudProductoEnvio] WITH(ROWLOCK) (sprId,envId) VALUES	(@ult_sol_pro ,@ult_env_pro )";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR AL CREAR KLG SOLICITUD PRODUCTOENVIO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                ResponseCode = 11;
                try
                {         
				    /*sentencia = "INSERT INTO [dbo].[KlgEnvioItem] WITH(ROWLOCK) (envId,prdId,eitCantidad,eitEstado) " +
                            "VALUES	(" + ult_env_pro + ",0, " + MontoComision + ",'DE')";*/
                    sentencia = "INSERT INTO [dbo].[KlgEnvioItem] WITH(ROWLOCK) (envId,prdId,eitCantidad,eitEstado) VALUES	(@ult_env_pro  ,@prdId, @MontoComision ,@eitEstado)";
                    mySqlCommand.CommandText = sentencia;
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@prdId",  cons.MULTI_PRODUCTO);//0);
                    mySqlCommand.Parameters.AddWithValue("@MontoComision", MontoComision);
                    mySqlCommand.Parameters.AddWithValue("@eitEstado", "DE");
                    mySqlCommand.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR AL CREAR KLG ENVIO ITEM";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                ResponseCode = 12;
                try
                {
                    mySqlCommand.CommandText = "UPDATE  [dbo].[KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = @newestado  WHERE sprId =  @ult_sol_pro  AND prdId = @prdId AND spiEstado = @oldestado ";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@newestado", "DE");
                    mySqlCommand.Parameters.AddWithValue("@prdId", cons.MULTI_PRODUCTO);// 0);
                    mySqlCommand.Parameters.AddWithValue("@oldestado", "AU");
                    resupdate = mySqlCommand.ExecuteNonQuery();
                    if (resupdate == 0)
                    {
                         Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                        Message ="NO SE PUDO ACTUALIZAR SOLICITUD PRODUCTO IT";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }

                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR AL ACTUALIZAR KLGSOLICITUDPRODUCTOITEM";
                    //logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }


                ResponseCode =13;
                try
                {
                    mySqlCommand.CommandText = "SELECT stkId FROM [dbo].[KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId = @IdAgenteSolicitante  AND prdId  = @prdId";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteSolicitante", IdAgenteSolicitante);
                    mySqlCommand.Parameters.AddWithValue("@prdId", cons.MULTI_PRODUCTO);// 0);
                    StockIdOrigen = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR AL SELECCIONAR KLG STOCK AGENTE PRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                ResponseCode = 14;
                try
                {
				    //mySqlCommand.CommandText = "UPDATE [dbo].[KlgStockAgenteProducto] WITH(ROWLOCK) SET  stkCantidad = stkCantidad - " + MontoComision + " WHERE  stkId = " + StockIdOrigen + " AND ageId = " + IdAgenteSolicitante;
                    mySqlCommand.CommandText = "UPDATE  [dbo].[KlgStockAgenteProducto] WITH(ROWLOCK) SET  stkCantidad = stkCantidad - @MontoComision WHERE  stkId = @StockIdOrigen  AND ageId = @IdAgenteSolicitante and stkCantidad - @MontoComision >= 0 ";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@MontoComision", MontoComision);
                    mySqlCommand.Parameters.AddWithValue("@StockIdOrigen", StockIdOrigen);
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteSolicitante", IdAgenteSolicitante);
                    resupdate = mySqlCommand.ExecuteNonQuery();
                    if (resupdate == 0)
                    {
                        Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                       Message = "AGENTE SUPERIOR NO TIENE SALDO SUFICIENTE PARA REALIZAR LA COMISION";
                       _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                       logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                       tran.Rollback();
                       return false;
                    }
                     
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR AL ACTUALIZAR STOCK AGENTE PRODUCTO";
                   // logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                /* and stkCantidad - @MontoComision >= 0 */
                ResponseCode = 15;
                try
                {
                    //mySqlCommand.CommandText = "UPDATE [dbo].[KlgEnvio] WITH(ROWLOCK) SET envEstado = 'RE', envFechaRecepcion = GETDATE()  WHERE envId = " + ult_env_pro + " AND envEstado = 'DE'";
                    mySqlCommand.CommandText = "UPDATE  [dbo].[KlgEnvio] WITH(ROWLOCK) SET envEstado = @newestado, envFechaRecepcion = GETDATE()  WHERE envId = @ult_env_pro  AND envEstado = @oldestado";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@newestado", "RE");
                    mySqlCommand.Parameters.AddWithValue("@oldestado", "DE");
                    resupdate = mySqlCommand.ExecuteNonQuery();
                    if (resupdate == 0)
                    {
                        Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                        Message = "NO SE PUDO ACTUALIZAR ESTADO DE ENVIO ";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                  

                    }

                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR AL ACTUALIZAR KLGENVIO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }


                ResponseCode = 16;
                try{
                
              
				    //mySqlCommand.CommandText = "UPDATE [dbo].[KlgEnvioItem] WITH(ROWLOCK)  SET eitEstado = 'RE' WHERE envId = " + ult_env_pro + " AND prdId = 0 AND eitEstado = 'DE'";
                    mySqlCommand.CommandText = "UPDATE  [dbo].[KlgEnvioItem] WITH(ROWLOCK)  SET eitEstado = @newestado WHERE envId = @ult_env_pro  AND prdId = @prdId AND eitEstado = @oldestado ";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_env_pro", ult_env_pro);
                    mySqlCommand.Parameters.AddWithValue("@prdId", cons.MULTI_PRODUCTO);//0);
                    mySqlCommand.Parameters.AddWithValue("@newestado", "RE");
                    mySqlCommand.Parameters.AddWithValue("@oldestado", "DE");
                    resupdate = mySqlCommand.ExecuteNonQuery();
                    if (resupdate == 0)
                    {
                        Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                        Message = "NO SE PUDO ACTUALIZAR ENVIO ITEM";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                      
                 }
                catch (Exception ex)
                {
                  
                    Message = "ERROR AL ACTUALIZAR ENVIO ITEM";
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    //logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                ResponseCode = 17;
                try
                {
				    //mySqlCommand.CommandText = "UPDATE [dbo].[KlgSolicitudProductoItem] WITH(ROWLOCK) SET spiEstado = 'RE' WHERE sprId = " + ult_sol_pro + "  AND prdId = 0";
                    mySqlCommand.CommandText = "UPDATE  [dbo].[KlgSolicitudProductoItem] WITH(ROWLOCK) SET spiEstado = @spiEstado WHERE sprId = @ult_sol_pro  AND prdId = @prdId";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@spiEstado", "RE");
                    mySqlCommand.Parameters.AddWithValue("@prdId", cons.MULTI_PRODUCTO);
                    resupdate = mySqlCommand.ExecuteNonQuery();
                    if (resupdate == 0)
                    {
                        Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                        Message = "NO SE PUDO ACTUALIZAR KLG SOLICITUD PRODUCTO ITEM";
                       _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                       logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                       tran.Rollback();
                       return false;
                    }
                }
                catch (Exception ex)
                {

                    Message = "ERROR AL ACTUALIZAR KLG SOLICITUD PRODUCTO ITEM";
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    //logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                ResponseCode = 18;
                try
                {
				    // mySqlCommand.CommandText = "SELECT  stkId FROM [dbo].[KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId =" + IdAgenteDestinatario + " AND prdId IN ( 0)";
                    mySqlCommand.CommandText = "SELECT   stkId FROM [dbo].[KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId = @IdAgenteDestinatario  AND prdId = @prdId";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteDestinatario", IdAgenteDestinatario);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    StockIdDestina = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR AL SELECCIONAR KLG STOCK AGENTE PRODUCTO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }
                ResponseCode = 19;
                try
                {
                    //mySqlCommand.CommandText = "UPDATE [dbo].[KlgStockAgenteProducto] WITH(ROWLOCK) SET  stkCantidad = stkCantidad + " + MontoComision + " WHERE  stkId = " + StockIdDestina + " AND ageId = " + IdAgenteDestinatario + " and stkCantidad  + " + MontoComision + " >= 0";
                    mySqlCommand.CommandText = "UPDATE  [dbo].[KlgStockAgenteProducto] WITH(ROWLOCK) SET  stkCantidad = stkCantidad + @MontoComision  WHERE  stkId = @StockIdDestina AND ageId =  @IdAgenteDestinatario and stkCantidad  + @MontoComision >= 0";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@MontoComision", MontoComision);
                    mySqlCommand.Parameters.AddWithValue("@StockIdDestina", StockIdDestina);
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteDestinatario", IdAgenteDestinatario);
                      resupdate = mySqlCommand.ExecuteNonQuery();
                    if (resupdate == 0)
                    {
                        Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                        Message = "NO SE PUEDE ASINGAR COMISION, EL SALDO DEL AGENTE ES INVALIDO (NEGATIVO)";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                  
                    }
                        
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR AL ACTUALIZAR KLG STOCK AGENTE PRODUCTO";
                    //logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                     _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(new { MontoComision = MontoComision, StockIdDestina = StockIdDestina, IdAgenteDestinatario = IdAgenteDestinatario }));
                    tran.Rollback();
                    return false;
                }
                ResponseCode = 20;
                try
                {
                    //mySqlCommand.CommandText = " UPDATE [dbo].[KlgSolicitudProducto] WITH(ROWLOCK) SET sprEstado = 'CE' WHERE sprId = " + ult_sol_pro + " AND sprEstado = 'AU' AND ( ageIdSolicitante = " + IdAgenteDestinatario + " OR  ageIdDestinatario =" + IdAgenteDestinatario + " ) ";
                    mySqlCommand.CommandText = " UPDATE [dbo].[KlgSolicitudProducto] WITH(ROWLOCK) SET sprEstado = @newestado WHERE sprId = @ult_sol_pro   AND sprEstado = @oldestado AND     ageIdDestinatario = @IdAgenteDestinatario";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@newestado", "CE");
                    mySqlCommand.Parameters.AddWithValue("@ult_sol_pro", ult_sol_pro);
                    mySqlCommand.Parameters.AddWithValue("@oldestado", "AU");
                    mySqlCommand.Parameters.AddWithValue("@IdAgenteDestinatario", IdAgenteDestinatario);
                    resupdate = mySqlCommand.ExecuteNonQuery();
                    if (resupdate == 0)
                    {
                        Response_Code = UtilResut.StrBussErrorCode(ResponseCode);
                        Message ="NO SE PUDO ACTUALIZAR SOLICITUD PRODUCTO";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Response_Code = UtilResut.StrErrorCode(ResponseCode);
                    Message = "ERROR AL ACTUALIZAR SOLICITUD PRODUCTO";
                    //logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(this.GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return false;
                }

                Response_Code = "00";
                Message = "TRANSACCION OK";
                if(committransaction)
                tran.Commit();
                return true;

            }
            catch (Exception ex)
            {
                Response_Code = UtilResut.StrErrorCode(ResponseCode);
                Message = "ERROR AL TRANSFERIR LA COMISIÓN";
                logger.ErrorLow(ex.Message + " .-. " + ex.StackTrace);
                //rollbakc preventivo
                try { if (tran == null)tran.Rollback(); }
                catch (Exception exin) { }
               
                return false;
            }
            finally { if(committransaction) mySqlConnection.Close(); }

        }

        public bool ProcesaSolicitudProducto(decimal sprId, int usr_id, decimal age_id, decimal amount, DateTime date, string comentario, ref string Response_Code, ref string Message)
        {

            string age_comisionadeposito = string.Empty;
            decimal age_montocomision = 0;
            decimal LimiteCredito = 0;
            decimal credito = 0;
            decimal ctaSaldo;
            string sLimiteCredito = string.Empty;
            decimal IdAgenteSolicitante = 0;

            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            SqlConnection mySqlConnection = new SqlConnection(strConnString);
            SqlCommand mySqlCommand = new SqlCommand();

            try
            {

                mySqlConnection.Open();
                mySqlCommand.Connection = mySqlConnection;

                try
                {

                    
                    mySqlCommand.CommandText = "SELECT  ataValor FROM dbo.KcrAtributoAgencia  WITH(ROWLOCK)   WHERE    ageid 	= '" + age_id + "' and attId='LimiteCredito'";
                    sLimiteCredito = mySqlCommand.ExecuteScalar().ToString();
                    LimiteCredito = Convert.ToDecimal(sLimiteCredito);

                }
                catch (Exception ex)
                {
                    Response_Code = "01";
                    Message = "NO SE PUDO LEER LIMITE DE CREDITO ";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    return false;

                }


                try
                {
                    
                    mySqlCommand.CommandText = "SELECT  ctaSaldo FROM KfnCuentaCorriente  WITH(ROWLOCK)   WHERE    ageid 	= '" + age_id + "'";
                    ctaSaldo = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "02";
                    Message = "SALDO EN CTA. CTE. NO EXISTE";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    return false;
                }

                try
                {
                    
                    mySqlCommand.CommandText = "SELECT  age_montocomision FROM  agente  WITH(NOLOCK)  WHERE    age_id 	= '" + age_id + "'";
                    age_montocomision = (decimal)mySqlCommand.ExecuteScalar();

                }
                catch (Exception ex)
                {
                    Response_Code = "04";
                    Message = "MONTO DE COMISIÓN NO EXISTE";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    return false;
                }


                try
                {
                    
                    mySqlCommand.CommandText = "SELECT  age_comisionadeposito FROM  agente  WITH(NOLOCK)  WHERE    age_id 	= '" + age_id + "'";
                    age_comisionadeposito = mySqlCommand.ExecuteScalar().ToString();
                }
                catch (Exception ex)
                {

                    Response_Code = "05";
                    Message = "COMISION EN DEPOSITO NO EXISTE";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    return false;
                }




                try
                {
                    
                    mySqlCommand.CommandText = "SELECT  age_id_sup FROM  agente  WITH(NOLOCK)  WHERE   age_id 	=  " + age_id;
                    IdAgenteSolicitante = Convert.ToInt16(mySqlCommand.ExecuteScalar().ToString());
                }
                catch (Exception ex)
                {
                    Response_Code = "03";
                    Message = "USUARIO NO REGISTRADO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    return false;
                }


                credito = LimiteCredito + ctaSaldo;

                if (amount <= credito)
                {
                    if (AcreditacionSolicitud(sprId, age_id, IdAgenteSolicitante, amount, usr_id, date, comentario, ref Response_Code, ref  Message))
                    {
                        if ((age_comisionadeposito == "S") && (Response_Code == "00"))
                        {
                            if (LiquidaComisionKinacu(age_id, usr_id, amount, date, age_montocomision, ref Response_Code, ref Message))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    Response_Code = "03";
                    Message = "LA AGENCIA NO POSEE LIMITE DE CREDITO PARA ESTA TRANSACCION";
                    return false;
                }
            }


            catch (Exception ex)
            {

                Response_Code = "04";
                Message = "ERROR AL PROCESAR SOLICITUD DE PRODUCTO";
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                return false;
            }
            finally { mySqlConnection.Close(); }

        }

        public bool AcreditacionSolicitud(decimal sprId, decimal age_id, decimal IdAgenteSolicitante, decimal amount, int usr_id, DateTime date, string comentario, ref string Response_Code, ref string Message)
        {


            string age_estado = string.Empty;
            string cubNumero = string.Empty;
            decimal ctaSaldo;
            decimal ctaId;

            decimal IdBodegaOrigen = 0;
            decimal IdBodegaDestino = 0;


            decimal ult_env_pro;
            decimal StockIdOrigen;
            decimal StockIdDestino;
            decimal sec_number_auditoria;
            int sec_number_aut;
            int sec_numbercta_movi;
            string AgenteNombre = string.Empty;
            string sentencia = string.Empty;
            string age_comisionadeposito = string.Empty;
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            SqlConnection mySqlConnection = new SqlConnection(strConnString);
            SqlCommand mySqlCommand = new SqlCommand();


            SqlTransaction tran = null;

            try
            {
                mySqlConnection.Open();
                tran = mySqlConnection.BeginTransaction();
                mySqlCommand.Transaction = tran;
                mySqlCommand.Connection = mySqlConnection;


                //Inicio Proceso de Acreditacion 


                IdBodegaOrigen = IdAgenteSolicitante;
                IdBodegaDestino = age_id;


                
                mySqlCommand.CommandText = " UPDATE KlgSolicitudProducto WITH(ROWLOCK)  SET sprEstado = 'AU', sprFechaAprobacion =" + "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ", sprImporteSolicitud = " + amount.ToString().Replace(",", ".") +
                                            " WHERE sprId = " + sprId + " AND sprEstado = 'PE'";
                mySqlCommand.ExecuteNonQuery();

                
                mySqlCommand.CommandText = " UPDATE KlgSolicitudProductoItem WITH(ROWLOCK) SET spiEstado = 'AU', spiCantidadAutorizada = " + amount.ToString().Replace(",", ".") +
                                           " WHERE sprId = " + sprId + " AND prdId = 0 AND spiEstado = 'PE' ";
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; 
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "ENVIOPRODUCTO");
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear(); 
                mySqlCommand.Parameters.AddWithValue("@paramValue", "ENVIOPRODUCTO");
                ult_env_pro = (int)mySqlCommand.ExecuteScalar();

                
                sentencia = "INSERT INTO [KlgEnvio] WITH(ROWLOCK) (envId,envEstado,envFechaEnvio,envFechaRecepcion,envNumeroRemito,envNumeroFactura,envObservaciones,ageIdBodegaOrigen,ageIdBodegaDestino) " +
                            "VALUES (" + ult_env_pro + ",'DE','" + date.ToString("yyyy-MM-dd HH:mm:ss") + "',NULL,'','','Envio de productos Virtuales en linea'," + IdBodegaOrigen + "," + IdBodegaDestino + ")";

                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                
                sentencia = "INSERT INTO [KlgSolicitudProductoEnvio] WITH(ROWLOCK) (sprId,envId)  " +
                            "VALUES (" + sprId + "," + ult_env_pro + ")";

                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                
                sentencia = "INSERT INTO [KlgEnvioItem] WITH(ROWLOCK) (envId,prdId,eitCantidad,eitEstado)" +
                            "VALUES (" + ult_env_pro + "," + 0 + "," + amount.ToString().Replace(",", ".") + ",'DE')";

                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = 'DE' WHERE sprId =" + sprId + " AND prdId = 0 AND spiEstado = 'AU'";
                mySqlCommand.ExecuteNonQuery();




                
                mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId =" + IdAgenteSolicitante + " AND prdId IN (0)";
                StockIdOrigen = (decimal)mySqlCommand.ExecuteScalar();



                
                mySqlCommand.CommandText = " UPDATE [KlgStockAgenteProducto] WITH(ROWLOCK) SET  stkCantidad = stkCantidad + - " + amount.ToString().Replace(",", ".") + " WHERE  stkId = " + StockIdOrigen + " AND ageId = " + IdAgenteSolicitante + "and stkCantidad + - " + amount + ">= 0";
                mySqlCommand.ExecuteNonQuery();



                
                mySqlCommand.CommandText = " UPDATE [KlgEnvio] WITH(ROWLOCK) SET envEstado = 'RE', envFechaRecepcion = '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'  WHERE envId = " + ult_env_pro + " AND envEstado = 'DE'";
                mySqlCommand.ExecuteNonQuery();




                
                mySqlCommand.CommandText = "UPDATE [KlgEnvioItem] WITH(ROWLOCK) SET eitEstado = 'RE'  WHERE envId = " + ult_env_pro + " AND prdId = 0 AND eitEstado = 'DE'";
                mySqlCommand.ExecuteNonQuery();



                
                mySqlCommand.CommandText = "UPDATE [KlgSolicitudProductoItem] WITH(ROWLOCK)  SET spiEstado = 'RE'  WHERE sprId = " + sprId + "  AND prdId = 0";
                mySqlCommand.ExecuteNonQuery();



                
                mySqlCommand.CommandText = "SELECT  stkId FROM [KlgStockAgenteProducto]  WITH(NOLOCK) WHERE ageId =" + age_id + " AND prdId IN (0)";
                StockIdDestino = (decimal)mySqlCommand.ExecuteScalar();



                
                mySqlCommand.CommandText = "UPDATE KlgStockAgenteProducto SET  stkCantidad = stkCantidad + " + amount.ToString().Replace(",", ".") + "  WHERE  stkId =" + StockIdDestino + " AND ageId =" + age_id + " and stkCantidad + " + amount.ToString().Replace(",", ".") + ">= 0 ";
                mySqlCommand.ExecuteNonQuery();



                
                mySqlCommand.CommandText = " UPDATE KlgSolicitudProducto WITH(ROWLOCK) SET sprEstado = 'CE' WHERE sprId = " + sprId + "  AND sprEstado = 'AU' AND ( ageIdSolicitante =" + age_id.ToString() + " OR ageIdDestinatario =" + age_id.ToString() + ")";
                mySqlCommand.ExecuteNonQuery();

                //Fin de proceso de Acreditacion  



                // {Inicio el proceso para registar en cta cte


                
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; 
                mySqlCommand.Parameters.Clear(); 
                mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; 
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                sec_number_auditoria = (int)mySqlCommand.ExecuteScalar();


                
                sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                    "VALUES ( " + sec_number_auditoria + ", " + usr_id + ", null, '" + (String.IsNullOrEmpty(comentario) ? "Despacho de productos nro." + ult_env_pro : comentario) + "' , GETDATE()," + ult_env_pro + ",'DISTRIBUCION', 'ENVIOPRODUCTO')";

                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                // Volvemos a leer el saldo de la cta cte. 

                
                mySqlCommand.CommandText = "SELECT  ctaId   FROM KfnCuentaCorriente  WITH (NOLOCK) WHERE ageId =" + age_id;
                ctaId = (decimal)mySqlCommand.ExecuteScalar();



                
                mySqlCommand.CommandText = "SELECT  ctaSaldo   FROM KfnCuentaCorriente  WITH (NOLOCK) WHERE ageId =" + age_id;
                ctaSaldo = (decimal)mySqlCommand.ExecuteScalar();




                
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; 
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; 
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "TRANSACCION");
                sec_number_aut = (int)mySqlCommand.ExecuteScalar();



                
                mySqlCommand.CommandText = "UPDATE KfnCuentaCorriente WITH(ROWLOCK) SET ctaSaldo = ctaSaldo + - " + amount.ToString().Replace(",", ".") + " WHERE ctaId =" + ctaId + " AND ageId = " + age_id + " AND ctaSaldo =" + ctaSaldo + " AND ( (ctaSaldo + - " + amount.ToString().Replace(",", ".") + " >= 0) OR (ABS(ctaSaldo + - " + amount.ToString().Replace(",", ".") + ") <= 600) )";
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; 
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                mySqlCommand.ExecuteNonQuery();

                
                mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; 
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "CTACTEMOVIMIENTO");
                sec_numbercta_movi = (int)mySqlCommand.ExecuteScalar();


                
                sentencia = "INSERT INTO KfnCuentaCorrienteMovimiento (ccmId,ctaId,ccmFecha,ccmImporte  ,ccmSaldo,ccmDetalle,ccmNumeroTransaccion,ttrId)" +
                            "VALUES ( " + sec_numbercta_movi + "," + ctaId + ", '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'," + -amount + "," + ctaSaldo.ToString() + ",'Asignacion de productos No.: " + sprId.ToString() + "'" + "," + sec_number_aut.ToString() + ", 2000)";

                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue"; 
                mySqlCommand.Parameters.Clear(); 
                mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                sec_number_auditoria = (int)mySqlCommand.ExecuteScalar();


                
                sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                            "VALUES (" + sec_number_auditoria + "," + usr_id + ", null, 'Asignacion de productos',  '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'," + sprId.ToString() + ", 'DISTRIBUCION', 'SOLICITUDPRODUCTO')";

                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();

                // }Fin del proceso para registar en cta cte

                Response_Code = "00";
                Message = "TRANSACCION OK";
                tran.Commit();
                return true;

            }
            catch (Exception ex)
            {
                Response_Code = "99";
                Message = "ERROR AL REALIZAR LA DISTRIBUCION DE PRODUCTO";
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                tran.Rollback();
                return false;
            }
            finally { mySqlConnection.Close(); }
        }

        public bool RechazaAvisoDeposito(decimal DepositId, decimal age_id, int usr_id, DateTime date, string comentario, ref string Response_Code, ref string Message)
        {
            string age_estado = string.Empty;
            string cubNumero = string.Empty;
            int sec_number_audit;
            string AgenteNombre = string.Empty;
            string sentencia = string.Empty;
            //int usr_id;
            string age_comisionadeposito = string.Empty;
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            SqlConnection mySqlConnection = new SqlConnection(strConnString);
            SqlCommand mySqlCommand = new SqlCommand();

            SqlTransaction tran = null;

            try
            {
                mySqlConnection.Open();
                tran = mySqlConnection.BeginTransaction();
                mySqlCommand.Transaction = tran;
                mySqlCommand.Connection = mySqlConnection;



                //try
                //{
                //    
                //    //mySqlCommand.CommandText = "SELECT  convert(int,usr_id_modificacion) FROM  agente  WITH(NOLOCK)  WHERE   age_id 	=  '" + age_id + "'";
                //    mySqlCommand.CommandText = "SELECT  convert(int,usr_id) FROM  agente  WITH(NOLOCK)  WHERE   age_id 	=  '" + age_id + "'";
                //    usr_id = (int)mySqlCommand.ExecuteScalar();
                //}
                //catch (Exception ex)
                //{
                //    Response_Code = "10";
                //    Message = "USUARIO NO REGISTRADO ";
                //    return false;
                //}


                
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear(); 
                mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                sec_number_audit = (int)mySqlCommand.ExecuteScalar();


                
                sentencia = "UPDATE KfnDeposito SET  depEstado = 'RE', depComentarioProcesamiento =  " + "'" + comentario + "'" +
                            ", processingUsrId =" + usr_id + "   WHERE  depId =" + DepositId;
                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                
                sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                            "VALUES ( " + sec_number_audit + "," + usr_id + ", null," + "'" + comentario + "'" + "," + "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + DepositId.ToString() + ",'FINANCIERO','RECHAZO')";
                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();



                Response_Code = "00";
                Message = "TRANSACCION OK";
                tran.Commit();
                return true;

            }
            catch (Exception ex)
            {
                Response_Code = "01";
                Message = "ERROR AL RECHAZAR DEPOSITO";
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                return false;

            }
            finally { mySqlConnection.Close(); }
        }

        public bool RechazaSolicitudProducto(decimal sprId, decimal age_id, int usr_id, DateTime date, string comentario, ref string Response_Code, ref string Message)
        {
            string age_estado = string.Empty;
            string cubNumero = string.Empty;
            int sec_number_audit;
            string AgenteNombre = string.Empty;
            string sentencia = string.Empty;
            //int usr_id;

            decimal ageIdDestinatario;
            string age_comisionadeposito = string.Empty;
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            SqlConnection mySqlConnection = new SqlConnection(strConnString);
            SqlCommand mySqlCommand = new SqlCommand();

            SqlTransaction tran = null;

            try
            {
                mySqlConnection.Open();
                tran = mySqlConnection.BeginTransaction();
                mySqlCommand.Transaction = tran;
                mySqlCommand.Connection = mySqlConnection;



                //try
                //{
                //    
                //    //mySqlCommand.CommandText = "SELECT  convert(int,usr_id_modificacion) FROM  agente  WITH(NOLOCK)  WHERE   age_id 	=  '" + age_id + "'";
                //    mySqlCommand.CommandText = "SELECT  convert(int,usr_id) FROM  agente  WITH(NOLOCK)  WHERE   age_id 	=  '" + age_id + "'";
                //    usr_id = (int)mySqlCommand.ExecuteScalar();
                //}
                //catch (Exception ex)
                //{
                //    Response_Code = "10";
                //    Message = "USUARIO NO REGISTRADO ";
                //    return false;
                //}


                try
                {
                    
                    mySqlCommand.CommandText = "SELECT  ageIdDestinatario FROM KlgSolicitudProducto  WHERE sprId = " + sprId;
                    ageIdDestinatario = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Response_Code = "11";
                    Message = "NO SE PUDO OBTENER AGENCIA DESTINO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    return false;
                }




                
                sentencia = " UPDATE KlgSolicitudProducto WITH(ROWLOCK) SET sprEstado = 'RJ', sprFechaAprobacion = " + "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'" + " WHERE sprId = " + sprId + "  AND sprEstado = 'PE' " +
                             " AND ( ageIdSolicitante =" + ageIdDestinatario + " OR ageIdDestinatario = " + ageIdDestinatario + ")";
                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                
                sentencia = " UPDATE KlgSolicitudProductoItem WITH(ROWLOCK) SET spiEstado = 'RJ'  WHERE sprId = " + sprId;
                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();



                
                mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number + 1 WHERE sec_objectName =  @paramValue";
                mySqlCommand.Parameters.Clear(); 
                mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                mySqlCommand.ExecuteNonQuery();


                
                mySqlCommand.CommandText = "SELECT  sec_number FROM secuencia WHERE sec_objectName =  @paramValue"; 
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                sec_number_audit = (int)mySqlCommand.ExecuteScalar();


                
                sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio)" +
                    "VALUES ( " + sec_number_audit + "," + usr_id + ", null,'" + (String.IsNullOrEmpty(comentario) ? " Rechazo de la solicitud nro. :" + sprId.ToString() : comentario) + "'," + "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + sprId.ToString() + ",'DISTRIBUCION','SOLICITUDPRODUCTO')";
                mySqlCommand.CommandText = sentencia;
                mySqlCommand.ExecuteNonQuery();


                Response_Code = "00";
                Message = "TRANSACCION OK";
                tran.Commit();
                return true;

            }
            catch (Exception ex)
            {
                Response_Code = "01";
                Message = "ERROR AL RECHAZAR SOLICITUD DE PRODUCTO";
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                return false;

            }
            finally { mySqlConnection.Close(); }
        }

        public DistributionSummary GetSolicitud(long distributionId)
        {
            string Agencia;
            int AgenciaId;
            DateTime FechaSolicitud;
            decimal Monto;
            string Banco;
            string Cuenta;
            string Referencia;
            DateTime FechaDeposito;
            string Estado;
            string Comentario;

            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            SqlConnection mySqlConnection = new SqlConnection(strConnString);
            SqlCommand mySqlCommand = new SqlCommand();
            SqlDataReader reader;
            string sql;

            var oSol = new DistributionSummary();

            mySqlConnection.Open();
            mySqlCommand.Connection = mySqlConnection;

            try
            {
                sql = "SELECT A.SourceDepositRetailerId AgenciaId, A.SourceDepositRetailer agencia ,A.TicketDate FechaSolicitud,  A.Amount Monto," +
                        " B.banNombre Banco,A.BankAccountNumber Cuenta," +
                           " A.TicketId Referencia,A.DepositDate FechaDeposito , A.[State] Estado,A.Notes Comentario, A.DepositId DepositId " +
                           " FROM KfnDepositoCuentaAgente A WITH (NOLOCK) " +
                           " JOIN KfnBanco B WITH (NOLOCK)  on A.BankId = B.banId " +
                           " JOIN KfnCuentaBanco  C WITH (NOLOCK)  on C.banId = B.banId and a.BankAccountId = C.cubId " +
                           " WHERE A.DepositId = " + distributionId;

                mySqlCommand.CommandText = sql;
                reader = mySqlCommand.ExecuteReader();

                if (reader.HasRows)
                    if (reader.Read())
                        oSol = new DistributionSummary()
                        {
                            TargetAgentID = Convert.ToInt32(reader["AgenciaId"].ToString()),
                            TargetAgentName = reader["agencia"].ToString(),
                            RequestTime = Convert.ToDateTime(reader["FechaSolicitud"].ToString()),
                            Amount = Convert.ToDecimal(reader["Monto"].ToString()),
                            BankName = reader["Banco"].ToString(),
                            AccountNumber = reader["Cuenta"].ToString(),
                            ReferenceNumber = reader["Referencia"].ToString(),
                            DepositDate = Convert.ToDateTime(reader["FechaDeposito"].ToString()),
                            Status = reader["Estado"].ToString(),
                            DepositComment = reader["Comentario"].ToString(),
                            OriginalDistributionID = Convert.ToInt64(reader["DepositId"].ToString())
                        };

                reader.Close();

                return oSol;
            }
            catch (Exception ex)
            {
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " . Exception: ", ex.Message, ". ", ex.StackTrace));
                return null;
            }
            finally { mySqlConnection.Close(); }
        }

        public DistributionSummary GetSolicitudProducto(long distributionId)
        {
            string Agencia;
            int AgenciaId;
            DateTime FechaSolicitud;
            decimal Monto;
            string Banco;
            string Cuenta;
            string Referencia;
            DateTime FechaDeposito;
            string Estado;
            string Comentario;

            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            SqlConnection mySqlConnection = new SqlConnection(strConnString);
            SqlCommand mySqlCommand = new SqlCommand();
            SqlDataReader reader;
            string sql;

            var oSol = new DistributionSummary();

            mySqlConnection.Open();
            mySqlCommand.Connection = mySqlConnection;

            try
            {
                sql = @"SELECT 
	                        sp.ageIdSolicitante AgenciaId,a.age_nombre agencia,sp.sprFecha FechaSolicitud,sp.sprImporteSolicitud Monto,
	                        '' Banco,'' Cuenta,'' Referencia,sp.sprFecha FechaDeposito,sp.sprEstado Estado,'' Comentario, CAST(sp.sprId as varchar) DepositoId
                        FROM KlgSolicitudProducto sp WITH (NOLOCK) INNER JOIN 
                        Agente a WITH (NOLOCK) ON a.age_id = sp.ageIdSolicitante
                        WHERE sp.sprId=" + distributionId + @"
                        AND sp.sprEstado = 'PE' 
                        AND sp.sltId = 201 ";
                    
                mySqlCommand.CommandText = sql;
                reader = mySqlCommand.ExecuteReader();

                if (reader.HasRows)
                    if (reader.Read())
                        oSol = new DistributionSummary()
                        {
                            TargetAgentID = Convert.ToInt32(reader["AgenciaId"].ToString()),
                            TargetAgentName = reader["agencia"].ToString(),
                            RequestTime = Convert.ToDateTime(reader["FechaSolicitud"].ToString()),
                            Amount = Convert.ToDecimal(reader["Monto"].ToString()),
                            BankName = reader["Banco"].ToString(),
                            AccountNumber = reader["Cuenta"].ToString(),
                            ReferenceNumber = reader["Referencia"].ToString(),
                            DepositDate = Convert.ToDateTime(reader["FechaDeposito"].ToString()),
                            Status = reader["Estado"].ToString(),
                            DepositComment = reader["Comentario"].ToString(),
                            OriginalDistributionID = Convert.ToInt64(reader["DepositoId"].ToString())
                        };

                reader.Close();

                return oSol;
            }
            catch (Exception ex)
            {
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                return null;
            }
            finally { mySqlConnection.Close(); }
        }

        public DistributionList ListaSolicitudes(int Agencia, int CantFilas)
        {
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            SqlConnection mySqlConnection = new SqlConnection(strConnString);
            SqlCommand mySqlCommand = new SqlCommand();
            SqlDataReader reader;
            string sql;

            var listaSol = new DistributionList();
            var oSol = new DistributionSummary();

            mySqlConnection.Open();
            mySqlCommand.Connection = mySqlConnection;

            try
            {
                //sql = "SELECT TOP (" + CantFilas + ") A.SourceDepositRetailerId AgenciaId, A.SourceDepositRetailer agencia ,A.TicketDate FechaSolicitud,  A.Amount Monto," +
                //        " B.banNombre Banco,A.BankAccountNumber Cuenta," +
                //           " A.TicketId Referencia,A.DepositDate FechaDeposito , A.[State] Estado,A.Notes Comentario " +
                //           " FROM KfnDepositoCuentaAgente A WITH (NOLOCK) " +
                //           " JOIN KfnBanco B WITH (NOLOCK)  on A.BankId = B.banId " +
                //           " JOIN KfnCuentaBanco  C WITH (NOLOCK)  on C.banId = B.banId and a.BankAccountId = C.cubId " +
                //           " WHERE A.TargetDepositRetailerId = " + AgenciaPadre + " and  State = 'PE' ORDER BY A.TicketDate  DESC";

                SqlParameter cantFilasSqlParameter = new SqlParameter();
                cantFilasSqlParameter.ParameterName = "@CantFilas";
                cantFilasSqlParameter.SqlDbType = SqlDbType.Int;
                cantFilasSqlParameter.Direction = ParameterDirection.Input;
                cantFilasSqlParameter.Value = CantFilas;
                mySqlCommand.Parameters.Add(cantFilasSqlParameter);

                SqlParameter agenciaPadreSqlParameter = new SqlParameter();
                agenciaPadreSqlParameter.ParameterName = "@agencia";
                agenciaPadreSqlParameter.SqlDbType = SqlDbType.Int;
                agenciaPadreSqlParameter.Direction = ParameterDirection.Input;
                agenciaPadreSqlParameter.Value = Agencia;
                mySqlCommand.Parameters.Add(agenciaPadreSqlParameter);

                sql = @"SELECT TOP (@CantFilas) a.age_id AgenciaId, a.age_nombre agencia, d.depFechaComprobante FechaSolicitud, d.depMonto Monto,
                            B.banNombre Banco,cb.cubNumero Cuenta,
                            d.depComprobante Referencia,d.depFecha FechaDeposito ,--s.est_descripcion Estado,
							(CASE d.depEstado WHEN 'AU' THEN 'Autorizada' WHEN 'CE' THEN 'Cerrada' WHEN 'PE' THEN 'Pendiente' WHEN 'RE' THEN 'Rechazada' END) Estado,
							(CASE d.depEstado WHEN 'PE' THEN '' WHEN 'AU' THEN d.depComentario ELSE d.depComentarioProcesamiento END) Comentario, 
                            approv.traFecha FechaAprobacion --approv.ccmFecha FechaAprobacion  
                            FROM	dbo.KfnDeposito d WITH (NOLOCK) INNER JOIN 
		                            dbo.Agente a WITH (NOLOCK) ON d.ageIdOrigen = a.age_id INNER JOIN 
									dbo.KfnCuentaBanco cb WITH (NOLOCK) ON cb.cubId = d.cubId INNER JOIN 
		                            dbo.KfnBanco B WITH (NOLOCK)  on cb.banId = B.banId 
									--JOIN Estado s WITH (NOLOCK) ON (s.est_codigo = d.depEstado AND s.est_dominio = 'RECARGA') 
									INNER JOIN dbo.KfnCuentaCorriente AS cc WITH(NOLOCK) ON cc.ageId = d.ageIdOrigen 
									--LEFT JOIN dbo.KfnCuentaCorrienteMovimiento AS approv WITH(NOLOCK) ON (approv.ccmNumeroTransaccion = d.ccmNumeroTransaccion AND approv.ctaId = cc.ctaId AND approv.ccmImporte = d.depMonto AND approv.ccmDetalle = d.depComentarioProcesamiento) 
                                    LEFT JOIN dbo.KcrTransaccion AS approv WITH(NOLOCK) ON d.depId=approv.traidreferencia and approv.trasubdominio = (CASE d.depEstado WHEN 'RE' THEN 'RECHAZO' ELSE 'AUTORIZACION' END)
                           WHERE d.ageIdOrigen = @agencia and d.depFecha>DATEADD(day,-14,getdate()) 
						   --and d.depEstado = 'PE' 
                            ORDER BY d.depFechaComprobante DESC, d.depId DESC";

                mySqlCommand.CommandText = sql;
                reader = mySqlCommand.ExecuteReader();

                if (reader.HasRows)
                    while (reader.Read())
                        listaSol.Add(new DistributionSummary()
                        {
                            TargetAgentID = Convert.ToInt32(reader["AgenciaId"].ToString()),
                            TargetAgentName = reader["agencia"].ToString(),
                            RequestTime = Convert.ToDateTime(reader["FechaSolicitud"].ToString()),
                            Amount = Convert.ToDecimal(reader["Monto"].ToString()),
                            BankName = reader["Banco"].ToString(),
                            AccountNumber = reader["Cuenta"].ToString(),
                            ReferenceNumber = reader["Referencia"].ToString(),
                            DepositDate = Convert.ToDateTime(reader["FechaDeposito"].ToString()),
                            Status = reader["Estado"].ToString(),
                            ApprovalComment = (reader["Comentario"] != null ? reader["Comentario"].ToString() : ""),
                            HasDeposit = true,
                            ApprobationDate = ((reader["FechaAprobacion"] != null && !String.IsNullOrEmpty(reader["FechaAprobacion"].ToString())) ? (Convert.ToDateTime(reader["FechaAprobacion"].ToString())) : (Convert.ToDateTime("1999-01-01")))
                        });

                reader.Close();

                sql = string.Empty;
                sql = @"SELECT  TOP (@CantFilas) sp.ageIdSolicitante AgenciaId, a.age_nombre agencia,
		            --s.est_descripcion Estado,
		            (CASE sp.sprEstado WHEN 'AU' THEN 'Autorizada' WHEN 'CE' THEN 'Cerrada' WHEN 'PE' THEN 'Pendiente' WHEN 'RJ' THEN 'Rechazada' END) Estado,
		            sp.sprFecha FechaSolicitud,sp.sprImporteSolicitud Monto,sp.sprFechaAprobacion FechaAprobacion,
                    '' Comentario
                    FROM KlgSolicitudProducto sp WITH (NOLOCK) 
                    INNER JOIN Agente a WITH (NOLOCK) ON a.age_id = sp.ageIdSolicitante 
					--INNER JOIN Estado s WITH (NOLOCK) ON (s.est_codigo = sp.sprEstado AND s.est_dominio = 'RECARGA') 
                    --LEFT JOIN KcrTransaccion t with (NOLOCK) on t.traIdReferencia=sp.sprId and a.age_id = (select u.age_id FROM usuario u WHERE u.usr_id = t.usrId) and t.trasubdominio = (CASE WHEN sp.sprEstado='CE' THEN 'ENVIOPRODUCTO' ELSE 'SOLICITUDPRODUCTO' END)
                    WHERE sp.sprFecha>DATEADD(day,-14,getdate()) 
                    AND sp.ageIdSolicitante = @agencia 
                    --AND sp.sprEstado = 'PE' 
                    AND sp.sltId = 201 
                    ORDER BY sprFecha DESC";

                mySqlCommand.CommandText = sql;
                reader = mySqlCommand.ExecuteReader();

                if (reader.HasRows)
                    while (reader.Read())
                        listaSol.Add(new DistributionSummary()
                        {
                            TargetAgentID = Convert.ToInt32(reader["AgenciaId"].ToString()),
                            TargetAgentName = reader["agencia"].ToString(),
                            RequestTime = Convert.ToDateTime(reader["FechaSolicitud"].ToString()),
                            Amount = Convert.ToDecimal(reader["Monto"].ToString()),
                            BankName = "",
                            AccountNumber = "",
                            ReferenceNumber = "",
                            DepositDate = Convert.ToDateTime("1999-01-01"),
                            Status = reader["Estado"].ToString(),
                            DepositComment = "",
                            ApprovalComment = (reader["Comentario"] != null ? reader["Comentario"].ToString() : ""),
                            HasDeposit = false,
                            ApprobationDate = ((reader["FechaAprobacion"] != null && !String.IsNullOrEmpty(reader["FechaAprobacion"].ToString())) ? (Convert.ToDateTime(reader["FechaAprobacion"].ToString())) : (Convert.ToDateTime("1999-01-01")))
                        });

                var result = new DistributionList();
                foreach (var item in listaSol.OrderByDescending(d => d.RequestTime).Take(CantFilas))
                    result.Add(item);

                return result;
            }
            catch (Exception ex)
            {
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " . Exception: ", ex.Message, ". ", ex.StackTrace));
                return null;
            }
            finally { mySqlConnection.Close(); }
        }

        public DistributionList ListaSolicitudesPendientes(int AgenciaPadre, int CantFilas)
        {
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            SqlConnection mySqlConnection = new SqlConnection(strConnString);
            SqlCommand mySqlCommand = new SqlCommand();
            SqlDataReader reader;
            string sql;

            var listaSol = new DistributionList();
            var oSol = new DistributionSummary();

            mySqlConnection.Open();
            mySqlCommand.Connection = mySqlConnection;

            try
            {
                //sql = "SELECT TOP (" + CantFilas + ") A.SourceDepositRetailerId AgenciaId, A.SourceDepositRetailer agencia ,A.TicketDate FechaSolicitud,  A.Amount Monto," +
                //        " B.banNombre Banco,A.BankAccountNumber Cuenta," +
                //           " A.TicketId Referencia,A.DepositDate FechaDeposito , A.[State] Estado,A.Notes Comentario " +
                //           " FROM KfnDepositoCuentaAgente A WITH (NOLOCK) " +
                //           " JOIN KfnBanco B WITH (NOLOCK)  on A.BankId = B.banId " +
                //           " JOIN KfnCuentaBanco  C WITH (NOLOCK)  on C.banId = B.banId and a.BankAccountId = C.cubId " +
                //           " WHERE A.TargetDepositRetailerId = " + AgenciaPadre + " and  State = 'PE' ORDER BY A.TicketDate  DESC";

                SqlParameter cantFilasSqlParameter = new SqlParameter();
                cantFilasSqlParameter.ParameterName = "@CantFilas";
                cantFilasSqlParameter.SqlDbType = SqlDbType.Int;
                cantFilasSqlParameter.Direction = ParameterDirection.Input;
                cantFilasSqlParameter.Value = CantFilas;
                mySqlCommand.Parameters.Add(cantFilasSqlParameter);

                SqlParameter agenciaPadreSqlParameter = new SqlParameter();
                agenciaPadreSqlParameter.ParameterName = "@AgenciaPadre";
                agenciaPadreSqlParameter.SqlDbType = SqlDbType.Int;
                agenciaPadreSqlParameter.Direction = ParameterDirection.Input;
                agenciaPadreSqlParameter.Value = AgenciaPadre;
                mySqlCommand.Parameters.Add(agenciaPadreSqlParameter);

                sql = @"SELECT CAST(d.depId as varchar) DepositoId, a.age_id AgenciaId, a.age_nombre agencia, d.depFechaComprobante FechaSolicitud, d.depMonto Monto,
                            B.banNombre Banco,cb.cubNumero Cuenta,cb.cubId CuentaId,
                            d.depComprobante Referencia,d.depFecha FechaDeposito , d.depEstado Estado,(CASE WHEN d.depComentario = 'ppjr were here' THEN '' ELSE d.depComentario END) Comentario 
                            FROM	dbo.KfnCuentaBanco cb WITH (NOLOCK) INNER JOIN
		                            dbo.KfnDeposito d WITH (NOLOCK) ON d.cubId = cb.cubId INNER JOIN
		                            dbo.Agente a WITH (NOLOCK) ON d.ageIdOrigen = a.age_id INNER JOIN
		                            dbo.KfnBanco B WITH (NOLOCK)  on cb.banId = B.banId 
                            JOIN KfnCuentaBanco  C WITH (NOLOCK)  on C.banId = B.banId and d.cubId = C.cubId 
                           WHERE cb.ageId = @AgenciaPadre and d.depEstado = 'PE' ORDER BY d.depFechaComprobante DESC, d.depId DESC";

                mySqlCommand.CommandText = sql;
                reader = mySqlCommand.ExecuteReader();

                if (reader.HasRows)
                    while (reader.Read())
                        listaSol.Add(new DistributionSummary()
                        {
                            OriginalDistributionID = Convert.ToInt64(reader["DepositoId"].ToString()),
                            TargetAgentID = Convert.ToInt32(reader["AgenciaId"].ToString()),
                            TargetAgentName = reader["agencia"].ToString(),
                            RequestTime = Convert.ToDateTime(reader["FechaSolicitud"].ToString()),
                            Amount = Convert.ToDecimal(reader["Monto"].ToString()),
                            BankName = reader["Banco"].ToString(),
                            AccountId = Convert.ToInt32(reader["CuentaId"].ToString()),
                            AccountNumber = reader["Cuenta"].ToString(),
                            ReferenceNumber = reader["Referencia"].ToString(),
                            DepositDate = Convert.ToDateTime(reader["FechaDeposito"].ToString()),
                            Status = reader["Estado"].ToString(),
                            DepositComment = reader["Comentario"].ToString(),
                            HasDeposit = true
                        });

                reader.Close();

                sql = string.Empty;
                sql = @"SELECT CAST(sp.sprId as varchar) DepositoId, sp.ageIdSolicitante AgenciaId, a.age_nombre agencia,sp.sprEstado Estado,sp.sprFecha FechaSolicitud,sp.sprImporteSolicitud Monto 
                    FROM KlgSolicitudProducto sp WITH (NOLOCK) INNER JOIN 
                    Agente a WITH (NOLOCK) ON a.age_id = sp.ageIdSolicitante
                    WHERE sp.ageIdDestinatario = @AgenciaPadre 
                    AND sp.sprEstado = 'PE' 
                    AND sp.sltId = 201 
                    ORDER BY sprFecha DESC";

                mySqlCommand.CommandText = sql;
                reader = mySqlCommand.ExecuteReader();

                if (reader.HasRows)
                    while (reader.Read())
                        listaSol.Add(new DistributionSummary()
                        {
                            OriginalDistributionID = Convert.ToInt64(reader["DepositoId"].ToString()),
                            TargetAgentID = Convert.ToInt32(reader["AgenciaId"].ToString()),
                            TargetAgentName = reader["agencia"].ToString(),
                            RequestTime = Convert.ToDateTime(reader["FechaSolicitud"].ToString()),
                            Amount = Convert.ToDecimal(reader["Monto"].ToString()),
                            BankName = "",
                            AccountNumber = "",
                            ReferenceNumber = "",
                            DepositDate = Convert.ToDateTime("1999-01-01"),
                            Status = "",
                            DepositComment = "",
                            HasDeposit = false
                        });

                var result = new DistributionList();
                foreach (var item in listaSol.OrderBy(d => d.RequestTime))
                    result.Add(item);

                return result;
            }
            catch (Exception ex)
            {
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " . Exception: ", ex.Message, ". ", ex.StackTrace));
                return null;
            }
            finally { mySqlConnection.Close(); }
        }

        public double GetTimeZone()
        {
            double timezone = 0;
            #if Debug
                    timezone = -5.0;
                        return timezone;
            #endif
            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["BASEConnectionString"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@" SELECT [TimeZone]
                              FROM [dbo].[Country]
                              WHERE countryid={0}"), ConfigurationManager.AppSettings["CountryID"]);

                //logger.InfoHigh(query);
                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                timezone = (double)command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                //logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el AgentId de una agencia en la base de datos de Kinacu"));
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " . Exception: ", ex.Message, ". ", ex.StackTrace));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return (timezone);
        }

        public string GetBankNumber(int accountId)
        {
            string bankNumber;

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@" SELECT [cubNumero]
                            FROM [dbo].[KfnCuentaBanco]
                            WHERE cubId={0}"), accountId.ToString());

                //logger.InfoHigh(query);
                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                bankNumber = (string)command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                //logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el AgentId de una agencia en la base de datos de Kinacu"));
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " . Exception: ", ex.Message, ". ", ex.StackTrace));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return (bankNumber);
        }



        public int GetDistributionId(DateTime transactionDate, decimal sourceAgentId, decimal targetAgentId, decimal amount)
        {
            SqlConnection mySqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            SqlCommand mySqlCommand = new SqlCommand();
            int result;

            try
            {
                mySqlConnection.Open();
                mySqlCommand.Connection = mySqlConnection;

                mySqlCommand.CommandText = String.Concat("SELECT top 1 CAST([sprId] as int) ",
                                                        "FROM KlgSolicitudProducto WITH (NOLOCK) ",
                                                        "WHERE sltid in(201,202) and ageidbodegaorigen=@sourceagent and ageidbodegadestino=@targetagent ",
                                                        "and sprimportesolicitud=@amount and CAST(sprfecha as date)=@transactiondate  order by 1 desc");
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@sourceagent", sourceAgentId);
                mySqlCommand.Parameters.AddWithValue("@targetagent", targetAgentId);
                mySqlCommand.Parameters.AddWithValue("@amount", amount);
                mySqlCommand.Parameters.AddWithValue("@transactiondate", transactionDate.Date);
                result = Convert.ToInt32(mySqlCommand.ExecuteScalar());
            }
            catch (Exception ex)
            {
                logger.ExceptionMedium("Falló la búsqueda de distributionid: " + ex.Message + " .-. " + ex.StackTrace);
                return 0;
            }
            finally
            {
                try
                {
                    mySqlConnection.Close();
                }
                catch (Exception ex)
                {
                    logger.ExceptionMedium("Falló la búsqueda de distributionid: " + ex.Message + " .-. " + ex.StackTrace);
                }
            }

            return result;
        }

        private string GetParameters(SqlCommand mySqlCommand)
        {
            IEnumerable<object> queryparameters = mySqlCommand.Parameters.Cast<SqlParameter>().Select(p => new { p.ParameterName, p.Value });
            string v = string.Join(",", queryparameters.ToArray());
            return v;
        }
    }
}