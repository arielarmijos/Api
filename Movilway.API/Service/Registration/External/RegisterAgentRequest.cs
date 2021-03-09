using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.External;

namespace Movilway.API.Service.Registration.External
{

    [MessageContract(IsWrapped = false)]
    public class RegisterAgentRequest
    {
        [MessageBodyMember(Name = "RegisterAgentRequest", Namespace = "http://api.movilway.net/schema")]
        public RegisterAgentRequestBody Request { set; get; }

        public RegisterAgentRequest()
        {
            Request = new RegisterAgentRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class RegisterAgentRequestBody : SessionApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public String Agent { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public String Name { set; get; }

        [DataMember(Order = 3, IsRequired = true)]
        public String LegalName { set; get; }

        [DataMember(Order = 4, IsRequired = true)]
        public String Address { set; get; }

        [DataMember(Order = 5, IsRequired = true)]
        public String PhoneNumber { set; get; }

        [DataMember(Order = 6, IsRequired = true)]
        public int ProvinceID { set; get; }

        [DataMember(Order = 7, IsRequired = true)]
        public int CityID { set; get; }

        [DataMember(Order = 8)]
        public String ContactName { set; get; }

        [DataMember(Order = 9)]
        public DateTime BirthDate { set; get; }

        [DataMember(Order = 10)]
        public Gender Gender { set; get; }

        [DataMember(Order = 11, IsRequired = true)]
        public String NationalIDType { set; get; }

        [DataMember(Order = 12, IsRequired = true)]
        public String NationalID { set; get; }

        [DataMember(Order = 13, IsRequired = true)]
        public String SMSAddress { set; get; }

        [DataMember(Order = 14, IsRequired = true)]
        public String Email { set; get; }

        [DataMember(Order = 15)]
        public String Referrer { set; get; }

        [DataMember(Order = 16)]
        public String MNO1 { set; get; }

        [DataMember(Order = 17)]
        public String MNO2 { set; get; }

        [DataMember(Order = 18)]
        public String MNO3 { set; get; }

        [DataMember(Order = 19)]
        public String MNO4 { set; get; }

        [DataMember(Order = 20)]
        public String MNO5 { set; get; }
    }
}