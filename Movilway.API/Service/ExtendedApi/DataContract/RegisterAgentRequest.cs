using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.External;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{

    [MessageContract(IsWrapped = false)]
    public class RegisterAgentRequest:IMovilwayApiRequestWrapper<RegisterAgentRequestBody>
    {
        [MessageBodyMember(Name = "RegisterAgentRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public RegisterAgentRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "RegisterAgentRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class RegisterAgentRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 1, IsRequired = true)]
        public AgentDetails Agent { set; get; }
    }

    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class AgentDetails
    {
        [Loggable]
        [DataMember(Order = 1, IsRequired = true)]
        public String Agent { set; get; }

        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public String Name { set; get; }

        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String LegalName { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public String Address { set; get; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public String PhoneNumber { set; get; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = true)]
        public int ProvinceID { set; get; }

        [Loggable]
        [DataMember(Order = 7, IsRequired = true)]
        public int CityID { set; get; }

        [Loggable]
        [DataMember(Order = 8)]
        public String ContactName { set; get; }

        [Loggable]
        [DataMember(Order = 9)]
        public DateTime BirthDate { set; get; }

        [Loggable]
        [DataMember(Order = 10)]
        public Gender Gender { set; get; }

        [Loggable]
        [DataMember(Order = 11, IsRequired = true)]
        public String NationalIDType { set; get; }

        [Loggable]
        [DataMember(Order = 12, IsRequired = true)]
        public String NationalID { set; get; }

        [Loggable]
        [DataMember(Order = 13, IsRequired = true)]
        public String SMSAddress { set; get; }

        [Loggable]
        [DataMember(Order = 14, IsRequired = true)]
        public String Email { set; get; }
        
        [Loggable]
        [DataMember(Order = 15)]
        public String Referrer { set; get; }

        [Loggable]
        [DataMember(Order = 16)]
        public String MNO1 { set; get; }

        [Loggable]
        [DataMember(Order = 17)]
        public String MNO2 { set; get; }

        [Loggable]
        [DataMember(Order = 18)]
        public String MNO3 { set; get; }

        [Loggable]
        [DataMember(Order = 19)]
        public String MNO4 { set; get; }

        [Loggable]
        [DataMember(Order = 20)]
        public String MNO5 { set; get; }
    }
}