using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract.SOSIT;
using System;
using System.Xml;

namespace Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest.Operations
{
    public class ListNotificationsOperation : OperationRequestBase
    {
        /// <summary>
        /// Campos del XML response de la operacion GET_NOTIFICATIONS
        /// </summary>
        public static readonly string[] GetcamposxmlOutput = { "createddate", "notifyid", "from" };

        /// <summary>
        /// Obtiene los datos de la petición configurados en el archivo según la operacion
        /// </summary>
        /// <param name="operation">Operacion</param>
        /// <returns>Un objeto <see cref="Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest.Request">Movilway.API.Service.ExtendedApi.Provider.SOSIT</see> que contiene la petición deseada según el tipo </returns>
        /// 
        protected override Request getRequest(XmlDocument ParamXMLData, int ParamWorkorderid, int detailId, Request Request, string ParamTechnicianKey, string ParamUrl)
        {

            Request.Url = ParamUrl + ParamWorkorderid + "/notification/?OPERATION_NAME=GET_NOTIFICATIONS&TECHNICIAN_KEY=" + ParamTechnicianKey;
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

            objectData = new ListNotificationsResponse();
            if (NodeDetails != null && statuscode.Equals("200"))
            {
                ((ListNotificationsResponse)objectData).ResponseCode = 0;
                ((ListNotificationsResponse)objectData).ResponseMessage = "Transacción aprobada";
                ((ListNotificationsResponse)objectData).Notifications = new System.Collections.Generic.List<Notification>();

                XmlNodeList listRecordNodes = NodeDetails.SelectNodes("record");
                string[] camposxmlResponseGet = GetcamposxmlOutput;
                foreach (XmlNode auxRecord in listRecordNodes)
                {
                    Notification newNotification = new Notification();
                    foreach (string nameItem in camposxmlResponseGet)
                    {
                        XmlNode tempNode = auxRecord.SelectSingleNode("parameter[name[text() = '" + nameItem + "']]");
                        if (tempNode != null)
                        {
                            string valueItem = tempNode.SelectSingleNode("value").InnerText;
                            if (nameItem.Equals("createddate"))
                            {
                               newNotification.Createddate = UtilsSOSIT.FromUtcToLocalTime(valueItem); ;
                            }
                            else if (nameItem.Equals("notifyid"))
                            {
                                newNotification.Notifyid = Convert.ToInt32(valueItem);
                            }
                            else if (nameItem.Equals("from"))
                            {
                                newNotification.From = valueItem;
                            }                            
                        }
                    }
                    ((ListNotificationsResponse)objectData).Notifications.Add(newNotification);
                }
            }
            else
            {
                ((ListNotificationsResponse)objectData).ResponseCode = 07;
                ((ListNotificationsResponse)objectData).ResponseMessage = "No fue posible consultar el listado de  notas asociadas a una solicitud";
            }
            return objectData;
        }

       
    }
}