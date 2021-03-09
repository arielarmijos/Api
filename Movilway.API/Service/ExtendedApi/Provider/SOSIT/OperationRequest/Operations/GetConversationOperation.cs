using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract.SOSIT;
using System;
using System.Text;
using System.Xml;

namespace Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest.Operations
{
    public class GetConversationOperation : OperationRequestBase
    {
        /// <summary>
        /// Campos del XML response de la operacion GET_CONVERSATION
        /// </summary>
        public static readonly string[] GetcamposxmlOutput = {"conversation id", "title", "description", "toaddress"};


        /// <summary>
        /// Obtiene los datos de la petición configurados en el archivo según la operacion
        /// </summary>
        /// <param name="operation">Operacion</param>
        /// <returns>Un objeto <see cref="Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest.Request">Movilway.API.Service.ExtendedApi.Provider.SOSIT</see> que contiene la petición deseada según el tipo </returns>
        /// 
        protected override Request getRequest(XmlDocument ParamXMLData, int ParamWorkorderid, int detailId, Request Request, string ParamTechnicianKey, string ParamUrl)
        {

            Request.Url = ParamUrl + ParamWorkorderid + "/conversation/" + detailId +
                        "?OPERATION_NAME=GET_CONVERSATION&TECHNICIAN_KEY=" + ParamTechnicianKey;
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

            objectData = new GetConversationResponse();
            if (NodeDetails != null && statuscode.Equals("200"))
            {
                ((GetConversationResponse)objectData).ResponseCode = 0;
                ((GetConversationResponse)objectData).ResponseMessage = "Transacción aprobada";

                string[] camposxmlResponseGet = GetcamposxmlOutput;

                foreach (string nameItem in camposxmlResponseGet)
                {
                    XmlNode tempNode = NodeDetails.SelectSingleNode("parameter[name[text() = '" + nameItem.ToUpper() + "']]");
                    if (tempNode != null)
                    {
                        string valueItem = tempNode.SelectSingleNode("value").InnerText;

                        if (nameItem.Equals("conversation id"))
                        {
                            ((GetConversationResponse)objectData).Conversationid = Convert.ToInt32(valueItem);
                        }
                        else if (nameItem.Equals("title"))
                        {
                            ((GetConversationResponse)objectData).Title = valueItem;
                           
                        }
                        else if (nameItem.Equals("description"))
                        {
                            ((GetConversationResponse)objectData).Description = Encoding.UTF8.GetString(Encoding.Default.GetBytes(valueItem)); 
                        }
                        else if (nameItem.Equals("toaddress"))
                        {
                            ((GetConversationResponse)objectData).Toaddress = valueItem;
                        }
                       
                    }
                }
            }
            else
            {
                ((GetConversationResponse)objectData).ResponseCode = 07;
                ((GetConversationResponse)objectData).ResponseMessage = "No fue posible consultar la conversacion de una solicitud";
            }

            return objectData;

        }


    }
}