using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract.SOSIT;
using Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest;
using Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest.Operations;
using Movilway.Logging;
using System.Xml;



namespace Movilway.API.Service.ExtendedApi.Provider.SOSIT
{
    internal partial class SolicitudeProvider : AGenericPlatformAuthentication
    {     
        /// <summary>
        /// Crea una nueva Solicitud en SOS IT de Celistics
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la Solicitud</param>
        /// <returns>Respuesta de la creacio de solicitud</returns>
        public AddSolicitudeResponse AddSolicitude(AddSolicitudeRequest request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.LogRequest(request);            
                   
            AddSolicitudeResponse response = new AddSolicitudeResponse();

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

            AddSolicitudeOperation OperationRequest = new AddSolicitudeOperation();

            string[] Addcamposxmlinput = OperationRequest.Addcamposxmlinput;
            XmlDocument DataInputXML = CreateXmlDataInput(request, Addcamposxmlinput); ;
            
            //llamado REST al API SOS IT
            response = (AddSolicitudeResponse)OperationRequest.CallOperation(this.urlApi, this.technicianKey, this.TimeOutSOSIT, Request.EnumOperation.ADD_REQUEST, -1,-1, -1, null,DataInputXML);

            return response;
        }

        private XmlDocument CreateXmlDataInput(AddSolicitudeRequest request, string[] ParamAddcamposxmlinput)
        {
            XmlDocument DataInputXML = new XmlDocument();

            DataInputXML.LoadXml("<Operation><Details></Details></Operation>");
            XmlNode nodeRoot = DataInputXML.SelectSingleNode("Operation/Details");
            XmlElement nodeParameter = null;
            XmlElement nodeName = null;
            XmlElement nodeValue = null;
            string[] camposxmlinputAdd = ParamAddcamposxmlinput;

            for (int i = 0; i < camposxmlinputAdd.Length; i++)
            {
                string nameItem = camposxmlinputAdd[i];
                nodeParameter = DataInputXML.CreateElement("parameter");
                nodeName = DataInputXML.CreateElement("name");
                nodeName.InnerText = nameItem;
                nodeValue = DataInputXML.CreateElement("value");
                GetNodeValue(request, nodeValue, nameItem);                
                nodeParameter.AppendChild(nodeName);
                nodeParameter.AppendChild(nodeValue);
                nodeRoot.AppendChild(nodeParameter);
            }

            return DataInputXML;
        }

        private static void GetNodeValue(AddSolicitudeRequest request, XmlElement nodeValue, string nameItem)
        {
            switch (nameItem)
            {
                case "subject":
                    nodeValue.InnerText = request.Subject;
                    break;
                case "description":
                    nodeValue.InnerText = request.Description;
                    break;
                case "site":
                    nodeValue.InnerText = request.Site;
                    break;
                case "category":
                    nodeValue.InnerText = request.Category;
                    break;
                case "subcategory":
                    nodeValue.InnerText = request.Subcategory;
                    break;
                case "priority":
                    nodeValue.InnerText = request.Priority;
                    break;
                case "Service Category":
                    nodeValue.InnerText = request.ServiceCategory;
                    break;
                case "item":
                    nodeValue.InnerText = request.Item;
                    break;
                case "Request Type":
                    nodeValue.InnerText = request.RequestType;
                    break;
            }
        }
    }
}