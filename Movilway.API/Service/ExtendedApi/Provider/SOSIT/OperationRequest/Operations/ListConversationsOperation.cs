using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract.SOSIT;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest.Operations
{
    public class ListConversationsOperation : OperationRequestBase
    {
        /// <summary>
        /// Campos del XML response de la operacion GET_ALL_CONVERSATIONS
        /// </summary>
        public static readonly string[] GetcamposxmlOutput = { "createddate", "notifyid", "from", "description" };

        /// <summary>
        /// Obtiene los datos de la petición configurados en el archivo según la operacion
        /// </summary>
        /// <param name="operation">Operacion</param>
        /// <returns>Un objeto <see cref="Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest.Request">Movilway.API.Service.ExtendedApi.Provider.SOSIT</see> que contiene la petición deseada según el tipo </returns>
        /// 
        protected override Request getRequest(XmlDocument ParamXMLData, int ParamWorkorderid, int detailId, Request Request, string ParamTechnicianKey, string ParamUrl)
        {
            Request.Url = ParamUrl + ParamWorkorderid + "/conversations/?OPERATION_NAME=GET_ALL_CONVERSATIONS&TECHNICIAN_KEY=" + ParamTechnicianKey;
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

            objectData = new ListConversationsResponse();
            if (NodeDetails != null && statuscode.Equals("200"))
            {
                ((ListConversationsResponse)objectData).ResponseCode = 0;
                ((ListConversationsResponse)objectData).ResponseMessage = "Transacción aprobada";
                ((ListConversationsResponse)objectData).Conversations = new System.Collections.Generic.List<Conversation>();

                XmlNodeList listRecordNodes = NodeDetails.SelectNodes("//record[parameter[name[text() = 'from'] and value[text() != 'System']]]");
               
                string[] camposxmlResponseGet = GetcamposxmlOutput;
                foreach (XmlNode auxRecord in listRecordNodes)
                {
                    Conversation newConversation = new Conversation();
                    foreach (string nameItem in camposxmlResponseGet)
                    {
                        XmlNode tempNode = auxRecord.SelectSingleNode("parameter[name[text() = '" + nameItem + "']]");
                        if (tempNode != null)
                        {
                            string valueItem = tempNode.SelectSingleNode("value").InnerText;
                            if (nameItem.Equals("createddate"))
                            {
                               newConversation.Createddate = UtilsSOSIT.FromUtcToLocalTime(valueItem); ;
                            }
                            else if (nameItem.Equals("notifyid"))
                            {
                                newConversation.Notifyid = Convert.ToInt32(valueItem);
                            }
                            else if (nameItem.Equals("from"))
                            {
                                newConversation.From = valueItem;
                            }
                            else if (nameItem.Equals("description"))
                            {
                                string tempDesctiption = Encoding.UTF8.GetString(Encoding.Default.GetBytes(valueItem));
                                tempDesctiption = Regex.Replace(tempDesctiption, "<br />", "\n").Trim();
                                tempDesctiption = Regex.Replace(tempDesctiption, "<br>", "\n").Trim();
                                newConversation.Description = Regex.Replace(tempDesctiption, @"<[^>]+>|&quot;|&nbsp;", "").Trim();
                                
                            }
                        }
                    }
                    ((ListConversationsResponse)objectData).Conversations.Add(newConversation);
                }
            }
            else
            {
                ((ListConversationsResponse)objectData).ResponseCode = 07;
                ((ListConversationsResponse)objectData).ResponseMessage = "No fue posible consultar el listado de  conversaciones de una solicitud";
            }
            return objectData;
        }
    }
}