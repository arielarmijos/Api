using Movilway.API.Core;
using Movilway.API.KinacuWebService;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{

    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetAgent)]
    public class GetAgentProvider : AKinacuProvider
    {



        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetAgentProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        /// <summary>
        /// Retorna los datos del Agente en la clase AgentEdit, con el fin de editar el agente
        /// Precondicion:
        /// -El agente existe en el sistema
        /// Postcondicion:
        /// Se Obtienen los datos del agente necesarios para la pantalla de edicion

        /// </summary>
        /// <param name="requestObject"></param>
        /// <param name="kinacuWS"></param>
        /// <param name="sessionID"></param>
        /// <returns>Retorna un IMovilwayApiResponse con los datos del Agente a editar</returns>
        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, string sessionID)
        {


            GetAgentResponseBody response = new GetAgentResponseBody();
            if (sessionID.Equals("0"))
            {
                response.ResponseCode = 90;
                response.ResponseMessage = "SESIÓN INVALIDA";
                return response;
            }

  
            try
            {
                GetAgentRequestBody request = requestObject as GetAgentRequestBody;
                RequestAgent reqAgent = null;

                logger.InfoLow(() => TagValue.New().Message("[API] " + base.LOG_PREFIX + "[GetAgentEdit]").Tag("[SEND-DATA] GetAgentRequestParameters ").Value(request));

                /*
                dynamic data = new { AgeId = request.AgeId, Login = request.AuthenticationData.Username };
                if (!Utils.HaveRelationWithAgent(data))
                {
                    response.ResponseCode = 90;
                    response.ResponseMessage = "ERROR DE PERMISOS";
                    response.TransactionID = 0;
                    response.Errors = new ErrorItems() { new ErrorItem() { ErrorId = "90", ErrorDescription = "NO TIENE PERMISOS PARA EDITAR ESTE REGISTRO" } };

                    return response;
                }*/


                GenericApiResult<RequestAgent> result = Utils.GetAgentEditById(request.AgeId);

                //falta builder
                response.ResponseCode = result.ResponseCode;
                response.ResponseMessage = result.ResponseMessage;

                if (result.IsObjectValidResult())
                {


                    reqAgent = result.ObjectResult; //result["Obj_Result"] as RequestAgent;
                    response.Agente = new AgentEdit()
                    {

                        RIF = reqAgent.age_cuit,
                        AgentName = reqAgent.age_nombre,
                        LegalName = reqAgent.age_razonsocial,
                        Address = reqAgent.age_direccion,
                        BetweenStreets = reqAgent.age_entrecalles,
                        City = reqAgent.age_ciu_id.ToString(CultureInfo.InvariantCulture),
                        Province = reqAgent.pro_id.ToString(CultureInfo.InvariantCulture),
                        Country = reqAgent.pai_id.ToString(CultureInfo.InvariantCulture),
                        Phone = reqAgent.age_tel,
                        NumberIMEI = reqAgent.age_cel,
                        Email = reqAgent.age_email,
                        ContactPerson = reqAgent.age_contacto,
                        SubLevels = reqAgent.age_subNiveles.ToString(CultureInfo.InvariantCulture),
                        Pdv = reqAgent.age_pdv,
                        CommissionableDeposits = reqAgent.age_comisionadeposito != null && reqAgent.age_comisionadeposito.Equals("S"), //? "S" : "N" ,
                        Commission =reqAgent.age_montocomision,
                         Notes=reqAgent.age_observaciones,
                        //primer usuario
                        UserName1 = reqAgent.usr_nombre,
                        UserLastName1 = reqAgent.usr_apellido,
                        AccessLogin1 = reqAgent.acc_login,

                        //segundo acceso segundo usuario
                        //por defecto login es vacio
                        AccessLogin2 = reqAgent.second_acc_login,
                        //validacion pueden haber agentes con solo un acceso
                        AccessType2 = reqAgent.second_tac_id > 0 ? Utils.GetAccessName(Convert.ToInt32(reqAgent.second_tac_id)) : "",

                        //tercer acceso segundo usuario
                        AvailableSecondAccessSecondUser = reqAgent.av_sc_ac_secondUser,
                        AccessLogin3 = reqAgent.third_acc_login,
                     

                        IsAdministrator2 = reqAgent.usr_administrador,
                        Group = Convert.ToInt32(reqAgent.grpId),

                        //booleano que indica si tiene comision
                        SalesCommission = reqAgent.comisionporventa,
                        //productos comision
                        AgeId = reqAgent.age_id,
                        TaxCategory = reqAgent.ct_id == 0 ? 1 : Convert.ToDecimal(reqAgent.ct_id),
                        SegmentId = reqAgent.sa_id == 0 ? 1 : Convert.ToDecimal(reqAgent.sa_id)
                    };

                    //TODO ABOUT CHEK ALL PRODUCTS
                    //llamar a product lis GetProductListProvider (ApiServiceName.GetProductList)
                    //hacer la logica para saber si todos los productos estan chequeados o no 

                    try 
                    { 

                       if (reqAgent.third_tac_id != 0)
                        response.Agente.AccessType3 = Utils.GetAccessName(Convert.ToInt32(reqAgent.third_tac_id));
                    }
                    catch (Exception ex)
                    {
                        logger.InfoLow(() => TagValue.New().Tag("ERROR OBTENIENDO NOMBRE DEL AGENTE").Exception(ex));
                    }

                    response.Agente.AutomaticAuthorization = Convert.ToBoolean(reqAgent.autorizacionAutomatica);
                    response.Agente.AutomaticReverse = Convert.ToBoolean(reqAgent.quitaAutomatica);
                    response.Agente.AutomaticReposition = Convert.ToBoolean(reqAgent.generacionAutomatica);
                    response.Agente.AsynchronousTopup = Convert.ToBoolean(reqAgent.recargaAsincronica);

                    //evitar problemas con la cultura

                    response.Agente.CheckingAccountCreditLimit = Convert.ToDecimal(reqAgent.limiteCredito);
                    response.Agente.MinimumOrderAmount = Convert.ToDecimal(reqAgent.montoMinimoPorPedido);
                    response.Agente.MaximumOrderAmount = Convert.ToDecimal(reqAgent.montoMaximoPorPedido);
                    response.Agente.MaximumMonthlyAmount = Convert.ToDecimal(reqAgent.pedidoMaximoMensual);
                    response.Agente.MaximumAuthorizedDailyAmount = Convert.ToDecimal(reqAgent.autorizacionAutomaticaMontoDiario);

                    response.Agente.ProductsCommission = new List<ProductCommision>();


                    if (reqAgent.productos != null && reqAgent.productos.Count() > 0)
                    {
                        reqAgent.productos.ForEach(p => { response.Agente.ProductsCommission.Add(new ProductCommision() { ProductId = p.prdId, Commission = p.comision }); });
                    }



                }

            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = "ERROR INESPERADO OBTENIDO AGENTE " + ex.Message;

                logger.ErrorLow(String.Concat("[QRY] ", LOG_PREFIX, " ", response.ResponseCode, "-", response.ResponseMessage, ". Exception: ", ex.Message, ". ", ex.StackTrace));
            }


            return response;
        }
    }
}
