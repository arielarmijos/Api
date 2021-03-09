using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract.SOSIT;
using Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest;
using Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest.Operations;


namespace Movilway.API.Service.ExtendedApi.Provider.SOSIT
{
    internal partial class SolicitudeProvider : AGenericPlatformAuthentication
    {
        /// <summary>
        /// Crea una nueva Solicitud en SOS IT de Celistics
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la Solicitud a consultar</param>
        /// <returns>Respuesta de la consulta de una determinada solicitud</returns>
        public GetSolicitudeResponse GetSolicitude(GetSolicitudeRequest request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.LogRequest(request);

            GetSolicitudeResponse response = new GetSolicitudeResponse();

            string sessionId = this.GetSessionId(request, response, out this.errorMessage);
            if (this.errorMessage != ErrorMessagesMnemonics.None)
            {
                return response;
            }

            if (!request.IsValidRequest())
            {
                this.SetResponseErrorCode(response, ErrorMessagesMnemonics.InvalidRequiredFields);
                return response;
            }

            GetSolicitudeOperation OperationRequest = new GetSolicitudeOperation();
            //Llamado REST al API SOS IT
            response = (GetSolicitudeResponse)OperationRequest.CallOperation(this.urlApi, this.technicianKey, this.TimeOutSOSIT, Request.EnumOperation.GET_REQUEST, request.Workorderid, -1,-1, null, null);

             if (request.WhitConversations)
             {
                 ListConversationsRequest requestConversation = new ListConversationsRequest();
                 requestConversation.AuthenticationData = request.AuthenticationData;
                 requestConversation.DeviceType = request.DeviceType;
                 requestConversation.Platform = request.Platform;
                 requestConversation.Workorderid = request.Workorderid;
                 ListConversationsResponse responseConversation = ListConversations(requestConversation);

                 if (responseConversation != null && responseConversation.ResponseCode == 0)
                 {
                     response.Conversations = responseConversation.Conversations;               
                 }

             }
           

            return response;
        }
    }
}