using System;
using System.Web;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.Logging;
using Movilway.API.Data;
using Movilway.API.KinacuWebService;
using System.Globalization;
using System.Configuration;
using System.Data.SqlClient;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.MonthlyReport)]
    public class MonthlyReportProvider : AKinacuProvider
    {

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetTransactionProvider));
        protected override ILogger ProviderLogger { get { return logger; } }


        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, string sessionID)
        {
            string providerName = "CreditNoteProvider";
            MonthlyReportRequestBody request = requestObject as MonthlyReportRequestBody;
            MonthlyReportResponseBody response = new MonthlyReportResponseBody()
            {
                ResponseCode = 90,
                ResponseMessage = "Error inesperado",
                TransactionID = 0
            };

            if (sessionID.Equals("0"))
            {
                response.ResponseMessage = "Error de sesion";
          
                return response;
            }




            try
            {

                using (SqlConnection connection = Movilway.API.Utils.Database.GetKinacuDbConnection())
                {


                    connection.Open();
                    // db.db = connection;

                    var cmd = connection.CreateCommand();

                    cmd.Parameters.AddWithValue("rep_type", request.ReportType.ToString());
                    cmd.Parameters.AddWithValue("acc_login", request.AuthenticationData.Username);
                    cmd.Parameters.AddWithValue("tac_id", request.DeviceType);
                    cmd.Parameters.AddWithValue("state", "PE");
                    cmd.Parameters.AddWithValue("executiondate", request.ExecutionDate);



                    cmd.CommandText = @"

                    DECLARE @usr_id as decimal(5,0)
                    
                    SELECT @usr_id = [usr_id]
                    FROM [dbo].[Acceso]
                    WHERE [acc_login] = @acc_login  and [tac_id] = @tac_id
                    
                    declare @dateDiff as int = (SELECT  CAST(par_valor as int) FROM [Parametro] WITH(NOLOCK) WHERE par_id ='TimeDifference') 

                   declare @date as datetime =  dateadd(minute,@dateDiff,getdate())
                    
                    INSERT INTO [dbo].[MwReportRequest]
                               ([rep_type]
                               ,[usr_id]
                               ,[state]
                               ,[req_date]
                                ,[executiondate]
                             
                               )
                         VALUES
                               (@rep_type
                               ,CONVERT ( INT ,isnull( @usr_id,0))
                               ,@state
                               ,@date
                               ,@executiondate
                               )
                    
                    
                    SELECT @@IDENTITY
                    
                    
                    ";


                    var result = cmd.ExecuteScalar();

                    logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[ReportProvider] Result " + result);

                    long _tran = 0;
                    if (result != null)
                    {

                        if (!long.TryParse(result.ToString(), out _tran))
                            _tran = 0;

                        response.ResponseCode = 0;
                        response.ResponseMessage = "OK";
                        response.TransactionID = (int)_tran;
                    }

                }
            }
            catch (Exception ex)
            {
                ProviderLogger.ExceptionLow(() => TagValue.New()
                    .MethodName(providerName)
                    .Exception(ex));
            }
            finally
            {
            }

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[" + providerName + "] [RECV-DATA] ReportProviderResult {response={" + response + "}}");
            return response;
        }
    }
}