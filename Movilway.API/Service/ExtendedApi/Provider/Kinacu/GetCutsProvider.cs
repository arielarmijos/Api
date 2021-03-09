using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Movilway.API.KinacuWebService;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using Movilway.API.Service.ExtendedApi.DataContract;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    //TODO validar provider
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetCutsProvider)]
    public class GetCutsProvider : AKinacuProvider
    {

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetCutsProvider));

        protected override ILogger ProviderLogger { get { return logger; } }



        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {



            GetCutsResponseBody response = new GetCutsResponseBody();
            if (sessionID.Equals("0"))
            {
                response.ResponseCode = 90;
                response.ResponseMessage = "error session";
                response.TransactionID = 0;
                return response;
            }





            try
            {

                GetCutsRequestBody request = requestObject as GetCutsRequestBody;


                //Código de transacción de corte manual
                //Código de usuario (Quien realizó el corte)
                //Nombre usuario
                //Código de Cliente (a quien se realizó la distribución)
                //Nombre del Cliente
                //Fecha y Hora de transacción de corte 
                //Código de Producto
                //Descripción de Producto
                //Cantidad de Pines ( 1 PIN = 1 Dólar) – Total por operadora
                //Unidad (Fracción)

                //Ariel 2021-Ma-09 Comentado 
              //  response =   Utils.GetConsolidatedCuts(request);



            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = "ERROR INESPERADO " + ex.Message;
                response.TransactionID = 0;

            }


            return response;
        }
    }
}