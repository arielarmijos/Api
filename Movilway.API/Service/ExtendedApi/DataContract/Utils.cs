using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    public class Utils
    {
        public static string logFormat(object b)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(b.GetType().Name + "={");

            if (b.GetType().CustomAttributes.Any(a => a.AttributeType.Name.Equals("CollectionDataContractAttribute")))
                sb.Append(b.ToString());
            else
            {
                var properties = b.GetType().GetProperties().Where(p => Attribute.GetCustomAttributes(p, typeof(DataMemberAttribute)).Any()).OrderBy(p => ((DataMemberAttribute)Attribute.GetCustomAttribute(p, typeof(DataMemberAttribute))).Order);
                int i = 0;
                foreach (var prop in properties)
                {
                    if (prop.GetValue(b) != null && prop.GetValue(b).GetType().GetProperties().Any() && prop.GetValue(b).GetType() != typeof(String) && prop.GetValue(b).GetType() != typeof(DateTime))
                        sb.Append(logFormat(prop.GetValue(b)));
                    else
                        sb.Append(prop.Name + "=" + (prop.CustomAttributes.Any(a => a.AttributeType.Name.Equals("LoggableAttribute")) ? (prop.GetValue(b) ?? "").ToString() : "******"));

                    sb.Append(i++ < properties.Count() - 1 ? "," : "");
                }
            }
            sb.Append("}");
            var result = sb.ToString();
            return result;
        }
    }
}