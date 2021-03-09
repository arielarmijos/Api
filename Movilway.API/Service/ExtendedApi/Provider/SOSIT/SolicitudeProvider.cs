using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.Logging;
using Movilway.API.Service.ExtendedApi.DataContract.SOSIT;
using System.Configuration;

namespace Movilway.API.Service.ExtendedApi.Provider.SOSIT
{
    /// <summary>
    //Clase principal de SOS IT Solicitudes, en la cual se implementa la logica necesaria para dar
    //respuestaa los servicios disponibles en la interfaz
    /// </summary>
    [ServiceProviderImpl(Platform = ApiTargetPlatform.SOSIT, ServiceName = ApiServiceName.SOSIT)]
    internal partial class SolicitudeProvider : AGenericPlatformAuthentication
    {
        /// <summary>
        /// Código de respuesta exitoso para los servicios 
        /// </summary>
        public static readonly string CodigoRespuestaExitoso = "00";

        /// <summary>
        /// Variable para almacenar todos los mensajes de retorno de los diferentes llamados
        /// </summary>
        private ErrorMessagesMnemonics errorMessage = ErrorMessagesMnemonics.None;

        /// <summary>
        /// Gets or sets url API SOSIT
        /// </summary>
        private string urlApi = string.Empty;

        /// <summary>
        /// Gets or sets TECHNICIAN_KEY
        /// </summary>
        private string technicianKey = string.Empty;

        /// <summary>
        /// Gets or sets formato en el cual se envia la data al API SOSIT
        /// </summary>
        private string format = string.Empty;

        /// <summary>
        /// Gets or sets tiempo de espera al llamar el  API SOSIT
        /// </summary>
        private string TimeOutSOSIT = string.Empty;
        


        /// <summary>
        /// Initializes a new instance of the <see cref="CashProvider" /> class.
        /// </summary>
        public SolicitudeProvider()
        {
            SolicitudeProvider.logger = LoggerFactory.GetLogger(typeof(SolicitudeProvider));

            try
            {
                this.urlApi = ConfigurationManager.AppSettings["SOSITURLAPI"];
            }
            catch (Exception)
            {
            }                      
            
            try
            {
                this.TimeOutSOSIT = ConfigurationManager.AppSettings["TimeOutSOSIT"];
            }
            catch (Exception)
            {
            }

            try
            {
                this.technicianKey = ConfigurationManager.AppSettings["TECHNICIAN_KEY"];
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Obtiene el mensaje de códigos de respuesta para los servicios
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
                    ret = "La agencia no tiene solicitudes creadas";
                    break;
                case "04":
                    ret = "La solicitud no tiene notificaciones creadas";
                    break;
                case "05":
                    ret = "La solicitud no tiene conversaciones";
                    break;
                case "06":
                    ret = "Time Out Consumiendo API";
                    break;
                case "07":
                    ret = "No fue posible crear la solicitud";
                    break;
                default:
                    ret = string.Concat(codigoRespuesta, " - Desconocido");
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
                var formato = "";

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
                catch (Exception ex)
                { }

            }

            return ret;
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
        /// Obtiene el código de Prioridad
        /// </summary>
        /// <param name="tipo">Prioridad</param>
        /// <returns>Código para el tipo de identificación especificado</returns>
        public static long GetPriorityCod(DataContract.SOSIT.EnumPriority priority)
        {
            long ret = -1;

            switch (priority)
            {
                case EnumPriority.Alto:
                    ret = 1;
                    break;
                case EnumPriority.Bajo:
                    ret = 2;
                    break;
                case EnumPriority.Critica:
                    ret = 3;
                    break;
                case EnumPriority.Medio:
                    ret = 4;
                    break;
                case EnumPriority.Normal:
                    ret = 5;
                    break;
            }

            return ret;
        }


        /// <summary>
        /// Obtiene el código de la categoria de servicio
        /// </summary>
        /// <param name="tipo">Categoria Servicio</param>
        /// <returns>Código para el estado del giro especificado</returns>
        public static long GetServiceCategoryCod(DataContract.SOSIT.EnumServiceCategory category)
        {
            long ret = -1;

            switch (category)
            {
                case EnumServiceCategory.ActiveDirectory:
                    ret = 1;
                    break;
                case EnumServiceCategory.AplicacionesIntelligence:
                    ret = 2;
                    break;
                case EnumServiceCategory.CChannel:
                    ret = 3;
                    break;
                case EnumServiceCategory.CLearning:
                    ret = 4;
                    break;
                case EnumServiceCategory.ComprasGenerales:
                    ret = 5;
                    break;
                case EnumServiceCategory.Comunicaciones:
                    ret = 6;
                    break;
                case EnumServiceCategory.CorreoElectronico:
                    ret = 7;
                    break;
                case EnumServiceCategory.EquiposAccesorios:
                    ret = 8;
                    break;
                case EnumServiceCategory.Intranet:
                    ret = 9;
                    break;
                case EnumServiceCategory.JDA:
                    ret = 10;
                    break;
                case EnumServiceCategory.LogisticaInternacional:
                    ret = 11;
                    break;
                case EnumServiceCategory.Lync:
                    ret = 12;
                    break;
                case EnumServiceCategory.MSProjectServer:
                    ret = 13;
                    break;
                case EnumServiceCategory.OPMovilway:
                    ret = 14;
                    break;
                case EnumServiceCategory.PiaGlobal:
                    ret = 15;
                    break;
                case EnumServiceCategory.PiaGuatemala:
                    ret = 16;
                    break;
                case EnumServiceCategory.PiaPanama:
                    ret = 17;
                    break;
                case EnumServiceCategory.SapB1:
                    ret = 18;
                    break;
                case EnumServiceCategory.SapB1Celistics:
                    ret = 19;
                    break;
                case EnumServiceCategory.SapB1Movilway:
                    ret = 20;
                    break;
                case EnumServiceCategory.SeguridadCorporativa:
                    ret = 21;
                    break;
                case EnumServiceCategory.ServicioRed:
                    ret = 22;
                    break;
                case EnumServiceCategory.ServicioTelefoniaAvaya:
                    ret = 23;
                    break;
                case EnumServiceCategory.Sharepoint:
                    ret = 24;
                    break;
                case EnumServiceCategory.SoporteAplicaciones:
                    ret = 25;
                    break;
                case EnumServiceCategory.TIInfraestructura:
                    ret = 26;
                    break;
                case EnumServiceCategory.TIServiciosCPS:
                    ret = 27;
                    break;
                case EnumServiceCategory.TMSBrasil:
                    ret = 28;
                    break;

            }

            return ret;
        }

        /// <summary>
        /// Obtiene el código de Tipo de Solicitud
        /// </summary>
        /// <param name="tipo">TipoSolicitud</param>
        /// <returns>Código para el tipo de identificación especificado</returns>
        public static long GetTypeSolicitudeCod(DataContract.SOSIT.TypeSolicitude type)
        {
            long ret = -1;

            switch (type)
            {
                case TypeSolicitude.AccesoAplicaciones:
                    ret = 1;
                    break;
                case TypeSolicitude.Consulta:
                    ret = 2;
                    break;
                case TypeSolicitude.CuentasUsuariosLicencias:
                    ret = 3;
                    break;
                case TypeSolicitude.Incidente:
                    ret = 4;
                    break;
                case TypeSolicitude.RequerimientoMenor:
                    ret = 5;
                    break;
            }

            return ret;
        }


    }
}