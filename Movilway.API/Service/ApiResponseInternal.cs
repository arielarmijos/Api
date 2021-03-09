using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service
{
    public abstract class ApiResponseInternal
    {
        public int ResponseCode { set; get; }
        public String ResponseMessage { set; get; }
        public int TransactionID { set; get; }

        public void SetResponseNamespace(ResponseNamespace responseNamespace)
        {
            ResponseMessage = responseNamespace.ToString() + ":" + ResponseCode + ":" + ResponseMessage;
        }

        private void SetResponseInfo(int ResponseCode, String ResponseMessage, int TransactionID)
        {
            this.ResponseCode = ResponseCode;
            this.ResponseMessage = ResponseMessage;
            this.TransactionID = TransactionID;
        }

        public void SetThrowedException(Exception exception)
        {
            Boolean IsBackEndException = false;
            switch (exception.GetType().ToString())
            {
                case "System.ServiceModel.CommunicationException":
                case "System.ServiceModel.EndpointNotFoundException": 
                    SetResponseInfo(100, "Communication Exception", 0);
                    IsBackEndException = true;
                    break;
                default:
                    SetResponseInfo(99, "Internal Error", 0);
                    break;
            }
            SetResponseNamespace(IsBackEndException ? ResponseNamespace.BAC : ResponseNamespace.API);
        }

        public enum ResponseNamespace
        {
            API,
            BAC,
            IPR,
            MNO
        }
    }
}