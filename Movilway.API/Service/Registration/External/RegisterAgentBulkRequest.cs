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
    public class RegisterAgentBulkRequest
    {
        [MessageBodyMember(Name = "RegisterAgentBulkRequest", Namespace = "http://api.movilway.net/schema")]
        public RegisterAgentBulkRequestBody Request { set; get; }

        public RegisterAgentBulkRequest()
        {
            Request = new RegisterAgentBulkRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class RegisterAgentBulkRequestBody : SessionApiRequest
    {
        [DataMember(IsRequired=true)]
        public RegisterAgentBulkCollection Agents { set; get; }
    }

    [CollectionDataContract(Namespace="http://api.movilway.net/schema")]
    public class RegisterAgentBulkCollection:List<RegisterAgentBulkItem>{}

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class RegisterAgentBulkItem
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

        [DataMember(Order = 8, IsRequired = true)]
        public String ContactName { set; get; }

        [DataMember(Order = 9, IsRequired = true)]
        public DateTime BirthDate { set; get; }

        [DataMember(Order = 10, IsRequired = true)]
        public Gender Gender { set; get; }

        [DataMember(Order = 11, IsRequired = true)]
        public String NationalIDType { set; get; }

        [DataMember(Order = 12, IsRequired = true)]
        public String NationalID { set; get; }

        [DataMember(Order = 13, IsRequired = true)]
        public String SMSAddress { set; get; }

        [DataMember(Order = 14, IsRequired = true)]
        public String Email { set; get; }

        [DataMember(Order = 15, IsRequired = true)]
        public String Referrer { set; get; }

        [DataMember(Order = 16, IsRequired = true)]
        public String MNO1 { set; get; }

        [DataMember(Order = 17, IsRequired = false)]
        public String MNO2 { set; get; }

        [DataMember(Order = 18, IsRequired = false)]
        public String MNO3 { set; get; }

        [DataMember(Order = 19, IsRequired = false)]
        public String MNO4 { set; get; }

        [DataMember(Order = 20, IsRequired = false)]
        public String MNO5 { set; get; }
    }
}