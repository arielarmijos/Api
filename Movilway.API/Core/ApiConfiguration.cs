using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Movilway.API.Core
{
    /// <summary>
    /// Clase para contener las constantes que modifican el comportamiento del api
    /// </summary>
    public class ApiConfiguration
    {
        public static bool API_SECURE_OPTIMIZATION = false;

        public static bool IS_AVAILABLE_SECURITY = false;

        public static bool VALIDATE_COMISSION_BY_SALES = false;

        static ApiConfiguration()
        {
            Boolean.TryParse(ConfigurationManager.AppSettings["API_SECURE_OPTIMIZATION"], out API_SECURE_OPTIMIZATION);

            Boolean.TryParse(ConfigurationManager.AppSettings["IS_AVAILABLE_SECURITY"], out IS_AVAILABLE_SECURITY);

            Boolean.TryParse(ConfigurationManager.AppSettings["VALIDATE_COMISSION_BY_SALES"], out VALIDATE_COMISSION_BY_SALES);
        }

    }
}