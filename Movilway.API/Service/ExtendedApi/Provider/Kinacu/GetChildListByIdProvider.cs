using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using Movilway.API.KinacuWebService;
using Movilway.API.KinacuLogisticsWebService;
using System.Text;


namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetChildListById)]
    public class GetChildListByIdProvider : AKinacuProvider
    {
            private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetChildListByIdProvider));
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

            GetChildListResponseBody response = new GetChildListResponseBody();
            if (sessionID.Equals("0")){
                response.ResponseCode = 90;
               response.ResponseMessage = "error session";
                response.TransactionID = 0;


                return response;
                }


            try
            {

         
                GetChildListRequestByIdBody request = requestObject as GetChildListRequestByIdBody;
                /*
                dynamic data = new { AgeId = request.AgeId, Login = request.AuthenticationData.Username };
                if (!Utils.IsValidEditAgent(data))
                {
                    response.ResponseCode = 90;
                    response.ResponseMessage = "ERROR DE PERMISOS -  NO TIENE PERMISOS PARA ACCEDER A LOS HIJOS DE ESTE AGENTE";
                    response.TransactionID = 0;
                    return response;
                }*/


                logger.InfoLow(() => TagValue.New().Message("[KIN] " + base.LOG_PREFIX + "[GetChildListProviderById]").Tag("[SEND-DATA] getChildRetailersParameters ").Value(request));

                bool extendedValues = request.ExtendedValues ?? false;
                List<BasicAgentInfo> results  = Utils.GetChildsById(request.AgeId, extendedValues);

                response.ChildList.AddRange(results);

                response.ResponseCode = 0;
                response.ResponseMessage = "OK";
                response.TransactionID = 0;
                
    
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = "ERROR INESPERADO "+ex.Message;
                response.TransactionID = 0;
                
            }

            return (response);
        }

        

    }
    }