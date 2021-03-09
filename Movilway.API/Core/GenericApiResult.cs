using Movilway.API.Service.ExtendedApi.DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Core
{
    /// <summary>
    /// Wrapper para cualquier tipo de respuesta en el api
    /// </summary>
    /// <typeparam name="T">Object Creado para cualquier proceso de negocio</typeparam>
    public class GenericApiResult<T> : Movilway.API.Service.ExtendedApi.DataContract.Common.IMovilwayApiResponse
    {

        //public static GenericApiResult<T> FactoryGenericApiResult(T data = default(T), int code = 0, string Message= "OK")
        //{
        //    GenericApiResult<T> result = new GenericApiResult<T>();
        //    result.ObjectResult = data;
        //    return result;
        //}

        public T ObjectResult { get; set; }

        public int? ResponseCode
        {
            get;
            set;
        }

        public string ResponseMessage
        {
            get;
            set;
        }

        public int? TransactionID
        {
            get;
            set;
        }

        public GenericApiResult()
        {
            //evitar valores nulos
            //por defecto no es valido el resultado
            ResponseCode = 99;
            ResponseMessage = "";
            TransactionID = 0;
        }

        public GenericApiResult(T t)
            : this()
        {
            //evitar valores nulos
            ObjectResult = t;

        }

        /// <summary>
        /// Determina si el codigo de resultado es de una operacion correcta
        /// </summary>
        /// <returns></returns>
        public bool IsValidResult()
        {
            return ResponseCode == 0;
        }

        /// <summary>
        /// Determina si el resultado es valido segun las reglas de movilway
        /// existe objectResult diferente de null
        /// y ResponseCode es cero
        /// </summary>
        /// <returns></returns>
        public bool IsObjectValidResult()
        {
            bool result = false;
            result = ResponseCode == 0 && ObjectResult != null && (ObjectResult is Boolean ? Convert.ToBoolean(ObjectResult) : true);
            return result;
        }

        // se puede incluir el resultado un metodo protegido como delegado para
        // su sobre carga retorne un bool indicado la valides
        // protected bool ValidateResult()
        //{
        //    return true;
        //}

        public override string ToString()
        {
            return Movilway.API.Service.ExtendedApi.DataContract.Utils.logFormat(this);
        }



        /// <summary>
        /// Asigna los valores de la operacion en caso de que la operacion fue erronea
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ResponseCode"></param>
        /// <param name="ResponseMessage"></param>
        /// <param name="TransactionID"></param>
        public void SetResultData(T t, int ResponseCode = UtilResut.OK_RESPONSE_CODE, string ResponseMessage = "OK", int TransactionID = 0)
        {
            ObjectResult = t;
            this.ResponseCode = ResponseCode != UtilResut.OK_RESPONSE_CODE ? UtilResut.BussErrorCode(ResponseCode) : ResponseCode;
            this.ResponseMessage = ResponseMessage;
            this.TransactionID = TransactionID;
        }

        /// <summary>
        /// Asigna resultado fallido y asocia datos de mensaje segun las excepcion que haya ocurrido segun el formato definido
        /// </summary>
        /// <param name="t">Resultado fallido</param>
        /// <param name="ex">Excepcion inesperada</param>
        /// <param name="ResponseCode">Codigo de la operacions</param>
        /// <param name="ResponseMessage">Mensaje de error</param>
        /// <param name="TransactionID"></param>
        public void SetErrorResultData(T t, Exception ex, int ResponseCode = 0, string ResponseMessage = "ERROR", int TransactionID = 0)
        {
            ObjectResult = t;
            this.ResponseCode = UtilResut.ResponseCode(ex, ResponseCode);
            this.ResponseMessage =String.Concat("[",ex.GetType().Name.ToUpper(),"]"," ", ResponseMessage);
            this.TransactionID = TransactionID;

        }

        /// <summary>
        /// Converts GenericApiResult to especified Type, and Mapping atributes in IMovilwayApiResponse
        /// </summary>
        /// <typeparam name="T">Generic Type to convert</typeparam>
        /// <returns></returns>
        /// 
        
        //falta inclur handler de resultado
        public T ConvertResponse<T>(T instance = default(T)) where T : Movilway.API.Service.ExtendedApi.DataContract.Common.IMovilwayApiResponse
        {
            T t  = default(T);

            if (instance == null)
            {
                Type type = t.GetType();
                System.Reflection.ConstructorInfo ctor = type.GetConstructor(new Type[] { });
                t = (T)ctor.Invoke(new object[] { });
            }

            Movilway.API.Service.ExtendedApi.DataContract.Common.IMovilwayApiResponse response = t;
            response.ResponseCode = this.ResponseCode;
            response.ResponseMessage = this.ResponseMessage;
            response.TransactionID = this.TransactionID;
            return t;
        }
    }

    public static  class UtilResut
    {
        /// <summary>
        /// 
        /// </summary>
       public const int OK_RESPONSE_CODE = 0;
        /// <summary>
        /// LOS ERRORES DEL 90 AL 99 SON ERRORES DE PERMISOS BASICOS DE EJECUCION
        /// </summary>
       const int SECURE_CONSTRAINS_ERRORS = 90;
        /// <summary>
        /// LOS ERRORES DEL 100 AL 199 SON ERRORES DE NEGOCIO
        /// </summary>
        const int BUSSINES_ERRORS = 100;
        /// <summary>
        /// LOS ERRORES DEL 200 AL 299 SON ERRORES DE BASEDEDATOS
        /// EXCEPCIONES SQLEXCEPTION
        /// </summary>
        const int DATABASE_ERRORS = 200;
        /// <summary>
        /// LOS ERRORES DEL 300 AL 399 SON ERRORES AL CONSUMIR ERRORES
        /// EXCEPCIONES WEBEXCEPTION
        /// </summary>
        const int SERVICE_ERRORS = 300;
        /// <summary>
        /// LOS ERRORES DEL 400 AL 499 SON ERRORES AL CONSUMIR ERRORES
        /// EXCEPCIONES IOEXCEPTIONS O MANEJOS DE STREAMS , ARCHIVOS ETC
        /// </summary>
        const int IO_ERRORS = 400;
        /// <summary>
        /// LOS ERRORES DEL 500 AL 599 SON ERRORES INESPERADOS
        /// PARA BLOQUES CATCH CON EXCEPTION
        /// </summary>
        const int UNEXPECTED_ERRORS = 500;


   

        public static int ConstrainErrorCode(int errorid)
        {
            return CreateErrorCode(SECURE_CONSTRAINS_ERRORS, errorid);
        }
        public static int BussErrorCode(int errorid)
        {
            return CreateErrorCode(BUSSINES_ERRORS, errorid);
        }

        public static int DbErrorCode(int errorid)
        {
            return CreateErrorCode(DATABASE_ERRORS, errorid);
        }

    
        public static int SvcErrorCode(int errorid)
        {
            return CreateErrorCode(SERVICE_ERRORS, errorid);
        }

        public static int IOErrorCode(int errorid)
        {
            return CreateErrorCode(IO_ERRORS, errorid);
        }

        public static int ErrorCode(int errorid)
        {
            return CreateErrorCode(UNEXPECTED_ERRORS, errorid);
        }


        public static string StrConsErrorCode(int errorid)
        {
            return CreateErrorCode(SECURE_CONSTRAINS_ERRORS, errorid).ToString();
        }
        public static string StrBussErrorCode(int errorid)
        {
            return CreateErrorCode(BUSSINES_ERRORS, errorid).ToString();
        }

        public static string StrDbErrorCode(int errorid)
        {
            return CreateErrorCode(DATABASE_ERRORS, errorid).ToString();
        }


        public static string StrSvcErrorCode(int errorid)
        {
            return CreateErrorCode(SERVICE_ERRORS, errorid).ToString();
        }

        public static string StrIOErrorCode(int errorid)
        {
            return CreateErrorCode(IO_ERRORS, errorid).ToString();
        }

        public static string StrErrorCode(int errorid)
        {
            return CreateErrorCode(UNEXPECTED_ERRORS, errorid).ToString();
        }

        private static int CreateErrorCode(int TYPEERROR, int errorid)
        {
            return TYPEERROR + errorid;
        }


        internal static int? ResponseCode(Exception ex, int ResponseCode)
        {
            int result = UtilResut.ErrorCode(ResponseCode);

            string fullname = ex.GetType().FullName;

            if (fullname.Equals("Movilway.API.Core.ApiException", StringComparison.InvariantCultureIgnoreCase) )
            {
                result = UtilResut.BussErrorCode(ResponseCode);
            }
            else if (fullname.IndexOf(".Data.", StringComparison.InvariantCultureIgnoreCase) > 0)
            {
                result = UtilResut.DbErrorCode(ResponseCode);
            }
            else if (fullname.IndexOf(".Net.", StringComparison.InvariantCultureIgnoreCase) > 0)
            {
                result = UtilResut.SvcErrorCode(ResponseCode);
            }
            else if (fullname.IndexOf(".IO.", StringComparison.InvariantCultureIgnoreCase) > 0)
            {
                result = UtilResut.IOErrorCode(ResponseCode);
            }
         

         

            return result;
        }


        internal static int ResponseCode(Exception ex)
        {
            if (ex is Security.SecurityException)
                return SECURE_CONSTRAINS_ERRORS;

            int result =UNEXPECTED_ERRORS;


            //string fullname = ex.GetType().FullName;

            //if (fullname.Equals("Movilway.API.Core.ApiException", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    result = BUSSINES_ERRORS;
            //}
            //else if (fullname.IndexOf(".Data.", StringComparison.InvariantCultureIgnoreCase) > 0)
            //{
            //    result = DATABASE_ERRORS;
            //}
            //else if (fullname.IndexOf(".Net.", StringComparison.InvariantCultureIgnoreCase) > 0)
            //{
            //    result = SERVICE_ERRORS;
            //}
            //else if (fullname.IndexOf(".IO.", StringComparison.InvariantCultureIgnoreCase) > 0)
            //{
            //    result = IO_ERRORS;
            //}
            



            return result;
        }
    }
}