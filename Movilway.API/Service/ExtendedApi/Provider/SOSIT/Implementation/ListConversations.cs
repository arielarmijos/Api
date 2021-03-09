using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract.SOSIT;
using Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest;
using Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest.Operations;


namespace Movilway.API.Service.ExtendedApi.Provider.SOSIT
{
    internal partial class SolicitudeProvider : AGenericPlatformAuthentication
    {
        /// <summary>
        /// Lista las Conversaciones asociadas a una  Solicitud en SOS IT de Celistics
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la Solicitud a consultar</param>
        /// <returns>Respuesta de la lista de conversaciones de una solicitud/returns>
        public ListConversationsResponse ListConversations(ListConversationsRequest request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.LogRequest(request);

            ListConversationsResponse response = new ListConversationsResponse();

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

            ListConversationsOperation OperationRequest = new ListConversationsOperation();
            //Llamado REST al API SOS IT
            response = (ListConversationsResponse)OperationRequest.CallOperation(this.urlApi, this.technicianKey, this.TimeOutSOSIT, Request.EnumOperation.GET_ALL_CONVERSATIONS, request.Workorderid, -1,-1, null, null);

            return response;
        }
    }
}