// <copyright file="Cash.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.Provider.Cash472.Queries
{
    /// <summary>
    /// Cash database related queries
    /// </summary>
    internal static class Cash
    {
        /// <summary>
        /// Obtiene la información de un cliente
        /// </summary>
        public static readonly string GetInfoCliente = @"
            DECLARE @TipoIdent INT = @TipoIdentificacionId;
            DECLARE @NumeroIdent VARCHAR(20) = @NumeroIdentificacion;

            SELECT
                 Id
                ,TipoIdentificacionId
                ,NumeroIdentificacion
                ,FechaExpedicion
                ,RazonSocial
                ,PrimerNombre
                ,SegundoNombre
                ,PrimerApellido
                ,SegundoApellido
                ,Ciudad
                ,Direccion
                ,Telefono
                ,Celular
                ,ExternalId
            FROM
                [dbo].[Cliente] WITH(READUNCOMMITTED)
            WHERE
                TipoIdentificacionId = @TipoIdent
                AND NumeroIdentificacion = @NumeroIdent;
                
        ";

        /// <summary>
        /// Inserta un nuevo cliente en DB
        /// </summary>
        public static readonly string InsertCliente = @"
            DECLARE @Ret INT = 0;

            BEGIN TRY
                INSERT INTO [dbo].[Cliente]
                       ([TipoIdentificacionId]
                       ,[NumeroIdentificacion]
                       ,[FechaExpedicion]
                       ,[RazonSocial]
                       ,[PrimerNombre]
                       ,[SegundoNombre]
                       ,[PrimerApellido]
                       ,[SegundoApellido]
                       ,[Ciudad]
                       ,[Direccion]
                       ,[Telefono]
                       ,[Celular])
                 VALUES
                       (@TipoIdentificacionId
                       ,@NumeroIdentificacion
                       ,@FechaExpedicion
                       ,@RazonSocial
                       ,@PrimerNombre
                       ,@SegundoNombre
                       ,@PrimerApellido
                       ,@SegundoApellido
                       ,@Ciudad
                       ,@Direccion
                       ,@Telefono
                       ,@Celular)
                ;
                SET @Ret = SCOPE_IDENTITY();
            END TRY
            BEGIN CATCH
                SET @Ret = 0;
                THROW;
            END CATCH;

            SELECT @Ret;
        ";

        /// <summary>
        /// Actualiza un cliente en DB
        /// </summary>
        public static readonly string UpdateCliente = @"
            DECLARE @Id INT = @ClienteId;

            BEGIN TRY
                UPDATE [dbo].[Cliente] SET
                     [FechaExpedicion] = @FechaExpedicion
                    ,[RazonSocial] = @RazonSocial
                    ,[PrimerNombre] = @PrimerNombre
                    ,[SegundoNombre] = @SegundoNombre
                    ,[PrimerApellido] = @PrimerApellido
                    ,[SegundoApellido] = @SegundoApellido
                    ,[Ciudad] = @Ciudad
                    ,[Direccion] = @Direccion
                    ,[Telefono] = @Telefono
                    ,[Celular] = @Celular
                 WHERE
                    [Id] = @Id
                ;
            END TRY
            BEGIN CATCH
                THROW;
            END CATCH;
        ";

        /// <summary>
        /// Inserta un nuevo giro en DB
        /// </summary>
        public static readonly string InsertGiro = @"
            DECLARE @Ret INT = 0;

            BEGIN TRY
                DECLARE @DepOrigen NVARCHAR(50) = (SELECT Nombre from [dbo].[Departamento] WITH(NOLOCK) WHERE [CodDANE] = SUBSTRING(@CiudadOrigenDANE, 1, 2)); 
                DECLARE @CiudadOrigen NVARCHAR(50) = (SELECT Nombre from [dbo].[Ciudad] WITH(NOLOCK) WHERE [CodDANEDepto] = SUBSTRING(@CiudadOrigenDANE, 1, 2) AND  [CodDANE] = SUBSTRING(@CiudadOrigenDANE, 3, 3));

                DECLARE @DepDestino NVARCHAR(50) = (SELECT Nombre from [dbo].[Departamento] WITH(NOLOCK) WHERE [CodDANE] = SUBSTRING(@CiudadDestinoDANE, 1, 2)); 
                DECLARE @CiudadDestino NVARCHAR(50) = (SELECT Nombre from [dbo].[Ciudad] WITH(NOLOCK) WHERE [CodDANEDepto] = SUBSTRING(@CiudadDestinoDANE, 1, 2) AND [CodDANE] = SUBSTRING(@CiudadDestinoDANE, 3, 3));

                INSERT INTO [dbo].[Giro]
                   ([EmisorId]
                   ,[ReceptorId]
                   ,[EstadoId]
                   ,[Pdv]
                   ,[Fecha]
                   ,[TotalRecibido]
                   ,[TotalAEntregar]
                   ,[Flete]
                   ,[IncluyeFlete]
                   ,[CiudadOrigenDANE]
                   ,[DepartamentoOrigen]
                   ,[CiudadOrigen]
                   ,[CiudadDestinoDANE]
                   ,[DepartamentoDestino]
                   ,[CiudadDestino]
                   ,[AgenciaId]
                   ,[AgenciaNombre]
                   ,[AgenciaDireccion]
                   ,[AccesoTipo]
                   ,[Acceso]
                   )
             VALUES
                   (@EmisorId
                   ,@ReceptorId
                   ,@EstadoId
                   ,@Pdv
                   ,DEFAULT
                   ,@TotalRecibido
                   ,@TotalAEntregar
                   ,@Flete
                   ,@IncluyeFlete
                   ,@CiudadOrigenDANE
                   ,@DepOrigen
                   ,@CiudadOrigen
                   ,@CiudadDestinoDANE
                   ,@DepDestino
                   ,@CiudadDestino
                   ,@AgenciaId
                   ,@AgenciaNombre
                   ,@AgenciaDireccion
                   ,@AccesoTipo
                   ,@Acceso)
                ;
                SET @Ret = SCOPE_IDENTITY();
            END TRY
            BEGIN CATCH
                SET @Ret = 0;
                THROW;
            END CATCH;

            SELECT @Ret;
        ";

        /// <summary>
        /// Anula un giro en DB
        /// </summary>
        public static readonly string AnularGiro = @"
            DECLARE @Ret INT = 0;
            DECLARE @Id INT = @IdGiro;
            DECLARE @Estado INT = @IdEstado;

            BEGIN TRY
                UPDATE [dbo].[Giro]
                SET
                     Anulado = 1
                    ,FechaAnulado = SYSDATETIMEOFFSET()
                    ,ErrorApi = @Error
                    ,ErrorApiDescripcion = @Descripcion
                    ,EstadoId = @Estado
                WHERE Id = @Id;
                SET @Ret = @@ROWCOUNT;
            END TRY
            BEGIN CATCH
                SET @Ret = 0;
                THROW;
            END CATCH;

            SELECT @Ret;
        ";

        /// <summary>
        /// Inserta un giro en DB a partir del proceso completo con 472
        /// </summary>
        public static readonly string InsertGiroCash472 = @"
            DECLARE @Ret INT = 0;

            BEGIN TRY
                INSERT INTO [dbo].[Giro]
                   ([EmisorId]
                   ,[ReceptorId]
                   ,[Pdv]
                   ,[Fecha]
                   ,[TotalRecibido]
                   ,[TotalAEntregar]
                   ,[Flete]
                   ,[IncluyeFlete]
                   ,[FechaConstitucion]
                   ,[FechaEmision]
                   ,[CiudadOrigen]
                   ,[CiudadDestino]
                   ,[CodigoTransaccionConstitucion]
                   ,[CodigoTransaccionEmision]
                   ,[Token]
                   ,[Pin]
                   ,[CodigoAutorizacion]
                   ,[NumeroFactura]
                   ,[NumeroTransaccion]
                   ,[ExternalId])
             VALUES
                   (@EmisorId
                   ,@ReceptorId
                   ,@Pdv
                   ,DEFAULT
                   ,@TotalRecibido
                   ,@TotalAEntregar
                   ,@Flete
                   ,@IncluyeFlete
                   ,@FechaConstitucion
                   ,@FechaEmision
                   ,@CiudadOrigen
                   ,@CiudadDestino
                   ,@CodigoTransaccionConstitucion
                   ,@CodigoTransaccionEmision
                   ,@Token
                   ,@Pin
                   ,@CodigoAutorizacion
                   ,@NumeroFactura
                   ,@NumeroTransaccion
                   ,@ExternalId)
                ;
                SET @Ret = SCOPE_IDENTITY();
            END TRY
            BEGIN CATCH
                SET @Ret = 0;
                THROW;
            END CATCH;

            SELECT @Ret;
        ";

        /// <summary>
        /// Obtiene la información de un giro dado el Id asignado internamente en la BD de Giros
        /// </summary>
        public static readonly string GetInfoGiro = @"
            DECLARE @Id INT = @IdGiro;

                    SELECT h.[Id]
                      ,h.[EmisorId]
                      ,h.[ReceptorId]
                      ,h.[EstadoId]
                      ,h.[Pdv]
                      ,h.[TotalRecibido]
                      ,h.[TotalAEntregar]
                      ,h.[Flete]
                      ,h.[IncluyeFlete]
                      ,h.[CiudadOrigenDANE]
                      ,h.[DepartamentoOrigen]
                      ,h.[CiudadOrigen]
                      ,h.[CiudadDestinoDANE]
                      ,h.[DepartamentoDestino]
                      ,h.[CiudadDestino]
                      ,h.[Token]
                      ,h.[Pin]
                      ,h.[NumeroFactura472]
                      ,h.[NumeroFactura]
                      ,h.[NumeroComprobantePago472]
                      ,h.[ExternalId]
                      ,h.[Fecha]
                      ,h.[FechaUltimaTransaccion]
                      ,Detalle.DetalleId
                      ,Detalle.[GiroId]
                      ,Detalle.[TipoTransaccionId]
                      ,Detalle.[CodigoRespuesta472]
                      ,Detalle.[CodigoTransaccion]
                      ,Detalle.[CodigoAutorizacion472]
                      ,Detalle.[NumeroReferencia472]
                      ,Detalle.[FechaEnvio472]
                      ,Detalle.[FechaRespuesta472]
                      ,h.[ErrorApi]
                      ,h.[ErrorApiDescripcion]
                      ,Detalle.[ErrorProtocolo]
                      ,Detalle.[ErrorProtocoloDescripcion]
                      ,Detalle.[DetalleRespuesta]
                  FROM [dbo].[Giro] h WITH(READUNCOMMITTED)
                  LEFT JOIN 
                  (
                    SELECT 
                       d.[Id] as DetalleId
                      ,d.[GiroId]
                      ,d.[TipoTransaccionId]
                      ,d.[CodigoRespuesta472]
                      ,d.[CodigoTransaccion]
                      ,d.[CodigoAutorizacion472]
                      ,d.[NumeroReferencia472]
                      ,d.[FechaEnvio472]
                      ,d.[FechaRespuesta472]
                      ,d.[ErrorProtocolo]
                      ,d.[ErrorProtocoloDescripcion]
                      ,d.[DetalleRespuesta]
                  FROM [dbo].[GiroDetalle] d WITH(READUNCOMMITTED) 
                  WHERE d.Activo = 1 
                  ) AS Detalle
                   ON Detalle.[GiroId] = h.[Id]
                  WHERE h.id = @Id;
                ";

        /// <summary>
        /// Obtiene la información de un giro dado el Id del Giro reportado por 472
        /// </summary>
        public static readonly string GetInfoGiroPorExternalId = @"
            DECLARE @Id INT = @IdGiro;

            SELECT 
                    h.[Id]
                    ,h.[EmisorId]
                    ,h.[ReceptorId]
                    ,h.[EstadoId]
                    ,h.[Pdv]
                    ,h.[TotalRecibido]
                    ,h.[TotalAEntregar]
                    ,h.[Flete]
                    ,h.[IncluyeFlete]
                    ,h.[CiudadOrigenDANE]
                    ,h.[DepartamentoOrigen]
                    ,h.[CiudadOrigen]
                    ,h.[CiudadDestinoDANE]
                    ,h.[DepartamentoDestino]
                    ,h.[CiudadDestino]
                    ,h.[Token]
                    ,h.[Pin]
                    ,h.[NumeroFactura472]
                    ,h.[NumeroFactura]
                    ,h.[NumeroComprobantePago472]
                    ,h.[ExternalId]
                    ,h.[Fecha]
                    ,h.[FechaUltimaTransaccion]
                    ,h.[Anulado]
                    ,Detalle.DetalleId
                    ,Detalle.[GiroId]
                    ,Detalle.[TipoTransaccionId]
                    ,Detalle.[CodigoRespuesta472]
                    ,Detalle.[CodigoTransaccion]
                    ,Detalle.[CodigoAutorizacion472]
                    ,Detalle.[NumeroReferencia472]
                    ,Detalle.[FechaEnvio472]
                    ,Detalle.[FechaRespuesta472]
                    ,h.[ErrorApi]
                    ,h.[ErrorApiDescripcion]
                    ,Detalle.[ErrorProtocolo]
                    ,Detalle.[ErrorProtocoloDescripcion]
                    ,Detalle.[DetalleRespuesta]
                FROM [dbo].[Giro] h WITH(READUNCOMMITTED)
                LEFT JOIN 
                (        SELECT d.[Id] as DetalleId
                            ,d.[GiroId]
                            ,d.[TipoTransaccionId]
                            ,d.[CodigoRespuesta472]
                            ,d.[CodigoTransaccion]
                            ,d.[CodigoAutorizacion472]
                            ,d.[NumeroReferencia472]
                            ,d.[FechaEnvio472]
                            ,d.[FechaRespuesta472]
                            ,d.[ErrorProtocolo]
                            ,d.[ErrorProtocoloDescripcion]
                            ,d.[DetalleRespuesta]
                        FROM [dbo].[GiroDetalle] d WITH(READUNCOMMITTED) 
                        WHERE d.Activo = 1
                ) As Detalle ON Detalle.[GiroId] = h.[Id]
                WHERE h.ExternalId = @Id and h.[Anulado] = 0;
        ";

        /// <summary>
        /// Obtiene información de la agencia.
        /// </summary>
        public static readonly string InfoAgencia = @"
            SELECT
                 [Id]
                ,[Ciudad]
                ,[Habilitado]
            FROM
                [dbo].[Agencia] WITH(READUNCOMMITTED)
            WHERE
                Id = @Id
        ";

        /// <summary>
        /// Inserta un nuevo pago en DB
        /// </summary>
        public static readonly string InsertPago = @"
            DECLARE @Ret INT = 0;

            BEGIN TRY
                INSERT INTO [dbo].[Pago]
                   ([GiroId]
                   ,[ExternalId]
                   ,[EmisorId]
                   ,[ReceptorId]
                   ,[Pdv]
                   ,[CiudadPdv]
                   ,[Fecha]
                   ,[TotalRecibido]
                   ,[TotalAEntregar]
                   ,[Flete]
                   ,[IncluyeFlete]
                   ,[ValorPago])
                VALUES
                   (@GiroId
                   ,@ExternalId
                   ,@EmisorId
                   ,@ReceptorId
                   ,@Pdv
                   ,@CiudadPdv
                   ,DEFAULT
                   ,@TotalRecibido
                   ,@TotalAEntregar
                   ,@Flete
                   ,@IncluyeFlete
                   ,@ValorPago)
                ;
                SET @Ret = SCOPE_IDENTITY();
            END TRY
            BEGIN CATCH
                SET @Ret = 0;
                THROW;
            END CATCH;

            SELECT @Ret;
        ";

        /// <summary>
        /// Obtiene la información de un pago
        /// </summary>
        public static readonly string GetInfoPago = @"
            DECLARE @Id INT = @IdPago;

            SELECT
                 [Id]
                ,[GiroId]
                ,[ExternalId]
                ,[EmisorId]
                ,[ReceptorId]
                ,[Pdv]
                ,[CiudadPdv]
                ,[Fecha]
                ,[TotalRecibido]
                ,[TotalAEntregar]
                ,[Flete]
                ,[IncluyeFlete]
                ,[ValorPago]
                ,[CodigoTransaccion]
                ,[CodigoAutorizacion]
                ,[NumeroFactura]
                ,[NumeroReferencia]
                ,[NumeroComprobantePago]
                ,[FechaPago]
                ,[Anulado]
                ,[FechaAnulado]
                ,[ErrorApi]
                ,[ErrorApiDescripcion]
                ,[ErrorProtocolo]
                ,[ErrorProtocoloDescripcion]
                ,[FechaErrorProtocolo]
            FROM
                [dbo].[Pago] WITH(READUNCOMMITTED)
            WHERE
                Id = @Id
            ;
        ";

        /// <summary>
        /// Anula un pago en DB
        /// </summary>
        public static readonly string AnularPago = @"
            DECLARE @Ret INT = 0;
            DECLARE @Id INT = @IdPago;

            BEGIN TRY
                UPDATE [dbo].[Pago]
                SET
                     Anulado = 1
                    ,FechaAnulado = SYSDATETIMEOFFSET()
                    ,ErrorApi = @Error
                    ,ErrorApiDescripcion = @Descripcion
                WHERE Id = @Id;
                SET @Ret = @@ROWCOUNT;
            END TRY
            BEGIN CATCH
                SET @Ret = 0;
                THROW;
            END CATCH;

            SELECT @Ret;
        ";

        /// <summary>
        /// Obtiene la información de un cliente en listas restrictivas
        /// </summary>
        public static readonly string GetOcurrenciasClienteListasRestrictivas = @"
            DECLARE @TipoIdent INT = @TipoIdentificacionId;
            DECLARE @NumeroIdent VARCHAR(20) = @NumeroIdentificacion;

            SELECT COUNT(1) AS 'Ocurrencias'  
              FROM [dbo].[ListaRestrictiva] WITH(READUNCOMMITTED)
              where [Activo] = 'True' AND
              (  ( [TipoIdentificacion1Id] = @TipoIdent AND NumeroIdentificacion1 = @NumeroIdent) OR 
              ( [TipoIdentificacion2Id] = @TipoIdent AND NumeroIdentificacion2 = @NumeroIdent) OR 
              ( [TipoIdentificacion13d] = @TipoIdent AND NumeroIdentificacion3 = @NumeroIdent));";


        /// <summary>
        /// Obtiene la lista de las ciudades con codigo DANE.
        /// </summary>
        public static readonly string ListCities = @"
              select d.[CodDANE] + c.[CodDANE] as CodDANE, c.[Nombre]  
              from [dbo].[Departamento] d with(nolock)
              join [dbo].[Ciudad]  c with(nolock) on  d.[CodDANE] = c.[CodDANEDepto] 
              order by c.[Nombre]";

        /// <summary>
        /// Informacion factura
        /// </summary>
        private static readonly string GetInfoFacturaSelect = @"
            SELECT
                 G.[Id]
                ,G.ExternalId
                ,G.Pin
                ,G.NumeroFactura
                ,G.DepartamentoOrigen
                ,G.CiudadOrigen
                ,G.AgenciaNombre
                ,G.AgenciaDireccion
                ,G.AccesoTipo
                ,G.Acceso
                ,G.TotalRecibido
                ,G.TotalAEntregar
                ,G.Flete
                ,G.IncluyeFlete
                ,G.Fecha AS 'FechaOffset'
                ,F.ResolucionDIAN AS 'FacturaResolucion'
                ,F.Prefijo AS 'FacturaPrefijo'
                ,F.FechaResolucionDIAN AS 'FacturaFecha'
                ,F.ValorMinimo AS 'FacturaDesde'
                ,F.ValorMaximo AS 'FacturaHasta'
                ,(O.PrimerNombre + ' ' + O.PrimerApellido) AS 'OrigenNombre'
                ,(ODNI.NombreCorto + ' ' + O.NumeroIdentificacion) AS 'OrigenDni'
                ,('Tel ' + CAST(O.Telefono AS VARCHAR)) AS 'OrigenTel'
                ,(D.PrimerNombre + ' ' + D.PrimerApellido) AS 'DestinoNombre'
                ,(DDNI.NombreCorto + ' ' + D.NumeroIdentificacion) AS 'DestinoDni'
                ,('Tel ' + CAST(D.Telefono AS VARCHAR)) AS 'DestinoTel'
            FROM
                [dbo].[Giro] AS G WITH(READUNCOMMITTED)
                JOIN [dbo].[NumeracionFactura] AS F WITH(READUNCOMMITTED) ON (F.ResolucionDIAN = G.ResolucionDIAN)
                JOIN [dbo].[Cliente] AS O WITH(READUNCOMMITTED) ON (O.Id = G.EmisorId)
                JOIN [dbo].[TipoIdentificacion] AS ODNI WITH(READUNCOMMITTED) ON (ODNI.Id = O.TipoIdentificacionId)
                JOIN [dbo].[Cliente] AS D WITH(READUNCOMMITTED) ON (D.Id = G.ReceptorId)
                JOIN [dbo].[TipoIdentificacion] AS DDNI WITH(READUNCOMMITTED) ON (DDNI.Id = D.TipoIdentificacionId)
        ";

        /// <summary>
        /// Informacion factura
        /// </summary>
        public static readonly string GetInfoFacturaById = GetInfoFacturaSelect + @"
            WHERE
                G.[Id] = @id;
        ";

        /// <summary>
        /// Informacion factura
        /// </summary>
        public static readonly string GetInfoFacturaByExternalId = GetInfoFacturaSelect + @"
            WHERE
                G.[ExternalId] = @id;
        ";

    }
}
