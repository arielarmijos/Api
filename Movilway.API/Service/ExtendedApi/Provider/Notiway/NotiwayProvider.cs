using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.Provider.Notiway;
using Movilway.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Notiway;
using System.Configuration;
using System.Runtime.Caching;
using System.Data.Objects.SqlClient;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;
using System.Data;
using System.Threading;
using System.Data.SqlClient;

namespace Movilway.API.Service.ExtendedApi.Provider.Notiway
{
    /// <summary>
    /// Clase principal de Notiway en la cual se implementa la logica necesaria para dar respuesta
    /// a los servicios disponibles en la interfaz
    /// </summary>
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Notiway, ServiceName = ApiServiceName.Notiway)]
    public class NotiwayProvider : AGenericPlatformAuthentication
    {
        /// <summary>
        /// ID que define los tipos de mensajes urgentes/popup
        /// </summary>
        private static readonly int UrgentPopupMessageTypeId = 1;

        /// <summary>
        /// ID que define el tipo de acceso POS
        /// </summary>
        private static readonly int PosDeviceTypeId = 6;

        /// <summary>
        /// ID que define el tipo de acceso POSWEB
        /// </summary>
        private static readonly int PosWebDeviceTypeId = 12;

        /// <summary>
        /// Variable para almacenar todos los mensajes de retorno de los diferentes llamados
        /// </summary>
        private ErrorMessagesMnemonics errorMessage = ErrorMessagesMnemonics.None;

        #region QuerySelect
        private static readonly String QUERY_SELECT = @"SET NOCOUNT ON;

        -- Parameters
        DECLARE @CountryId INT = @CountryP;
        DECLARE @PlatformId INT = @PlatformP;
        DECLARE @Login VARCHAR(20) = @LoginP;
        DECLARE @DeviceTypeId INT = @DeviceTypeP;
        DECLARE @OnlyUnread BIT = @OnlyUnreadP;
        -- END Parameters

        -- CurrentDay Info
        DECLARE @UserTimeZone FLOAT = (SELECT TimeZone FROM dbo.Country WITH(READUNCOMMITTED) WHERE CountryId = @CountryId);
        DECLARE @CurrentDateTime DATETIME = DATEADD(MINUTE, @UserTimeZone * 60, GETUTCDATE());
        DECLARE @CurrentTime TIME = CAST(@CurrentDateTime AS TIME);
        DECLARE @CurrentDate DATE = CAST(@CurrentDateTime AS DATE);
        SET DATEFIRST 1;
        DECLARE @DayOfWeek CHAR(1) = (CASE WHEN DATEPART(WEEKDAY, @CurrentDate) = 7 THEN 0 ELSE DATEPART(WEEKDAY, @CurrentDate) END);
        -- END CurrentDay Info

        -- User Info
        DECLARE @UserId INT = NULL;
        DECLARE @ProvinceId INT = NULL;
        DECLARE @CityId INT = NULL;
        DECLARE @BranchId INT = NULL;
        DECLARE @BranchLineage VARCHAR(255) = NULL;

        SELECT
             @UserId = U.UserId
            ,@ProvinceId = U.ProvinceId
            ,@CityId = U.CityId
            ,@BranchId = U.BranchId
            ,@BranchLineage = '.' + B.Lineage + '.'
        FROM
            dbo.Access AS A WITH(READUNCOMMITTED)
            JOIN dbo.[User] AS U WITH(READUNCOMMITTED) ON (U.UserId = A.UserId AND U.PlatformId = A.PlatformId AND U.CountryId = A.CountryId)
            JOIN dbo.Branch AS B WITH(READUNCOMMITTED) ON (B.BranchId = U.BranchId AND B.PlatformId = U.PlatformId AND B.CountryId = U.CountryId)
        WHERE
            A.AccessTypeId = @DeviceTypeId
            AND A.PlatformId = @PlatformId
            AND A.CountryId = @CountryId
            AND A.[Login] = @Login
        ;
        -- END User Info

        -- Messages
        WITH AvailableMsgs AS (
            SELECT
                 M.MessageId
                ,M.Title
                ,M.Abstract
                ,M.[MessageTypeId]
                ,M.Detail
                ,M.ImageURL
                ,MS.MessageScheduleId
                ,MS.EndDate AS ExpirationDate
                ,(SELECT COUNT(1) FROM dbo.MessageAccessTypes WITH(READUNCOMMITTED) WHERE MessageId = M.MessageId) AS FilterAccessTypes
                ,(SELECT COUNT(1) FROM dbo.MessageProviders WITH(READUNCOMMITTED) WHERE MessageId = M.MessageId) AS FilterProviders
                ,(SELECT COUNT(1) FROM dbo.MessageProducts WITH(READUNCOMMITTED) WHERE MessageId = M.MessageId) AS FilterProducts
            FROM
                dbo.[Message] AS M WITH(READUNCOMMITTED)
                JOIN dbo.MessageSchedule AS MS WITH(READUNCOMMITTED) ON (MS.MessageId = M.MessageId AND MS.StatusId = 'AC')
            WHERE
                M.StatusId = 'AC'
                AND (M.CountryId IS NULL OR M.CountryId = @CountryId)
                AND (M.PlatformId IS NULL OR M.PlatformId = @PlatformId)
                AND (M.ProvinceId IS NULL OR M.ProvinceId = @ProvinceId)
                AND (M.CityId IS NULL OR M.CityId = @CityId)
                AND (
                    M.BranchId IS NULL
                    OR (M.BranchId IS NOT NULL AND M.IncludeSubBranches = 0 AND M.BranchId = @BranchId)
                    OR (M.BranchId IS NOT NULL AND M.IncludeSubBranches = 1 AND CHARINDEX('.' + CAST(M.BranchId AS VARCHAR(255)) + '.', @BranchLineage) > 0)
                )
                AND (
                    M.MessageDestination = 'TODOS'
                    OR (M.MessageDestination = 'POS' AND @DeviceTypeId = 6)
                    OR (M.MessageDestination = 'POS WEB' AND @DeviceTypeId = 12)
                ) 
                AND MS.StartDate <= @CurrentDate
                AND MS.EndDate >= @CurrentDate 
                AND MS.StartTime <= @CurrentTime
                AND MS.EndTime >= @CurrentTime
                AND CHARINDEX(@DayOfWeek, MS.[Days]) > 0
        ),
        Msgs AS (
            SELECT
                M.*
            FROM
                AvailableMsgs AS M
            WHERE
                (FilterAccessTypes = 0 OR EXISTS(
                    SELECT 1 FROM dbo.MessageAccessTypes WITH(READUNCOMMITTED)
                    WHERE MessageId = M.MessageId AND AccessTypeId = @DeviceTypeId
                ))
                AND (FilterProviders = 0 OR EXISTS(
                    SELECT 1 FROM dbo.BranchProducts WITH(READUNCOMMITTED)
                    WHERE
                        CountryId = @CountryId AND PlatformId = @PlatformId AND BranchId = @BranchId
                        AND ProviderId IN (
                            SELECT ProviderId FROM dbo.MessageProviders WITH(READUNCOMMITTED)
                            WHERE MessageId = M.MessageId
                        )
                ))
                AND (FilterProducts = 0 OR EXISTS(
                    SELECT 1 FROM dbo.BranchProducts WITH(READUNCOMMITTED)
                    WHERE
                        CountryId = @CountryId AND PlatformId = @PlatformId AND BranchId = @BranchId
                        AND ProductId IN (
                            SELECT ProductId FROM dbo.MessageProducts WITH(READUNCOMMITTED)
                            WHERE MessageId = M.MessageId AND CountryId = @CountryId AND PlatformId = @PlatformId
                        )
                ))
        )
        SELECT
            M.*, CAST(ISNULL(A.FirstDeliveryDateTime, @CurrentDate) AS DATE) AS FirstDeliveryDate, CAST(A.LastReadDateTime AS DATE) AS LastReadDate
        FROM
            Msgs AS M
            LEFT JOIN dbo.AuditMessage AS A WITH(READUNCOMMITTED) ON (A.MessageId = M.MessageId AND A.ScheduleId = M.MessageScheduleId AND A.CountryId = @CountryId AND A.PlatformId = @PlatformId AND A.UserId = @UserId)
        WHERE
            @OnlyUnread = 0
            OR (@OnlyUnread = 1 AND A.LastReadDateTime IS NULL)
        ;
        -- END Messages";
        #endregion

        #region QueryAuditBulk
        private static readonly String QUERY_AUDIT_BULK = @"-- Parameters
        DECLARE @CountryId INT = @CountryP;
        DECLARE @PlatformId INT = @PlatformP;
        DECLARE @Login VARCHAR(20) = @LoginP;
        DECLARE @ScheduleList VARCHAR(500) = @ScheduleListP;
        DECLARE @DeviceTypeId INT = @DeviceTypeP;
        DECLARE @MarkAsRead BIT = @MarkAsReadP;
        -- END Parameters

        -- CurrentDay Info
        DECLARE @UserTimeZone FLOAT = (SELECT TimeZone FROM dbo.Country WITH(READUNCOMMITTED) WHERE CountryId = @CountryId);
        DECLARE @CurrentDateTime DATETIMEOFFSET = SWITCHOFFSET(CONVERT(DATETIMEOFFSET, GETUTCDATE()), @UserTimeZone * 60);
        -- END CurrentDay Info

        -- User Info
        DECLARE @UserId INT = NULL;
        DECLARE @BranchId INT = NULL;

        SELECT
             @UserId = U.UserId
            ,@BranchId = U.BranchId
        FROM
            dbo.Access AS A WITH(READUNCOMMITTED)
            JOIN dbo.[User] AS U WITH(READUNCOMMITTED) ON (U.UserId = A.UserId AND U.PlatformId = A.PlatformId AND U.CountryId = A.CountryId)
        WHERE
            A.AccessTypeId = @DeviceTypeId
            AND A.PlatformId = @PlatformId
            AND A.CountryId = @CountryId
            AND A.[Login] = @Login
        ;
        -- END User Info

        -- Audit Messages
        WITH ScheduleIds AS (
            SELECT t.value('.', 'INT') AS ScheduleId 
            FROM (
                SELECT CAST('<root><id>' + REPLACE(ISNULL(@ScheduleList, ''), ',', '</id><id>') + '</id></root>' AS XML) AS ScheduleListXml
            ) AS schedules 
            CROSS APPLY schedules.ScheduleListXml.nodes('/root/id') AS x(t) 
            WHERE @ScheduleList IS NOT NULL AND LEN(LTRIM(RTRIM(@ScheduleList))) > 0 AND LEN(LTRIM(RTRIM(@ScheduleList))) <> '-1'
        ),
        Msgs AS (
            SELECT
                MessageId, MessageScheduleId AS ScheduleId
            FROM
                dbo.MessageSchedule WITH(READUNCOMMITTED)
            WHERE
                MessageScheduleId IN (
                    SELECT ScheduleId FROM ScheduleIds
                )
        )
        MERGE dbo.AuditMessage AS tgt
        USING Msgs AS src
        ON (tgt.MessageId = src.MessageId AND tgt.[ScheduleId] = src.[ScheduleId] AND tgt.CountryId = @CountryId AND tgt.PlatformId = @PlatformId AND tgt.UserId = @UserId)
        WHEN MATCHED THEN UPDATE SET 
             tgt.[DeliveryCounter] += 1
            ,tgt.[LastDeliveryDateTime] = @CurrentDateTime
            ,tgt.[ReadCounter] += (CASE WHEN @MarkAsRead = 1 THEN 1 ELSE 0 END)
            ,tgt.[LastReadDateTime] = (CASE WHEN @MarkAsRead = 1 THEN @CurrentDateTime ELSE tgt.[LastReadDateTime] END)
            ,tgt.[FirstReadDateTime] = (CASE WHEN (@MarkAsRead = 1 AND tgt.[FirstReadDateTime] IS NULL) THEN @CurrentDateTime ELSE tgt.[FirstReadDateTime] END)
        WHEN NOT MATCHED THEN INSERT 
            ([MessageId],[ScheduleId],[CountryId],[PlatformId],[BranchId]
             ,[ParentBranchId],[UserId],[DeliveryCounter],[FirstDeliveryDateTime]
             ,[LastDeliveryDateTime],[ReadCounter],[FirstReadDateTime],[LastReadDateTime])
             VALUES
             (src.MessageId,src.[ScheduleId],@CountryId,@PlatformId,@BranchId
             ,NULL,@UserId,1,@CurrentDateTime,@CurrentDateTime
             ,(CASE WHEN @MarkAsRead = 1 THEN 1 ELSE 0 END)
             ,(CASE WHEN @MarkAsRead = 1 THEN @CurrentDateTime ELSE NULL END)
             ,(CASE WHEN @MarkAsRead = 1 THEN @CurrentDateTime ELSE NULL END))
        ;
        -- END Audit Messages";
        #endregion

        #region QueryAuditRead
        private static readonly String QUERY_AUDIT_READ = @"-- Parameters
        DECLARE @CountryId INT = @CountryP;
        DECLARE @PlatformId INT = @PlatformP;
        DECLARE @Login VARCHAR(20) = @LoginP;
        DECLARE @ScheduleId INT = @ScheduleP;
        DECLARE @DeviceTypeId INT = @DeviceTypeP;
        -- END Parameters

        -- CurrentDay Info
        DECLARE @UserTimeZone FLOAT = (SELECT TimeZone FROM dbo.Country WITH(READUNCOMMITTED) WHERE CountryId = @CountryId);
        DECLARE @CurrentDateTime DATETIMEOFFSET = SWITCHOFFSET(CONVERT(DATETIMEOFFSET, GETUTCDATE()), @UserTimeZone * 60);
        -- END CurrentDay Info

        -- User Info
        DECLARE @UserId INT = NULL;
        DECLARE @BranchId INT = NULL;

        SELECT
             @UserId = A.UserId
        FROM
            dbo.Access AS A WITH(READUNCOMMITTED)
        WHERE
            A.AccessTypeId = @DeviceTypeId
            AND A.PlatformId = @PlatformId
            AND A.CountryId = @CountryId
            AND A.[Login] = @Login
        ;
        -- END User Info

        -- Audit Messages
        WITH Msgs AS (
            SELECT
                MessageId, MessageScheduleId AS ScheduleId
            FROM
                dbo.MessageSchedule WITH(READUNCOMMITTED)
            WHERE
                MessageScheduleId = @ScheduleId
        )
        MERGE dbo.AuditMessage AS tgt
        USING Msgs AS src
        ON (tgt.MessageId = src.MessageId AND tgt.[ScheduleId] = src.[ScheduleId] AND tgt.CountryId = @CountryId AND tgt.PlatformId = @PlatformId AND tgt.UserId = @UserId)
        WHEN MATCHED THEN UPDATE SET 
             tgt.[ReadCounter] += 1
            ,tgt.[LastReadDateTime] = @CurrentDateTime
            ,tgt.[FirstReadDateTime] = (CASE WHEN tgt.[FirstReadDateTime] IS NULL THEN @CurrentDateTime ELSE tgt.[FirstReadDateTime] END)
        ;
        -- END Audit Messages";
        #endregion

        /// <summary>
        /// Inicializacion de la libreria log
        /// </summary>
        static NotiwayProvider()
        {
            logger = LoggerFactory.GetLogger(typeof(NotiwayProvider));
        }

        /// <summary>
        /// Obtiene la lista de mensajes pendientes a ser enviados para el usuario que realiza la peticion
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario</param>
        /// <returns>Un objeto <c>GetNotiwayMessageListResponse</c> que contiene la lista de mensajes pendientes</returns>
        public GetNotiwayNewsListResponse GetNotiwayNewsList(GetNotiwayNewsListRequest request)
        {
            String _methodName = String.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

            LogRequest(request);

            GetNotiwayNewsListResponse response = new GetNotiwayNewsListResponse();
            var sessionId = GetSessionId(request, response, out errorMessage);
            if (errorMessage != ErrorMessagesMnemonics.None)
            {
                return response;
            }

            var countryId = Convert.ToInt32(ConfigurationManager.AppSettings["CountryId"]);
            var platformId = Convert.ToInt32(String.IsNullOrEmpty(request.Platform) ? ConfigurationManager.AppSettings["DefaultPlatform"] : request.Platform);

            if (countryId == 14 && platformId == 1)
            {
                countryId = 1;
                platformId = 4;
            }

            using (var db = new Movilway.API.Data.Notiway.NotiwayEntities())
            {
                ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(_methodName)
                    .Message("[" + sessionId + "] " + "Obteniendo lista de mensajes ...")
                );

                var list = db.Database.SqlQuery<NotiwayQuerySelectItem>(
                    QUERY_SELECT,
                    new SqlParameter("@CountryP", countryId),
                    new SqlParameter("@PlatformP", platformId),
                    new SqlParameter("@LoginP", request.AuthenticationData.Username),
                    new SqlParameter("@DeviceTypeP", request.DeviceType),
                    new SqlParameter("@OnlyUnreadP", request.OnlyUnread ? "1" : "0")
                );

                foreach (var item in list)
                {
                    NotiwayNews aux = new NotiwayNews()
                    {
                        NewsId = item.MessageId
                        ,
                        ScheduleId = item.MessageScheduleId
                        ,
                        ExpirationDate = item.ExpirationDate.ToString("yyyy-MM-dd")
                        ,
                        Title = item.Title
                        ,
                        Abstract = item.Abstract
                        ,
                        Type = item.MessageTypeId
                        ,
                        FirstDeliveryDate = item.FirstDeliveryDate.ToString("yyyy-MM-dd")
                        ,
                        LastReadDate = ((item.LastReadDate != null && item.LastReadDate.HasValue) ? (item.LastReadDate.Value.ToString("yyyy-MM-dd")) : "")
                    };

                    if (request.DeviceType == PosWebDeviceTypeId)
                    {
                        // POS WEB
                        aux.Detail = String.IsNullOrEmpty(item.Detail) ? "" : item.Detail;
                        aux.ImageURL = String.IsNullOrEmpty(item.ImageURL) ? "" : item.ImageURL;
                    }
                    response.NewsList.Add(aux);
                }

                ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(_methodName)
                    .Message("[" + sessionId + "] " + "Total mensajes a ser enviados: " + response.NewsList.Count)
                );

                try
                {
                    ThreadPool.QueueUserWorkItem(unused => writeAudit(sessionId, countryId, platformId, request.AuthenticationData.Username, request.DeviceType, request.MarkAsReaded, response.NewsList));
                }
                catch (Exception e)
                {
                    ProviderLogger.ExceptionLow(() => TagValue.New()
                        .MethodName(_methodName)
                        .Message("Error iniciando hilo de escritura mensajes de auditoria")
                        .Exception(e)
                    );
                }
            }

            response.ResponseCode = 0;
            response.ResponseMessage = "Exito";
            response.Quantity = response.NewsList.Count;

            LogResponse(response);
            return response;
        }

        /// <summary>
        /// Escribe en base de datos la auditoria para cada mensaje enviado
        /// </summary>
        /// <param name="SessionId">ID de la sesion que genero los mensajes (solo para log)</param>
        /// <param name="CountryId">ID del pais al que pertenece el usuario</param>
        /// <param name="PlatformId">ID de la plataforma al que pertenece el usuario</param>
        /// <param name="Login">Login del usuario</param>
        /// <param name="DeviceType">ID del tipo de acceso</param>
        /// <param name="MarkAsReaded">Incrementar el contador de lecturas para el mensaje automaticamente</param>
        /// <param name="messages">Lista de mensajes enviados que se escribiran en los datos de auditoria</param>
        private void writeAudit(String SessionId, int CountryId, int PlatformId, String Login, int DeviceType, bool MarkAsReaded, List<NotiwayNews> messages)
        {
            String _methodName = String.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

            if (messages != null && messages.Count > 0)
            {
                ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(_methodName)
                    .Message("[AUDITORIA] [" + SessionId + "] " + "Total mensajes a ser actualizados en auditoria: " + messages.Count)
                );

                try
                {
                    using (var db = new Movilway.API.Data.Notiway.NotiwayEntities())
                    {
                        db.Database.ExecuteSqlCommand(
                            QUERY_AUDIT_BULK,
                            new SqlParameter("@CountryP", CountryId),
                            new SqlParameter("@PlatformP", PlatformId),
                            new SqlParameter("@LoginP", Login),
                            new SqlParameter("@ScheduleListP", String.Join(",", messages.Select(s => s.ScheduleId).ToList())),
                            new SqlParameter("@DeviceTypeP", DeviceType),
                            new SqlParameter("@MarkAsReadP", MarkAsReaded ? "1" : "0")
                        );
                    }

                    ProviderLogger.InfoLow(() => TagValue.New()
                        .MethodName(_methodName)
                        .Message("[AUDITORIA] [" + SessionId + "] " + "Hecho")
                    );
                }
                catch (Exception e)
                {
                    ProviderLogger.ExceptionLow(() => TagValue.New()
                       .MethodName(_methodName)
                        .Message("[AUDITORIA] [" + SessionId + "] " + "Error actualizando auditoria de mensajes")
                       .Exception(e)
                    );
                }
            }
        }

        /// <summary>
        /// Notifica a Notiway que un mensaje fue leido
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario y el Id del mensaje</param>
        public void NotiwayNewsReadNotification(NotiwayNewsReadNotificationRequest request)
        {
            String _methodName = String.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

            LogRequest(request);

            var sessionId = GetSessionId(request, out errorMessage);

            if (errorMessage != ErrorMessagesMnemonics.None)
            {
                return;
            }

            var countryId = Convert.ToInt32(ConfigurationManager.AppSettings["CountryId"]);
            var platformId = Convert.ToInt32(String.IsNullOrEmpty(request.Platform) ? ConfigurationManager.AppSettings["DefaultPlatform"] : request.Platform);

            if (countryId == 14 && platformId == 1)
            {
                countryId = 1;
                platformId = 4;
            }

            try
            {
                using (var db = new Movilway.API.Data.Notiway.NotiwayEntities())
                {
                    ProviderLogger.InfoLow(() => TagValue.New()
                        .MethodName(_methodName)
                        .Message("[" + sessionId + "] " + "Actualizando registro ...")
                    );

                    db.Database.ExecuteSqlCommand(
                        QUERY_AUDIT_READ,
                        new SqlParameter("@CountryP", countryId),
                        new SqlParameter("@PlatformP", platformId),
                        new SqlParameter("@LoginP", request.AuthenticationData.Username),
                        new SqlParameter("@ScheduleP", request.ScheduleId),
                        new SqlParameter("@DeviceTypeP", request.DeviceType)
                    );
                }

                ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(_methodName)
                    .Message("[" + sessionId + "] " + "Hecho")
                );
            }
            catch (Exception e)
            {
                ProviderLogger.ExceptionLow(() => TagValue.New()
                   .MethodName(_methodName)
                    .Message("[AUDITORIA] [" + sessionId + "] " + "Error actualizando auditoria de mensaje")
                   .Exception(e)
                );
            }
        }
    }
}
