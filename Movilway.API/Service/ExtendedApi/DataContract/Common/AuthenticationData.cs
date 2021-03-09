using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract.Common
{
    [Loggable]
    [DataContract(Namespace="http://api.movilway.net/schema/extended")]
    public class AuthenticationData : IEquatable<AuthenticationData>
    {
        [Loggable]
        [DataMember(IsRequired = false, EmitDefaultValue = false, Order=0)]
        public String Username { set; get; }

        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 1)]
        public String Password { set; get; }

        //cookie temp
        [Loggable]
        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 2)]
        public String SessionID { set; get; }


        //cookie monster
        //[Loggable]
        //[DataMember(IsRequired = false, EmitDefaultValue = false, Order = 3)]
        public String Tokken { set; get; }

        public override  bool Equals(Object obj)
        {
            AuthenticationData other = obj as AuthenticationData;
            if (other != null)
                return Equals(other);
            else
                return false;
        }

        public bool Equals(AuthenticationData other)
        {
            if (other == null)
                return false;

            return String.Equals(Username, other.Username) &&
                String.Equals(SessionID,other.SessionID) ;
        }


        public static bool operator ==(AuthenticationData emp1, AuthenticationData emp2)
        {
            if (object.ReferenceEquals(emp1, emp2)) return true;
            if (object.ReferenceEquals(emp1, null)) return false;
            if (object.ReferenceEquals(emp2, null)) return false;

            return emp1.Equals(emp2);
        }

        public static bool operator !=(AuthenticationData emp1, AuthenticationData emp2)
        {
            if (object.ReferenceEquals(emp1, emp2)) return false;
            if (object.ReferenceEquals(emp1, null)) return true;
            if (object.ReferenceEquals(emp2, null)) return true;

            return !emp1.Equals(emp2);
        }

        public override int GetHashCode()
        {

            int hash = 10007;
            int secondPrime = 20011;

            int aux = !string.IsNullOrEmpty(Username) ? Username.GetHashCode() : 0;
            hash = hash * secondPrime + aux;

            aux = !string.IsNullOrEmpty(SessionID)?SessionID.GetHashCode() : 0;
            hash = hash * secondPrime + aux;

            return 0;
        }


   

    }
}