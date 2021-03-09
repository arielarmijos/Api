// <copyright file="Kinacu.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Utils.Queries
{
    using System;

    /// <summary>
    /// Consultas realizadas contra la base de datos de Kinacu
    /// </summary>
    internal static class Kinacu
    {
        /// <summary>
        /// Consulta hora pais
        /// </summary>
        public static readonly string CalcHourCountry = @"
            DECLARE @DATETIME_COUNTRY AS DATETIME = GETDATE();

            BEGIN TRY
                SELECT @DATETIME_COUNTRY = DATEADD(minute,(SELECT CONVERT(INT, [par_valor]) FROM [Parametro] WITH(READUNCOMMITTED) WHERE [par_id] = 'TimeDifference'), @DATETIME_COUNTRY ); 
            END TRY
            BEGIN CATCH
            END CATCH;
        ";

        /// <summary>
        /// Consulta hora pais
        /// </summary>
        public static readonly string SelectCalcHourCountry = CalcHourCountry + @"
            SELECT @DATETIME_COUNTRY;
        ";

        /// <summary>
        /// Release Kinacu
        /// </summary>
        public static readonly string GetReleaseKinacu = @"
            SELECT par_valor FROM [Parametro] WITH(READUNCOMMITTED) WHERE par_id = 'Release';
        ";

        /// <summary>
        /// Incrementa la secuencia de un objeto dado
        /// </summary>
        public static readonly string UpdateSequence = @"
            DECLARE @newid INT = 0;

            UPDATE Secuencia WITH (ROWLOCK) SET @newid = sec_number = sec_number + 1
            WHERE sec_objectName = @paramValue;

            SELECT @newid;
        ";

        /// <summary>
        /// Informacion deposito pendiente
        /// </summary>
        private static readonly string GetInfoAgenteSelect = @"
            SELECT
                a.[age_id] AS 'BranchId',
                ISNULL(a.[age_id_sup], 0) AS 'OwnerId',
                'NationalId' AS 'NationalIdType',
                a.[age_cuit] AS 'NationalId',
                a.[age_nombre] AS 'Name',
                a.[age_razonsocial] AS 'LegalName',
                a.[age_direccion] AS 'Address',
                ISNULL(a.[age_email], '') AS 'Email',
                CAST(a.[age_subNiveles] AS INT) AS 'SubLevel',
                a.[age_pdv] AS 'Pdv',
                CAST(a.ct_id AS INT) AS TaxCategory,
                CAST(a.sa_id AS INT) AS Segment,
                ISNULL(ct.ciu_id, 0) AS 'CityId',
                ISNULL(ct.ciu_nombre, '') AS 'City',
                ISNULL(pr.pro_id, 0) AS 'ProvinceId',
                ISNULL(pr.pro_nombre, '') AS 'Province'
            FROM
                [dbo].[Agente] AS a WITH (READUNCOMMITTED)
                LEFT JOIN [dbo].[Ciudad] AS ct WITH (READUNCOMMITTED) ON (ct.ciu_id = a.ciu_id)
                LEFT JOIN [dbo].[Provincia] AS pr WITH (READUNCOMMITTED) ON (pr.pro_id = ct.pro_id)
        ";

        /// <summary>
        /// Informacion deposito pendiente
        /// </summary>
        public static readonly string GetInfoAgenteById = GetInfoAgenteSelect + @"
            WHERE
                a.[age_id] = @id;
        ";

        /// <summary>
        /// Informacion deposito pendiente
        /// </summary>
        public static readonly string GetInfoAgenteByLogin = GetInfoAgenteSelect + @"
                JOIN [dbo].[Usuario] AS u WITH (READUNCOMMITTED) ON (a.age_id = u.age_id)
                JOIN [dbo].[Acceso] AS ac WITH (READUNCOMMITTED) ON (ac.usr_id = u.usr_id)
            WHERE
                ac.acc_login = @login;
        ";

        /// <summary>
        /// Informacion deposito pendiente
        /// </summary>
        public static readonly string GetInfoDeposito = @"
            SELECT TOP 1 
                D.depid AS Id,

                AO.age_id AS IdAgenciaOrigen,
                ISNULL(AP.age_id, 0) AS IdAgenciaPadre,
                ISNULL(AA.age_id, 0) AS IdAgenciaAbuelo,

                AO.age_nombre AS NombreAgenciaOrigen,
                ISNULL(AP.age_nombre, '') AS NombreAgenciaPadre,
                ISNULL(AA.age_nombre, '') AS NombreAgenciaAbuelo,

                ISNULL(CO.cubid, 0) AS IdCub,
                ISNULL(CP.cubid, 0) AS IdCubPadre,
                ISNULL(CA.cubid, 0) AS IdCubAbuelo,

                CP.cubnumero AS CubPadreNumero,
                D.depMonto AS Monto,

                ISNULL(AO.usr_id, 0) AS IdUsuario,
                ISNULL(AP.usr_id, 0) AS IdUsuarioPadre,
                ISNULL(AA.usr_id, 0) AS IdUsuarioAbuelo,

                D.depFecha AS FechaDeposito,
                D.depFechaComprobante AS FechaComprobante,
                D.depEstado AS Estado,
                D.depComprobante AS Comprobante,
                D.depComentario AS Comentario,
                (
                    ISNULL(B2.banNombre,'')
                    + ' ' + ISNULL(CP.cubnumero,'')
                    + ' ' + ISNULL(D.depComprobante,'')
                    + ' ' + ISNULL(CONVERT(VARCHAR(10), D.depFechaComprobante, 103), '')
                    + ' ' + ISNULL(CONVERT(VARCHAR(10), D.depFecha, 103), '')
                ) AS ComentarioCompleto 
            FROM
                KfnDeposito D WITH(READUNCOMMITTED) 
                INNER JOIN KfnCuentaBanco CP WITH(READUNCOMMITTED) ON (CP.cubid = D.cubid) 
                INNER JOIN KfnBanco B2 WITH(READUNCOMMITTED) ON (B2.banId = CP.banId) 

                INNER JOIN Agente AO WITH(READUNCOMMITTED) ON (AO.age_id = D.ageIdOrigen) 
                LEFT JOIN Agente AP WITH(READUNCOMMITTED) ON (AP.age_id = AO.age_id_sup) 
                LEFT JOIN Agente AA WITH(READUNCOMMITTED) ON (AA.age_id = AP.age_id_sup) 

                LEFT JOIN KfnCuentaBanco CO WITH(READUNCOMMITTED) ON (CO.banId = CP.banId AND CO.ageId = AO.age_id) 
                LEFT JOIN KfnCuentaBanco CA WITH(READUNCOMMITTED) ON (CA.banId = CP.banId AND CA.ageId = AA.age_id) 
                
            WHERE
                depid = @id
                AND depEstado = @estado;
        ";

        /// <summary>
        /// Obtiene la información de la cuenta corriente para la agencia especificada
        /// </summary>
        public static readonly string GetInfoCuentaCorriente = @"
            SELECT
                c.ctaId AS Id,
                c.ctaSaldo AS Saldo,
                (CASE WHEN lc.ataValor IS NULL THEN CAST(0 AS DECIMAL) ELSE CAST(lc.ataValor AS DECIMAL) END) AS LimiteCredito
            FROM
                KfnCuentaCorriente AS c WITH(READUNCOMMITTED) 
                LEFT JOIN KcrAtributoAgencia AS lc WITH(READUNCOMMITTED) ON (lc.ageid = c.ageId AND lc.attid = 'LimiteCredito')
            WHERE
                c.ageId = @id;
        ";

        /// <summary>
        /// Actualiza la informacion de cuenta corriente
        /// </summary>
        public static readonly string UpdateCuentaCorriente = @"
            UPDATE KfnCuentaCorriente WITH(ROWLOCK)
            SET ctaSaldo = ctaSaldo + @depMonto
            WHERE
                ctaId = @ctaId
                AND ageId = @ageId
                AND ctaSaldo = @ctaSaldo;
        ";

        /// <summary>
        /// Actualiza la informacion de cuenta corriente
        /// </summary>
        public static readonly string UpdateCuentaCorrienteResta = @"
            UPDATE KfnCuentaCorriente WITH(ROWLOCK)
            SET ctaSaldo = ctaSaldo - @depMonto
            WHERE
                ctaId = @ctaId
                AND ageId = @ageId;
        ";

        /// <summary>
        /// Actualiza la informacion de cuenta corriente
        /// </summary>
        public static readonly string UpdateCuentaCorrienteRestaLimiteCredito = @"
            UPDATE KfnCuentaCorriente WITH(ROWLOCK)
            SET ctaSaldo = ctaSaldo - @depMonto
            WHERE
                ctaId = @ctaId
                AND ageId = @ageId
                AND (ctaSaldo - @depMonto + @limiteCredito) >= 0;
        ";

        /// <summary>
        /// Insert movimiento cuenta corriente
        /// </summary>
        public static readonly string InsertMovimientoCuentaCorriente = CalcHourCountry + @"
            INSERT INTO KfnCuentaCorrienteMovimiento WITH(ROWLOCK)
            (ccmId, ctaId, ccmFecha, ccmImporte, ccmSaldo, ccmDetalle, ccmNumeroTransaccion, ttrId)
            VALUES(@traId, @ctaId, @DATETIME_COUNTRY, @depMonto, @ctaSaldo, @sComentario, @traId, @ttrId);
        ";

        /// <summary>
        /// Inserta movimiento de auditoria
        /// </summary>
        public static readonly string InsertAuditoria = CalcHourCountry + @"
            INSERT INTO KcrTransaccion WITH(ROWLOCK)
            (traId, usrId, usrIdSuperior, traComentario, traFecha, traIdReferencia, traDominio, traSubdominio)
            VALUES
            (@traId, @usrId, @usrIdSuperior, @traComentario, @DATETIME_COUNTRY, @iCodigo, @traDominio, @traSubdominio);
        ";

        /// <summary>
        /// Insert movimiento solicitud de producto
        /// </summary>
        public static readonly string InsertSolicitudProducto = CalcHourCountry + @"
            INSERT INTO KlgSolicitudProducto WITH(ROWLOCK) 
            (sprId, usrId, ageIdSolicitante, ageIdDestinatario, prvIdDestinatario, sprEstado, sprFecha,
            sprFechaAprobacion, sprImporteSolicitud, sltId, ageIdBodegaOrigen, ageIdBodegaDestino, usrIdInitiator)
            VALUES
            (@sprId, @usrId, @ageIdSolicitante, @ageIdDestinatario, @prvIdDestinatario, @sprEstado, @DATETIME_COUNTRY,
            @DATETIME_COUNTRY, @sprImporteSolicitud, @sltId, @ageIdBodegaOrigen, @ageIdBodegaDestino, @usrIdInitiator);
        ";

        /// <summary>
        /// Insert movimiento solicitud de producto - item
        /// </summary>
        public static readonly string InsertSolicitudProductoItem = @"
            INSERT INTO KlgSolicitudProductoItem WITH(ROWLOCK)
            (sprId, prdId, spiCantidadSolicitada, spiCantidadAutorizada, spiPrecioUnitario, spiEstado)
            VALUES
            (@sprId, @prdId, @spiCantidadSolicitada, @spiCantidadAutorizada, @spiPrecioUnitario, @spiEstado);
        ";

        /// <summary>
        /// Insert movimiento envio de producto
        /// </summary>
        public static readonly string InsertEnvioProducto = CalcHourCountry + @"
            INSERT INTO KlgEnvio WITH(ROWLOCK) (envId, envEstado, envFechaEnvio, envFechaRecepcion, envNumeroRemito, envNumeroFactura, envObservaciones, ageIdBodegaOrigen, ageIdBodegaDestino)
            VALUES
            (@envId, @envEstado, @DATETIME_COUNTRY, @DATETIME_COUNTRY, @envNumeroRemito, @envNumeroFactura, @envObservaciones, @ageIdBodegaOrigen, @ageIdBodegaDestino);
        ";

        /// <summary>
        /// Insert movimiento solicitud de producto - item
        /// </summary>
        public static readonly string InsertSolicitudProductoEnvio = @"
            INSERT INTO KlgSolicitudProductoEnvio WITH(ROWLOCK) (sprId, envId) VALUES (@solicitudId, @envioId);
        ";

        /// <summary>
        /// Insert movimiento envio de producto
        /// </summary>
        public static readonly string InsertEnvioItem = @"
            INSERT INTO KlgEnvioItem WITH(ROWLOCK) (envId, prdId, eitCantidad, eitEstado) VALUES (@envioId, @prdId, @monto, @estado);
        ";
    }
}
