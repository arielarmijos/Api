using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.DataContract.Common
{
    public static class EnumExtensions
    {
        public static string ToDescription(this Enum enumeration)
        {
            Type type = enumeration.GetType();
            MemberInfo[] memInfo = type.GetMember(enumeration.ToString());

            if (null != memInfo && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(ErrorMessagesDescription), false);
                if (null != attrs && attrs.Length > 0)
                {
                    return ((ErrorMessagesDescription)attrs[0]).Text;
                }
            }

            return enumeration.ToString();
        }
    }
}