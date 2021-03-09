using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.Internal;

namespace Movilway.API.Service.Registration.Internal
{
    public class RegisterAgentRequestInternal:ApiRequestInternal
    {
        public String Agent { set; get; }
        public String Name { set; get; }
        public String LegalName { set; get; }
        public String Address { set; get; }
        public String PhoneNumber { set; get; }
        public int CountryID { set; get; }
        public int ProvinceID { set; get; }
        public int CityID { set; get; }
        public String ContactName { set; get; }
        public DateTime BirthDate { set; get; }
        public Gender Gender { set; get; }
        public String NationalIDType { set; get; }
        public String NationalID { set; get; }
        public String SMSAddress { set; get; }
        public String Email { set; get; }
        public String Referrer { set; get; }
        public String MNO1 { set; get; }
        public String MNO2 { set; get; }
        public String MNO3 { set; get; }
        public String MNO4 { set; get; }
        public String MNO5 { set; get; }
    }
}