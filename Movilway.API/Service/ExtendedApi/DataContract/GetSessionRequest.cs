using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetSessionRequest:IMovilwayApiRequestWrapper<GetSessionRequestBody>
    {
        [MessageBodyMember(Name = "GetSessionRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public GetSessionRequestBody Request { set; get; }

        public AuthenticationData GetAuthenticationData()
        {
            return (new AuthenticationData()
            {
                Username = Request.Username,
                Password = Request.Password
            });
        }

        public GetSessionRequest()
        {
            Request = new GetSessionRequestBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetSessionRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetSessionRequestBody:IMovilwayApiRequest
    {
        public Boolean IsFinancial { get { return false; } }

        [Loggable]
        [DataMember(Order=0, IsRequired=true)]
        public String Username { set; get; }

        [DataMember(Order = 1, IsRequired = true)]
        public String Password { set; get; }

        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public int DeviceType { set; get; }

        //public String SessionId { set; get; }

        //public String Tokken { set; get; }

        [Loggable]
        [DataMember(Order = 3, IsRequired = false)]
        public string Platform { set; get; }

        public GetSessionRequestBody()
        {
            //SessionId = string.Empty;
            //Tokken = string.Empty;
        }


        public AuthenticationData AuthenticationData
        {
            get
            {
                return new AuthenticationData() 
                { 
                    Username = Username, 
                    Password = Password,
                    //SessionID = SessionId,
                    //Tokken = Tokken
                };
            }
        }

        public override string ToString()
        {
            return Utils.logFormat(this);
            //return "GetSessionRequest {UserName=" + this.AuthenticationData.Username + ",Password=******,DeviceType=" + this.DeviceType + ",Platform=" + this.Platform + "}";
        }
    }
        
}