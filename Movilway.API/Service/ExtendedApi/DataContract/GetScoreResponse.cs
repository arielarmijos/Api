using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetScoreResponse : IMovilwayApiResponseWrapper<GetScoreResponseBody>
    {
        [MessageBodyMember(Name = "GetScoreResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetScoreResponseBody Response { set; get; }

        public GetScoreResponse()
        {
            Response = new GetScoreResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetScoreResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetScoreResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3)]
        public Score Score { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class Score
    {
        [Loggable]
        [DataMember(Order = 0, IsRequired = true)]
        public int BranchId { set; get; }

        [Loggable]
        [DataMember(Order = 1, EmitDefaultValue = false)]
        public string BranchName { set; get; }

        [Loggable]
        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string LotteryType { set; get; }

        [Loggable]
        [DataMember(Order = 3, EmitDefaultValue = false)]
        public bool Confirmed { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public int YearToDate { set; get; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public int CurrentMonth { set; get; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = true)]
        public int Standard { set; get; }

        [Loggable]
        [DataMember(Order = 7, IsRequired = true)]
        public int Bonus { set; get; }

        [Loggable]
        [DataMember(Order = 8, IsRequired = true)]
        public int Behaviour { set; get; }

        [Loggable]
        [DataMember(Order = 9, IsRequired = true)]
        public int NetworkStandard { set; get; }

        [Loggable]
        [DataMember(Order = 10, IsRequired = true)]
        public int NetworkBonus { set; get; }

        [Loggable]
        [DataMember(Order = 11, IsRequired = true)]
        public int NetworkBehaviour { set; get; }

        [Loggable]
        [DataMember(Order = 12, IsRequired = true)]
        public int Questionnaire { set; get; }

        [Loggable]
        [DataMember(Order = 13, IsRequired = true)]
        public int Registration { set; get; }
    }
}