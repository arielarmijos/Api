using System.Runtime.Serialization;
using System.Xml.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;

namespace Movilway.API.Service.ExtendedApi.DataContract.SOSIT
{
    public class AddSolicitudeResponse : AGenericApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddSolicitudeResponse" /> class.
        /// </summary>
        public AddSolicitudeResponse()
            : base()
        {
            this.ResponseMessage = string.Empty;
        }

        /// <summary>
        /// Gets or sets Workorderid
        /// </summary>
        [DataMember(Order = 3), XmlElement]
        public string Workorderid { set; get; }

    }
}