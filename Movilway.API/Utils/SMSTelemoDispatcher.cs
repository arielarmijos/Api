using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Movilway.API.Util
{
    public class SMSTelemoDispatcher
    {


       


        public static string Send(string client_id, string mobile, string message)
        {
            try
            {


                SMSDispatch.DispatchSMSwebservice _service = FactoryDispatchSMSwebservice();

                return _service.send(client_id, mobile, message);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static string SmsToCustomer(string client_id, string customer_id, string mobile, string message)
        {
            try
            {
                SMSDispatch.DispatchSMSwebservice _service = FactoryDispatchSMSwebservice();

                return _service.sms_to_customer(client_id, customer_id, mobile, message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private static SMSDispatch.DispatchSMSwebservice FactoryDispatchSMSwebservice()
        {
            string Url = ConfigurationManager.AppSettings["SMSDispatch"] as String;
            if (String.IsNullOrEmpty(Url))
            {
                throw new Exception("No se ha configurado correctamente la url del Servicio SMSDispatch");
            }
            SMSDispatch.DispatchSMSwebservice _service = new SMSDispatch.DispatchSMSwebservice();
            _service.Url = Url;

            return _service;
        }

    }
}