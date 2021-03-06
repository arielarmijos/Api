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
    
    public partial class City
    {
        public City()
        {
            this.Branches = new HashSet<Branch>();
            this.Messages = new HashSet<Message>();
            this.MessageTickets = new HashSet<MessageTicket>();
            this.Users = new HashSet<User>();
        }
    
        public int CityId { get; set; }
        public int ProvinceId { get; set; }
        public int CountryId { get; set; }
        public string CityName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    
        public virtual ICollection<Branch> Branches { get; set; }
        public virtual Province Province { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<MessageTicket> MessageTickets { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
