using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.Logging.Attribute;
using Movilway.API.Service.ExtendedApi.DataContract.Common;


namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetAuditoryUsersReportResponse:IMovilwayApiResponseWrapper<GetAuditoryUsersReportResponsetBody>
    {

        [MessageBodyMember(Name = "GetAuditoryUsersReportResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetAuditoryUsersReportResponsetBody Response { set; get; }

    }

    [Loggable]
    [DataContract(Name = "GetAuditoryUsersReportResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetAuditoryUsersReportResponsetBody : ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [DataMember(Order = 3)]
        public UserAuditoryList UserAuditoryList { set; get; }

        public GetAuditoryUsersReportResponsetBody()
        {
            UserAuditoryList = new UserAuditoryList();
        }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }

    }


    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "UserInfo")]
    public class UserAuditoryList : List<UserInfo> { }

    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class UserInfo
    {
        //Login: Usuario con el que se ingresa(“Acceso” en Kinacu)
        [Loggable]
        [DataMember(Order = 1)]
        public string Login { get; set; }
        //Nombre: Nombre del Usuario
        [Loggable]
        [DataMember(Order = 2)]
        public string Nombre { get; set; }
        //Apellido: Apellido del Usuario
        [Loggable]
        [DataMember(Order = 3)]
        public string Apellido { get; set; }
        //Estado: Activo/Inactivo
        [Loggable]
        [DataMember(Order = 4)]
        public string Estado { get; set; }
        //Fecha Creación: Fecha de creación de usuario(año/mes/día)
        [Loggable]
        [DataMember(Order = 5)]
        public DateTime? FechaCreacion { get; set; }
        //FechaBaja: Fecha en la que se deshabilitó o desactivo usuario(año, mes, día)
        [Loggable]
        [DataMember(Order = 6)]
        public DateTime? FechaBaja { get; set; }
        //PasswordExp(año/mes/día) : Fecha de caducidad de password
        [Loggable]
        [DataMember(Order = 7)]
        public DateTime? PasswordExp { get; set; }
        //PosCode(): Nombre del Rol asignado al usuario
        [Loggable]
        [DataMember(Order = 8)]
        public string PosCode  { get; set; }


        //PosDescription: Descripción de Rol
        [Loggable]
        [DataMember(Order = 9)]
        public string PosDescription { get; set; }


    }
}