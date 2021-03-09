// <copyright file="CreditNoteProvider.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Web;


    using Movilway.API.Service.ExtendedApi.DataContract;
    using Movilway.API.Service.ExtendedApi.DataContract.Common;
    using Movilway.Logging;
    using Movilway.API.KinacuWebService;
    using Movilway.API.Utils.Models.Kinacu;

    /// <summary>
    /// Implementación método CreditNote
    /// </summary>
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.CreditNote)]
    public class CreditNoteProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(CreditNoteProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, string sessionID)
        {
            string providerName = "CreditNoteProvider";
            CreditNoteRequestBody request = requestObject as CreditNoteRequestBody;
            CreditNoteResponseBody response = new CreditNoteResponseBody()
            {
                ResponseCode = 90,
                ResponseMessage = "Error inesperado",
                TransactionID = 0
            };

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[" + providerName + "] [SEND-DATA] CreditNoteParameters {AgentId=" + request.AgentId + ",Amount=" + request.Amount + ",Comment=" + request.Comment + ",SupportDocument=" + request.SupportDocument + "}");

            if (sessionID.Equals("0"))
            {
                response.ResponseMessage = "Error de sesion";
                logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[" + providerName + "] [RECV-DATA] CreateAgentResult {response={" + response + "}}");
                return response;
            }

            API.Utils.KinacuDb db = new API.Utils.KinacuDb();

            do
            {
                SqlConnection connection = Movilway.API.Utils.Database.GetKinacuDbConnection();
                SqlTransaction trx = null;
                bool success = false;

                try
                {
                    connection.Open();
                    db.db = connection;

                    InfoAgente originador = db.GetInfoAgente(null, request.AuthenticationData.Username);
                    InfoAgente agencia = db.GetInfoAgente(request.AgentId);

                    if (agencia == null || agencia.OwnerId != originador.BranchId)
                    {
                        response.ResponseMessage = "Agencia inválida";
                        break;
                    }

                    trx = connection.BeginTransaction();
                    db.databaseTransaction = trx;

                    string comentario = string.Concat(
                        "Agente ",
                        originador.BranchId,
                        ": ",
                        string.IsNullOrEmpty(request.SupportDocument) ? string.Empty : string.Concat("Comp. ", request.SupportDocument, "-"),
                        request.Comment);

                    int id = db.CrearMovimientoCuentaCorriente((int)agencia.BranchId, request.Amount, comentario, false, Core.cons.TtrIdMovimientoCuentaCorrienteNotaCredito);
                    if (id != 0)
                    {
                        success = true;
                        response.ResponseCode = 0;
                        response.ResponseMessage = string.Empty;
                        response.TransactionID = id;

                        db.InsertarMovimientoAuditoria((int)originador.BranchId, null, comentario, id, "FINANCIERO", "DECREMENTO");

                        trx.Commit();
                        // trx.Rollback();
                        trx.Dispose();
                        trx = null;
                        db.databaseTransaction = null;
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
                    if (!success && trx != null)
                    {
                        try
                        {
                            trx.Rollback();
                        }
                        catch (Exception)
                        {
                        }
                    }

                    try
                    {
                        connection.Close();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            while (false);

            logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[" + providerName + "] [RECV-DATA] CreateAgentResult {response={" + response + "}}");
            return response;
        }
    }
}
