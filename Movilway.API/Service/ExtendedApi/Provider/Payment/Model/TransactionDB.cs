using Movilway.API.Service.ExtendedApi.DataContract.Payment;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.Payment.Model
{
    public class TransactionDB
    {

        private readonly string QUERY_UPDATETRANSACTION = @"if len(ltrim(rtrim(@message)))>0
	begin
		set @message='|'+@message;
	end

if @codigoExterno='0'
	begin
		update [Transaccion] set EstadoTransaccion=@estado, HistorialEstados=HistorialEstados+','+convert(varchar(25), @estado),Mensaje=Mensaje+@message where Codigo_transaccion=@codigoTransaccion and EstadoTransaccion!=0;
	end
else
	begin
		update [Transaccion] set EstadoTransaccion=@estado, HistorialEstados=HistorialEstados+','+convert(varchar(25), @estado),CodigoDeTransaccionExterno=@codigoExterno,Mensaje=Mensaje+@message where Codigo_transaccion=@codigoTransaccion and EstadoTransaccion!=0;
	end

	select @@ROWCOUNT";

        private readonly string QUERY_UPDATETRANSACTIONOK = "update [Transaccion] set EstadoTransaccion=@estado, HistorialEstados=HistorialEstados+','+convert(varchar(25), @estado),Mensaje=Mensaje+@message where Codigo_transaccion=@codigoTransaccion;";

        private readonly string QUERY_CREATETRANASCTION = @"declare @codigoCliente int
select @codigoCliente=Codigo_cliente from clientes where codigo_externo=@cliente and Fuente=@fuente
INSERT INTO [dbo].[Transaccion] ([Codigo_pais] ,[CodigoTipoTransaccion] ,[CodigoDeTransaccionExterno] ,[Cliente] ,[FechaTransaccion] ,[EstadoTransaccion] ,[Mensaje] ,[HistorialEstados],[Monto]) VALUES (@country ,@tipoTran ,@codExterno ,@codigoCliente ,getdate() ,@estadoTransaccion ,'' ,'1',@monto);
select SCOPE_IDENTITY();";

        public int GetTransaction(CrearTransaccionRequest transaction)
        {

            decimal tranId = 0;
            using (Transaction_DB db = new Transaction_DB())
            {

                tranId = db.Database.SqlQuery<decimal>(QUERY_CREATETRANASCTION,
                   new SqlParameter("@country", Convert.ToInt32(ConfigurationManager.AppSettings["CountryID"])),
                   new SqlParameter("@tipoTran", (int)TipoTransaccion.PSE),
                   new SqlParameter("@codExterno", "0"),
                   new SqlParameter("@cliente", Convert.ToInt32(transaction.Client)),
                   new SqlParameter("@estadoTransaccion", (int)TransaccionEstado.Creado),
                   new SqlParameter("@fuente", 1),
                   new SqlParameter("@monto", transaction.TotalConIva)).FirstOrDefault();
            }

            return Convert.ToInt32(tranId);
        }

        public int UpdateTransaction(ActualizarTransaccionRequest request)
        {

            decimal updated = 0;

            using (Transaction_DB db = new Transaction_DB())
            {

                if (request.Estado == Convert.ToInt32(TransaccionEstado.Exitoso))
                {
                    updated = db.Database.SqlQuery<int>(QUERY_UPDATETRANSACTIONOK,
                    new SqlParameter("@estado", request.Estado),
                    new SqlParameter("@codigoTransaccion", request.CodigoTransaccion),
                    new SqlParameter("@message", request.Mensaje)
                    ).FirstOrDefault();

                    
                }
                else {
                    updated = db.Database.SqlQuery<int>(QUERY_UPDATETRANSACTION,
                    new SqlParameter("@estado", request.Estado),
                    new SqlParameter("@codigoTransaccion", request.CodigoTransaccion),
                    new SqlParameter("@codigoExterno", String.IsNullOrEmpty(request.CodigoTransaccionExterno) ? "0" : request.CodigoTransaccionExterno),
                    new SqlParameter("@message", request.Mensaje)
                    ).FirstOrDefault();
                }

                

            }
            return Convert.ToInt32(updated);

        }

    }
}