//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iVolunteer.Models.SQL
{
    using System;
    using System.Collections.Generic;
    
    public partial class SQL_Friendship
    {
        public string UserID { get; set; }
        public string FriendID { get; set; }
        public bool Status { get; set; }
    
        public virtual SQL_Account SQL_Account { get; set; }
        public virtual SQL_Account SQL_Account1 { get; set; }
    }
}
