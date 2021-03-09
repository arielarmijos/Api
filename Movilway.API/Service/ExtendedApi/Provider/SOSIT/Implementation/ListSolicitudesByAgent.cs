using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract.SOSIT;
using Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest;
using Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest.Operations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml;

namespace Movilway.API.Service.ExtendedApi.Provider.SOSIT
{
    internal partial class SolicitudeProvider : AGenericPlatformAuthentication
    {

        /// <summary>
        /// Lista dodas las solicitudes asociadas a una agencia
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información de la Solicitud</param>
        /// <returns>Respuesta de la consulta de las solicitudes asociadas a una agencia</returns>
        public ListSolicitudesResponse ListSolicitudesByAgent(ListSolicitudesRequest request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.LogRequest(request);

            ListSolicitudesResponse response = new ListSolicitudesResponse();

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

            ListSolicitudesOperation OperationRequest = new ListSolicitudesOperation();

            string[] filtercamposxmlInput = OperationRequest.FiltercamposxmlInput;
            XmlDocument DataInputXML = FilterXmlDataInput(request, filtercamposxmlInput); 

            //llamado REST al API SOS IT
            response = (ListSolicitudesResponse)OperationRequest.CallOperation(this.urlApi, this.technicianKey, this.TimeOutSOSIT, Request.EnumOperation.GET_REQUESTS, -1, -1, request.Branchid, request.CountryAcronym, DataInputXML);

            //List<Solicitude> listFilter = response.Solicitudes.Where(x => x.subject.Contains("[" + request.CountryAcronym + "-" + request.Branchid + "]")).ToList();

            //response.Solicitudes = listFilter;

            return response;
        }

        private XmlDocument FilterXmlDataInput(ListSolicitudesRequest request, string[] ParamAddcamposxmlinput)
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
                GetValueNodeFilter(request, nodeValue, nameItem);
                nodeParameter.AppendChild(nodeName);
                nodeParameter.AppendChild(nodeValue);
                nodeRoot.AppendChild(nodeParameter);
            }
            return DataInputXML;
        }

        private static void GetValueNodeFilter(ListSolicitudesRequest request, XmlElement nodeValue, string nameItem)
        {
            switch (nameItem)
            {
                case "from":
                    nodeValue.InnerText = request.from;
                    break;
                case "limit":
                    nodeValue.InnerText = request.limit;
                    break;
                case "filterby":
                    nodeValue.InnerText = request.filterby;
                    break;
            }
        }
    }
}