using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetUsersAgentRequest : IMovilwayApiRequestWrapper<GetUsersAgentRequestBody>
    {
        [MessageBodyMember(Name = "GetUsersAgentRequest")]
        public GetUsersAgentRequestBody Request { set; get; }

    }

    [Loggable]
    [DataContract(Name = "GetUsersAgentRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetUsersAgentRequestBody :ASecuredApiRequest// ASecuredFinancialApiRequest, IEquatable<GetUsersAgentRequestBody>
    {
        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public int Agent { set; get; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = true)]
        public bool Onlychildren { get; set; }

        [Loggable]
        [DataMember(Order = 7, IsRequired = true)]
        public bool ShowAccess { get; set; }


        public override string ToString()
        {
            return Utils.logFormat(this);

        }


        public override bool Equals(object obj)
        {
            TopUpRequestBody objval = obj as TopUpRequestBody;

            if (obj != null)
                return Equals(objval);
            else
                return false;
        }
       
    }
}