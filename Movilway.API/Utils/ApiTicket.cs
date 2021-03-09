using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Movilway.API.Utils
{
    public class ApiTicket
    {

        private static bool ApiTicketEnabled = Boolean.TryParse(ConfigurationManager.AppSettings["ApiTicketEnabled"], out ApiTicketEnabled);


        private static string ApiTicket_dateformat = (ConfigurationManager.AppSettings["ApiTicket_dateformat"] ?? "yyyy-MM-dd HH:mm:ss");

        private static readonly string prefix = "aptck";

        private static bool IsApiTicketEnabled()
        {

           var aux =  ConfigurationManager.AppSettings["ApiTicketEnabled_"+(ConfigurationManager.AppSettings["CountryID"] ??"")];

            var flagaux = false;
            if(!Boolean.TryParse(aux, out flagaux)){

                return ApiTicketEnabled;
            }
            else
                return flagaux;

        }


        public static string FormatSaledataTopUp(ILogger logger, string Provider, string LOG_PREFIX, TopUpRequestBody request, bool kinacuTopUpResponse, int transactionId, string saleData)
        {
            var _saleData = saleData;
            try
            {


                if (IsApiTicketEnabled() && kinacuTopUpResponse)
                {

                    // andres vargas
                    //TODO LOGICA QUEMADA :-/
                    //COMO HACERLO POR PRODUCTO SIENDO QUE LOS CODIGOS DE LOS PRODUCTOS CAMBIAN 0_o
                    //HACER UN HANDLER POR PRODUCTO Y ASOCIARLO POR CONFIGURACION ^_^a

                      var keyticket = ConfigurationManager.AppSettings[prefix + request.DeviceType + "_" + request.MNO] ?? string.Empty;

                      if (!string.IsNullOrEmpty(keyticket))
                      {



                          Dictionary<string, string> _dicc = new Dictionary<string, string>();

                          _dicc = new Dictionary<string, string>();
                          var arr = (request.TerminalID ?? "").Split(' ');

                          //DVLP: Se cambia el limite inferior debido a que los nombres de los convenios
                          // pueden estar compuestos por mas de una palabra y al momento de generar el ticket se recorta
                          // el nombre Impreso
                          //if (arr.Length > 2)
                          if (arr.Length > 1)
                          {
                              // DVLP: Validar que el ultimo campo no sea numerico, Intento para soportar los valores seleccionados en tipos ListStrings para POS
                              int auxValue = 0;
                              bool isNumber = Int32.TryParse(arr[arr.Length - 1].ToString(), out auxValue);
                              var _arrlist = arr.ToList();
                              if (isNumber)
                              { //QUITAL EL ULTIMO 
                                  _arrlist.RemoveAt(_arrlist.Count - 1);                                 
                              }
                             
                              _dicc.Add("convenio", String.Join(" ", _arrlist.ToArray()));
                          } 
                          //SEVE 
                          else if (arr.Length > 0)
                          {
                              _dicc.Add("convenio", arr[0]);
                          }
                          else
                          {
                              _dicc.Add("convenio", "");
                          }
                          //_dicc.Add("6", "POS");

                          var lines = saleData.Split(';');
                          if (lines.Length > 1)
                          {
                              foreach (var line in lines)
                              {
                                  var lineparts = line.Split(':');
                                  if (lineparts.Length == 2)
                                  {
                                      var key = lineparts[0].ToLower().TrimStart().TrimEnd();
                                      if (!string.IsNullOrEmpty(key) && !_dicc.Keys.Contains(key))
                                          _dicc.Add(key, lineparts[1].TrimStart().TrimEnd());
                                  }
                                  else if (lineparts.Length > 2)
                                  {
                                      // si el split tiene mas de dos partes
                                      var key = lineparts[0].ToLower().TrimStart().TrimEnd();

                                      if (!string.IsNullOrEmpty(key) && !_dicc.Keys.Contains(key))
                                      {
                                          var laux = lineparts.ToList();
                                          //quitar la parte de llave para luego poner juntas las partes nuevametne
                                          laux.RemoveAt(0);



                                          var valueaux = String.Join(":", laux.ToArray());
                                          _dicc.Add(key, valueaux);
                                      }
                                  }
                                  else if (lineparts.Length == 1)
                                  {
                                      // si el split tiene mas de dos partes
                                      var key = lineparts[0].ToLower().TrimStart().TrimEnd();
                                      if (!string.IsNullOrEmpty(key) && !_dicc.Keys.Contains(key))
                                          _dicc.Add(key, "");

                                  }

                              }
                          }


                          return _FormatSaledataTopUp(logger, Provider, LOG_PREFIX, request, kinacuTopUpResponse, transactionId, saleData, _dicc);

                      }
                }
            }
            catch (Exception ex)
            {
                logger.ErrorLow("[API] " + LOG_PREFIX + "[" + Provider + "] ERROR FormatSaledataTopUp [" + ex.Message + "] [" + ex.GetType().Name + "] [" + ex.StackTrace + "]");
                _saleData = saleData;
            }
            return _saleData;
        }


        private static string _FormatSaledataTopUp(ILogger logger, string Provider, string LOG_PREFIX, TopUpRequestBody request, bool kinacuTopUpResponse, int transactionId, string saleData, Dictionary<string, string> _dicc = null)
        {

            var _saleData = saleData;

            try
            {

                var keyticket = ConfigurationManager.AppSettings[prefix + request.DeviceType + "_" + request.MNO] ?? string.Empty;


                var typerequest = request.GetType();
                var keyticketresult = keyticket;

                var matches = Regex.Matches(keyticket, @"{([a-zA-Z0-9]+\.[a-zA-Z0-9]+|[a-zA-Z0-9]+)}");
                var bandera = true;

                for (int i = 0; i < matches.Count && bandera; i++) //   foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    var match = matches[i];

                    var auxval = match.Value.Substring(1,match.Value.Length-2);// .Replace("{", "").Replace("}", "");

                    var parts = auxval.Split('.');


                    string firstpart = parts[0].ToLower();
                    string secondpart = "";
                    if (parts.Length > 1)
                        secondpart = parts[1].ToLower(); ;


                    var change = "";

                    /*
                     * DATENOW
                            request
   transactionId
   _dicc
                          */

                    switch (firstpart)
                    {
                        case "datenow":
                            //if (!string.IsNullOrEmpty(secondpart))
                            //{
                            //    try
                            //    {
                            //        change = DateTime.Now.ToString(secondpart, ApiTicket_dateformat);
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        logger.ErrorLow("[API] " + LOG_PREFIX + "[TopUpProvider] ERROR Formatting date [" + secondpart + "] " + ex.Message + " ");
                            //        throw new Exception("ERROR Formatting date [" + secondpart + "]");
                            //    }

                            //}
                            //else
                            change = DateTime.Now.ToString(ApiTicket_dateformat);

                            break;
                        case "request":

                            if (!string.IsNullOrEmpty(secondpart))
                            {


                                var prop = typerequest.GetProperties().FirstOrDefault(p => p.Name.ToLower() == secondpart);

                                if (prop != null)
                                {
                                    change = prop.GetValue(request, new object[] { }).ToString();
                                }
                                else
                                {
                                    bandera = false;
                                    logger.ErrorLow("[API] " + LOG_PREFIX + " No se econtro la propiedad en el request " + match.Value);
                                }
                                // throw new Exception("No se econtro la propiedad en el request " + match.Value);
                            }
                            else
                            {
                                bandera = false;
                                logger.ErrorLow("[API] " + LOG_PREFIX + " No se definio la propiedad para el request " + match.Value);
                            }



                            break;
                        case "transactionid":

                            change = transactionId.ToString();

                            break;

                        case "dictionary":


                            if (!string.IsNullOrEmpty(secondpart))
                            {

                                var auxvalue = string.Empty;

                                if (_dicc.TryGetValue(secondpart, out auxvalue))
                                {
                                    change = auxvalue;
                                }
                                else
                                {

                                    bandera = false;
                                    logger.ErrorLow("[API] " + LOG_PREFIX + " No se econtro el valor en el diccionario " + match.Value);
                                    // throw new Exception("No se econtro el valor en el diccionario " + match.Value);
                                }


                            }
                            else
                            {

                                bandera = false;
                                logger.ErrorLow("[API] " + LOG_PREFIX + " No se definio la llave para el diccionario " + match.Value);
                                //   throw new Exception("No se definio la llave para el diccionario " + match.Value);
                            }




                            break;

                        default:

                            bandera = false;
                            logger.ErrorLow("[API] " + LOG_PREFIX + " NO ESTA CONTEMPLADO ESTA LLAVE EN EL TEMPLATE " + match.Value);

                            break;

                    }


                    keyticketresult = keyticketresult.Replace(match.Value, change);
                }




                if (!bandera)
                    _saleData = saleData;
                else
                {
                    logger.InfoLow("[API] " + LOG_PREFIX + "[" + Provider + "]  Kinacuticket" + saleData);//+ " " + (_saleData));
                    _saleData = keyticketresult;
                }

                logger.InfoLow("[API] " + LOG_PREFIX + "[" + Provider + "]  FormatSaledata result " + bandera);//+ " " + (_saleData));


            }
            catch (Exception ex)
            {
                logger.ErrorLow("[API] " + LOG_PREFIX + "[" + Provider + "] ERROR Formating FormatSaledata  [" + ex.Message + "] [" + ex.GetType().Name + "] [" + ex.StackTrace + "]");
                _saleData = saleData;
            }





            return _saleData;
        }

        //si se quiere cambiar el tickete de la consulta no se 
        public static string FormatSaledata(ILogger logger, string Provider, dynamic request, string LOG_PREFIX, bool kinacuTopUpResponse, int transactionId, string saleData)
        {
            var _saleData = saleData;
            try
            {


                if (IsApiTicketEnabled() && kinacuTopUpResponse)
                {

                    //var keyticket = ConfigurationManager.AppSettings[prefix + request.DeviceType + "_" + request.MNO] ?? string.Empty;

                    //if (!string.IsNullOrEmpty(keyticket))
                    {

                        // andres vargas
                        //TODO LOGICA QUEMADA :-/
                        //COMO HACERLO POR PRODUCTO SIENDO QUE LOS CODIGOS DE LOS PRODUCTOS CAMBIAN 0_o
                        //HACER UN HANDLER POR PRODUCTO Y ASOCIARLO POR CONFIGURACION ^_^a
                        Dictionary<string, string> _dicc = new Dictionary<string, string>();



                        var lines = saleData.Split(';');
                        if (lines.Length > 1)
                        {
                            foreach (var line in lines)
                            {
                                var lineparts = line.Split(':');
                                if (lineparts.Length == 2)
                                {
                                    var key = lineparts[0].ToLower().TrimStart().TrimEnd();
                                    if (!string.IsNullOrEmpty(key) && !_dicc.Keys.Contains(key))
                                        _dicc.Add(key, lineparts[1].TrimStart().TrimEnd());
                                }
                                else if (lineparts.Length > 2)
                                {
                                    // si el split tiene mas de dos partes
                                    var key = lineparts[0].ToLower().TrimStart().TrimEnd();

                                    if (!string.IsNullOrEmpty(key) && !_dicc.Keys.Contains(key))
                                    {
                                        var laux = lineparts.ToList();
                                        //quitar la parte de llave para luego poner juntas las partes nuevametne
                                        laux.RemoveAt(0);

                                        var valueaux = String.Join(":", laux.ToArray());
                                        _dicc.Add(key, valueaux);
                                    }
                                }
                                else if (lineparts.Length == 1)
                                {
                                    // si el split tiene mas de dos partes
                                    var key = lineparts[0].ToLower().TrimStart().TrimEnd();
                                    if (!string.IsNullOrEmpty(key) && !_dicc.Keys.Contains(key))
                                        _dicc.Add(key, "");

                                }

                            }
                        }
                        
                        
                        
                        if (_dicc.ContainsKey("pdvrepresented"))
                        {
                            var pdvRep = _dicc["pdvrepresented"];

                            var arr2 = (pdvRep ?? "").Split(' ');


                            if (arr2.Length > 2)
                            {
                                var _arrlist = arr2.ToList();
                                //QUITAL EL ULTIMO 
                                _arrlist.RemoveAt(_arrlist.Count - 1);
                                _dicc.Add("convenio", String.Join(" ", _arrlist.ToArray()));
                            }
                            // SI EL ARREGLO ES UNO
                            else if (arr2.Length > 0)
                            {
                                _dicc.Add("convenio", arr2[0]);
                            }
                            else
                            {
                                _dicc.Add("convenio", "");
                            }
                        }

                        var ProductoId = "";
                        
                        var existsvalue = _dicc.TryGetValue("productoid", out ProductoId);

                        if (!existsvalue)
                        {
                            ProductoId = ConfigurationManager.AppSettings["aptck_defproduct"];
                            logger.WarningHigh("[API] Producto por defecto " + ProductoId);
                        }
                            

                        if (!string.IsNullOrEmpty(ProductoId))
                        {

                            var keyticket = ConfigurationManager.AppSettings[prefix + request.DeviceType + "_" + ProductoId] ?? string.Empty;
                            if (!string.IsNullOrEmpty(keyticket))
                            {
                                return _FormatSaledata(logger, Provider, LOG_PREFIX, request, kinacuTopUpResponse, transactionId, saleData, _dicc, keyticket);
                            }
                           
                        }
                        else
                        {
                            logger.ErrorLow("[API] No existe ProductoId en la impresion del tickete ");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.ErrorLow("[API] " + LOG_PREFIX + "[" + Provider + "] ERROR FormatSaledata [" + ex.Message + "] [" + ex.GetType().Name + "] [" + ex.StackTrace + "]");
                _saleData = saleData;
            }
            return _saleData;
        }


        private static string _FormatSaledata(ILogger logger, string Provider, string LOG_PREFIX, dynamic request, bool kinacuTopUpResponse, int transactionId, string saleData, Dictionary<string, string> _dicc, string keyticket)
        {

            var _saleData = saleData;

            try
            {
          

                var keyticketresult = keyticket;

                var matches = Regex.Matches(keyticket, @"{([a-zA-Z0-9]+\.[a-zA-Z0-9]+|[a-zA-Z0-9]+)}");
                var bandera = true;

                for (int i = 0; i < matches.Count && bandera; i++) //   foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    var match = matches[i];

                    var auxval = match.Value.Substring(1, match.Value.Length - 2);

                    var parts = auxval.Split('.');


                    string firstpart = parts[0].ToLower();
                    string secondpart = "";
                    if (parts.Length > 1)
                        secondpart = parts[1].ToLower(); ;


                    var change = "";

                    /*
                     * DATENOW
                            request
   transactionId
   _dicc
                          */

                    switch (firstpart)
                    {
                        case "datenow":
                      
                            change = DateTime.Now.ToString(ApiTicket_dateformat);

                            break;

                        case "transactionid":

                            change = transactionId.ToString();

                            break;

                        case "dictionary":


                            if (!string.IsNullOrEmpty(secondpart))
                            {

                                var auxvalue = string.Empty;

                                if (_dicc.TryGetValue(secondpart, out auxvalue))
                                {
                                    change = auxvalue;
                                }
                                else
                                {

                                    bandera = false;
                                    logger.ErrorLow("[API] " + LOG_PREFIX + " No se econtro el valor en el diccionario " + match.Value);
                                    // throw new Exception("No se econtro el valor en el diccionario " + match.Value);
                                }


                            }
                            else
                            {

                                bandera = false;
                                logger.ErrorLow("[API] " + LOG_PREFIX + " No se definio la llave para el diccionario " + match.Value);
                                //   throw new Exception("No se definio la llave para el diccionario " + match.Value);
                            }




                            break;

                        default:

                            bandera = false;
                            logger.ErrorLow("[API] " + LOG_PREFIX + " NO ESTA CONTEMPLADO ESTA LLAVE EN EL TEMPLATE " + match.Value);

                            break;

                    }


                    keyticketresult = keyticketresult.Replace(match.Value, change);
                }




                if (!bandera)
                    _saleData = saleData;
                else
                {
                    logger.InfoLow("[API] " + LOG_PREFIX + "[" + Provider + "]  Kinacuticket" + saleData);//+ " " + (_saleData));
                    _saleData = keyticketresult;
                }

                logger.InfoLow("[API] " + LOG_PREFIX + "[" + Provider + "]  FormatSaledata result " + bandera);//+ " " + (_saleData));


            }
            catch (Exception ex)
            {
                logger.ErrorLow("[API] " + LOG_PREFIX + "[" + Provider + "] ERROR Formating FormatSaledata  [" + ex.Message + "] [" + ex.GetType().Name + "] [" + ex.StackTrace + "]");
                _saleData = saleData;
            }





            return _saleData;
        }


      
    }
}