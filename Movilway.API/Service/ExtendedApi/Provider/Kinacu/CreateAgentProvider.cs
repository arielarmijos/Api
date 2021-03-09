using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using Movilway.API.KinacuWebService;
using System.Text;
using System.Security.Cryptography;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.CreateAgent)]
    public class CreateAgentProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(CreateAgentProvider));
        protected override ILogger ProviderLogger { get { return logger; } }
        private String _GenericError = "NO SE PUDO CREAR EL AGENTE";
        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
                  CreateAgentResponseBody  response = new CreateAgentResponseBody();
            try
            {

                if (sessionID.Equals("0"))
                    return new CreateAgentResponseBody()
                    {
                        ResponseCode = 90,
                        ResponseMessage = "error session",
                        TransactionID = 0,
                        Errors = { new ErrorItem() { ErrorId = "90", ErrorDescription = "Session invalida" } }
                    };

              

                CreateAgentRequestBody request = requestObject as CreateAgentRequestBody;


                logger.InfoLow(()=> TagValue.New().Message( "[API] " + base.LOG_PREFIX + "[CreateAgentProvider]").Tag("[SEND-DATA] createAgentParameters ").Value(request));



                int currentUserId = Utils.GetUserId(request.AuthenticationData.Username); /// OJO
                MD5 md5 = new MD5CryptoServiceProvider();

        
              
              
                var result = Utils.ValidateSoldChildComissions(request.AuthenticationData.Username, request.Commission);
                if (!result.IsObjectValidResult())
                {

                    response.ResponseCode = 100;
                    response.ResponseMessage = "[ERROR DE NEGOCIO] COMISION POR DEPOSITO";
                    response.Errors = new ErrorItems() { new ErrorItem() { ErrorId = "" + result.ResponseCode, ErrorDescription = result.ResponseMessage } };
                    return response;
                }

                var reqAgent = new RequestAgent()
                {
                    age_cuit = request.RIF,
                    age_nombre = request.AgentName,
                    age_razonsocial = request.LegalName,
                    age_direccion = request.Address,
                    age_entrecalles = request.BetweenStreets,
                    age_ciu_id = decimal.Parse(request.City),
                    age_tel = request.Phone,
                    age_cel = request.NumberIMEI,
                    age_email = request.Email,
                    age_contacto = request.ContactPerson,
                    age_subNiveles = decimal.Parse(request.SubLevels),
                    age_pdv = request.Pdv,
                    age_observaciones=request.Notes,

                    ct_id = request.TaxCategory == 0 ? 1 : request.TaxCategory, //Categoria Tributaria 
                    ta_id = 1, //Tipo de agencia
                    sa_id = request.SegmentId == 0 ? 1 : request.SegmentId, // segmento de la agencia

                    age_tipo = "SU",
                    age_autenticaterminal = "N",
                    age_estado = "AC",
                    //Solicitud de operaciones no se debio cambiar
                    age_prefijosrest = "",
                
                    usr_id_modificacion = currentUserId,

                    age_comisionadeposito = request.CommissionableDeposits ? "S" : "N",
                    age_montocomision = request.Commission,
                    limiteCredito = request.CheckingAccountCreditLimit.ToString(),
                    autorizacionAutomatica = request.AutomaticAuthorization.ToString(),
                    quitaAutomatica = request.AutomaticReverse.ToString(),
                    generacionAutomatica = request.AutomaticReposition.ToString(),
                    montoMinimoPorPedido = request.MinimumOrderAmount.ToString(),
                    montoMaximoPorPedido = request.MaximumOrderAmount.ToString(),
                    pedidoMaximoMensual = request.MaximumMonthlyAmount.ToString(),
                    autorizacionAutomaticaMontoDiario = request.MaximumAuthorizedDailyAmount.ToString(),
                    recargaAsincronica = request.AsynchronousTopup.ToString(),
                    comisionporventa = request.SalesCommission,

                    usr_nombre = request.UserName1,
                    usr_apellido = request.UserLastName1,
                    
                    acc_login = request.AccessLogin1,
                    acc_password = Convert.ToBase64String(md5.ComputeHash(Encoding.Unicode.GetBytes(request.AccessPassword1))),
                    //segundo usuario
                    acc_validityDate = DateTime.Now.AddDays(360),
                    second_acc_validityDate = DateTime.Now.AddDays(360),
                    third_acc_validityDate = DateTime.Now.AddDays(360),
                    // primer acceso segundo usuario
                    second_acc_login = request.AccessLogin2,
                    second_acc_password = Convert.ToBase64String(md5.ComputeHash(Encoding.Unicode.GetBytes(request.AccessPassword2))),
                    /*request.AccessType2.ToUpper() == "POS" ? 6 : request.AccessType2.ToUpper() == "WEB" ? 1 : request.AccessType2.ToUpper() == "POSWEB" ? 12 : request.AccessType2.ToUpper() == "USSD" ? 9 : request.AccessType2.ToUpper() == "SMS" ? 2 : 0*/
                    second_tac_id = Utils.GetAccessTypeCode(request.AccessType2),

     



                    usr_administrador = request.IsAdministrator2,
                    grpId = request.Group,
                    //Valores Originales
                    acc_cambiopassword = "N",
                    second_acc_cambiopassword = "N",
                    third_acc_cambiopassword = "N"
                    //

                    //TODO
                    //acc_cambiopassword = "",
                    //second_acc_cambiopassword = ""
                    //
                };

                               //segundo acceso segundo usuario
                if (reqAgent.av_sc_ac_secondUser = request.AvailableSecondAccessSecondUser) { 
                     reqAgent.third_acc_login = request.AccessLogin3;
                     reqAgent.third_acc_password = Convert.ToBase64String(md5.ComputeHash(Encoding.Unicode.GetBytes(request.AccessPassword3)));
                     reqAgent.third_tac_id = Utils.GetAccessTypeCode(request.AccessType3);
                }


              
                    foreach (ProductCommision item in request.ProductsCommission) {
                        reqAgent.productos.Add(new RequestAgent.Product() { prdId = item.ProductId, comision = item.Commission });
                    }

                  
                ResponseAgent responseAgent = Utils.CreateAgent(reqAgent);

                //bool isSuccessful = String.IsNullOrEmpty(request.RIF) ? false : request.RIF.EndsWith("0");
                bool isSuccessful = int.Parse(responseAgent.response_code) == 0;

                    response.ResponseCode = isSuccessful ? 0 : 1;
                    response.ResponseMessage = isSuccessful ? "OK" : "ER";
                    response.TransactionID = 0;
                    response.AgeId = responseAgent.AgeId;

                if (!isSuccessful)
                {
                    var errors = new ErrorItems();
                    errors.Add(new ErrorItem() { ErrorId = responseAgent.response_code, ErrorDescription = responseAgent.message });
                    //var errors = new ErrorItems();
                    //for (int i = 0; i < new Random().Next(1, 6); i++)
                    //    errors.Add(new ErrorItem() { ErrorId = i.ToString(), ErrorDescription = "Error random " + i });
                    response.Errors = errors;
                }

                logger.InfoLow("[QRY] " + base.LOG_PREFIX + "[CreateAgentProvider] [RECV-DATA] CreateAgentResult {response={" + response + "}}");
            }
            catch (Exception ex)
            {

               response.ResponseCode = 500;
               response.ResponseMessage = _GenericError;
               response.TransactionID = 0;
               response.Errors = new ErrorItems() { new ErrorItem() { ErrorId = "90", ErrorDescription = ex.Message } };

               string mensaje = String.Concat("[API] " + base.LOG_PREFIX + "[CreateAgentProvider] ", ". Exception: ", ex.Message, ". ", ex.StackTrace);
               logger.ErrorLow(mensaje);

            }
            return (response);
        }
    }
}