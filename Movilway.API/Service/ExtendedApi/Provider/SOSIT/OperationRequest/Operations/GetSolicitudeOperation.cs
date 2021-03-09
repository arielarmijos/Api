using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract.SOSIT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest.Operations
{
    public class GetSolicitudeOperation : OperationRequestBase
    {
        /// <summary>
        /// Campos del XML response de la operacion GET_REQUEST
        /// </summary>
        public static readonly string[] GetcamposxmlOutput = { "workorderid", "createdtime", "subject", "description", "status", "category", "subcategory", "item", "priority", "group" };

        /// <summary>
        /// Obtiene los datos de la petición configurados en el archivo según la operacion
        /// </summary>
        /// <param name="operation">Operacion</param>
        /// <returns>Un objeto <see cref="Movilway.API.Service.ExtendedApi.Provider.SOSIT.OperationRequest.Request">Movilway.API.Service.ExtendedApi.Provider.SOSIT</see> que contiene la petición deseada según el tipo </returns>
        /// 
        protected override Request getRequest(XmlDocument ParamXMLData, int ParamWorkorderid, int detailId, Request Request, string ParamTechnicianKey, string ParamUrl)
        {

            Request.Url = ParamUrl + ParamWorkorderid + "/?OPERATION_NAME=GET_REQUEST&TECHNICIAN_KEY=" + ParamTechnicianKey;
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

            objectData = new GetSolicitudeResponse();
            if (NodeDetails != null && statuscode.Equals("200"))
            {
                ((GetSolicitudeResponse)objectData).ResponseCode = 0;
                ((GetSolicitudeResponse)objectData).ResponseMessage = "Transacción aprobada";

                string[] camposxmlResponseGet = GetcamposxmlOutput;

                foreach (string nameItem in camposxmlResponseGet)
                {
                    XmlNode tempNode = NodeDetails.SelectSingleNode("parameter[name[text() = '" + nameItem + "']]");
                    if (tempNode != null)
                    {
                        string valueItem = tempNode.SelectSingleNode("value").InnerText;

                        if (nameItem.Equals("workorderid"))
                        {
                            ((GetSolicitudeResponse)objectData).Workorderid = valueItem;
                        }
                        else if (nameItem.Equals("createdtime"))
                        {
                            ((GetSolicitudeResponse)objectData).Createdtime = UtilsSOSIT.FromUtcToLocalTime(valueItem); ;
                        }
                        else if (nameItem.Equals("subject"))
                        {
                            ((GetSolicitudeResponse)objectData).Subject = valueItem;
                        }
                        else if (nameItem.Equals("description"))
                        {
                            string tempDesctiption = Encoding.UTF8.GetString(Encoding.Default.GetBytes(valueItem)); 
                            tempDesctiption = Regex.Replace(tempDesctiption, "<br />", "\n").Trim();
                            ((GetSolicitudeResponse)objectData).Description = Regex.Replace(tempDesctiption, @"<[^>]+>|&quot;|&nbsp;", "").Trim();                            
                        }
                        else if (nameItem.Equals("status"))
                        {
                            ((GetSolicitudeResponse)objectData).Status = valueItem;
                        }
                        else if (nameItem.Equals("category"))
                        {
                            ((GetSolicitudeResponse)objectData).Category = valueItem;
                        }
                        else if (nameItem.Equals("subcategory"))
                        {
                            ((GetSolicitudeResponse)objectData).Subcategory = valueItem;
                        }
                        else if (nameItem.Equals("item"))
                        {
                            ((GetSolicitudeResponse)objectData).Item = valueItem;
                        }
                        else if (nameItem.Equals("priority"))
                        {
                            ((GetSolicitudeResponse)objectData).Priority = valueItem;
                        }
                        else if (nameItem.Equals("group"))
                        {
                            ((GetSolicitudeResponse)objectData).Group = valueItem;
                        }
                    }
                }
            }
            else
            {
                ((GetSolicitudeResponse)objectData).ResponseCode = 07;
                ((GetSolicitudeResponse)objectData).ResponseMessage = "No fue posible consultar la solicitud";
            }

            return objectData;
        }



    }
}