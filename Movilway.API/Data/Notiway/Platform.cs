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
    
    public partial class Platform
    {
        public Platform()
        {
            this.Branches = new HashSet<Branch>();
            this.Messages = new HashSet<Message>();
            this.MessageTickets = new HashSet<MessageTicket>();
            this.Products = new HashSet<Product>();
            this.Users = new HashSet<User>();
            this.AuditMessages = new HashSet<AuditMessage>();
            this.AuditMessageTickets = new HashSet<AuditMessageTicket>();
        }
    
        public int PlatformId { get; set; }
        public int PlatformTypeId { get; set; }
        public string PlatformName { get; set; }
    
        public virtual ICollection<Branch> Branches { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<MessageTicket> MessageTickets { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<AuditMessage> AuditMessages { get; set; }
        public virtual ICollection<AuditMessageTicket> AuditMessageTickets { get; set; }
    }
}
