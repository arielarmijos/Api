using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract.SOSIT;
using System;
using System.Xml;

namespace Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest.Operations
{
    public class AddSolicitudeOperation : OperationRequestBase
    {
        /// <summary>
        /// Campos del INPUT_DATA = XML para la creacion de Solicitudes ADD_REQUEST
        /// </summary>
        public readonly string[] Addcamposxmlinput = { "subject", "description", "site", "category", "subcategory", "Request Type", "priority", "Service Category", "item" };



        /// <summary>
        /// Obtiene los datos de la petición configurados en el archivo según la operacion
        /// </summary>
        /// <param name="operation">Operacion</param>
        /// <returns>Un objeto <see cref="Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest.Request">Movilway.API.Service.ExtendedApi.Provider.SOSIT</see> que contiene la petición deseada según el tipo </returns>
        /// 
        protected override Request getRequest(XmlDocument ParamXMLData, int ParamWorkorderid, int detailId, Request Request, string ParamTechnicianKey, string ParamUrl)
        {

            Request.Url = ParamUrl + "?OPERATION_NAME=ADD_REQUEST&TECHNICIAN_KEY=" + ParamTechnicianKey + "&format=XML&INPUT_DATA=" + ParamXMLData.InnerXml;
            Request.Host = ParamUrl;
            Request.Method = "POST";
            Request.ContentType = @"application/xml; charset=utf-8";
            Request.KeepAlive = true;
            Request.Referer = Request.Host;

            return Request;
        }


        /// <summary>
        /// Crea el response del API Movilway dependiendo de la operacion 
        /// </summary>
        protected override AGenericApiResponse CreateResponseObject(string PageContent, int BranchId = -1, string CountryAcronym = null)
        {
            AGenericApiResponse objectData = null;
            XmlDocument xmlDoc = new XmlDocument();
            string statuscode = "";
            string message = "";
            
            xmlDoc.LoadXml(PageContent);
            statuscode = xmlDoc.DocumentElement.SelectSingleNode("response/operation/result/statuscode").InnerText;
            message = xmlDoc.DocumentElement.SelectSingleNode("response/operation/result/message").InnerText;

            XmlNode NodeworkorderidAdd = xmlDoc.DocumentElement.SelectSingleNode("response/operation/Details/workorderid");

            objectData = new AddSolicitudeResponse();
            if (NodeworkorderidAdd != null && statuscode.Equals("200"))
            {
                ((AddSolicitudeResponse)objectData).ResponseCode = 0;
                ((AddSolicitudeResponse)objectData).ResponseMessage = "Transacción aprobada";
                ((AddSolicitudeResponse)objectData).Workorderid = NodeworkorderidAdd.InnerText;
            }
            else
            {
                ((AddSolicitudeResponse)objectData).ResponseCode = 07;
                ((AddSolicitudeResponse)objectData).ResponseMessage = "No fue posible crear la solicitud";
                ((AddSolicitudeResponse)objectData).Workorderid = "-1";
            }
            return objectData;
        }

        
    }
}