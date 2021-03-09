using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.Logging.Attribute;
using Movilway.API.Service.ExtendedApi.DataContract.Common;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetAgentInfoResponse : IMovilwayApiResponseWrapper<GetAgentInfoResponseBody>
    {
        [MessageBodyMember(Name = "GetAgentInfoResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetAgentInfoResponseBody Response { set; get; }

        public GetAgentInfoResponse()
        {
            Response = new GetAgentInfoResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetAgentInfoResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetAgentInfoResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3)]
        public AgentInfo AgentInfo { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class AgentInfo
    {
        [Loggable]
        [DataMember(Order = 0, EmitDefaultValue=false)]
        public String Agent { set; get; }

        public long AgentID { set; get; }

        public long ReferenceID { set; get; }

        [Loggable]
        [DataMember(Order = 3)]
        public long OwnerID { set; get; }

        [Loggable]
        [DataMember(Order = 4, EmitDefaultValue = false)]
        public long BranchID { set; get; }

        [Loggable]
        [DataMember(Order = 5, EmitDefaultValue = false)]
        public String NationalIDType{ set; get; }

        [Loggable]
        [DataMember(Order = 6, EmitDefaultValue = false)]
        public String NationalID { set; get; }

        [Loggable]
        [DataMember(Order = 7, EmitDefaultValue = false)]
        public String Name { set; get; }

        [Loggable]
        [DataMember(Order = 8, EmitDefaultValue = false)]
        public String LegalName { set; get; }

        [Loggable]
        [DataMember(Order = 9, EmitDefaultValue = false)]
        public String Address { set; get; }

        [Loggable]
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public String Email { set; get; }

        [Loggable]
        [DataMember(Order = 11, EmitDefaultValue = false)]
        public int? Depth { set; get; }

        [Loggable]
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public String PhoneNumber { set; get; }

        [Loggable]
        [DataMember(Order = 13, EmitDefaultValue = false)]
        public String BirthDate { set; get; }

        [Loggable]
        [DataMember(Order = 14, EmitDefaultValue = false)]
        public String Gender { set; get; }

        [Loggable]
        [DataMember(Order = 15, EmitDefaultValue = false)]
        public int SubLevel { set; get; }

        [Loggable]
        [DataMember(Order = 16, EmitDefaultValue = false)]
        public CommissionGroupList CommissionGroups { set; get; }

        [Loggable]
        [DataMember(Order = 17, EmitDefaultValue = false)]
        public string PDVID { set; get; }

        [Loggable]
        [DataMember(Order = 18, EmitDefaultValue = false)]
        public int TaxCategory { set; get; }

        [Loggable]
        [DataMember(Order = 19, EmitDefaultValue = false)]
        public TaxCategoryList TaxCategories { set; get; }

        [Loggable]
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public int SegmentId { get; set; }

        [Loggable]
        [DataMember(Order = 21, EmitDefaultValue = false)]
        public SegmentList SegmentList { set; get; }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended",
        KeyName = "ID", ValueName = "GroupName", ItemName = "CommissionGroup")]
    public class CommissionGroupList : Dictionary<int, string> { }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", KeyName = "ID", ValueName = "CategoryName", ItemName = "TaxCategory")]
    public class TaxCategoryList : Dictionary<int, string> { }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", KeyName = "ID", ValueName = "Name", ItemName = "Segment")]
    public class SegmentList : Dictionary<int, string> { }
}
