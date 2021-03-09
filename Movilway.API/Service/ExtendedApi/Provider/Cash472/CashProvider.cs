// <copyright file="CashProvider.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.Provider.Cash472
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using DataContract.Cash472;
    using DataContract.Common;
    using Movilway.Logging;

    /// <summary>
    /// Clase principal de CashIn - CashOut en la cual se implementa la logica necesaria para dar respuesta
    /// a los servicios disponibles en la interfaz
    /// </summary>
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Cash, ServiceName = ApiServiceName.Cash)]
    internal partial class CashProvider : AGenericPlatformAuthentication
    {
        /// <summary>
        /// Código de respuesta exitoso para los servicios de MultiPay 472
        /// </summary>
        public static readonly string CodigoRespuestaExitoso = "00";

        /// <summary>
        /// Variable para almacenar todos los mensajes de retorno de los diferentes llamados
        /// </summary>
        private ErrorMessagesMnemonics errorMessage = ErrorMessagesMnemonics.None;

        /// <summary>
        /// Gets or sets multipayUsuario
        /// </summary>
        private MultiPay472.Usuario multipayUsuario;

        /// <summary>
        /// Gets or sets multiplayNitRed
        /// </summary>
        private string multipayNitRed = string.Empty;

        /// <summary>
        /// Gets or sets multipayTerminal
        /// </summary>
        private string multipayTerminal = string.Empty;

        /// <summary>
        /// Gets or sets multipayTopUpMno
        /// </summary>
        private string multipayTopUpMno = string.Empty;

        /// <summary>
        /// Gets or sets multipayReverseTopUpMno
        /// </summary>
        private string multipayReverseTopUpMno = string.Empty;

        /// <summary>
        /// Gets or sets multipayTripleDesKey
        /// </summary>
        private string multipayTripleDesKey = string.Empty;

        /// <summary>
        /// Gets or sets smssEnabled
        /// </summary>
        private bool smssEnabled = false;

        /// <summary>
        /// Gets or sets culture
        /// </summary>
        private System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("es-CO");

        /// <summary>
        /// Initializes static members of the <see cref="CashProvider" /> class.
        /// </summary>
        static CashProvider()
        {
            CashProvider.logger = LoggerFactory.GetLogger(typeof(CashProvider));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CashProvider" /> class.
        /// </summary>
        public CashProvider()
        {
            this.multipayUsuario = new MultiPay472.Usuario() { Usuario1 = string.Empty, Clave = string.Empty };
            try
            {
                this.multipayUsuario.Usuario1 = ConfigurationManager.AppSettings["MultiPayUsername"];
            }
            catch (Exception)
            {
            }

            try
            {
                this.multipayUsuario.Clave = ConfigurationManager.AppSettings["MultiPayPassword"];
            }
            catch (Exception)
            {
            }

            try
            {
                this.multipayTerminal = ConfigurationManager.AppSettings["MultiPayTerminal"];
            }
            catch (Exception)
            {
            }

            try
            {
                this.multipayNitRed = ConfigurationManager.AppSettings["MultiPayNitRed"];
            }
            catch (Exception)
            {
            }

            try
            {
                this.multipayTopUpMno = ConfigurationManager.AppSettings["MultiPayTopUpMno"];
            }
            catch (Exception)
            {
            }

            try
            {
                this.multipayReverseTopUpMno = ConfigurationManager.AppSettings["MultiPayReverseTopUpMno"];
            }
            catch (Exception)
            {
            }

            try
            {
                this.multipayTripleDesKey = ConfigurationManager.AppSettings["MultiPayTripeDesKey"];
            }
            catch (Exception)
            {
            }

            try
            {
                this.smssEnabled = Boolean.Parse(ConfigurationManager.AppSettings["SMSsGirosEnabled"]);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Obtiene el mensaje de códigos de respuesta para los servicios
        /// de MultiPay 472
        /// </summary>
        /// <param name="codigoRespuesta">Código de respuesta</param>
        /// <returns>Mensaje para el código especificado</returns>
        public static string ObtenerMensajeCodigoRespuesta(string codigoRespuesta)
        {
            string ret = string.Empty;

            switch (codigoRespuesta)
            {
                case "00":
                    ret = "Transacción aprobada";
                    break;
                case "01":
                    ret = "Datos de red inválidos";
                    break;
                case "02":
                    ret = "Datos de usuario inválidos";
                    break;
                case "03":
                    ret = "Monto giro inválido";
                    break;
                case "04":
                    ret = "Token no existe";
                    break;
                case "05":
                    ret = "ID giro no existe";
                    break;
                case "06":
                    ret = "Transacción original no existe";
                    break;
                case "07":
                    ret = "El usuario no tiene giros por cobrar";
                    break;
                case "08":
                    ret = "Giro no esta disponible para el pago";
                    break;
                case "09":
                    ret = "Tipo de identificación inválida";
                    break;
                case "10":
                    ret = "Valor del giro no válido";
                    break;
                default:
                    ret = string.Concat(codigoRespuesta, " - Desconocido");
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Obtiene el código de tipo de identificación para los servicios
        /// de MultiPay 472
        /// </summary>
        /// <param name="tipo">Tipo de identificación</param>
        /// <returns>Código para el tipo de identificación especificado</returns>
        public static long ObtenerCodigoTipoIdentificacion(DataContract.Cash472.TipoIdentificacion tipo)
        {
            long ret = -1;

            switch (tipo)
            {
                case TipoIdentificacion.CedulaCiudadania:
                    ret = 13;
                    break;
                case TipoIdentificacion.TarjetaExtranjeria:
                    ret = 21;
                    break;
                case TipoIdentificacion.CedulaExtranjeria:
                    ret = 22;
                    break;
                case TipoIdentificacion.Nit:
                    ret = 31;
                    break;
                case TipoIdentificacion.Pasaporte:
                    ret = 41;
                    break;
                case TipoIdentificacion.DocumentoExtranjero:
                    ret = 42;
                    break;
                case TipoIdentificacion.Otro:
                    ret = 0;
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Obtiene el tipo de identificación de un cliente a partir del código
        /// de MultiPay 472
        /// </summary>
        /// <param name="codigo">Código tipo de identificación</param>
        /// <returns>Tipo de identificación de un cliente</returns>
        public static DataContract.Cash472.TipoIdentificacion ObtenerTipoIdentificacion(long codigo)
        {
            DataContract.Cash472.TipoIdentificacion ret = DataContract.Cash472.TipoIdentificacion.Otro;

            switch (codigo)
            {
                case 13:
                    ret = TipoIdentificacion.CedulaCiudadania;
                    break;
                case 21:
                    ret = TipoIdentificacion.TarjetaExtranjeria;
                    break;
                case 22:
                    ret = TipoIdentificacion.CedulaExtranjeria;
                    break;
                case 31:
                    ret = TipoIdentificacion.Nit;
                    break;
                case 41:
                    ret = TipoIdentificacion.Pasaporte;
                    break;
                case 42:
                    ret = TipoIdentificacion.DocumentoExtranjero;
                    break;
                case 0:
                    ret = TipoIdentificacion.Otro;
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Obtiene una fecha a partir del string retornado por los servicios Web
        /// de Multipay 472
        /// </summary>
        /// <param name="fecha">Fecha a parsear</param>
        /// <returns>Un objeto <c>DateTime</c> que contiene la fecha, <c>DateTime.MinValue</c> si ocurre algún error parseando la fecha</returns>
        public static DateTime ObtenerFechaDesdeString(string fecha)
        {
            DateTime ret = DateTime.MinValue;

            if (!string.IsNullOrEmpty(fecha))
            {
                var formato = string.Empty;

                try
                {
                    formato = fecha.IndexOf("a.m", StringComparison.CurrentCultureIgnoreCase) > -1 || fecha.IndexOf("p.m", StringComparison.CurrentCultureIgnoreCase) > -1 ? "dd/MM/yyyy hh:mm:ss" : "dd/MM/yyyy HH:mm:ss";
                    bool pm = fecha.Contains("p.m.");
                    fecha = fecha.Replace("p.m.", string.Empty).Replace("a.m.", string.Empty).Trim();

                    ret = DateTime.ParseExact(fecha, formato, System.Globalization.CultureInfo.InvariantCulture);
                    if (pm)
                    {
                        ret = ret.AddHours(12);
                    }
                }
                catch (Exception)
                {
                }
            }

            return ret;
        }

        /// <summary>
        /// Obtiene el código de tipo de cliente para los servicios
        /// de MultiPay 472
        /// </summary>
        /// <param name="tipo">Tipo de cliente</param>
        /// <returns>Código para el tipo de cliente especificado</returns>
        public static long ObtenerCodigoTipoCliente(DataContract.Cash472.TipoCliente tipo)
        {
            long ret = -1;

            switch (tipo)
            {
                case TipoCliente.Emisor:
                    ret = 1;
                    break;
                case TipoCliente.Receptor:
                    ret = 2;
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Obtiene el código de tipo de Transaccion efectuadas sobre un Giro
        /// de MultiPay 472
        /// </summary>
        /// <param name="tipo">Tipo de Transaccion</param>
        /// <returns>Código para el tipo de transaccion especificado</returns>
        public static long ObtenerCodigoTipoTransaccion(DataContract.Cash472.TipoTransaccion tipo)
        {
            long ret = -1;

            switch (tipo)
            {
                case TipoTransaccion.Cotizacion:
                    ret = 1;
                    break;
                case TipoTransaccion.Constitucion:
                    ret = 2;
                    break;
                case TipoTransaccion.Emision:
                    ret = 3;
                    break;
                case TipoTransaccion.ReversoEmision:
                    ret = 4;
                    break;
                case TipoTransaccion.Pago:
                    ret = 5;
                    break;
                case TipoTransaccion.ReversoPago:
                    ret = 6;
                    break;
                case TipoTransaccion.DevoluciónIncluidoFlete:
                    ret = 7;
                    break;
                case TipoTransaccion.DevoluciónNoIncluidoFlete:
                    ret = 8;
                    break;
                case TipoTransaccion.SincronizacionRobot472:
                    ret = 9;
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Obtiene el código del estado de un Giro
        /// </summary>
        /// <param name="tipo">Estado Giro</param>
        /// <returns>Código para el estado del giro especificado</returns>
        public static long ObtenerCodigoEstadoGiro(DataContract.Cash472.EstadoGiro tipo)
        {
            long ret = -1;

            switch (tipo)
            {
                case EstadoGiro.Emitido:
                    ret = 1;
                    break;
                case EstadoGiro.Enmendado:
                    ret = 2;
                    break;
                case EstadoGiro.Anulado:
                    ret = 3;
                    break;
                case EstadoGiro.Pagado:
                    ret = 4;
                    break;
                case EstadoGiro.SolicitudAnulacion:
                    ret = 5;
                    break;
                case EstadoGiro.SolicitudEmendacion:
                    ret = 6;
                    break;
                case EstadoGiro.Inactivo:
                    ret = 7;
                    break;
                case EstadoGiro.Reembolsable:
                    ret = 8;
                    break;
                case EstadoGiro.Consitucion:
                    ret = 9;
                    break;
                case EstadoGiro.ReversadoEmision:
                    ret = 10;
                    break;
                case EstadoGiro.DevueltoConFlete:
                    ret = 11;
                    break;
                case EstadoGiro.DevueltoSinFlete:
                    ret = 12;
                    break;
                case EstadoGiro.SolicitudDevolucion:
                    ret = 13;
                    break;
                case EstadoGiro.PagoDevuelto:
                    ret = 14;
                    break;
                case EstadoGiro.GiroRedServi:
                    ret = 15;
                    break;
                case EstadoGiro.Inactivo90Dias:
                    ret = 16;
                    break;
                case EstadoGiro.Reembolsable90Dias:
                    ret = 17;
                    break;
                case EstadoGiro.EnProceso:
                    ret = 18;
                    break;
                case EstadoGiro.ErrorProcesando:
                    ret = 19;
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Obtiene una nueva instancia del cliente SOAP de MultiPay 472
        /// </summary>
        /// <returns>Una nueva instancia del objeto <c>MultiPay472.Service1SoapClient</c></returns>
        private MultiPay472.Service1SoapClient GetSoapClient()
        {
            MultiPay472.Service1SoapClient client = new MultiPay472.Service1SoapClient();

            int timeout = 60;
            try
            {
                timeout = Convert.ToInt32(ConfigurationManager.AppSettings["MultiPayTimeout"]);
            }
            catch (Exception)
            {
            }

            client.InnerChannel.OperationTimeout = TimeSpan.FromSeconds(timeout);
            return client;
        }

        /// <summary>
        /// Genera un código de transacción para ser enviado a MultiPay472
        /// <para />
        /// Si se especifica un código base, se genera el código de transacción a partir de este
        /// </summary>
        /// <param name="from">Código base</param>
        /// <returns>Un <c>string</c> que especifica el código de transacción</returns>
        private string GenerarCodigoTransaccion(string from = null)
        {
            string ret = string.Empty;

            if (!string.IsNullOrEmpty(from))
            {
                if (from.Length < 12)
                {
                    from = this.GenerateRandomString(12 - from.Length) + from;
                }

                ret = from.Substring(0, 12);
            }
            else
            {
                ret = this.GenerateRandomString(12);
            }

            return ret;
        }

        /// <summary>
        /// Genera un <c>string</c> aleatorio de la longitud indicada
        /// </summary>
        /// <param name="length">Longitud del string</param>
        /// <returns>Un <c>string</c> que contiene la cadena generada</returns>
        private string GenerateRandomString(int length)
        {
            Random rd = new Random();
            const string Caracteres = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
            char[] gnerado = new char[length];

            for (int i = 0; i < length; i++)
            {
                gnerado[i] = Caracteres[rd.Next(0, Caracteres.Length)];
            }

            return new string(gnerado);
        }

        /// <summary>
        /// Establece los valores del objeto especificado (origen o destino) a partir del cliente especificado
        /// </summary>
        /// <param name="cliente">Cliente con la información básica</param>
        /// <param name="origen">Objeto origen</param>
        /// <param name="destino">Objeto destino</param>
        private void EstablecerValoresCliente472(DataContract.Cash472.Cliente cliente, MultiPay472.Originador origen, MultiPay472.Destinatario destino)
        {
            if (origen != null)
            {
                if (origen.OIdentificacionCliente == null)
                {
                    origen.OIdentificacionCliente = new MultiPay472.IdentificacionCliente();
                }

                origen.ConHuella = false;

                origen.OIdentificacionCliente.TipoIdentificacion = Cash472.CashProvider.ObtenerCodigoTipoIdentificacion(cliente.TipoIdentificacion);
                origen.OIdentificacionCliente.NumeroIdentificacion = cliente.NumeroIdentificacion;
                origen.PrimerNombre = cliente.PrimerNombre;
                origen.PrimerApellido = cliente.PrimerApellido;
                origen.CodigoDaneCiudadDomicilio = cliente.CiudadDomicilio;
                origen.NumeroTelefono = cliente.Telefono;

                if (cliente.FechaExpedicion != null && cliente.FechaExpedicion.HasValue)
                {
                    origen.FechaExpedicion = cliente.FechaExpedicion.Value.ToString("dd/MM/yyyy");
                }

                if (cliente.Celular != null && cliente.Celular.HasValue)
                {
                    origen.NumeroCelular = cliente.Celular.Value;
                }

                if (!string.IsNullOrEmpty(cliente.SegundoNombre))
                {
                    origen.SegundoNombre = cliente.SegundoNombre;
                }

                if (!string.IsNullOrEmpty(cliente.SegundoApellido))
                {
                    origen.SegundoApellido = cliente.SegundoApellido;
                }

                if (!string.IsNullOrEmpty(cliente.Direccion))
                {
                    origen.Direccion = cliente.Direccion;
                }
            }

            if (destino != null)
            {
                if (destino.OIdentificacionCliente == null)
                {
                    destino.OIdentificacionCliente = new MultiPay472.IdentificacionCliente();
                }

                destino.OIdentificacionCliente.TipoIdentificacion = Cash472.CashProvider.ObtenerCodigoTipoIdentificacion(cliente.TipoIdentificacion);
                destino.OIdentificacionCliente.NumeroIdentificacion = cliente.NumeroIdentificacion;
                destino.PrimerNombre = cliente.PrimerNombre;
                destino.PrimerApellido = cliente.PrimerApellido;
                destino.CodigoDaneCiudadDomicilio = cliente.CiudadDomicilio;
                destino.NumeroTelefono = cliente.Telefono;

                if (cliente.Celular != null && cliente.Celular.HasValue)
                {
                    destino.NumeroCelular = cliente.Celular.Value;
                }

                if (!string.IsNullOrEmpty(cliente.SegundoNombre))
                {
                    destino.SegundoNombre = cliente.SegundoNombre;
                }

                if (!string.IsNullOrEmpty(cliente.SegundoApellido))
                {
                    destino.SegundoApellido = cliente.SegundoApellido;
                }

                if (!string.IsNullOrEmpty(cliente.Direccion))
                {
                    destino.Direccion = cliente.Direccion;
                }
            }
        }

        /// <summary>
        /// Establece el error especificado en el objeto de respuesta de una petición
        /// </summary>
        /// <param name="response">Objeto de respuesta de la petición</param>
        /// <param name="code">Error a esteblecer</param>
        private void SetResponseErrorCode(AGenericApiResponse response, ErrorMessagesMnemonics code)
        {
            if (response != null)
            {
                response.ResponseCode = (int)code;
                response.ResponseMessage = code.ToDescription();
            }
        }

        /// <summary>
        /// Formats money order amounts
        /// </summary>
        /// <param name="amount">Amount to be formatted</param>
        /// <returns>Formatted amount</returns>
        private string FormatMoney(long amount)
        {
            string moneyFormat = "N2";
            return string.Concat("$", amount.ToString(moneyFormat, this.culture));
        }

        /// <summary>
        /// Formats money order amounts
        /// </summary>
        /// <param name="amount">Amount to be formatted</param>
        /// <returns>Formatted amount</returns>
        private string FormatMoney(decimal amount)
        {
            string moneyFormat = "N2";
            return string.Concat("$", amount.ToString(moneyFormat, this.culture));
        }
    }
}
