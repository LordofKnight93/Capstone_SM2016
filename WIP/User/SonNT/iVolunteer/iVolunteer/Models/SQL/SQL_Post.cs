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
    
    public partial class SQL_Post
    {
        public string PostID { get; set; }
        public System.DateTime DateCreate { get; set; }
        public System.DateTime DateLastActivity { get; set; }
        public string ProjectID { get; set; }
        public string GroupID { get; set; }
        public string AlbumID { get; set; }
        public bool Permission { get; set; }
    
        public virtual SQL_Album SQL_Album { get; set; }
        public virtual SQL_Group SQL_Group { get; set; }
        public virtual SQL_Project SQL_Project { get; set; }
    }
}
