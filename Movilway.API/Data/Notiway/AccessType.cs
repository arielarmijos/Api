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
    
    public partial class AccessType
    {
        public AccessType()
        {
            this.Messages = new HashSet<Message>();
            this.MessageTickets = new HashSet<MessageTicket>();
            this.Accesses = new HashSet<Access>();
        }
    
        public int AccessTypeId { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<MessageTicket> MessageTickets { get; set; }
        public virtual ICollection<Access> Accesses { get; set; }
    }
}
