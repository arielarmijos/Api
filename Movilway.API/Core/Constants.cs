using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Core
{
    /// <summary>
    /// Clase para incluir constantes de desarrollo, que a su vez se encuntren en persistencia
    /// </summary>
    public static  class cons
    {
        //CONSTANTES PARA CACHE

        public const string  CACHE_SESSION_PREFIX = "CACHE_SESSION_PREFIX_";

        //CONSTANTES PARA LOS ACCESOS
        public const decimal ACCESS_WEB = 1m;

        public const int ACCESS_H2H = 3;
        //

        public const decimal ACCESS_POS = 6m;

        public const decimal ACCESS_POSWEB = 12m;
     

  
        //CONSTANTES PARA LOS ROLES
        public const int ROL_ADMINPOSWEB = 20;

        //AGENTE CONCENTRADOR
        public const decimal AGENTE_CONCENTRADOR = 0m;

        //MULTIPORDUCTO
        public const decimal MULTI_PRODUCTO = 0m;

        
        //Distribución de credito por comisiones 
        public const decimal SOLICITUD_COMISIONES = 501;



        //CONSTANTES ESTADOS DE DSIPOSITIVOS
        public const Int16 DEVICE_DELETE = -1;
        public const Int16 DEVICE_IN_BLACK_LIST = 0;
        public const Int16 DEVICE_ACTIVE = 1;//¿Seguro?
        public const Int16 DEVICE_TEMPORAL = 2;//No Seguro
        public const Int16 DEVICE_INACTIVE = 3;//Dispositivos nuevos registrados
        public const Int16 DEVICE_NEW = 4;//Dispositivos nuevos registrados

        //COSNTANTES TYPOS DE DISPOSITIVOS
        public const Int16 DEVICE_TYPE_WEB = 1;

        /// <summary>
        /// BD - ID depósitos activos
        /// </summary>
        public const string DepositoActivo = "AC";

        /// <summary>
        /// BD - ID depósitos cancelados
        /// </summary>
        public const string DepositoCancelado = "CA";

        /// <summary>
        /// BD - ID depósitos anulados
        /// </summary>
        public const string DepositoAnulado = "AN";

        /// <summary>
        /// BD - ID depósitos autorizados
        /// </summary>
        public const string DepositoAutorizado = "AU";

        /// <summary>
        /// BD - ID depósitos activos
        /// </summary>
        public const string DepositoRechazado = "RE";

        /// <summary>
        /// BD - ID solicitud de productos cerrados
        /// </summary>
        public static readonly string SolicitudProductoCerrado = "CE";

        /// <summary>
        /// BD - Llave secuencia depósito
        /// </summary>
        public static readonly string SecuenciaDeposito = "DEPOSITO";

        /// <summary>
        /// BD - Llave secuencia transacción
        /// </summary>
        public static readonly string SecuenciaTransaccion = "TRANSACCION";

        /// <summary>
        /// BD - Llave secuencia auditoria
        /// </summary>
        public static readonly string SecuenciaAuditoria = "AUDITORIA";

        /// <summary>
        /// BD - Llave secuencia movimiento cuenta corriente
        /// </summary>
        public static readonly string SecuenciaMovimientoCuentaCorriente = "CTACTEMOVIMIENTO";

        /// <summary>
        /// BD - Llave secuencia solicitud producto
        /// </summary>
        public static readonly string SecuenciaSolicitudProducto = "SOLICITUDPRODUCTO";

        /// <summary>
        /// BD - Llave secuencia envio de producto
        /// </summary>
        public static readonly string SecuenciaEnvioProducto = "ENVIOPRODUCTO";

        /// <summary>
        /// BD - ID terminal robot
        /// </summary>
        public static readonly string NumeroTerminalRobot = "0000000000";

        /// <summary>
        /// BD - Nombre terminal robot
        /// </summary>
        public static readonly string NombreTerminalRobot = " AUTOMATICO ROBOT C:NO";

        /// <summary>
        /// BD - Nombre terminal robot (para comisión)
        /// </summary>
        public static readonly string NombreTerminalRobotComision = " AUTOMATICO ROBOT C:SI({0}:{1})";

        /// <summary>
        /// BD - TTR ID movimientos cuenta corriente
        /// </summary>
        public static readonly int TtrIdMovimientoCuentaCorriente = 1000;

        /// <summary>
        /// BD - TTR ID movimientos cuenta corriente
        /// </summary>
        public static readonly int TtrIdMovimientoCuentaCorrienteNotaCredito = 1003;

        /// <summary>
        /// BD - TTR ID movimientos cuenta corriente
        /// </summary>
        public static readonly int TtrIdMovimientoCuentaCorrienteNotaDebito = 1004;
    }
}
