//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iVolunteer.Models.Data_Definition_Class.SQL
{
    using System;
    using System.Collections.Generic;
    
    public partial class SQL_User_Group
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string GroupID { get; set; }
        public int RelationType { get; set; }
    
        public virtual SQL_Account SQL_Account { get; set; }
        public virtual SQL_Group SQL_Group { get; set; }
    }
}
