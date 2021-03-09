using Movilway.API.Core.Security;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Movilway.API.Core;
using Movilway.Logging;

namespace Movilway.API.Service.ExtendedApi.Security
{
    public class SecureAccessValidator
    {



        private readonly  string  Prefix = "TIME_TOKKEN";
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(SecureAccessValidator));


        private string LOG_PREFIX = HttpContext.Current.Session["LOG_PREFIX"].ToString();

        private TimeSpan t = TimeSpan.FromSeconds(12);

        public SecureAccessValidator()
        {


        }

        public SecureAccessValidator(string PosfixConfiguration)
        {
    //        <add key="TIME_TOKKEN_TopUpProvider" value="0:02:00"  />
    //<add key="TIME_TOKKEN_TransferProvider" value="0:02:00"  />
    //<add key="TIME_TOKKEN" value="0:02:00"  />
            string key = ConfigurationManager.AppSettings[String.Concat(Prefix, "_", PosfixConfiguration)] ?? ConfigurationManager.AppSettings[String.Concat( PosfixConfiguration)]?? "";

               if (!string.IsNullOrEmpty(key))
               {
                   t = TimeSpan.Parse(key);
               }
        }



        /// <summary>
        /// Valida que el acceso este activo y se ha valido
        /// </summary>
        /// <returns></returns>
        public void IsValidAccess(IMovilwayApiRequest request)//GenericApiResult<bool>
        {

            logger.InfoHigh(String.Concat(LOG_PREFIX," IsValidAccess "));
            bool acceso = true;
          //  GenericApiResult<bool> result = new GenericApiResult<bool>(true);
            TrustedDevice device = null;
            //TODO quitar
            device = new TrustedDevice()
            {
                Status = cons.DEVICE_ACTIVE
            };
             string message = "";


            try
            {
               


                if (string.IsNullOrEmpty(request.AuthenticationData.Tokken))
                {
                    message = "NO SE ENVIO EL TOKKEN DE AUTENTICACION";
                    logger.ErrorHigh(String.Concat(LOG_PREFIX, message));

                    throw new Exception(message);
                }

                String jsonDecrypt = Cryptography.decrypt(request.AuthenticationData.Tokken);

                logger.InfoHigh(()=>TagValue.New().Message(String.Concat(LOG_PREFIX, " ACCESS TOKKEN  ")).Tag("DATA").Value(jsonDecrypt));

                Dictionary<string, string> dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecrypt);

                DateTime NowUtc = DateTime.UtcNow;
                DateTime AccessDtUtc = DateTime.ParseExact(dictionary["Fecha"], "yyyyMMdd HH:mm:ss", null);

                logger.InfoHigh(String.Concat("FECHAS UTC ", NowUtc.ToString("yyyyMMdd HH:mm:ss"), " - ", AccessDtUtc.ToString("yyyyMMdd HH:mm:ss")));


             


                if((NowUtc- AccessDtUtc) > t){
                    throw  new SecurityException("EL TIEMPO DEL TOKKEN SE HA EXPIRADO");
                }

//                // buscar en los accessos de la base de datos
//                string strConnString = ConfigurationManager.ConnectionStrings["SECURE_DB"].ConnectionString;
//                using (SqlConnection mySqlConnection = new SqlConnection(strConnString))
//                {


//                    mySqlConnection.Open();
//                    SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
//                    //select by serial
//                    mySqlCommand.CommandText = @"
//                        SELECT        Device.DeviceId, Device.UserId, Device.Token, Device.Hash, Device.DeviceTypeId, Device.FriendlyName, Device.Description, Device.DateActivated, Device.Status, Device.LastAccess, Device.DateBlocked, 
//                         Device.Model, Device.OS, DeviceType.Name AS Type
//FROM            Device INNER JOIN
//                         DeviceType ON Device.DeviceTypeId = DeviceType.DeviceTypeId WHERE   Device.Token = @SerialTokken AND Device.UserId = @userId ";

//                    //mySqlCommand.Parameters.AddWithValue("@SerialTokken", token);
//                    //mySqlCommand.Parameters.AddWithValue("@userId", userid);

                     
//                    using (var reader = mySqlCommand.ExecuteReader())
//                    {
//                        if (reader.HasRows && reader.Read())
//                        {
//                            device = new TrustedDevice()
//                              {
//                                  ID = (long)((int)reader["DeviceId"]),
//                                  UserId = (int)reader["UserId"],
//                                  Token = (string)reader["Token"],
//                                  Hash = (string)reader["Hash"],
//                                  //--
//                                  IdType = (int)reader["DeviceTypeId"],
//                                  Type = (string)reader["Type"],
//                                  //--
//                                  FriendlyName = (string)reader["FriendlyName"],
//                                  DateActivated = (DateTime)reader["DateActivated"],
//                                  Status = (Int16)reader["Status"],
//                              };

//                        }
//                        else
//                            throw new Exception("NO SE ECONTRARON DATOS DEL ACCESO SEGUN LA AUTENTICACION");

//                    }
//                }


                if (device.Status != cons.DEVICE_ACTIVE)
                {
                
                    message = "ACCESSO NO VALIDO";
                    logger.ErrorHigh(String.Concat(LOG_PREFIX, message));
                    //RECONSIDERAR RETORNULL 
                    throw new SecurityException(message);
                }



            }

            catch (Exception ex)
            {
                if (!(ex is SecurityException))
                logger.InfoHigh(String.Concat(LOG_PREFIX, " ERROR INESPERADO VALIDANDO ACCESO ",ex.Message," - ",ex.StackTrace));

                throw ;
            }

        
        }

        public static void ValidAccess(IMovilwayApiRequest request)
        {
            if (ApiConfiguration.IS_AVAILABLE_SECURITY)
                if (request is ASecuredFinancialApiRequest)
                {
                    SecureAccessValidator validator = new SecureAccessValidator();

                    validator.IsValidAccess(request);
                   
                }
        }
    }
}