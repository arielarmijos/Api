//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Movilway.API.Data.Notiway
{
    using System;
    using System.Collections.Generic;
    
    public partial class Province
    {
        public Province()
        {
            this.Cities = new HashSet<City>();
            this.Messages = new HashSet<Message>();
            this.MessageTickets = new HashSet<MessageTicket>();
        }
    
        public int ProvinceId { get; set; }
        public int CountryId { get; set; }
        public string ProvinceName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    
        public virtual ICollection<City> Cities { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<MessageTicket> MessageTickets { get; set; }
    }
}
