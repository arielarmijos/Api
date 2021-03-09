using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.Logging;
using Movilway.API.Data;
using System.Data.SqlClient;
using System.Net;
using System.Data;
using Movilway.API.KinacuWebService;
using System.Reflection;
using Movilway.API.Core;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    public static class Utils
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(Utils));
        private static string LOG_PREFIX = HttpContext.Current.Session["LOG_PREFIX"].ToString();

        public static int BuildResponseCode(bool result, string message)
        {

            int code = 99;
            try
            {
                code = int.Parse(message.Split('-')[0]);
            }
            catch (Exception) { }

            return result ? 0 : code;
        }

        public static SummaryItems SalesSummary(String agentReference, DateTime summaryDate)
        {
            var productsId = (ConfigurationManager.AppSettings["ReverseProductsID"] ?? "-1,-2").Split(',');
            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            SummaryItems summaries = new SummaryItems();
            try
            {
                var query =
                    String.Format(
                        (@" select t.Fecha,p.prd_nombre'Tipo',t.Aprobadas,t.""Monto Aprobado"",cast(t.IdProducto as varchar)'IdProducto' from
                            (
	                            select cast(v.vta_fecha as date)'Fecha',cast(count(1) as decimal)'Aprobadas',cast(sum(v.vta_monto) as decimal(14,4))'Monto Aprobado',v.prd_id'IdProducto'
	                            from Venta v with (NOLOCK)
	                            join Acceso a with (NOLOCK) on v.usr_id=a.usr_id
	                            where a.acc_login='{0}' and v.vta_fecha >= '{1}' and v.vta_fecha < '{2}' and v.vta_estado='AC'
	                            group by a.acc_login,cast(v.vta_fecha as date),v.prd_id
                            ) t 
                            join Producto p with (NOLOCK) on t.idproducto=p.prd_id
                            order by cast(t.fecha as date) desc
                            "), agentReference, summaryDate.ToString("yyyy-MM-dd"), summaryDate.AddDays(1).ToString("yyyy-MM-dd"));
                //var query =
                //    String.Format(
                //        (@" select cast(v.vta_fecha as date)'Fecha',p.prd_nombre'Tipo',cast(count(1) as decimal)'Aprobadas',cast(sum(v.vta_monto) as decimal(14,4))'Monto Aprobado'
                //            from Venta v with (NOLOCK)
                //            join Acceso a with (NOLOCK) on v.usr_id=a.usr_id
                //            join Producto p with (NOLOCK) on v.prd_id=p.prd_id
                //            where a.acc_login='{0}' and v.vta_fecha >= '{1}' and v.vta_fecha < '{2}' and v.vta_estado='AC'
                //            group by a.acc_login,cast(v.vta_fecha as date),p.prd_nombre
                //            order by cast(v.vta_fecha as date) desc
                //            "), agentReference, summaryDate.ToString("yyyy-MM-dd"), summaryDate.AddDays(1).ToString("yyyy-MM-dd"));

                //logger.InfoHigh(query);
                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        summaries.Add(new SummaryItem()
                        {
                            TransactionType = "Recarga - " + reader.GetString(1),
                            TotalAmount = reader.GetDecimal(3) * (productsId.Contains(reader.GetString(4)) ? -1 : 1),
                            TransactionCount = int.Parse(reader.GetDecimal(2).ToString())
                        });
                    }
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
            return (summaries);
        }

        internal static bool ValidarDeposito(BuyStockRequestBody request, string reference, out string message)
        {
            message = "";

            bool havetovalidate = false;

            Boolean.TryParse((ConfigurationManager.AppSettings["BUYSTOCK_validatedeposits"] ?? "false"), out havetovalidate);

            if (!havetovalidate)
            {

                message = "Validacion desactivada";

                logger.InfoHigh("[" + reference + "] ValidarDeposito La validacion por deposito no esta activa [" + ConfigurationManager.AppSettings["BUYSTOCK_validatedeposits"] + "] se retorna true");

                return true;

            }

            bool result = false;

            bool flag = false;

            string ageIdOrigen = "";
            try
            {
                ageIdOrigen = Utils.GetAgentIdByAcces(request.AuthenticationData.Username, request.DeviceType).ToString();
            }
            catch (Exception ex)
            {


                logger.WarningHigh("[" + reference + "] ValidarDeposito Error obteniendo la agenciaid del usuario " + request.AuthenticationData.Username + " se omite la validacion.");

                logger.ErrorHigh("[" + reference + "] ValidarDeposito Error obteniendo la agenciaid del usuario " + request.AuthenticationData.Username + " " + ex.Message + " " + ex.StackTrace);

                message = "No se pudo obtener agenciaid se omite validacion.";
                result = true;
                return result;


            }


            var CURRENTDAY = "CURRENTDAY";
            var LAST24HOURS = "LAST24HOURS";

            var dayoption = (ConfigurationManager.AppSettings["ValidarDeposito_DATE"] ?? LAST24HOURS);


            try
            {
                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString))
                {


                    //(CASE @dayoption WHEN '"+ CURRENTDAY + "' THEN getdate() ELSE END) 
                    var query =
                            //String.Format(
                            //    (
                            @" 




SELECT   TOP (1)     KfnDeposito.depId, KfnDeposito.ageIdOrigen, KfnDeposito.cubId, KfnDeposito.depComprobante, KfnDeposito.depFechaComprobante, KfnDeposito.depFecha, KfnDeposito.depMonto, KfnDeposito.depEstado, 
                         KfnDeposito.depComentario, KfnDeposito.depComentarioProcesamiento, KfnDeposito.ccmNumeroTransaccion, KfnDeposito.usrid, KfnDeposito.processingUsrId, KfnDeposito.depNumeroTerminal, 
                         KfnDeposito.depNombreTerminal
FROM            KfnDeposito WITH (READUNCOMMITTED)
            
WHERE [depComprobante] = @depComprobante 
--AND [depEstado]  = 'PE'
AND ( cast( [depFechaComprobante] as date) = cast(  @date as date ) )
AND [ageIdOrigen] = @ageIdOrigen
AND (KfnDeposito.cubId = @bank)      
and  KfnDeposito.depMonto = @depMonto
ORDER BY  KfnDeposito.depId desc


                            ";//, agentReference, summaryDate.ToString("yyyy-MM-dd"), summaryDate.AddDays(1).ToString("yyyy-MM-dd"));


                    logger.InfoHigh(query);
                    sqlConnection.Open();

                    var command = new SqlCommand(query, sqlConnection);

                    command.Parameters.AddWithValue("@bank", request.BankName);
                    command.Parameters.AddWithValue("@date", request.TransactionDate.Date);

                    command.Parameters.AddWithValue("@ageIdOrigen", ageIdOrigen);
                    command.Parameters.AddWithValue("@depMonto", request.Amount);
                    command.Parameters.AddWithValue("@depComprobante", request.TransactionReference);

                    logger.InfoHigh("[" + reference + "] BUYSTOCK_ValidateDeposit params bankName= " + request.BankName
                        + "; TransactionDate=" + request.TransactionDate
                          + "; TransactionDateToValidate=" + request.TransactionDate.Date
                        + "; ageIdOrigen " + ageIdOrigen
                    + "; Amount " + request.Amount
                    + "; transactionReference " + request.TransactionReference + " ");




                    var reader = command.ExecuteReader();
                    if (reader.HasRows && reader.Read())
                    {

                        var depId = reader["depId"];
                        //var _ageIdOrigen = reader["ageIdOrigen"];
                        //var cubId = reader["cubId"];
                        string depComprobante = (string)reader["depComprobante"];
                        DateTime depFechaComprobante = (DateTime)reader["depFechaComprobante"];
                        DateTime depFecha = (DateTime)reader["depFecha"];
                        //var depMonto = reader["depMonto"];
                        string depEstado = reader["depEstado"].ToString();
                        //var depComentario = reader["depComentario"];
                        //var depComentarioProcesamiento = reader["depComentarioProcesamiento"];
                        //var ccmNumeroTransaccion = reader["ccmNumeroTransaccion"];
                        //var usrid = reader["usrid"];
                        //var processingUsrId = reader["processingUsrId"];
                        //var depNumeroTerminal = reader["depNumeroTerminal"];
                        //var depNombreTerminal = reader["depNombreTerminal"];
                        //var banId = reader["banId"];
                        logger.InfoHigh("[" + reference + "] BUYSTOCK_ValidateDeposit ULTIMO DEPOSITO ENCONTRADO depId = " + depId + ";depComprobante = " + depComprobante + "; depEstado = " + depEstado + "; depFechaComprobante= " + depFechaComprobante + ".");


                        if (string.Equals(depEstado , "PE", StringComparison.InvariantCultureIgnoreCase))
                        {
                            result = false;

                            string _comprobante = depComprobante ?? "";

                            if (_comprobante.Length > 11)
                                _comprobante = _comprobante.Substring(_comprobante.Length - 11);

                            message = "Ya existe un desposito con el banco, comprobante (" + _comprobante + ") y monto (" + request.Amount.ToString("0.00", CultureInfo.InvariantCulture) + ") para la fecha indicada.";

                            if (message.Length > 140)
                            {

                                message = message.Substring(0, 140);
                            }

                            logger.InfoHigh("[" + reference + "] " + message + " | depid =" + depId + "; agency =" + ageIdOrigen);

                        }
                        else
                        {
                            result = true;
                        }


                        logger.InfoHigh("[" + reference + "] BUYSTOCK_ValidateDeposit result " + result + ".");
                        return result;
                    }
                    else
                    {


                        result = true;
                    }
                }
            }
            catch (Exception e)
            {
                message = "No se pudo realizar la validacion, se omite la misma.";
                result = true;

                logger.ErrorHigh(() => TagValue.New().Exception(e).Message("Error consultando datos de Deposito."));

            }
            finally
            {

            }

            logger.InfoHigh("[" + reference + "] BUYSTOCK_ValidateDeposit result " + result + ".");



            return result;
        }

        public static SummaryItems SalesSummary(String agentReference, DateTime InitialDate, DateTime EndDate)
        {

            SummaryItems summaries = new SummaryItems();
            try
            {
                var CountryId = int.Parse(ConfigurationManager.AppSettings["CountryID"]);
                var PlatformId = 1;

                var productsId = (ConfigurationManager.AppSettings["ReverseProductsID"] ?? "-1,-2");//.Split(',');
                // using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString))
                //{

                var ReverseProducts = (ConfigurationManager.AppSettings["ReverseProductsID"] ?? "-1,-2");
                //using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["BASEConnectionString"].ConnectionString))
                //{
                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["BASEConnectionString"].ConnectionString))
                {
                    sqlConnection.Open();
                    //TODO QUERY PARA QUITAR
                    //@LowerLimit
                    var query =

                      @"WITH AgeInfo AS (
	                    SELECT 
                            TOP (1) u.BranchId, a.UserId, a.AccessTypeId 
                        FROM 
                            [dbo].[Access] AS a WITH(READUNCOMMITTED) 
                            JOIN [dbo].[User] AS u WITH(READUNCOMMITTED) ON (u.CountryId = a.CountryId AND u.PlatformId = a.PlatformId AND u.UserId = a.UserId)
	                    WHERE a.CountryId = @CountryId AND a.PlatformId = @PlatformId AND a.[Login] = @Login
                        )
                        ,
                        ReverseProductsIds AS (
                            SELECT t.value('.', 'INT') AS ProductId 
                            FROM (
                                SELECT CAST('<root><id>' + REPLACE(ISNULL(@ReverseProducts, ''), ',', '</id><id>') + '</id></root>' AS XML) AS ProductsXml
                            ) AS prods 
                            CROSS APPLY prods.ProductsXml.nodes('/root/id') AS x(t) 
                            WHERE @ReverseProducts IS NOT NULL AND LEN(LTRIM(RTRIM(@ReverseProducts))) > 0 AND LEN(LTRIM(RTRIM(@ReverseProducts))) <> '-1'
                        ),
                        TrxCurrentDay AS (
	                        SELECT
		                         'Recargas' AS 'TrxType'
		                        ,pn.ProductName AS 'Product'
                                ,(CASE WHEN p.ProductId IS NOT NULL THEN t.amount ELSE (t.amount * -1) END) AS 'Amount' 
	                        FROM
		                        [dbo].[TransactionCurrentDay] AS t WITH(READUNCOMMITTED) 
		                        JOIN [dbo].[Product] AS pn WITH(READUNCOMMITTED) ON (pn.CountryId = t.CountryId AND pn.PlatformId = t.PlatformId AND pn.ProductId = t.ProductId) 
		                        LEFT JOIN ReverseProductsIds AS p ON (p.ProductId = t.ProductId) 
	                        WHERE 
		                        t.CountryId = @CountryId 
		                        AND t.PlatformId = @PlatformId 
		                        AND t.DateValue >= CAST(GETDATE() AS DATE) 
		                        AND t.DateValue <= @UpperLimit 
		                        --AND t.AccessTypeId = (SELECT AccessTypeId FROM AgeInfo) 
		                        --AND t.UserId = (SELECT UserId FROM AgeInfo) 
                                AND t.BranchId = (SELECT BranchId FROM AgeInfo) 
		                        AND t.StatusId = @StatusId 
                        ),
                        TrxThreeMonths AS (
	                        SELECT
		                         'Recargas' AS 'TrxType'
		                        ,pn.ProductName AS 'Product'
                                ,(CASE WHEN p.ProductId IS NOT NULL THEN t.amount ELSE (t.amount * -1) END) AS 'Amount'
	                        FROM
		                        [dbo].[TransactionThreeMonths] AS t WITH(READUNCOMMITTED) 
		                        JOIN [dbo].[Product] AS pn WITH(READUNCOMMITTED) ON (pn.CountryId = t.CountryId AND pn.PlatformId = t.PlatformId AND pn.ProductId = t.ProductId) 
		                        LEFT JOIN ReverseProductsIds AS p ON (p.ProductId = t.ProductId) 
	                        WHERE 
		                        t.CountryId = @CountryId 
		                        AND t.PlatformId = @PlatformId 
		                        AND t.DateValue <= @UpperLimit 		
		                        AND t.DateValue >= @LowerLimit 
		                        --AND t.AccessTypeId = (SELECT AccessTypeId FROM AgeInfo) 
		                        --AND t.UserId = (SELECT UserId FROM AgeInfo) 
                                AND t.BranchId = (SELECT BranchId FROM AgeInfo) 
		                        AND t.StatusId = @StatusId  
                        ),
                        AllInfo AS (
                            SELECT * FROM TrxCurrentDay 
                            UNION ALL 
                            SELECT * FROM TrxThreeMonths 
                        )
                        SELECT 
                             TrxType AS TrxType
	                        ,Product AS Product
                            ,SUM(Amount) AS TotalAmount
                            ,COUNT(1) AS TotalTrx
                        FROM 
                            AllInfo 
                        GROUP BY TrxType, Product
                        ;";



                    //var query =
                    //    String.Format(
                    //        (@" select cast(v.vta_fecha as date)'Fecha',p.prd_nombre'Tipo',cast(count(1) as decimal)'Aprobadas',cast(sum(v.vta_monto) as decimal(14,4))'Monto Aprobado'
                    //            from Venta v with (NOLOCK)
                    //            join Acceso a with (NOLOCK) on v.usr_id=a.usr_id
                    //            join Producto p with (NOLOCK) on v.prd_id=p.prd_id
                    //            where a.acc_login='{0}' and v.vta_fecha >= '{1}' and v.vta_fecha < '{2}' and v.vta_estado='AC'
                    //            group by a.acc_login,cast(v.vta_fecha as date),p.prd_nombre
                    //            order by cast(v.vta_fecha as date) desc
                    //            "), agentReference, summaryDate.ToString("yyyy-MM-dd"), summaryDate.AddDays(1).ToString("yyyy-MM-dd"));

                    //logger.InfoHigh(query);


                    var command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@CountryId", CountryId);//3
                    command.Parameters.AddWithValue("@PlatformId", PlatformId);//INT = 1;
                    command.Parameters.AddWithValue("@Login", agentReference);//"440044"
                    command.Parameters.AddWithValue("@LowerLimit", InitialDate.ToString("yyyyMMdd"));//"20150201"
                    command.Parameters.AddWithValue("@UpperLimit", EndDate.ToString("yyyyMMdd"));//"20150512"
                    command.Parameters.AddWithValue("@ReverseProducts", ReverseProducts); //VARCHAR(50) = '';
                    command.Parameters.AddWithValue("@StatusId ", "AC");

                    using (var reader = command.ExecuteReader())
                    {


                        if (reader.HasRows)
                            while (reader.Read())
                            {
                                summaries.Add(new SummaryItem()
                                {
                                    TransactionType = reader.GetString(0) + " - " + reader.GetString(1),

                                    TotalAmount = reader.GetDecimal(2),//reader.GetDecimal(3) * (productsId.Contains(reader.GetString(4)) ? -1 : 1),
                                    TransactionCount = reader.GetInt32(3)

                                });
                            }
                    }

                }
            }
            catch (Exception e)
            {
                string Message = "ERROR TRANTANDO DE CONSULTAR EL RESÚMEN EN LA BASE DE DATOS DE KINACU";
                logger.ErrorLow(() => TagValue.New().Exception(e).Message(Message));
                throw new Exception(Message, e);
            }

            return (summaries);
        }

        public static SummaryItems SalesSummaryByAgent(String agentReference, DateTime summaryDate, out decimal initialAmount, out decimal finalAmount)
        {
            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["BASEConnectionString"].ConnectionString);
            SummaryItems summaries = new SummaryItems();
            try
            {
                var query =
                    String.Format(
                        (@" SELECT 'Recargas' 'Tipo',count(1)'Cantidad',IsNull(sum(t.amount),0)'Monto'
                            from (
	                            select distinct primarycode,amount from (
                                    SELECT t.*
		                            FROM [TransactionCurrentDay] t with (NOLOCK)
		                            join [Branch] b with (NOLOCK) on b.countryid=t.countryid and b.platformid=t.platformid and b.branchid=t.branchid
		                            join [User] u with (NOLOCK) on u.branchid=b.branchid and u.countryid=b.countryid and u.platformid=b.platformid
		                            join [Access] a with (NOLOCK) on u.userid=a.userid and u.countryid=a.countryid and u.platformid=a.platformid
                                    where a.[login]='{0}' and t.statusid='AC' and t.countryid={2} and t.datevalue='{1}' and t.productid in ({3})

		                            UNION

		                            SELECT t.*
		                            FROM [TransactionThreeMonths] t with (NOLOCK)
		                            join [Branch] b with (NOLOCK) on b.countryid=t.countryid and b.platformid=t.platformid and b.branchid=t.branchid
		                            join [User] u with (NOLOCK) on u.branchid=b.branchid and u.countryid=b.countryid and u.platformid=b.platformid
		                            join [Access] a with (NOLOCK) on u.userid=a.userid and u.countryid=a.countryid and u.platformid=a.platformid
                                    where a.[login]='{0}' and t.statusid='AC' and t.countryid={2} and t.datevalue='{1}' and t.productid in ({3})
	                            ) t
                                UNION
	                            select distinct primarycode,amount*-1 from (
                                    SELECT t.*
		                            FROM [TransactionCurrentDay] t with (NOLOCK)
		                            join [Branch] b with (NOLOCK) on b.countryid=t.countryid and b.platformid=t.platformid and b.branchid=t.branchid
		                            join [User] u with (NOLOCK) on u.branchid=b.branchid and u.countryid=b.countryid and u.platformid=b.platformid
		                            join [Access] a with (NOLOCK) on u.userid=a.userid and u.countryid=a.countryid and u.platformid=a.platformid
                                    where a.[login]='{0}' and t.statusid='AC' and t.countryid={2} and t.datevalue='{1}' and t.productid not in ({3})

		                            UNION

		                            SELECT t.*
		                            FROM [TransactionThreeMonths] t with (NOLOCK)
		                            join [Branch] b with (NOLOCK) on b.countryid=t.countryid and b.platformid=t.platformid and b.branchid=t.branchid
		                            join [User] u with (NOLOCK) on u.branchid=b.branchid and u.countryid=b.countryid and u.platformid=b.platformid
		                            join [Access] a with (NOLOCK) on u.userid=a.userid and u.countryid=a.countryid and u.platformid=a.platformid
                                    where a.[login]='{0}' and t.statusid='AC' and t.countryid={2} and t.datevalue='{1}' and t.productid in ({3})
	                            ) t
                                UNION
	                            select distinct primarycode,amount*-1 from (
                                    SELECT t.*
		                            FROM [TransactionCurrentDay] t with (NOLOCK)
		                            join [Branch] b with (NOLOCK) on b.countryid=t.countryid and b.platformid=t.platformid and b.branchid=t.branchid
		                            join [User] u with (NOLOCK) on u.branchid=b.branchid and u.countryid=b.countryid and u.platformid=b.platformid
		                            join [Access] a with (NOLOCK) on u.userid=a.userid and u.countryid=a.countryid and u.platformid=a.platformid
                                    where a.[login]='{0}' and t.statusid='AC' and t.countryid={2} and t.datevalue='{1}' and t.productid not in ({3})

		                            UNION

		                            SELECT t.*
		                            FROM [TransactionThreeMonths] t with (NOLOCK)
		                            join [Branch] b with (NOLOCK) on b.countryid=t.countryid and b.platformid=t.platformid and b.branchid=t.branchid
		                            join [User] u with (NOLOCK) on u.branchid=b.branchid and u.countryid=b.countryid and u.platformid=b.platformid
		                            join [Access] a with (NOLOCK) on u.userid=a.userid and u.countryid=a.countryid and u.platformid=a.platformid
                                    where a.[login]='{0}' and t.statusid='AC' and t.countryid={2} and t.datevalue='{1}' and t.productid not in ({3})
	                            ) t
                            ) t

                            UNION

                            select 'Dist Recibidas' 'Tipo',sum(T.cantidad)'Cantidad',sum(T.Monto)'Monto' from 
                            (
	                            SELECT count(1)'Cantidad',IsNull(sum(r.amount),0)'Monto'
	                            FROM [Request] r with (NOLOCK)
	                            join [Branch] b with (NOLOCK) on r.recipientbranchid=b.branchid and r.platformid=b.platformid and r.countryid=b.countryid
	                            join [User] u with (NOLOCK) on u.branchid=b.branchid and u.countryid=b.countryid and u.platformid=b.platformid
	                            join [Access] a with (NOLOCK) on u.userid=a.userid and u.countryid=a.countryid and u.platformid=a.platformid
	                            where a.[login]='{0}' and r.requesttypeid in (201,202) and r.statusid='CE' and r.countryid={2} and cast(r.datecreated as date)='{1}'
                            ) T

                            UNION

                            select 'Dist Realizadas' 'Tipo',sum(T.cantidad)'Cantidad',sum(T.Monto)'Monto' from 
                            (
	                            SELECT count(1)'Cantidad',IsNull(sum(r.amount*-1),0)'Monto'
	                            FROM [Request] r with (NOLOCK)
	                            join [Branch] b with (NOLOCK) on r.applicantbranchid=b.branchid and r.platformid=b.platformid and r.countryid=b.countryid
	                            join [User] u with (NOLOCK) on u.branchid=b.branchid and u.countryid=b.countryid and u.platformid=b.platformid
	                            join [Access] a with (NOLOCK) on u.userid=a.userid and u.countryid=a.countryid and u.platformid=a.platformid
	                            where a.[login]='{0}' and r.requesttypeid=201 and r.statusid='CE' and r.countryid={2} and cast(r.datecreated as date)='{1}'

	                            UNION

	                            SELECT count(1)'Cantidad',IsNull(sum(r.amount*-1),0)'Monto'
                                FROM [Request] r with (NOLOCK)
                                join [Branch] b with (NOLOCK) on r.applicantbranchid=b.branchid and r.platformid=b.platformid and r.countryid=b.countryid
                                join [User] u with (NOLOCK) on u.branchid=b.branchid and u.countryid=b.countryid and u.platformid=b.platformid
                                join [Access] a with (NOLOCK) on u.userid=a.userid and u.countryid=a.countryid and u.platformid=a.platformid
                                where a.[login]='{0}' and r.requesttypeid=202 and r.statusid='CE' and r.countryid={2} and cast(r.datecreated as date)='{1}'
                            ) T

                            UNION

                            SELECT 'Comisiones' 'Tipo',sum(T.Cantidad)'Cantidad',sum(T.Monto)'Monto' from
                            (
	                            SELECT count(1)'Cantidad',IsNull(sum(r.amount),0)'Monto'
	                            FROM [Request] r with (NOLOCK)
	                            join [Branch] b with (NOLOCK) on r.recipientbranchid=b.branchid and r.platformid=b.platformid and r.countryid=b.countryid
	                            join [User] u with (NOLOCK) on u.branchid=b.branchid and u.countryid=b.countryid and u.platformid=b.platformid
	                            join [Access] a with (NOLOCK) on u.userid=a.userid and u.countryid=a.countryid and u.platformid=a.platformid
	                            where a.[login]='{0}' and r.requesttypeid=501 and r.statusid='CE' and r.countryid={2} and cast(r.datecreated as date)='{1}'
                            ) T
                            "), agentReference, summaryDate.ToString("yyyy-MM-dd"), ConfigurationManager.AppSettings["CountryID"], ConfigurationManager.AppSettings["ReverseProductsID"] ?? "-1");

                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        summaries.Add(new SummaryItem()
                        {
                            TransactionType = reader.GetString(0),
                            TotalAmount = reader.GetDecimal(2),
                            TransactionCount = reader.GetInt32(1)
                        });
                    }
                reader.Close();

                query =
                    String.Format(
                        (@" SELECT h.stockamount 'Monto'
                            FROM [BranchBalanceHistory] h with (NOLOCK)
                            join [Branch] b with (NOLOCK) on b.countryid=h.countryid and b.platformid=h.platformid and b.branchid=h.branchid
                            join [User] u with (NOLOCK) on u.branchid=b.branchid and u.countryid=b.countryid and u.platformid=b.platformid
                            join [Access] a with (NOLOCK) on u.userid=a.userid and u.countryid=a.countryid and u.platformid=a.platformid
                            where a.[login]='{0}' and h.countryid={2} and h.dateimportvalue='{1}'"),
                        agentReference, summaryDate.AddDays(-1).ToString("yyyyMMdd"), ConfigurationManager.AppSettings["CountryID"]);
                command = new SqlCommand(query, sqlConnection);
                initialAmount = 0m;
                reader = command.ExecuteReader();
                if (reader.HasRows)
                    if (reader.Read())
                        initialAmount = reader.GetDecimal(0);
                finalAmount = initialAmount;
                foreach (var item in summaries)
                {
                    finalAmount += item.TotalAmount;
                    item.TotalAmount = Math.Abs(item.TotalAmount);
                }
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el resúmen por transacción en la base de datos de BASE"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return (summaries);
        }

        public static SummaryItems SalesSummaryByAgent(String agentReference, DateTime InitialDate, DateTime EndDate, out decimal initialAmount, out decimal finalAmount)
        {
            SummaryItems summaries = new SummaryItems();
            //
            initialAmount = 0m; finalAmount = 0m;


            try
            {
                var CountryId = int.Parse(ConfigurationManager.AppSettings["CountryID"]);
                var PlatformId = 1;
                var ReverseProducts = (ConfigurationManager.AppSettings["ReverseProductsID"] ?? "-1,-2");

                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["BASEConnectionString"].ConnectionString))
                {
                    sqlConnection.Open();

                    var query =
                        @"DECLARE @CountryId INT = @CountryId2;
                        DECLARE @PlatformId INT = @PlatformId2;
                        DECLARE @Login VARCHAR(25) = @Login2;
                        DECLARE @LowerLimit DATE = @LowerLimit2;
                        DECLARE @UpperLimit DATE = @UpperLimit2;
                        DECLARE @ReverseProducts VARCHAR(50) = @ReverseProducts2;
                        WITH AgeInfo AS (
                            SELECT 
                                TOP (1) u.BranchId, a.UserId, a.AccessTypeId 
                            FROM 
                                [dbo].[Access] AS a WITH(READUNCOMMITTED) 
                                JOIN [dbo].[User] AS u WITH(READUNCOMMITTED) ON (u.CountryId = a.CountryId AND u.PlatformId = a.PlatformId AND u.UserId = a.UserId)
                            WHERE a.CountryId = @CountryId AND a.PlatformId = @PlatformId AND a.[Login] = @Login
                        )
                        ,
                        ReverseProductsIds AS (
                            SELECT t.value('.', 'INT') AS ProductId 
                            FROM (
                                SELECT CAST('<root><id>' + REPLACE(ISNULL(@ReverseProducts, ''), ',', '</id><id>') + '</id></root>' AS XML) AS ProductsXml
                            ) AS prods 
                            CROSS APPLY prods.ProductsXml.nodes('/root/id') AS x(t) 
                            WHERE @ReverseProducts IS NOT NULL AND LEN(LTRIM(RTRIM(@ReverseProducts))) > 0 AND LEN(LTRIM(RTRIM(@ReverseProducts))) <> '-1'
                        ),
                        TrxCurrentDay AS (
                            SELECT
                                 (CASE WHEN p.ProductId IS NOT NULL THEN t.amount ELSE (t.amount * -1) END) AS 'Amount'
                                ,t.UserId AS 'UserId'
                            FROM
                                [dbo].[TransactionCurrentDay] AS t WITH(READUNCOMMITTED) 
                                LEFT JOIN ReverseProductsIds AS p ON (p.ProductId = t.ProductId) 
                            WHERE 
                                t.CountryId = @CountryId 
                                AND t.PlatformId = @PlatformId 
                                AND t.DateValue >= CAST(GETDATE() AS DATE) 
                                AND t.DateValue <= @UpperLimit 
                                AND t.BranchId = (SELECT BranchId FROM AgeInfo) 
                                AND t.StatusId = 'AC' 
                        ),
                        TrxThreeMonths AS (
                            SELECT
                                  (CASE WHEN p.ProductId IS NOT NULL THEN t.amount ELSE (t.amount * -1) END) AS 'Amount'
                                 ,t.UserId AS 'UserId'
                            FROM
                                [dbo].[TransactionThreeMonths] AS t WITH(READUNCOMMITTED) 
                                LEFT JOIN ReverseProductsIds AS p ON (p.ProductId = t.ProductId) 
                            WHERE 
                                t.CountryId = @CountryId 
                                AND t.PlatformId = @PlatformId 
                                AND t.DateValue <= @UpperLimit 		
                                AND t.DateValue >= @LowerLimit 
                                AND t.BranchId = (SELECT BranchId FROM AgeInfo) 
                                AND t.StatusId = 'AC' 
                        ),
                        AllTrx AS (
                            SELECT * FROM TrxCurrentDay 
                            UNION ALL 
                            SELECT * FROM TrxThreeMonths 
                        ),
                        TransactionsByUser AS (
                            SELECT 
                                 RTRIM(CONCAT('Recargas - ', u.UserName, ' ', u.Surname)) AS 'TrxType'
                                ,t.Amount AS 'Amount'
                            FROM 
                                AllTrx AS t 
                                JOIN [dbo].[User] AS u WITH(READUNCOMMITTED) ON (u.CountryId = @CountryId AND u.PlatformId = @PlatformId AND u.UserId = t.UserId) 
                        ),
                        Requests AS (
                            SELECT
                               (CASE 
                                    WHEN (r.ApplicantBranchId = (SELECT BranchId FROM AgeInfo) AND r.RequestTypeId IN (201, 202)) THEN 'Dist Realizadas' 
                                    WHEN (r.RecipientBranchId = (SELECT BranchId FROM AgeInfo) AND r.RequestTypeId IN (201, 202)) THEN 'Dist Recibidas'
                                    WHEN r.RequestTypeId = 501 THEN (CASE WHEN (r.RecipientBranchId = (SELECT BranchId FROM AgeInfo)) THEN 'Comisiones Recibidas' ELSE 'Comisiones Realizadas' END)
                                    WHEN r.RequestTypeId = 205 THEN (CASE WHEN (r.ApplicantBranchId = (SELECT BranchId FROM AgeInfo)) THEN 'Dist Realizadas' ELSE 'Dist Recibidas' END) 
                                    ELSE rt.[Description]
                                END) AS 'TrxType'
                                ,(CASE WHEN (r.ApplicantBranchId = (SELECT BranchId FROM AgeInfo)) THEN (r.Amount * -1) ELSE r.Amount END) AS 'Amount'
                            FROM   
                                [dbo].[Request] AS r WITH(READUNCOMMITTED) 
                                JOIN [dbo].[RequestType] AS rt WITH(READUNCOMMITTED) ON (rt.RequestTypeId = r.RequestTypeId) 
                            WHERE  
                                r.CountryId = @CountryId 
                                AND r.PlatformId = @PlatformId 
                                AND r.DateApproved < DATEADD(DAY, 1, @UpperLimit)
                                AND r.DateApproved >= @LowerLimit 
                                AND (
                                    r.ApplicantBranchId = (SELECT BranchId FROM AgeInfo) 
                                    OR 
                                    r.RecipientBranchId = (SELECT BranchId FROM AgeInfo)
                                )
                                AND r.RequestTypeId IN (201, 202, 205, 501)
                                AND r.StatusId = 'CE'
                        ),
                        InitialStock AS (
                            SELECT TOP (1)
                                 'Balance' AS 'TrxType'
                                ,bh.StockAmount AS 'Amount'
                            FROM
                                [dbo].[BranchBalanceHistory] AS bh WITH(READUNCOMMITTED) 
                            WHERE 
                                bh.CountryId = @CountryId
                                AND bh.PlatformId = @PlatformId 
                                AND bh.BranchId = (SELECT BranchId FROM AgeInfo) 
                                AND bh.DateStockValue >= @LowerLimit 
                            ORDER BY bh.DateStockValue ASC
                        ),
                        AllInfo AS (
                            SELECT * FROM TransactionsByUser 
                            UNION ALL 
                            SELECT * FROM Requests 
                            UNION ALL 
                            SELECT * FROM InitialStock 
                        )
                        SELECT 
                             TrxType AS TrxType
                            ,SUM(Amount) AS TotalAmount
                            ,COUNT(1) AS TotalTrx
                        FROM 
                            AllInfo 
                        WHERE 
                            TrxType <> 'N/A' 
                        GROUP BY TrxType
                        ;";



                    var command = new SqlCommand(query, sqlConnection);


                    command.Parameters.AddWithValue("@CountryId2", CountryId); //INT = 3;
                    command.Parameters.AddWithValue("@PlatformId2", PlatformId);// INT = 1;
                    command.Parameters.AddWithValue("@Login2", agentReference);// VARCHAR(25) = '440044';
                    command.Parameters.AddWithValue("@LowerLimit2", InitialDate.ToString("yyyyMMdd"));// DATE = '20150201';
                    command.Parameters.AddWithValue("@UpperLimit2", EndDate.ToString("yyyyMMdd"));// DATE = '20150512';
                    command.Parameters.AddWithValue("@ReverseProducts2", ReverseProducts);// VARCHAR(50) = '';

                    string prefixrecargas = "Recargas";

                    //Dist Recibidas, Dist Realizadas, Comisiones, Recargas
                    Dictionary<string, SummaryItem> reportesorder = new Dictionary<string, SummaryItem>
                    {
                        {"Dist Realizadas", new SummaryItem()
                        {
                            TransactionType = "Dist Realizadas",
                            TotalAmount = 0,
                            TransactionCount = 0
                        }},
                        {"Dist Recibidas", new SummaryItem()
                         {
                        TransactionType = "Dist Recibidas",
                        TotalAmount = 0,
                        TransactionCount = 0
                         }},
                        {"Comisiones Recibidas",new SummaryItem()
                         {
                        TransactionType = "Comisiones Recibidas",
                        TotalAmount = 0,
                        TransactionCount = 0
                        }},
                        {"Comisiones Realizadas",new SummaryItem()
                         {
                        TransactionType = "Comisiones Realizadas",
                        TotalAmount = 0,
                        TransactionCount = 0
                        }}/*,
                        {
                        prefixrecargas,new SummaryItem()
                        {
                            TransactionType = "Recargas",
                            TotalAmount = 0,
                            TransactionCount = 0
                        }
                        }*/
                    };


                    int aux = 0;

                    using (var reader = command.ExecuteReader())
                    {


                        if (reader.HasRows)
                        {

                            while (reader.Read())
                            {
                                string trxType = reader["TrxType"].ToString();


                                if (trxType.Equals("Balance"))
                                {
                                    SummaryItem balance = new SummaryItem()
                                    {
                                        TransactionType = reader.GetString(0),
                                        TotalAmount = reader.GetDecimal(1),
                                        TransactionCount = reader.GetInt32(2)
                                    };
                                    //summaries.Add(balance);
                                    //encaso de que finalAmount
                                    finalAmount += balance.TotalAmount;
                                    initialAmount = balance.TotalAmount;
                                }
                                else if (trxType.IndexOf(prefixrecargas) > -1)
                                {
                                    //tipos de datos Recargas por usuario
                                    SummaryItem balance = new SummaryItem()
                                    {
                                        TransactionType = reader.GetString(0),
                                        TotalAmount = reader.GetDecimal(1),
                                        TransactionCount = reader.GetInt32(2)
                                    };

                                    SummaryItem auxIt = null;
                                    //summaries.Add(balance);
                                    //encaso de que finalAmount

                                    if (reportesorder.TryGetValue(trxType, out auxIt))
                                    {
                                        aux++;
                                        logger.ErrorLow(String.Concat(LOG_PREFIX, " DATOS REPETIDOS EN QUERY "));
                                        trxType = String.Concat(trxType, " ", "[" + aux + "]");
                                    }
                                    reportesorder.Add(trxType, balance);
                                    finalAmount += balance.TotalAmount;

                                }
                                else
                                {
                                    SummaryItem item = null;
                                    if (reportesorder.TryGetValue(trxType, out item))
                                    {
                                        //item = reportesorder[trxType];
                                        item.TotalAmount = reader.GetDecimal(1);
                                        item.TransactionCount = reader.GetInt32(2);
                                        finalAmount += item.TotalAmount;
                                    }
                                    else
                                    {

                                        SummaryItem balance = new SummaryItem()
                                        {
                                            TransactionType = reader.GetString(0),
                                            TotalAmount = reader.GetDecimal(1),
                                            TransactionCount = reader.GetInt32(2)
                                        };
                                        finalAmount += item.TotalAmount;
                                        reportesorder.Add(trxType, balance);
                                        logger.ErrorHigh(String.Concat(LOG_PREFIX, "ERROR AL GENERAR REPORTE NO SE ENCONTRO LA LLAVE EN EL DICCIONARIO ", trxType));

                                    }
                                }
                            }
                        }
                    }

                    summaries.AddRange(reportesorder.Values.ToList());

                }

            }
            catch (Exception e)
            {
                string Message = "ERROR TRANTANDO DE CONSULTAR EL RESÚMEN POR TRANSACCIÓN EN LA BASE DE DATOS DE BASE";
                logger.ErrorLow(() => TagValue.New().Exception(e).Message(Message));
                throw new Exception(Message, e);
            }

            return (summaries);
        }


        public static RolList GetRoles(string agentReference)
        {
            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            RolList roles = new RolList();
            try
            {
                var query =
                    String.Format(
                        (@" SELECT cast(ru.[rol_id] as int),r.rol_descabreviada
                              FROM [dbo].[RolUsuario] ru with (NOLOCK)
                              join [dbo].[Acceso] ac with (NOLOCK) on ac.usr_id=ru.usr_id
                              join [dbo].[Rol] r with (NOLOCK) on r.rol_id=ru.rol_id
                              where ac.acc_login='{0}'"), agentReference);

                sqlConnection.Open();
                var command = new SqlCommand(query, sqlConnection);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        roles.Add(new Rol()
                        {
                            RolId = reader.GetInt32(0),
                            RolName = reader.GetString(1)
                        });
                    }
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar los roles de un agente en la base de datos de Kinacu"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return (roles);
        }

        public static bool ChangeBranchStatus(string agentId)
        {
            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            var result = false;
            try
            {
                sqlConnection.Open();
                var mySqlCommand = new SqlCommand();
                mySqlCommand.Connection = sqlConnection;

                mySqlCommand.CommandText = "UPDATE agente SET age_estado = (CASE age_estado WHEN 'AC' THEN 'SU' WHEN 'SU' THEN 'AC' ELSE age_estado END) WHERE age_id=@branchid";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@branchid", agentId);

                if (mySqlCommand.ExecuteNonQuery().Equals(1))
                    result = true;
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de cambiar el status de un agente en la base de datos de Kinacu"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return result;
        }

        /*
        public static IEnumerable<ReportTransactionData> EnumerableTransactionsForReport(String agentReference, DateTime LowerLimit, DateTime UpperLimit, string transactionType)
        {

            var countryId = int.Parse(ConfigurationManager.AppSettings["CountryID"]);
            var platformId = 1;

            using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["BASEConnectionString"].ConnectionString))
            {



                sqlConnection.Open();
                RerportDataList transactions = new RerportDataList();
                var mySqlCommand = new SqlCommand();
                mySqlCommand.Connection = sqlConnection;

                #region trxsql
                String transactionsSql = @"TrxCurrentDay AS (
			SELECT
				t.primarycode AS 'TransactionId', 
				'recarga' AS 'TransactionType', 
				t.transactiondate AS 'TransactionDate',
         		t.amount AS 'Amount', 
				@Login AS 'Origin', 
				p.productname AS 'Product', 
				t.destinationnumber AS 'Destination' 
			FROM
				[dbo].[TransactionCurrentDay] AS t WITH(READUNCOMMITTED) 
				JOIN [dbo].[Product] AS p WITH(READUNCOMMITTED) ON (p.CountryId = t.CountryId AND p.PlatformId = t.PlatformId AND p.ProductId = t.ProductId) 
			WHERE 
				t.CountryId = @CountryId 
				AND t.PlatformId = @PlatformId 
				AND t.DateValue <= GETDATE()  
				AND t.DateValue > @UpperLimit 
				AND t.AccessTypeId = (SELECT AccessTypeId FROM AgeInfo) 
				AND t.UserId = (SELECT UserId FROM AgeInfo) 
				AND t.StatusId = 'AC' 
		),
		TrxThreeMonths AS (
			SELECT
				t.primarycode AS 'TransactionId', 
				'recarga' AS 'TransactionType', 
				t.transactiondate AS 'TransactionDate',
				t.amount AS 'Amount', 
				@Login AS 'Origin', 
				p.productname AS 'Product', 
				t.destinationnumber AS 'Destination' 
			FROM
				[dbo].[TransactionThreeMonths] AS t WITH(READUNCOMMITTED) 
				JOIN [dbo].[Product] AS p WITH(READUNCOMMITTED) ON (p.CountryId = t.CountryId AND p.PlatformId = t.PlatformId AND p.ProductId = t.ProductId) 
			WHERE 
				t.CountryId = @CountryId 
				AND t.PlatformId = @PlatformId 
				AND t.DateValue >= @LowerLimit
				AND t.DateValue <= @UpperLimit 
				AND t.AccessTypeId = (SELECT AccessTypeId FROM AgeInfo) 
				AND t.UserId = (SELECT UserId FROM AgeInfo) 
				AND t.StatusId = 'AC' 
		)";
                #endregion

                #region requestssql
                String requestsSql = @"Requests AS (
			SELECT
				 r.RequestId AS 'TransactionId'
				,rt.[Description] AS 'TransactionType'
				,r.DateApproved AS 'TransactionDate'
				,CASE WHEN r.RequestTypeId IN (205, 7010) THEN (r.Amount * -1) ELSE r.Amount END AS 'Amount' 
				,CASE WHEN r.RequestTypeId IN (205, 7010) THEN brec.BranchName ELSE bapl.BranchName END AS 'Origin'
				,'' AS 'Product'
				,CASE WHEN r.RequestTypeId IN (205, 7010) THEN bapl.BranchName ELSE brec.BranchName END AS 'Destination'
			FROM   
				[dbo].[Request] AS r WITH(READUNCOMMITTED) 
				JOIN [dbo].[RequestType] AS rt WITH(READUNCOMMITTED) ON (rt.RequestTypeId = r.RequestTypeId) 
				JOIN [dbo].[Branch] AS bapl WITH(READUNCOMMITTED) ON (bapl.CountryId = r.CountryId AND bapl.PlatformId = r.PlatformId AND bapl.BranchId = r.ApplicantBranchId) 
				JOIN [dbo].[Branch] AS brec WITH(READUNCOMMITTED) ON (brec.CountryId = r.CountryId AND brec.PlatformId = r.PlatformId AND brec.BranchId = r.RecipientBranchId) 
				--JOIN [dbo].[user] AS uapl WITH(READUNCOMMITTED) ON (uapl.CountryId = bapl.CountryId AND uapl.PlatformId = bapl.PlatformId AND uapl.BranchId = bapl.BranchId) 
				--JOIN [dbo].[Access] AS aapl WITH(READUNCOMMITTED) ON (aapl.CountryId = uapl.CountryId AND aapl.PlatformId = uapl.PlatformId AND aapl.UserId = uapl.UserId AND aapl.Login = @Login) 
			WHERE  
				r.CountryId = @CountryId 
				AND r.PlatformId = @PlatformId 
				AND r.DateApproved < DATEADD(DAY, 1, @UpperLimit)
				AND r.DateApproved >= @LowerLimit 
				AND (
					r.ApplicantBranchId = (SELECT BranchId FROM AgeInfo) 
					OR 
					r.RecipientBranchId = (SELECT BranchId FROM AgeInfo)
				)
				AND r.RequestTypeId NOT IN (204, 501) -- Compra | Movimiento de credito por comision
				AND r.StatusId = 'CE'
		)";
                #endregion

                #region commissionssql
                String commissionsSql = @"Commissions AS (
			SELECT
				 r.RequestId AS 'TransactionId'
				,'Comision' AS 'TransactionType'
				,r.DateApproved AS 'TransactionDate'
				,CASE WHEN r.RequestTypeId IN (205, 7010) THEN (r.Amount * -1) ELSE r.Amount END AS 'Amount' 
				,CASE WHEN r.RequestTypeId IN (205, 7010) THEN brec.BranchName ELSE bapl.BranchName END AS 'Origin'
				,'' AS 'Product'
				,CASE WHEN r.RequestTypeId IN (205, 7010) THEN bapl.BranchName ELSE brec.BranchName END AS 'Destination'
			FROM   
				[dbo].[Request] AS r WITH(READUNCOMMITTED) 
				JOIN [dbo].[RequestType] AS rt WITH(READUNCOMMITTED) ON (rt.RequestTypeId = r.RequestTypeId) 
				JOIN [dbo].[Branch] AS bapl WITH(READUNCOMMITTED) ON (bapl.CountryId = r.CountryId AND bapl.PlatformId = r.PlatformId AND bapl.BranchId = r.ApplicantBranchId) 
				JOIN [dbo].[Branch] AS brec WITH(READUNCOMMITTED) ON (brec.CountryId = r.CountryId AND brec.PlatformId = r.PlatformId AND brec.BranchId = r.RecipientBranchId) 
				--JOIN [dbo].[user] AS uapl WITH(READUNCOMMITTED) ON (uapl.CountryId = bapl.CountryId AND uapl.PlatformId = bapl.PlatformId AND uapl.BranchId = bapl.BranchId) 
				--JOIN [dbo].[Access] AS aapl WITH(READUNCOMMITTED) ON (aapl.CountryId = uapl.CountryId AND aapl.PlatformId = uapl.PlatformId AND aapl.UserId = uapl.UserId AND aapl.Login = @Login) 
			WHERE  
				r.CountryId = @CountryId 
				AND r.PlatformId = @PlatformId 
				AND r.DateApproved < DATEADD(DAY, 1, @UpperLimit)
				AND r.DateApproved >= @LowerLimit 
				AND r.RecipientBranchId = (SELECT BranchId FROM AgeInfo)
				AND r.RequestTypeId = 501 
				AND r.StatusId = 'CE'
		)";
                #endregion

                StringBuilder query = new StringBuilder(@"WITH AgeInfo AS (
			SELECT 
				TOP (1) u.BranchId, a.UserId, a.AccessTypeId 
			FROM 
				[dbo].[Access] AS a WITH(READUNCOMMITTED) 
				JOIN [dbo].[User] AS u WITH(READUNCOMMITTED) ON (u.CountryId = a.CountryId AND u.PlatformId = a.PlatformId AND u.UserId = a.UserId)
			WHERE a.CountryId = @CountryId AND a.PlatformId = @PlatformId AND a.[Login] = @Login
		), ");

                if (String.IsNullOrEmpty(transactionType))
                {
                    query.Append(transactionsSql + ",");
                    query.Append(requestsSql + ",");
                    query.Append(commissionsSql);
                    query.Append(@"SELECT * FROM TrxCurrentDay
				UNION ALL 
				SELECT * FROM TrxThreeMonths 
				UNION ALL 
				SELECT * FROM Requests 
				UNION ALL 
				SELECT * FROM Commissions 
			");
                }
                else if (transactionType.ToLower() == "recarga")
                {
                    query.Append(transactionsSql);
                    query.Append(@" SELECT * FROM TrxCurrentDay
				UNION ALL 
				SELECT * FROM TrxThreeMonths 
			");
                }
                else if (transactionType.ToLower() == "comision")
                {
                    query.Append(commissionsSql);
                    query.Append(@" SELECT * FROM Commissions");
                }
                else if (transactionType.ToLower() == "solicitud")
                {
                    query.Append(requestsSql);
                    query.Append(@" SELECT * FROM Requests");
                }

                query.Append(@" ORDER BY TransactionDate DESC");

                mySqlCommand.CommandText = query.ToString();
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@CountryId", countryId);
                mySqlCommand.Parameters.AddWithValue("@PlatformId", platformId);
                mySqlCommand.Parameters.AddWithValue("@Login", agentReference);
                mySqlCommand.Parameters.AddWithValue("@LowerLimit", LowerLimit.ToString("yyyyMMdd"));
                mySqlCommand.Parameters.AddWithValue("@UpperLimit", UpperLimit.ToString("yyyyMMdd"));
                //mySqlCommand.ExecuteScalar();
                using (var reader = mySqlCommand.ExecuteReader())
                {

                    if (reader.HasRows)
                        while (reader.Read())
                        {
                            yield return new ReportTransactionData()
                             {
                                 OriginalTransactionID = Convert.ToInt64(reader["TransactionId"]),
                                 TransactionType = reader["TransactionType"].ToString(),
                                 LastTimeModified = Convert.ToDateTime(reader["TransactionDate"]),
                                 Amount = Convert.ToDecimal(reader["Amount"]),
                                 //Origin Product Destination
                                 Origin = reader["Origin"].ToString(),
                                 Product = reader["Product"].ToString(),
                                 Destination = reader["Destination"].ToString()
                                 // RelatedParties = new RelatedParties() { reader.GetString(4), reader.GetString(5), reader.GetString(6) }
                             };
                        }
                }

            }    
        }*/

        //Ariel 2021-Mar-9
      //  public static RerportDataList TransactionsForReport(String agentReference, DateTime LowerLimit, DateTime UpperLimit, string transactionType, int Top)
      //  {
      //      RerportDataList transactions = new RerportDataList();

      //      StringBuilder query = new StringBuilder();
      //      try
      //      {


      //          using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["BASEConnectionString"].ConnectionString))
      //          {

      //              //indica si es un parametro valido para el tipo de transaccion
      //              bool isvalidTranType = true;


      //              bool iswithTop = Top > 0;
      //              string TOPstr = iswithTop ? string.Concat(" TOP ", Top) : " TOP 100 PERCENT  ";
      //              var countryId = int.Parse(ConfigurationManager.AppSettings["CountryID"]);
      //              var platformId = 1;

      //              sqlConnection.Open();
      //              var mySqlCommand = new SqlCommand();
      //              mySqlCommand.Connection = sqlConnection;

      //              #region trxsql
      //              String transactionsSql = @"TrxCurrentDay AS (
      //              SELECT
      //                      t.primarycode AS 'TransactionId', 
      //                      'recarga' AS 'TransactionType', 
      //                      t.transactiondate AS 'TransactionDate',
      //                      t.amount AS 'Amount', 
      //                      a.[Login] AS 'Origin', 
      //                      p.productname AS 'Product', 
      //                      t.destinationnumber AS 'Destination',
      //                      t.[ExternalCode] as 'RefOperadora' 
      //                  FROM
      //                      [dbo].[TransactionCurrentDay] AS t WITH(READUNCOMMITTED) 
      //                      JOIN [dbo].[Product] AS p WITH(READUNCOMMITTED) ON (p.CountryId = t.CountryId AND p.PlatformId = t.PlatformId AND p.ProductId = t.ProductId) 
      //                  	JOIN [dbo].[Access] AS a WITH(READUNCOMMITTED) ON (a.CountryId = t.CountryId AND a.PlatformId = t.PlatformId AND a.UserId = t.UserId AND a.AccessTypeId = t.AccessTypeId) 
      //                  WHERE 
      //                      t.CountryId = @CountryId 
      //                      AND t.PlatformId = @PlatformId 
      //                      AND t.DateValue >= CAST(GETDATE() AS DATE)  
      //                      AND t.DateValue <= @UpperLimit 
      //                      --AND t.AccessTypeId = (SELECT AccessTypeId FROM AgeInfo) 
      //                      --AND t.UserId = (SELECT UserId FROM AgeInfo) 
      //                      AND t.BranchId = (SELECT BranchId FROM AgeInfo) 
      //                      AND t.StatusId = 'AC' 
      //              ),
      //              TrxThreeMonths AS (
      //                  SELECT
      //                      t.primarycode AS 'TransactionId', 
      //                      'recarga' AS 'TransactionType', 
      //                      t.transactiondate AS 'TransactionDate',
      //                      t.amount AS 'Amount', 
      //                      a.[Login] AS 'Origin', 
      //                      p.productname AS 'Product', 
      //                      t.destinationnumber AS 'Destination',
      //                      t.[ExternalCode] as 'RefOperadora' 
      //                  FROM
      //                      [dbo].[TransactionThreeMonths] AS t WITH(READUNCOMMITTED) 
      //                      JOIN [dbo].[Product] AS p WITH(READUNCOMMITTED) ON (p.CountryId = t.CountryId AND p.PlatformId = t.PlatformId AND p.ProductId = t.ProductId) 
      //                  	JOIN [dbo].[Access] AS a WITH(READUNCOMMITTED) ON (a.CountryId = t.CountryId AND a.PlatformId = t.PlatformId AND a.UserId = t.UserId AND a.AccessTypeId = t.AccessTypeId) 
      //                  WHERE 
      //                      t.CountryId = @CountryId 
      //                      AND t.PlatformId = @PlatformId 
      //                      AND t.DateValue <= @UpperLimit 
      //                      AND t.DateValue >= @LowerLimit
      //                      --AND t.AccessTypeId = (SELECT AccessTypeId FROM AgeInfo) 
      //                      --AND t.UserId = (SELECT UserId FROM AgeInfo) 
      //                      AND t.BranchId = (SELECT BranchId FROM AgeInfo) 
      //                      AND t.StatusId = 'AC' 
      //              )";
      //              #endregion

      //              #region requestssql
      //              String requestsSql = @"Requests AS (
      //              SELECT
      //                   r.RequestId AS 'TransactionId'
      //                  ,(CASE 
      //                      WHEN (r.ApplicantBranchId = (SELECT BranchId FROM AgeInfo) AND r.RequestTypeId IN (201, 202)) THEN 'Dist Realizada' 
      //                      WHEN (r.RecipientBranchId = (SELECT BranchId FROM AgeInfo) AND r.RequestTypeId IN (201, 202)) THEN 'Dist Recibida'
      //                      WHEN r.RequestTypeId = 501 THEN (CASE WHEN (r.RecipientBranchId = (SELECT BranchId FROM AgeInfo)) THEN 'Comision Recibida' ELSE 'Comision Realizada' END)
      //                      WHEN r.RequestTypeId = 205 THEN (CASE WHEN (r.ApplicantBranchId = (SELECT BranchId FROM AgeInfo)) THEN 'Dist Recibida' ELSE 'Dist Realizada' END) 
      //                      ELSE rt.[Description]
      //                  END) AS 'TransactionType'
      //                  ,r.DateCreated AS 'TransactionDate'
      //                  ,r.Amount AS 'Amount' 
      //                  ,bapl.BranchName AS 'Origin'
      //                  ,'' AS 'Product'
      //                  ,brec.BranchName AS 'Destination',
      //                  '' as 'RefOperadora'
      //              FROM   
      //                  [dbo].[Request] AS r WITH(READUNCOMMITTED) 
      //                  JOIN [dbo].[RequestType] AS rt WITH(READUNCOMMITTED) ON (rt.RequestTypeId = r.RequestTypeId) 
      //                  JOIN [dbo].[Branch] AS bapl WITH(READUNCOMMITTED) ON (bapl.CountryId = r.CountryId AND bapl.PlatformId = r.PlatformId AND bapl.BranchId = r.ApplicantBranchId) 
      //                  JOIN [dbo].[Branch] AS brec WITH(READUNCOMMITTED) ON (brec.CountryId = r.CountryId AND brec.PlatformId = r.PlatformId AND brec.BranchId = r.RecipientBranchId) 
      //                  --JOIN [dbo].[user] AS uapl WITH(READUNCOMMITTED) ON (uapl.CountryId = bapl.CountryId AND uapl.PlatformId = bapl.PlatformId AND uapl.BranchId = bapl.BranchId) 
      //                  --JOIN [dbo].[Access] AS aapl WITH(READUNCOMMITTED) ON (aapl.CountryId = uapl.CountryId AND aapl.PlatformId = uapl.PlatformId AND aapl.UserId = uapl.UserId AND aapl.Login = @Login) 
      //              WHERE  
      //                  r.CountryId = @CountryId 
      //                  AND r.PlatformId = @PlatformId 
      //                  AND r.DateApproved < DATEADD(DAY, 1, @UpperLimit)
      //                  AND r.DateApproved >= @LowerLimit 
      //                  AND (
      //                      r.ApplicantBranchId = (SELECT BranchId FROM AgeInfo) 
      //                      OR 
      //                      r.RecipientBranchId = (SELECT BranchId FROM AgeInfo)
      //                  )
      //                  AND r.RequestTypeId <> 204 -- Compra
      //                  AND r.StatusId = 'CE'
      //               )";
      //              #endregion

      //              #region commissionssql
      //              String commissionsSql = @"Commissions AS (
      //              SELECT
      //                   r.RequestId AS 'TransactionId'
      //                  ,(CASE WHEN (r.RecipientBranchId = (SELECT BranchId FROM AgeInfo)) THEN 'Comision Recibida' ELSE 'Comision Realizada' END) AS 'TransactionType'
      //                  ,r.DateApproved AS 'TransactionDate'
      //                  ,r.Amount AS 'Amount' 
      //                  ,bapl.BranchName AS 'Origin'
      //                  ,'' AS 'Product'
      //                  ,brec.BranchName AS 'Destination',
      //                  '' as 'RefOperadora'
      //              FROM   
      //                  [dbo].[Request] AS r WITH(READUNCOMMITTED) 
      //                  JOIN [dbo].[RequestType] AS rt WITH(READUNCOMMITTED) ON (rt.RequestTypeId = r.RequestTypeId) 
      //                  JOIN [dbo].[Branch] AS bapl WITH(READUNCOMMITTED) ON (bapl.CountryId = r.CountryId AND bapl.PlatformId = r.PlatformId AND bapl.BranchId = r.ApplicantBranchId) 
      //                  JOIN [dbo].[Branch] AS brec WITH(READUNCOMMITTED) ON (brec.CountryId = r.CountryId AND brec.PlatformId = r.PlatformId AND brec.BranchId = r.RecipientBranchId) 
      //                  --JOIN [dbo].[user] AS uapl WITH(READUNCOMMITTED) ON (uapl.CountryId = bapl.CountryId AND uapl.PlatformId = bapl.PlatformId AND uapl.BranchId = bapl.BranchId) 
      //                  --JOIN [dbo].[Access] AS aapl WITH(READUNCOMMITTED) ON (aapl.CountryId = uapl.CountryId AND aapl.PlatformId = uapl.PlatformId AND aapl.UserId = uapl.UserId AND aapl.Login = @Login) 
      //              WHERE  
      //                  r.CountryId = @CountryId 
      //                  AND r.PlatformId = @PlatformId 
      //                  AND r.DateApproved < DATEADD(DAY, 1, @UpperLimit)
      //                  AND r.DateApproved >= @LowerLimit 
      //                  AND (
      //                      r.ApplicantBranchId = (SELECT BranchId FROM AgeInfo) 
      //                      OR 
      //                      r.RecipientBranchId = (SELECT BranchId FROM AgeInfo)
      //                  )
      //                  AND r.RequestTypeId = 501 
      //                  AND r.StatusId = 'CE'
      //              )";
      //              #endregion

      //              query = new StringBuilder(@"
      //              DECLARE @CountryId INT = @CountryId2;
      //              DECLARE @PlatformId INT = @PlatformId2;
      //              DECLARE @Login VARCHAR(25) = @Login2;
      //              DECLARE @LowerLimit DATE = @LowerLimit2;
      //              DECLARE @UpperLimit DATE = @UpperLimit2;
      //              WITH AgeInfo AS (
      //              SELECT 
      //                  TOP (1) u.BranchId, a.UserId, a.AccessTypeId 
      //              FROM 
      //                  [dbo].[Access] AS a WITH(READUNCOMMITTED) 
      //                  JOIN [dbo].[User] AS u WITH(READUNCOMMITTED) ON (u.CountryId = a.CountryId AND u.PlatformId = a.PlatformId AND u.UserId = a.UserId)
      //              WHERE a.CountryId = @CountryId AND a.PlatformId = @PlatformId AND a.[Login] = @Login
      //              ), ");


      //              switch (transactionType)
      //              {
      //                  default:

      //                      if (String.IsNullOrEmpty(transactionType))
      //                      {
      //                          // if (iswithTop)
      //                          {
      //                              query.Append(String.Concat(transactionsSql, ","));
      //                              query.Append(String.Concat(requestsSql, ","));
      //                              //query.Append(String.Concat(commissionsSql, ","));
      //                              query.Append(@"AllTransactions AS (SELECT * FROM TrxCurrentDay
				  //            UNION ALL 
				  //            SELECT  * FROM TrxThreeMonths
				  //            UNION ALL 
				  //            SELECT  * FROM Requests 
					 //)");
      //                              query.Append(String.Concat(@" SELECT ", TOPstr, @" * FROM AllTransactions"));

      //                          }
      //                          //                                else
      //                          //                                {
      //                          //                                    query.Append(transactionsSql + ",");
      //                          //                                    query.Append(requestsSql + ",");
      //                          //                                    query.Append(commissionsSql);
      //                          //                                    query.Append(@"SELECT  * FROM TrxCurrentDay
      //                          //                                                UNION ALL 
      //                          //                                                SELECT  * FROM TrxThreeMonths 
      //                          //                                                UNION ALL 
      //                          //                                                SELECT   * FROM Requests
      //                          //                                                UNION ALL 
      //                          //                                                SELECT  * FROM Commissions 
      //                          //			                                    ");
      //                          //                                }
      //                      }
      //                      else
      //                          isvalidTranType = false;
      //                      break;
      //                  case "recarga":

      //                      //if (iswithTop)
      //                      {
      //                          query.Append(transactionsSql + ",");
      //                          query.Append(@"AllTransactions AS (SELECT * FROM TrxCurrentDay
      //                                  UNION ALL 
      //                                  SELECT  * FROM TrxThreeMonths
      //                                  )");
      //                          query.Append(String.Concat(@" SELECT ", TOPstr, @" * FROM AllTransactions"));
      //                      }
      //                      //                            else
      //                      //                            {
      //                      //                                query.Append(transactionsSql);
      //                      //                                query.Append(@" SELECT  * FROM TrxCurrentDay
      //                      //				                    UNION ALL 
      //                      //				                    SELECT  * FROM TrxThreeMonths 
      //                      //			                        ");
      //                      //                            }


      //                      break;
      //                  case "comision":
      //                      query.Append(commissionsSql);
      //                      query.Append(String.Concat(@" SELECT ", TOPstr, @" * FROM Commissions"));
      //                      break;
      //                  case "solicitud":
      //                      query.Append(String.Concat(requestsSql));
      //                      query.Append(String.Concat(@" SELECT ", TOPstr, @" * FROM Requests"));
      //                      break;
      //              }

      //              if (isvalidTranType)
      //              {
      //                  query.Append(@" ORDER BY TransactionDate DESC");

      //                  mySqlCommand.CommandText = query.ToString();
      //                  mySqlCommand.Parameters.Clear();
      //                  mySqlCommand.Parameters.AddWithValue("@CountryId2", countryId);
      //                  mySqlCommand.Parameters.AddWithValue("@PlatformId2", platformId);
      //                  mySqlCommand.Parameters.AddWithValue("@Login2", agentReference);
      //                  mySqlCommand.Parameters.AddWithValue("@LowerLimit2", LowerLimit.ToString("yyyyMMdd"));
      //                  mySqlCommand.Parameters.AddWithValue("@UpperLimit2", UpperLimit.ToString("yyyyMMdd"));

      //                  /*
      //               t.primarycode AS 'TransactionId', 
      //                    'recarga' AS 'TransactionType', 
      //                    t.transactiondate AS 'TransactionDate',
      //                    t.amount AS 'Amount', 
      //                    @Login AS 'Origin', 
      //                    p.productname AS 'Product', 
      //                    t.destinationnumber AS 'Destination'  
                     
      //             */

      //                  using (var reader = mySqlCommand.ExecuteReader())
      //                  {


      //                      if (reader.HasRows)
      //                          while (reader.Read())
      //                          {
      //                              transactions.Add(new ReportTransactionData()
      //                              {
      //                                  OriginalTransactionID = Convert.ToInt64(reader["TransactionId"]),
      //                                  TransactionType = reader["TransactionType"].ToString(),
      //                                  LastTimeModified = Convert.ToDateTime(reader["TransactionDate"]),
      //                                  Amount = Convert.ToDecimal(reader["Amount"]),
      //                                  //Origin Product Destination
      //                                  Origin = reader["Origin"].ToString(),
      //                                  Product = reader["Product"].ToString(),
      //                                  Destination = reader["Destination"].ToString(),
      //                                  //Ronald
      //                                  RefOperadora = Convert.ToString(reader["RefOperadora"])
      //                                  //
      //                                  /*
      //                                  OriginalTransactionID = (long)reader[0],//Convert.ToInt64(reader["TransactionId"]),
      //                                  TransactionType = (string)reader[1],//reader["TransactionType"].ToString(),
      //                                  LastTimeModified = (DateTime)reader[2],//Convert.ToDateTime(reader["TransactionDate"]),
      //                                  Amount =(decimal)reader[3],// Convert.ToDecimal(reader["Amount"]),
      //                                  //Origin Product Destination
      //                                  Origin = (string)reader[4],//reader["Origin"].ToString(),
      //                                  Product = (string)reader[5],//reader["Product"].ToString(),
      //                                  Destination = (string)reader[6]//reader["Destination"].ToString(),
      //                                   */
      //                              });
      //                          }
      //                  }
      //              }

      //          }
      //      }
      //      catch (Exception e)
      //      {
      //          string message = "ERROR TRANTANDO DE CONSULTAR LAS ÚLTIMAS TRANSACCIONES DE UN AGENTE EN LA BASE DE DATOS DE BASE";
      //          logger.ErrorLow(() => TagValue.New().Exception(e).Message(String.Concat(message,
      //              ""
      //              //query.ToString()
      //              )));
      //          throw new Exception(message, e);
      //      }

      //      return (transactions);
      //  }

        /// <summary>
        /// Obtiene el reporte de transacciones para toda la jerarquía de agencias de un H2H
        /// </summary>
        /// <param name="agentReference">Acceso del usuario H2H</param>
        /// <param name="LowerLimit">Limite inferior</param>
        /// <param name="UpperLimit">Limite superior</param>
        /// <param name="transactionType">Tipo de transacción</param>
        /// <param name="Top">Número máximo de registros</param>
        /// <returns></returns>
        //public static RerportDataList TransactionsForReportH2H(string agentReference, DateTime LowerLimit, DateTime UpperLimit, string transactionType, int Top)
        //{
        //    RerportDataList transactions = new RerportDataList();

        //    StringBuilder query = new StringBuilder();
        //    try
        //    {
        //        // indica si es un parametro valido para el tipo de transaccion
        //        bool isvalidTranType = true;

        //        bool iswithTop = Top > 0;
        //        string TOPstr = iswithTop ? string.Concat(" TOP ", Top) : " TOP 100 PERCENT ";
        //        var countryId = int.Parse(ConfigurationManager.AppSettings["CountryID"]);
        //        var platformId = 1;

        //        #region trxsql
        //        string transactionsSql = @"TrxCurrentDay AS (
        //            SELECT
        //                t.primarycode AS 'TransactionId', 
        //                'recarga' AS 'TransactionType', 
        //                t.transactiondate AS 'TransactionDate',
        //                t.amount AS 'Amount', 
        //                b.BranchName AS 'Origin', 
        //                p.productname AS 'Product', 
        //                t.destinationnumber AS 'Destination' 
        //            FROM
        //                [dbo].[TransactionCurrentDay] AS t WITH(READUNCOMMITTED) 
        //                JOIN AgenciasJerarquia AS b WITH(READUNCOMMITTED) ON (b.CountryId = t.CountryId AND b.PlatformId = t.PlatformId AND b.BranchId = t.BranchId) 
        //                JOIN [dbo].[Product] AS p WITH(READUNCOMMITTED) ON (p.CountryId = t.CountryId AND p.PlatformId = t.PlatformId AND p.ProductId = t.ProductId) 
        //            WHERE 
        //                t.CountryId = @CountryId 
        //                AND t.PlatformId = @PlatformId 
        //                AND t.DateValue <= @UpperLimit 
        //                AND t.DateValue >= CAST(GETDATE() AS DATE) 
        //                AND t.StatusId = 'AC' 
        //        ),
        //        TrxThreeMonths AS (
        //            SELECT
        //                t.primarycode AS 'TransactionId', 
        //                'recarga' AS 'TransactionType', 
        //                t.transactiondate AS 'TransactionDate',
        //                t.amount AS 'Amount', 
        //                b.BranchName AS 'Origin', 
        //                p.productname AS 'Product', 
        //                t.destinationnumber AS 'Destination' 
        //            FROM
        //                [dbo].[TransactionThreeMonths] AS t WITH(READUNCOMMITTED) 
        //                JOIN AgenciasJerarquia AS b WITH(READUNCOMMITTED) ON (b.CountryId = t.CountryId AND b.PlatformId = t.PlatformId AND b.BranchId = t.BranchId)
        //                JOIN [dbo].[Product] AS p WITH(READUNCOMMITTED) ON (p.CountryId = t.CountryId AND p.PlatformId = t.PlatformId AND p.ProductId = t.ProductId) 
        //            WHERE 
        //                t.CountryId = @CountryId 
        //                AND t.PlatformId = @PlatformId 
        //                AND t.DateValue <= @UpperLimit 
        //                AND t.DateValue >= @LowerLimit 
        //                AND t.StatusId = 'AC' 
        //        )";
        //        #endregion

        //        #region requestssql
        //        // De esta manera no sabemos quién es el origen y quién es el destino
        //        string requestsSql = string.Empty;
        //        #endregion

        //        #region commissionssql
        //        // De esta manera no sabemos quién es el origen y quién es el destino
        //        string commissionsSql = string.Empty;
        //        #endregion

        //        query = new StringBuilder(@"
        //        DECLARE @CountryId INT = @CountryId2;
        //        DECLARE @PlatformId INT = @PlatformId2;
        //        DECLARE @Login VARCHAR(25) = @Login2;
        //        DECLARE @LowerLimit DATE = @LowerLimit2;
        //        DECLARE @UpperLimit DATE = @UpperLimit2;

        //        WITH AgeInfo AS (
        //            SELECT 
        //                TOP (1) u.BranchId, a.UserId, a.AccessTypeId, b.Lineage
        //            FROM 
        //                [dbo].[Access] AS a WITH(READUNCOMMITTED) 
        //                JOIN [dbo].[User] AS u WITH(READUNCOMMITTED) ON (u.CountryId = a.CountryId AND u.PlatformId = a.PlatformId AND u.UserId = a.UserId)
        //                JOIN [dbo].[Branch] AS b WITH(READUNCOMMITTED) ON (b.CountryId = u.CountryId AND b.PlatformId = u.PlatformId AND b.BranchId = u.BranchId)
        //            WHERE a.CountryId = @CountryId AND a.PlatformId = @PlatformId AND a.[Login] = @Login
        //        ),
        //        AgenciasJerarquia AS (
        //            SELECT
        //                 B.CountryId
        //                ,B.PlatformId
        //                ,B.BranchId
        //                ,B.ParentBranchId
        //                ,B.BranchName
        //            FROM
        //                [dbo].[Branch] AS B WITH(READUNCOMMITTED)
        //            WHERE
        //                B.CountryId = @CountryId AND B.PlatformId = @PlatformId AND B.Lineage LIKE CONCAT((SELECT Lineage FROM AgeInfo), '%')
        //        ), ");

        //        switch (transactionType)
        //        {
        //            default:
        //                if (string.IsNullOrEmpty(transactionType))
        //                {
        //                    // if (iswithTop)
        //                    {
        //                        query.Append(String.Concat(transactionsSql, ","));
        //                        // query.Append(String.Concat(requestsSql, ","));
        //                        // query.Append(String.Concat(commissionsSql, ","));
        //                        query.Append(@"AllTransactions AS (SELECT * FROM TrxCurrentDay
        //                            UNION ALL 
        //                            SELECT  * FROM TrxThreeMonths
        //                            -- UNION ALL 
        //                            -- SELECT  * FROM Requests 
        //                            )");
        //                        query.Append(String.Concat(@" SELECT ", TOPstr, @" * FROM AllTransactions"));
        //                    }
        //                }
        //                else
        //                    isvalidTranType = false;
        //                break;
        //            case "recarga":
        //                // if (iswithTop)
        //                {
        //                    query.Append(transactionsSql + ",");
        //                    query.Append(@"AllTransactions AS (SELECT * FROM TrxCurrentDay
        //                            UNION ALL 
        //                            SELECT  * FROM TrxThreeMonths
        //                            )");
        //                    query.Append(String.Concat(@" SELECT ", TOPstr, @" * FROM AllTransactions"));
        //                }
        //                break;
        //        }

        //        if (isvalidTranType)
        //        {
        //            query.Append(@" ORDER BY TransactionDate DESC");

        //            using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["BASEConnectionString"].ConnectionString))
        //            {
        //                sqlConnection.Open();
        //                var mySqlCommand = new SqlCommand();
        //                mySqlCommand.Connection = sqlConnection;

        //                mySqlCommand.CommandText = query.ToString();
        //                mySqlCommand.Parameters.Clear();
        //                mySqlCommand.Parameters.AddWithValue("@CountryId2", countryId);
        //                mySqlCommand.Parameters.AddWithValue("@PlatformId2", platformId);
        //                mySqlCommand.Parameters.AddWithValue("@Login2", agentReference);
        //                mySqlCommand.Parameters.AddWithValue("@LowerLimit2", LowerLimit.ToString("yyyyMMdd"));
        //                mySqlCommand.Parameters.AddWithValue("@UpperLimit2", UpperLimit.ToString("yyyyMMdd"));

        //                using (var reader = mySqlCommand.ExecuteReader())
        //                {
        //                    if (reader.HasRows)
        //                    {
        //                        while (reader.Read())
        //                        {
        //                            transactions.Add(new ReportTransactionData()
        //                            {
        //                                OriginalTransactionID = Convert.ToInt64(reader["TransactionId"]),
        //                                TransactionType = reader["TransactionType"].ToString(),
        //                                LastTimeModified = Convert.ToDateTime(reader["TransactionDate"]),
        //                                Amount = Convert.ToDecimal(reader["Amount"]),
        //                                //Origin Product Destination
        //                                Origin = reader["Origin"].ToString(),
        //                                Product = reader["Product"].ToString(),
        //                                Destination = reader["Destination"].ToString()
        //                                /*
        //                                OriginalTransactionID = (long)reader[0],//Convert.ToInt64(reader["TransactionId"]),
        //                                TransactionType = (string)reader[1],//reader["TransactionType"].ToString(),
        //                                LastTimeModified = (DateTime)reader[2],//Convert.ToDateTime(reader["TransactionDate"]),
        //                                Amount =(decimal)reader[3],// Convert.ToDecimal(reader["Amount"]),
        //                                //Origin Product Destination
        //                                Origin = (string)reader[4],//reader["Origin"].ToString(),
        //                                Product = (string)reader[5],//reader["Product"].ToString(),
        //                                Destination = (string)reader[6]//reader["Destination"].ToString(),
        //                                    */
        //                            });
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        string message = "ERROR TRANTANDO DE CONSULTAR LAS ÚLTIMAS TRANSACCIONES DE UN H2H EN LA BASE DE DATOS BASE";
        //        logger.ErrorLow(() => TagValue.New().Exception(e).Message(string.Concat(message, string.Empty)));
        //        throw new Exception(message, e);
        //    }

        //    return (transactions);
        //}

        public static TransactionList LastTransactions(String agentReference, int count, string transactionType)
        {
            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            var countryId = int.Parse(ConfigurationManager.AppSettings["CountryID"]);
            var platformId = 1;

            TransactionList transactions = new TransactionList();
            try
            {
                sqlConnection.Open();
                var mySqlCommand = new SqlCommand();
                mySqlCommand.Connection = sqlConnection;

                mySqlCommand.CommandText = String.Concat(@"select ag.age_id from Acceso a with (NOLOCK)
                                                            join Usuario u with (NOLOCK) on a.usr_id=u.usr_id
                                                            join Agente ag with (NOLOCK) on ag.age_id=u.age_id
                                                            where a.acc_login=@loginname");
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@loginname", agentReference);

                object objagentId = mySqlCommand.ExecuteScalar();
                string agentId = string.Empty;
                if (objagentId != null)
                    agentId = objagentId.ToString();
                else
                {
                    string _m = "NO SE PUDO ENCONTRAR BRANCH ID PARA LOS PARAMETROS ESTABLECIDOS";
                    logger.ErrorLow(() => TagValue.New().Message(_m).Tag("PARAMS").Value(String.Concat("@loginname:", agentReference, ",@countryid:", countryId, "@platformid:", platformId)));
                    throw new Exception(_m);
                }

                string queryBase = "";
                if (String.IsNullOrEmpty(transactionType))
                {
                    queryBase = @"   

declare @dateDiff as int = isnull((SELECT  CAST(par_valor as int) FROM [Parametro] WITH(NOLOCK) WHERE par_id ='TimeDifference'),0)
declare @date as datetime = dateadd(minute,@dateDiff,getdate())

select top {0} * from 
                                            (
                                                select top {0} OriginalTransactionID,TransactionType,LastTimeModified,Amount,Origen,Producto,DestinationNumber from
                                                (
                                                    select top {0} cast(v.vta_id as varchar)'OriginalTransactionID',
                                                        'Recarga' 'TransactionType',
                                                        v.vta_fecha'LastTimeModified',
                                                        v.vta_monto'Amount',
                                                        a.acc_login'Origen',
                                                        p.prd_nombre'Producto',
                                                        cast(v.vta_destino as varchar)'DestinationNumber' 
                                                    from venta v with (NOLOCK) 
                                                        join recarga r with (NOLOCK) on v.vta_id=r.vta_id
                                                        join acceso a with (NOLOCK) on a.usr_id=v.usr_id and a.tac_id=v.tac_id
                                                        join producto p with (NOLOCK) on v.prd_id=p.prd_id
                                                    where a.acc_login='{1}' and v.vta_estado='AC' and r.rec_estado='OK'
                                                        and v.vta_fecha between dateadd(day,-1,@date) and @date
                                                    order by 1 desc
                                                ) t order by LastTimeModified desc
                
                                                UNION
                
                                                select top {0} OriginalTransactionID,TransactionType,LastTimeModified,Amount,Origen,Producto,DestinationNumber from
                                                (
                                                    select top {0} cast (max(s.exeId) as varchar)'OriginalTransactionID',
                                                        'Comision' 'TransactionType',   
                                                        a.exeFecha 'LastTimeModified',           
                                                        sum(salValor) 'Amount',
	                                                    ep.entDenominacion 'Origen',
                                                        f.prdNombre 'Producto',
                                                        e.entDenominacion 'DestinationNumber'
                                                    from {2}..kmmSalida s with (NOLOCK)
                                                        join {2}..kmmEntidad e  with (NOLOCK) on   s.entId = e.entId  
                                                        join {2}..kmmEntidad ep  with (NOLOCK) on   ep.entId = e.entIdContenedora  
                                                        join {2}..kmmProducto f with (NOLOCK)  on s.prdId = f.prdId
                                                        join {2}..kmmEjecucion a with (NOLOCK)  on a.exeId = s.exeId      
                                                    where s.entId in (select entId from {2}..kmmagente with (NOLOCK)  where ageId={3}) 
                                                        and a.exeFecha between dateadd(day,-1,getdate()) and getdate()
                                                    group by a.exeFecha,f.prdNombre,ep.entDenominacion,f.prdNombre,e.entDenominacion
		                                            order by 1 desc
                                                ) t order by LastTimeModified desc
                                                
                                                UNION
                
                                                select top {0} OriginalTransactionID,TransactionType,LastTimeModified,Amount,Origen,Producto,DestinationNumber from
                                                (
                                                    select top {0} cast(sp.sprId as varchar)'OriginalTransactionID',
                                                        st.sltDescripcion 'TransactionType',           
                                                        sp.sprFechaAprobacion 'LastTimeModified',             
                                                        sp.sprImporteSolicitud 'Amount',
                                                        a.age_nombre  'Origen',
                                                        'Multiproducto' 'Producto',
                                                        b.age_nombre  'DestinationNumber'
                                                    FROM KlgSolicitudProducto sp with(nolock),
                                                        KlgSolicitudTipo st with (NOLOCK),
                                                        Agente a with (NOLOCK),
                                                        Agente b with (NOLOCK), 
                                                        Agente d with (NOLOCK),
                                                        Agente c with (NOLOCK)       
                                                    WHERE sp.sprFecha BETWEEN  dateadd(day,-1,@date) AND convert(datetime,@date,101) + 1 
                                                        and not st.sltId in(204,501)
                                                        and sp.sltId = st.sltId
                                                        and sp.ageIdBodegaOrigen = a.age_id
                                                        and sp.ageIdBodegaDestino = b.age_id
                                                        and sp.ageIdSolicitante = c.age_id
                                                        and sp.ageIdDestinatario = d.age_id
                                    	                and sp.sprEstado = 'CE'
                                                        and (a.age_id = {3} or b.age_id = {3} or c.age_id = {3} or d.age_id = {3})
		                                            order by 1 desc
                                                ) t order by LastTimeModified desc
                                            ) t order by LastTimeModified desc
                                        ";
                }
                else if (transactionType.ToLower() == "recarga")
                {
                    queryBase = @"  

declare @dateDiff as int = isnull((SELECT  CAST(par_valor as int) FROM [Parametro] WITH(NOLOCK) WHERE par_id ='TimeDifference'),0)
declare @date as datetime = dateadd(minute,@dateDiff,getdate())

                                            select top {0} * from 
                                            (
                                                select top {0} OriginalTransactionID,TransactionType,LastTimeModified,Amount,Origen,Producto,DestinationNumber from
                                                (
                                                    select top {0} cast(v.vta_id as varchar)'OriginalTransactionID',
                                                        'Recarga' 'TransactionType',
                                                        v.vta_fecha'LastTimeModified',
                                                        v.vta_monto'Amount',
                                                        a.acc_login'Origen',
                                                        p.prd_nombre'Producto',
                                                        cast(v.vta_destino as varchar)'DestinationNumber' 
                                                    from venta v with (NOLOCK) 
                                                        join recarga r with (NOLOCK) on v.vta_id=r.vta_id
                                                        join acceso a with (NOLOCK) on a.usr_id=v.usr_id and a.tac_id=v.tac_id
                                                        join producto p with (NOLOCK) on v.prd_id=p.prd_id
                                                    where a.acc_login='{1}' and v.vta_estado='AC' and r.rec_estado='OK'
                                                        and v.vta_fecha between dateadd(day,-1, @date ) and  @date 
                                                    order by 1 desc
                                                ) t order by LastTimeModified desc
                                            ) t order by LastTimeModified desc
                                        ";
                }
                else if (transactionType.ToLower() == "comision")
                {
                    queryBase = @" 
declare @dateDiff as int = isnull((SELECT  CAST(par_valor as int) FROM [Parametro] WITH(NOLOCK) WHERE par_id ='TimeDifference'),0)
declare @date as datetime = dateadd(minute,@dateDiff,getdate())

                                            select top {0} * from 
                                            (
                                                select top {0} OriginalTransactionID,TransactionType,LastTimeModified,Amount,Origen,Producto,DestinationNumber from
                                                (
                                                    select top {0} cast (s.exeId as varchar)'OriginalTransactionID',
                                                        'Comision' 'TransactionType',   
                                                        a.exeFecha 'LastTimeModified',           
                                                        salValor 'Amount',
                                                        e.entDenominacion 'Origen',
                                                        f.prdNombre 'Producto',
                                                        ltrim(rtrim(substring(salDetalle,charindex(':',salDetalle)+1,charindex(',',salDetalle)-charindex(':',salDetalle)-1))) 'DestinationNumber'
                                                    from {2}..kmmSalida s with (NOLOCK)
                                                        join {2}..kmmEntidad e  with (NOLOCK) on   s.entId = e.entId  
                                                        join {2}..kmmProducto f with (NOLOCK)  on s.prdId = f.prdId
                                                        join {2}..kmmEjecucion a with (NOLOCK)  on a.exeId = s.exeId      
                                                    where s.entId in (select entId from {2}..kmmagente with (NOLOCK)  where ageId={3}) 
                                                        and a.exeFecha between dateadd(day,-1, @date ) and  @date 
		                                            order by 1 desc
                                                ) t order by LastTimeModified desc
                                            ) t order by LastTimeModified desc
                                        ";
                }
                else if (transactionType.ToLower() == "asignacion")
                {
                    queryBase = @"
declare @dateDiff as int = isnull((SELECT  CAST(par_valor as int) FROM [Parametro] WITH(NOLOCK) WHERE par_id ='TimeDifference'),0)
declare @date as datetime = dateadd(minute,@dateDiff,getdate())


                                            select top {0} * from 
                                            (
                                                select top {0} OriginalTransactionID,TransactionType,LastTimeModified,Amount,Origen,Producto,DestinationNumber from
                                                (
                                                    select top {0} cast(sp.sprId as varchar)'OriginalTransactionID',
                                                        st.sltDescripcion 'TransactionType',           
                                                        sp.sprFechaAprobacion 'LastTimeModified',             
                                                        sp.sprImporteSolicitud 'Amount',
                                                        a.age_nombre  'Origen',
                                                        'Multiproducto' 'Producto',
                                                        b.age_nombre  'DestinationNumber'
                                                    FROM KlgSolicitudProducto sp with(nolock),
                                                        KlgSolicitudTipo st with (NOLOCK),
                                                        Agente a with (NOLOCK),
                                                        Agente b with (NOLOCK), 
                                                        Agente d with (NOLOCK),
                                                        Agente c with (NOLOCK)       
                                                    WHERE sp.sprFecha BETWEEN  dateadd(day,-1, @date ) AND convert(datetime, @date ,101) + 1 
                                                        and not st.sltId in(204,501)
                                                        and sp.sltId = st.sltId
                                                        and sp.ageIdBodegaOrigen = a.age_id
                                                        and sp.ageIdBodegaDestino = b.age_id
                                                        and sp.ageIdSolicitante = c.age_id
                                                        and sp.ageIdDestinatario = d.age_id
                                    	                and sp.sprEstado = 'CE'
                                                        and (a.age_id = {3} or b.age_id = {3} or c.age_id = {3} or d.age_id = {3})
                                                        and st.sltDescripcion = 'asignacion'
		                                            order by 1 desc
                                                ) t order by LastTimeModified desc
                                            ) t order by LastTimeModified desc
                                        ";
                }
                else if (transactionType.ToLower() == "solicitud")
                {
                    queryBase = @"

declare @dateDiff as int = isnull((SELECT  CAST(par_valor as int) FROM [Parametro] WITH(NOLOCK) WHERE par_id ='TimeDifference'),0)
declare @date as datetime = dateadd(minute,@dateDiff,getdate())


                                            select top {0} * from 
                                            (
                                                select top {0} OriginalTransactionID,TransactionType,LastTimeModified,Amount,Origen,Producto,DestinationNumber from
                                                (
                                                    select top {0} cast(sp.sprId as varchar)'OriginalTransactionID',
                                                        st.sltDescripcion 'TransactionType',           
                                                        sp.sprFechaAprobacion 'LastTimeModified',             
                                                        sp.sprImporteSolicitud 'Amount',
                                                        a.age_nombre  'Origen',
                                                        'Multiproducto' 'Producto',
                                                        b.age_nombre  'DestinationNumber'
                                                    FROM KlgSolicitudProducto sp with(nolock),
                                                        KlgSolicitudTipo st with (NOLOCK),
                                                        Agente a with (NOLOCK),
                                                        Agente b with (NOLOCK), 
                                                        Agente d with (NOLOCK),
                                                        Agente c with (NOLOCK)       
                                                    WHERE sp.sprFecha BETWEEN  dateadd(day,-1,GETDATE()) AND convert(datetime,GETDATE(),101) + 1 
                                                        and not st.sltId in(204,501)
                                                        and sp.sltId = st.sltId
                                                        and sp.ageIdBodegaOrigen = a.age_id
                                                        and sp.ageIdBodegaDestino = b.age_id
                                                        and sp.ageIdSolicitante = c.age_id
                                                        and sp.ageIdDestinatario = d.age_id
                                    	                and sp.sprEstado = 'CE'
                                                        and (a.age_id = {3} or b.age_id = {3} or c.age_id = {3} or d.age_id = {3})
                                                        and st.sltDescripcion = 'solicitud'
		                                            order by 1 desc
                                                ) t order by LastTimeModified desc
                                            ) t order by LastTimeModified desc
                                        ";
                }

                #region newBadQueries
                //if (String.IsNullOrEmpty(transactionType))
                //    mySqlCommand.CommandText = String.Concat("SELECT top ", count.Equals(0) ? "10" : count.ToString(), " * FROM ",
                //                                            "(",
                //                                                "SELECT t.OriginalTransactionID,t.TransactionType,t.LastTimeModified,t.Amount,t.Origin,t.Product,t.DestinationNumber FROM ",
                //                                                "(",
                //                                                    "SELECT t.primarycode'OriginalTransactionID','Recarga' 'TransactionType',t.transactiondate'LastTimeModified',t.amount'Amount',a.[login]'Origin',p.productname'Product',t.DestinationNumber ",
                //                                                    "FROM [transactioncurrentday] t WITH (NOLOCK) ",
                //                                                    "JOIN [Access] a WITH (NOLOCK) on a.userid=t.userid and a.accesstypeid=t.accesstypeid and a.countryid=t.countryid and a.platformid=t.platformid ",
                //                                                    "JOIN [Product] p WITH (NOLOCK) on p.productid=t.productid and p.countryid=t.countryid and p.platformid=t.platformid ",
                //                                                    "WHERE t.transactiondate>=convert(date,getdate()) and a.[login]=@loginname and t.statusid='AC' and t.countryid=@countryid and t.platformid=@platformid ",
                //                                                "UNION ",
                //                                                    "SELECT t.primarycode'OriginalTransactionID','Recarga' 'TransactionType',t.transactiondate'LastTimeModified',t.amount'Amount',a.[login]'Origin',p.productname'Product',t.DestinationNumber ",
                //                                                    "FROM [transactionthreemonths] t WITH (NOLOCK) ",
                //                                                    "JOIN [Access] a WITH (NOLOCK) on a.userid=t.userid and a.accesstypeid=t.accesstypeid and a.countryid=t.countryid and a.platformid=t.platformid ",
                //                                                    "JOIN [Product] p WITH (NOLOCK) on p.productid=t.productid and p.countryid=t.countryid and p.platformid=t.platformid ",
                //                                                    "WHERE a.[login]=@loginname and t.statusid='AC' and t.transactiondate>convert(date,dateadd(day,-30,getdate())) and t.countryid=@countryid and t.platformid=@platformid ",
                //                                                ") t ",
                //                                            "UNION ",
                //                                                "SELECT r.requestid'OriginalTransactionID',rt.[description]'TransactionType',r.datecreated'LastTimeModified',r.amount'Amount',bapl.branchname'Origin','N/A' 'Product',brec.branchname'DestinationNumber' ",
                //                                                "FROM request r WITH (NOLOCK) ",
                //                                                "JOIN requesttype rt WITH (NOLOCK) ON r.requesttypeid=rt.requesttypeid ",
                //                                                "JOIN branch bapl WITH (NOLOCK) ON bapl.branchid=r.applicantbranchid and bapl.countryid=r.countryid and bapl.platformid=r.platformid ",
                //                                                "JOIN [user] uapl WITH (NOLOCK) ON uapl.branchid=bapl.branchid and uapl.countryid=bapl.countryid and uapl.PlatformId=bapl.platformid ",
                //                                                "JOIN access aapl WITH (NOLOCK) ON aapl.userid=uapl.userid and aapl.countryid=uapl.countryid and aapl.platformid=uapl.platformid and aapl.login=@loginname ",
                //                                                "JOIN branch brec WITH (NOLOCK) ON brec.branchid=r.recipientbranchid and brec.countryid=r.countryid and brec.platformid=r.platformid ",
                //                                                "WHERE r.countryid=@countryid AND r.platformid=@platformid AND r.requesttypeid not in (204,501) and r.datecreated>convert(date,dateadd(day,-30,getdate())) ",
                //                                            "UNION ",
                //                                                "SELECT r.requestid'OriginalTransactionID',rt.[description]'TransactionType',r.datecreated'LastTimeModified',r.amount'Amount',bapl.branchname'Origin','N/A' 'Product',brec.branchname'DestinationNumber' ",
                //                                                "FROM request r WITH (NOLOCK) ",
                //                                                "JOIN requesttype rt WITH (NOLOCK) ON r.requesttypeid=rt.requesttypeid ",
                //                                                "JOIN branch bapl WITH (NOLOCK) ON bapl.branchid=r.applicantbranchid and bapl.countryid=r.countryid and bapl.platformid=r.platformid ",
                //                                                "JOIN [user] uapl WITH (NOLOCK) ON uapl.branchid=bapl.branchid and uapl.countryid=bapl.countryid and uapl.PlatformId=bapl.platformid ",
                //                                                "JOIN access aapl WITH (NOLOCK) ON aapl.userid=uapl.userid and aapl.countryid=uapl.countryid and aapl.platformid=uapl.platformid and aapl.login=@loginname ",
                //                                                "JOIN branch brec WITH (NOLOCK) ON brec.branchid=r.recipientbranchid and brec.countryid=r.countryid and brec.platformid=r.platformid ",
                //                                                "WHERE r.countryid=@countryid AND r.platformid=@platformid AND r.requesttypeid not in (204,501) and r.datecreated>convert(date,dateadd(day,-30,getdate())) ",
                //                                            "UNION ",
                //                                                "SELECT r.requestid'OriginalTransactionID','Comisión' 'TransactionType',r.datecreated'LastTimeModified',r.amount'Amount',bapl.branchname'Origin','N/A' 'Product',brec.branchname'DestinationNumber' ",
                //                                                "FROM request r WITH (NOLOCK) ",
                //                                                "JOIN requesttype rt WITH (NOLOCK) ON r.requesttypeid=rt.requesttypeid ",
                //                                                "JOIN branch bapl WITH (NOLOCK) ON bapl.branchid=r.applicantbranchid and bapl.countryid=r.countryid and bapl.platformid=r.platformid ",
                //                                                "JOIN branch brec WITH (NOLOCK) ON brec.branchid=r.recipientbranchid and brec.countryid=r.countryid and brec.platformid=r.platformid ",
                //                                                "JOIN [user] urec WITH (NOLOCK) ON urec.branchid=brec.branchid and urec.countryid=brec.countryid and urec.PlatformId=brec.platformid ",
                //                                                "JOIN access arec WITH (NOLOCK) ON arec.userid=urec.userid and arec.countryid=urec.countryid and arec.platformid=urec.platformid and arec.login=@loginname ",
                //                                                "WHERE r.countryid=@countryid AND r.platformid=@platformid AND r.requesttypeid in (501) and r.datecreated>convert(date,dateadd(day,-30,getdate())) ",
                //                                            ") t ",
                //                                            "ORDER BY t.LastTimeModified DESC");
                //else if (transactionType.ToLower() == "recarga")
                //    mySqlCommand.CommandText = String.Concat("SELECT top ", count.Equals(0) ? "10" : count.ToString(), " * FROM ",
                //                                            "(",
                //                                                "SELECT t.OriginalTransactionID,t.TransactionType,t.LastTimeModified,t.Amount,t.Origin,t.Product,t.DestinationNumber FROM ",
                //                                                "(",
                //                                                    "SELECT t.primarycode'OriginalTransactionID','Recarga' 'TransactionType',t.transactiondate'LastTimeModified',t.amount'Amount',a.[login]'Origin',p.productname'Product',t.DestinationNumber ",
                //                                                    "FROM [transactioncurrentday] t WITH (NOLOCK) ",
                //                                                    "JOIN [Access] a WITH (NOLOCK) on a.userid=t.userid and a.accesstypeid=t.accesstypeid and a.countryid=t.countryid and a.platformid=t.platformid ",
                //                                                    "JOIN [Product] p WITH (NOLOCK) on p.productid=t.productid and p.countryid=t.countryid and p.platformid=t.platformid ",
                //                                                    "WHERE t.transactiondate>=convert(date,getdate()) and a.[login]=@loginname and t.statusid='AC' and t.countryid=@countryid and t.platformid=@platformid ",
                //                                                "UNION ",
                //                                                    "SELECT t.primarycode'OriginalTransactionID','Recarga' 'TransactionType',t.transactiondate'LastTimeModified',t.amount'Amount',a.[login]'Origin',p.productname'Product',t.DestinationNumber ",
                //                                                    "FROM [transactionthreemonths] t WITH (NOLOCK) ",
                //                                                    "JOIN [Access] a WITH (NOLOCK) on a.userid=t.userid and a.accesstypeid=t.accesstypeid and a.countryid=t.countryid and a.platformid=t.platformid ",
                //                                                    "JOIN [Product] p WITH (NOLOCK) on p.productid=t.productid and p.countryid=t.countryid and p.platformid=t.platformid ",
                //                                                    "WHERE a.[login]=@loginname and t.statusid='AC' and t.transactiondate>convert(date,dateadd(day,-30,getdate())) and t.countryid=@countryid and t.platformid=@platformid ",
                //                                                ") t ",
                //                                            ") t ",
                //                                            "ORDER BY t.LastTimeModified DESC");
                //else if (transactionType.ToLower() == "comision")
                //    mySqlCommand.CommandText = String.Concat("SELECT top ", count.Equals(0) ? "10" : count.ToString(), " * FROM ",
                //                                            "(",
                //                                                "SELECT r.requestid'OriginalTransactionID','Comisión' 'TransactionType',r.datecreated'LastTimeModified',r.amount'Amount',bapl.branchname'Origin','N/A' 'Product',brec.branchname'DestinationNumber' ",
                //                                                "FROM request r WITH (NOLOCK) ",
                //                                                "JOIN requesttype rt WITH (NOLOCK) ON r.requesttypeid=rt.requesttypeid ",
                //                                                "JOIN branch bapl WITH (NOLOCK) ON bapl.branchid=r.applicantbranchid and bapl.countryid=r.countryid and bapl.platformid=r.platformid ",
                //                                                "JOIN [user] uapl WITH (NOLOCK) ON uapl.branchid=bapl.branchid and uapl.countryid=bapl.countryid and uapl.PlatformId=bapl.platformid ",
                //                                                "JOIN access aapl WITH (NOLOCK) ON aapl.userid=uapl.userid and aapl.countryid=uapl.countryid and aapl.platformid=uapl.platformid and aapl.login=@loginname ",
                //                                                "JOIN branch brec WITH (NOLOCK) ON brec.branchid=r.recipientbranchid and brec.countryid=r.countryid and brec.platformid=r.platformid ",
                //                                                "WHERE r.countryid=@countryid AND r.platformid=@platformid AND r.requesttypeid in (501) and r.datecreated>convert(date,dateadd(day,-30,getdate())) ",
                //                                            "UNION ",
                //                                                "SELECT r.requestid'OriginalTransactionID','Comisión' 'TransactionType',r.datecreated'LastTimeModified',r.amount'Amount',bapl.branchname'Origin','N/A' 'Product',brec.branchname'DestinationNumber' ",
                //                                                "FROM request r WITH (NOLOCK) ",
                //                                                "JOIN requesttype rt WITH (NOLOCK) ON r.requesttypeid=rt.requesttypeid ",
                //                                                "JOIN branch bapl WITH (NOLOCK) ON bapl.branchid=r.applicantbranchid and bapl.countryid=r.countryid and bapl.platformid=r.platformid ",
                //                                                "JOIN branch brec WITH (NOLOCK) ON brec.branchid=r.recipientbranchid and brec.countryid=r.countryid and brec.platformid=r.platformid ",
                //                                                "JOIN [user] urec WITH (NOLOCK) ON urec.branchid=brec.branchid and urec.countryid=brec.countryid and urec.PlatformId=brec.platformid ",
                //                                                "JOIN access arec WITH (NOLOCK) ON arec.userid=urec.userid and arec.countryid=urec.countryid and arec.platformid=urec.platformid and arec.login=@loginname ",
                //                                                "WHERE r.countryid=@countryid AND r.platformid=@platformid AND r.requesttypeid in (501) and r.datecreated>convert(date,dateadd(day,-30,getdate())) ",
                //                                            ") t ",
                //                                            "ORDER BY t.LastTimeModified DESC");
                //else if (transactionType.ToLower() == "solicitud")
                //    mySqlCommand.CommandText = String.Concat("SELECT top ", count.Equals(0) ? "10" : count.ToString(), " * FROM ",
                //                                            "(",
                //                                                "SELECT r.requestid'OriginalTransactionID',rt.[description]'TransactionType',r.datecreated'LastTimeModified',r.amount'Amount',bapl.branchname'Origin','N/A' 'Product',brec.branchname'DestinationNumber' ",
                //                                                "FROM request r WITH (NOLOCK) ",
                //                                                "JOIN requesttype rt WITH (NOLOCK) ON r.requesttypeid=rt.requesttypeid ",
                //                                                "JOIN branch bapl WITH (NOLOCK) ON bapl.branchid=r.applicantbranchid and bapl.countryid=r.countryid and bapl.platformid=r.platformid ",
                //                                                "JOIN [user] uapl WITH (NOLOCK) ON uapl.branchid=bapl.branchid and uapl.countryid=bapl.countryid and uapl.PlatformId=bapl.platformid ",
                //                                                "JOIN access aapl WITH (NOLOCK) ON aapl.userid=uapl.userid and aapl.countryid=uapl.countryid and aapl.platformid=uapl.platformid and aapl.login=@loginname ",
                //                                                "JOIN branch brec WITH (NOLOCK) ON brec.branchid=r.recipientbranchid and brec.countryid=r.countryid and brec.platformid=r.platformid ",
                //                                                "WHERE r.countryid=@countryid AND r.platformid=@platformid AND r.requesttypeid not in (204,501) and r.datecreated>convert(date,dateadd(day,-30,getdate())) ",
                //                                            "UNION ",
                //                                                "SELECT r.requestid'OriginalTransactionID',rt.[description]'TransactionType',r.datecreated'LastTimeModified',r.amount'Amount',bapl.branchname'Origin','N/A' 'Product',brec.branchname'DestinationNumber' ",
                //                                                "FROM request r WITH (NOLOCK) ",
                //                                                "JOIN requesttype rt WITH (NOLOCK) ON r.requesttypeid=rt.requesttypeid ",
                //                                                "JOIN branch bapl WITH (NOLOCK) ON bapl.branchid=r.applicantbranchid and bapl.countryid=r.countryid and bapl.platformid=r.platformid ",
                //                                                "JOIN branch brec WITH (NOLOCK) ON brec.branchid=r.recipientbranchid and brec.countryid=r.countryid and brec.platformid=r.platformid ",
                //                                                "JOIN [user] urec WITH (NOLOCK) ON urec.branchid=brec.branchid and urec.countryid=brec.countryid and urec.PlatformId=brec.platformid ",
                //                                                "JOIN access arec WITH (NOLOCK) ON arec.userid=urec.userid and arec.countryid=urec.countryid and arec.platformid=urec.platformid and arec.login=@loginname ",
                //                                                "WHERE r.countryid=@countryid AND r.platformid=@platformid AND r.requesttypeid not in (204,501) and r.datecreated>convert(date,dateadd(day,-30,getdate())) ",
                //                                            ") t ",
                //                                            "ORDER BY t.LastTimeModified DESC");
                //
                //mySqlCommand.CommandText = String.Format(mySqlCommand.CommandText, agentReference);
                //mySqlCommand.Parameters.Clear();
                //mySqlCommand.Parameters.AddWithValue("@loginname", agentReference);
                //mySqlCommand.Parameters.AddWithValue("@countryid", countryId);
                //mySqlCommand.Parameters.AddWithValue("@platformid", platformId);

                //var reader = mySqlCommand.ExecuteReader();
                //if (reader.HasRows)
                //    while (reader.Read())
                //    {
                //        transactions.Add(new TransactionSummary()
                //        {
                //            OriginalTransactionID = Convert.ToInt64(reader["OriginalTransactionID"]),
                //            TransactionType = (string)reader["TransactionType"],
                //            LastTimeModified = Convert.ToDateTime(reader["LastTimeModified"]),
                //            Amount = Convert.ToDecimal(reader["Amount"]),
                //            RelatedParties = new RelatedParties() { reader.GetString(4), reader.GetString(5), reader.GetString(6) }
                //        });
                //    }
                #endregion


                string query =
                    String.Format(queryBase, count.ToString(), agentReference, sqlConnection.Database.Replace("TRAN", "COMI"), agentId);

                var command = new SqlCommand(query, sqlConnection);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        transactions.Add(new TransactionSummary()
                        {
                            OriginalTransactionID = long.Parse(reader.GetString(0)),
                            TransactionType = reader.GetString(1),
                            LastTimeModified = reader.GetDateTime(2),
                            Amount = reader.GetDecimal(3),
                            RelatedParties = new RelatedParties() { reader.GetString(4), reader.GetString(5), reader.GetString(6) },
                            Recipient = reader.GetString(6)
                        });
                    }
                }
                else
                {
                    logger.ErrorLow(() => TagValue.New().Message("NO SE ECONTRARON DATO").Tag("TIPO DE PARAMETRO").Value(transactionType));
                }
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar las últimas transacciones de un agente en la base de datos de Kinacu").Tag("STACK-TRACE").Value(e.StackTrace));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return (transactions);
        }

        public static DistributionMadeList GetDistributionsMadeByAgent(int countryId, int platformId, string agentReference, DateTime lowerLimit, DateTime upperLimit, int[] distributionTypeId)
        {
            Dictionary<string, object> queryParams = new Dictionary<string, object>()
            {
                { "@CountryIdParam", countryId },
                { "@PlatformIdParam", platformId },
                { "@LoginParam", agentReference },
                { "@LowerLimitParam", lowerLimit.ToString("yyyy-MM-dd") },
                { "@UpperLimitParam", upperLimit.ToString("yyyy-MM-dd") }//,
               // { "@RequestTypeIdParam", distributionTypeId }
            };

            string query = @"
                DECLARE @CountryId INT = @CountryIdParam;
                DECLARE @PlatformId INT = @PlatformIdParam;
                DECLARE @Login VARCHAR(25) = @LoginParam;
                DECLARE @LowerLimit DATE = @LowerLimitParam;
                DECLARE @UpperLimit DATE = @UpperLimitParam;
                --DECLARE @RequestTypeId INT = @RequestTypeIdParam;

                SET @UpperLimit = DATEADD(DAY, 1, @UpperLimit);

                WITH AgeInfo AS (
                    SELECT 
                        TOP (1) u.BranchId, a.UserId, a.AccessTypeId
                    FROM 
                        [dbo].[Access] AS a WITH(READUNCOMMITTED) 
                        JOIN [dbo].[User] AS u WITH(READUNCOMMITTED) ON (u.CountryId = a.CountryId AND u.PlatformId = a.PlatformId AND u.UserId = a.UserId)
                    WHERE a.CountryId = @CountryId AND a.PlatformId = @PlatformId AND a.[Login] = @Login
                ),
                Req AS (
                    SELECT
                        R.*
                    FROM
                        dbo.Request AS R WITH(READUNCOMMITTED)
                    WHERE
                        R.CountryId = @CountryId
                        AND R.PlatformId = @PlatformId
                        AND R.DateApproved >= @LowerLimit
                        AND R.DateApproved < @UpperLimit
                        AND R.ApplicantBranchId = (SELECT BranchId FROM AgeInfo)
                        
                        AND R.RequestTypeId in(" + String.Join(",", distributionTypeId) + @")
                        AND R.StatusId = 'CE'
                )
                SELECT
                     Req.RequestId AS 'DistributionId'
                    ,BD.BranchId AS 'TargetAgentId'
                    ,BD.BranchName AS 'TargetAgentName'
                    ,BD.LegalNumber AS 'TargetAgentLegalNumber'
                    ,BD.PhoneNumber AS 'TargetAgentPhoneNumber'
                    ,BD.[Address] AS 'TargetAgentAddress'
                    ,Req.Amount AS 'Base'
                    ,CAST((Req.Amount * (BD.CommissionByPurchase / 100)) AS MONEY) AS 'Commission'
                    ,BD.CommissionByPurchase AS 'Percentage'
                    ,(Req.Amount + CAST((Req.Amount * (BD.CommissionByPurchase / 100)) AS MONEY)) AS 'Total'
                    ,Req.DateCreated AS 'ApprovalDate'
                    ,Req.RequestTypeId as 'RequestTypeId'
                FROM
                    Req
                    JOIN dbo.Branch AS BA WITH(READUNCOMMITTED) ON (BA.CountryId = Req.CountryId AND BA.PlatformId = Req.PlatformId AND BA.BranchId = Req.ApplicantBranchId)
                    JOIN dbo.Branch AS BD WITH(READUNCOMMITTED) ON (BD.CountryId = Req.CountryId AND BD.PlatformId = Req.PlatformId AND BD.BranchId = Req.RecipientBranchId)
                ORDER BY
                    Req.DateApproved ASC
                ;";

            List<DistributionMade> dists = new List<DistributionMade>();
            using (SqlConnection connection = Movilway.API.Utils.Database.GetBaseDbConnection())
            {
                connection.Open();
                dists = Movilway.API.Utils.Dwh<DistributionMade>.ExecuteReader(connection, query, queryParams, null);
            }
            DistributionMadeList ret = new DistributionMadeList();
            ret.AddRange(dists);
            return ret;
        }

        public static string GetParentAgent(string agentReference)
        {
            string agentInfo = "";

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@"  select cast(ags.age_id as varchar) + '-' + ags.age_nombre'Agent-Name' from Agente ags with (NOLOCK) 
                            join Agente ag with (NOLOCK) on ags.age_id=ag.age_id_sup
                            join Usuario u with (NOLOCK) on u.age_id=ag.age_id
                            join Acceso ac with (NOLOCK) on ac.usr_id=u.usr_id
                            where ac.acc_login='{0}' "), agentReference);

                //logger.InfoHigh(query);
                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                agentInfo = (string)command.ExecuteScalar();
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el agente padre en la base de datos de Kinacu"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return (agentInfo);
        }

        public static string GetAgentEmail(string agentReference)
        {
            string agentInfo = "";

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@"  select ag.age_email'Agent-Email' from Agente ag with (NOLOCK) 
                                join Usuario u with (NOLOCK) on u.age_id=ag.age_id
                                join Acceso ac with (NOLOCK) on ac.usr_id=u.usr_id
                                where ac.acc_login='{0}' "), agentReference);

                //logger.InfoHigh(query);
                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                agentInfo = (string)command.ExecuteScalar();
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el email de un agente en la base de datos de Kinacu"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return (agentInfo);
        }

        public static int GetUserId(string agentReference)
        {
            int userId = 0;

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@"  select cast(u.usr_id as int)
                                from Usuario u with (NOLOCK)
                                join Acceso ac with (NOLOCK) on ac.usr_id=u.usr_id
                                where ac.acc_login='{0}' "), agentReference);

                //logger.InfoHigh(query);
                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                userId = (int)command.ExecuteScalar();
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el id de un usuario en la base de datos de Kinacu"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return (userId);
        }

        public static int GetUserId(string agentReference, decimal acceso)
        {
            int userId = 0;

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query = @"  select cast(u.usr_id as int)
                                from Usuario u with (NOLOCK)
                                join Acceso ac with (NOLOCK) on ac.usr_id=u.usr_id
                                where ac.acc_login = @acc_login and  [tac_id]  = @tac_id ";

                //logger.InfoHigh(query);
                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);

                command.Parameters.AddWithValue("@acc_login", agentReference);
                command.Parameters.AddWithValue("@tac_id", acceso);

                userId = (int)command.ExecuteScalar();
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el id de un usuario en la base de datos de Kinacu"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return (userId);
        }

        public static ParameterList GetParameters()
        {
            ParameterList result = new ParameterList();

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@" SELECT cast([tac_id] as int)'AccessTypeId'
                                ,[tac_descripcion]'AccessTypeName'
                                ,[tac_pwd_minimo]'MinimumPasswordLength'
                                ,[tac_pwd_maximo]'MaximumPasswordLength'
                                ,[tac_usr_minimo]'MinimumAccessLength'
                                ,[tac_usr_maximo]'MaximumAccessLength'
                            FROM [TipoAcceso] "));

                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                        result.Add(new ParametersInfo()
                        {
                            AccessTypeId = reader.GetInt32(0),
                            AccessTypeName = reader.GetString(1),
                            MinimumPasswordLength = reader.GetInt32(2),
                            MaximumPasswordLength = reader.GetInt32(3),
                            MinimumAccessLength = reader.GetInt32(4),
                            MaximumAccessLength = reader.GetInt32(5)
                        });
                }
                else
                {
                    result = new ParameterList();
                }
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar la información de un agente en la base de datos de Kinacu"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }

            return (result);
        }

        public static AgentInfo GetAgentInfoById(string agentId)
        {
            AgentInfo result = new AgentInfo();

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query = string.Format(
                        @"  SELECT a.[age_id]'BranchID'
                                  ,isnull(a.[age_id_sup],0)'OwnerID'
                                  ,'nationalId' 'NationalIDType'
                                  ,a.[age_cuit]'NationalID'
                                  ,a.[age_nombre]'Name'
                                  ,a.[age_razonsocial]'LegalName'
                                  ,a.[age_direccion]'Address'
                                  ,isnull(a.[age_email],'')'Email'
                                  ,cast(a.[age_subNiveles] as int)'SubLevel'
                                  ,a.[age_pdv]'PDV'
                                  ,cast(a.ct_id as int) TaxCategory
                                  ,cast(a.sa_id AS INT) Segment
                                  ,isnull(a.[age_tel],'')'PhoneNumber'
                              FROM [dbo].[Agente] a with (NOLOCK)
                              where a.age_id = {0} ", agentId);

                //logger.InfoHigh(query);
                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                        result = new AgentInfo()
                        {
                            BranchID = long.Parse(reader.GetDecimal(0).ToString()),
                            OwnerID = long.Parse(reader.GetDecimal(1).ToString()),
                            NationalIDType = reader.GetString(2),
                            NationalID = reader.GetString(3),
                            Name = reader.GetString(4),
                            LegalName = reader.GetString(5),
                            Address = reader.GetString(6),
                            Email = reader.GetString(7),
                            SubLevel = reader.GetInt32(8),
                            PDVID = reader.GetString(9),
                            TaxCategory = reader.GetInt32(10),
                            SegmentId = reader.GetInt32(11),
                            PhoneNumber = reader.GetString(12)
                        };
                }
                else
                {
                    result = new AgentInfo() { BranchID = -1 };
                }

                command.Dispose();
                reader.Close();

                // se obtiene la lista de categoría tributaria
                query = "SELECT [ct_id], [ct_nombre] FROM [CategoriaTributaria] WITH(READUNCOMMITTED)";
                command = new SqlCommand(query, sqlConnection);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    result.TaxCategories = new TaxCategoryList();
                    while (reader.Read())
                    {
                        result.TaxCategories.Add(Convert.ToInt32(reader.GetDecimal(0)), reader.GetString(1));
                    }
                }

                command.Dispose();
                reader.Close();

                // se obtiene la lista de segmentos
                query = "SELECT [sa_id], [sa_nombre] FROM [dbo].[SegmentoAgencia] WITH(READUNCOMMITTED)";
                command = new SqlCommand(query, sqlConnection);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    result.SegmentList = new SegmentList();
                    while (reader.Read())
                    {
                        result.SegmentList.Add(Convert.ToInt32(reader.GetDecimal(0)), reader.GetString(1));
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar la información de un agente en la base de datos de Kinacu"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }

            try
            {
                sqlConnection.ConnectionString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString.Replace("TRAN_", "COMI_");

                var query =
                    String.Format(
                        (@" SELECT cast([grpId] as int)'groupId',[grpNombre]'groupName'
                            FROM [dbo].[KmmGrupo] 
                            where ageId={0} and grpEstado='AC'"), result.BranchID);

                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    result.CommissionGroups = new CommissionGroupList();
                    while (reader.Read())
                        result.CommissionGroups.Add(reader.GetInt32(0), reader.GetString(1));
                }
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar la información de los grupos de comisión a que pertenece un agente"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }

            return (result);
        }

        public static AgentInfo GetAgentInfo(string agentReference)
        {
            string agentInfo = string.Empty;
            AgentInfo result = new AgentInfo();

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@"  SELECT a.[age_id]'BranchID'
                                  ,isnull(a.[age_id_sup],0)'OwnerID'
	                              ,'nationalId' 'NationalIDType'
                                  ,a.[age_cuit]'NationalID'
                                  ,a.[age_nombre]'Name'
                                  ,a.[age_razonsocial]'LegalName'
                                  ,a.[age_direccion]'Address'
	                              ,isnull(a.[age_email],'')'Email'
                                  ,cast(a.[age_subNiveles] as int)'SubLevel'
                                  ,a.[age_pdv]'PDV'
                                  ,cast(a.ct_id as int) TaxCategory
                                  ,cast(a.sa_id as int) Segment
                                  ,isnull(a.[age_tel],'')'PhoneNumber'
                              FROM [dbo].[Agente] a with (NOLOCK)
                              join [dbo].[Usuario] u with (NOLOCK) on a.age_id=u.age_id
                              join [dbo].[Acceso] ac with (NOLOCK) on ac.usr_id=u.usr_id
                              where ac.acc_login='{0}' "), agentReference);

                //logger.InfoHigh(query);
                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                        result = new AgentInfo()
                        {
                            BranchID = long.Parse(reader.GetDecimal(0).ToString()),
                            OwnerID = long.Parse(reader.GetDecimal(1).ToString()),
                            NationalIDType = reader.GetString(2),
                            NationalID = reader.GetString(3),
                            Name = reader.GetString(4),
                            LegalName = reader.GetString(5),
                            Address = reader.GetString(6),
                            Email = reader.GetString(7),
                            SubLevel = reader.GetInt32(8),
                            PDVID = reader.GetString(9),
                            TaxCategory = reader.GetInt32(10),
                            SegmentId = reader.GetInt32(11),
                            PhoneNumber = reader.GetString(12)
                        };
                }
                else
                {
                    result = new AgentInfo() { BranchID = -1 };
                }

                command.Dispose();
                reader.Close();

                // se obtiene la lista de categoría tributaria
                query = "SELECT [ct_id], [ct_nombre] FROM [CategoriaTributaria] WITH(READUNCOMMITTED)";
                command = new SqlCommand(query, sqlConnection);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    result.TaxCategories = new TaxCategoryList();
                    while (reader.Read())
                    {
                        result.TaxCategories.Add(Convert.ToInt32(reader.GetDecimal(0)), reader.GetString(1));
                    }
                }

                command.Dispose();
                reader.Close();

                // se obtiene la lista de segmentos
                query = "SELECT [sa_id], [sa_nombre] FROM [dbo].[SegmentoAgencia] WITH(READUNCOMMITTED)";
                command = new SqlCommand(query, sqlConnection);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    result.SegmentList = new SegmentList();
                    while (reader.Read())
                    {
                        result.SegmentList.Add(Convert.ToInt32(reader.GetDecimal(0)), reader.GetString(1));
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar la información de un agente en la base de datos de Kinacu"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }

            try
            {
                sqlConnection.ConnectionString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString.Replace("TRAN_", "COMI_");

                var query =
                    String.Format(
                        (@" SELECT cast([grpId] as int)'groupId',[grpNombre]'groupName'
                            FROM [dbo].[KmmGrupo] 
                            where ageId={0} and grpEstado='AC'"), result.BranchID);

                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    result.CommissionGroups = new CommissionGroupList();
                    while (reader.Read())
                        result.CommissionGroups.Add(reader.GetInt32(0), reader.GetString(1));
                }
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar la información de los grupos de comisión a que pertenece un agente"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }

            return (result);
        }

        public static string GetValidatedEmailByLogin(string agentReference)
        {
            string result = "";

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@" SELECT top 1 acl.email
	                        FROM [{0}].[POSWEB_CONTROL{1}].[dbo].[ACLUser] acl 
	                        JOIN {2}[{3}].[dbo].[Mingo] m with (NOLOCK) on m.[user]=acl.[login] and m.statusid='AC'
	                        WHERE acl.[login] = '{4}'
	                        ORDER BY acl.signupdate DESC"),
                    ConfigurationManager.ConnectionStrings["MacroProductosEntities"].ConnectionString.Substring(ConfigurationManager.ConnectionStrings["MacroProductosEntities"].ConnectionString.IndexOf("data source")).Split(';').Where(p => p.Split('=')[0].ToLower() == "data source").Select(p => p.Split('=')[1]).First(),
                    (ConfigurationManager.ConnectionStrings["SECURE_DB"].ConnectionString.Split(';').Where(p => p.Split('=')[0].ToLower() == "initial catalog").Select(p => p.Split('=')[1]).First().ToLower().Contains("_dev") ? "_DEV" : ""),
                    (ConfigurationManager.ConnectionStrings["SECURE_DB"].ConnectionString.Split(';').Where(p => p.Split('=')[0].ToLower() == "data source").Select(p => p.Split('=')[1]).First() != ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString.Split(';').Where(p => p.Split('=')[0].ToLower() == "data source").Select(p => p.Split('=')[1]).First() ? String.Concat("[", ConfigurationManager.ConnectionStrings["SECURE_DB"].ConnectionString.Split(';').Where(p => p.Split('=')[0].ToLower() == "data source").Select(p => p.Split('=')[1]).First(), "].") : ""),
                    ConfigurationManager.ConnectionStrings["SECURE_DB"].ConnectionString.Split(';').Where(p => p.Split('=')[0].ToLower() == "initial catalog").Select(p => p.Split('=')[1]).First(),
                    agentReference);

                logger.InfoHigh("Query: " + query);
                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                result = (string)command.ExecuteScalar();
                logger.InfoHigh("Result: " + result);

                command.Dispose();
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el email validado de un agente según el ID en la base de datos"));
                result = "";
            }
            finally
            {
                sqlConnection.Close();
            }

            return (result);
        }

        public static string GetValidatedEmailById(string agentReference)
        {
            string result = "";

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@" SELECT top 1 acl.email
	                        FROM [dbo].[Agente] a with (NOLOCK)
	                        JOIN [{0}].[POSWEB_CONTROL{1}].[dbo].[ACLUser] acl with (NOLOCK) on acl.username=a.age_nombre
	                        LEFT JOIN {2}[{3}].[dbo].[Mingo] m with (NOLOCK) on m.[user]=acl.[login] and m.statusid='AC'
	                        where a.age_id = '{4}'
	                        ORDER BY acl.signupdate DESC"),
                    ConfigurationManager.ConnectionStrings["MacroProductosEntities"].ConnectionString.Substring(ConfigurationManager.ConnectionStrings["MacroProductosEntities"].ConnectionString.IndexOf("data source")).Split(';').Where(p => p.Split('=')[0].ToLower() == "data source").Select(p => p.Split('=')[1]).First(),
                    (ConfigurationManager.ConnectionStrings["SECURE_DB"].ConnectionString.Split(';').Where(p => p.Split('=')[0].ToLower() == "initial catalog").Select(p => p.Split('=')[1]).First().ToLower().Contains("_dev") ? "_DEV" : ""),
                    (ConfigurationManager.ConnectionStrings["SECURE_DB"].ConnectionString.Split(';').Where(p => p.Split('=')[0].ToLower() == "data source").Select(p => p.Split('=')[1]).First() != ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString.Split(';').Where(p => p.Split('=')[0].ToLower() == "data source").Select(p => p.Split('=')[1]).First() ? String.Concat("[", ConfigurationManager.ConnectionStrings["SECURE_DB"].ConnectionString.Split(';').Where(p => p.Split('=')[0].ToLower() == "data source").Select(p => p.Split('=')[1]).First(), "].") : ""),
                    ConfigurationManager.ConnectionStrings["SECURE_DB"].ConnectionString.Split(';').Where(p => p.Split('=')[0].ToLower() == "initial catalog").Select(p => p.Split('=')[1]).First(),
                    agentReference);

                //logger.InfoHigh(query);
                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                result = (string)command.ExecuteScalar();

                command.Dispose();
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el email validado de un agente según el ID en la base de datos"));
                result = "";
            }
            finally
            {
                sqlConnection.Close();
            }

            return (result);
        }

        private static void sendMessage(string mobilePhone, string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"http://www1.telemo.com.ve/applications/interconnection/mtPost.php?");
            sb.Append("cid=");
            sb.Append("52");
            sb.Append("&mensaje_id=");
            sb.Append("4");
            sb.Append("&usuario=");
            sb.Append(mobilePhone);
            sb.Append("&contenido=");
            sb.Append(message);
            sb.Append("&date=");
            sb.Append(DateTime.Now.ToString("yyyy-MM-dd"));
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sb.ToString());
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        }

        public static Score GetScore(string agentId)
        {
            //var ran = new Random();
            //return new Score()
            //{
            //    BranchId = ran.Next(10, 999),
            //    BranchName = agentReference ?? "myAgent",
            //    LotteryType = "Retailer",
            //    Confirmed = true,
            //    YearToDate = ran.Next(10, 9999),
            //    CurrentMonth = ran.Next(10, 9999),
            //    Standard = ran.Next(10, 9999),
            //    Bonus = ran.Next(10, 9999),
            //    Behaviour = ran.Next(10, 9999),
            //    NetworkStandard = ran.Next(10, 9999),
            //    NetworkBonus = ran.Next(10, 9999),
            //    NetworkBehaviour = ran.Next(10, 9999),
            //    Questionnaire = ran.Next(10, 9999)
            //};

            Score result = new Score()
            {
                BranchId = int.Parse(agentId),
                BranchName = "Unknown",
                LotteryType = "Unknown",
                Confirmed = false,
                YearToDate = 0,
                CurrentMonth = 0,
                Standard = 0,
                Bonus = 0,
                Behaviour = 0,
                NetworkStandard = 0,
                NetworkBonus = 0,
                NetworkBehaviour = 0,
                Questionnaire = 0
            };

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["LOYALTY_DB"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@" SELECT s.[BranchId]
	                              ,cast(s.BranchId as varchar)'BranchName'--,a.agentname'BranchName'
                                  ,IsNull(cast(s.[LotteryTypeId] as varchar),'Retailer')'LotteryType'
                                  ,IsNull(s.[Confirmed],0)'Confirmed'
                                  ,s.[YearToDate]
                                  ,s.[CurrentMonth]
                                  ,s.[Standard]
                                  ,s.[Bonus]
                                  ,s.[Behaviour]
                                  ,s.[NetworkStandard]
                                  ,s.[NetworkBonus]
                                  ,s.[NetworkBehaviour]
                                  ,s.[Questionnaire]
								  ,s.[Registration]
                            FROM [dbo].[Score] s
                            --JOIN [dbo].[AgentExtended] a on s.countryid=a.countryid and s.platformid=a.platformid and s.branchid=a.agentid
                            WHERE s.branchid={0} and s.countryid={1}"), agentId, ConfigurationManager.AppSettings["CountryID"]);

                //logger.InfoHigh(query);
                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                        result = new Score()
                        {
                            BranchId = reader.GetInt32(0),
                            BranchName = reader.GetString(1),
                            LotteryType = reader.GetString(2),
                            Confirmed = reader.GetInt32(3) == 0 ? false : true,
                            YearToDate = reader.GetInt32(4),
                            CurrentMonth = reader.GetInt32(5),
                            Standard = reader.GetInt32(6),
                            Bonus = reader.GetInt32(7),
                            Behaviour = reader.GetInt32(8),
                            NetworkStandard = reader.GetInt32(9),
                            NetworkBonus = reader.GetInt32(10),
                            NetworkBehaviour = reader.GetInt32(11),
                            Questionnaire = reader.GetInt32(12),
                            Registration = reader.GetInt32(13)
                        };
                }
            }
            catch (Exception e)
            {
                //logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar la información de un agente en la base de datos de Kinacu"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return (result);
        }

        public static AgentDistributionList GetAgentDistributionList(string agentReference, string agentChildReference)
        {
            AgentDistributionList distributions = new AgentDistributionList();

            var existsChild = !String.IsNullOrEmpty(agentChildReference);

            string QUERY_TRAN_SUFIX = existsChild ? " and ag.age_id=" + agentChildReference : "";
            string QUERY_BASE_SUFIX = existsChild ? " and branchid=" + agentChildReference : "";

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query = "SELECT cast(ag.[age_id] as varchar),ag.[age_cuit],ag.[age_nombre],st.[stkCantidad],cc.[ctaSaldo],ag.[age_estado]" +
                            /*(existsChild ? ",(SELECT TOP 1 IsNull(CONVERT(char(10), ccmFecha ,112),'') FROM [KfnCuentaCorrienteMovimiento] cm with (NOLOCK) where ctaId=cc.ctaId order by ccmFecha desc)" : "") +*/
                            ",IsNull(CONVERT(char(10), (SELECT MAX(ccmFecha) FROM [KfnCuentaCorrienteMovimiento] cm WITH (READUNCOMMITTED) where ctaId=cc.ctaId),112),'')" +
                            String.Format(
                                (@" FROM [Agente] ag WITH (READUNCOMMITTED)
                                    join [KlgStock] st WITH (READUNCOMMITTED) on st.[ageIdPropietario]=ag.[age_Id]
                                    join [KfnCuentaCorriente] cc WITH (READUNCOMMITTED) on cc.[ageId]=ag.[age_Id]
                                    where ag.age_id_sup={0}" + QUERY_TRAN_SUFIX), agentReference);

                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                var reader = command.ExecuteReader();
                AgentDistributionInfo agentDistributionInfo;
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        agentDistributionInfo = new AgentDistributionInfo()
                        {
                            AgentId = int.Parse(reader.GetString(0)),
                            LegalNumber = reader.GetString(1),
                            AgentName = reader.GetString(2),
                            Stock = reader.GetDecimal(3),
                            Account = reader.GetDecimal(4),
                            Status = reader.GetString(5)
                        };

                        /*if (existsChild)*/
                        agentDistributionInfo.LastDistribution = reader.GetString(6);

                        distributions.Add(agentDistributionInfo);
                    }
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

            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["BASEConnectionString"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@" SELECT [BranchId],IsNull(CONVERT(char(10), [DateLastPurchase],112),''),IsNull(CONVERT(char(10), [DateLastTransaction],112),'')
                            FROM [Branch] with(readuncommitted)
                            where platformid=1 and countryid={0} and parentbranchid={1}" + QUERY_BASE_SUFIX), ConfigurationManager.AppSettings["CountryID"], agentReference);

                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        if (distributions.Count(d => d.AgentId == reader.GetInt32(0)) > 0)
                        {
                            /*if (!existsChild) distributions.Single(d => d.AgentId == reader.GetInt32(0)).LastDistribution = reader.GetString(1);*/
                            distributions.Single(d => d.AgentId == reader.GetInt32(0)).LastTopUp = reader.GetString(2);
                        }
                    }
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

            return (distributions);
        }

        private static string GetDateText(int days)
        {
            if (days == -1)
                return "Indeterminado";
            if (days == 0)
                return "Hoy";
            if (days == 1)
                return "Ayer";
            if (days > 1 && days <= 30)
                return "Hace " + days + " días";
            if (days > 30 && days <= 60)
                return "Hace 1 mes";
            if (days > 60 && days <= 90)
                return "Hace 2 meses";
            if (days > 90 && days <= 120)
                return "Hace 3 meses";
            if (days > 120)
                return "Hace más de 3 meses";
            return "Error";
        }

        public static decimal GetAgentCheckingAccountBalance(string agentReference)
        {
            string agentBalance = "";

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@" SELECT cast([ctaSaldo] as varchar)
                            FROM [KfnCuentaCorriente] cc with (NOLOCK)
                            join [Usuario] u with (NOLOCK) on u.age_id=cc.ageid
                            join [Acceso] ac with (NOLOCK) on ac.usr_id=u.usr_id
                            where ac.acc_login='{0}' "), agentReference);

                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                agentBalance = (string)command.ExecuteScalar();
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el balance de cuenta corriente de un agente en la base de datos de Kinacu"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return decimal.Parse(agentBalance);
        }

        public static List<BasicAgentInfo> GetAgentExtendedValues(string agentIds)
        {
            var agentExtended = new List<BasicAgentInfo>();


            try
            {
                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString))
                {
                    var query = String.Concat(
                                "SELECT a.age_id ,a.age_nombre,a.age_email,p.pro_nombre ,c.ciu_nombre,isnull(s.stkcantidad,0) stkCantidad,a.age_estado,",
                                    "(SELECT COUNT(1) FROM [Agente] WITH (NOLOCK) WHERE age_id_sup=a.age_id)'childs', age_pdv ",
                                "FROM [Agente] a WITH (NOLOCK) ",
                                "JOIN [Ciudad] c WITH (NOLOCK) on a.ciu_id=c.ciu_id ",
                                "JOIN [Provincia] p WITH (NOLOCK) on p.pro_id=c.pro_id ",
                                "LEFT JOIN [KlgStock] s WITH (NOLOCK) on s.ageIdpropietario=a.age_id ",
                                "WHERE a.age_id IN (", agentIds, ") ");

                    sqlConnection.Open();

                    var command = new SqlCommand(query, sqlConnection);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                            {

                                agentExtended.Add(new BasicAgentInfo()
                                {
                                    Agent = Decimal.ToInt32((decimal)reader["age_id"]).ToString(),
                                    Name = reader["age_nombre"].ToString(),
                                    Email = reader["age_email"].ToString(),
                                    Department = (string)reader["pro_nombre"],
                                    City = (string)reader["ciu_nombre"],
                                    CurrentBalance = (decimal)reader["stkCantidad"],
                                    Status = (string)reader["age_estado"],
                                    ChildsCount = (int)reader["childs"],
                                    PDVId = (string)reader["age_pdv"]
                                });

                            }
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar los datos de los agentes hijos en la base de datos de Kinacu"));
                throw;
            }

            return agentExtended;
        }

        private static string GetParameters(SqlCommand mySqlCommand)
        {
            string v = string.Empty;
            if (mySqlCommand.Parameters != null && mySqlCommand.Parameters.Count > 0)
            {
                IEnumerable<object> queryparameters =
            mySqlCommand.Parameters.Cast<SqlParameter>().Select(p => new { p.ParameterName, p.Value });
                v = string.Join(",", queryparameters.ToArray());
            }
            return v;
        }
        /// <summary>
        /// Retorna el tipo de acceso a partir del de la cadena que se pasa como parametro
        /// </summary>
        /// <param name="accesstype"></param>
        /// <returns></returns>
        public static int GetAccessTypeCode(string accesstype)
        {
            //TODO LA CONEXION NO ES CORRECTA
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            int result = 0;
            try
            {
                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {

                    mySqlConnection.Open();
                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                    SqlDataReader mySqlDataReader = null;

                    //TODO APLICAR CACHE

                    //seleccionar desde la base de datos COM COL
                    // PUDE QUE TENAGA UNA COMISISON PARA TODOS 
                    //LOS PROPDUCTOS O PARA ESPECIFICA APRA ALGUNOS
                    mySqlCommand.CommandText = "SELECT [tac_id] FROM [dbo].[TipoAcceso] WHERE [tac_descripcion] = @tac_descripcion";
                    //commission PercentageKmm.Models.Commissions.PercentageOfSalesModel
                    //,[acc_password] ,[acc_lastlogin] ,[acc_intentos],[acc_cambiopassword] ,[acc_validityDate] ,[acc_terminalId]

                    mySqlCommand.Parameters.AddWithValue("@tac_descripcion", accesstype);

                    using (mySqlDataReader = mySqlCommand.ExecuteReader())
                    {
                        //cambiar a una lista de acceso

                        if (mySqlDataReader.HasRows && mySqlDataReader.Read())
                        {
                            result = Decimal.ToInt32((decimal)mySqlDataReader["tac_id"]);
                        }
                        else
                        {
                            throw new Exception("TIPO DE ACCESO " + accesstype + " NO ES VALIDO ");
                        }


                    }



                }
            }
            catch (Exception ex)
            {
                throw;

            }

            return result;


        }



        public static TipoAcceso GetTipoAcceso(int ta_id, bool ExtendedValues)
        {
            TipoAcceso result = null;

            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;

            try
            {
                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {

                    mySqlConnection.Open();
                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                    SqlDataReader mySqlDataReader = null;

                    //TODO CACHE APLICAR 

                    //seleccionar desde la base de datos COM COL
                    // PUDE QUE TENAGA UNA COMISISON PARA TODOS 
                    //LOS PROPDUCTOS O PARA ESPECIFICA APRA ALGUNOS
                    string Columns = ",tac_usr_minimo      ,tac_usr_maximo      ,tac_periodovalidez      ,tac_chequearvalidez      ,tac_maxreintento      ,tac_pwd_minimo      ,tac_pwd_maximo      ,dda_id_usuario      ,dda_id_password      ,tac_valoresduplicados ";

                    mySqlCommand.CommandText = String.Concat("SELECT tac_id,tac_descripcion ", ExtendedValues ? Columns : "", " FROM TipoAcceso WHERE tac_id  = @tac_id");

                    //commission PercentageKmm.Models.Commissions.PercentageOfSalesModel
                    //,[acc_password] ,[acc_lastlogin] ,[acc_intentos],[acc_cambiopassword] ,[acc_validityDate] ,[acc_terminalId]


                    //TODO APLICAR FILTRO DEL CACHE
                    mySqlCommand.Parameters.AddWithValue("@tac_id", ta_id);

                    using (mySqlDataReader = mySqlCommand.ExecuteReader())
                    {
                        //cambiar a una lista de acceso

                        if (mySqlDataReader.HasRows && mySqlDataReader.Read())
                        {
                            result.ID = Decimal.ToInt32((decimal)mySqlDataReader["tac_id"]);
                            result.DESCRIPCION = (string)mySqlDataReader["tac_descripcion"];

                            if (ExtendedValues)
                            {
                                result.USR_MINIMO = (int)mySqlDataReader["tac_usr_minimo"];
                                result.USR_MAXIMO = (int)mySqlDataReader["tac_usr_maximo"];
                                result.PERIODOVALIDEZ = (int)mySqlDataReader["tac_periodovalidez"];
                                result.CHEQUEARVALIDEZ = (char)mySqlDataReader["tac_chequearvalidez"];
                                result.MAXREINTENTO = (int)mySqlDataReader["tac_maxreintento"];
                                result.PWD_MINIMO = (int)mySqlDataReader["tac_pwd_minimo"];
                                result.PWD_MAXIMO = (int)mySqlDataReader["tac_pwd_maximo"];
                                result.DDA_ID_USUARIO = (int)mySqlDataReader["dda_id_usuario"];
                                result.DDA_ID_PASSWORD = (int)mySqlDataReader["dda_id_password"];
                                result.VALORESDUPLICADOS = (int)mySqlDataReader["tac_valoresduplicados"];
                            }
                        }
                        else
                        {
                            throw new Exception("TIPO DE ACCESO " + ta_id + " NO ES VALIDO ");
                        }


                    }



                }
            }
            catch (Exception ex)
            {
                throw;

            }


            return result;
        }

        /// <summary>
        /// Retorna toda la lista de Tipo de accesos en el sistema
        /// </summary>
        /// <param name="ExtendedValues">True si se debe incluir las columnas adicionales a tac_id y tac_descripcion</param>
        /// <returns>Lista de Tipos de acceso</returns>
        public static List<TipoAcceso> GetTipoAccess(bool ExtendedValues)
        {
            List<TipoAcceso> result = null;
            //TODO APLICAR CACHE 
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;

            try
            {
                result = new List<TipoAcceso>();
                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {

                    mySqlConnection.Open();
                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                    SqlDataReader mySqlDataReader = null;

                    //seleccionar desde la base de datos COM COL
                    // PUDE QUE TENAGA UNA COMISISON PARA TODOS 
                    //LOS PROPDUCTOS O PARA ESPECIFICA APRA ALGUNOS
                    string Columns = ",tac_usr_minimo      ,tac_usr_maximo      ,tac_periodovalidez      ,tac_chequearvalidez      ,tac_maxreintento      ,tac_pwd_minimo      ,tac_pwd_maximo      ,dda_id_usuario      ,dda_id_password      ,tac_valoresduplicados ";

                    mySqlCommand.CommandText = String.Concat("SELECT tac_id,tac_descripcion ", ExtendedValues ? Columns : "", " FROM TipoAcceso ");

                    //commission PercentageKmm.Models.Commissions.PercentageOfSalesModel
                    //,[acc_password] ,[acc_lastlogin] ,[acc_intentos],[acc_cambiopassword] ,[acc_validityDate] ,[acc_terminalId]


                    using (mySqlDataReader = mySqlCommand.ExecuteReader())
                    {
                        //cambiar a una lista de acceso

                        if (mySqlDataReader.HasRows)
                        {
                            while (mySqlDataReader.Read())
                            {


                                TipoAcceso ta = new TipoAcceso();
                                ta.ID = Decimal.ToInt32((decimal)mySqlDataReader["tac_id"]);
                                ta.DESCRIPCION = (string)mySqlDataReader["tac_descripcion"];

                                if (ExtendedValues)
                                {
                                    ta.USR_MINIMO = (int)mySqlDataReader["tac_usr_minimo"];
                                    ta.USR_MAXIMO = (int)mySqlDataReader["tac_usr_maximo"];
                                    ta.PERIODOVALIDEZ = (int)mySqlDataReader["tac_periodovalidez"];
                                    ta.CHEQUEARVALIDEZ = (char)mySqlDataReader["tac_chequearvalidez"];
                                    ta.MAXREINTENTO = (int)mySqlDataReader["tac_maxreintento"];
                                    ta.PWD_MINIMO = (int)mySqlDataReader["tac_pwd_minimo"];
                                    ta.PWD_MAXIMO = (int)mySqlDataReader["tac_pwd_maximo"];
                                    ta.DDA_ID_USUARIO = (int)mySqlDataReader["dda_id_usuario"];
                                    ta.DDA_ID_PASSWORD = (int)mySqlDataReader["dda_id_password"];
                                    ta.VALORESDUPLICADOS = (int)mySqlDataReader["tac_valoresduplicados"];
                                }
                                result.Add(ta);
                            }
                        }


                    }



                }
            }
            catch (Exception ex)
            {
                throw;

            }


            return result;
        }

        /// <summary>
        /// Retorna el tipo de acceso a partir del de la cadena que se pasa como parametro
        /// </summary>
        /// <param name="accesstype"></param>
        /// <returns></returns>
        public static string GetAccessName(int accesstype)
        {

            //default el accesso es cero
            /*
            string  result = "";
            switch (accesstype)
            {
                case 1 :
                    result = "WEB";
                    break;
                case 2:
                    result ="SMS";
                    break;
               case  3:
                    result ="DAV-PC";
                    break;
               case 4:
                    result = "CAJA";
                    break;
                case  5:
                    result ="Mobile";
                    break;
                case 6:
                    result = "POS";
                    break;
                case    8:

                    result = "Desktop";

                    break;
                case 9:
                    result = "USSD";
                    break;
                case  10:
                    result = "USSD-Movistar";

                    break;

                case 12:
                    result = "POSWEB";
                    break;
                default:
                    throw new Exception("TIPO DE ACCESSO NO VALIDO");

            }
            */
            string result = "";
            //TODO LA CONEXION NO ES CORRECTA
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;

            try
            {
                //TODO APLICAR CACHE
                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {
                    mySqlConnection.Open();
                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                    SqlDataReader mySqlDataReader = null;

                    //seleccionar desde la base de datos COM COL
                    // PUDE QUE TENAGA UNA COMISISON PARA TODOS 
                    //LOS PROPDUCTOS O PARA ESPECIFICA APRA ALGUNOS
                    mySqlCommand.CommandText = "SELECT [tac_descripcion] FROM [dbo].[TipoAcceso] WHERE [tac_id] = @tac_id";
                    //commission PercentageKmm.Models.Commissions.PercentageOfSalesModel
                    //,[acc_password] ,[acc_lastlogin] ,[acc_intentos],[acc_cambiopassword] ,[acc_validityDate] ,[acc_terminalId]

                    mySqlCommand.Parameters.AddWithValue("@tac_id", accesstype);

                    using (mySqlDataReader = mySqlCommand.ExecuteReader())
                    {
                        //cambiar a una lista de acceso

                        if (mySqlDataReader.HasRows && mySqlDataReader.Read())
                        {
                            result = mySqlDataReader["tac_descripcion"].ToString();
                        }
                        else
                        {
                            throw new Exception("TIPO DE ACCESO " + accesstype + " NO ES VALIDO");
                        }


                    }



                }
            }
            catch (Exception ex)
            {
                throw;

            }


            return result;

        }
        /// <summary>
        /// Retorna los datos del agente a Editar en un objeto de tipo RequestAgent bajo el contexto de
        /// un GenericApiResult con el respectivo codigo de respuesta y mensaje
        /// Precondicion:
        /// -El agente existe en el sistema
        /// -Se han validado los permisos para acceder a los datos
        /// Postcondicion:
        /// Retorna los datos del agent correspondiente al id
        /// </summary>
        /// <param name="AgeId">Id del agente en el sistema</param>
        /// <returns>GenericApiResult<RequestAgent> con los datos del agente</returns>
        public static GenericApiResult<RequestAgent> GetAgentEditById(int AgeId)
        {

            logger.InfoLow("[QRY] " + Utils.LOG_PREFIX + "[GetAgentEditById]");

            ///falta FactoryMethod
            GenericApiResult<RequestAgent> result = new GenericApiResult<RequestAgent>();
            RequestAgent obj = new RequestAgent();

            //
            //def values

            obj.age_id = AgeId;

            int Response_Code = 0;
            string Message = "";

            //valor para guardar la comision por Porcentage
            int commissionPercentage = 0;

            string _m = "";
            Int32? entId = 0;
            try
            {

                string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;

                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {


                    mySqlConnection.Open();

                    SqlCommand mySqlCommand = new SqlCommand();
                    // SqlTransaction tran = null;
                    SqlDataReader mySqlDataReader = null;
                    mySqlCommand.Connection = mySqlConnection;

                    Response_Code = 1;
                    try
                    {
                        // datos del agente y del usuario 
                        //TRAN_
                        mySqlCommand.CommandText = " SELECT ag.age_id, ag.age_id_sup, ag.usr_id, ag.age_nombre, ag.ciu_id, ag.age_direccion, ag.age_razonsocial, ag.age_cuit, ag.age_tel, ag.age_email, ag.age_cel, ag.age_contacto, ag.age_observaciones,  ag.age_estado, ag.age_subNiveles, ag.age_tipo, ag.age_autenticaterminal, ag.age_prefijosrest, ag.age_fecalta, ag.usr_id_modificacion, ag.age_pdv, ag.age_entrecalles, ag.ct_id, ag.ta_id, ag.sa_id, ag.age_comisionadeposito, ag.age_montocomision, us.usr_nombre, us.usr_apellido, us.usr_estado,  Provincia.pro_id, Pais.pai_id FROM  Agente ag  WITH(NOLOCK) INNER JOIN Usuario us   WITH(NOLOCK) ON ag.usr_id = us.usr_id	INNER JOIN  Ciudad WITH(NOLOCK) ON ag.ciu_id = Ciudad.ciu_id INNER JOIN   Provincia WITH(NOLOCK) ON Ciudad.pro_id = Provincia.pro_id INNER JOIN    Pais  WITH(NOLOCK) ON Provincia.pai_id = Pais.pai_id		 WHERE  ag.age_id = @age_id";

                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@age_id", obj.age_id);

                        using (mySqlDataReader = mySqlCommand.ExecuteReader())
                        {
                            if (mySqlDataReader.HasRows && mySqlDataReader.Read())
                            {
                                obj.age_cuit = mySqlDataReader["age_cuit"].ToString();
                                obj.age_nombre = mySqlDataReader["age_nombre"].ToString();
                                obj.age_id_sup = (decimal)mySqlDataReader["age_id_sup"];
                                obj.age_razonsocial = mySqlDataReader["age_razonsocial"].ToString();
                                obj.age_direccion = mySqlDataReader["age_direccion"].ToString();
                                obj.age_entrecalles = mySqlDataReader["age_entrecalles"].ToString();
                                obj.age_ciu_id = (decimal)mySqlDataReader["ciu_id"];
                                obj.pro_id = (decimal)mySqlDataReader["pro_id"];
                                obj.pai_id = (decimal)mySqlDataReader["pai_id"];
                                obj.age_tel = mySqlDataReader["age_tel"].ToString();
                                obj.age_cel = mySqlDataReader["age_cel"].ToString();
                                obj.age_email = mySqlDataReader["age_email"].ToString();
                                obj.age_contacto = mySqlDataReader["age_contacto"].ToString();
                                obj.age_subNiveles = (decimal)mySqlDataReader["age_subNiveles"];
                                obj.age_pdv = mySqlDataReader["age_pdv"].ToString();
                                obj.age_comisionadeposito = mySqlDataReader["age_comisionadeposito"].ToString();
                                obj.age_montocomision = (decimal)mySqlDataReader["age_montocomision"];
                                obj.usr_nombre = mySqlDataReader["usr_nombre"].ToString();
                                obj.usr_apellido = mySqlDataReader["usr_apellido"].ToString();
                                obj.usr_id = (decimal)mySqlDataReader["usr_id"];
                                obj.ct_id = (decimal)mySqlDataReader["ct_id"];
                                obj.sa_id = (decimal)mySqlDataReader["sa_id"];
                            }
                            else
                            {
                                Message = "NO SE ENCONTRARON DATOS DEL AGENTE";

                                _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                                logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));

                                // result["Response_Code"]=  Response_Code;
                                //  result["Message"] =  Message;
                                //return result;
                                result.ResponseCode = Response_Code;
                                result.ResponseMessage = Message;
                                return result;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = "ERROR AL SELECCIONAR DATOS DEL AGENTE";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));

                        result.ResponseCode = Response_Code;
                        result.ResponseMessage = Message;
                        return result;
                    }

                    //si es administrador

                    Response_Code = 211;
                    try
                    {
                        //TRAN_
                        mySqlCommand.CommandText = "SELECT usr_id FROM RolUsuario WHERE usr_id = @usr_id and rol_id = @rol_id";

                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@usr_id", obj.usr_id);
                        mySqlCommand.Parameters.AddWithValue("@rol_id", 20);
                        object res = mySqlCommand.ExecuteScalar();
                        obj.usr_administrador = res != null;
                    }
                    catch (Exception ex)
                    {
                        Message = "ERROR AL VALIDAR ROLES DE AGENTE";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));

                        result.ResponseCode = Response_Code;
                        result.ResponseMessage = Message;
                        return result;
                    }


                    //accesos
                    //ENCAPSUALAR

                    Response_Code = 2;
                    try
                    {
                        IEnumerable<dynamic> listAccesos = GetAccesosByUser(obj.usr_id);

                        int i = 0;
                        ///TODO MAXIMO DE ACCESOS
                        ///CONSIDERAR LOGICA EN PROVIDER
                        int max = 3;
                        foreach (dynamic p in listAccesos)
                        {
                            if (i == max)
                                break;

                            switch (i)
                            {
                                case 0:

                                    obj.tac_id = (decimal)p.tac_id;
                                    obj.acc_login = p.acc_login;
                                    break;
                                case 1:
                                    obj.second_tac_id = (decimal)p.tac_id;
                                    obj.second_acc_login = p.acc_login;
                                    break;
                                case 2:
                                    obj.third_tac_id = (decimal)p.tac_id;
                                    obj.third_acc_login = p.acc_login;
                                    obj.av_sc_ac_secondUser = p.acc_estado.ToString().Equals("AC");
                                    break;
                            }
                            i++;


                        }


                    }
                    catch (Exception ex)
                    {
                        Message = "ERROR AL SELECCIONAR LOS ACCESOS";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));

                        result.ResponseCode = Response_Code;
                        result.ResponseMessage = Message;
                        return result;
                    }


                    Response_Code = 3;
                    try
                    {


                        // lsitado de atributos para invocar por reflexion de codigo para actualizar la entidad

                        obj.limiteCredito = "0";
                        obj.autorizacionAutomaticaMontoDiario = "0";
                        obj.montoMinimoPorPedido = "0";
                        obj.montoMaximoPorPedido = "0";
                        obj.pedidoMaximoMensual = "0";

                        obj.autorizacionAutomatica = "False";
                        obj.quitaAutomatica = "False";
                        obj.generacionAutomatica = "False";
                        obj.recargaAsincronica = "False";

                        // string[] attId = new string[] { "LimiteCredito", "AutorizacionAutomatica", "QuitaAutomatica", "GeneracionAutomatica", "MontoMinimoPorPedido", "MontoMaximoPorPedido", "PedidoMaximoMensual", "AutorizacionAutomaticaMontoDiario", "RecargaAsincronica" };
                        //se realiza un for un llamado de la propiedad por refelxion

                        string AtDominio = "AGENTE";
                        //TRAN_
                        mySqlCommand.CommandText = "SELECT ataValor,attId FROM  KcrAtributoAgencia WITH(NOLOCK) WHERE ageId = @ageId and attDominio =  @attDominio and attId in ('LimiteCredito', 'AutorizacionAutomatica', 'QuitaAutomatica', 'GeneracionAutomatica', 'MontoMinimoPorPedido', 'MontoMaximoPorPedido', 'PedidoMaximoMensual', 'AutorizacionAutomaticaMontoDiario', 'RecargaAsincronica' ) ";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@ageId", obj.age_id);

                        mySqlCommand.Parameters.AddWithValue("@attDominio", AtDominio);
                        //mySqlCommand.Parameters.AddWithValue("@attId", string.Join(",", attId));



                        using (var reader = mySqlCommand.ExecuteReader())
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                    switch ((string)reader["attId"])
                                    {
                                        //booleanos
                                        case "LimiteCredito":
                                            obj.limiteCredito = (string)reader["ataValor"];
                                            break;
                                        case "AutorizacionAutomatica":
                                            obj.autorizacionAutomatica = (string)reader["ataValor"];
                                            break;
                                        case "QuitaAutomatica":
                                            obj.quitaAutomatica = (string)reader["ataValor"];
                                            break;
                                        case "RecargaAsincronica":
                                            obj.recargaAsincronica = (string)reader["ataValor"];
                                            break;

                                        //valores numericos
                                        case "GeneracionAutomatica":
                                            obj.generacionAutomatica = (string)reader["ataValor"];
                                            break;
                                        case "MontoMinimoPorPedido":
                                            obj.montoMinimoPorPedido = (string)reader["ataValor"];
                                            break;
                                        case "MontoMaximoPorPedido":
                                            obj.montoMaximoPorPedido = (string)reader["ataValor"];
                                            break;
                                        case "PedidoMaximoMensual":
                                            obj.pedidoMaximoMensual = (string)reader["ataValor"];
                                            break;
                                        case "AutorizacionAutomaticaMontoDiario":
                                            obj.autorizacionAutomaticaMontoDiario = (string)reader["ataValor"];
                                            break;


                                    }
                        }

                    }
                    catch (Exception ex)
                    {


                        Message = "ERROR AL SELECCIONAR  KCRATRIBUTOAGENCIA ";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));


                        result.ResponseCode = Response_Code;
                        result.ResponseMessage = Message;
                        return result;
                    }




                    Response_Code = 4;
                    try
                    {
                        entId = GetEnitdadId(obj.age_id);
                    }
                    catch (Exception ex)
                    {

                        Message = "ERROR AL SELECCIONAR ENTIDAD EN COMISION";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);

                        logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));


                        result.ResponseCode = Response_Code;
                        result.ResponseMessage = Message;
                        return result;
                    }

                    //productos
                    Response_Code = 5;
                    //SE REEMPLAZA LA BASE DE DATOS DE TRAN_COL A COMI_COL
                    string strConnStringCOMI = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString.Replace("TRAN_", "COMI_");
                    bool value = Convert.ToBoolean(ConfigurationManager.AppSettings["COMISSION_BY_SALES"]);

                    using (SqlConnection mySqlConnectionCOMI = new SqlConnection(strConnStringCOMI))
                    {

                        mySqlConnectionCOMI.Open();

                        SqlCommand mySqlCommandCOMI = new SqlCommand();
                        // SqlTransaction tran = null;
                        SqlDataReader mySqlDataReaderCOMI = null;
                        mySqlCommandCOMI.Connection = mySqlConnectionCOMI;

                        #region comisiones

                        try
                        {
                            if (value && entId.HasValue)
                            {
                                //seleccionar desde la base de datos COM COL
                                // PUDE QUE TENAGA UNA COMISISON PARA TODOS 
                                //LOS PROPDUCTOS O PARA ESPECIFICA APRA ALGUNOS
                                //PENDIENTE SI DEBE HACER JOIN CON LOS PRODUCTOS
                                /*
                                 //COLOMBIA NO HAY MATCH ENTRE AGENTES EN TRAN Y COM 
                                 INNER JOIN [dbo].[KmmAgente] Kma  WITH(NOLOCK) 
                                                            ON Kma.entId =  kvep.entId  
                                                            INNER JOIN [dbo].[KlgAgenteProducto] kap on  kap.ageId = Kma.ageId and kvep.prdId = kap.prdId and kap.agpEstado = @agpEstado 
                                 
                                 */
                                // COMO SE HACE UN DELETE INSERT AL ACTUALIZAR SE CONFIA EN ESTE QUERY SIN NECESIDAD A VALIDAR EL ESTADO DE LA BASE DE DATOS
                                //TODO falta KlgAgenteProducto hacer join KlgAgenteProducto este con estado AC
                                //COMI:
                                //mySqlCommandCOMI.CommandText = @"SELECT  kvep.[prdId], kvep.[vepValor] as comision 
                                //                             FROM [dbo].[KmmVariableEntidadProductoAgen] kvep  WITH //(NOLOCK)
                                //WHERE  kvep.entId = @entId and kvep.varId = @varId and  kvep.mdlId = @mdlId
                                //order by kvep.[prdId]";
                                //commission PercentageKmm.Models.Commissions.PercentageOfSalesModel
                                //,[acc_password] ,[acc_lastlogin] ,[acc_intentos],[acc_cambiopassword] ,[acc_validityDate] ,[acc_terminalId]
                                //TODO PENDIENTE VALIDACION SI ES ESTADO AC
                                //SI NI ES ESTADO AC DEBERIA RETORNAR EL PRODUCTO CON LA COMISION REGISTRADA

                                mySqlCommandCOMI.CommandText = @"SELECT  kp.[prdIdExterno] as prdId, kvep.[vepValor] as comision FROM [dbo].[KmmVariableEntidadProductoAgen] kvep  WITH(NOLOCK) 
                                        JOIN [dbo].[KmmProducto] kp WITH(NOLOCK) on  kvep.[prdId] = kp.[prdId]
                                        WHERE  kvep.entId = @entId and kvep.varId = @varId and  kvep.mdlId = @mdlId
                                        order by kvep.[prdId]";

                                mySqlCommandCOMI.Parameters.Clear();
                                mySqlCommandCOMI.Parameters.AddWithValue("@entId", entId.Value);
                                mySqlCommandCOMI.Parameters.AddWithValue("@varId", "commissionPercentage");
                                mySqlCommandCOMI.Parameters.AddWithValue("@mdlId", "Kmm.Models.Commissions.PercentageOfSalesModel");
                                // mySqlCommand.Parameters.AddWithValue("@agpEstado", "AC");

                                using (mySqlDataReaderCOMI = mySqlCommandCOMI.ExecuteReader())
                                {
                                    //cambiar a una lista de acceso
                                    if (mySqlDataReaderCOMI.HasRows)
                                    {
                                        while (mySqlDataReaderCOMI.Read())
                                        {
                                            obj.productos.Add(new RequestAgent.Product()
                                            {
                                                comision = Convert.ToDecimal(mySqlDataReaderCOMI["comision"]),
                                                prdId = Decimal.ToInt32((decimal)mySqlDataReaderCOMI["prdId"])
                                            });
                                        }
                                    }
                                }
                            }
                            else
                            {
                                mySqlCommand.CommandText = "SELECT [prdId] , 0  comision FROM [dbo].[KlgAgenteProducto]  WITH(NOLOCK)  WHERE   [ageId]  = @ageId AND [agpEstado] = @agpEstado";
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@ageId", obj.age_id);
                                mySqlCommand.Parameters.AddWithValue("@agpEstado", "AC");
                                //TRAN_
                                using (mySqlDataReader = mySqlCommand.ExecuteReader())
                                {
                                    //cambiar a una lista de acceso
                                    if (mySqlDataReader.HasRows)
                                    {
                                        while (mySqlDataReader.Read())
                                        {
                                            obj.productos.Add(new RequestAgent.Product()
                                            {
                                                comision = Convert.ToDecimal(mySqlDataReader["comision"]),
                                                prdId = Decimal.ToInt32((decimal)mySqlDataReader["prdId"])
                                            });
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Message = "ERROR AL SELECCIONAR LOS PRODUCTOS";
                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommandCOMI)));

                            result.ResponseCode = Response_Code;
                            result.ResponseMessage = Message;
                            return result;
                        }


                        if (entId.HasValue)
                        {


                            Response_Code = 6;
                            ///TODO FALTA OBTENER  GRUPO ID KmmGrupoEntidad COMISIONES
                            try
                            {
                                //COMI_COL
                                mySqlCommandCOMI.CommandText = "SELECT Kgp.grpId FROM [dbo].[KmmGrupoEntidad] Kgp  WHERE  Kgp.[entId] =  @entId";
                                mySqlCommandCOMI.Parameters.Clear();
                                mySqlCommandCOMI.Parameters.AddWithValue("@entId", entId.Value);
                                using (mySqlDataReaderCOMI = mySqlCommandCOMI.ExecuteReader())
                                {
                                    //cambiar a una lista de acceso

                                    if (mySqlDataReaderCOMI.HasRows && mySqlDataReaderCOMI.Read())
                                    {
                                        obj.grpId = (decimal)mySqlDataReaderCOMI["grpId"];
                                    }
                                    else
                                    {
                                        //Message = "NO SE PUDO ACTUALIZAR EL ESTADO DE LA SOLICITUD DE PRODUCTO";
                                        // _m = String.Concat("[API] ", LOG_PREFIX, " ", Response_Code, "-", Message);
                                        logger.InfoLow(() => TagValue.New().Message("NO SE PUDO ENCONTRAR GRUPO PARA LA ENTIDAD").Message("[INPUT]").Value(GetParameters(mySqlCommandCOMI)));

                                        // result["Response_Code"] = Response_Code;
                                        //  result["Message"] = Message;
                                        // return result;
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                Message = "ERROR AL SELECCIONAR GRUPO ENTIDAD";
                                _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommandCOMI)));

                                result.ResponseCode = Response_Code;
                                result.ResponseMessage = Message;
                                return result;
                            }

                            Response_Code = 7;
                            //productos general
                            //se coloca el nombre de la base de datos en la conexion
                            try
                            {


                                //COMI_COL
                                mySqlCommandCOMI.CommandText = "SELECT kvea.varId,kvea.entId,kvea.ageId,kvea.mdlId,kvea.veaValor FROM   [dbo].KmmVariableEntidadAgente kvea  WITH(NOLOCK) where kvea.[entId]  = @entId and   kvea.varId in (@varId1,@varId2) and kvea.mdlId = @modId and kvea.ageId  = @ageId  order by kvea.varId";



                                mySqlCommandCOMI.Parameters.Clear();
                                mySqlCommandCOMI.Parameters.AddWithValue("@entId", entId.Value);
                                //mySqlCommand.Parameters.AddWithValue("@age_id", "A" + obj.age_id);
                                mySqlCommandCOMI.Parameters.AddWithValue("@varId1", "commissionPercentage");
                                mySqlCommandCOMI.Parameters.AddWithValue("@varId2", "impactOnMoneyBag");
                                mySqlCommandCOMI.Parameters.AddWithValue("@modId", "Kmm.Models.Commissions.PercentageOfSalesModel");
                                mySqlCommandCOMI.Parameters.AddWithValue("@ageId", obj.age_id_sup);

                                //valor por defecto
                                obj.commissionPercentage = -1;
                                obj.impactOnMoneyBag = false;

                                using (mySqlDataReaderCOMI = mySqlCommandCOMI.ExecuteReader())
                                {
                                    //cambiar a una lista de acceso

                                    if (mySqlDataReaderCOMI.HasRows)
                                    {

                                        while (mySqlDataReaderCOMI.Read())
                                        {
                                            #region byswitch
                                            /*
                                                 switch (mySqlDataReader["varId"].ToString())
                                                 {
                                                     case "commissionPercentage":
                                                         obj.commissionPercentage = (decimal)mySqlDataReader["veaValor"];
                                                         break;
                                                     case "impactOnMoneyBag":
                                                         obj.impactOnMoneyBag =Convert.ToBoolean( mySqlDataReader["veaValor"].ToString());
                                                         break;
                                                   
                                                 }*/
                                            #endregion
                                            object objval = mySqlDataReaderCOMI["veaValor"];
                                            if (objval != null && objval != DBNull.Value)
                                            {
                                                PropertyInfo propertyInfo = obj.GetType().GetProperty(mySqlDataReaderCOMI["varId"].ToString());
                                                propertyInfo.SetValue(obj, Convert.ChangeType(objval, propertyInfo.PropertyType), null);
                                            }
                                        }
                                    }


                                }

                                //valido que los dos productos esten
                                //   if (obj.impactOnMoneyBag &&  obj.commissionPercentage != -1)
                                /*
                                if(obj.productos.Count == 0)
                                {
                                    //codigo para datos onmoneybag
                                    /*
                                    if(obj.productos.Count > 0)
                                        obj.productos.Insert(0,new RequestAgent.Product() { prdId = 0, comision = obj.commissionPercentage });
                                    else
                                    obj.productos.Add(new RequestAgent.Product() { prdId = 0, comision = obj.commissionPercentage });


                                }*/
                            }
                            catch (Exception ex)
                            {
                                Message = "ERROR AL SELECCIONAR ATRIBUTOS DE COMISION";
                                _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommandCOMI)));

                                result.ResponseCode = Response_Code;
                                result.ResponseMessage = Message;
                                return result;
                            }

                        }
                        else
                        {
                            //marcar agente no tiene comision

                        }
                        #endregion
                    }


                    result.ResponseCode = 0;
                    result.ResponseMessage = "OK";
                    result.ObjectResult = obj;



                }
            }
            catch (Exception ex)
            {
                Response_Code = 90;
                Message = "ERROR ABRIR CONEXION DE BASE DE DATOS";
                _m = String.Concat("[QRY] ", LOG_PREFIX, " ", Response_Code, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                logger.ErrorLow(_m);

                result.ResponseCode = Response_Code;
                result.ResponseMessage = Message;
            }
            return result;

        }
        /// <summary>
        /// Método que retorna una enumeración (carga perezosa) de los accesos del usuario asociado al agente
        /// </summary>
        /// <param name="user_id">id del usuario a consultar los accesos</param>
        /// <returns>IEnumerable<dynamic> los cuales cumplen con la siguiente estructura {tac_id,acc_login,usr_id,acc_estado} </returns>
        private static IEnumerable<dynamic> GetAccesosByUser(decimal user_id)
        {
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;

            using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
            {

                mySqlConnection.Open();
                SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                SqlDataReader mySqlDataReader = null;
                mySqlCommand.CommandText = "SELECT [tac_id],[acc_login],[usr_id],[acc_estado]   FROM [dbo].[Acceso]  WITH(NOLOCK)    where [usr_id] = @usr_id order by [tac_id]";

                mySqlCommand.Parameters.AddWithValue("@usr_id", user_id);

                using (mySqlDataReader = mySqlCommand.ExecuteReader())
                {
                    if (mySqlDataReader.HasRows)
                    {
                        while (mySqlDataReader.Read())
                        {


                            yield return new { tac_id = (decimal)mySqlDataReader["tac_id"], acc_login = mySqlDataReader["acc_login"].ToString(), usr_id = (decimal)mySqlDataReader["usr_id"], acc_estado = mySqlDataReader["acc_estado"].ToString() };
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Crea una agente en el systema
        /// valida que los datos de los accessos  esten completos y no esten repetidos entre ellos
        /// 
        /// </summary>
        /// <param name="oRequest">Datos del nuevo agente</param>
        /// <returns>Retorna un response Agent Con codigo 0 si pudi crear el agente, delo contrario retorna un codigo diferente a 0, diferente a 0 con el respectivo mensaje</returns>
        public static ResponseAgent CreateAgent(RequestAgent oRequest)
        {
            decimal new_user = 0;
            //decimal second_new_user = 0;
            decimal new_age_id = 0;
            decimal new_count_id = 0;
            decimal secuencia = 0;
            //string sentencia = string.Empty;
            decimal age_id_sup = 0;
            int acc_login = 0;
            int second_acc_login = 0;
            int third_acc_login = 0;
            //decimal stkId = 0;
            //decimal prdId = 0;
            //decimal ageId = 0;
            //decimal staOrden = 0;
            //decimal stkCantidad = 0;
            decimal sec_number_stock = 0;
            string age_comisionadeposito = string.Empty;
            ResponseAgent oResp = new ResponseAgent();
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            SqlConnection mySqlConnection = new SqlConnection(strConnString);
            SqlCommand mySqlCommand = new SqlCommand();
            SqlTransaction tran = null;
            SqlDataReader mySqlDataReader = null;

            oResp.result = false;
            string _m = string.Empty;
            //StringBuilder sb = new StringBuilder();
            logger.InfoLow(() => TagValue.New().Tag("[OP]").Value("CreateAgent Crear Agente"));
            try
            {

                oResp.response_code = "01";

                #region VALIDACION DE ACCESSOS
                //AV
                //VALIDACION DE ACCESSOS
                //valida que los accessos esten completos y no tengan datos repetidos

                //primer acceso completo
                if (string.IsNullOrEmpty(oRequest.acc_login) || string.IsNullOrEmpty(oRequest.acc_password))
                {
                    oResp.message = "LOS DATOS DEL PRIMER ACCESO ESTAN INCOMPLETOS";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message));
                    return oResp;
                }

                //segundo acceso completo
                if (oRequest.second_tac_id <= 0m ||
                    string.IsNullOrEmpty(oRequest.second_acc_login) || string.IsNullOrEmpty(oRequest.second_acc_password))
                {
                    oResp.message = "LOS DATOS DEL SEGUNDO ACCESO ESTAN INCOMPLETOS";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message));
                    return oResp;
                }

                // verificacion de valores entre el primer acceso y el segundo
                if (oRequest.second_tac_id == cons.ACCESS_WEB)// .ACCESO_WEB)
                {
                    oResp.message = "TIPO DE ACCESO INVALIDO PARA SEGUNDO ACCESO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message));
                    return oResp;
                }

                if (oRequest.acc_login.Equals(oRequest.second_acc_login, StringComparison.InvariantCultureIgnoreCase))
                {
                    oResp.message = "LOS DATOS DEL SEGUNDO ACCESO (LOGIN) NO PUEDEN SER IGUALES AL PRIMER ACCESO";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message));
                    return oResp;
                }
                //


                // si el segundo dato del agente esta habilitado
                if (oRequest.av_sc_ac_secondUser)
                {

                    //tercer acceso completo datos completos
                    if (oRequest.third_tac_id <= 0m ||
                      string.IsNullOrEmpty(oRequest.third_acc_login) ||
                       string.IsNullOrEmpty(oRequest.third_acc_password))
                    {
                        oResp.message = "LOS DATOS DEL TERCER ACCESSO ESTAN INCOMPLETOS";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message));
                        return oResp;
                    }

                    //verificacion entre el primer y tercer acceso
                    if (oRequest.third_tac_id == cons.ACCESS_WEB || oRequest.third_acc_login.Equals(oRequest.acc_login, StringComparison.InvariantCultureIgnoreCase))
                    {
                        oResp.message = "LOS DATOS DEL TERCER ACCESO (TIPO O LOGIN) NO PUEDEN SER IGUALES AL PRIMER ACCESO";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message));
                        return oResp;
                    }


                    //verificacion entre el segundo y tercer acceso
                    if (oRequest.third_tac_id == oRequest.second_tac_id ||
                    oRequest.third_acc_login.Equals(oRequest.second_acc_login, StringComparison.InvariantCultureIgnoreCase))
                    {
                        oResp.message = "LOS DATOS DEL TERCER ACCESO (TIPO O LOGIN) NO PUEDEN SER IGUALES AL SEGUNDO ACCESO";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message));
                        return oResp;
                    }
                }


                #endregion


                mySqlConnection.Open();
                //comiensa la transaccion
                tran = mySqlConnection.BeginTransaction();
                mySqlCommand.Transaction = tran;
                mySqlCommand.Connection = mySqlConnection;
                mySqlCommand.CommandTimeout = 120;

                //primer acceso
                oResp.response_code = "02";
                try
                {
                    mySqlCommand.CommandText = "SELECT Count(*) FROM Acceso WHERE acc_login=@loginname";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@loginname", oRequest.acc_login);
                    acc_login = (int)mySqlCommand.ExecuteScalar();
                    if (acc_login > 0)
                    {

                        oResp.message = "EL PRIMER ACCESO ESTA ACTUALMENTE EN USO POR OTRO USUARIO";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message));
                        tran.Rollback();
                        return oResp;
                    }
                }
                catch (Exception ex)
                {
                    oResp.message = "ERROR AL CONSULTAR EL ACCESO PRINCIPAL DEL USUARIO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return oResp;
                }


                //segundo usuario acceso
                oResp.response_code = "03";
                try
                {
                    mySqlCommand.CommandText = "SELECT Count(*) FROM Acceso WHERE acc_login=@loginname";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@loginname", oRequest.second_acc_login);
                    second_acc_login = (int)mySqlCommand.ExecuteScalar();

                    if (second_acc_login > 0)
                    {
                        oResp.message = "EL SEGUNDO ACCESO ESTA ACTUALMENTE EN USO POR OTRO USUARIO";
                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message));
                        tran.Rollback();
                        return oResp;
                    }
                }
                catch (Exception ex)
                {
                    oResp.message = "ERROR AL CONSULTAR EL SEGUNDO ACCESO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return oResp;
                }


                //tercer acceso
                if (oRequest.av_sc_ac_secondUser)
                {
                    oResp.response_code = "04";
                    try
                    {
                        mySqlCommand.CommandText = "SELECT Count(*) FROM Acceso WHERE acc_login=@loginname";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@loginname", oRequest.third_acc_login);
                        third_acc_login = (int)mySqlCommand.ExecuteScalar();

                        if (third_acc_login > 0)
                        {
                            oResp.message = "EL TERCER ACCESSO ESTA EN USO POR OTRO USUARIO";
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message));
                            tran.Rollback();
                            return oResp;
                        }
                    }
                    catch (Exception ex)
                    {
                        oResp.message = "ERROR AL CONSULTAR EL TERCER ACCESO";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return oResp;
                    }
                }




                oResp.response_code = "05";
                //Creamos el primer usuario
                try
                {
                    mySqlCommand.CommandText = "UPDATE secuencia SET sec_number=sec_number+1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "USUARIO");

                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        oResp.message = "NO SE PUDO ACTUALIZAR LA SECUENCIA DEL USUARIO";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return oResp;
                    }
                }
                catch (Exception ex)
                {
                    oResp.message = "ERROR AL ACTUALIZAR LA SECUENCIA DEL USUARIO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return oResp;

                }



                try
                {
                    mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "USUARIO");
                    new_user = Convert.ToDecimal(mySqlCommand.ExecuteScalar());
                }
                catch (Exception ex)
                {

                    oResp.response_code = "06";
                    oResp.message = "NO SE PUDO LEER LA SECUENCIA DEL USUARIO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return oResp;

                }



                oResp.response_code = "07";
                try
                {
                    mySqlCommand.CommandText = "UPDATE secuencia SET sec_number = sec_number+1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "AGENTE");

                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        oResp.message = "NO SE PUDO ACTUALIZAR LA SECUENCIA DE LA AGENCIA";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return oResp;
                    }
                }
                catch (Exception ex)
                {

                    oResp.message = "NO SE PUDO ACTUALIZAR LA SECUENCIA DE LA AGENCIA";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return oResp;
                }


                try
                {
                    mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "AGENTE");
                    new_age_id = Convert.ToDecimal(mySqlCommand.ExecuteScalar());
                }
                catch (Exception ex)
                {

                    oResp.response_code = "08";
                    oResp.message = "ERROR AL SELECCIONAR LA SECUENCIA DE LA AGENCIA";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return oResp;
                }


                //Creamos el 1er usuario
                try
                {
                    /*b.Append(String.Format("INSERT INTO Usuario (usr_id,usr_estado,usr_id_sup,usr_nombre,usr_apellido,usr_tipo_doc,usr_num_doc,usr_domicilio,usr_telLaboral,usr_telPersonal,usr_telCelular,ciu_id,usr_barrio,age_id,usr_email,usr_horaLaboralDesde,usr_minutosLaboralDesde,usr_horaLaboralHasta,usr_minutosLaboralHasta)" +
                                 "VALUES ({0},'AC',null,'{1}','{2}',null,null,null,null,null,null,{3},null,{4},null,0,0,23,59)", new_user.ToString(), oRequest.usr_nombre, oRequest.usr_apellido, oRequest.age_ciu_id.ToString(), new_age_id));*/
                    //cambiar
                    mySqlCommand.CommandText = "INSERT INTO Usuario (usr_id,usr_estado,usr_id_sup,usr_nombre,usr_apellido,usr_tipo_doc,usr_num_doc,usr_domicilio,usr_telLaboral,usr_telPersonal,usr_telCelular,ciu_id,usr_barrio,age_id,usr_email,usr_horaLaboralDesde,usr_minutosLaboralDesde,usr_horaLaboralHasta,usr_minutosLaboralHasta) VALUES (@userId,@usr_estado,@usr_id_sup,@userName,@userLastName,@usr_tipo_doc,@usr_num_doc,@usr_domicilio,@usr_telLaboral,@usr_telPersonal,@usr_telCelular,@cityId,@usr_barrio,@ageId,@usr_email,@usr_horaLaboralDesde,@usr_minutosLaboralDesde,@usr_horaLaboralHasta,@usr_minutosLaboralHasta)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@userId", new_user);
                    mySqlCommand.Parameters.AddWithValue("@usr_estado", "AC");
                    mySqlCommand.Parameters.AddWithValue("@usr_id_sup", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@userName", oRequest.usr_nombre);
                    mySqlCommand.Parameters.AddWithValue("@userLastName", oRequest.usr_apellido);
                    mySqlCommand.Parameters.AddWithValue("@usr_tipo_doc", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@usr_num_doc", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@usr_domicilio", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@usr_telLaboral", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@usr_telPersonal", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@usr_telCelular", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@cityId", oRequest.age_ciu_id);
                    mySqlCommand.Parameters.AddWithValue("@usr_barrio", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                    mySqlCommand.Parameters.AddWithValue("@usr_email", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@usr_horaLaboralDesde", 0);
                    mySqlCommand.Parameters.AddWithValue("@usr_minutosLaboralDesde", 0);
                    mySqlCommand.Parameters.AddWithValue("@usr_horaLaboralHasta", 23);
                    mySqlCommand.Parameters.AddWithValue("@usr_minutosLaboralHasta", 59);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "09";
                    oResp.message = "ERROR AL CREAR USUARIO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    //sb.Append(String.Format("SELECT top 1 a.age_id FROM [Agente] a WITH(NOLOCK) join [Usuario] u WITH(NOLOCK) on a.age_id=u.age_id where u.usr_id in ({0})", oRequest.usr_id_modificacion));
                    mySqlCommand.CommandText = "SELECT  a.age_id FROM [Agente] a WITH(NOLOCK) JOIN [Usuario] u WITH(NOLOCK) ON a.age_id=u.age_id WHERE u.usr_id=@userId";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@userId", oRequest.usr_id_modificacion);
                    age_id_sup = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "10";
                    oResp.message = "ERROR AL SELECCIONAR LA AGENCIA PADRE DEL USUARIO ENVIADO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return oResp;
                }

                //Validamos que tengan las columnas de comision en la tabla agencia 
                try
                {
                    /*sb.Append(String.Format("INSERT INTO Agente (age_id,age_id_sup,usr_id,age_nombre,ciu_id,age_direccion,age_razonsocial,age_cuit,age_tel,age_email,age_cel,age_contacto,age_observaciones,age_estado,age_subNiveles,age_tipo,age_autenticaterminal,age_prefijosrest,age_fecalta,usr_id_modificacion,age_pdv,age_entrecalles,ct_id,ta_id,sa_id)" +
                                "VALUES ({0},{1},{2},'{3}',{4},'{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','AC',{13},'{14}','{15}','{16}','{17}',{18},'{19}','{20}',{21},{22},{23})", 
                                new_age_id.ToString("F"), age_id_sup.ToString("F"),new_user.ToString("F"),oRequest.age_nombre,oRequest.age_ciu_id.ToString("F"),oRequest.age_direccion,oRequest.age_razonsocial, oRequest.age_cuit, oRequest.age_tel,oRequest.age_email,
                                oRequest.age_cel,oRequest.age_contacto,oRequest.age_observaciones,oRequest.age_subNiveles.ToString("F"),oRequest.age_tipo,oRequest.age_autenticaterminal,oRequest.age_prefijosrest,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                oRequest.usr_id_modificacion.ToString("F"),oRequest.age_pdv,oRequest.age_entrecalles,oRequest.ct_id.ToString("F"),oRequest.ta_id.ToString("F"),oRequest.sa_id.ToString("F")));*/
                    mySqlCommand.CommandText = @"INSERT INTO
                        Agente (age_id, age_id_sup, usr_id, age_nombre, ciu_id,age_direccion, age_razonsocial, age_cuit, age_tel, age_email, age_cel, age_contacto, age_observaciones, age_estado, age_subNiveles, age_tipo, age_autenticaterminal, age_prefijosrest, age_fecalta, usr_id_modificacion, age_pdv, age_entrecalles, ct_id, ta_id, sa_id, age_comisionadeposito, age_montocomision)
                        VALUES (@ageId, @ageIdSup, @userId, @ageName, @cityId, @ageAdress, @ageLegalNumber, @ageCuit, @agePhone, @ageEmail, @ageMobile, @ageContact, @ageObserv, @age_estado, @ageSubLevels, @ageType, @ageAuth, @agePrefix, @ageDate, @ageUserId, @agePdv, @ageStreets, @ageCt, @ageTa, @ageSa, @ageCommission, @ageCommissionAmount)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                    mySqlCommand.Parameters.AddWithValue("@ageIdSup", age_id_sup);
                    mySqlCommand.Parameters.AddWithValue("@userId", new_user);
                    mySqlCommand.Parameters.AddWithValue("@ageName", oRequest.age_nombre);
                    mySqlCommand.Parameters.AddWithValue("@cityId", oRequest.age_ciu_id);
                    mySqlCommand.Parameters.AddWithValue("@ageAdress", oRequest.age_direccion);
                    mySqlCommand.Parameters.AddWithValue("@ageLegalNumber", oRequest.age_razonsocial);
                    mySqlCommand.Parameters.AddWithValue("@ageCuit", oRequest.age_cuit);
                    mySqlCommand.Parameters.AddWithValue("@agePhone", oRequest.age_tel);
                    mySqlCommand.Parameters.AddWithValue("@ageEmail", oRequest.age_email);
                    mySqlCommand.Parameters.AddWithValue("@ageMobile", oRequest.age_cel);
                    mySqlCommand.Parameters.AddWithValue("@ageContact", string.IsNullOrEmpty(oRequest.age_contacto) ? string.Empty : oRequest.age_contacto);
                    mySqlCommand.Parameters.AddWithValue("@ageObserv", string.IsNullOrEmpty(oRequest.age_observaciones) ? string.Empty : oRequest.age_observaciones);
                    mySqlCommand.Parameters.AddWithValue("@age_estado", "AC");
                    mySqlCommand.Parameters.AddWithValue("@ageSubLevels", oRequest.age_subNiveles);
                    mySqlCommand.Parameters.AddWithValue("@ageType", oRequest.age_tipo);
                    mySqlCommand.Parameters.AddWithValue("@ageAuth", oRequest.age_autenticaterminal);
                    mySqlCommand.Parameters.AddWithValue("@agePrefix", oRequest.age_prefijosrest);
                    mySqlCommand.Parameters.AddWithValue("@ageDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    mySqlCommand.Parameters.AddWithValue("@ageUserId", oRequest.usr_id_modificacion);
                    mySqlCommand.Parameters.AddWithValue("@agePdv", string.IsNullOrEmpty(oRequest.age_pdv) ? string.Empty : oRequest.age_pdv);
                    mySqlCommand.Parameters.AddWithValue("@ageStreets", string.IsNullOrEmpty(oRequest.age_entrecalles) ? string.Empty : oRequest.age_entrecalles);
                    mySqlCommand.Parameters.AddWithValue("@ageCt", oRequest.ct_id);
                    mySqlCommand.Parameters.AddWithValue("@ageTa", oRequest.ta_id);
                    mySqlCommand.Parameters.AddWithValue("@ageSa", oRequest.sa_id);
                    mySqlCommand.Parameters.AddWithValue("@ageCommission", string.IsNullOrEmpty(oRequest.age_comisionadeposito) ? "N" : oRequest.age_comisionadeposito);
                    mySqlCommand.Parameters.AddWithValue("@ageCommissionAmount", string.IsNullOrEmpty(oRequest.age_comisionadeposito) || oRequest.age_comisionadeposito.Equals("N") ? 0m : oRequest.age_montocomision);
                    mySqlCommand.ExecuteNonQuery();

                }
                catch (Exception ex)
                {

                    oResp.response_code = "11";
                    oResp.message = "ERROR AL CREAR LA AGENCIA";
                    _m = string.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return oResp;
                }

                //Configuramos los atributos de la agencia
                try
                {
                    /*sentencia = "INSERT INTO KcrAtributoAgencia (ageId,attId,attDominio,ataValor)" +
                                    "VALUES ( " + new_age_id + ",'LimiteCredito'," + "'AGENTE'" + "," + "'" + oRequest.limiteCredito + "'" + ")";*/
                    mySqlCommand.CommandText = "INSERT INTO KcrAtributoAgencia (ageId,attId,attDominio,ataValor) VALUES (@ageId,@attId,@attDomain,@attValue)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                    mySqlCommand.Parameters.AddWithValue("@attId", "LimiteCredito");
                    mySqlCommand.Parameters.AddWithValue("@attDomain", "AGENTE");
                    mySqlCommand.Parameters.AddWithValue("@attValue", oRequest.limiteCredito);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "12";
                    oResp.message = "ERROR AL CREAR  ATRIBUTO AGENCIA LimiteCredito";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    /*sentencia = "INSERT INTO KcrAtributoAgencia (ageId,attId,attDominio,ataValor)" +
                                 "VALUES ( " + new_age_id + ",'AutorizacionAutomatica'," + "'AGENTE'" + "," + "'" + oRequest.autorizacionAutomatica + "'" + ")";*/
                    mySqlCommand.CommandText = "INSERT INTO KcrAtributoAgencia (ageId,attId,attDominio,ataValor) VALUES (@ageId,@attId,@attDomain,@attValue)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                    mySqlCommand.Parameters.AddWithValue("@attId", "AutorizacionAutomatica");
                    mySqlCommand.Parameters.AddWithValue("@attDomain", "AGENTE");
                    mySqlCommand.Parameters.AddWithValue("@attValue", oRequest.autorizacionAutomatica);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "13";
                    oResp.message = "ERROR AL CREAR  ATRIBUTO AGENCIA  AutorizacionAutomatica";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    /* sentencia = "INSERT INTO KcrAtributoAgencia (ageId,attId,attDominio,ataValor)" +
                                    "VALUES ( " + new_age_id + ",'QuitaAutomatica'," + "'AGENTE'" + "," + "'" + oRequest.quitaAutomatica + "'" + ")";*/
                    mySqlCommand.CommandText = "INSERT INTO KcrAtributoAgencia (ageId,attId,attDominio,ataValor) VALUES (@ageId,@attId,@attDomain,@attValue)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                    mySqlCommand.Parameters.AddWithValue("@attId", "QuitaAutomatica");
                    mySqlCommand.Parameters.AddWithValue("@attDomain", "AGENTE");
                    mySqlCommand.Parameters.AddWithValue("@attValue", oRequest.quitaAutomatica);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "14";
                    oResp.message = "ERROR AL CREAR ATRIBUTO AGENCIA QuitaAutomatica";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    /*sentencia = "INSERT INTO KcrAtributoAgencia (ageId,attId,attDominio,ataValor)" +
                                     "VALUES ( " + new_age_id + ",'GeneracionAutomatica'," + "'AGENTE'" + "," + "'" + oRequest.generacionAutomatica + "'" + ")";*/
                    mySqlCommand.CommandText = "INSERT INTO KcrAtributoAgencia (ageId,attId,attDominio,ataValor) VALUES (@ageId,@attId,@attDomain,@attValue)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                    mySqlCommand.Parameters.AddWithValue("@attId", "GeneracionAutomatica");
                    mySqlCommand.Parameters.AddWithValue("@attDomain", "AGENTE");
                    mySqlCommand.Parameters.AddWithValue("@attValue", oRequest.generacionAutomatica);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "15";
                    oResp.message = "ERROR AL CREAR ATRIBUTO AGENCIA GENERACION AUTOMATICA GeneracionAutomatica";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    //no hay rollback
                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    /*sentencia = "INSERT INTO KcrAtributoAgencia (ageId,attId,attDominio,ataValor)" +
                                     "VALUES ( " + new_age_id + ",'MontoMinimoPorPedido'," + "'AGENTE'" + "," + "'" + oRequest.montoMinimoPorPedido + "'" + ")";*/
                    mySqlCommand.CommandText = String.Concat("INSERT INTO KcrAtributoAgencia (ageId,attId,attDominio,ataValor) ",
                                                            "VALUES (@ageId,@attId,@attDomain,@attValue)");
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                    mySqlCommand.Parameters.AddWithValue("@attId", "MontoMinimoPorPedido");
                    mySqlCommand.Parameters.AddWithValue("@attDomain", "AGENTE");
                    mySqlCommand.Parameters.AddWithValue("@attValue", oRequest.montoMinimoPorPedido);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "16";
                    oResp.message = "ERROR AL CREAR ATRIBUTO AGENCIA MontoMinimoPorPedido";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    /* sentencia = "INSERT INTO KcrAtributoAgencia (ageId,attId,attDominio,ataValor)" +
                                    "VALUES ( " + new_age_id + ",'MontoMaximoPorPedido'," + "'AGENTE'" + "," + "'" + oRequest.montoMaximoPorPedido + "'" + ")";
                    mySqlCommand.CommandText = sentencia;*/
                    mySqlCommand.CommandText = String.Concat("INSERT INTO KcrAtributoAgencia (ageId,attId,attDominio,ataValor) ",
                                                            "VALUES (@ageId,@attId,@attDomain,@attValue)");
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                    mySqlCommand.Parameters.AddWithValue("@attId", "MontoMaximoPorPedido");
                    mySqlCommand.Parameters.AddWithValue("@attDomain", "AGENTE");
                    mySqlCommand.Parameters.AddWithValue("@attValue", oRequest.montoMaximoPorPedido);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "17";
                    oResp.message = "ERROR AL CREAR ATRIBUTO AGENCIA MontoMaximoPorPedido";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    /*sentencia = "INSERT INTO KcrAtributoAgencia (ageId,attId,attDominio,ataValor)" +
                                    "VALUES ( " + new_age_id + ",'PedidoMaximoMensual'," + "'AGENTE'" + "," + "'" + oRequest.pedidoMaximoMensual + "'" + ")";*/
                    mySqlCommand.CommandText = "INSERT INTO KcrAtributoAgencia (ageId,attId,attDominio,ataValor) VALUES (@ageId,@attId,@attDomain,@attValue)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                    mySqlCommand.Parameters.AddWithValue("@attId", "PedidoMaximoMensual");
                    mySqlCommand.Parameters.AddWithValue("@attDomain", "AGENTE");
                    mySqlCommand.Parameters.AddWithValue("@attValue", oRequest.pedidoMaximoMensual);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "18";
                    oResp.message = "ERROR AL CREAR ATRIBUTO AGENCIA PedidoMaximoMensual";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    /*sentencia = "INSERT INTO KcrAtributoAgencia (ageId,attId,attDominio,ataValor)" +
                                "VALUES ( " + new_age_id + ",'AutorizacionAutomaticaMontoDiario'," + "'AGENTE'" + "," + "'" + oRequest.autorizacionAutomaticaMontoDiario + "'" + ")";*/
                    mySqlCommand.CommandText = "INSERT INTO KcrAtributoAgencia (ageId,attId,attDominio,ataValor) VALUES (@ageId,@attId,@attDomain,@attValue)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                    mySqlCommand.Parameters.AddWithValue("@attId", "AutorizacionAutomaticaMontoDiario");
                    mySqlCommand.Parameters.AddWithValue("@attDomain", "AGENTE");
                    mySqlCommand.Parameters.AddWithValue("@attValue", oRequest.autorizacionAutomaticaMontoDiario);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "19";
                    oResp.message = "ERROR AL CREAR ATRIBUTO AGENCIA AutorizacionAutomaticaMontoDiario";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    /*sentencia = "INSERT INTO KcrAtributoAgencia (ageId,attId,attDominio,ataValor)" +
                                    "VALUES ( " + new_age_id + ",'RecargaAsincronica'," + "'AGENTE'" + "," + "'" + oRequest.recargaAsincronica + "'" + ")";*/
                    mySqlCommand.CommandText = "INSERT INTO KcrAtributoAgencia (ageId,attId,attDominio,ataValor) VALUES (@ageId,@attId,@attDomain,@attValue)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                    mySqlCommand.Parameters.AddWithValue("@attId", "RecargaAsincronica");
                    mySqlCommand.Parameters.AddWithValue("@attDomain", "AGENTE");
                    mySqlCommand.Parameters.AddWithValue("@attValue", oRequest.recargaAsincronica);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "20";
                    oResp.message = "ERROR AL CREAR ATRIBUTO AGENCIA RecargaAsincronica";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                //Creaamos el acceso para el primer usuario 
                //acceso web 
                //PRIMER ACCESO
                try
                {
                    /*sentencia = "INSERT INTO Acceso (tac_id,usr_id,acc_login,acc_password,acc_lastlogin,acc_intentos,acc_estado,acc_cambiopassword,acc_validityDate)" +
                                 "VALUES ( 1," + new_user.ToString() + "," + "'" + oRequest.acc_login + "'" + "," + "'" + oRequest.acc_password + "'" + ",null,0,'AC','N',NULL" + ")";*/
                    mySqlCommand.CommandText = "INSERT INTO Acceso (tac_id,usr_id,acc_login,acc_password,acc_lastlogin,acc_intentos,acc_estado,acc_cambiopassword,acc_validityDate) VALUES (@tacId,@userId,@loginName,@password,@acc_lastlogin,@acc_intentos,@acc_estado,@changePwd,@validityDate)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@tacId", cons.ACCESS_WEB);//ACCESO_WEB);
                    mySqlCommand.Parameters.AddWithValue("@userId", new_user);
                    mySqlCommand.Parameters.AddWithValue("@loginName", oRequest.acc_login);
                    mySqlCommand.Parameters.AddWithValue("@password", oRequest.acc_password);
                    mySqlCommand.Parameters.AddWithValue("@acc_lastlogin", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@acc_intentos", 0);
                    mySqlCommand.Parameters.AddWithValue("@acc_estado", "AC");
                    mySqlCommand.Parameters.AddWithValue("@changePwd", string.IsNullOrEmpty(oRequest.acc_cambiopassword) ? "N" : oRequest.acc_cambiopassword);

                    if (string.IsNullOrEmpty(oRequest.acc_cambiopassword))
                        mySqlCommand.Parameters.AddWithValue("@validityDate", DBNull.Value);
                    else
                        mySqlCommand.Parameters.AddWithValue("@validityDate", oRequest.acc_validityDate);

                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "21";
                    oResp.message = "ERROR AL CREAR ACCESO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                //Creamos el acceso del tipo seleccionado para el mismo usuario 

                //SEGUNDO ACCESO
                try
                {
                    /*sentencia = "INSERT INTO Acceso (tac_id,usr_id,acc_login,acc_password,acc_lastlogin,acc_intentos,acc_estado,acc_cambiopassword,acc_validityDate)" +
                                 "VALUES ( " + oRequest.second_tac_id + "," + new_user.ToString() + "," + "'" + oRequest.second_acc_login + "'" + "," + "'" + oRequest.second_acc_password + "'" + ",null,0,'AC','N',NULL" + ")";*/
                    mySqlCommand.CommandText = "INSERT INTO Acceso (tac_id,usr_id,acc_login,acc_password,acc_lastlogin,acc_intentos,acc_estado,acc_cambiopassword,acc_validityDate) VALUES (@tacId,@userId,@loginName,@password,@acc_lastlogin,@acc_intentos,@acc_estado,@changePwd,@validityDate)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@tacId", oRequest.second_tac_id);
                    mySqlCommand.Parameters.AddWithValue("@userId", new_user);
                    mySqlCommand.Parameters.AddWithValue("@loginName", oRequest.second_acc_login);
                    mySqlCommand.Parameters.AddWithValue("@password", oRequest.second_acc_password);

                    mySqlCommand.Parameters.AddWithValue("@acc_lastlogin", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@acc_intentos", 0);
                    mySqlCommand.Parameters.AddWithValue("@acc_estado", "AC");

                    mySqlCommand.Parameters.AddWithValue("@changePwd", string.IsNullOrEmpty(oRequest.second_acc_cambiopassword) ? "N" : oRequest.second_acc_cambiopassword);

                    if (string.IsNullOrEmpty(oRequest.second_acc_cambiopassword))
                        mySqlCommand.Parameters.AddWithValue("@validityDate", DBNull.Value);
                    else
                        mySqlCommand.Parameters.AddWithValue("@validityDate", oRequest.second_acc_validityDate);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "22";
                    oResp.message = "ERROR AL CREAR SEGUNDO ACCESO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                //TERCER ACEESO
                if (oRequest.av_sc_ac_secondUser)
                {
                    try
                    {
                        /*sentencia = "INSERT INTO Acceso (tac_id,usr_id,acc_login,acc_password,acc_lastlogin,acc_intentos,acc_estado,acc_cambiopassword,acc_validityDate)" +
                                     "VALUES ( " + oRequest.second_tac_id + "," + new_user.ToString() + "," + "'" + oRequest.second_acc_login + "'" + "," + "'" + oRequest.second_acc_password + "'" + ",null,0,'AC','N',NULL" + ")";*/
                        mySqlCommand.CommandText = "INSERT INTO Acceso (tac_id,usr_id,acc_login,acc_password,acc_lastlogin,acc_intentos,acc_estado,acc_cambiopassword,acc_validityDate) VALUES (@tacId,@userId,@loginName,@password,@acc_lastlogin,@acc_intentos,@acc_estado,@changePwd,@validityDate)";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@tacId", oRequest.third_tac_id);
                        mySqlCommand.Parameters.AddWithValue("@userId", new_user);
                        mySqlCommand.Parameters.AddWithValue("@loginName", oRequest.third_acc_login);
                        mySqlCommand.Parameters.AddWithValue("@password", oRequest.third_acc_password);

                        mySqlCommand.Parameters.AddWithValue("@acc_lastlogin", DBNull.Value);
                        mySqlCommand.Parameters.AddWithValue("@acc_intentos", 0);
                        mySqlCommand.Parameters.AddWithValue("@acc_estado", "AC");

                        mySqlCommand.Parameters.AddWithValue("@changePwd", string.IsNullOrEmpty(oRequest.third_acc_cambiopassword) ? "N" : oRequest.second_acc_cambiopassword);

                        if (string.IsNullOrEmpty(oRequest.third_acc_cambiopassword))
                            mySqlCommand.Parameters.AddWithValue("@validityDate", DBNull.Value);
                        else
                            mySqlCommand.Parameters.AddWithValue("@validityDate", oRequest.second_acc_validityDate);

                        mySqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {

                        oResp.response_code = "23";
                        oResp.message = "ERROR AL CREAR SEGUNDO ACCESO";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return oResp;
                    }
                }
                //Creamos el rol administrador del primer usuario
                try
                {
                    /* sentencia = "INSERT INTO RolUsuario (usr_id,rol_id) " +
                                 "VALUES ( " + new_user.ToString() + ",1)";*/
                    mySqlCommand.CommandText = "INSERT INTO RolUsuario (usr_id,rol_id) VALUES (@userId,@roleId)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@userId", new_user);
                    mySqlCommand.Parameters.AddWithValue("@roleId", 1);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "24";
                    oResp.message = "ERROR ALCREAR ROL ADMINISTRADOR DEL 1ER. USUARIO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    /* sentencia = "INSERT INTO RolUsuario (usr_id,rol_id)" +
                                "VALUES ( " + new_user.ToString() + ",4)";*/
                    mySqlCommand.CommandText = "INSERT INTO RolUsuario (usr_id,rol_id) VALUES (@userId,@roleId)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@userId", new_user);
                    mySqlCommand.Parameters.AddWithValue("@roleId", 4);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "25";
                    oResp.message = "ERROR AL INSERTAR ROL WEB DEL 1ER. USUARIO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                //Creamos el rol vendedor del usuario
                try
                {
                    /*sentencia = "INSERT INTO RolUsuario (usr_id,rol_id) " +
                                 "VALUES ( " + new_user.ToString() + ",3)";*/
                    mySqlCommand.CommandText = "INSERT INTO RolUsuario (usr_id,rol_id) VALUES (@userId,@roleId)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@userId", new_user);
                    mySqlCommand.Parameters.AddWithValue("@roleId", 3);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "26";
                    oResp.message = "ERROR AL INSERTAR ROL VENDEDOR DEL 1er. USUARIO";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                // Agregamos el rol Administrador PosWEB al usuario
                if (oRequest.usr_administrador)
                {
                    try
                    {
                        /* sentencia = "INSERT INTO RolUsuario (usr_id,rol_id)" +
                                    "VALUES ( " + new_user.ToString() + ",20)";*/
                        mySqlCommand.CommandText = "INSERT INTO RolUsuario (usr_id,rol_id) VALUES (@userId,@roleId)";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@userId", new_user);
                        mySqlCommand.Parameters.AddWithValue("@roleId", 20);
                        mySqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {

                        oResp.response_code = "27";
                        oResp.message = "ERROR AL INSERTAR ROL ADMINISTRADOR POSWEB DEL 1er. USUARIO";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                        tran.Rollback();
                        return oResp;
                    }
                }

                oResp.response_code = "28";
                try
                {
                    mySqlCommand.CommandText = "UPDATE secuencia SET sec_number=sec_number+1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "CUENTACORRIENTE");

                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {

                        //se lanza una excepcion sin mensaje para que se registre el error en catch
                        oResp.message = "NO SE PUDO ACTUALIZAR SECUENCIA CUENTACORRIENTE";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return oResp;
                    }
                }
                catch (Exception ex)
                {


                    oResp.message = "ERROR AL ACTUALIZAR LA SECUENCIA DE LA CUENTACORRIENTE";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "CUENTACORRIENTE");
                    new_count_id = Convert.ToDecimal(mySqlCommand.ExecuteScalar());
                }
                catch (Exception ex)
                {

                    oResp.response_code = "29";
                    oResp.message = "ERROR AL SELECCIONAR LA SECUENCIA DE LA CUENTACORRIENTE";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    /*sentencia = "INSERT INTO KfnCuentaCorriente (ctaId,ctaSaldo,ageId)" +
                                 "VALUES ( " + new_count_id + ",0," + new_age_id + ")";*/
                    mySqlCommand.CommandText = "INSERT INTO KfnCuentaCorriente (ctaId,ctaSaldo,ageId) VALUES (@ctaId,@ctaBalance,@ageId)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ctaId", new_count_id);
                    mySqlCommand.Parameters.AddWithValue("@ctaBalance", 0);
                    mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "30";
                    oResp.message = "ERROR AL INSERTAR CTA. CTE.";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                oResp.response_code = "31";
                try
                {
                    mySqlCommand.CommandText = "UPDATE secuencia SET sec_number=sec_number+1 WHERE sec_objectName=@paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        oResp.result = false;
                        oResp.message = "NO SE PUDO ACTUALIZAR LA SECUENCIA DE LA AUDITORIA";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return oResp;
                    }
                }
                catch (Exception ex)
                {

                    oResp.message = "ERROR AL ACTUALIZAR LA SECUENCIA DE LA AUDITORIA";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName=@paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                    secuencia = Convert.ToDecimal(mySqlCommand.ExecuteScalar());
                }
                catch (Exception ex)
                {

                    oResp.response_code = "32";
                    oResp.message = "ERROR AL SELECCIONAR LA SECUENCIA DE LA AUDITORIA";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    /*sentencia = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio) " +
                                 "VALUES ( " + secuencia + ", " + new_user + ",NULL," + "'" + "  La agencia " + new_age_id.ToString() + " fue creada correctamente " + "'" + " ," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + new_age_id.ToString() + ",'AGENTE','ALTA'" + ")";*/
                    mySqlCommand.CommandText = "INSERT INTO KcrTransaccion (traId,usrId,usrIdSuperior,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio) VALUES (@traId,@userId,@usrIdSuperior,@traComment,@traDate,@traReference,@traDomain,@traSubDomain)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@traId", secuencia);
                    mySqlCommand.Parameters.AddWithValue("@userId", new_user);
                    mySqlCommand.Parameters.AddWithValue("@usrIdSuperior", DBNull.Value);
                    mySqlCommand.Parameters.AddWithValue("@traComment", "La agencia " + new_age_id + " fue creada correctamente");
                    mySqlCommand.Parameters.AddWithValue("@traDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    mySqlCommand.Parameters.AddWithValue("@traReference", new_age_id);
                    mySqlCommand.Parameters.AddWithValue("@traDomain", "AGENTE");
                    mySqlCommand.Parameters.AddWithValue("@traSubDomain", "ALTA");
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "33";
                    oResp.message = "ERROR AL INSERTAR TRANSACCION ";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }
                //Fin de creacion de agencia

                oResp.response_code = "34";
                try
                {
                    mySqlCommand.CommandText = "UPDATE secuencia SET sec_number=sec_number+1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "STOCK");
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        oResp.result = false;
                        oResp.message = "NO SE PUDO ACTUALIZAR LA SECUENCIA DE LA STOCK";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return oResp;
                    }

                }
                catch (Exception ex)
                {

                    oResp.message = "NO SE PUDO ACTUALIZAR LA SECUENCIA DE LA STOCK";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "STOCK");
                    sec_number_stock = Convert.ToDecimal(mySqlCommand.ExecuteScalar());
                }
                catch (Exception ex)
                {

                    oResp.response_code = "35";
                    oResp.message = "ERROR AL SELECCIONAR LA SECUENCIA DE LA STOCK";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    /* sentencia = "INSERT INTO KlgStock (stkId,prdId,ageIdPropietario,stkCantidad,stkStockSeguridad,stkPuntoMinimoReposicion,stkPuntoMedioReposicion) " +
                                 "VALUES ( " + sec_number_stock + ",0, " + new_age_id + ",0,0,0,0" + ")";*/
                    mySqlCommand.CommandText = "INSERT INTO KlgStock (stkId,prdId,ageIdPropietario,stkCantidad,stkStockSeguridad,stkPuntoMinimoReposicion,stkPuntoMedioReposicion) VALUES (@stkId,@prdId,@ageId,@stkCantidad,@stkStockSeguridad,@stkPuntoMinimoReposicion,@stkPuntoMedioReposicion)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@stkId", sec_number_stock);
                    mySqlCommand.Parameters.AddWithValue("@prdId", 0);
                    mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                    mySqlCommand.Parameters.AddWithValue("@stkCantidad", 0);
                    mySqlCommand.Parameters.AddWithValue("@stkStockSeguridad", 0);
                    mySqlCommand.Parameters.AddWithValue("@stkPuntoMinimoReposicion", 0);
                    mySqlCommand.Parameters.AddWithValue("@stkPuntoMedioReposicion", 0);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "36";
                    oResp.message = "ERROR AL INSERTAR EL STOCK INICAL ";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    //Duda Comparar version offline
                    /* sentencia = "INSERT INTO KlgStockAgente (stkId,ageId,staOrden)" +
                                 "VALUES ( " + sec_number_stock + "," + new_age_id + ",1" + ")";*/
                    mySqlCommand.CommandText = "INSERT INTO KlgStockAgente (stkId,ageId,staOrden) VALUES (@stkId,@ageId,@order)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@stkId", sec_number_stock);
                    mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                    mySqlCommand.Parameters.AddWithValue("@order", 1);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    oResp.response_code = "37";
                    oResp.message = "ERROR AL INSERTAR EL STOCK INICAL ";
                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                    tran.Rollback();
                    return oResp;
                }

                // Habilitamos los productos enviados desde el frontend.
                foreach (var item in oRequest.productos)
                {
                    if (item.prdId == 0) //Configuramos todos los productos que tiene asignado la agencia padre a la agencia que se esta creando.
                    {
                        try
                        {
                            /*sentencia = "INSERT KlgAgenteProducto (ageId,prdId,agpEstado) " +
                                        "SELECT " + new_age_id + ",prdId,agpEstado FROM KlgAgenteProducto WHERE not prdid=0 and ageId=" + age_id_sup;*/
                            //mySqlCommand.CommandText = "INSERT INTO KlgAgenteProducto (ageId,prdId,agpEstado) SELECT @ageId,prdId,agpEstado FROM KlgAgenteProducto WHERE not prdid=@prdid and ageId=@ageIdSup";
                            mySqlCommand.CommandText = "INSERT INTO KlgAgenteProducto (ageId,prdId,agpEstado) SELECT @ageId,prdId,agpEstado FROM KlgAgenteProducto WHERE not prdid=@prdid and ageId=@ageIdSup and agpEstado = @agpEstado";
                            mySqlCommand.Parameters.Clear();
                            mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                            mySqlCommand.Parameters.AddWithValue("@ageIdSup", age_id_sup);
                            mySqlCommand.Parameters.AddWithValue("@prdid", 0);
                            mySqlCommand.Parameters.AddWithValue("@agpEstado", "AC");
                            mySqlCommand.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {

                            oResp.response_code = "38";
                            oResp.message = "ERROR AL INSERTAR TODOS LOS PRODUCTOS";
                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                            tran.Rollback();
                            return oResp;
                        }

                        try
                        {
                            /*sentencia = "INSERT INTO KlgStockAgente  (stkId,ageId,staOrden) " +
                                        "SELECT stkId, " + new_age_id + ", staOrden FROM KlgStockAgenteProducto WHERE not prdid=0 and ageId=" + age_id_sup;*/
                            mySqlCommand.CommandText = "INSERT INTO KlgStockAgente (stkId,ageId,staOrden) SELECT stkId,@ageId,staOrden FROM KlgStockAgenteProducto WHERE not prdid=@prdid and ageId=@ageIdSup";
                            mySqlCommand.Parameters.Clear();
                            mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                            mySqlCommand.Parameters.AddWithValue("@ageIdSup", age_id_sup);
                            mySqlCommand.Parameters.AddWithValue("@prdid", 0);
                            mySqlCommand.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {

                            oResp.response_code = "39";
                            oResp.message = "ERROR AL INSERTAR EL STOCK POR LA AGENCIA KlgStockAgente";
                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                            tran.Rollback();
                            return oResp;
                        }
                    }
                    else
                    {
                        try
                        {


                            //Opcion 1 separacion de seleccion e insercion
                            //sentencia = "SELECT  stkId,prdId,ageId,staOrden,stkCantidad FROM KlgStockAgenteProducto  WHERE ageId = 0 and prdId = " + item.prdId;
                            /*
                            mySqlCommand.CommandText = String.Concat("SELECT stkId,prdId,ageId,staOrden,stkCantidad FROM KlgStockAgenteProducto WHERE ageId=0 and prdId=@productId");
                            mySqlCommand.Parameters.Clear();
                            mySqlCommand.Parameters.AddWithValue("@productId", item.prdId);
                            List<object> objs = new List<object>();

                            #region Seleccion de datos
                            mySqlDataReader = mySqlCommand.ExecuteReader();
                            using (mySqlDataReader)
                            {
                                if (mySqlDataReader.HasRows)
                                    while (mySqlDataReader.Read())
                                    {
                                        var obj = new
                                        {
                                            stkId = (decimal)mySqlDataReader["stkId"],
                                            prdId = (decimal)mySqlDataReader["prdId"],
                                            staOrden = (decimal)mySqlDataReader["staOrden"],
                                            stkCantidad = (decimal)mySqlDataReader["stkCantidad"]
                                        };
                                        objs.Add(obj);
                                    }
                            }
                            #endregion

                            #region Insercion de datos
                                foreach(dynamic obj in objs){

                                    try
                                    {
                                      
                                        mySqlCommand.CommandText = String.Concat("INSERT INTO KlgAgenteProducto (ageId,prdId,agpEstado) ",
                                                                                "VALUES (@ageId,@prdId,@status)");
                                        mySqlCommand.Parameters.Clear();
                                        mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                                        mySqlCommand.Parameters.AddWithValue("@prdId", obj.prdId);
                                        mySqlCommand.Parameters.AddWithValue("@status", "AC");
                                        mySqlCommand.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        oResp.response_code = "44";
                                        oResp.message = "ERROR AL INSERTAR EL AGENTE X PRODUCTO ";
                                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                                        mySqlDataReader.Close();
                                        tran.Rollback();
                                        return oResp;
                                    }

                                    try
                                    {
                                    
                                        mySqlCommand.CommandText = String.Concat("INSERT INTO KlgStockAgente (stkId,ageId,staOrden) ",
                                                                                "VALUES (@stkId,@new_age_id,@order)");
                                        mySqlCommand.Parameters.Clear();
                                        mySqlCommand.Parameters.AddWithValue("@stkId", obj.stkId);
                                        mySqlCommand.Parameters.AddWithValue("@new_age_id", new_age_id);
                                        mySqlCommand.Parameters.AddWithValue("@order", obj.staOrden);
                                        mySqlCommand.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        oResp.response_code = "45";
                                        oResp.message = "ERROR AL INSERTAR STOCK X AGENTE ";
                                        logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                                        mySqlDataReader.Close();
                                        tran.Rollback();
                                        return oResp;
                                    }

                                }

                            #endregion
                            */
                            //Opcion 2 insert select que da documentado

                            try
                            {
                                // KlgStockAgente KlgAgenteProducto
                                mySqlCommand.CommandText = String.Concat(@"DECLARE @ErrorMessage NVARCHAR(4000);
                                                                        DECLARE @ErrorSeverity INT;
                                                                        DECLARE @ErrorState INT;
                                                                        declare @stkId as decimal
                                                                        declare   @prdId as decimal
                                                                        declare   @staOrden as decimal
                                                                        declare   @stkCantidad  as decimal
                                                                        SELECT @stkId = CONVERT(  DECIMAL,stkId),@prdId = CONVERT(  DECIMAL,prdId),@staOrden = CONVERT(  DECIMAL,ageId),@staOrden  = CONVERT(  DECIMAL,staOrden), @stkCantidad= CONVERT(  DECIMAL,stkCantidad)                                                          FROM KlgStockAgenteProducto WHERE ageId=0 and prdId=@productId if not  @prdId is null BEGIN begin try  INSERT INTO KlgAgenteProducto (ageId,prdId,agpEstado) VALUES (@ageId,@prdId,'AC') end try begin catch    SELECT  @ErrorMessage ='40-ERROR AL INSERTAR EL AGENTE X PRODUCTO-'+ ERROR_MESSAGE(),        @ErrorSeverity = ERROR_SEVERITY(),        @ErrorState = ERROR_STATE();    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState ) end catch begin try INSERT INTO KlgStockAgente (stkId,ageId,staOrden) VALUES (@stkId,@ageId,@staOrden); end try begin catch SELECT  @ErrorMessage = '41-ERROR AL INSERTAR STOCK X AGENTE-'+ ERROR_MESSAGE(),        @ErrorSeverity = ERROR_SEVERITY(),        @ErrorState = ERROR_STATE();    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState ) end catch END;");
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@productId", item.prdId);
                                mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                                mySqlCommand.ExecuteNonQuery();
                            }

                            catch (Exception ex)
                            {

                                string[] messages = ex.Message.Split('-');
                                if (messages.Length == 3)
                                {

                                    oResp.response_code = messages[0];
                                    oResp.message = messages[1];
                                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", messages[2], ". ", ex.StackTrace);
                                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                                    tran.Rollback();
                                    return oResp;

                                }
                                else
                                {

                                    oResp.response_code = "42";
                                    oResp.message = ex.Message;
                                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                                    tran.Rollback();
                                    return oResp;

                                }
                            }


                        }
                        catch (Exception ex)
                        {

                            oResp.response_code = "43";
                            oResp.message = "ERROR AL LEER AGENTE POR PRODUCTO";
                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                            tran.Rollback();
                            return oResp;
                        }
                    }
                }

                //TODO 
                //AV la variable no se tiene encuenta
                //if (oRequest.comisionporventa)
                bool value = Convert.ToBoolean(ConfigurationManager.AppSettings["COMISSION_BY_SALES"]);//oRequest.comisionporventa = Convert.ToBoolean( ConfigurationManager.AppSettings["COMISSION_BY_SALES"]);

                if (value)//oRequest.comisionporventa)
                {
                    // Configuramos las comisiones de la agencia por producto                               
                    try
                    {
                        oResp = ConfigCommissions(oRequest, new_age_id, age_id_sup, oRequest.grpId);
                        if (!oResp.result)
                        {
                            tran.Rollback();
                            return oResp;
                        }
                    }
                    catch (Exception ex)
                    {
                        oResp.response_code = "44";
                        oResp.message = "ERROR AL CONFIGURAR COMISIONES";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return oResp;
                    }
                }

                /*
               
                  
                 // se actualiza dependiendo de comision por deposito opor venta
                {
                    // Configuramos las comisiones de la agencia por producto                               
                    try
                    {
                        oResp = ConfigCommissions(oRequest, new_age_id, age_id_sup, oRequest.grpId);
                        if (!oResp.result)
                        { 
                            tran.Rollback();
                            return oResp;
                        }
                    }
                    catch (Exception ex)
                    {
                        oResp.response_code = "44";
                        oResp.message = "ERROR AL CONFIGURAR COMISIONES";
                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return oResp;
                    }
                }
                */
                oResp.response_code = "00";
                oResp.message = "TRANSACCION OK";
                oResp.result = true;
                oResp.AgeId = Decimal.ToInt32(new_age_id);
                tran.Commit();
                return oResp;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                oResp.response_code = "45";
                oResp.message = "NO SE PUDO ABRIR LA BASE DE DATOS";

                _m = String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                logger.ErrorLow(_m);

                return oResp;

            }
            finally { mySqlConnection.Close(); }
        }

        private static List<RequestAgent.Product> GetProductsComission(decimal ageid)
        {
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString.Replace("TRAN_", "COMI_");
            List<RequestAgent.Product> productos = null;
            try
            {
                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {
                    mySqlConnection.Open();

                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                    mySqlCommand.CommandTimeout = 120;
                    SqlDataReader mySqlDataReader = null;
                    productos = new List<RequestAgent.Product>();
                    //seleccionar desde la base de datos COM COL
                    // PUDE QUE TENAGA UNA COMISISON PARA TODOS 
                    //LOS PROPDUCTOS O PARA ESPECIFICA APRA ALGUNOS
                    mySqlCommand.CommandText = "SELECT  kvep.[prdId], kvep.[vepValor] as comision from [dbo].[KmmEntidadExtension] kext WITH(NOLOCK) inner join  [KmmVariableEntidadProductoAgen] kvep WITH(NOLOCK)  on kext.[entId] = kvep.[entId] where kext.[eneCodigo] = @age_id and varId = @varId and  mdlId = @mdlId";
                    //commission PercentageKmm.Models.Commissions.PercentageOfSalesModel
                    //,[acc_password] ,[acc_lastlogin] ,[acc_intentos],[acc_cambiopassword] ,[acc_validityDate] ,[acc_terminalId]
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", "A" + ageid);
                    mySqlCommand.Parameters.AddWithValue("@varId", "commissionPercentage");
                    mySqlCommand.Parameters.AddWithValue("@mdlId", "Kmm.Models.Commissions.PercentageOfSalesModel");

                    using (mySqlDataReader = mySqlCommand.ExecuteReader())
                    {
                        //cambiar a una lista de acceso

                        if (mySqlDataReader.HasRows)
                        {


                            while (mySqlDataReader.Read())
                            {
                                productos.Add(new RequestAgent.Product()
                                {
                                    comision = Convert.ToDecimal(mySqlDataReader["comision"]),
                                    prdId = Decimal.ToInt32((decimal)mySqlDataReader["prdId"])
                                });
                            }



                        }


                    }



                }
            }
            catch (Exception ex)
            {
                throw;

            }

            return productos;

        }
        private static List<RequestAgent.Product> GetProductsComission(int ageid)
        {
            //TODO LA CONEXION NO ES CORRECTA
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString.Replace("TRAN_", "COMI_");
            List<RequestAgent.Product> productos = null;
            try
            {
                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {
                    mySqlConnection.Open();
                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                    SqlDataReader mySqlDataReader = null;
                    productos = new List<RequestAgent.Product>();
                    //seleccionar desde la base de datos COM COL
                    // PUDE QUE TENAGA UNA COMISISON PARA TODOS 
                    //LOS PROPDUCTOS O PARA ESPECIFICA APRA ALGUNOS
                    mySqlCommand.CommandText = "SELECT  kvep.[prdId], kvep.[vepValor] as comision from [dbo].[KmmEntidadExtension] kext inner join  [dbo].[KmmVariableEntidadProductoAgen] kvep on kext.[entId] = kvep.[entId] where kext.[eneCodigo] = @age_id and varId = @varId and  mdlId = @mdlId";
                    //commission PercentageKmm.Models.Commissions.PercentageOfSalesModel
                    //,[acc_password] ,[acc_lastlogin] ,[acc_intentos],[acc_cambiopassword] ,[acc_validityDate] ,[acc_terminalId]
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@age_id", "A" + ageid);
                    mySqlCommand.Parameters.AddWithValue("@varId", "commissionPercentage");
                    mySqlCommand.Parameters.AddWithValue("@mdlId", "Kmm.Models.Commissions.PercentageOfSalesModel");

                    using (mySqlDataReader = mySqlCommand.ExecuteReader())
                    {
                        //cambiar a una lista de acceso

                        if (mySqlDataReader.HasRows)
                        {


                            while (mySqlDataReader.Read())
                            {
                                productos.Add(new RequestAgent.Product()
                                {
                                    comision = Convert.ToDecimal(mySqlDataReader["comision"]),
                                    prdId = Decimal.ToInt32((decimal)mySqlDataReader["prdId"])
                                });
                            }



                        }


                    }



                }
            }
            catch (Exception ex)
            {
                throw;

            }

            return productos;

        }

        /// <summary>
        /// Actualiza un agente en el sistema, y crea los datos que hagan falta 
        /// Valida los accesos del agente, esten completos y no repetidos valida 
        /// Actualiza los datos en la tabla agente agente
        /// Actualiza los datos del usuario 
        /// Actualiza los datos 
        /// 
        /// Pre Condicion: Los datos del agente usuario,
        /// los accesos estan completos
        /// los atributos par ala entidad KcrAtributoAgencia de la agencia ya existen en el systema
        /// Se ha valdiado que el  agente de modificacion tiene una realacion padre hijo o es el mismo agente
        /// Post Condicion: Se Actualiza un agente en el sistema de transacciones y comisiones con estado AC
        /// </summary>
        /// <param name="oRequest"></param>
        /// <returns>GenericApiResult indicando true si pudo concluir la operacion o false si no pudo terminar la operacion con su respectivo codigo de error</returns>
        public static GenericApiResult<bool> UpdateAgent(RequestAgent oRequest, bool caneditproducts = true)
        {
            //variable para mapear datos de persistencia del agente
            GenericApiResult<bool> result = new GenericApiResult<bool>();
            result.ObjectResult = false;

            int ResponseCode = 0;
            string Message = string.Empty;

            string _m = string.Empty;
            int? entId = 0;

            try
            {
                decimal age_id_sup = 0M;
                decimal usr_id = 0M;
                string age_nombre = string.Empty;

                //varaible aux validar logins
                int acc_login = 0;

                //indica si tiene acceso adminsitrador
                bool contienaccessoadministrador = false;

                logger.InfoLow(() => TagValue.New().Tag("[OP]").Value("UpdateAgent Actualizar Agente"));
                string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;

                ResponseCode = 1;
                // valida el primer acceso este completo
                if (string.IsNullOrEmpty(oRequest.acc_login))
                {
                    Message = "LOS DATOS DEL PRIMER ACCESO ESTAN INCOMPLETOS";
                    logger.ErrorLow(string.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message));
                    result.ResponseCode = ResponseCode;
                    result.ResponseMessage = Message;
                    return result;
                }

                //segundo acceso
                if (oRequest.second_tac_id <= 0m ||
                    string.IsNullOrEmpty(oRequest.second_acc_login))
                {
                    Message = "LOS DATOS DEL SEGUNDO ACCESO ESTAN INCOMPLETOS";
                    logger.ErrorLow(string.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message));
                    result.ResponseCode = ResponseCode;
                    result.ResponseMessage = Message;
                    return result;
                }

                //se evita manejar un else
                //validacion segundo acceso
                if (oRequest.second_tac_id == cons.ACCESS_WEB)//.ACCESO_WEB)
                {
                    Message = "TIPO DE ACCESO INVALIDO PARA SEGUNDO ACCESO";
                    logger.ErrorLow(string.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message));
                    result.ResponseCode = ResponseCode;
                    result.ResponseMessage = Message;
                    return result;
                }

                if (oRequest.acc_login.Equals(oRequest.second_acc_login, StringComparison.InvariantCultureIgnoreCase))
                {
                    Message = "LOS DATOS DEL SEGUNDO ACCESO (LOGIN) NO PUEDEN SER IGUALES AL PRIMER ACCESO";
                    logger.ErrorLow(string.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message));
                    result.ResponseCode = ResponseCode;
                    result.ResponseMessage = Message;
                    return result;
                }

                //si el tercer acceso esta activado
                if (oRequest.av_sc_ac_secondUser)
                {

                    // tercer acceso completo
                    if (oRequest.third_tac_id <= 0m ||
                      string.IsNullOrEmpty(oRequest.third_acc_login))
                    {
                        Message = "LOS DATOS DEL TERCER ACCESSO ESTAN INCOMPLETOS";
                        logger.ErrorLow(string.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message));
                        result.ResponseCode = ResponseCode;
                        result.ResponseMessage = Message;
                        return result;
                    }

                    // tercer acceso no tiene datos repetidos con respecto al primero
                    if (oRequest.third_tac_id == cons.ACCESS_WEB || oRequest.third_acc_login.Equals(oRequest.acc_login, StringComparison.InvariantCultureIgnoreCase))
                    {
                        result.ResponseMessage = "LOS DATOS DEL TERCER ACCESO (TIPO O LOGIN) NO PUEDEN SER IGUALES AL PRIMER ACCESO";
                        logger.ErrorLow(string.Concat("[QRY] ", LOG_PREFIX, " ", result.ResponseCode, "-", result.ResponseMessage));
                        return result;
                    }

                    // tercer acceso no tiene datos repetidos con respecto al segundo
                    if (oRequest.third_tac_id == oRequest.second_tac_id ||
                    oRequest.third_acc_login.Equals(oRequest.second_acc_login, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Message = "LOS DATOS DEL TERCER ACCESO (TIPO O LOGIN) NO PUEDEN SER IGUALES AL SEGUNDO ACCESO";
                        logger.ErrorLow(string.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message));
                        result.ResponseCode = ResponseCode;
                        result.ResponseMessage = Message;
                        return result;
                    }
                }

                try
                {

                    using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                    {
                        mySqlConnection.Open();
                        SqlCommand mySqlCommand = new SqlCommand();
                        SqlTransaction tran = null;
                        SqlDataReader mySqlDataReader = null;

                        mySqlCommand.Connection = mySqlConnection;
                        mySqlCommand.CommandTimeout = 120;

                        //variables para crear querys dinamicos
                        string QUERY = string.Empty;
                        string WHERE = string.Empty;

                        //indica si se esta actualiazando asi mismo delo contrario es el agente padre
                        //si y solo si se esta cumpliendo la preciondicion indicada en el comentario
                        bool IsUpdatingItSelf = oRequest.age_id == oRequest.age_modificacion;

                        ResponseCode = 2;
                        // trae datos necesarios para ejecutar el proceso de actualizacion
                        // [age_id_sup]  ,[usr_id], [age_nombre]
                        try
                        {

                            mySqlCommand.CommandText = "SELECT [age_id_sup] , [usr_id], [age_nombre] FROM [dbo].[Agente] WITH(NOLOCK) WHERE [age_id] = @age_id";
                            mySqlCommand.Parameters.Clear();

                            mySqlCommand.Parameters.AddWithValue("@age_id", oRequest.age_id);

                            using (mySqlDataReader = mySqlCommand.ExecuteReader())
                            {
                                if (mySqlDataReader.HasRows && mySqlDataReader.Read())
                                {
                                    age_id_sup = (decimal)mySqlDataReader["age_id_sup"];
                                    usr_id = (decimal)mySqlDataReader["usr_id"];
                                    age_nombre = mySqlDataReader["age_nombre"].ToString();
                                }
                                else
                                {
                                    Message = "NO SE ENCONTRATRON DATOS DEL AGENTE EN EL SISTEMA";
                                    logger.ErrorLow(string.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message));
                                    result.ResponseCode = ResponseCode;
                                    result.ResponseMessage = Message;
                                    return result;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Message = "ERROR AL CONSULTAR EL ACCESO PRINCIPAL DEL USUARIO";
                            _m = string.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                            result.ResponseCode = ResponseCode;
                            result.ResponseMessage = Message;
                            return result;
                        }

                        var agentdata = new
                        {
                            age_id = oRequest.age_id,
                            age_id_sup = age_id_sup,
                            usr_id = usr_id,
                            age_nombre = age_nombre
                        };

                        ResponseCode = 3;
                        // VERIFICA SI CONTIENE EL ROL 20
                        try
                        {
                            mySqlCommand.CommandText = "SELECT [usr_id] FROM [dbo].[RolUsuario] WITH(NOLOCK) WHERE [usr_id] = @usr_id and rol_id = @rol_id";
                            mySqlCommand.Parameters.Clear();

                            mySqlCommand.Parameters.AddWithValue("@usr_id", agentdata.usr_id);
                            mySqlCommand.Parameters.AddWithValue("@rol_id", 20);

                            using (mySqlDataReader = mySqlCommand.ExecuteReader())
                            {
                                contienaccessoadministrador = mySqlDataReader.HasRows;
                            }
                        }
                        catch (Exception ex)
                        {
                            Message = "ERROR AL VERIFICAR TIPO DE ACCESO";
                            _m = string.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                            result.ResponseCode = ResponseCode;
                            result.ResponseMessage = Message;
                            return result;
                        }

                        ResponseCode = 4;
                        // Verifica que el primer login no este asignado en el Systema
                        try
                        {
                            mySqlCommand.CommandText = "SELECT COUNT(1) FROM Acceso WITH(NOLOCK) WHERE acc_login = @loginname AND usr_id <> @usr_id";
                            mySqlCommand.Parameters.Clear();
                            mySqlCommand.Parameters.AddWithValue("@loginname", oRequest.acc_login);
                            mySqlCommand.Parameters.AddWithValue("@usr_id", agentdata.usr_id);
                            acc_login = (int)mySqlCommand.ExecuteScalar();
                            if (acc_login > 0)
                            {
                                Message = "EL PRIMER ACCESO ESTA ACTUALMENTE EN USO POR OTRO USUARIO";
                                logger.ErrorLow(string.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message));

                                result.ResponseCode = ResponseCode; result.ResponseMessage = Message; return result;
                            }
                        }
                        catch (Exception ex)
                        {
                            Message = "ERROR AL CONSULTAR EL ACCESO PRINCIPAL DEL USUARIO";
                            _m = string.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                            result.ResponseCode = ResponseCode;
                            result.ResponseMessage = Message;
                            return result;
                        }

                        ResponseCode = 5;
                        //Verifica que el segundo login no este asignado en el Systema
                        try
                        {
                            mySqlCommand.CommandText = "SELECT COUNT(1) FROM Acceso WITH(NOLOCK) WHERE acc_login = @loginname AND usr_id <> @usr_id";
                            mySqlCommand.Parameters.Clear();
                            mySqlCommand.Parameters.AddWithValue("@loginname", oRequest.second_acc_login);
                            mySqlCommand.Parameters.AddWithValue("@usr_id", agentdata.usr_id);
                            acc_login = (int)mySqlCommand.ExecuteScalar();
                            if (acc_login > 0)
                            {
                                Message = "EL SEGUNDO ACCESO ESTA ACTUALMENTE EN USO POR OTRO USUARIO";
                                logger.ErrorLow(string.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message));
                                result.ResponseCode = ResponseCode; result.ResponseMessage = Message; return result;
                            }
                        }
                        catch (Exception ex)
                        {
                            Message = "ERROR AL CONSULTAR EL SEGUNDO ACCESO DEL USUARIO";
                            _m = string.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                            result.ResponseCode = ResponseCode;
                            result.ResponseMessage = Message;
                            return result;
                        }


                        if (oRequest.av_sc_ac_secondUser)
                        {

                            //Verifica que el tercer login no este asignado en el Systema
                            ResponseCode = 6;
                            try
                            {
                                mySqlCommand.CommandText = "SELECT COUNT(1) FROM Acceso WITH(NOLOCK) WHERE acc_login = @loginname AND usr_id <> @usr_id";
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@loginname", oRequest.third_acc_login);
                                mySqlCommand.Parameters.AddWithValue("@usr_id", agentdata.usr_id);
                                acc_login = (int)mySqlCommand.ExecuteScalar();
                                if (acc_login > 0)
                                {
                                    Message = "EL TERCER ACCESO ESTA ACTUALMENTE EN USO POR OTRO USUARIO";
                                    logger.ErrorLow(string.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message));

                                    result.ResponseCode = ResponseCode; result.ResponseMessage = Message; return result;
                                }
                            }
                            catch (Exception ex)
                            {
                                Message = "ERROR AL CONSULTAR EL TERCER ACCESO DEL USUARIO";
                                _m = string.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                                result.ResponseCode = ResponseCode;
                                result.ResponseMessage = Message;
                                return result;
                            }
                        }

                        // se incia la transaccion cuando se van empezar a actualizar los datos
                        tran = mySqlConnection.BeginTransaction();
                        mySqlCommand.Transaction = tran;

                        //Actualizan los datos del agente
                        try
                        {
                            //incluir variable de configuracion 
                            bool CanChangeComision = !IsUpdatingItSelf;

                            //mySqlCommand.CommandText = 
                            QUERY = @"UPDATE [dbo].[Agente] WITH(ROWLOCK) SET
                                 [age_nombre] = @age_nombre
                                ,[ciu_id] = @ciu_id
                                ,[age_direccion] = @age_direccion
                                ,[age_razonsocial] = @age_razonsocial
                                ,[age_cuit] = @age_cuit
                                ,[age_tel] = @age_tel
                                ,[age_email] = @age_email
                                ,[age_cel] = @age_cel
                                ,[age_contacto] = @age_contacto
                                ,[age_observaciones] = @age_observaciones
                                ,[age_estado] = @age_estado
                                ,[age_subNiveles] = @age_subNiveles
                                ,[age_tipo] = @age_tipo
                                ,[age_autenticaterminal] = @age_autenticaterminal
                                ,[age_fecalta] = (CASE WHEN [age_fecalta] IS NULL THEN @age_fecalta ELSE [age_fecalta] END)
                                ,[usr_id_modificacion] = @usr_id_modificacion
                                ,[age_pdv] = @age_pdv
                                ,[age_entrecalles] = @age_entrecalles
                                ,[ct_id] = @ct_id
                                ,[ta_id] = @ta_id
                                ,[sa_id] = @sa_id ";
                            //,[age_prefijosrest]  = @age_prefijosrest
                            //,[age_comisionadeposito] = @age_comisionadeposito, [age_montocomision]  = @age_montocomision	";

                            WHERE = " WHERE [age_id] = @age_id";

                            mySqlCommand.Parameters.Clear();
                            //set primary key
                            mySqlCommand.Parameters.AddWithValue("@age_id", agentdata.age_id);
                            //
                            mySqlCommand.Parameters.AddWithValue("@age_nombre", oRequest.age_nombre);
                            mySqlCommand.Parameters.AddWithValue("@ciu_id", oRequest.age_ciu_id);
                            mySqlCommand.Parameters.AddWithValue("@age_direccion", oRequest.age_direccion);
                            mySqlCommand.Parameters.AddWithValue("@age_razonsocial", oRequest.age_razonsocial);
                            mySqlCommand.Parameters.AddWithValue("@age_cuit", oRequest.age_cuit);
                            mySqlCommand.Parameters.AddWithValue("@age_tel", oRequest.age_tel);
                            mySqlCommand.Parameters.AddWithValue("@age_email", oRequest.age_email);
                            mySqlCommand.Parameters.AddWithValue("@age_cel", oRequest.age_cel);
                            mySqlCommand.Parameters.AddWithValue("@age_contacto", String.IsNullOrEmpty(oRequest.age_contacto) ? "" : oRequest.age_contacto);
                            mySqlCommand.Parameters.AddWithValue("@age_observaciones", String.IsNullOrEmpty(oRequest.age_observaciones) ? "" : oRequest.age_observaciones);
                            mySqlCommand.Parameters.AddWithValue("@age_estado", "AC");
                            mySqlCommand.Parameters.AddWithValue("@age_subNiveles", oRequest.age_subNiveles);
                            mySqlCommand.Parameters.AddWithValue("@age_tipo", oRequest.age_tipo);
                            mySqlCommand.Parameters.AddWithValue("@age_autenticaterminal", oRequest.age_autenticaterminal);

                            //NO SE MODIFICA INICIALMENTE
                            ///mySqlCommand.Parameters.AddWithValue("@age_prefijosrest", oRequest.age_prefijosrest);
                            ///

                            mySqlCommand.Parameters.AddWithValue("@age_fecalta", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            mySqlCommand.Parameters.AddWithValue("@usr_id_modificacion", oRequest.usr_id_modificacion);
                            mySqlCommand.Parameters.AddWithValue("@age_pdv", String.IsNullOrEmpty(oRequest.age_pdv) ? "" : oRequest.age_pdv);
                            mySqlCommand.Parameters.AddWithValue("@age_entrecalles", String.IsNullOrEmpty(oRequest.age_entrecalles) ? "" : oRequest.age_entrecalles);
                            mySqlCommand.Parameters.AddWithValue("@ct_id", oRequest.ct_id);
                            mySqlCommand.Parameters.AddWithValue("@ta_id", oRequest.ta_id);
                            mySqlCommand.Parameters.AddWithValue("@sa_id", oRequest.sa_id);
                            //mySqlCommand.Parameters.AddWithValue("@age_comisionadeposito", String.IsNullOrEmpty(oRequest.age_comisionadeposito) ? "N" : oRequest.age_comisionadeposito);
                            //mySqlCommand.Parameters.AddWithValue("@age_montocomision", String.IsNullOrEmpty(oRequest.age_comisionadeposito) || oRequest.age_comisionadeposito.Equals("N") ? 0m : oRequest.age_montocomision);

                            //generacion dinamica
                            if (CanChangeComision)
                            {
                                QUERY = string.Concat(QUERY, ",[age_comisionadeposito] = @age_comisionadeposito, [age_montocomision]  = @age_montocomision	");

                                mySqlCommand.Parameters.AddWithValue("@age_comisionadeposito", string.IsNullOrEmpty(oRequest.age_comisionadeposito) ? "N" : oRequest.age_comisionadeposito);
                                mySqlCommand.Parameters.AddWithValue("@age_montocomision", string.IsNullOrEmpty(oRequest.age_comisionadeposito) || oRequest.age_comisionadeposito.Equals("N") ? 0m : oRequest.age_montocomision);
                            }

                            //habilitar prefijorest
                            //if (!string.IsNullOrEmpty(oRequest.age_prefijosrest))
                            //{

                            //    QUERY = String.Concat(QUERY, " ,[age_prefijosrest]  = @age_prefijosrest");

                            //   mySqlCommand.Parameters.AddWithValue("@age_prefijosrest", oRequest.age_prefijosrest);
                            //}

                            ////
                            mySqlCommand.CommandText = string.Concat(QUERY, WHERE);

                            if (mySqlCommand.ExecuteNonQuery() == 0)
                            {
                                Message = "NO SE PUDO ACTUALIZAR DATOS DEL AGENTE";
                                _m = string.Concat("[API] ", LOG_PREFIX, " ", ResponseCode, "-", Message);
                                logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                tran.Rollback();
                                result.ResponseCode = ResponseCode;
                                result.ResponseMessage = Message;
                                return result;
                            }

                        }
                        catch (Exception ex)
                        {

                            ResponseCode = 7;
                            Message = "ERROR AL CREAR LA AGENCIA";
                            _m = string.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                            tran.Rollback();
                            result.ResponseCode = ResponseCode;
                            result.ResponseMessage = Message;
                            return result;
                        }

                        ResponseCode = 8;
                        // actualiza los datos del usuario
                        try
                        {


                            mySqlCommand.CommandText = @"UPDATE [Usuario] WITH(ROWLOCK) SET
                                 [usr_estado] = @usr_estado
                                ,[usr_nombre] = @usr_nombre
                                ,[usr_apellido] = @usr_apellido
                                ,[ciu_id] = @ciu_id
                                ,[usr_horaLaboralDesde] = @usr_horaLaboralDesde
                                ,[usr_minutosLaboralDesde] = @usr_minutosLaboralDesde
                                ,[usr_horaLaboralHasta] = @usr_horaLaboralHasta
                                ,[usr_minutosLaboralHasta] = @usr_minutosLaboralHasta 
                                WHERE [usr_id] = @usr_id ";

                            mySqlCommand.Parameters.Clear();
                            mySqlCommand.Parameters.AddWithValue("@usr_id", agentdata.usr_id);
                            mySqlCommand.Parameters.AddWithValue("@usr_estado", "AC");
                            mySqlCommand.Parameters.AddWithValue("@usr_nombre", oRequest.usr_nombre);
                            mySqlCommand.Parameters.AddWithValue("@usr_apellido", oRequest.usr_apellido);
                            mySqlCommand.Parameters.AddWithValue("@ciu_id", oRequest.age_ciu_id);
                            mySqlCommand.Parameters.AddWithValue("@usr_horaLaboralDesde", 0);
                            mySqlCommand.Parameters.AddWithValue("@usr_minutosLaboralDesde", 0);
                            mySqlCommand.Parameters.AddWithValue("@usr_horaLaboralHasta", 23);
                            mySqlCommand.Parameters.AddWithValue("@usr_minutosLaboralHasta", 59);

                            if (mySqlCommand.ExecuteNonQuery() == 0)
                            {
                                Message = "NO SE PUDO ACTUALIZAR DATOS DEL USUARIO";
                                _m = string.Concat("[API] ", LOG_PREFIX, " ", ResponseCode, "-", Message);
                                logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                tran.Rollback();
                                result.ResponseCode = ResponseCode; result.ResponseMessage = Message; return result;
                            }

                        }
                        catch (Exception ex)
                        {
                            Message = "ERROR AL CREAR USUARIO";
                            _m = string.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                            tran.Rollback();
                            result.ResponseCode = ResponseCode;
                            result.ResponseMessage = Message;
                            return result;
                        }



                        //Se actualiza los datos de Atributos de la agencia
                        //pre: todos los atributos existen en el systema
                        //se ha validado que el agente que se esta modificando es el mismo o tiene una ralacion apdre hijo
                        #region attibutos dela agencia
                        if (!IsUpdatingItSelf)
                        {
                            ResponseCode = 9;
                            string AtDominio = "AGENTE";

                            //variables para hacer un  llamado de propiedad por refelxion
                            string attributtes = ConfigurationManager.AppSettings["ATRIBUTTE_AGENCIES"] as string ?? "limiteCredito,autorizacionAutomatica,quitaAutomatica,generacionAutomatica,montoMinimoPorPedido,montoMaximoPorPedido,pedidoMaximoMensual,autorizacionAutomaticaMontoDiario,recargaAsincronica";

                            string[] atribobj = attributtes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);//new string[] { 
                            //limiteCredito no se debe editar en colombia
                            //"limiteCredito", 
                            //"autorizacionAutomatica", "quitaAutomatica", "generacionAutomatica", "montoMinimoPorPedido", "montoMaximoPorPedido", "pedidoMaximoMensual", "autorizacionAutomaticaMontoDiario", "recargaAsincronica" };

                            // colocar en el webconfig especificando update
                            //"limiteCredito", "autorizacionAutomatica", "quitaAutomatica", "generacionAutomatica", "montoMinimoPorPedido", "montoMaximoPorPedido", "pedidoMaximoMensual", "autorizacionAutomaticaMontoDiario", "recargaAsincronica" 


                            //valores para registros de la tabla KcrAtributoAgencia como son los mismos valores con respecto a atribobj se manipula el primer caracter
                            //string[] attId    = new string[] { "LimiteCredito", "AutorizacionAutomatica", "QuitaAutomatica", "GeneracionAutomatica", "MontoMinimoPorPedido", "MontoMaximoPorPedido", "PedidoMaximoMensual", "AutorizacionAutomaticaMontoDiario", "RecargaAsincronica" };

                            for (int i = 0; i < atribobj.Length; i++)
                            {
                                var attributo = atribobj[i];
                                //se transforma el primer caracter a mayuscyla para insertar en la tabla 
                                var att = char.ToUpper(attributo[0]) + attributo.Substring(1);

                                try
                                {
                                    mySqlCommand.CommandText = "UPDATE KcrAtributoAgencia WITH(ROWLOCK) SET ataValor = @ataValor WHERE ageId = @ageId and attDominio = @attDominio and attId = @attId";
                                    mySqlCommand.Parameters.Clear();
                                    mySqlCommand.Parameters.AddWithValue("@ageId", oRequest.age_id);
                                    mySqlCommand.Parameters.AddWithValue("@attId", att);
                                    mySqlCommand.Parameters.AddWithValue("@attDominio", AtDominio);

                                    mySqlCommand.Parameters.AddWithValue("@ataValor", oRequest.GetType().GetProperty(attributo).GetValue(oRequest, null));

                                    //DEBEN EXISTIR TODOS LOS ATRIBUTOS INDICADOS
                                    if (mySqlCommand.ExecuteNonQuery() == 0)
                                    {
                                        Message = "NO SE PUDO AL ACUTALIZAR  KCRATRIBUTOAGENCIA " + att;

                                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                                        tran.Rollback();
                                        result.ResponseCode = ResponseCode;
                                        result.ResponseMessage = Message;
                                        return result;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Message = "ERROR AL ACTUALIZAR  KCRATRIBUTOAGENCIA " + att;
                                    _m = string.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                                    tran.Rollback();
                                    result.ResponseCode = ResponseCode;
                                    result.ResponseMessage = Message;
                                    return result;
                                }
                            }
                        }
                        #endregion

                        //actualizacion de accesos
                        #region actualizacion accesos

                        //obtner los accesos
                        //al tener una llave combinada y no tener la posibilidad de editar todos los accesos teniendo encuenta 
                        //la existencia o la ordinalidad de los datos
                        //se selecciona todos los accesos guardados con el fin de realizar el update de la manera correcta
                        //sin perder datos y no tener problemas por llave principal

                        //evaluar la posibilidad de hacer un delete insert en el momento que se complete la funcionalidad para todos los accesos
                        List<dynamic> lista = null;
                        try
                        {
                            lista = GetAccesosByUser(agentdata.usr_id).ToList<dynamic>();
                        }
                        catch (Exception ex)
                        {
                            ResponseCode = 10;
                            Message = "ERROR AL BUSCAR ACCESSOS";
                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                            logger.ErrorLow(() => TagValue.New().Exception(ex).Message("[INPUT]").Value(agentdata.usr_id));
                            tran.Rollback();
                            result.ResponseCode = ResponseCode;
                            result.ResponseMessage = Message;
                            return result;
                        }

                        //variables para el los valores anteriores
                        decimal old_typeacces = 0m;
                        //dato auxiliar y determinar el acceso antiguo
                        dynamic auxaccess = null;

                        //PRIMER ACCESO
                        //se actualiza el primer acceso ACCESO_WEB
                        #region primeraccesso
                        ResponseCode = 11;


                        QUERY = " UPDATE [dbo].[Acceso] WITH(ROWLOCK)  SET [acc_login] = @acc_login  ,[acc_estado] = @acc_estado   ,[acc_cambiopassword] = @acc_cambiopassword   ,[acc_validityDate] = @acc_validityDate ";

                        WHERE = " WHERE [tac_id] = @tac_id and [usr_id] = @usr_id";

                        try
                        {


                            /*
                            mySqlCommand.CommandText = " UPDATE [dbo].[Acceso] WITH(ROWLOCK)  SET [acc_login] = @acc_login   , [acc_password] = @acc_password   ,[acc_estado] = @acc_estado   ,[acc_cambiopassword] = @acc_cambiopassword   ,[acc_validityDate] = @acc_validityDate WHERE [tac_id] = @tac_id and [usr_id] = @usr_id";
                            */

                            mySqlCommand.Parameters.Clear();
                            mySqlCommand.Parameters.AddWithValue("@tac_id", cons.ACCESS_WEB);
                            mySqlCommand.Parameters.AddWithValue("@usr_id", agentdata.usr_id);
                            mySqlCommand.Parameters.AddWithValue("@acc_login", oRequest.acc_login);

                            //si se realizo el cambio de password se actualiza, de lo contrario se deja el password actual
                            if (!string.IsNullOrEmpty(oRequest.acc_password))
                            {
                                mySqlCommand.Parameters.AddWithValue("@acc_password", oRequest.acc_password);
                                QUERY = string.Concat(QUERY, ", [acc_password] = @acc_password");
                            }
                            ////             TODO ESTO NO ES VALIDO!!!!               else                                mySqlCommand.Parameters.AddWithValue("@acc_password", "acc_password");

                            mySqlCommand.Parameters.AddWithValue("@acc_estado", "AC");
                            mySqlCommand.Parameters.AddWithValue("@acc_cambiopassword", string.IsNullOrEmpty(oRequest.acc_cambiopassword) ? "N" : oRequest.acc_cambiopassword);

                            if (string.IsNullOrEmpty(oRequest.acc_cambiopassword))
                                mySqlCommand.Parameters.AddWithValue("@acc_validityDate", DBNull.Value);
                            else
                                mySqlCommand.Parameters.AddWithValue("@acc_validityDate", oRequest.acc_validityDate);

                            mySqlCommand.CommandText = string.Concat(QUERY, WHERE);

                            if (mySqlCommand.ExecuteNonQuery() == 0)
                            {
                                Message = "NO SE PUDO ACTUALIZAR EL PRIMER ACCESO DEL AGENTE";
                                _m = String.Concat("[API] ", LOG_PREFIX, " ", ResponseCode, "-", Message);
                                logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                tran.Rollback();
                                result.ResponseCode = ResponseCode; result.ResponseMessage = Message; return result;

                            }
                        }
                        catch (Exception ex)
                        {


                            Message = "ERROR AL CREAR ACCESO";
                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                            tran.Rollback();
                            result.ResponseCode = ResponseCode;
                            result.ResponseMessage = Message;
                            return result;
                        }
                        #endregion

                        //SEGUNDO ACCESO
                        //se actualiza el segundo acceso seleccionado por el usuario
                        //de lo contrario lo crea
                        #region segundo acceso



                        QUERY = " UPDATE [dbo].[Acceso] WITH(ROWLOCK) SET [acc_login] = @acc_login , [tac_id] = @tac_id   , [acc_estado] = @acc_estado   ,[acc_cambiopassword] = @acc_cambiopassword   ,[acc_validityDate] = @acc_validityDate ";

                        WHERE = " WHERE [tac_id] = @old_typeacces  and [usr_id] = @usr_id";

                        ResponseCode = 12;
                        try
                        {
                            // si tiene mas de una acceso
                            if (lista.Count > 1)
                            {
                                //por defecto el siguiente de la lista de datos 
                                auxaccess = lista[1];
                                bool FIND = false;
                                try
                                {

                                    dynamic AUX = lista.First(p => p.tac_id == oRequest.second_tac_id);
                                    //si se encuentra el datos en los datos guardados
                                    int INDEX = lista.IndexOf(AUX);
                                    auxaccess = new { AUX.tac_id };
                                    // se remuve el dato por el indice
                                    lista.RemoveAt(INDEX);
                                    FIND = true;

                                }
                                catch (Exception ex) { }

                                old_typeacces = auxaccess.tac_id;
                                /*
                                mySqlCommand.CommandText = "UPDATE [dbo].[Acceso] WITH(ROWLOCK) SET [acc_login] = @acc_login , [tac_id] = @tac_id   ,[acc_password] = @acc_password   ,[acc_estado] = @acc_estado   ,[acc_cambiopassword] = @acc_cambiopassword   ,[acc_validityDate] = @acc_validityDate WHERE [tac_id] = @old_typeacces  and [usr_id] = @usr_id";
                                */
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@tac_id", oRequest.second_tac_id);
                                mySqlCommand.Parameters.AddWithValue("@usr_id", agentdata.usr_id);
                                mySqlCommand.Parameters.AddWithValue("@acc_login", oRequest.second_acc_login);
                                mySqlCommand.Parameters.AddWithValue("@old_typeacces", old_typeacces);

                                // solo se actualiza si hubo modificacion en el password
                                if (!string.IsNullOrEmpty(oRequest.second_acc_password))
                                {
                                    QUERY = string.Concat(QUERY, ", [acc_password] = @acc_password");
                                    mySqlCommand.Parameters.AddWithValue("@acc_password", oRequest.second_acc_password);

                                }


                                mySqlCommand.Parameters.AddWithValue("@acc_estado", "AC");
                                mySqlCommand.Parameters.AddWithValue("@acc_cambiopassword", string.IsNullOrEmpty(oRequest.second_acc_cambiopassword) ? "N" : oRequest.second_acc_cambiopassword);

                                if (string.IsNullOrEmpty(oRequest.second_acc_cambiopassword))
                                    mySqlCommand.Parameters.AddWithValue("@acc_validityDate", DBNull.Value);
                                else
                                    mySqlCommand.Parameters.AddWithValue("@acc_validityDate", oRequest.second_acc_validityDate);

                                mySqlCommand.CommandText = string.Concat(QUERY, WHERE);

                                if (mySqlCommand.ExecuteNonQuery() == 0)
                                {
                                    Message = "NO SE PUDO ACTUALIZAR EL SEGUNDO ACCESO DEL AGENTE";
                                    _m = String.Concat("[API] ", LOG_PREFIX, " ", ResponseCode, "-", Message);
                                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                    tran.Rollback();
                                    result.ResponseCode = ResponseCode; result.ResponseMessage = Message; return result;

                                }

                                // si no se encontro el tipo de acceso en el tipo de acceso 
                                // se remueve el datos de la lista
                                if (!FIND)
                                    lista.RemoveAt(1);

                            }
                            else
                            {
                                //si no tiene mas de un acceso se inserta el segundo Acceso
                                mySqlCommand.CommandText = "INSERT INTO Acceso (tac_id,usr_id,acc_login,acc_password,acc_lastlogin,acc_intentos,acc_estado,acc_cambiopassword,acc_validityDate) VALUES (@tac_id,@usr_id,@acc_login,@acc_password,@acc_lastlogin,@acc_intentos,@acc_estado,@acc_cambiopassword,@acc_validityDate)";
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@tac_id", oRequest.second_tac_id);
                                mySqlCommand.Parameters.AddWithValue("@usr_id", agentdata.usr_id);
                                mySqlCommand.Parameters.AddWithValue("@acc_login", oRequest.second_acc_login);

                                // OBLIGATORIO PASSWORD 
                                if (!string.IsNullOrEmpty(oRequest.second_acc_password))
                                    mySqlCommand.Parameters.AddWithValue("@acc_password", oRequest.second_acc_password);
                                else
                                    throw new Exception("ERROR EL PASSWORD DEL SEGUNDO ACCESO ES INVALIDO");

                                mySqlCommand.Parameters.AddWithValue("@acc_lastlogin", DBNull.Value);
                                mySqlCommand.Parameters.AddWithValue("@acc_intentos", 0);



                                mySqlCommand.Parameters.AddWithValue("@acc_estado", "AC");
                                mySqlCommand.Parameters.AddWithValue("@acc_cambiopassword", string.IsNullOrEmpty(oRequest.second_acc_cambiopassword) ? "N" : oRequest.second_acc_cambiopassword);

                                if (string.IsNullOrEmpty(oRequest.second_acc_cambiopassword))
                                    mySqlCommand.Parameters.AddWithValue("@acc_validityDate", DBNull.Value);
                                else
                                    mySqlCommand.Parameters.AddWithValue("@acc_validityDate", oRequest.second_acc_validityDate);

                                mySqlCommand.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {


                            Message = "ERROR AL CREAR SEGUNDO ACCESO";
                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                            tran.Rollback();
                            result.ResponseCode = ResponseCode; result.ResponseMessage = Message; return result;
                        }
                        #endregion
                        //TERCER ACEESO
                        //Actualiza o crea el tercer acceso si esta habilitado por parte del ususario
                        ResponseCode = 13;
                        #region tercer acceso
                        if (oRequest.av_sc_ac_secondUser)
                        {
                            try
                            {


                                if (lista.Count > 1)
                                {
                                    //por defecto el siguiente de la lista de datos 

                                    auxaccess = lista[lista.Count() - 1];
                                    bool FIND = false;
                                    try
                                    {
                                        dynamic AUX = lista.First(p => p.tac_id == oRequest.third_tac_id);
                                        int INDEX = lista.IndexOf(AUX);
                                        //si se encuentra el datos en los datos guardados
                                        auxaccess = new { AUX.tac_id };
                                        lista.RemoveAt(INDEX);
                                        FIND = true;
                                    }
                                    catch (Exception ex) { }

                                    //se determina el acceso antigui para actualizar el dato
                                    old_typeacces = auxaccess.tac_id;
                                    //

                                    QUERY = " UPDATE [dbo].[Acceso] WITH(ROWLOCK) SET [acc_login] = @acc_login , [tac_id] = @tac_id   ,[acc_estado] = @acc_estado   ,[acc_cambiopassword] = @acc_cambiopassword   ,[acc_validityDate] = @acc_validityDate ";

                                    WHERE = " WHERE [tac_id] = @old_typeacces  and [usr_id] = @usr_id";

                                    /*
                                  QUERY = " UPDATE [dbo].[Acceso] WITH(ROWLOCK)  SET [acc_login] = @acc_login  ,[acc_estado] = @acc_estado   ,[acc_cambiopassword] = @acc_cambiopassword   ,[acc_validityDate] = @acc_validityDate ";*/


                                    /*
                                         mySqlCommand.CommandText = " UPDATE [dbo].[Acceso] WITH(ROWLOCK)  SET [acc_login] = @acc_login, [tac_id] = @tac_id   ,[acc_password] = @acc_password   ,[acc_estado] = @acc_estado   ,[acc_cambiopassword] = @acc_cambiopassword   ,[acc_validityDate] = @acc_validityDate WHERE [tac_id] = @old_typeacces and [usr_id] = @usr_id ";
                                         */
                                    mySqlCommand.Parameters.Clear();

                                    mySqlCommand.Parameters.AddWithValue("@usr_id", agentdata.usr_id);
                                    mySqlCommand.Parameters.AddWithValue("@old_typeacces", old_typeacces);

                                    mySqlCommand.Parameters.AddWithValue("@acc_login", oRequest.third_acc_login);

                                    mySqlCommand.Parameters.AddWithValue("@tac_id", oRequest.third_tac_id);

                                    // solo se actualiza si hubo modificacion en el password
                                    if (!string.IsNullOrEmpty(oRequest.third_acc_password))
                                    {
                                        QUERY = string.Concat(QUERY, ", [acc_password] = @acc_password");
                                        mySqlCommand.Parameters.AddWithValue("@acc_password", oRequest.third_acc_password);

                                    }
                                    mySqlCommand.Parameters.AddWithValue("@acc_estado", "AC");

                                    mySqlCommand.Parameters.AddWithValue("@acc_cambiopassword", string.IsNullOrEmpty(oRequest.third_acc_cambiopassword) ? "N" : oRequest.third_acc_cambiopassword);

                                    if (string.IsNullOrEmpty(oRequest.third_acc_cambiopassword))
                                        mySqlCommand.Parameters.AddWithValue("@acc_validityDate", DBNull.Value);
                                    else
                                        mySqlCommand.Parameters.AddWithValue("@acc_validityDate", oRequest.third_acc_validityDate);


                                    mySqlCommand.CommandText = string.Concat(QUERY, WHERE);
                                    if (mySqlCommand.ExecuteNonQuery() == 0)
                                    {
                                        Message = "NO SE PUDO ACTUALIZAR EL TERCER ACCESO DEL AGENTE";
                                        _m = String.Concat("[API] ", LOG_PREFIX, " ", ResponseCode, "-", Message);
                                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                        tran.Rollback();
                                        result.ResponseCode = ResponseCode; result.ResponseMessage = Message; return result;

                                    }
                                    //si no se encontro la se remueve por el indice que se accedio
                                    if (!FIND)
                                        lista.RemoveAt(lista.Count() - 1);


                                }
                                else
                                {
                                    // no tiene un tercer acceso
                                    mySqlCommand.CommandText = "INSERT INTO Acceso (tac_id,usr_id,acc_login,acc_password,acc_lastlogin,acc_intentos,acc_estado,acc_cambiopassword,acc_validityDate) VALUES (@tac_id,@usr_id,@acc_login,@acc_password,@acc_lastlogin,@acc_intentos,@acc_estado,@acc_cambiopassword,@acc_validityDate)";
                                    mySqlCommand.Parameters.Clear();
                                    mySqlCommand.Parameters.AddWithValue("@tac_id", oRequest.third_tac_id);
                                    mySqlCommand.Parameters.AddWithValue("@usr_id", agentdata.usr_id);
                                    mySqlCommand.Parameters.AddWithValue("@acc_login", oRequest.third_acc_login);

                                    // solo se actualiza si hubo modificacion en el password
                                    if (!string.IsNullOrEmpty(oRequest.third_acc_password))
                                        mySqlCommand.Parameters.AddWithValue("@acc_password", oRequest.third_acc_password);
                                    else
                                        throw new Exception("ERROR EL PASSWORD DEL TERCER ACCESO ES INVALIDO");

                                    mySqlCommand.Parameters.AddWithValue("@acc_lastlogin", DBNull.Value);
                                    mySqlCommand.Parameters.AddWithValue("@acc_intentos", 0);
                                    mySqlCommand.Parameters.AddWithValue("@acc_estado", "AC");
                                    mySqlCommand.Parameters.AddWithValue("@acc_cambiopassword", string.IsNullOrEmpty(oRequest.third_acc_cambiopassword) ? "N" : oRequest.third_acc_cambiopassword);

                                    if (string.IsNullOrEmpty(oRequest.third_acc_cambiopassword))//.acc_cambiopassword))
                                        mySqlCommand.Parameters.AddWithValue("@acc_validityDate", DBNull.Value);
                                    else
                                        mySqlCommand.Parameters.AddWithValue("@acc_validityDate", oRequest.third_acc_validityDate);

                                    mySqlCommand.ExecuteNonQuery();

                                }
                            }
                            catch (Exception ex)
                            {


                                Message = "ERROR AL CREAR TERCER ACCESO";
                                _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                tran.Rollback();
                                result.ResponseCode = ResponseCode; result.ResponseMessage = Message; return result;
                            }
                        }
                        else
                        {
                            //si se deshabilita el tercer acceso y  tiene tercer acceso se cambia de estado el acceso
                            if (lista.Count > 1)
                            {
                                //por defecto la lista selecciona el TERCER ACCESO
                                auxaccess = lista[lista.Count() - 1];
                                old_typeacces = auxaccess.tac_id;

                                // puede que tenga o no tenga el tercer acceso si lo tiene suspenderlo
                                mySqlCommand.CommandText = " UPDATE [dbo].[Acceso] WITH(ROWLOCK) SET [acc_estado] = @acc_estado, [tac_id] = @tac_id  WHERE [tac_id] = @old_typeacces and usr_id = @usr_id";
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("old_typeacces", old_typeacces);
                                mySqlCommand.Parameters.AddWithValue("@acc_estado", "SU");

                                mySqlCommand.Parameters.AddWithValue("tac_id", oRequest.third_tac_id);
                                mySqlCommand.Parameters.AddWithValue("@usr_id", agentdata.usr_id);

                                if (mySqlCommand.ExecuteNonQuery() == 0)
                                {
                                    Message = "NO SE PUDO DESHABILITAR EL ESTADO DEL TERCER ACCESO DEL AGENTE";
                                    _m = String.Concat("[API] ", LOG_PREFIX, " ", ResponseCode, "-", Message);
                                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                    tran.Rollback();
                                    result.ResponseCode = ResponseCode; result.ResponseMessage = Message; return result;

                                }
                            }
                        }
                        #endregion

                        #endregion


                        #region ROLES DE USUARIO
                        //EN LA CREACION DEL USUARIO SE CREAN LOS REOLES 1,3,4 Y 20 SI es adminstrador
                        //si se agrega o se deshabilita el accesp 20
                        if (oRequest.usr_administrador)
                        {
                            if (!contienaccessoadministrador)
                            {
                                try
                                {

                                    mySqlCommand.CommandText = "INSERT INTO RolUsuario (usr_id,rol_id) VALUES (@userId,@roleId)";
                                    mySqlCommand.Parameters.Clear();
                                    mySqlCommand.Parameters.AddWithValue("@userId", agentdata.usr_id);
                                    mySqlCommand.Parameters.AddWithValue("@roleId", 20);
                                    mySqlCommand.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {

                                    ResponseCode = 14;
                                    Message = "ERROR AL INSERTAR ROL ADMINISTRADOR POSWEB DEL 1er. USUARIO";
                                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                                    tran.Rollback();
                                    result.ResponseCode = ResponseCode; result.ResponseMessage = Message; return result;
                                }
                            }
                        }
                        else
                        {
                            if (contienaccessoadministrador)
                            {

                                try
                                {
                                    mySqlCommand.CommandText = "DELETE FROM [RolUsuario]  WHERE [usr_id] = @usr_id and  [rol_id]  = @rol_id";
                                    mySqlCommand.Parameters.Clear();
                                    mySqlCommand.Parameters.AddWithValue("@usr_id", agentdata.usr_id);
                                    mySqlCommand.Parameters.AddWithValue("@rol_id", cons.ROL_ADMINPOSWEB);
                                    mySqlCommand.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {

                                    ResponseCode = 15;
                                    Message = "ERROR AL INSERTAR ROL ADMINISTRADOR POSWEB DEL 1er. USUARIO";
                                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                                    tran.Rollback();
                                    result.ResponseCode = ResponseCode;
                                    result.ResponseMessage = Message;
                                    return result;
                                }
                            }
                        }

                        #endregion

                        //CREACION DE LA AUDITORIA
                        #region auditoria
                        //Se registra la auditoria indicando la actualizacion del agente por la persona que realiza la operacion
                        ResponseCode = 16;
                        try
                        {
                            decimal secuencia = 0m;

                            mySqlCommand.CommandText = "UPDATE secuencia WITH(ROWLOCK) SET sec_number=sec_number+1 WHERE sec_objectName=@paramValue";
                            mySqlCommand.Parameters.Clear();
                            mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");
                            if (mySqlCommand.ExecuteNonQuery() == 0)
                            {
                                Message = "NO SE PUDO ACTUALIZAR LA SECUENCIA DE LA AUDITORIA";
                                _m = String.Concat("[API] ", LOG_PREFIX, " ", ResponseCode, "-", Message);
                                logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                tran.Rollback();
                                result.ResponseCode = ResponseCode;
                                result.ResponseMessage = Message;
                                return result;
                            }

                            mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WITH(NOLOCK) WHERE sec_objectName=@paramValue";
                            mySqlCommand.Parameters.Clear();
                            mySqlCommand.Parameters.AddWithValue("@paramValue", "AUDITORIA");

                            secuencia = Convert.ToDecimal(mySqlCommand.ExecuteScalar());


                            mySqlCommand.CommandText = "INSERT INTO KcrTransaccion (traId,usrId,traComentario,traFecha,traIdReferencia,traDominio,traSubdominio) VALUES (@traId,@userId,@traComment,@traDate,@traReference,@traDomain,@traSubDomain)";
                            mySqlCommand.Parameters.Clear();
                            mySqlCommand.Parameters.AddWithValue("@traId", secuencia);
                            mySqlCommand.Parameters.AddWithValue("@userId", agentdata.usr_id);
                            // mySqlCommand.Parameters.AddWithValue("@usrIdSuperior", DBNull.Value);
                            mySqlCommand.Parameters.AddWithValue("@traComment", "La agencia " + oRequest.age_id + " fue actualizada por el usuario " + oRequest.usr_id_modificacion);
                            mySqlCommand.Parameters.AddWithValue("@traDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            mySqlCommand.Parameters.AddWithValue("@traReference", agentdata.age_id);
                            mySqlCommand.Parameters.AddWithValue("@traDomain", "AGENTE");
                            mySqlCommand.Parameters.AddWithValue("@traSubDomain", "ACTUALIZACION");
                            mySqlCommand.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {


                            Message = "ERROR AL INSERTAR TRANSACCION ";
                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                            tran.Rollback();
                            result.ResponseCode = ResponseCode;
                            result.ResponseMessage = Message;
                            return result;
                        }

                        //Fin de creacion de agencia
                        #endregion

                        #region stock

                        ///SECUENCIA DE STOCK Y CREACION DE STOCK NA

                        //manejo de stock
                        // no se puede hacer delete insert existe un campo agpEstado
                        //cambiar agpestado por producto stock 
                        //NA
                        ResponseCode = 17;
                        try
                        {

                            if (caneditproducts)
                            {
                                List<RequestAgent.Product> allProductsAvailable = oRequest.productos;
                                if (oRequest.productos.Count == 1 && oRequest.productos[0].prdId == 0)
                                {
                                    allProductsAvailable = GetProductsByZeroAvaibleInStock(agentdata.age_id_sup).ToList();//retorna todos los activos del padre
                                }


                                List<RequestAgent.Product> productosindb = Utils.GetListStockProducts(oRequest.age_id).ToList();


                                mySqlCommand.CommandText = "UPDATE [KlgAgenteProducto] SET [agpEstado] = @agpEstado WHERE [ageId] = @ageId AND  [prdId] = @prdId ";
                                foreach (var pdb in productosindb)
                                {
                                    RequestAgent.Product paux = null;
                                    string estado = "";
                                    try
                                    {
                                        paux = allProductsAvailable.First(p => p.prdId == pdb.prdId);
                                    }
                                    catch (Exception ex) { }


                                    //SI EXISTE EN LA BASE DE DATOS, PERO NO EN LA LISTA
                                    if (paux == null)
                                    {
                                        //cambiar a estado DE

                                        estado = "DE";
                                        if (pdb.prdId != 0)
                                        {
                                            mySqlCommand.Parameters.Clear();
                                            mySqlCommand.Parameters.AddWithValue("@ageId", oRequest.age_id);
                                            mySqlCommand.Parameters.AddWithValue("@prdId", pdb.prdId);
                                            mySqlCommand.Parameters.AddWithValue("@agpEstado", estado);
                                            int aux = mySqlCommand.ExecuteNonQuery();
                                            //      logger.InfoHigh(() => TagValue.New().Message("ACTUALIZANDO KlgAgenteProducto").Tag("[REGISTROS]" + aux).Value(new { ageId = oRequest.age_id, prdId = pdb.prdId, agpEstado = estado }));
                                        }

                                    }



                                }

                                //INSERTAR LOS NUEVOS PRODUCTOS SELECCIONADOS
                                //SOLO SE INSERTAN LOS PRODUCTOS QUE EXISTAN EN EL AGENTE CONCENTRADOR
                                mySqlCommand.CommandText = @" 
                            declare @stkId as decimal
                            declare @staOrden as decimal

                            IF EXISTS (SELECT * FROM KlgAgenteProducto WITH(NOLOCK) WHERE [ageId] = @ageId AND  [prdId] = @prdId )
                             BEGIN
	                            UPDATE [KlgAgenteProducto]   SET  [agpEstado] = @agpEstado 
	                            WHERE [ageId] = @ageId AND  [prdId] = @prdId

	                            IF NOT EXISTS( SELECT * FROM KlgStockAgenteProducto WITH(NOLOCK) WHERE ageId=@ageId and prdId=@prdId ) AND 
	                               EXISTS(SELECT stkId,prdId,ageId,staOrden,stkCantidad FROM KlgStockAgenteProducto WITH(NOLOCK) WHERE ageId=@concentrador and prdId=@prdId )
	                            BEGIN
		                            SELECT @stkId = CONVERT(  DECIMAL,stkId), @staOrden  = CONVERT(  DECIMAL,staOrden) FROM KlgStockAgenteProducto WITH(NOLOCK) WHERE ageId=@concentrador and prdId=@prdId   
		                            INSERT INTO KlgStockAgente (stkId,ageId,staOrden) VALUES (@stkId,@ageId,@staOrden)
	                            END
                             END
                            ELSE IF EXISTS(SELECT stkId,prdId,ageId,staOrden,stkCantidad FROM KlgStockAgenteProducto WITH(NOLOCK) WHERE ageId=@concentrador and prdId=@prdId )
                            BEGIN
	                            SELECT @stkId = CONVERT(  DECIMAL,stkId), @staOrden  = CONVERT(  DECIMAL,staOrden) FROM KlgStockAgenteProducto WITH(NOLOCK) WHERE ageId=@concentrador and prdId=@prdId   
                                INSERT INTO KlgAgenteProducto (ageId,prdId,agpEstado) VALUES (@ageId,@prdId,@agpEstado)
	                            INSERT INTO KlgStockAgente (stkId,ageId,staOrden) VALUES (@stkId,@ageId,@staOrden)
                            END";
                                //
                                foreach (var pdb in allProductsAvailable)
                                {
                                    RequestAgent.Product paux = null;
                                    string estado = "";
                                    try
                                    {
                                        paux = productosindb.First(p => p.prdId == pdb.prdId);
                                    }
                                    catch (Exception ex) { }
                                    mySqlCommand.Parameters.Clear();

                                    //SI EL PRODUCTO SELECCIONADO EN LA NUEVA LISTA DE PRODUCTOS
                                    //NO EXISTE  EN BD ACTUALIZAR EL REGISTRO
                                    if (paux == null)
                                    {
                                        if (pdb.prdId != 0)
                                        {
                                            estado = "AC";
                                            mySqlCommand.Parameters.Clear();
                                            mySqlCommand.Parameters.AddWithValue("@ageId", oRequest.age_id);
                                            mySqlCommand.Parameters.AddWithValue("@prdId", pdb.prdId);
                                            mySqlCommand.Parameters.AddWithValue("@agpEstado", estado);
                                            mySqlCommand.Parameters.AddWithValue("@concentrador", cons.AGENTE_CONCENTRADOR);
                                            int aux = mySqlCommand.ExecuteNonQuery();
                                        }
                                    }

                                }


                            }

                        }
                        catch (Exception ex)
                        {

                            Message = "ERROR AL ACTUALIZAR PRODUCTOS";
                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                            tran.Rollback();
                            result.ResponseCode = ResponseCode;
                            result.ResponseMessage = Message;
                            return result;
                        }
                        #endregion

                        #region comisiones

                        //se actualizan o crean los datos en comisiones

                        bool value = Convert.ToBoolean(ConfigurationManager.AppSettings["COMISSION_BY_SALES"]);
                        //A FUTURO ESCENARIO ESPECIAL , PERMITIR A ALGUNOS AGENTES COMISIONES POR VENTA
                        if (value)// (&& ||) CanHaveComision
                        {
                            GenericApiResult<bool> resultcomisiones = UpdateComisiones(oRequest, agentdata, caneditproducts);

                            if (!resultcomisiones.IsObjectValidResult())
                            {
                                // si no se pudo validar el dato se realiza el roollback
                                tran.Rollback();
                                result.ResponseCode = resultcomisiones.ResponseCode;
                                result.ResponseMessage = resultcomisiones.ResponseMessage;
                                return result;
                            }
                        }


                        //anterior codigo manejaba solo una conexion se agregaba el nombre de la base de datos de comision

                        #endregion


                        tran.Commit();
                        ResponseCode = 0;
                        Message = "TRANSACCION OK";
                        result.ObjectResult = true;
                        result.ResponseCode = ResponseCode;
                        result.ResponseMessage = Message;
                        return result;



                    }

                }
                catch (Exception ex)
                {
                    ResponseCode = 18;
                    Message = "NO SE PUDO ABRIR LA BASE DE DATOS";

                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(_m);
                    result.ResponseCode = ResponseCode;
                    result.ResponseMessage = Message;

                }

            }
            catch (Exception ex)
            {

                ResponseCode = 19;
                Message = "ERROR INESPERADO UPDATE AGENT ";

                _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                logger.ErrorLow(_m);

                result.ResponseCode = ResponseCode;
                result.ResponseMessage = Message;


            }

            return result;

        }

        private static IEnumerable<RequestAgent.Product> GetProductsByZeroAvaibleInStock(decimal agent_parent/*,bool all=true*/)//No hace falta el all porque solo se llama en el metodo de actualziar
        {

            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;

            using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
            {


                mySqlConnection.Open();

                SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                // SqlTransaction tran = null;

                mySqlCommand.Connection = mySqlConnection;

                // datos del agente y del usuario 
                mySqlCommand.CommandText = " SELECT prdId FROM KlgAgenteProducto WITH(NOLOCK) WHERE not prdid=0 and ageId = @ageId and agpEstado=@agpEstado";
                mySqlCommand.Parameters.AddWithValue("@ageId", agent_parent);
                mySqlCommand.Parameters.AddWithValue("@agpEstado", "AC");


                using (var reader = mySqlCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                            yield return new RequestAgent.Product() { prdId = Decimal.ToInt32((decimal)reader["prdId"]) };
                }

            }


        }
        /// <summary>
        /// Actauliza o crea los datos de la comisiones
        /// Pre Condicion: 
        /// Para actualizar los datos deben existir datos en la tabla KmmEntidadExtension KmmEntidad
        /// Para la creacion el agente padre de la agencia debe estar registrado en en la base de datos de Comisiones KmmAgente
        /// Post Condicion:
        /// Se crea los datos del agente relacionados en la base de datos comisiones, con estados AC y sin datos para Puntos disponibles 
        /// </summary>
        /// <param name="oRequest">Datos de para actualizar</param>
        /// <param name="agentdata">datos del agente necesario para realizar la actualizacion [agentdata {age_id, age_id_sup, usr_id}]        
        /// <returns>GenericApiResult indicando true si pudo concluir la operacion o false si no pudo terminar la operacion</returns>
        public static GenericApiResult<bool> UpdateComisiones(RequestAgent oRequest, dynamic agentdata, bool caneditprodutcs = true)
        {

            //variable para mapear datos de persistencia del agente
            GenericApiResult<bool> result = new GenericApiResult<bool>();
            result.ObjectResult = false;
            RequestAgent obj = new RequestAgent();
            Int32 ResponseCode = 0;
            String Message = "";


            string _m = "";
            int? entId = 0;

            try
            {


                logger.InfoLow(() => TagValue.New().Tag("[OP]").Value("UpdateComisiones Actualizar comisiones"));
                string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString.Replace("TRAN_", "COMI_"); ;




                try
                {

                    using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                    {
                        mySqlConnection.Open();
                        SqlCommand mySqlCommand = new SqlCommand();
                        SqlTransaction tran = null;
                        SqlDataReader mySqlDataReader = null;




                        mySqlCommand.Connection = mySqlConnection;
                        mySqlCommand.CommandTimeout = 120;

                        //POR AGENTE ID SELECCIONAR DATOS COMO EL 
                        //se va a manejar agenteid

                        ResponseCode = 2;


                        // se incia la transaccion cuando se van empezar a actualizar los datos
                        tran = mySqlConnection.BeginTransaction();
                        mySqlCommand.Transaction = tran;



                        #region comisiones
                        //consulta la llave primaria en la base de datos de comisiones 
                        try
                        {
                            entId = GetEnitdadId(oRequest.age_id);
                        }
                        catch (Exception ex)
                        {

                            Message = "ERROR AL SELECCIONAR ENTIDAD EN COMISION";
                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                            tran.Rollback();
                            result.ResponseCode = ResponseCode; result.ResponseMessage = Message; return result;
                        }

                        //si no existe datos del agente en comisiones
                        //crea los respectivos datos
                        if (!entId.HasValue)
                        {
                            #region creacion de datos en comisiones
                            decimal entIdContenedora = 0m;
                            ResponseCode = 3;

                            try
                            {
                                //actualiza la secuencia para la creacion en KmmAgente
                                mySqlCommand.CommandText = "UPDATE secuencia WITH(ROWLOCK) SET sec_number=sec_number+1 WHERE sec_objectName = @paramValue";
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@paramValue", "KMMENTIDAD");
                                if (mySqlCommand.ExecuteNonQuery() == 0)
                                {

                                    Message = "NO SE PUDO ACTUALIZAR LA SECUENCIA DEL KMMENTIDAD";
                                    _m = String.Concat("[API] ", LOG_PREFIX, " ", ResponseCode, "-", Message);
                                    logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));
                                    tran.Rollback();
                                    result.ResponseCode = ResponseCode;
                                    result.ResponseMessage = Message;
                                    return result;
                                }

                            }
                            catch (Exception ex)
                            {

                                Message = "ERROR AL ACTUALIZAR LA SECUENCIA DEL KMMENTIDAD";
                                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                                tran.Rollback();
                                result.ResponseCode = ResponseCode;
                                result.ResponseMessage = Message;
                                return result;
                            }

                            ResponseCode = 4;
                            try
                            {
                                mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WITH(NOLOCK) WHERE sec_objectName= @paramValue";
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@paramValue", "KMMENTIDAD");
                                entId = Decimal.ToInt32(Convert.ToDecimal(mySqlCommand.ExecuteScalar()));
                            }
                            catch (Exception ex)
                            {

                                Message = "ERROR AL SELECCIONAR LA SECUENCIA DEL KMMENTIDAD";
                                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                                tran.Rollback();
                                result.ResponseCode = ResponseCode;
                                result.ResponseMessage = Message;
                                return result;
                            }
                            ResponseCode = 5;
                            try
                            {
                                //selecciona la entidad superior
                                mySqlCommand.CommandText = "SELECT entId FROM KmmAgente WITH(NOLOCK) WHERE ageId = @ageId";
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@ageId", agentdata.age_id_sup);
                                entIdContenedora = (decimal)mySqlCommand.ExecuteScalar();
                            }
                            catch (Exception ex)
                            {


                                Message = "ERROR AL SELECCIONAR LA ENTIDAD CONTENEDORA DE LA AGENCIA";
                                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                                tran.Rollback();
                                result.ResponseCode = ResponseCode;
                                result.ResponseMessage = Message;
                                return result;
                            }
                            ResponseCode = 6;
                            try
                            {
                                // inserta la entidad
                                mySqlCommand.CommandText = "INSERT INTO KmmEntidad (entId,etyId,entIdContenedora,entDenominacion,entEstado) VALUES (@entId,@etyId,@entIdContainer,@ageName,@entEstado)";
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@entId", entId);
                                mySqlCommand.Parameters.AddWithValue("@etyId", "A");
                                mySqlCommand.Parameters.AddWithValue("@entIdContainer", entIdContenedora);
                                mySqlCommand.Parameters.AddWithValue("@ageName", oRequest.age_nombre);
                                mySqlCommand.Parameters.AddWithValue("@entEstado", "AC");
                                mySqlCommand.ExecuteNonQuery();
                            }
                            catch (SqlException ex)
                            {

                                ResponseCode += 200;
                                Message = "[SQLEXCEPTION] ERROR BASE DE DATOS AL CREAR DATOS DE COMISION ENTIDAD";

                                //547
                                switch (ex.Number)
                                {
                                    //FOERING KEY
                                    case 547:
                                        /*
                                        if (GeneralSettings.GetCurrentContry() == Countries.COLOMBIA) { 
                                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                                        }
                                        else*/
                                        {
                                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                            tran.Rollback();
                                            result.ResponseCode = ResponseCode;
                                            result.ResponseMessage = Message;
                                            return result;
                                        }

                                        break;

                                    default:
                                        //DEFAULT
                                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                        tran.Rollback();
                                        result.ResponseCode = ResponseCode;
                                        result.ResponseMessage = Message;
                                        return result;
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {

                                Message = "ERROR AL INSERTAR EN KMM_ENTIDAD";
                                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                                tran.Rollback();
                                result.ResponseCode = ResponseCode;
                                result.ResponseMessage = Message;
                                return result;
                            }
                            ResponseCode = 7;
                            try
                            {
                                //inserta el registro en KmmAgente
                                mySqlCommand.CommandText = "INSERT INTO KmmAgente (ageId,agePermisoUso,agePermisoDelegacion,entId) VALUES (@ageId,@agePermisoUso,@agePermisoDelegacion,@entId)";
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@ageId", agentdata.age_id);
                                mySqlCommand.Parameters.AddWithValue("@agePermisoUso", "N");
                                mySqlCommand.Parameters.AddWithValue("@agePermisoDelegacion", "N");
                                mySqlCommand.Parameters.AddWithValue("@entId", entId);
                                mySqlCommand.ExecuteNonQuery();
                            }
                            catch (SqlException ex)
                            {

                                ResponseCode += 200;
                                Message = "[SQLEXCEPTION] ERROR BASE DE DATOS AL CREAR DATOS DE COMISION AGENTE";

                                //547
                                switch (ex.Number)
                                {
                                    //FOERING KEY
                                    case 547:
                                        /*
                                        if (GeneralSettings.GetCurrentContry() == Countries.COLOMBIA)
                                        {
                                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                                        }
                                        else*/
                                        {
                                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                            tran.Rollback();
                                            result.ResponseCode = ResponseCode;
                                            result.ResponseMessage = Message;
                                            return result;
                                        }

                                        break;

                                    default:
                                        //DEFAULT
                                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                        tran.Rollback();
                                        result.ResponseCode = ResponseCode;
                                        result.ResponseMessage = Message;
                                        return result;
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {

                                Message = "ERROR AL INSERTAR EN KMM_AGENTE";
                                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                                tran.Rollback();
                                result.ResponseCode = ResponseCode;
                                result.ResponseMessage = Message;
                                return result;
                            }

                            ResponseCode = 8;
                            try
                            {
                                //inserta los datos en KmmEntidadExtension
                                mySqlCommand.CommandText = "INSERT INTO KmmEntidadExtension (entId,eneRazonSocial,eneNombre,eneDocumento,eneCalle,ciuId,eneTelefono,eneCelular,eneEmail,enePuntosDisponibles,eneAgrupaPuntos,eneCodigo) VALUES  (@entId,@ageLegalName,@ageName,@ageCuit,@ageAddress,@ageCityId,@agePhone,@ageMobile,@ageEmail,@enePuntosDisponibles,@eneAgrupaPuntos,@eneCodigo)";
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@entId", entId);
                                mySqlCommand.Parameters.AddWithValue("@ageLegalName", oRequest.age_razonsocial);
                                mySqlCommand.Parameters.AddWithValue("@ageName", oRequest.age_nombre);
                                mySqlCommand.Parameters.AddWithValue("@ageCuit", oRequest.age_cuit);
                                mySqlCommand.Parameters.AddWithValue("@ageAddress", oRequest.age_direccion);
                                mySqlCommand.Parameters.AddWithValue("@ageCityId", oRequest.age_ciu_id);
                                mySqlCommand.Parameters.AddWithValue("@agePhone", oRequest.age_tel);
                                mySqlCommand.Parameters.AddWithValue("@ageMobile", oRequest.age_cel);
                                mySqlCommand.Parameters.AddWithValue("@ageEmail", oRequest.age_email);
                                mySqlCommand.Parameters.AddWithValue("@enePuntosDisponibles", 0);
                                mySqlCommand.Parameters.AddWithValue("@eneAgrupaPuntos", "N");
                                mySqlCommand.Parameters.AddWithValue("@eneCodigo", String.Concat("A", agentdata.age_id));

                                mySqlCommand.ExecuteNonQuery();
                            }
                            catch (SqlException ex)
                            {

                                ResponseCode += 200;
                                Message = "[SQLEXCEPTION] ERROR BASE DE DATOS AL CREAR DATOS DE COMISION ENTIDAD EXTENSION";

                                //547
                                switch (ex.Number)
                                {
                                    //FOERING KEY
                                    case 547:
                                        /*
                                    if (GeneralSettings.GetCurrentContry() == Countries.COLOMBIA)
                                    {
                                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                                    }
                                    else*/
                                        {
                                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                            tran.Rollback();
                                            result.ResponseCode = ResponseCode;
                                            result.ResponseMessage = Message;
                                            return result;
                                        }

                                        break;

                                    default:
                                        //DEFAULT
                                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                        tran.Rollback();
                                        result.ResponseCode = ResponseCode;
                                        result.ResponseMessage = Message;
                                        return result;
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {


                                Message = "ERROR AL CREAR EN KMMENTIDADEXTENSION";
                                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                                tran.Rollback();
                                result.ResponseCode = ResponseCode;
                                result.ResponseMessage = Message;
                                return result;
                            }


                            ResponseCode = 9;
                            try
                            {
                                // inserta los datosn en KmmModeloAgente
                                mySqlCommand.CommandText = "INSERT INTO KmmModeloAgente (mdlId,ageId,magEstado) VALUES (@modId,@ageId,@status)";
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@modId", "Kmm.Models.Commissions.PercentageOfSalesModel");
                                mySqlCommand.Parameters.AddWithValue("@ageId", agentdata.age_id);
                                mySqlCommand.Parameters.AddWithValue("@status", "AC");
                                mySqlCommand.ExecuteNonQuery();
                            }
                            catch (SqlException ex)
                            {

                                ResponseCode += 200;
                                Message = "[SQLEXCEPTION] ERROR BASE DE DATOS AL CREAR DATOS DE COMISION MODELO AGENTE";

                                //547
                                switch (ex.Number)
                                {
                                    //FOERING KEY
                                    case 547:
                                        /*
                                        if (GeneralSettings.GetCurrentContry() == Countries.COLOMBIA)
                                        {
                                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                                        }
                                        else*/
                                        {
                                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                            tran.Rollback();
                                            result.ResponseCode = ResponseCode;
                                            result.ResponseMessage = Message;
                                            return result;
                                        }

                                        break;

                                    default:
                                        //DEFAULT
                                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                        tran.Rollback();
                                        result.ResponseCode = ResponseCode;
                                        result.ResponseMessage = Message;
                                        return result;
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Message = "ERROR AL CREAR EN KMMENTIDADEXTENSION";
                                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                                tran.Rollback();
                                result.ResponseCode = ResponseCode;
                                result.ResponseMessage = Message;
                                return result;
                            }

                            #endregion
                        }
                        //TODO LA CONEXION ES A LA BASE DE DATOS DE TRAN COL
                        else
                        {
                            //actualizar los datos en la base de datos de comisiones
                            #region KmmEntidadExtension Y KmmEntidad
                            ResponseCode = 10;
                            try
                            {
                                #region all update
                                //ALLQUERY UPDATE [dbo].[KmmEntidadExtension]        SET [eneRazonSocial] = @eneRazonSocial      ,[eneNombre] = @eneNombre      ,[eneApellido] = @eneApellido      ,[eneDocumento] = @eneDocumento      ,[eneCalle] = @eneCalle      ,[eneNro] = @eneNro      ,[enePiso] = @enePiso      ,[eneDpto] = @eneDpto      ,[exeCodigoPostal] = @exeCodigoPostal      ,[ciuId] = @ciuId      ,[eneSexo] = @eneSexo      ,[eneNacimiento] = @eneNacimiento      ,[eneTelefono] = @eneTelefono      ,[eneCelular] = @eneCelular      ,[eneEmail] = @eneEmail      ,[eneEstadoCivil] = @eneEstadoCivil      ,[eneHijos] = @eneHijos      ,[enePuntosDisponibles] = @enePuntosDisponibles      ,[eneAgrupaPuntos] = @eneAgrupaPuntos      WHERE [eneCodigo] = @eneCodigo
                                #endregion

                                mySqlCommand.CommandText = "UPDATE [KmmEntidadExtension]  WITH(ROWLOCK)      SET [eneRazonSocial] = @eneRazonSocial      ,[eneNombre] = @eneNombre       ,[eneDocumento] = @eneDocumento      ,[eneCalle] = @eneCalle             ,[ciuId] = @ciuId        ,[eneTelefono] = @eneTelefono      ,[eneCelular] = @eneCelular      ,[eneEmail] = @eneEmail            ,[enePuntosDisponibles] = @enePuntosDisponibles  ,[eneAgrupaPuntos]  = @eneAgrupaPuntos        WHERE [entId] = @entId";
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@eneRazonSocial", oRequest.age_razonsocial);
                                mySqlCommand.Parameters.AddWithValue("@eneNombre", oRequest.age_nombre);
                                mySqlCommand.Parameters.AddWithValue("@eneDocumento", oRequest.age_cuit);
                                mySqlCommand.Parameters.AddWithValue("@eneCalle", oRequest.age_direccion);
                                mySqlCommand.Parameters.AddWithValue("@ciuId", oRequest.age_ciu_id);
                                mySqlCommand.Parameters.AddWithValue("@eneTelefono", oRequest.age_tel);
                                mySqlCommand.Parameters.AddWithValue("@eneCelular", oRequest.age_cel);
                                mySqlCommand.Parameters.AddWithValue("@eneEmail", oRequest.age_email);
                                mySqlCommand.Parameters.AddWithValue("@enePuntosDisponibles", 0);
                                mySqlCommand.Parameters.AddWithValue("@eneAgrupaPuntos", "N");
                                mySqlCommand.Parameters.AddWithValue("@entId", entId);


                                if (mySqlCommand.ExecuteNonQuery() == 0)
                                {
                                    Message = "NO SE PUDO ACTUALIZAR LOS DATOS DEL AGENTE EN COMISION";
                                    _m = String.Concat("[API] ", LOG_PREFIX, " ", ResponseCode, "-", Message);
                                    logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));
                                    tran.Rollback();
                                    result.ResponseCode = ResponseCode; result.ResponseMessage = Message; return result;

                                }
                            }
                            catch (SqlException ex)
                            {

                                ResponseCode += 200;
                                Message = "[SQLEXCEPTION] ERROR BASE DE DATOS AL ACTUALIZAR DATOS DE COMISION  ENTIDAD EXTENSION";

                                //547
                                switch (ex.Number)
                                {
                                    //FOERING KEY
                                    case 547:

                                        if (GeneralSettings.GetCurrentContry() == Countries.COLOMBIA)
                                        {
                                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                            logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));

                                        }
                                        else
                                        {
                                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                            logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));
                                            tran.Rollback();
                                            result.ResponseCode = ResponseCode;
                                            result.ResponseMessage = Message;
                                            return result;
                                        }

                                        break;

                                    default:
                                        //DEFAULT
                                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                        logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));
                                        tran.Rollback();
                                        result.ResponseCode = ResponseCode;
                                        result.ResponseMessage = Message;
                                        return result;
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {

                                Message = "ERROR AL ACTUALIZAR LOS DATOS DEL AGENTE EN COMISION";
                                _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));

                                tran.Rollback();
                                result.ResponseCode = ResponseCode;
                                result.ResponseMessage = Message;
                                return result;

                            }

                            ResponseCode = 11;
                            try
                            {

                                mySqlCommand.CommandText = "UPDATE [KmmEntidad]   WITH(ROWLOCK)  SET   [entDenominacion] = @entDenominacion, [entEstado] = @entEstado        WHERE [entId] = @entId";
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@entDenominacion", oRequest.age_nombre);
                                mySqlCommand.Parameters.AddWithValue("@entEstado", "AC");
                                mySqlCommand.Parameters.AddWithValue("@entId", entId.Value);
                                // mySqlCommand.Parameters.AddWithValue("@eneAgrupaPuntos", "N");
                                if (mySqlCommand.ExecuteNonQuery() == 0)
                                {
                                    Message = "NO SE PUDO ACTUALIZAR DE ENTIDAD EN COMISION";
                                    _m = String.Concat("[API] ", LOG_PREFIX, " ", ResponseCode, "-", Message);
                                    logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));
                                    tran.Rollback();
                                    result.ResponseCode = ResponseCode; result.ResponseMessage = Message; return result;

                                }
                            }
                            catch (SqlException ex)
                            {

                                ResponseCode += 200;
                                Message = "[SQLEXCEPTION] ERROR BASE DE DATOS AL ACTUALIZAR DATOS DE COMISION ENTIDAD";

                                //547
                                switch (ex.Number)
                                {
                                    //FOERING KEY
                                    case 547:
                                        /*
                                        if (GeneralSettings.GetCurrentContry() == Countries.COLOMBIA)
                                        {
                                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                                        }
                                        else*/
                                        {
                                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                            logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));
                                            tran.Rollback();
                                            result.ResponseCode = ResponseCode;
                                            result.ResponseMessage = Message;
                                            return result;
                                        }

                                        break;

                                    default:
                                        //DEFAULT
                                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                        logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));
                                        tran.Rollback();
                                        result.ResponseCode = ResponseCode;
                                        result.ResponseMessage = Message;
                                        return result;
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {

                                Message = "ERROR AL ACTUALIZAR LOS DATOS DEL AGENTE EN COMISION";
                                _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));

                                tran.Rollback();
                                result.ResponseCode = ResponseCode;
                                result.ResponseMessage = Message;
                                return result;

                            }
                            #endregion
                        }

                        #region Grupo
                        ResponseCode = 12;
                        try
                        {
                            // si tiene grupo se inserta de lo contrario no lo inserta
                            if (oRequest.grpId > 0)
                            {
                                //inserta la entidad KmmGrupoEntidad en casod e no existir
                                //mySqlCommand.CommandText = "IF NOT EXISTS  (SELECT * FROM KmmGrupoEntidad WITH(NOLOCK) WHERE grpId = @grpId AND entId = @entId)  INSERT INTO KmmGrupoEntidad (grpId,entId) VALUES (@grpId,@entId)";

                                mySqlCommand.CommandText =
                                    @"DECLARE @countRecords INT;
                                        SELECT @countRecords = Count(*) FROM KmmGrupoEntidad WITH(NOLOCK) WHERE entId = @entId;
                                        IF (@countRecords = 0)
                                        BEGIN
                                            INSERT INTO KmmGrupoEntidad (grpId,entId) VALUES (@grpId,@entId);
                                        END
                                        ELSE IF (@countRecords = 1)
                                        BEGIN
                                            UPDATE KmmGrupoEntidad set grpId = @grpId WHERE  entId = @entId;
                                        END
                                        ELSE IF (@countRecords > 1)
                                        BEGIN
                                            DELETE KmmGrupoEntidad WHERE entId = @entId;
	                                        INSERT INTO KmmGrupoEntidad (grpId,entId) VALUES (@grpId,@entId);
                                        END";

                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@grpId", oRequest.grpId);
                                mySqlCommand.Parameters.AddWithValue("@entId", entId);
                                mySqlCommand.ExecuteNonQuery();
                            }
                        }
                        catch (SqlException ex)
                        {

                            ResponseCode += 200;
                            Message = "[SQLEXCEPTION] ERROR BASE DE DATOS AL ACTUALIZAR DATOS DE GRUPO ENTIDAD";

                            //547
                            switch (ex.Number)
                            {
                                //FOERING KEY
                                case 547:

                                    if (GeneralSettings.GetCurrentContry() == Countries.COLOMBIA)
                                    {
                                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                        logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));

                                    }
                                    else
                                    {
                                        _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                        logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));
                                        tran.Rollback();
                                        result.ResponseCode = ResponseCode;
                                        result.ResponseMessage = Message;
                                        return result;
                                    }

                                    break;

                                default:
                                    //DEFAULT
                                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                    logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));
                                    tran.Rollback();
                                    result.ResponseCode = ResponseCode;
                                    result.ResponseMessage = Message;
                                    return result;
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {


                            Message = "ERROR  AL CREAR KMMGRUPOENTIDAD";
                            logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                            tran.Rollback();
                            result.ResponseCode = ResponseCode;
                            result.ResponseMessage = Message;
                            return result;
                        }
                        #endregion

                        ResponseCode = 20;
                        if (entId.HasValue)
                        {
                            #region productoscomision

                            //MODIFICAR DATOS EN BASE DE DATOS DE COMISION
                            //TENER ENCUENTA LA CONEXION
                            //tienen unos cuantos y seleciona todos para comision 
                            //tiene todos y deselecciona la opcion y selecciona unos cuantos
                            // cambios left join lin

                            //if (oRequest.comisionporventa)

                            if (caneditprodutcs)
                            {

                                #region caneditproducts

                                // se borra comision aplicado a todos los productos 
                                mySqlCommand.CommandText = " DELETE [dbo].[KmmVariableEntidadAgente] where [entId] = @entId and [mdlId] = @mdlId and [ageId] = @ageId";
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@entId", entId);
                                mySqlCommand.Parameters.AddWithValue("@ageId", agentdata.age_id_sup);
                                mySqlCommand.Parameters.AddWithValue("@mdlId", "Kmm.Models.Commissions.PercentageOfSalesModel");
                                mySqlCommand.ExecuteNonQuery();

                                //se borran todos los productos que tengan comision por venta
                                mySqlCommand.CommandText = " DELETE FROM [KmmVariableEntidadProductoAgen]  WHERE varId = @varId AND entId = @entId AND ageId =@ageId  AND mdlId = @mdlId";
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@varId", "commissionPercentage");
                                mySqlCommand.Parameters.AddWithValue("@entId", entId);
                                mySqlCommand.Parameters.AddWithValue("@ageId", agentdata.age_id_sup);
                                mySqlCommand.Parameters.AddWithValue("@mdlId", "Kmm.Models.Commissions.PercentageOfSalesModel");
                                mySqlCommand.ExecuteNonQuery();


                                //TODO tener encuenta que no puede tener una comision mas alta que los productos del padre
                                List<RequestAgent.Product> allProductsAvailable = oRequest.productos;

                                if (oRequest.productos.Count == 1 && oRequest.productos[0].prdId == 0)
                                {
                                    IEnumerable<RequestAgent.Product> partialEnumeration = GetProductsByZeroAvaibleInStock(agentdata.age_id_sup);
                                    allProductsAvailable = partialEnumeration.ToList<RequestAgent.Product>();
                                    allProductsAvailable.ToList().ForEach(s => s.comision = oRequest.productos[0].comision);
                                }


                                List<RequestAgent.Product> listProductCommi = GetProductsFromCommi(allProductsAvailable);


                                int index = 0;
                                // producto 

                                try
                                {
                                    decimal d = agentdata.age_id_sup;
                                    int entSup = GetEnitdadId(d).Value;
                                    // lista de las comisiones por producto de la agencia padre
                                    List<RequestAgent.Product> listproducts = GetComissionSalesProducts(entSup);

                                    if (ApiConfiguration.VALIDATE_COMISSION_BY_SALES)
                                    {
                                        bool hasError = false;
                                        for (; index < listProductCommi.Count; index++)
                                        {
                                            // si existe acutalizar si no insertar

                                            var item = listProductCommi[index];

                                            RequestAgent.Product itemparent = listproducts.Find(p => p.prdId == item.prdId);

                                            bool save = (itemparent == null || itemparent.comision == 0 || item.comision <= itemparent.comision);

                                            if (save)
                                            {

                                                mySqlCommand.CommandText = "  INSERT INTO [KmmVariableEntidadProductoAgen] (varId,entId,ageId,prdId,mdlId,vepValor)  VALUES (@varId,@entId,@ageId,@prdId,@mdlId,@vepValor)";
                                                mySqlCommand.Parameters.Clear();
                                                mySqlCommand.Parameters.AddWithValue("@varId", "commissionPercentage");
                                                mySqlCommand.Parameters.AddWithValue("@entId", entId);
                                                mySqlCommand.Parameters.AddWithValue("@ageId", agentdata.age_id_sup);
                                                mySqlCommand.Parameters.AddWithValue("@prdId", item.prdId);
                                                mySqlCommand.Parameters.AddWithValue("@mdlId", "Kmm.Models.Commissions.PercentageOfSalesModel");

                                                mySqlCommand.Parameters.AddWithValue("@vepValor", item.comision);
                                                mySqlCommand.ExecuteNonQuery();

                                            }
                                            else
                                            {
                                                decimal comision = itemparent == null ? 0m : itemparent.comision;
                                                string nombre = GetNameProduct(item.prdId);
                                                _m = String.Concat("LA COMISION PARA EL PRODUCTO ", nombre, " NO PUEDE SER MAYOR A ", comision);
                                                logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));
                                                result.ResponseCode = ResponseCode;
                                                result.ResponseMessage = result.ResponseMessage + "\n" + _m;
                                                hasError = true;

                                            }
                                        }

                                        if (hasError)
                                        {
                                            tran.Rollback();
                                            return result;
                                        }
                                    }
                                    else
                                    {
                                        for (; index < listProductCommi.Count; index++)
                                        {

                                            var item = listProductCommi[index];
                                            mySqlCommand.CommandText = "  INSERT INTO [KmmVariableEntidadProductoAgen] (varId,entId,ageId,prdId,mdlId,vepValor)  VALUES (@varId,@entId,@ageId,@prdId,@mdlId,@vepValor)";
                                            mySqlCommand.Parameters.Clear();
                                            mySqlCommand.Parameters.AddWithValue("@varId", "commissionPercentage");
                                            mySqlCommand.Parameters.AddWithValue("@entId", entId);
                                            mySqlCommand.Parameters.AddWithValue("@ageId", agentdata.age_id_sup);
                                            mySqlCommand.Parameters.AddWithValue("@prdId", item.prdId);
                                            mySqlCommand.Parameters.AddWithValue("@mdlId", "Kmm.Models.Commissions.PercentageOfSalesModel");
                                            mySqlCommand.Parameters.AddWithValue("@vepValor", item.comision);
                                            mySqlCommand.ExecuteNonQuery();

                                        }

                                    }


                                }
                                catch (SqlException ex)
                                {
                                    string productData = "";
                                    if (index < oRequest.productos.Count)
                                    {
                                        var item = oRequest.productos[index];

                                        productData = String.Concat("PrdId = ", item.prdId, " Comision = ", item.comision);
                                    }
                                    ResponseCode = 214;
                                    Message = "[SQLEXCEPTION] ERROR BASE DE DATOS AL ACTUALIZAR DATOS DE COMISION";

                                    //547
                                    switch (ex.Number)
                                    {
                                        //FOERING KEY
                                        case 547:
                                            /*
                                            if (GeneralSettings.GetCurrentContry() == Countries.COLOMBIA) { 
                                                _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));

                                            }
                                            else*/
                                            {
                                                _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, " ", "[ERROR DE INTEGRIDAD REFERENCIAL] ", productData, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                                logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));
                                                tran.Rollback();
                                                result.ResponseCode = ResponseCode;
                                                result.ResponseMessage = Message;
                                                return result;
                                            }

                                            break;

                                        default:
                                            //= 547

                                            _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                            logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));
                                            tran.Rollback();
                                            result.ResponseCode = ResponseCode;
                                            result.ResponseMessage = Message;
                                            return result;
                                            break;
                                    }


                                }

                                catch (Exception ex)
                                {

                                    ResponseCode = 14;
                                    Message = "ERROR AL ACTUALIZAR DATOS DE COMISION DE PRODUCTO";
                                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                                    logger.ErrorLow(() => TagValue.New().Message(_m).Tag("[INPUT]").Value(GetParameters(mySqlCommand)));
                                    tran.Rollback();
                                    result.ResponseCode = ResponseCode;
                                    result.ResponseMessage = Message;
                                    return result;
                                }

                                ///


                                #endregion
                            }
                            else
                            {
                                logger.InfoLow(String.Concat("[QRY] ", LOG_PREFIX, " NO SE PUEDE EDIAR LA COMISION POR VENTA DE LOS PRODUCTOS"));
                            }

                            /*
                            else
                            {
                                // debiria quitar la condicion on money bag
                                //"impactOnMoneyBag");commissionPercentage

                            }
                             * */
                            #endregion
                        }
                        else
                        {
                            logger.InfoLow(String.Concat("NO SE GUARDAN DATOS PARA PRODUCTO COMISION AGENTE ", agentdata.age_id));
                        }

                        #endregion


                        tran.Commit();
                        ResponseCode = 0;
                        Message = "TRANSACCION OK";
                        result.ObjectResult = true;
                        result.ResponseCode = ResponseCode;
                        result.ResponseMessage = Message;
                        return result;
                    }

                }
                catch (Exception ex)
                {
                    ResponseCode = 15;
                    Message = "NO SE PUDO ABRIR LA BASE DE DATOS";

                    _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                    logger.ErrorLow(_m);
                    result.ResponseCode = ResponseCode;
                    result.ResponseMessage = Message;

                }

            }
            catch (Exception ex)
            {

                ResponseCode = 16;
                Message = "ERROR INESPERADO UPDATE AGENT ";

                _m = String.Concat("[QRY] ", LOG_PREFIX, " ", ResponseCode, "-", Message, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                logger.ErrorLow(_m);

                result.ResponseCode = ResponseCode;
                result.ResponseMessage = Message;


            }

            return result;

        }

        private static List<RequestAgent.Product> GetComissionSalesProducts(decimal entConenedora)
        {


            //LISTA DE DATOS PARA VALIDAR LOS DATOS DE LA COMISION DE VENTA
            //selecciona la entidad superior
            //seleccionar entidad 
            //mySqlCommand.CommandText = "SELECT entId FROM KmmAgente WHERE ageId = @ageId";
            //mySqlCommand.Parameters.Clear();
            //mySqlCommand.Parameters.AddWithValue("@ageId", agentdata.age_id_sup);
            //entIdContenedora = (decimal)mySqlCommand.ExecuteScalar();

            //seleccionar la lista de datos de mi padre

            List<RequestAgent.Product> lista = new List<RequestAgent.Product>();


            try
            {

                string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString.Replace("TRAN_", "COMI_");

                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {
                    mySqlConnection.Open();

                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                    mySqlCommand.CommandText = "SELECT  kp.prdId ,kp.vepValor FROM KmmVariableEntidadProductoAgen as kp  WITH(NOLOCK)   WHERE    kp.entId = @ageId and  kp.varId = 'commissionPercentage' and kp.mdlId =  'Kmm.Models.Commissions.PercentageOfSalesModel'";
                    mySqlCommand.Parameters.AddWithValue("@ageId", entConenedora);

                    using (var reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                            {
                                lista.Add(new RequestAgent.Product()
                                {
                                    prdId = Convert.ToInt32(reader["prdId"]),
                                    comision = Convert.ToDecimal(reader["vepValor"])
                                });
                            }
                    }
                }


                return lista;

            }
            catch (Exception ex)
            {
                throw;
            }


        }
        private static string GetNameProduct(int producto)
        {


            //LISTA DE DATOS PARA VALIDAR LOS DATOS DE LA COMISION DE VENTA
            //selecciona la entidad superior
            //seleccionar entidad 
            //mySqlCommand.CommandText = "SELECT entId FROM KmmAgente WHERE ageId = @ageId";
            //mySqlCommand.Parameters.Clear();
            //mySqlCommand.Parameters.AddWithValue("@ageId", agentdata.age_id_sup);
            //entIdContenedora = (decimal)mySqlCommand.ExecuteScalar();

            //seleccionar la lista de datos de mi padre


            string nombre = "";

            try
            {

                string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString.Replace("TRAN_", "COMI_");

                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {
                    mySqlConnection.Open();

                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                    mySqlCommand.CommandText = "SELECT kp.prdNombre FROM KmmProducto as kp WITH(NOLOCK) WHERE  kp.prdId = @prdId";
                    mySqlCommand.Parameters.AddWithValue("@prdId", producto);


                    object obj = mySqlCommand.ExecuteScalar();
                    nombre = obj != null ? obj.ToString() : String.Concat("[Producto ", producto, "]");

                }


                return nombre;

            }
            catch (Exception ex)
            {
                logger.ErrorLow(String.Concat("ERROR BUSCANDO NOMBRE DEL PRODUCTO [", producto, "] [", ex.GetType().Name, "] [", ex.StackTrace, "]"));
                throw;
            }


        }
        /// <summary>
        /// Retorna el entId en al base de datos de comisiones para el agente
        /// identificado en age_id
        /// </summary>
        /// <param name="age_id">agente que se esta editando</param>
        /// <returns></returns>
        private static Int32? GetEnitdadId(decimal age_id)
        {
            int result = 0;
            try
            {

                string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString.Replace("TRAN_", "COMI_");

                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {
                    mySqlConnection.Open();

                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                    mySqlCommand.CommandText = "SELECT [entId]  FROM [KmmAgente] WITH(NOLOCK) where [ageId]  = @ageId";
                    mySqlCommand.Parameters.AddWithValue("@ageId", age_id);

                    object aux = mySqlCommand.ExecuteScalar();
                    if (aux != null)
                        result = Convert.ToInt32(aux);
                    else
                        return null; // throw new Exception("NO SE ECONTRO ENTID PARA LA AGENCIA " + age_id);
                }
                return result;

            }
            catch (Exception ex)
            {
                throw;
            }

        }



        public static Dictionary<string, object> GetDistributionByExternalIdRequest(GetTransactionRequestBody request)
        {

            Dictionary<string, object> _dictionary = new Dictionary<string, object>();

            try
            {

                string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;

                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {
                    mySqlConnection.Open();

                    using (SqlCommand mySqlCommand = mySqlConnection.CreateCommand())
                    {


                        mySqlCommand.CommandText = @"SELECT    TOP (1) sp.sprId, sp.sprImporteSolicitud, sp.sprEstado, sp.sprFechaAprobacion, sp.sltId, e.envId, e.envEstado, e.envFechaEnvio, e.envFechaRecepcion, e.envNumeroRemito, e.envObservaciones, 
                         e.ageIdBodegaOrigen, e.ageIdBodegaDestino, age_sol.age_nombre AS AgenteSolicitante, age_des.age_nombre AS AgenteDestinatario, Sol_type.sltDescripcion
FROM            KlgSolicitudProducto AS sp WITH (NOLOCK) INNER JOIN
                         KlgSolicitudProductoEnvio AS spe WITH (NOLOCK) ON sp.sprId = spe.sprId INNER JOIN
                         KlgEnvio AS e WITH (NOLOCK) ON spe.envId = e.envId INNER JOIN
                         Agente AS age_sol WITH (NOLOCK) ON age_sol.age_id = sp.ageIdSolicitante INNER JOIN
                         Usuario AS user_sol WITH (NOLOCK) ON age_sol.age_id = user_sol.age_id INNER JOIN
                         Acceso AS ac_sol WITH (NOLOCK) ON user_sol.usr_id = ac_sol.usr_id INNER JOIN
                         Agente AS age_des ON sp.ageIdDestinatario = age_des.age_id INNER JOIN
                         KlgSolicitudTipo AS Sol_type ON sp.sltId = Sol_type.sltId
WHERE        (e.envNumeroRemito = @envNumeroRemito) AND (ac_sol.acc_login = @login)
ORDER BY sp.sprId DESC
";

                        mySqlCommand.Parameters.AddWithValue("@envNumeroRemito", request.ParameterValue);
                        mySqlCommand.Parameters.AddWithValue("@login", request.AuthenticationData.Username);

                        using (var reader = mySqlCommand.ExecuteReader())
                        {

                            if (reader.HasRows && reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var ColumnName = reader.GetName(i);
                                    var name = ColumnName;
                                    if (_dictionary.Keys.Contains(name))
                                        name = name + i;

                                    var _value = reader[ColumnName];

                                    _dictionary.Add(name, _value);
                                }

                            }

                        }

                    }

                }
            }
            catch (Exception)
            {

                throw;

            }

            return _dictionary;

        }

        public static bool LoginDispobible(string login, Decimal age_id = -1)
        {
            bool result = false;
            string _m = null;
            try
            {

                string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;

                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {

                    mySqlConnection.Open();
                    using (SqlCommand mySqlCommand = mySqlConnection.CreateCommand())
                    {
                        mySqlCommand.Connection = mySqlConnection;

                        int acc_login = 0;
                        try
                        {

                            if (age_id != -1)
                            {
                                mySqlCommand.CommandText = "SELECT Count(*) FROM Acceso  WITH(NOLOCK) WHERE acc_login=@loginname and usr_id <> (SELECT  [usr_id] FROM [Agente]  WITH(NOLOCK) where [age_id] = @age_id) ";
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@loginname", login);
                                mySqlCommand.Parameters.AddWithValue("@age_id", age_id);
                                acc_login = (int)mySqlCommand.ExecuteScalar();
                            }
                            else
                            {
                                mySqlCommand.CommandText = "SELECT Count(*) FROM Acceso  WITH(NOLOCK) WHERE acc_login=@loginname ";
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@loginname", login);
                                acc_login = (int)mySqlCommand.ExecuteScalar();
                            }

                            result = acc_login == 0;
                        }
                        catch (Exception ex)
                        {
                            _m = String.Concat("[QRY] ERROR SELECCIONANDO TIPOS DE ACCESO. Exception:", ex.Message, ". ", ex.StackTrace);
                            logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                            throw ex;
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                _m = String.Concat("[QRY] ERROR INICIALIZANDO CONEXION LoginDispobible. Exception: ", ex.Message, ". ", ex.StackTrace);
                logger.ErrorLow(_m);
                throw;
            }
            return result;
        }


        //Configuracion de comisiones
        public static ResponseAgent ConfigCommissions(RequestAgent oRequest, decimal new_age_id, decimal age_id_sup, decimal grpId)
        {
            logger.InfoLow(() => TagValue.New().Tag("[OP]").Value("ConfigCommissions entrando a configurar comisiones"));
            decimal entId = 0;
            decimal entIdContenedora = 0;
            string sentencia = string.Empty;
            ResponseAgent oResp = new ResponseAgent();
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString.Replace("TRAN_", "COMI_");
            SqlConnection mySqlConnection = null;//new SqlConnection(strConnString);
            SqlCommand mySqlCommand = new SqlCommand();
            SqlTransaction tran = null;
            string _m = "";
            oResp.result = false;
            //variable auxiliar que indica si el metodo debe hacer commit a la base de datos

            try
            {


                mySqlConnection = new SqlConnection(strConnString);
                mySqlConnection.Open();
                tran = mySqlConnection.BeginTransaction();
                mySqlCommand = new SqlCommand();
                mySqlCommand.Transaction = tran;
                mySqlCommand.Connection = mySqlConnection;
                mySqlCommand.CommandTimeout = 120;

                oResp.response_code = "01";//"80";
                try
                {
                    mySqlCommand.CommandText = "UPDATE secuencia SET sec_number=sec_number+1 WHERE sec_objectName = @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "KMMENTIDAD");
                    if (mySqlCommand.ExecuteNonQuery() == 0)
                    {
                        oResp.message = "NO SE PUDO ACTUALIZAR LA SECUENCIA DEL KMMENTIDAD";
                        _m = String.Concat("[API] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message);
                        logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                        tran.Rollback();
                        return oResp;
                    }

                }
                catch (Exception ex)
                {

                    oResp.message = "ERROR AL ACTUALIZAR LA SECUENCIA DEL KMMENTIDAD";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    mySqlCommand.CommandText = "SELECT sec_number FROM secuencia WITH(NOLOCK) WHERE sec_objectName= @paramValue";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@paramValue", "KMMENTIDAD");
                    entId = Convert.ToDecimal(mySqlCommand.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    oResp.response_code = "02";//"81";
                    oResp.message = "ERROR AL SELECCIONAR LA SECUENCIA DEL KMMENTIDAD";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    mySqlCommand.CommandText = "SELECT entId FROM KmmAgente WITH(NOLOCK) WHERE ageId = @ageId";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ageId", age_id_sup);
                    entIdContenedora = (decimal)mySqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    oResp.response_code = "03";//"82";
                    oResp.message = "ERROR AL SELECCIONAR LA ENTIDAD CONTENEDORA DE LA AGENCIA";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    /*sentencia = "INSERT INTO [dbo].KmmEntidad (entId,etyId,entIdContenedora,entDenominacion,entEstado)" +
                                    "VALUES ( " + entId + ",'A'," + entIdContenedora + "," + "'" + oRequest.age_nombre + "'" + "," + "'AC')";*/
                    mySqlCommand.CommandText = "INSERT INTO KmmEntidad (entId,etyId,entIdContenedora,entDenominacion,entEstado) VALUES (@entId,@etyId,@entIdContainer,@ageName,@entEstado)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@entId", entId);
                    mySqlCommand.Parameters.AddWithValue("@etyId", "A");
                    mySqlCommand.Parameters.AddWithValue("@entIdContainer", entIdContenedora);
                    mySqlCommand.Parameters.AddWithValue("@ageName", oRequest.age_nombre);
                    mySqlCommand.Parameters.AddWithValue("@entEstado", "AC");
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    oResp.response_code = "04";
                    oResp.message = "ERROR AL INSERTAR EN KMM_ENTIDAD";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    /* sentencia = "INSERT INTO [dbo].KmmAgente (ageId,agePermisoUso,agePermisoDelegacion,entId) " +
                                    "VALUES ( " + new_age_id + ",'N','N'," + entId + ")";*/
                    mySqlCommand.CommandText = "INSERT INTO KmmAgente (ageId,agePermisoUso,agePermisoDelegacion,entId) VALUES (@ageId,@agePermisoUso,@agePermisoDelegacion,@entId)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                    mySqlCommand.Parameters.AddWithValue("@agePermisoUso", "N");
                    mySqlCommand.Parameters.AddWithValue("@agePermisoDelegacion", "N");
                    mySqlCommand.Parameters.AddWithValue("@entId", entId);
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    oResp.response_code = "05";
                    oResp.message = "ERROR AL INSERTAR EN KMM_AGENTE";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    /*sentencia = "INSERT INTO [dbo].KmmEntidadExtension (entId,eneRazonSocial,eneNombre,eneApellido,eneDocumento,eneCalle,eneNro,enePiso,eneDpto,exeCodigoPostal,ciuId,eneSexo,eneNacimiento,eneTelefono,eneCelular,eneEmail,eneEstadoCivil,eneHijos,enePuntosDisponibles,eneAgrupaPuntos,eneCodigo)" +
                                    "VALUES ( " + entId + "," + "'" + oRequest.age_razonsocial + "'" + "," + "'" + oRequest.age_nombre + "'" + ",NULL," + "'" + oRequest.age_cuit + "'" + "," + "'" + oRequest.age_direccion + "'" + ",NULL,NULL,NULL,NULL," + oRequest.age_ciu_id + ",NULL,NULL," + "'" + oRequest.age_tel + "'" + "," + "'" + oRequest.age_cel + "'" + "," + "'" + oRequest.age_email + "'" + "," + "NULL,NULL,0,'N'," + "'" + "A" + new_age_id.ToString() + "'" + ")";*/
                    mySqlCommand.CommandText = "INSERT INTO KmmEntidadExtension (entId,eneRazonSocial,eneNombre,eneDocumento,eneCalle,ciuId,eneTelefono,eneCelular,eneEmail,enePuntosDisponibles,eneAgrupaPuntos,eneCodigo) VALUES  (@entId,@ageLegalName,@ageName,@ageCuit,@ageAddress,@ageCityId,@agePhone,@ageMobile,@ageEmail,@enePuntosDisponibles,@eneAgrupaPuntos,@eneCodigo)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@entId", entId);
                    mySqlCommand.Parameters.AddWithValue("@ageLegalName", oRequest.age_razonsocial);
                    mySqlCommand.Parameters.AddWithValue("@ageName", oRequest.age_nombre);
                    mySqlCommand.Parameters.AddWithValue("@ageCuit", oRequest.age_cuit);
                    mySqlCommand.Parameters.AddWithValue("@ageAddress", oRequest.age_direccion);
                    mySqlCommand.Parameters.AddWithValue("@ageCityId", oRequest.age_ciu_id);
                    mySqlCommand.Parameters.AddWithValue("@agePhone", oRequest.age_tel);
                    mySqlCommand.Parameters.AddWithValue("@ageMobile", oRequest.age_cel);
                    mySqlCommand.Parameters.AddWithValue("@ageEmail", oRequest.age_email);
                    mySqlCommand.Parameters.AddWithValue("@enePuntosDisponibles", 0);
                    mySqlCommand.Parameters.AddWithValue("@eneAgrupaPuntos", "N");
                    mySqlCommand.Parameters.AddWithValue("@eneCodigo", String.Concat("A", new_age_id));
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    oResp.response_code = "06";
                    oResp.message = "ERROR AL CREAR EN KMMENTIDADEXTENSION";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return oResp;
                }

                // Configuramos las comisiones de la agencia
                try
                {
                    // Validamos el modelo de comisiones que debemos insertar 
                    List<RequestAgent.Product> allProductsAvailable = oRequest.productos;
                    if (oRequest.productos.Count == 1 && oRequest.productos[0] != null && oRequest.productos[0].prdId == 0)// Cuando la comisión es una sola para la agencia
                    {
                        IEnumerable<RequestAgent.Product> partialEnumeration = GetProductsByZeroAvaibleInStock(age_id_sup);
                        allProductsAvailable = partialEnumeration.ToList<RequestAgent.Product>();
                        allProductsAvailable.ToList().ForEach(s => s.comision = oRequest.productos[0].comision);
                    }

                    List<RequestAgent.Product> listProductCommi = GetProductsFromCommi(allProductsAvailable);

                    var agentdata = new { age_id_sup = age_id_sup };
                    //List<RequestAgent.Product> listproducts = GetComissionSalesProducts(agentdata);
                    int index = 0;

                    if (ApiConfiguration.VALIDATE_COMISSION_BY_SALES)
                    {
                        List< RequestAgent.Product> listproducts = GetComissionSalesProducts(entIdContenedora);
                        foreach (var item in listProductCommi)
                        {
                            try
                            {
                                var itemparent = listproducts.Find(p => p.prdId == item.prdId);
                                bool save = itemparent == null || itemparent.comision == 0 || item.comision <= itemparent.comision;
                                if (save)
                                {
                                    mySqlCommand.CommandText = "INSERT INTO KmmVariableEntidadProductoAgen (varId,entId,ageId,prdId,mdlId,vepValor)  VALUES (@varId,@entId,@ageId,@prdId,@modId,@varValue)";
                                    mySqlCommand.Parameters.Clear();
                                    mySqlCommand.Parameters.AddWithValue("@varId", "commissionPercentage");
                                    mySqlCommand.Parameters.AddWithValue("@entId", entId);
                                    mySqlCommand.Parameters.AddWithValue("@ageId", age_id_sup);
                                    mySqlCommand.Parameters.AddWithValue("@prdId", item.prdId);
                                    mySqlCommand.Parameters.AddWithValue("@modId", "Kmm.Models.Commissions.PercentageOfSalesModel");
                                    mySqlCommand.Parameters.AddWithValue("@varValue", item.comision);
                                    mySqlCommand.ExecuteNonQuery();

                                }
                                else
                                {
                                    decimal comision = itemparent == null ? 0m : itemparent.comision;
                                    string nombre = GetNameProduct(item.prdId);
                                    _m = String.Concat("LA COMISION PARA EL PRODUCTO ", nombre, " NO PUEDE SER MAYOR A ");
                                    logger.ErrorLow(() => TagValue.New().Message(_m).Message("[INPUT]").Value(GetParameters(mySqlCommand)));
                                    tran.Rollback();
                                    oResp.response_code = "09";
                                    oResp.message = _m;
                                    return oResp;
                                }
                            }
                            catch (Exception ex)
                            {
                                oResp.response_code = "09";
                                oResp.message = "ERROR AL CREAR KMMVARIABLEENTIDADAGENTE";
                                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". [", ex.GetType().FullName, "] ", ex.Message, ". ", ex.StackTrace));
                                tran.Rollback();
                                return oResp;
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in listProductCommi)
                        {
                            try
                            {
                                mySqlCommand.CommandText = "INSERT INTO KmmVariableEntidadProductoAgen (varId,entId,ageId,prdId,mdlId,vepValor)  VALUES (@varId,@entId,@ageId,@prdId,@modId,@varValue)";
                                mySqlCommand.Parameters.Clear();
                                mySqlCommand.Parameters.AddWithValue("@varId", "commissionPercentage");
                                mySqlCommand.Parameters.AddWithValue("@entId", entId);
                                mySqlCommand.Parameters.AddWithValue("@ageId", age_id_sup);
                                mySqlCommand.Parameters.AddWithValue("@prdId", item.prdId);
                                mySqlCommand.Parameters.AddWithValue("@modId", "Kmm.Models.Commissions.PercentageOfSalesModel");
                                mySqlCommand.Parameters.AddWithValue("@varValue", item.comision);
                                mySqlCommand.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                oResp.response_code = "09";
                                oResp.message = "ERROR AL CREAR KMMVARIABLEENTIDADAGENTE";
                                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". [", ex.GetType().FullName, "] ", ex.Message, ". ", ex.StackTrace));
                                tran.Rollback();
                                return oResp;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    oResp.response_code = "10";
                    oResp.message = "ERROR AL CONFIGURAR COMISIONES";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    /*   sentencia = "INSERT INTO dbo.KmmModeloAgente (mdlId,ageId,magEstado)" +
                                    "VALUES ('Kmm.Models.Commissions.PercentageOfSalesModel'," + new_age_id + ",'AC'" + ")";*/
                    mySqlCommand.CommandText = "INSERT INTO KmmModeloAgente (mdlId,ageId,magEstado) VALUES (@modId,@ageId,@status)";
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@modId", "Kmm.Models.Commissions.PercentageOfSalesModel");
                    mySqlCommand.Parameters.AddWithValue("@ageId", new_age_id);
                    mySqlCommand.Parameters.AddWithValue("@status", "AC");
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    oResp.response_code = "11";
                    oResp.message = "ERROR AL CREAR KMMMODELOAGENTE";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return oResp;
                }

                try
                {
                    // si tiene grupo se inserta de lo contrario no lo inserta
                    if (grpId > 0)
                    {
                        /*sentencia = " INSERT INTO [dbo].KmmGrupoEntidad (grpId,entId)" +
                                        "VALUES (" + grpId.ToString() + "," + entId + ")";*/
                        // como se esta creando solo se realiza inser
                        mySqlCommand.CommandText = "IF NOT EXISTS  (SELECT * FROM KmmGrupoEntidad WITH(NOLOCK) WHERE grpId = @grpId AND entId = @entId)  INSERT INTO KmmGrupoEntidad (grpId,entId) VALUES (@grpId,@entId)";
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@grpId", grpId);
                        mySqlCommand.Parameters.AddWithValue("@entId", entId);
                        mySqlCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    oResp.response_code = "12";
                    oResp.message = "ERROR  AL CREAR KMMGRUPOENTIDAD";
                    logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                    tran.Rollback();
                    return oResp;
                }

                tran.Commit();
                oResp.result = true;
                return oResp;
            }
            catch (Exception ex)
            {
                oResp.response_code = "13";
                oResp.message = "NO SE PUDO ABRIR LA BASE DE DATOS DE COMISIONES";
                oResp.result = false;
                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", oResp.response_code, "-", oResp.message, ". Exception: ", ex.Message, ". ", ex.StackTrace));
                return oResp;
            }
            finally { mySqlConnection.Close(); }
        }

        public static SaleData GetSaleStateByTargent(string agentReference, string target, out string message)
        {
            string agentInfo = "";
            int agentId = 0, ventaId = 0;
            SaleData result = new SaleData();
            message = "Transacción especificada no existe";

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                sqlConnection.Open();

                var query =
                    String.Format(
                        (@"  SELECT cast(a.[age_id] as int)'BranchID'
                              FROM [dbo].[Agente] a with (NOLOCK)
                              join [dbo].[Usuario] u with (NOLOCK) on a.age_id=u.age_id
                              join [dbo].[Acceso] ac with (NOLOCK) on ac.usr_id=u.usr_id
                              where ac.acc_login='{0}' "), agentReference);
                var command = new SqlCommand(query, sqlConnection);
                agentId = int.Parse(command.ExecuteScalar().ToString());


                query =
                    String.Format(
                        (@"  SELECT top 1 cast(vta_id as int)
                                FROM Venta with (NOLOCK)
                                WHERE vta_destino='{0}' and age_id={1}
                                ORDER BY vta_fecha DESC "), target, agentId);
                command = new SqlCommand(query, sqlConnection);
                ventaId = int.Parse(command.ExecuteScalar().ToString());


                query =
                    String.Format(
                        (@"  SELECT rec_id,vta_id,cast(rec_monto*100 as int),rec_estado,rec_ingreso
                                FROM Recarga with (NOLOCK)
                                WHERE vta_id={0} "), ventaId);
                command = new SqlCommand(query, sqlConnection);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                        result = new SaleData()
                        {
                            IdTransaccion = reader.GetDecimal(1), // OJO FALTA SABER SI ES VTA_ID O REC_ID
                            Customer = target,
                            Amount = reader.GetInt32(2),
                            Date = reader.GetDateTime(4),
                            ReloadState = (reader.GetString(3) == "OK" ? "Realizada" : "Error"),
                            ReloadStateCode = reader.GetString(3)
                        };
                    reader.Close();

                    query =
                        String.Format(
                            (@"  SELECT top 1 p.prd_nombre,a.age_nombre,cast(v.vta_id as varchar),v.vta_fecha,v.vta_destino,v.vta_monto,r.rec_transaccionOperadora,v.vta_pdvRepresentado
                                    FROM Venta v with (NOLOCK)
                                    join Producto p with (NOLOCK) on p.prd_id=v.prd_id
                                    join Agente a with (NOLOCK) on a.age_id=v.age_id
                                    join Recarga r with (NOLOCK) on v.vta_id=r.vta_id
                                    WHERE vta_destino = '{0}' and a.age_id={1}
                                    ORDER BY vta_fecha DESC "), target, agentId);
                    command = new SqlCommand(query, sqlConnection);
                    reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        if (reader.Read())//5/26/2014 5:34:38 PM
                            message = String.Format("ProductoNombre: {0};AgenciaNombre: {1};VentaId: {2};Fecha: {3};Destino: {4};Monto: {5};TransaccionOperadora: {6};Pdv: {7};PdvRepresented: {7}",
                                        reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3).ToString("M/dd/yyyy h:mm:ss tt"), reader.GetString(4), reader.GetDecimal(5), reader.GetString(6), reader.GetString(7));
                    }
                    else
                        message = "Error construyendo el message";

                }
                else
                {
                    message = "Error buscando la venta";
                    result = new SaleData()
                    {
                        IdTransaccion = 0,
                        Customer = "",
                        Amount = 0,
                        Date = DateTime.Parse("1900-01-01"),
                        ReloadState = "",
                        ReloadStateCode = ""
                    };
                }

            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar la información de un agente en la base de datos de Kinacu"));
                result = new SaleData()
                {
                    IdTransaccion = 0,
                    Customer = "",
                    Amount = 0,
                    Date = DateTime.Parse("1900-01-01"),
                    ReloadState = "",
                    ReloadStateCode = ""
                };
            }
            finally
            {
                sqlConnection.Close();
            }

            return (result);
        }

        public static List<BasicAgentInfo> GetChildsById(int ageIdSup, bool extendedValues)
        {
            var agentExtended = new List<BasicAgentInfo>();

            try
            {

                var query = "";

                if (extendedValues)
                {
                    query = "SELECT  a.age_id, a.age_nombre, a.age_email, p.pro_nombre, c.ciu_nombre, ISNULL(s.stkCantidad,0) stkCantidad , a.age_estado,(SELECT COUNT(1) FROM [Agente] WITH (NOLOCK) WHERE age_id_sup=a.age_id) childs FROM [Agente] a WITH (NOLOCK) JOIN [Ciudad] c WITH (NOLOCK) on a.ciu_id=c.ciu_id JOIN [Provincia] p WITH (NOLOCK) on p.pro_id=c.pro_id LEFT JOIN [KlgStock] s WITH (NOLOCK) on s.ageIdpropietario=a.age_id WHERE a.age_id_sup  = @age_id_sup order by a.age_nombre asc";
                }
                else
                {
                    query = "SELECT  a.age_id, a.age_nombre FROM [Agente] a WITH (NOLOCK) WHERE a.age_id_sup  = @age_id_sup order by a.age_nombre asc";
                }



                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString))
                {

                    sqlConnection.Open();

                    var command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@age_id_sup", ageIdSup);
                    using (var reader = command.ExecuteReader())
                    {


                        if (reader.HasRows)
                            while (reader.Read())
                            {
                                var ag = new BasicAgentInfo();
                                ag.Agent = reader["age_id"].ToString();
                                ag.Name = reader["age_nombre"].ToString();
                                if (extendedValues)
                                {
                                    ag.Email = reader["age_email"].ToString();
                                    ag.Department = (string)reader["pro_nombre"];
                                    ag.City = (string)reader["ciu_nombre"];
                                    ag.CurrentBalance = (decimal)reader["stkCantidad"];
                                    ag.Status = (string)reader["age_estado"];
                                    ag.ChildsCount = (int)reader["childs"];
                                }
                                agentExtended.Add(ag);
                            }

                    }

                }

            }
            catch (Exception ex)
            {
                string _m = "Error trantando de consultar los datos de los agentes hijos en la base de datos de Kinacu";
                logger.ErrorLow(() => TagValue.New().Exception(ex).Message(_m));
                throw new Exception(_m, ex);
            }
            return agentExtended;
        }
        /// <summary>
        /// Valida si el agente logeado tiene relacion directa con el agente a editar
        /// </summary>
        /// <param name="data">
        /// objeto dynamic que cumple con la siguiente estructura:
        /// AgeId (id del agente a editar)
        /// Login (Login del usuario logeado al api)
        /// </param>
        /// <returns>True si el agente logeado tiene relacion directa con el agente a editar</returns>
        public static bool HaveRelationWithAgent(dynamic data)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(data.Login))
            {

                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString))
                {
                    // id por login
                    decimal age_id = 0m;
                    //  agente superior por id 
                    decimal age_sup = 0m;
                    var command = new SqlCommand();

                    //Buqueda del age_id por login, se incluye el campo FIELD para diferenciar el resultado de la consulta
                    command.CommandText = "SELECT  Agente.age_id age_id, 'LOGIN'  FIELD FROM            Agente WITH(NOLOCK) INNER JOIN    Usuario WITH(NOLOCK) ON Agente.age_id = Usuario.age_id INNER JOIN    Acceso WITH(NOLOCK) ON Usuario.usr_id = Acceso.usr_id WHERE        (Acceso.acc_login = @acc_login) ";
                    command.Parameters.AddWithValue("@acc_login", data.Login);

                    //si el dato del agente a editar esta mapeado
                    if (data.AgeId != null && data.AgeId > 0m)
                    {
                        //se busca el agente padre del agente a editar, se realiza la union con la busqueda anterior 
                        // diferenciandolas por el campo FIELD
                        command.CommandText = string.Concat(command.CommandText, " UNION ALL SELECT [age_id_sup] age_id, 'ID_SUP' FIELD  FROM [dbo].[Agente] WITH(NOLOCK) where  [age_id] = @age_id");
                        command.Parameters.AddWithValue("@age_id", data.AgeId);
                    }
                    command.Connection = sqlConnection;
                    sqlConnection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                            {
                                switch (reader["FIELD"].ToString())
                                {
                                    case "LOGIN":
                                        age_id = (decimal)reader["age_id"];
                                        break;
                                    case "ID_SUP":
                                        age_sup = (decimal)reader["age_id"];
                                        break;

                                }
                            }
                    }

                    result =   //es el mismo agente a editar
                        (age_id == data.AgeId) ||
                        // es el agente padre
                        (age_id == age_sup);

                }
            }

            return result;
        }


        public static int GetAgentIdByAccessPosWeb(string login)
        {
            int ageP = 0;
            try
            {


                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString))
                {
                    sqlConnection.Open();
                    string cmd = @"SELECT        Agente.age_id
                            FROM            Agente WITH(NOLOCK) INNER JOIN
                         Usuario WITH(NOLOCK) ON Agente.age_id = Usuario.age_id INNER JOIN
                         Acceso WITH(NOLOCK) ON Usuario.usr_id = Acceso.usr_id
                        WHERE      Acceso.acc_login = @login and tac_id = @tac_id";

                    var command = new SqlCommand(cmd, sqlConnection);
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@tac_id", cons.ACCESS_POSWEB);
                    object result = command.ExecuteScalar();

                    if (result != null)
                        ageP = Decimal.ToInt32(Convert.ToDecimal(result));
                    else
                        throw new Exception(string.Concat("NO SE INFORMACION ACCESO POS WEB PARA ", login));

                }

                return ageP;
            }
            catch (Exception ex)
            {

                throw new Exception("ERROR CONSULTADNO AGENTE " + login + " POR ACCESO POSWEB", ex);
            }
        }

        public static int GetAgentIdByAcces(string login, int access)
        {
            int ageP = 0;
            try
            {


                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString))
                {
                    sqlConnection.Open();
                    string cmd = @"SELECT        Agente.age_id
                            FROM            Agente WITH(NOLOCK) INNER JOIN
                         Usuario WITH(NOLOCK) ON Agente.age_id = Usuario.age_id INNER JOIN
                         Acceso WITH(NOLOCK) ON Usuario.usr_id = Acceso.usr_id
                        WHERE      Acceso.acc_login = @login and tac_id = @tac_id";

                    var command = new SqlCommand(cmd, sqlConnection);
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@tac_id", access);
                    object result = command.ExecuteScalar();

                    if (result != null)
                        ageP = Decimal.ToInt32(Convert.ToDecimal(result));
                    else
                        throw new Exception(string.Concat("NO SE INFORMACION ACCESO POS WEB PARA ", login));

                }

                return ageP;
            }
            catch (Exception ex)
            {

                throw new Exception("ERROR CONSULTADNO AGENTE " + login + " POR ACCESO POSWEB", ex);
            }
        }



        public static int GetAgentIdParent(decimal agente)
        {
            int ageP = 0;
            try
            {


                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString))
                {
                    sqlConnection.Open();
                    string cmd = @"SELECT   Agente.age_id_sup
                            FROM    Agente WITH(NOLOCK)
                        WHERE      Agente.age_id = @agente";

                    var command = new SqlCommand(cmd, sqlConnection);
                    command.Parameters.AddWithValue("@agente", agente);

                    object result = command.ExecuteScalar();

                    if (result != null)
                        ageP = Decimal.ToInt32(Convert.ToDecimal(result));
                    else
                        throw new Exception(string.Concat("NO SE INFORMACION ACCESO POS WEB EL ID AGENTE ", agente));

                }

                return ageP;
            }
            catch (Exception ex)
            {

                throw new Exception(String.Concat("ERROR CONSULTADNO AGENTE  POR ID ", agente), ex);
            }
        }

        /// <summary>
        /// Retorna el codigo de acceso segun el login del usuario
        /// </summary>
        /// <param name="login">login del usuario registrado en el sistema</param>
        /// <returns></returns>
        public static int GetAccessCodeByLogin(string login)
        {
            int ageP = 0;
            try
            {


                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString))
                {
                    sqlConnection.Open();
                    string cmd = @"SELECT  Acceso.tac_id 
                                    FROM   Acceso WITH(NOLOCK)
                                    WHERE  Acceso.acc_login = @login";

                    var command = new SqlCommand(cmd, sqlConnection);
                    command.Parameters.AddWithValue("@login", login);

                    object result = command.ExecuteScalar();

                    if (result != null)
                        ageP = Decimal.ToInt32(Convert.ToDecimal(result));
                }

                return ageP;
            }
            catch (Exception ex)
            {

                throw new Exception("ERROR CONSULTADNO AGENTE " + login + " POR ACCESO POSWEB", ex);
            }
        }

        /// <summary>
        /// Retorna el status de un depósito
        /// </summary>
        /// <param name="login">login del usuario registrado en el sistema</param>
        /// <returns></returns>
        public static string GetDepositStatus(string date, decimal amount, string reference, string bankName)
        {
            string status = "UN";
            try
            {


                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString))
                {
                    sqlConnection.Open();
                    string cmd = @"SELECT CASE WHEN sum(CASE d.depestado WHEN 'AU' THEN 1 ELSE 0 END) > 0 THEN 'AU' 
			                                WHEN sum(CASE d.depestado WHEN 'PE' THEN 1 ELSE 0 END) > 0 THEN 'PE' 
			                                WHEN sum(CASE d.depestado WHEN 'RE' THEN 1 ELSE 0 END) > 0 THEN 'RE' 
			                                ELSE 'UN' END
                                    FROM [KfnDeposito] d with (NOLOCK)
                                    JOIN [KfnCuentaBanco] cb with (NOLOCK) on d.cubid=cb.cubid
                                    JOIN [KfnBanco] b with (NOLOCK) on b.banid=cb.banid
                                    where cast(d.depfechacomprobante as date) = @date and d.depmonto = @amount and d.depcomprobante=@reference and b.bannombre=@bankname ";

                    var command = new SqlCommand(cmd, sqlConnection);
                    command.Parameters.AddWithValue("@date", date);
                    command.Parameters.AddWithValue("@amount", amount);
                    command.Parameters.AddWithValue("@reference", reference);
                    command.Parameters.AddWithValue("@bankname", bankName);

                    object result = command.ExecuteScalar();

                    if (result != null)
                        status = result.ToString();
                }

                return status;
            }
            catch (Exception ex)
            {
                throw new Exception("ERROR CONSULTANDO FECHA " + date + ", MONTO " + amount + ", REFERENCE " + reference + ", BANKNAME " + bankName, ex);
            }
        }

        public static bool IsChildAgent(string login, int ageCh)
        {

            int ageP = -1;
            try
            {
                ageP = GetAgentIdByAccessPosWeb(login);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Concat("ERROR VALIDANDO JERARQUÍA ENTRE AGENTES ", login, " ",
                   ageCh), ex);
            }
            return IsChildAgent(ageP, ageCh);
        }

        public static bool IsChildAgent(int ageP, int ageCh)
        {
            bool result = false;
            /*
            @"  declare @ageP as int
            set @ageP =  698

            declare @ageCh as int
            SET @ageCh =-14611*/

            try
            {
                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString))
                {
                    string cmd = @" declare @bandera as int
                    set @bandera = 0

                    declare @result as int
                    set @result = 0

                    declare @index as int
                    set @index  = 0;

                    WHILE @bandera =0  BEGIN

	                    SELECT @index =  [age_id_sup]
	                      FROM [dbo].[Agente] WITH(NOLOCK)
	                    WHERE [age_id] = @ageCh

	                    if not @index is null and  @index = @ageP begin 
		                    set @bandera =1
		                    set @result = 1
	                    end
	                    else if @index is null or @index = 0 begin 
		                    set @bandera =1
	                    end

		                    set @ageCh = @index
                    END

                    SELECT @result as 'ISCHILD'";
                    sqlConnection.Open();
                    var command = new SqlCommand(cmd, sqlConnection);
                    command.Parameters.AddWithValue("@ageP", ageP);
                    command.Parameters.AddWithValue("@ageCh", ageCh);

                    result = ((int)command.ExecuteScalar()) == 1;

                }
            }
            catch (Exception ex)
            {
                throw new Exception("ERROR VALIDANDO JERARQUÍA ENTRE AGENTES " + ageP + " " + ageCh, ex);
            }

            return result;
        }

        public static GenericApiResult<bool> ValidateSoldChildComissions(string loginp, decimal comision)
        {
            int ageP = -1;
            try
            {
                ageP = GetAgentIdByAccessPosWeb(loginp);
            }
            catch (Exception ex)
            {
                throw;
            }
            return ValidateSoldChildComissions(ageP, comision);



        }

        /// <summary>
        /// Validar el padre 
        /// </summary>
        /// <param name="agenteId">agente que se esta modificando</param>
        /// <param name="comision">comision </param>
        /// <returns></returns>
        public static GenericApiResult<bool> ValidateSoldChildComissions(decimal agenteId, decimal comision)
        {
            int ageP = -1;
            try
            {
                // si se esta editando asi mismo
                //if (agenteId == agentePadre)
                ageP = GetAgentIdParent(agenteId);
                //else
                //ageP = agentePadre;
            }
            catch (Exception ex)
            {
                throw;
            }
            return ValidateSoldChildComissions(ageP, comision);



        }

        public static GenericApiResult<bool> ValidateSoldChildComissions(int ageidp, decimal comision)
        {
            //TODO LA CONEXION NO ES CORRECTA
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;
            GenericApiResult<bool> result = new GenericApiResult<bool>();

            try
            {
                int ResponseCode = 1;
                if (comision < 0m)
                {
                    result.SetResultData(false, ResponseCode, string.Concat("LA COMISION NO PUEDE SER NEGATIVA "));
                }

                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {
                    mySqlConnection.Open();
                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();

                    //seleccionar desde la base de datos COM COL
                    // PUDE QUE TENAGA UNA COMISISON PARA TODOS 
                    //LOS PROPDUCTOS O PARA ESPECIFICA APRA ALGUNOS
                    mySqlCommand.CommandText = @"SELECT age_montocomision  FROM [dbo].[Agente] WITH(NOLOCK) where   [age_id] = @ageidp";
                    //commission PercentageKmm.Models.Commissions.PercentageOfSalesModel
                    //,[acc_password] ,[acc_lastlogin] ,[acc_intentos],[acc_cambiopassword] ,[acc_validityDate] ,[acc_terminalId]

                    mySqlCommand.Parameters.AddWithValue("@ageidp", ageidp);

                    object aux = mySqlCommand.ExecuteScalar();
                    decimal parentcomission = 0m;
                    if (aux != null && aux != DBNull.Value)
                        parentcomission = (decimal)aux;


                    if (comision > parentcomission)
                        result.SetResultData(false, ResponseCode, string.Concat("LA COMISION NO PUEDE SER MAYOR A ", parentcomission.ToString("0.##", CultureInfo.InvariantCulture)));
                    else
                        result.SetResultData(true);



                }
            }
            catch (Exception ex)
            {
                throw;

            }


            return result;


        }

        public static IEnumerable<RequestAgent.Product> GetListStockProducts(decimal ageid)
        {
            //TODO LA CONEXION NO ES CORRECTA
            string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;



            using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
            {
                mySqlConnection.Open();
                SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = "SELECT [prdId] FROM [dbo].[KlgAgenteProducto] WITH(NOLOCK) WHERE [ageId] = @ageId  and [agpEstado] = @agpEstado  order by [prdId]";
                mySqlCommand.Parameters.AddWithValue("@ageId", ageid);
                mySqlCommand.Parameters.AddWithValue("@agpEstado", "AC");

                using (var reader = mySqlCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                            yield return new RequestAgent.Product() { prdId = Decimal.ToInt32((decimal)reader["prdId"]) };
                }
            }

        }

        //TODO Secutiy
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static TrustedDevices TrustedDevices(FilterTrustedDevices filter = null)
        {

            try
            {
                //IMPLEMENTACION DUMMY
                //TODO QUITAR
                TrustedDevices devices = new DataContract.TrustedDevices();

                //lock (_devicesdummy)
                //{
                //    if (_devicesdummy.Count < 0)
                //    {
                //        int n = _random.Next(1, 3) + 1 + _devicesdummy.Count;
                //        for (int i = _devicesdummy.Count; i < n; i++, _numberDevices++)
                //        {

                //            _devicesdummy.Add(FactoryDummyMetodDevice(_numberDevices));
                //            Thread.Sleep(10);
                //        }
                //    }
                //    devices = new DataContract.TrustedDevices(_devicesdummy.ToList());
                //}


                //

                string strConnString = ConfigurationManager.ConnectionStrings["SECURE_DB"].ConnectionString;



                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {
                    mySqlConnection.Open();
                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                    mySqlCommand.CommandText = @"SELECT Device.DeviceId, Device.UserId, Device.Token, Device.Hash, Device.DeviceTypeId, Device.FriendlyName, Device.Description, Device.DateActivated, Device.Status, Device.LastAccess, Device.DateBlocked, 
                         Device.Model, Device.OS, DeviceType.Name AS Type
                            FROM            Device INNER JOIN
                         DeviceType ON Device.DeviceTypeId = DeviceType.DeviceTypeId

                                                        where  Device.Status <> @Status";


                    mySqlCommand.Parameters.AddWithValue("@Status", cons.DEVICE_DELETE);


                    using (var reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                //todo mismo codigo de 
                                TrustedDevice device = new TrustedDevice()
                                {
                                    ID = (long)((int)reader["DeviceId"]),//(long)(),
                                    UserId = (string)reader["UserId"],
                                    Token = (string)reader["Token"],
                                    Hash = (string)reader["Hash"],
                                    Type = (string)reader["Type"],
                                    IdType = (int)reader["DeviceTypeId"],
                                    FriendlyName = (string)reader["FriendlyName"],

                                    //TODO VERIFICAR FECHA ADDTRUSTEDDEVICE
                                    DateActivated = (DateTime)reader["DateActivated"],  // RegisterDate = (DateTime)reader["DateActivated"],
                                    Status = ((Int16)reader["Status"]),



                                };
                                // se validan aquellos que pueden ser DBNull en base de datos
                                if (!reader["DateBlocked"].Equals(DBNull.Value))
                                    device.BloquedDate = (DateTime)reader["DateBlocked"];

                                if (!reader["LastAccess"].Equals(DBNull.Value))
                                    device.LastAccess = (DateTime)reader["LastAccess"];

                                if (!reader["Description"].Equals(DBNull.Value))
                                    device.Description = (string)reader["Description"];

                                if (!reader["Model"].Equals(DBNull.Value))
                                    device.Model = (string)reader["Model"];

                                if (!reader["OS"].Equals(DBNull.Value))
                                    device.OS = (string)reader["OS"];


                                devices.Add(device);


                            }
                        }
                    }
                }





                return devices;
            }
            catch (Exception ex)
            {


                throw;//new Exception( message,ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static bool SetStatusDevice(TrustedDevice device)
        {
            bool result = false;
            string message = "";
            try
            {
                //IMPLEMENTACION DUMMY
                //lock (_devicesdummy)
                //{
                //    if (_devicesdummy.Remove(device))
                //    {
                //        result = true;
                //    }
                //    else
                //    {
                //        result = false;
                //        message = "NO SE PUDO BORRAR EL REGISTRO";
                //        logger.ErrorLow(() => TagValue.New().Message(String.Concat("[QRY] ", LOG_PREFIX, " ", "90", "-", message)).Tag(device.GetType().Name).Value(device));
                //    }
                //}
                //


                string strConnString = ConfigurationManager.ConnectionStrings["SECURE_DB"].ConnectionString;


                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {
                    //TODO FALTA FECHAS DE MODIFICACION

                    mySqlConnection.Open();
                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();


                    switch (device.Status)
                    {
                        case cons.DEVICE_DELETE:
                            mySqlCommand.CommandText = @"UPDATE [dbo].[Device]
                               SET [Status] = @Status,
                              [DateBlocked] = @Date
                             WHERE [DeviceId] = @DeviceId ";//AND [Status] <> @Status";

                            break;
                        case cons.DEVICE_IN_BLACK_LIST:
                            mySqlCommand.CommandText = @"UPDATE [dbo].[Device]
                               SET [Status] = @Status,
                              [DateBlocked] = @Date
                             WHERE [DeviceId] = @DeviceId ";// AND [Status] <> @Status";

                            break;

                        default:
                            mySqlCommand.CommandText = @"UPDATE [dbo].[Device]
                               SET [Status] = @Status,
                              [DateActivated] = @Date
                             WHERE [DeviceId] = @DeviceId ";// AND [Status] <> @Status";
                            break;
                    }
                    mySqlCommand.Parameters.AddWithValue("@DeviceId", device.ID);
                    mySqlCommand.Parameters.AddWithValue("@Status", device.Status);
                    mySqlCommand.Parameters.AddWithValue("@Date", DateTime.Now);
                    int rowsafected = mySqlCommand.ExecuteNonQuery();
                    result = (rowsafected == 1);
                }

            }
            catch (Exception ex)
            {

                message = "ERROR AL BORRAR DISPOSITIVO";

                throw;//new Exception(message, ex);

            }


            return result;
        }


        internal static GenericApiResult<TrustedDevice> GetDeviceId(long ID, bool extendenValues = false)
        {
            GenericApiResult<TrustedDevice> result = new GenericApiResult<TrustedDevice>();
            result.SetResultData(null, 1, "DISPOSITIVO NO EXISTE");
            try
            {

                string strConnString = ConfigurationManager.ConnectionStrings["SECURE_DB"].ConnectionString;
                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {


                    mySqlConnection.Open();
                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                    //select by serial
                    mySqlCommand.CommandText = @"
                         SELECT        Device.DeviceId, Device.UserId, Device.Token, Device.Hash, Device.DeviceTypeId, Device.FriendlyName, Device.Description, Device.DateActivated, Device.Status, Device.LastAccess, Device.DateBlocked, 
                         Device.Model, Device.OS, DeviceType.Name AS Type
                        FROM            Device INNER JOIN
                         DeviceType ON Device.DeviceTypeId = DeviceType.DeviceTypeId 
                        WHERE  Device.DeviceId = @ID";

                    mySqlCommand.Parameters.AddWithValue("@ID", ID);


                    using (var reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            TrustedDevice device = new TrustedDevice()
                            {
                                ID = (long)((int)reader["DeviceId"]),
                                UserId = (string)reader["UserId"],
                                Token = (string)reader["Token"],
                                IdType = (int)reader["DeviceTypeId"],
                                Status = (Int16)reader["Status"]

                            };

                            if (extendenValues)
                            {

                                device.Hash = (string)reader["Hash"];
                                device.Type = (string)reader["Type"];
                                device.FriendlyName = (string)reader["FriendlyName"];
                                device.Description = (string)reader["Description"];
                                //TODO VERIFICAR FECHA ADDTRUSTEDDEVICE
                                device.DateActivated = (DateTime)reader["DateActivated"];


                                // se validan aquellos que pueden ser DBNull en base de datos
                                if (!reader["DateBlocked"].Equals(DBNull.Value))
                                    device.BloquedDate = (DateTime)reader["DateBlocked"];

                                if (!reader["LastAccess"].Equals(DBNull.Value))
                                    device.LastAccess = (DateTime)reader["LastAccess"];

                                if (!reader["Description"].Equals(DBNull.Value))
                                    device.Description = (string)reader["Description"];

                                if (!reader["Model"].Equals(DBNull.Value))
                                    device.Model = (string)reader["Model"];

                                if (!reader["OS"].Equals(DBNull.Value))
                                    device.OS = (string)reader["OS"];

                            }
                            result.SetResultData(device);

                        }

                    }
                }

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        internal static GenericApiResult<TrustedDevice> GetDeviceByKey(string token, String userid, bool extendenValues = false)
        {
            GenericApiResult<TrustedDevice> result = new GenericApiResult<TrustedDevice>();
            result.SetResultData(null, 1, string.Concat("NO EXISTE EN LA LISTA DE DISPOSITIVOS CONFIABLES EL DISPOSITIVO SERIAL [", token, "] "));
            try
            {

                string strConnString = ConfigurationManager.ConnectionStrings["SECURE_DB"].ConnectionString;
                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {


                    mySqlConnection.Open();
                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                    //select by serial
                    mySqlCommand.CommandText = String.Concat(
                        @"SELECT Device.DeviceId, Device.UserId, Device.Token, Device.Hash, Device.DeviceTypeId, Device.FriendlyName, Device.Description, Device.DateActivated, 
                                Device.Status, Device.LastAccess, Device.DateBlocked, Device.Model, Device.OS, DeviceType.Name AS Type
                        FROM Device 
                        INNER JOIN DeviceType ON Device.DeviceTypeId = DeviceType.DeviceTypeId ",
                        token.Length.Equals(24) ? "WHERE Device.Hash = @SerialTokken " : "WHERE Device.Token = @SerialTokken ",
                        "AND Device.UserId = @userId AND  Device.Status = @Status");

                    mySqlCommand.Parameters.AddWithValue("@SerialTokken", token);
                    mySqlCommand.Parameters.AddWithValue("@userId", userid);
                    mySqlCommand.Parameters.AddWithValue("Status", cons.DEVICE_ACTIVE);

                    using (var reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            TrustedDevice device = new TrustedDevice()
                            {
                                ID = (long)((int)reader["DeviceId"]),
                                UserId = (string)reader["UserId"],
                                Token = (string)reader["Token"],
                                Hash = (string)reader["Hash"],
                                //--
                                IdType = (int)reader["DeviceTypeId"],
                                Type = (string)reader["Type"],
                                //--
                                FriendlyName = (string)reader["FriendlyName"],
                                DateActivated = (DateTime)reader["DateActivated"],
                                Status = (Int16)reader["Status"],
                            };

                            //if (extendenValues)
                            {


                                // se validan aquellos que pueden ser DBNull en base de datos
                                if (!reader["DateBlocked"].Equals(DBNull.Value))
                                    device.BloquedDate = (DateTime)reader["DateBlocked"];

                                if (!reader["LastAccess"].Equals(DBNull.Value))
                                    device.LastAccess = (DateTime)reader["LastAccess"];

                                if (!reader["Description"].Equals(DBNull.Value))
                                    device.Description = (string)reader["Description"];

                                if (!reader["Model"].Equals(DBNull.Value))
                                    device.Model = (string)reader["Model"];

                                if (!reader["OS"].Equals(DBNull.Value))
                                    device.OS = (string)reader["OS"];

                            }
                            result.SetResultData(device);

                        }

                    }
                }

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        /// <summary>
        /// determina si el dispositivo es valido
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static GenericApiResult<bool> IsActiveDevice(TrustedDevice device)
        {
            // bool result = false;
            GenericApiResult<bool> result = new GenericApiResult<bool>();
            //supongo que esta habilitado
            result.SetResultData(true);

            //codigo de error
            int ResponseCode = 1;

            string Descripcion = device.DeviceDescription();

            if (device.Status == cons.DEVICE_IN_BLACK_LIST)
            {

                result.SetResultData(false, ResponseCode, String.Concat("EL ", Descripcion, " ESTA EN LA LISTA NEGRA"));
                return result;
            }



            try
            {
                switch (device.Status)
                {
                    case cons.DEVICE_TEMPORAL:

                        TimeSpan ts = Utils.DEVICE_TEMP_TIME;

                        //leer tiempo de activacion temporal desde web config



                        bool aux = (DateTime.Now - device.DateActivated) >= ts;
                        if (aux)
                        {
                            ResponseCode = 3;
                            result.SetResultData(false, ResponseCode, String.Concat("SE EXPIRO EL TIEMPO DE ACCESO PARA EL ", Descripcion));

                            return result;
                        }
                        break;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static GetAccessList GetLatestAccess(FilterLatestAccess filter = null)
        {
            string message = "";
            try
            {


                GetAccessList list = new GetAccessList();

                //lock (_accessListdummy)
                //{
                //    if (_accessListdummy.Count < 100)
                //    {
                //        int n = _random.Next(1, 3) + 1 + _accessListdummy.Count;
                //        for (int i = _accessListdummy.Count; i < n; i++, _numberAccess++)
                //        {
                //            _accessListdummy.Add(FactoryDummyMetodAccess(_numberAccess));
                //            Thread.Sleep(10);
                //        }
                //    }
                //    list = new GetAccessList(_accessListdummy.OrderByDescending(p=> p.Date).ToList());
                //}



                //List<{TypeReturn}>  result = new    List<{TypeReturn}>();

                string strConnString = ConfigurationManager.ConnectionStrings["{DATABASE}"].ConnectionString;
                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {
                    mySqlConnection.Open();
                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                    mySqlCommand.CommandText = @"
                                                       SELECT [AccessLogId]
                                                          ,[DeviceId]
                                                          ,[DateTime]
                                                          ,[IP]
                                                          ,[Action]
                                                          ,[Amount]
                                                      FROM [dbo].[AccessLog]
                                                         ";


                    using (var reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                list.Add(new Access()
                                {
                                    ID = (long)((int)reader["ID"]),
                                    DeviceID = (long)((int)reader["DeviceId"]),
                                    DateTime = (DateTime)(reader["DateTime"]),
                                    IP = (string)reader["IP"],
                                    Action = (string)reader["Action"]
                                    //ActionValue = reader["Amount"]

                                });
                            }
                        }
                    }
                }



                return list;
            }
            catch (Exception ex)
            {
                message = "ERROR AL OBTENER LA LISTA DE DISPOSITIVOS";

                throw;//new Exception( message,ex);
            }
        }

        public static GenericApiResult<bool> AddTrustedDevice(TrustedDevice device)
        {
            GenericApiResult<bool> result = new GenericApiResult<bool>();
            //supongo que todo saldra bien
            result.SetResultData(true);
            //primer alias
            int ResponseCode = 1;
            try
            {

                //VALIDACION HASH

                bool isValidHash = true;
                //

                MD5 md5 = new MD5CryptoServiceProvider();

                string preHash = Movilway.API.Core.Security.Cryptography.encrypt(string.Concat(device.Token, "_", device.Ticks));
                var hashval = Convert.ToBase64String(md5.ComputeHash(Encoding.Unicode.GetBytes(preHash)));

                isValidHash = hashval == device.Hash;


                //validar el peor caso imediato
                if (!isValidHash)
                {
                    result.SetResultData(false, ResponseCode, "EL HASH DE LA PETICION ES INVALIDO");
                    //retornar el error
                    return result;

                }

                // no else code

                ResponseCode = 2;
                #region IMPLEMENTACION DUMMY
                //IMPLEMENTACION DUMMY
                //lock (_devicesdummy)
                //{
                //    if (_devicesdummy.Find(p => p.Token == device.Token) == null)
                //    {
                //        device.ID = _numberDevices;
                //        _numberDevices++;
                //        device.Type = DEVICE_DESCRIPTION[device.IdType];
                //        _devicesdummy.Add(device);
                //        result.SetResultData(true);
                //    }
                //    else
                //    {
                //        result.SetResultData(false, ResponseCode, "EL DISPOSITIVO YA EXISTE EN LA LISTA");
                //    }
                //}
                //
                #endregion
                string strConnString = ConfigurationManager.ConnectionStrings["SECURE_DB"].ConnectionString;
                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {


                    mySqlConnection.Open();

                    SqlTransaction tran = mySqlConnection.BeginTransaction();
                    try
                    {

                        SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                        mySqlCommand.Transaction = tran;

                        //DISPOSITIVOS DIFERENTES A TEMPORAL
                        //-LOS CREA SI NO EXISTEN
                        //DISPOSITIVOS TEMPORALES
                        //-LOS CREA SI NO EXISTEN
                        //-LOS CAMBIA DE ESTADO DELETE DEVICE A TEMPORAL
                        String VALUES_DECLARATION, OPERATION, RESULT;
                        VALUES_DECLARATION = @"
                                            -- OPERACION ES OK
                                            DECLARE @OP_RESULT AS INT = 0

                                            DECLARE @DEVICE_ID_DATA AS int 
                                            DECLARE @STATUS_DATA AS smallint 
                                            DECLARE @DATE_ACTIVATED_DATA AS datetime
                                         
                                           SELECT @DEVICE_ID_DATA = DeviceId, @STATUS_DATA = Status, @DATE_ACTIVATED_DATA = DateActivated FROM dbo.Device WITH(READUNCOMMITTED) WHERE Hash = @Hash AND UserId = @UserId

                                           ";//);
                        if (device.Status != cons.DEVICE_TEMPORAL)
                        {


                            OPERATION = @"   
                                       
                                                -- si no es un estado temporal
           
                                                 IF (@DEVICE_ID_DATA IS NULL) 
                                                    BEGIN 
                                                        INSERT INTO [dbo].[Device]
                                                       ([UserId]
                                                       ,[Token]
                                                       ,[Hash]
                                                       ,[DeviceTypeId]
                                                       ,[FriendlyName]
                                                       ,[Description]
                                                       ,[DateActivated]
                                                       ,[Status]
                                                       ,[Model]
                                                       ,[OS]
                                                       ,[Secure]
                                                       ,[DateCreated])
                                                         VALUES
                                                       (@UserId,
                                                        @Token,
       	                                                @Hash,
                                                        @DeviceTypeId,
                                                        @FriendlyName,
                                                        @Description,
                                                        @DateActivated,
                                                        @Status,
                                                        @Model,
                                                        @OS,
                                                        @Secure,
                                                        @DateCreated);
                                                         
                                                    END
                                                    ELSE IF (@STATUS_DATA  = @DELETED)
                                                    BEGIN
                                                       
                                                        UPDATE [dbo].[Device]
                                                        SET 
                                                           [DeviceTypeId] = @DeviceTypeId, 
                                                           [FriendlyName] = @FriendlyName, 
                                                           [Description] = @Description,  
                                                           [DateActivated] = @DateActivated,
                                                           [Status] = @Status,
                                                           [Model] = @Model,
                                                           [OS] = @OS,
                                                           [Secure] = @Secure
                                                        WHERE  Hash =  @Hash AND UserId =  @UserId
	                                                   
                                                    END
                                                    ELSE 
                                                        -- ASIGNAR ERROR
                                                         SET @OP_RESULT=  @MY_ERRORCODE
                                                    
                                                 
                                         
                                            ";
                        }
                        else
                        {
                            OPERATION = @"   
                                       
                                              
                                                --AGREGA DISPOSITIVO TEMPORAL
                                                -- IF (NOT EXISTS(SELECT DeviceId FROM dbo.Device WITH(READUNCOMMITTED) WHERE Hash = @Hash AND UserId = @UserId))
                                                    IF (@DEVICE_ID_DATA IS NULL) BEGIN 

                                                        INSERT INTO [dbo].[Device]
                                                       ([UserId]
                                                       ,[Token]
                                                       ,[Hash]
                                                       ,[DeviceTypeId]
                                                       ,[FriendlyName]
                                                       ,[Description]
                                                       ,[DateActivated]
                                                       ,[Status]
                                                       ,[Model]
                                                       ,[OS]
                                                       ,[Secure]
                                                       ,[DateCreated])
                                                        VALUES
                                                       (@UserId,
                                                        @Token,
       	                                                @Hash,
                                                        @DeviceTypeId,
                                                        @FriendlyName,
                                                        @Description,
                                                        @DateActivated,
                                                        @Status,
                                                        @Model,
                                                        @OS,
                                                        @Secure,
                                                        @DateCreated);
														--SELECT DE LA RESPUESTA
													
															END 
                                                    -- SI EXISTE UN DISPOSITIVO EN ESTADO BORRADO
                                                    -- EXISTS(SELECT DeviceId FROM dbo.Device WITH(READUNCOMMITTED) WHERE Hash = @Hash AND UserId =  @UserId 
                                                    ELSE IF (  (@DEVICE_ID_DATA IS NOT NULL)  
                                                                and ( 
                                                                        (@STATUS_DATA = @DELETED)
                                                                        --TODO ES FUNCIONALIDAD VALIDA
                                                                        --validar estado temporal fecha vencida 
                                                                        OR
                                                                        (
                                                                              @STATUS_DATA =  @STATUS_TEMP AND
                                                                              dateadd(second, 
                                                                              datepart(hour,@TIME_EXPIRATION) * 3600 + 
                                                                              datepart(minute,@TIME_EXPIRATION) * 60 + 
                                                                              datepart(second,@TIME_EXPIRATION),@DATE_ACTIVATED_DATA) < @DATENOW 
                                                                        )
                                                                     ) 
                                                             )
                                                       
													BEGIN 
                                                     -- reactivar el dispositivo
                                                        UPDATE Device
                                                       SET DateActivated = @DateActivated
                                                          ,Status = @Status
                                                        WHERE Hash =  @Hash AND UserId =  @UserId
			
													END
                                                    ELSE
                                                     -- ASIGNAR ERROR
                                                      SET @OP_RESULT=  @MY_ERRORCODE  
                                                 

                                            
                                                ";
                        }
                        RESULT = @"
                                                SELECT @OP_RESULT
                                                --DETERMINAR EL ESTADO DEL DISPOSITOVO ANTES DE 
                                                --PERMITIRIA DETERMINAR COMPORTAMIENTOS SOSPECHOSOS INSITIR EN INSERTAR UN DISPOSITIVO YA BORRADO
                                                SELECT @STATUS_DATA
                                                ";
                        mySqlCommand.CommandText = String.Concat(VALUES_DECLARATION, OPERATION, RESULT);
                        //constantes en el query
                        TimeSpan tiempo = DEVICE_TEMP_TIME;
                        //TODO PROBLEMA 
                        mySqlCommand.Parameters.AddWithValue("@DATENOW", DateTime.Now);
                        mySqlCommand.Parameters.AddWithValue("@TIME_EXPIRATION", tiempo);
                        mySqlCommand.Parameters.AddWithValue("@DELETED", cons.DEVICE_DELETE);
                        mySqlCommand.Parameters.AddWithValue("@MY_ERRORCODE", ResponseCode);
                        mySqlCommand.Parameters.AddWithValue("@STATUS_TEMP", cons.DEVICE_TEMPORAL);
                        //datos
                        mySqlCommand.Parameters.AddWithValue("@UserId", device.UserId);
                        mySqlCommand.Parameters.AddWithValue("@Token", device.Token);
                        mySqlCommand.Parameters.AddWithValue("@Hash", device.Hash);
                        mySqlCommand.Parameters.AddWithValue("@DeviceTypeId", device.IdType);
                        mySqlCommand.Parameters.AddWithValue("@FriendlyName", device.FriendlyName);
                        mySqlCommand.Parameters.AddWithValue("@Description", device.Description);
                        //
                        mySqlCommand.Parameters.AddWithValue("@DateCreated", DateTime.Now);
                        mySqlCommand.Parameters.AddWithValue("@DateActivated", new DateTime(device.Ticks));
                        mySqlCommand.Parameters.AddWithValue("@Status", device.Status);
                        mySqlCommand.Parameters.AddWithValue("@Secure", device.Secure);

                        mySqlCommand.Parameters.AddWithValue("@Model", device.Model);
                        mySqlCommand.Parameters.AddWithValue("@OS", device.OS);
                        object rowsafected = ResponseCode;
                        object laststate = cons.DEVICE_DELETE;
                        //leer multiples resultados
                        using (SqlDataReader dr = mySqlCommand.ExecuteReader())
                        {



                            //if (dr.HasRows && dr.Read())
                            //{
                            //    rowsafected = dr[0];
                            //}

                            //if(dr.NextResult()){
                            //    if (dr.HasRows && dr.Read())
                            //    {
                            //        laststate = dr[0];
                            //    }
                            //}

                            int i = 1;
                            do
                            {
                                if (dr.HasRows && dr.Read())
                                {
                                    switch (i)
                                    {
                                        case 1:
                                            rowsafected = dr[0];
                                            break;
                                        case 2:
                                            if (!dr[0].Equals(System.DBNull.Value))
                                                laststate = dr[0];
                                            break;
                                    }
                                }
                                i++;
                            } while (dr.NextResult());

                        }

                        int resultint = Convert.ToInt32(rowsafected);
                        int lastestate = Convert.ToInt32(laststate);


                        tran.Commit();
                        // si la ejecucion del query retorna el mismo codigo error
                        if (resultint == ResponseCode)
                        {
                            result.SetResultData(false, ResponseCode, "EL DISPOSITIVO YA EXISTE EN LA LISTA");
                            //evaluar si el dispositivo tiene actividad sospechosa
                        }

                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        result.SetErrorResultData(false, ex, ResponseCode, "ERROR CREANDO DISPOSITIVO");
                        string _m = String.Concat("[QRY] ", LOG_PREFIX, " ", result.ResponseCode, "-", result.ResponseMessage, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(_m);
                        throw;
                    }

                }

                return result;
            }
            catch (Exception ex)
            {
                string _m = String.Concat("[QRY] ", LOG_PREFIX, " ", result.ResponseCode, "-", result.ResponseMessage, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                logger.ErrorLow(_m);
                result.SetErrorResultData(false, ex, ResponseCode, "ERROR INESPERADO GUARDANDO DISPOSITIVO");

                throw;
            }

        }
        public static GenericApiResult<bool> AddTrustedDeviceNuevo(TrustedDevice device)
        {
            GenericApiResult<bool> result = new GenericApiResult<bool>();
            //supongo que todo saldra bien
            result.SetResultData(true);
            //primer alias
            int ResponseCode = 1;
            try
            {

                //VALIDACION HASH

                bool isValidHash = true;
                //

                MD5 md5 = new MD5CryptoServiceProvider();

                string preHash = Movilway.API.Core.Security.Cryptography.encrypt(string.Concat(device.Token, "_", device.Ticks));
                var hashval = Convert.ToBase64String(md5.ComputeHash(Encoding.Unicode.GetBytes(preHash)));

                isValidHash = hashval == device.Hash;


                //validar el peor caso imediato
                if (!isValidHash)
                {
                    result.SetResultData(false, ResponseCode, "EL HASH DE LA PETICION ES INVALIDO");
                    //retornar el error
                    return result;

                }

                // no else code

                ResponseCode = 2;

                string strConnString = ConfigurationManager.ConnectionStrings["SECURE_DB"].ConnectionString;
                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {


                    mySqlConnection.Open();

                    //SqlTransaction tran = mySqlConnection.BeginTransaction();
                    try
                    {

                        SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                        //mySqlCommand.Transaction = tran;


                        mySqlCommand.CommandText = @"-->
                            declare @Estado int
                            declare @DevideId int
                            select top 1 @DevideId=deviceid,@Estado=[Status] FROM [Device] where [Token]=@Token and [UserId]=@UserId
                            --select @DevideId,@Estado


                            IF (@DevideId is null ) --en teorìa con la versiòn nueva no deberìa caer acà (Pero alguien con la app vieja si)
                            BEGIN 	
                            INSERT INTO [Device]
                                       ([UserId]
                                       ,[Token]
                                       ,[Hash]
                                       ,[DeviceTypeId]
                                       ,[FriendlyName]
                                       ,[Description]
                                       ,[DateActivated]
                                       ,[Status]
                                       ,[LastAccess]
                                       ,[DateBlocked]
                                       ,[Model]
                                       ,[OS]
                                       ,[Secure]
                                       ,[DateCreated]
                                       ,[IpAccess]
                                       ,[Long]
                                       ,[Lat])
                                 VALUES
                                       (@UserId
                                       ,@Token
                                       ,@Hash
                                       ,@DevideTypeId
                                       ,@FriendlyName
                                       ,@Description
                                       ,@Activated
                                       ,@Status
                                       ,@LastAccess
                                       ,NULL
                                       ,@Model
                                       ,@OS
                                       ,@Secure
                                       ,@Created
                                       ,@Ip
                                       ,@Long
                                       ,@Lat)
                            END 
                            ELSE 
                            BEGIN 
	                            UPDATE [Device] 
	                            SET [Hash]=@Hash, [DateActivated]=@Activated,[Status]=@Status,[DateBlocked]=NULL,[OS]=@OS,[Secure]=@Secure,[LastAccess]=@LastAccess
	                            WHERE  DeviceId=@DevideId and [Status] not in (0)
                            END 

                            If @Estado =0
	                            select -1
                            else
	                            select 1
                            ";

                        mySqlCommand.Parameters.AddWithValue("@UserId", device.UserId);
                        mySqlCommand.Parameters.AddWithValue("@Token", device.Token);

                        mySqlCommand.Parameters.AddWithValue("@Hash", device.Hash);
                        mySqlCommand.Parameters.AddWithValue("@DevideTypeId", device.IdType);
                        mySqlCommand.Parameters.AddWithValue("@FriendlyName", device.FriendlyName);
                        mySqlCommand.Parameters.AddWithValue("@Description", device.Description);
                        mySqlCommand.Parameters.AddWithValue("@Activated", new DateTime(device.Ticks));
                        mySqlCommand.Parameters.AddWithValue("@Status", device.Status);//REvisar
                        mySqlCommand.Parameters.AddWithValue("@LastAccess", DateTime.Now /*new DateTime(device.Ticks)*/);
                        mySqlCommand.Parameters.AddWithValue("@Model", device.Model);
                        mySqlCommand.Parameters.AddWithValue("@OS", device.OS);
                        mySqlCommand.Parameters.AddWithValue("@Secure", device.Secure);

                        mySqlCommand.Parameters.AddWithValue("@Created", DateTime.Now);
                        mySqlCommand.Parameters.AddWithValue("@Ip", "");
                        mySqlCommand.Parameters.AddWithValue("@Long", "-");
                        mySqlCommand.Parameters.AddWithValue("@Lat", "-");


                        int estado = -1;
                        using (SqlDataReader dr = mySqlCommand.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                estado = Convert.ToInt32(dr[0]);
                            }
                        }

                        if (estado == -1)
                        {
                            result.SetResultData(false, ResponseCode, "EL DISPOSITIVO SE ENCUENTRA EN UN ESTADO INVÀLIDO [BL]");
                            //retornar el error
                            return result;
                        }



                        //tran.Commit();


                    }
                    catch (Exception ex)
                    {
                        //tran.Rollback();
                        result.SetErrorResultData(false, ex, ResponseCode, "ERROR CREANDO DISPOSITIVO");
                        string _m = String.Concat("[QRY] ", LOG_PREFIX, " ", result.ResponseCode, "-", result.ResponseMessage, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                        logger.ErrorLow(_m);
                        throw;
                    }

                }

                return result;
            }
            catch (Exception ex)
            {
                string _m = String.Concat("[QRY] ", LOG_PREFIX, " ", result.ResponseCode, "-", result.ResponseMessage, ". Exception: ", ex.Message, ". ", ex.StackTrace);
                logger.ErrorLow(_m);
                result.SetErrorResultData(false, ex, ResponseCode, "ERROR INESPERADO GUARDANDO DISPOSITIVO");

                throw;
            }

        }
        public static TimeSpan DEVICE_TEMP_TIME
        {

            get
            {

                TimeSpan ts = TimeSpan.Parse("0:01:00");
                string DEVICE_TEMP_TIME = ConfigurationManager.AppSettings["DEVICE_TEMP_TIME"];
                if (!string.IsNullOrEmpty(DEVICE_TEMP_TIME))
                    ts = TimeSpan.Parse(DEVICE_TEMP_TIME);
                return ts;
            }
        }

        internal static BankAccount[] GetAccountBanks(string access, out string message)
        {
            List<BankAccount> bankAccounts = new List<BankAccount>();
            message = "";

            try
            {
                var query = String.Concat(@"SELECT [cubId],[cubNumero],[cubCBU],b.[banId],b.[banNombre] ",
                                           "FROM [KfnCuentaBanco] cb ",
                                           "JOIN [KfnBanco] b ON cb.banid=b.banid ",
                                           "JOIN [Usuario] u ON cb.ageid=u.age_id ",
                                           "JOIN [Acceso] a ON a.usr_id=u.usr_id ",
                                           "WHERE a.acc_login=@acc_login");

                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString))
                {
                    sqlConnection.Open();
                    var command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@acc_login", access);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                            {
                                var bankAccount = new BankAccount();
                                bankAccount.Id = int.Parse(reader["cubId"].ToString());
                                bankAccount.Number = reader["cubNumero"].ToString();
                                bankAccount.CBU = reader["cubCBU"].ToString();
                                bankAccount.BankId = int.Parse(reader["banId"].ToString());
                                bankAccount.BankName = reader["banNombre"].ToString();

                                bankAccounts.Add(bankAccount);
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                string _m = "Error trantando de consultar los datos de los bancos asociados en la base de datos de Kinacu";
                message = _m;
                logger.ErrorLow(() => TagValue.New().Exception(ex).Message(_m));
                throw new Exception(_m, ex);
            }
            return bankAccounts.ToArray();
        }

        //RM
        public static List<String> GetPinUser(String user)
        {

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            List<String> _users = new List<string>();
            //return _users;
            try
            {
                var query = " SELECT a.acc_login from Acceso a join Usuario u on u.usr_id=a.usr_id join [RolUsuario] ru on ru.usr_id=a.usr_id where u.[age_id]=(select u.age_id from [dbo].[Acceso] a join Usuario u on u.usr_id=a.usr_id where acc_login=@user) and a.tac_id=12 and ru.rol_id=@PINRol ";

                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                command.Parameters.AddWithValue("@user", user);
                command.Parameters.AddWithValue("@PINRol", 24);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        _users.Add(Convert.ToString(reader["acc_login"]));
                    }
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de obtener usuarios PIN"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return _users;
        }


        public static IEnumerable<User> GetUsers(int ageId, string loginRequest, int AccessRequest, bool onlyChildrens = false, bool includeAccess = false)
        {

            try
            {
                List<User> result = new List<User>();

                string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;



                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {
                    mySqlConnection.Open();
                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                    if (onlyChildrens)
                    {
                        if (includeAccess)
                            mySqlCommand.CommandText = @"select u.usr_id,u.usr_estado,u.usr_nombre,u.usr_apellido,acc.acc_login,acc.tac_id,ta.tac_descripcion,a.age_nombre  
                        from [dbo].[Usuario] u with(readuncommitted)
                        join  [Acceso] acc with(readuncommitted) on acc.usr_id=u.usr_id
                        join [dbo].[TipoAcceso] ta with(readuncommitted) on ta.tac_id= acc.tac_id
                        join [dbo].[Agente] a with(readuncommitted) on a.age_id=u.age_id
                        where u.age_id in (select age_id from agente with(readuncommitted) where age_id_sup=@ageId)
                        order by u.usr_id";
                        else
                            mySqlCommand.CommandText = @"select u.usr_id,u.usr_estado,u.usr_nombre,u.usr_apellido,a.age_nombre  
                                from [dbo].[Usuario] u with(readuncommitted)
                                join [dbo].[Agente] a with(readuncommitted) on a.age_id=u.age_id
                                where u.age_id in (select age_id from agente with(readuncommitted) where age_id_sup=@ageId)
                                order by u.usr_id";


                        // mySqlCommand.Parameters.AddWithValue("@ageId", ageId);
                    }
                    else
                    {
                        if (includeAccess)
                            mySqlCommand.CommandText = @"select u.usr_id,u.usr_estado,u.usr_nombre,u.usr_apellido,acc.acc_login,acc.tac_id,ta.tac_descripcion,a.age_nombre from [Usuario] u with(readuncommitted) join [Acceso] acc with(readuncommitted) on acc.usr_id=u.usr_id join [TipoAcceso] ta with(readuncommitted) on ta.tac_id= acc.tac_id join [dbo].[Agente] a with(readuncommitted) on a.age_id=u.age_id where u.age_id=@ageId order by u.usr_id;";
                        else
                            mySqlCommand.CommandText = @"select u.usr_id,u.usr_estado,u.usr_nombre,u.usr_apellido,a.age_nombre
                            from [dbo].[Usuario] u with(readuncommitted)
                            join [dbo].[Agente] a with(readuncommitted) on a.age_id=u.age_id
                            where u.age_id=@ageId 
                            order by u.usr_id";


                    }

                    mySqlCommand.Parameters.AddWithValue("@ageId", ageId);


                    int currentUser = -1;

                    using (var reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int userId = Convert.ToInt32(reader["usr_id"]);
                                //Si es un usuario nuevo.
                                if (currentUser != userId)
                                {
                                    result.Add(new User
                                    {
                                        UserId = userId,
                                        UserName = Convert.ToString(reader["usr_nombre"]),
                                        UserLastName = Convert.ToString(reader["usr_apellido"]),
                                        UserStatus = Convert.ToString(reader["usr_estado"]),
                                        AgentName = Convert.ToString(reader["age_nombre"])
                                    });
                                    currentUser = userId;
                                }
                                if (includeAccess)
                                    result.ElementAt(result.Count - 1).Access.Add(new GetUserAgentAccess { Login = Convert.ToString(reader["acc_login"]), AccessType = Convert.ToString(reader["tac_descripcion"]), AccessTypeId = Convert.ToInt32(reader["tac_id"]) });
                            }
                        }
                    }
                }





                return result;
            }
            catch (Exception ex)
            {


                throw;//new Exception( message,ex);
            }
        }

        public static bool SetUserStatus(int userId, string Status, int taid = 0, string comment = "")
        {

            try
            {


                string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;



                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {
                    mySqlConnection.Open();
                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                    if (Status.ToLower().Equals("bl"))
                    {
                        mySqlCommand.CommandText = @"update [dbo].[Usuario]  set usr_estado='BL' where usr_id=@userId;
                        declare @nextId int;
                        select @nextId=(sec_number+1) from secuencia where sec_objectName = 'BLOQUEO';
                        if( not exists(select * from [Bloqueos] where usr_id=@userId and [blo_fecha_baja] is null))
                        begin
                        INSERT INTO [dbo].[Bloqueos]
                                   ([blo_id]
                                   ,[blo_fecha]
                                   ,[usr_id]
                                   ,[tac_id]
                                   ,[blo_motivo]
                                   ,[blo_comentarios]
                                   ,[blo_fecha_baja]
                                   ,[blo_bloqueado_por])
                             VALUES
                                   (@nextId
                                   ,GETDATE()
		                           ,@userId
                                   ,@taid
                                   ,'ADMIN'
                                   ,NULL
                                   ,NULL
                                   ,'ADMIN');
                        Update secuencia set sec_number = sec_number + 1 where sec_objectName = 'BLOQUEO'
                        end";
                        mySqlCommand.Parameters.AddWithValue("@taid", taid == 0 ? 12 : taid);
                    }
                    else if (Status.ToLower().Equals("ac"))
                    {
                        mySqlCommand.CommandText = @"declare @BajaDate datetime=GETDATE();
                        update [dbo].[Usuario]  set usr_estado='AC' where usr_id=@userId;
                        update [dbo].[Acceso]  set acc_intentos=0 where usr_id=@userId and acc_intentos>=5;
                        declare @bloqId int;
                        select @bloqId=max([blo_id]) from [Bloqueos] where usr_id=@userId  and [blo_fecha_baja] is null;
                        update [Bloqueos] set [blo_comentarios]=@reason, [blo_fecha_baja]=@BajaDate where blo_id=@bloqId;";
                        mySqlCommand.Parameters.AddWithValue("@reason", comment);
                    }
                    else
                        throw new Exception("Estado no definido");
                    mySqlCommand.Parameters.AddWithValue("@userId", userId);

                    mySqlCommand.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {


                throw ex;//new Exception( message,ex);
            }
        }

        internal static bool ChangePassword(string userLoginRequest, int accessTypeRequest, int userIdTochange, int accessIdToChange, string newPassword)
        {

            try
            {


                string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString;



                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {
                    mySqlConnection.Open();
                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();


                    mySqlCommand.CommandText = @"declare @LoginAgent int
                        declare @UserAgent int

                        select @LoginAgent=u.age_id from usuario u 
                        join  [Acceso] a on a.usr_id=u.usr_id where a.acc_login=@userLogin and a.tac_id=@RequestAccessType;

                        select @UserAgent=u.age_id from usuario u 
                        join  [Acceso] a on a.usr_id=u.usr_id where a.usr_id=@userIdToChange and a.tac_id=@AccessIdToChange;


                        update acceso set acc_password=@NewPassword,acc_intentos=0
                        where usr_id=@userIdToChange and tac_id=@AccessIdToChange and ([dbo].[EsAgenciaDescendiente](@LoginAgent,@UserAgent)=1 or @LoginAgent=@UserAgent);";
                    mySqlCommand.Parameters.AddWithValue("@userLogin", userLoginRequest);
                    mySqlCommand.Parameters.AddWithValue("@RequestAccessType", accessTypeRequest);

                    mySqlCommand.Parameters.AddWithValue("@userIdToChange", userIdTochange);
                    mySqlCommand.Parameters.AddWithValue("@AccessIdToChange", accessIdToChange);
                    MD5 md5 = new MD5CryptoServiceProvider();
                    mySqlCommand.Parameters.AddWithValue("@NewPassword", Convert.ToBase64String(md5.ComputeHash(System.Text.Encoding.Unicode.GetBytes(newPassword))));



                    //mySqlCommand.Parameters.AddWithValue("@userId", userId);

                    int result = mySqlCommand.ExecuteNonQuery();


                    return result == 1;//true;
                }
            }
            catch (Exception ex)
            {


                throw ex;//new Exception( message,ex);
            }
        }

        internal static DateTime GetLocalTimeZone(int countryId = 0)
        {

            if (countryId == 0)
                countryId = Convert.ToInt32(ConfigurationManager.AppSettings["CountryID"]);

            //TimeZoneInfo _current = TimeZoneInfo.FindSystemTimeZoneById("Venezuela Standard Time");
            //int[] _utc_m3 = new int[] {1,2,9,16 };
            //int[] _utc_m4VE = new int[] { 5 };//Mosca con usar la hora de vzla
            //int[] _utc_m4 = new int[] { 13 };
            int[] _utc_AR = new int[] { 1, 14 };//SA Pacific Standard Time
            int[] _utc_m5 = new int[] { 3, 7, 4, 6, 17 };//SA Pacific Standard Time
            int[] _utc_m6 = new int[] { 10, 11, 12 };//Central America Standard Time

            if (_utc_AR.Contains(countryId))//Argentina
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time"));
            else if (countryId == 2)//Uruguay
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Montevideo Standard Time"));
            else if (_utc_m5.Contains(countryId))
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"));
            else if (countryId == 5)
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Venezuela Standard Time"));
            else if (countryId == 8)
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)"));
            else if (countryId == 9)
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time"));
            else if (_utc_m6.Contains(countryId))
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time"));
            else if (countryId == 13)
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time"));
            else if (countryId == 15)
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            else if (countryId == 16)
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time"));



            else return DateTime.Now;
        }


        public static SalesSummaryDashBoardItems SalesSummaryByAgentDashBoard(String agentReference, DateTime InitialDate, DateTime EndDate, out string ResponseMessage)
        {
            ResponseMessage = "OK";
            SalesSummaryDashBoardItems summariesDash = new SalesSummaryDashBoardItems();

            try
            {
                var CountryId = int.Parse(ConfigurationManager.AppSettings["CountryID"]);
                var PlatformId = 1;

                var ReverseProducts = (ConfigurationManager.AppSettings["ReverseProductsID"] ?? "-1,-2");

                var EmitirGiro = (ConfigurationManager.AppSettings["MultiPayTopUpMno"] != null ? int.Parse(ConfigurationManager.AppSettings["MultiPayTopUpMno"]) : -1);
                var PagarGiro = (ConfigurationManager.AppSettings["MultiPayReverseTopUpMno"] != null ? int.Parse(ConfigurationManager.AppSettings["MultiPayReverseTopUpMno"]) : -1);


                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["BASEConnectionString"].ConnectionString))
                {
                    sqlConnection.Open();
                    var query = @"
                        DECLARE @CountryId INT = @ParamCountryId;
                        DECLARE @PlatformId INT = @ParamPlatformId;
                        DECLARE @Login VARCHAR(60) = @ParamLogin;
                        DECLARE @StatusId VARCHAR(2) = @ParamStatusId;
                        DECLARE @LowerLimit DATE = @ParamLowerLimit;
                        DECLARE @UpperLimit DATE = @ParamUpperLimit;
                        DECLARE @ReverseProducts VARCHAR(50) = @ParamReverseProducts;  
                        DECLARE @EmitirGiro INT = @ParamEmitirGiro;
                        DECLARE @PagarGiro INT = @ParamPagarGiro;
                 
                        SET DATEFIRST 1;

                        WITH AgeInfo AS (
                        SELECT 
                        TOP (1) u.BranchId, a.UserId, a.AccessTypeId 
                        FROM 
                        [dbo].[Access] AS a WITH(READUNCOMMITTED) 
                        JOIN [dbo].[User] AS u WITH(READUNCOMMITTED) ON (u.CountryId = a.CountryId AND u.PlatformId = a.PlatformId AND u.UserId = a.UserId)
                        WHERE a.CountryId = @CountryId AND a.PlatformId = @PlatformId AND a.[Login] = @Login
                        ),
                        ReverseProductsIds AS (
                            SELECT t.value('.', 'INT') AS ProductId 
                            FROM (
                                SELECT CAST('<root><id>' + REPLACE(ISNULL(@ReverseProducts, ''), ',', '</id><id>') + '</id></root>' AS XML) AS ProductsXml
                            ) AS prods 
                            CROSS APPLY prods.ProductsXml.nodes('/root/id') AS x(t) 
                            WHERE @ReverseProducts IS NOT NULL AND LEN(LTRIM(RTRIM(@ReverseProducts))) > 0 AND LEN(LTRIM(RTRIM(@ReverseProducts))) <> '-1'
                        ),
                        TrxCurrentDay AS (
	                        SELECT
                                pn.ProductId AS 'ProductId'
		                        ,pn.ProductName AS 'Product'
                                ,(CASE WHEN p.ProductId IS NOT NULL THEN t.amount ELSE (t.amount * -1) END) AS 'Amount' 
		                        ,t.[DateValue]
	                        FROM
		                        [dbo].[TransactionCurrentDay] AS t WITH(READUNCOMMITTED) 
		                        JOIN [dbo].[Product] AS pn WITH(READUNCOMMITTED) ON (pn.CountryId = t.CountryId AND pn.PlatformId = t.PlatformId AND pn.ProductId = t.ProductId) 
		                        LEFT JOIN ReverseProductsIds AS p ON (p.ProductId = t.ProductId) 
	                        WHERE 
		                        t.CountryId = @CountryId 
		                        AND t.PlatformId = @PlatformId 
		                        AND t.DateValue >= CAST(GETDATE() AS DATE) 
		                        AND t.DateValue <= @UpperLimit 
		                        AND t.AccessTypeId = (SELECT AccessTypeId FROM AgeInfo) 
		                        AND t.BranchId = (SELECT BranchId FROM AgeInfo) 
		                        AND t.StatusId = @StatusId 
                                AND t.ProductId not in (@EmitirGiro,@PagarGiro)
                        ),
                        TrxThreeMonths AS (
	                        SELECT
                                pn.ProductId AS 'ProductId'
		                        ,pn.ProductName AS 'Product'
                                ,(CASE WHEN p.ProductId IS NOT NULL THEN t.amount ELSE (t.amount * -1) END) AS 'Amount'
		                        ,t.[DateValue]
	                        FROM
		                        [dbo].[TransactionThreeMonths] AS t WITH(READUNCOMMITTED) 
		                        JOIN [dbo].[Product] AS pn WITH(READUNCOMMITTED) ON (pn.CountryId = t.CountryId AND pn.PlatformId = t.PlatformId AND pn.ProductId = t.ProductId) 
		                        LEFT JOIN ReverseProductsIds AS p ON (p.ProductId = t.ProductId) 
	                        WHERE 
		                        t.CountryId = @CountryId 
		                        AND t.PlatformId = @PlatformId 
		                        AND t.DateValue <= @UpperLimit 		
		                        AND t.DateValue >= @LowerLimit 
		                        AND t.AccessTypeId = (SELECT AccessTypeId FROM AgeInfo) 
		                        AND t.BranchId = (SELECT BranchId FROM AgeInfo) 
		                        AND t.StatusId = @StatusId 
                                AND t.ProductId not in (@EmitirGiro,@PagarGiro)
                        ),
                        AllInfo AS (
                            SELECT * FROM TrxCurrentDay 
                            UNION ALL 
                            SELECT * FROM TrxThreeMonths 
                        )
                        SELECT
                            datepart(wk, [DateValue]) As NumWeek
	                        ,DATEADD(wk,DATEDIFF(wk,0,[DateValue]),0) InitDay
	                        ,DATEADD(wk,DATEDIFF(wk,0,[DateValue]),6) EndDay
                            ,ProductId
                            ,Product
                            ,SUM(Amount) * -1  AS TotalAmount
                            ,COUNT(1) AS TotalTrx
                        FROM 
                            AllInfo 
                        GROUP BY datepart(wk, [DateValue]), 
                                 datepart(yyyy, [DateValue]),
		                         DATEADD(wk,DATEDIFF(wk,0,[DateValue]),0), 
		                         DATEADD(wk,DATEDIFF(wk,0,[DateValue]),6),Product,ProductId
                        order by InitDay,NumWeek;";


                    var command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@ParamCountryId", CountryId);//3
                    command.Parameters.AddWithValue("@ParamPlatformId", PlatformId);//INT = 1;
                    command.Parameters.AddWithValue("@ParamLogin", agentReference);//"440044"
                    command.Parameters.AddWithValue("@ParamLowerLimit", InitialDate.ToString("yyyyMMdd"));//"20150201"
                    command.Parameters.AddWithValue("@ParamUpperLimit", EndDate.ToString("yyyyMMdd"));//"20150512"
                    command.Parameters.AddWithValue("@ParamReverseProducts", ReverseProducts); //VARCHAR(50) = '';
                    command.Parameters.AddWithValue("@ParamStatusId ", "AC");
                    command.Parameters.AddWithValue("@ParamEmitirGiro", EmitirGiro);//83
                    command.Parameters.AddWithValue("@ParamPagarGiro", PagarGiro);//84

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                summariesDash.Add(new SalesSummaryDashItem()
                                {
                                    NumWeek = reader.GetInt32(0),
                                    InitDate = reader.GetDateTime(1),
                                    EndDate = reader.GetDateTime(2),
                                    ProductId = reader.GetInt32(3),
                                    ProductName = reader.GetString(4),
                                    TotalAmount = reader.GetDecimal(5),
                                    TransactionCount = reader.GetInt32(6)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string Message = "ERROR TRATANDO DE CONSULTAR EL RESÚMEN DE VENTAS EN LA BASE DE DATOS DE KINACU";
                ResponseMessage = Message;
                logger.ErrorHigh(() => TagValue.New().Exception(e).Message(Message).Tag("Agencia").Value(agentReference).Tag("FechaInical").Value(InitialDate.ToString("yyyyMMdd")).Tag("FechaFinal").Value(EndDate.ToString("yyyyMMdd")));
            }

            return (summariesDash);

        }


        public static PurchasesSummaryDashBoardItems PurchasesSummaryByAgentDashBoard(String agentReference, DateTime InitialDate, DateTime EndDate, out string ResponseMessage)
        {
            ResponseMessage = "OK";
            PurchasesSummaryDashBoardItems summariesDash = new PurchasesSummaryDashBoardItems();

            try
            {
                var CountryId = int.Parse(ConfigurationManager.AppSettings["CountryID"]);
                var PlatformId = 1;

                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["BASEConnectionString"].ConnectionString))
                {
                    sqlConnection.Open();
                    var query = @"
					DECLARE @CountryId INT = @ParamCountryId;
					DECLARE @PlatformId INT = @ParamPlatformId;
					DECLARE @Login VARCHAR(25) = @ParamLogin;
					DECLARE @LowerLimit DATE = @ParamLowerLimit;
					DECLARE @UpperLimit DATE = @ParamUpperLimit;

                
				       	SET DATEFIRST 1;
                        SET @UpperLimit = DATEADD(DAY, 1, @UpperLimit);

                        WITH AgeInfo AS (
                            SELECT 
                                 TOP (1) u.BranchId, a.UserId, a.AccessTypeId
                            FROM 
                                [dbo].[Access] AS a WITH(READUNCOMMITTED) 
                                JOIN [dbo].[User] AS u WITH(READUNCOMMITTED) ON (u.CountryId = a.CountryId AND u.PlatformId = a.PlatformId AND u.UserId = a.UserId)
                            WHERE a.CountryId = 3 AND a.PlatformId = 1 AND UPPER(a.[Login]) = UPPER(@Login)
                        ),
                        Dep AS (
                            SELECT
                                D.*
                            FROM
                                [dbo].Request AS D WITH(READUNCOMMITTED)
                            WHERE
                                D.CountryId = @CountryId
                                AND D.PlatformId = @PlatformId
                                AND D.[DateCreated] >= @LowerLimit
                                AND D.[DateCreated] < @UpperLimit
                                AND D.[RecipientBranchId] = (SELECT BranchId FROM AgeInfo)                        
                                AND D.StatusId = 'CE'
								AND [RequestTypeId] = 202 
				        )
                        SELECT
				            DATEADD(wk,DATEDIFF(wk,0,Dep.[DateCreated]),0) InitDay
					        ,DATEADD(wk,DATEDIFF(wk,0,Dep.[DateCreated]),6) EndDay                                	
                            ,SUM(Dep.Amount) AS 'Amount'     
					        , COUNT(1) AS TotalDep                                                     
                        FROM
                            Dep                   
                            JOIN dbo.Branch AS BD WITH(READUNCOMMITTED) ON (BD.CountryId = Dep.CountryId AND BD.PlatformId = Dep.PlatformId AND BD.BranchId = Dep.[RecipientBranchId])
                        GROUP BY  DATEADD(wk,DATEDIFF(wk,0,Dep.[DateCreated]),0),DATEADD(wk,DATEDIFF(wk,0,Dep.[DateCreated]),6)    
				        ORDER BY  InitDay ASC;";


                    var command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@ParamCountryId", CountryId);//3
                    command.Parameters.AddWithValue("@ParamPlatformId", PlatformId);//INT = 1;
                    command.Parameters.AddWithValue("@ParamLogin", agentReference);//"440044" TestinC
                    command.Parameters.AddWithValue("@ParamLowerLimit", InitialDate.ToString("yyyyMMdd"));//"20160926"
                    command.Parameters.AddWithValue("@ParamUpperLimit", EndDate.ToString("yyyyMMdd"));//"20170417"                    

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                summariesDash.Add(new PurchasesSummaryDashItem()
                                {
                                    InitDate = reader.GetDateTime(0),
                                    EndDate = reader.GetDateTime(1),
                                    TotalAmount = reader.GetDecimal(2),
                                    DepositCount = reader.GetInt32(3)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string Message = "ERROR TRATANDO DE CONSULTAR EL RESÚMEN DE COMPRAS EN LA BASE DE DATOS DE KINACU";
                ResponseMessage = Message;
                logger.ErrorHigh(() => TagValue.New().Exception(e).Message(Message).Tag("Agencia").Value(agentReference).Tag("FechaInical").Value(InitialDate.ToString("yyyyMMdd")).Tag("FechaFinal").Value(EndDate.ToString("yyyyMMdd")));
            }

            return (summariesDash);

        }


        public static CommissionPurchasesSummaryDashItem CommissionPurchasesSummaryByAgentDashBoard(String agentReference, DateTime InitialDate, DateTime EndDate, out string ResponseMessage)
        {
            ResponseMessage = "OK";
            CommissionPurchasesSummaryDashItem summariesCommi = new CommissionPurchasesSummaryDashItem();

            try
            {
                var CountryId = int.Parse(ConfigurationManager.AppSettings["CountryID"]);
                var PlatformId = 1;

                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["BASEConnectionString"].ConnectionString))
                {
                    sqlConnection.Open();
                    var query = @"
                        DECLARE @CountryId INT = @ParamCountryId;
				        DECLARE @PlatformId INT = @ParamPlatformId;
				        DECLARE @Login VARCHAR(25) = @ParamLogin;
				        DECLARE @LowerLimit DATE = @ParamLowerLimit;
				        DECLARE @UpperLimit DATE = @ParamUpperLimit;
                
				       	WITH AgeInfo AS (
                        SELECT 
                                TOP (1) u.BranchId, a.UserId, a.AccessTypeId,b.CommissionByPurchase
                        FROM 
                            [dbo].[Access] AS a WITH(READUNCOMMITTED) 
                            JOIN [dbo].[User] AS u WITH(READUNCOMMITTED) ON (u.CountryId = a.CountryId AND u.PlatformId = a.PlatformId AND u.UserId = a.UserId)
                            JOIN [dbo].[Branch] AS b WITH(READUNCOMMITTED) ON  (b.CountryId = a.CountryId AND b.PlatformId = a.PlatformId AND b.BranchId = u.BranchId)
	                    WHERE a.CountryId = @CountryId AND a.PlatformId = @PlatformId AND a.[Login] = @Login
	                    ),
	                    CommiPurchases AS (
	                    SELECT P.Amount, P.RequestId, P.RecipientBranchId
	                    FROM [dbo].[Request] AS P WITH(READUNCOMMITTED) 
	                    WHERE P.CountryId = @CountryId 
	                    AND P.PlatformId = @PlatformId 
	                    AND P.[RecipientBranchId] =  (SELECT BranchId FROM AgeInfo) 
	                    AND P.RequestId  IN (SELECT Tra_Id_3 FROM [dbo].[Trazabilidad]  WITH(READUNCOMMITTED) WHERE CountryId = @CountryId AND PlatformId = @PlatformId)
	                    AND P.StatusId = 'CE'  
	                    AND [DateCreated] >= @LowerLimit
	                    AND [DateCreated]  <= @UpperLimit
	                    AND [RequestTypeId] IN (501)
	                    )	
	                    SELECT SUM(CP.Amount) AS TotalCommiPurchases, ISNULL(ag.CommissionByPurchase, 0) AS CommiPercentage  
	                    FROM  CommiPurchases AS CP 
	                     JOIN AgeInfo AS Ag ON CP.RecipientBranchId = Ag.BranchId
	                     GROUP BY Ag.BranchId, ag.CommissionByPurchase;";


                    var command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@ParamCountryId", CountryId);//3
                    command.Parameters.AddWithValue("@ParamPlatformId", PlatformId);//INT = 1;
                    command.Parameters.AddWithValue("@ParamLogin", agentReference);//"440044" TestinC
                    command.Parameters.AddWithValue("@ParamLowerLimit", InitialDate.ToString("yyyyMMdd"));//"20160926"
                    command.Parameters.AddWithValue("@ParamUpperLimit", EndDate.ToString("yyyyMMdd"));//"20170420"                    

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                summariesCommi.TotalCommiPurchases = reader.GetDecimal(0);
                                summariesCommi.CommiPercentage = reader.GetDecimal(1);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string Message = "ERROR TRATANDO DE CONSULTAR EL RESÚMEN DE COMISIONES DE COMPRAS EN LA BASE DE DATOS DE KINACU";
                ResponseMessage = Message;
                logger.ErrorHigh(() => TagValue.New().Exception(e).Message(Message).Tag("Agencia").Value(agentReference).Tag("FechaInical").Value(InitialDate.ToString("yyyyMMdd")).Tag("FechaFinal").Value(EndDate.ToString("yyyyMMdd")));
            }

            return (summariesCommi);

        }


        public static CommissionSalesSummaryDashItem CommissionSalesSummaryByAgentDashBoard(String agentReference, DateTime InitialDate, DateTime EndDate, out string ResponseMessage)
        {
            ResponseMessage = "OK";
            CommissionSalesSummaryDashItem summariesCommi = new CommissionSalesSummaryDashItem();

            try
            {
                var CountryId = int.Parse(ConfigurationManager.AppSettings["CountryID"]);
                var PlatformId = 1;


                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["BASEConnectionString"].ConnectionString))
                {
                    sqlConnection.Open();
                    var query = @"
                        DECLARE @CountryId INT = @ParamCountryId;
                        DECLARE @PlatformId INT = @ParamPlatformId;
                        DECLARE @Login VARCHAR(60) = @ParamLogin;
                        DECLARE @LowerLimit DATE = @ParamLowerLimit;
                        DECLARE @UpperLimit DATE = @ParamUpperLimit;                                  
                        
                 WITH AgeInfo AS (
                    SELECT 
                            TOP (1) u.BranchId, a.UserId, a.AccessTypeId,b.CommissionBySales
                    FROM 
                        [dbo].[Access] AS a WITH(READUNCOMMITTED) 
                        JOIN [dbo].[User] AS u WITH(READUNCOMMITTED) ON (u.CountryId = a.CountryId AND u.PlatformId = a.PlatformId AND u.UserId = a.UserId)
                        JOIN [dbo].[Branch] AS b WITH(READUNCOMMITTED) ON  (b.CountryId = a.CountryId AND b.PlatformId = a.PlatformId AND b.BranchId = u.BranchId)
	                WHERE a.CountryId = @CountryId AND a.PlatformId = @PlatformId AND a.[Login] = @Login
	                ),
	                CommiPurchases AS (
	                SELECT P.Amount, P.RequestId, P.RecipientBranchId
	                FROM [dbo].[Request] AS P WITH(READUNCOMMITTED) 
	                WHERE P.CountryId = @CountryId 
	                AND P.PlatformId = @PlatformId 
	                AND P.[RecipientBranchId] =  (SELECT BranchId FROM AgeInfo) 
	                AND P.RequestId NOT IN (SELECT Tra_Id_3 FROM [dbo].[Trazabilidad]  WITH(READUNCOMMITTED) WHERE CountryId = @CountryId AND PlatformId = @PlatformId )
	                AND P.StatusId = 'CE'  
	                AND [DateCreated] >= @LowerLimit
	                AND [DateCreated]  <= @UpperLimit
	                AND [RequestTypeId] IN (501)
	                )	
	                SELECT SUM(CP.Amount) AS TotalCommiSales, ISNULL(ag.CommissionBySales, '') AS CommiPercentage  
	                FROM  CommiPurchases AS CP 
	                 JOIN AgeInfo AS Ag ON CP.RecipientBranchId = Ag.BranchId
	                 GROUP BY Ag.BranchId, ag.CommissionBySales;";


                    var command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@ParamCountryId", CountryId);//3
                    command.Parameters.AddWithValue("@ParamPlatformId", PlatformId);//INT = 1;
                    command.Parameters.AddWithValue("@ParamLogin", agentReference);//"440044"
                    command.Parameters.AddWithValue("@ParamLowerLimit", InitialDate.ToString("yyyyMMdd"));//"20150201"
                    command.Parameters.AddWithValue("@ParamUpperLimit", EndDate.ToString("yyyyMMdd"));//"20150512"                    

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                summariesCommi.TotalCommiSales = reader.GetDecimal(0);
                                summariesCommi.CommiPercentage = reader.GetString(1);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string Message = "ERROR TRATANDO DE CONSULTAR EL RESÚMEN DE COMISIONES POR VENTAS EN LA BASE DE DATOS DE KINACU";
                ResponseMessage = Message;
                logger.ErrorHigh(() => TagValue.New().Exception(e).Message(Message).Tag("Agencia").Value(agentReference).Tag("FechaInical").Value(InitialDate.ToString("yyyyMMdd")).Tag("FechaFinal").Value(EndDate.ToString("yyyyMMdd")));
            }

            return (summariesCommi);

        }


        public static bool ChangeBranchContactInfo(string agentId, string paramEmail, string paramPhone)
        {
            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            var result = false;
            try
            {
                sqlConnection.Open();
                var mySqlCommand = new SqlCommand();
                mySqlCommand.Connection = sqlConnection;

                mySqlCommand.CommandText = @"UPDATE agente 
                                            SET age_tel = @paramPhone, 
                                            age_email = @paramEmail  
                                            WHERE age_id=@branchid";
                mySqlCommand.Parameters.Clear();
                mySqlCommand.Parameters.AddWithValue("@branchid", agentId);
                mySqlCommand.Parameters.AddWithValue("@paramEmail", paramEmail);
                mySqlCommand.Parameters.AddWithValue("@paramPhone", paramPhone);

                if (mySqlCommand.ExecuteNonQuery().Equals(1))
                    result = true;
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de cambiar email y telefono de un agente en la base de datos de Kinacu"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return result;
        }


        private static List<RequestAgent.Product> GetProductsFromCommi(List<RequestAgent.Product> paramList)
        {
            List<ProductCommi> tempProductsCommi = null;
            List<RequestAgent.Product> returnList = null;

            try
            {
                string strConnString = ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString.Replace("TRAN_", "COMI_");

                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
                {

                    mySqlConnection.Open();

                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                    // SqlTransaction tran = null;

                    mySqlCommand.Connection = mySqlConnection;

                    // datos del agente y del usuario 
                    mySqlCommand.CommandText = @" SELECT [prdId] ,[prdIdExterno],[prdNombre]
                    FROM [dbo].[KmmProducto] WITH(NOLOCK)";

                    using (var reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            tempProductsCommi = new List<ProductCommi>();
                            while (reader.Read())
                            {
                                tempProductsCommi.Add
                                    (new ProductCommi()
                                    {
                                        prdId = Decimal.ToInt32((decimal)reader["prdId"]),
                                        prdIdExterno = Decimal.ToInt32((decimal)reader["prdIdExterno"]),
                                        nombre = reader["prdNombre"].ToString()
                                    });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string Message = "ERROR TRATANDO DE CONSULTAR LOS PRODUCTOS EN LA BASE DE DATOS DE COMISIONES KmmProducto";
                logger.ErrorHigh(() => TagValue.New().Exception(e).Message(Message));
            }

            try
            {
                if (paramList != null && tempProductsCommi != null)
                {

                    returnList = new List<RequestAgent.Product>();
                    foreach (RequestAgent.Product item in paramList)
                    {
                        ProductCommi auxPcomm = tempProductsCommi.Find(x => x.prdIdExterno == item.prdId);
                        if (auxPcomm != null)
                        {
                            returnList.Add(new RequestAgent.Product()
                            {
                                prdId = auxPcomm.prdId,
                                comision = item.comision,
                                nombre = auxPcomm.nombre
                            });
                        }
                    }

                }
            }
            catch (Exception e)
            {
                string Message = "ERROR TRATANDO DE CREAR LA LISTA HOMOLOGA DE LOS PRODUCTOS EN LA BASE DE DATOS DE COMISIONES";
                logger.ErrorHigh(() => TagValue.New().Exception(e).Message(Message));
            }

            return returnList;
        }


        public static string GetAgentName(string agentLogin)
        {
#if DEBUG
            return "134";
#endif
            string agentName = "";

            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TRAN_DB"].ConnectionString);
            try
            {
                var query =
                    String.Format(
                        (@"   SELECT ISNULL(a.[age_razonsocial],' ') 'LegalName'                                   
                              FROM [dbo].[Agente] a with (NOLOCK)
                              join [dbo].[Usuario] u with (NOLOCK) on a.age_id=u.age_id
                              join [dbo].[Acceso] ac with (NOLOCK) on ac.usr_id=u.usr_id
                            WHERE ac.acc_login='{0}' "), agentLogin);

                sqlConnection.Open();

                var command = new SqlCommand(query, sqlConnection);
                agentName = (string)command.ExecuteScalar();
            }
            catch (Exception e)
            {
                logger.ErrorLow(() => TagValue.New().Exception(e).Message("Error trantando de consultar el nombre de la agencia en TRAN Login::>" + agentLogin).Tag("Funcion").Value("GetAgentName"));
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return (agentName);
        }

        public static decimal? GetTotalDebt(string agentId)
        {
            try
            {
                var bandera = false;

                bool.TryParse(ConfigurationManager.AppSettings["CHECK_MO"] ?? "false", out bandera);

                if (!bandera)
                    return null;

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //byte[] cred = UTF8Encoding.UTF8.GetBytes(String.Concat(ConfigurationManager.AppSettings["ShopifyAPIKey"], ":", ConfigurationManager.AppSettings["ShopifyAPIPassword"]));
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(cred));

                string moURL = ConfigurationManager.AppSettings["MO_URL"];
                Loans loans = new Loans();
                HttpResponseMessage response = new HttpResponseMessage();
                string data = "";

                logger.InfoLow(String.Concat(LOG_PREFIX, " [SEND-DATA] agentId=", agentId));

                //response = client.GetAsync(String.Format("http://internal-movilway-alpha-mo-services-inter-1375665364.us-east-1.elb.amazonaws.com/customers/{0}/loans?type_id=2", agentId)).GetAwaiter().GetResult();
                response = client.GetAsync(String.Format(String.Concat(moURL, "/customers/{0}/loans?type_id=2&status=ACTIVE"), agentId)).GetAwaiter().GetResult();
                logger.InfoLow(String.Concat(LOG_PREFIX, " [RESPONSE-CODE] HttpStatusCode: ", (int)response.StatusCode));


                data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    JavaScriptSerializer JSserializer = new JavaScriptSerializer();
                    loans = JSserializer.Deserialize<Loans>(data);

                    logger.InfoLow(String.Concat(LOG_PREFIX, " [RECEIVED-DATA] ", data));

                    if (loans.loans.Count() > 0)
                        return loans.loans.Where(l => l.status == "ACTIVE").Sum(l => l.total_debt_with_taxes);
                    else
                        return 0;
                }
                else
                {
                    logger.InfoLow(String.Concat(LOG_PREFIX, " [RECEIVED-DATA] ", data));

                    return 0;
                }
            }
            catch (Exception ex)
            {
                logger.ErrorLow(String.Concat(LOG_PREFIX, " [RECEIVED-DATA] Error recibiendo la data. ", ex.InnerException));
                return 0;
            }

        }
    }


    public class Loans
    {
        public Loan[] loans { get; set; }
    }

    public class Loan
    {
        public string loan_id { get; set; }
        public decimal amount { get; set; }
        public string contract_version { get; set; }
        public decimal interest_rate { get; set; }
        public decimal interest_with_taxes { get; set; }
        public string status { get; set; }
        public decimal total_paid_amount { get; set; }
        public decimal total_paid_amount_with_taxes { get; set; }
        public decimal total_capital_paid { get; set; }
        public decimal total_interest_paid { get; set; }
        public decimal capital_debt { get; set; }
        public decimal interest_debt { get; set; }
        public decimal total_debt { get; set; }
        public decimal total_debt_with_taxes { get; set; }
        public decimal potential_debt { get; set; }
        public DateTime? taken_at { get; set; }
        public DateTime? maximum_payment_date { get; set; }
        public DateTime? completed_at { get; set; }
    }


    public class RequestAgent
    {
        /// <summary>
        /// Constructor por defecto para evitar valres nulos
        /// </summary>
        public RequestAgent()
        {
            //evitar valores nulos
            productos = new List<RequestAgent.Product>();

            usr_nombre = string.Empty;
            usr_apellido = string.Empty;
            usr_tipo_doc = string.Empty;
            usr_num_doc = string.Empty;
            usr_domicilio = string.Empty;
            usr_telLaboral = string.Empty;
            usr_telPersonal = string.Empty;
            usr_telCelular = string.Empty;
            acc_login = string.Empty;
            second_acc_login = string.Empty;
            third_acc_login = string.Empty;
            second_acc_login = string.Empty;
            second_acc_password = string.Empty;
            third_acc_login = string.Empty;
            third_acc_password = string.Empty;
            age_nombre = string.Empty;
            age_direccion = string.Empty;
            age_razonsocial = string.Empty;
            age_cuit = string.Empty;
            age_tel = string.Empty;
            age_email = string.Empty;
            age_cel = string.Empty;
            age_contacto = string.Empty;
            age_observaciones = string.Empty;
            age_estado = string.Empty;
            age_tipo = string.Empty;
            age_autenticaterminal = string.Empty;
            age_prefijosrest = string.Empty;
            age_pdv = string.Empty;
            age_entrecalles = string.Empty;
            age_comisionadeposito = string.Empty;
            autorizacionAutomatica = string.Empty;
            autorizacionAutomaticaMontoDiario = string.Empty;
            generacionAutomatica = string.Empty;
            limiteCredito = string.Empty;
            montoMaximoPorPedido = string.Empty;
            montoMinimoPorPedido = string.Empty;
            pedidoMaximoMensual = string.Empty;
            quitaAutomatica = string.Empty;
            recargaAsincronica = string.Empty;
            roles_id = string.Empty;
            acc_cambiopassword = string.Empty;
            second_acc_cambiopassword = string.Empty;
            third_acc_cambiopassword = string.Empty;
        }

        public decimal age_id { get; set; }

        public string usr_nombre { get; set; }
        public string usr_apellido { get; set; }
        public string usr_tipo_doc { get; set; }
        public string usr_num_doc { get; set; }
        public string usr_domicilio { get; set; }
        public string usr_telLaboral { get; set; }
        public string usr_telPersonal { get; set; }
        public string usr_telCelular { get; set; }
        public decimal ciu_id { get; set; }
        public decimal pro_id { get; set; }
        public decimal pai_id { get; set; }
        public string usr_barrio { get; set; }
        public string usr_email { get; set; }

        public bool usr_administrador { get; set; }

        public decimal usr_id { get; set; }

        public decimal tac_id { get; set; }
        public decimal tac_description { get; set; }
        public string acc_login { get; set; }
        public string acc_password { get; set; }

        //segundo usuario
        //acceso segundo usuario
        public decimal second_tac_id { get; set; }
        public decimal second_tac_description { get; set; }
        public string second_acc_login { get; set; }
        public string second_acc_password { get; set; }
        //secundo acceso segundo usuario 
        //indica si el segundo acceso al segundo usuario esta habilitado
        public bool av_sc_ac_secondUser { get; set; }
        public decimal third_tac_id { get; set; }
        public decimal third_tac_description { get; set; }
        public string third_acc_login { get; set; }
        public string third_acc_password { get; set; }

        /*
        /// <summary>
        /// Valida las propiedades del objeto
        /// con las reglas de negocio
        /// </summary>
        public void ValidateData()
        {


            //validar segundo accesso segundo usuario
            if (av_sc_ac_secondUser)
            {
                if(third_tac_id <= 0m ||
                  string.IsNullOrEmpty(third_acc_login) ||
                   string.IsNullOrEmpty(third_acc_password))
                {
                    throw new Exception("LOS DATOS DEL SEGUNDO ACCESO PARA EL SEGUNDO USUARIO ESTAN INCOMPLETOS");
                }

                if (third_tac_id == second_tac_id ||
                    // SIN TENER ENCUENTA mayusculas
                third_acc_login.Equals(second_acc_login) )
                {
                    throw new Exception("LOS DATOS DEL SEGUNDO ACCESO (TIPO O LOGIN) PARA EL SEGUNDO USUARIO  NO PUEDEN SER IGUALES AL DE PRIMER ACCESO");
                }
            }
        }
        */

        // public decimal age_id_sup { get; set; }
        public string age_nombre { get; set; }
        public decimal age_ciu_id { get; set; }
        public string age_direccion { get; set; }
        public string age_razonsocial { get; set; }
        public string age_cuit { get; set; }
        public string age_tel { get; set; }
        public string age_email { get; set; }
        public string age_cel { get; set; }
        public string age_contacto { get; set; }
        public string age_observaciones { get; set; }
        public string age_estado { get; set; }
        public decimal age_subNiveles { get; set; }
        public string age_tipo { get; set; }
        public string age_autenticaterminal { get; set; }
        public string age_prefijosrest { get; set; }
        public decimal usr_id_modificacion { get; set; }
        public string age_pdv { get; set; }
        public string age_entrecalles { get; set; }
        public decimal? ct_id { get; set; }
        public decimal? ta_id { get; set; }
        public decimal? sa_id { get; set; }
        public string age_comisionadeposito { get; set; }
        public decimal age_montocomision { get; set; }
        //public string att_dominio { get; set; }
        public string autorizacionAutomatica { get; set; }
        public string autorizacionAutomaticaMontoDiario { get; set; }
        public string generacionAutomatica { get; set; }
        public string limiteCredito { get; set; }
        public string montoMaximoPorPedido { get; set; }
        public string montoMinimoPorPedido { get; set; }
        public string pedidoMaximoMensual { get; set; }
        public string quitaAutomatica { get; set; }
        public string recargaAsincronica { get; set; }
        public string roles_id { get; set; }

        //campos para exigir cambio de password en los dos accesos
        public string acc_cambiopassword { get; set; }
        public DateTime acc_validityDate { get; set; }

        public string second_acc_cambiopassword { get; set; }
        public DateTime second_acc_validityDate { get; set; }

        public string third_acc_cambiopassword { get; set; }
        public DateTime third_acc_validityDate { get; set; }


        public decimal grpId { get; set; }
        public bool comisionporventa { get; set; }

        public List<Product> productos { get; set; }

        public class Product
        {
            public int prdId { get; set; }
            public decimal comision { get; set; }
            public string nombre { get; set; }
        }


        // atributos que indican si selecciono una comision 
        // para todos los productos

        public bool impactOnMoneyBag { get; set; }

        public decimal commissionPercentage { get; set; }




        public decimal age_id_sup { get; set; }


        public decimal age_modificacion { get; set; }
    }

    public class ResponseAgent
    {
        public bool result { get; set; }
        public string autorization { get; set; }
        public string response_code { get; set; }
        public string message { get; set; }
        public int AgeId { get; set; }
    }

    public class ProductCommi
    {
        public int prdId { get; set; }
        public decimal prdIdExterno { get; set; }
        public string nombre { get; set; }
    }
}