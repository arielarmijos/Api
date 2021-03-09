using Movilway.API.Core;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.UpdateAgent)]
    public class UpdateAgentProvider : AKinacuProvider
    {
        private String _GenericError = "NO SE PUDO ACTUALIZAR EL AGENTE";

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(UpdateAgentProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

       
        /// <summary>
        /// Actualiza un agente en el sistema, tanto en el funente principal como en comisiones
        /// Precondicion: 
        /// -el agente a editar debe ser hijo del agente logeado en el sistema
        /// -los datos del agente estan completos
        /// -los datos de los accesos estan completos, no repetidos y disponibles en el sistema
        /// </summary>
        /// <param name="requestObject"></param>
        /// <param name="kinacuWS"></param>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        /// <returns>Retorna el tipo IMovilwayApiResponse con el respectivo codigo de error y mensaje</returns>
        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, KinacuWebService.SaleInterface kinacuWS, string sessionID)
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
                        Errors = new ErrorItems() { new ErrorItem() { ErrorId = "90", ErrorDescription = "Session invalida" } }
                   };

                CreateAgentRequestBody request = requestObject as CreateAgentRequestBody;


                logger.InfoLow(() => TagValue.New().Message("[API] " + base.LOG_PREFIX + "[UpdateAgentProvider]").Tag("[RCV-DATA] UpdateAgentProviderParameters ").Value(request));


                    dynamic data = new { AgeId = request.AgeId, Login = request.AuthenticationData.Username };
                    if (!Utils.HaveRelationWithAgent(data))
                    {
                       response.ResponseCode = 90;
                       response.ResponseMessage = "ERROR DE PERMISOS";
                       response.TransactionID = 0;
                       response.Errors = new ErrorItems() { new ErrorItem() { ErrorId = "90", ErrorDescription = "NO TIENE PERMISOS PARA EDITAR ESTE REGISTRO" }} ;
                       return response;
                    }


                    //validacion de edicion de productos 
                    //precondicion: se ha validado que el agente es padre del hijo
                    int AgeLogId = Utils.GetAgentIdByAccessPosWeb(request.AuthenticationData.Username);

                    // se descarta que el usuario logoeado no se ha el mismo que se esta editando
                    // tener encuenta casos especiales
                    bool CanEditProdutcs = AgeLogId != request.AgeId;

              
                    var result = Utils.ValidateSoldChildComissions(request.AgeId, request.Commission);//(request.AgeId, AgeLogId, request.Commission);
                  if(!result.IsObjectValidResult() ){
                     response.ResponseCode = 100;
                     response.ResponseMessage = "[ERROR DE NEGOCIO] COMISION POR DEPOSITO";
                     response.Errors = new ErrorItems() { new ErrorItem() { ErrorId = "" + result.ResponseCode, ErrorDescription =  result.ResponseMessage } };
                     return response;
                  }

              

                int currentUserId = Utils.GetUserId(request.AuthenticationData.Username);
                MD5 md5 = new MD5CryptoServiceProvider();

              
                var reqAgent = new RequestAgent()
                {
               
                    age_id = request.AgeId,
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
                    ct_id = request.TaxCategory, //Categoria Tributaria 
                    ta_id = 1, // Tipo de agencia
                    sa_id = request.SegmentId, // segmento de la agencia

                    age_tipo = "SU",
                    age_autenticaterminal = "N",
                    // Se quita este valor por peticion del usuario
                    age_prefijosrest = string.Empty,
                    age_estado = "AC",

                    age_comisionadeposito = request.CommissionableDeposits ? "S" : "N",
                    age_montocomision = request.Commission,
                    //TODO NULL POINTER PROBLEMAS DE CULTIRA
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

                    //segundo usuario
                    //TODO cada ves que se actualiza se extiende el periodo de valdiacion
                    acc_validityDate = DateTime.Now.AddDays(360),
                    second_acc_validityDate = DateTime.Now.AddDays(360),
                    third_acc_validityDate = DateTime.Now.AddDays(360),
                    // primer acceso segundo usuario
                    second_acc_login = request.AccessLogin2,


                    /*request.AccessType2.ToUpper() == "POS" ? 6 : request.AccessType2.ToUpper() == "WEB" ? 1 : request.AccessType2.ToUpper() == "POSWEB" ? 12 : request.AccessType2.ToUpper() == "USSD" ? 9 : request.AccessType2.ToUpper() == "SMS" ? 2 : 0*/


                    //segundo acceso segundo usuario
                    av_sc_ac_secondUser = request.AvailableSecondAccessSecondUser,
                    third_acc_login = request.AccessLogin3,

                    usr_administrador = request.IsAdministrator2,
                    grpId = request.Group,
                    //Valores Originales
                    acc_cambiopassword = "N",
                    second_acc_cambiopassword = "N",
                    third_acc_cambiopassword = "N",


                    usr_id_modificacion = currentUserId,
                    age_modificacion = AgeLogId
                    //
                    //TODO
                    //acc_cambiopassword = "",
                    //second_acc_cambiopassword = ""
                    //
                };

                reqAgent.second_tac_id = Utils.GetAccessTypeCode(request.AccessType2);

                if (!string.IsNullOrEmpty(request.AccessType3))
                    reqAgent.third_tac_id = Utils.GetAccessTypeCode(request.AccessType3);

                if (!string.IsNullOrEmpty(request.AccessPassword1))
                    reqAgent.acc_password = Convert.ToBase64String(md5.ComputeHash(Encoding.Unicode.GetBytes(request.AccessPassword1)));

                if (!string.IsNullOrEmpty(request.AccessPassword2))
                    reqAgent.second_acc_password = Convert.ToBase64String(md5.ComputeHash(Encoding.Unicode.GetBytes(request.AccessPassword2)));

                if (!string.IsNullOrEmpty(request.AccessPassword3))
                    reqAgent.third_acc_password = Convert.ToBase64String(md5.ComputeHash(Encoding.Unicode.GetBytes(request.AccessPassword3)));

                reqAgent.productos = new List<RequestAgent.Product>();

                request.ProductsCommission.ForEach(p =>
                    {
                        reqAgent.productos.Add(new RequestAgent.Product() { prdId = p.ProductId, comision = p.Commission });
                    });



                result = Utils.UpdateAgent(reqAgent, CanEditProdutcs);
                response.ResponseCode = result.ResponseCode;
                response.ResponseMessage = result.ResponseMessage;
                response.TransactionID = 0;

                if (!result.IsObjectValidResult())
                {
                    response.ResponseCode = response.ResponseCode;
                    response.ResponseMessage = _GenericError;

                    var errors = new ErrorItems();
                    //errores del result
                    errors.Add(new ErrorItem() { ErrorId = "" + result.ResponseCode, ErrorDescription = result.ResponseMessage });

                    response.Errors = errors;
                }
        
            }
            catch (Exception ex)
            {
                //cambiar error general erro inesperado

                response.ResponseCode = 500;
                response.ResponseMessage = _GenericError;
                response.TransactionID = 0;
                response.Errors = new ErrorItems() { new ErrorItem() { ErrorId = "90", ErrorDescription = ex.Message } };

                string mensaje = String.Concat("[API] " + base.LOG_PREFIX + "[UpdateAgentProvider] ", ". Exception: ", ex.Message, ". ", ex.StackTrace);
                logger.ErrorLow(mensaje);

            }

            logger.InfoLow(() => TagValue.New().Message("[API] " + base.LOG_PREFIX).Tag("[UpdateAgentProviderResult]").Value(response));
            return (response);
        }

          
    }
}