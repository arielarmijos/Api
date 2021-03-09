using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract.SOSIT;
using System;
using System.Xml;

namespace Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest.Operations
{
    public class ListSolicitudesOperation : OperationRequestBase
    {
        /// <summary>
        /// Campos del XML response de la operacion GET_REQUESTS
        /// </summary>
        public static readonly string[] GetcamposxmlOutput = { "workorderid", "requester", "createdby", "createdtime", "subject", "technician", "priority", "status" };

        /// <summary>
        /// Campos del XML asociadoa los filtros de la cionsulta de la operacion GET_REQUESTS
        /// </summary>
        public readonly string[] FiltercamposxmlInput = { "from", "limit", "filterby" };


        /// <summary>
        /// Obtiene los datos de la petición configurados en el archivo según la operacion
        /// </summary>
        /// <param name="operation">Operacion</param>
        /// <returns>Un objeto <see cref="Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest.Request">Movilway.API.Service.ExtendedApi.Provider.SOSIT</see> que contiene la petición deseada según el tipo </returns>
        /// 
        protected override Request getRequest(XmlDocument ParamXMLData, int ParamWorkorderid, int detailId, Request Request, string ParamTechnicianKey, string ParamUrl)
        {
            Request.Url = ParamUrl + "?OPERATION_NAME=GET_REQUESTS&TECHNICIAN_KEY=" + ParamTechnicianKey + "&format=XML&INPUT_DATA=" + ParamXMLData.InnerXml;
            Request.Host = ParamUrl;
            Request.Method = "GET";
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

            XmlNode NodeDetails = xmlDoc.DocumentElement.SelectSingleNode("response/operation/Details");

            objectData = new ListSolicitudesResponse();
            if (NodeDetails != null && statuscode.Equals("200"))
            {
                ((ListSolicitudesResponse)objectData).ResponseCode = 0;
                ((ListSolicitudesResponse)objectData).ResponseMessage = "Transacción aprobada";
                ((ListSolicitudesResponse)objectData).Solicitudes = new System.Collections.Generic.List<Solicitude>();

                XmlNodeList listRecordNodes = NodeDetails.SelectNodes("//record[parameter[contains(value, '[" + CountryAcronym + "-" + BranchId + "]')]]");
                string[] camposxmlResponseGet = GetcamposxmlOutput;
                foreach (XmlNode auxRecord in listRecordNodes)
                {
                    Solicitude newSolicitude = new Solicitude();
                    foreach (string nameItem in camposxmlResponseGet)
                    {
                        XmlNode tempNode = auxRecord.SelectSingleNode("parameter[name[text() = '" + nameItem + "']]");
                        if (tempNode != null)
                        {
                            string valueItem = tempNode.SelectSingleNode("value").InnerText;
                            if (nameItem.Equals("workorderid"))
                            {
                                newSolicitude.workorderid = Convert.ToInt32(valueItem);
                            }
                            else if (nameItem.Equals("requester"))
                            {
                                newSolicitude.requester = valueItem;
                            }
                            else if (nameItem.Equals("createdby"))
                            {
                                newSolicitude.createdby = valueItem;
                            }
                            else if (nameItem.Equals("createdtime"))
                            {                             
                                newSolicitude.createdtime = UtilsSOSIT.FromUtcToLocalTime(valueItem); ;
                            }
                            else if (nameItem.Equals("subject"))
                            {
                                newSolicitude.subject = valueItem;
                            }
                            else if (nameItem.Equals("technician"))
                            {
                                newSolicitude.technician = valueItem;
                            }
                            else if (nameItem.Equals("priority"))
                            {
                                newSolicitude.priority = valueItem;
                            }
                            else if (nameItem.Equals("status"))
                            {
                                newSolicitude.status = valueItem;
                            }
                        }
                    }
                    ((ListSolicitudesResponse)objectData).Solicitudes.Add(newSolicitude);
                }
            }
            else
            {
                ((ListSolicitudesResponse)objectData).ResponseCode = 07;
                ((ListSolicitudesResponse)objectData).ResponseMessage = "No fue posible consultar el listado de  conversaciones de una solicitud";
            }
            return objectData;
        }


    }
}