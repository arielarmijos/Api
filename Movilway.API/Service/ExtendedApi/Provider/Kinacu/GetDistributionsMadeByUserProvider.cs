using Movilway.API.KinacuWebService;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    //TODO COMPLETAR
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetUserDistributionList)]
    public class GetDistributionsMadeByUserProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetDistributionsMadeByUserProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        /// <summary>
        /// Retorna los Hijos de un agente dado el ID del agente padre
        /// Precondición:
        /// - Se han validado los permisos en el sistema
        /// Postcondición:
        /// Retorna la lista de BasicAgentInfo con los agentes Hijos 
        /// </summary>
        /// <param name="requestObject"></param>
        /// <param name="kinacuWS"></param>
        /// <param name="sessionID"></param>
        /// <returns>retorna IMovilwayApiResponse con los datos del agente</returns>
        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {

            GetDistributionsMadeByUserResponseBody response = new GetDistributionsMadeByUserResponseBody();// { };
            if (sessionID.Equals("0"))
            {
                response.ResponseCode = 90;
                response.ResponseMessage = "error session";
                response.TransactionID = 0;


                return response;
            }

            //obtener la agencia del usuario logueado retornar 
            //fecha
            
            GetDistributionsMadeByUserRequestBody request = requestObject as GetDistributionsMadeByUserRequestBody;

            try
            {
                var date = request.Date;

                //Nombre del reporte: “Nuevos Clientes”.
                //Fecha y Hora de emisión de reporte (año-mes-día hora:min:seg) 
                //Cuerpo de Reporte:
                //Código de usuario (Quien realizó la distribución -  es decir el user logueado)
                //Nombre usuario
                //Código de Cliente (a quien se realizó la distribución)
                //Nombre del Cliente
                //Valor de saldo distribuido 
                //Fecha y Hora de distribución realizada (año-mes-día hora:min:seg) 


                if (!request.FindByAgency)
                {
                    int currentUserId = Utils.GetUserId(request.AuthenticationData.Username);

                    //response.Distributions = Utils.GetDistributionsMadeByUser(date, currentUserId);
               

                }
                else
                {
                    //string currentUserId = Utils.GetAgentByAccess(request.AuthenticationData.Username).ToString();

                    //response.Distributions = Utils.GetDistributionsMadeByAency(date, currentUserId);

                }
             


                response.ResponseCode = 0;
                response.ResponseMessage = "OK";
            }
            catch (Exception ex)
            {
                logger.ErrorHigh(String.Concat(LOG_PREFIX, " [RECEIVED-DATA] GetDistributionsMadeByUser Provider. ", ex.GetType().FullName + " " + ex.Message + " " + ex.StackTrace));

                response.ResponseCode = 99;
                response.ResponseMessage = "Error. "+ex.Message;
            }
        
         
            //try
            //{


            //    GetChildListRequestByIdBody request = requestObject as GetChildListRequestByIdBody;
            //    /*
            //    dynamic data = new { AgeId = request.AgeId, Login = request.AuthenticationData.Username };
            //    if (!Utils.IsValidEditAgent(data))
            //    {
            //        response.ResponseCode = 90;
            //        response.ResponseMessage = "ERROR DE PERMISOS -  NO TIENE PERMISOS PARA ACCEDER A LOS HIJOS DE ESTE AGENTE";
            //        response.TransactionID = 0;
            //        return response;
            //    }*/


            //    logger.InfoLow(() => TagValue.New().Message("[KIN] " + base.LOG_PREFIX + "[GetChildListProviderById]").Tag("[SEND-DATA] getChildRetailersParameters ").Value(request));

            //    bool extendedValues = request.ExtendedValues ?? false;
            //    List<BasicAgentInfo> results = Utils.GetChildsById(request.AgeId, extendedValues);

            //    response.ChildList.AddRange(results);

            //    response.ResponseCode = 0;
            //    response.ResponseMessage = "OK";
            //    response.TransactionID = 0;


            //}
            //catch (Exception ex)
            //{
            //    response.ResponseCode = 500;
            //    response.ResponseMessage = "ERROR INESPERADO " + ex.Message;
            //    response.TransactionID = 0;

            //}

            //return (response);

           return response;
        }


    }
}