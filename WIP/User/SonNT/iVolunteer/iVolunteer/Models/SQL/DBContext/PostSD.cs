//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iVolunteer.Models.SQL.DBContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class PostSD
    {
        public string PostID { get; set; }
        public System.DateTime DateCreate { get; set; }
        public bool Permission { get; set; }
    
        public virtual AlbumSD AlbumSD { get; set; }
        public virtual GroupSD GroupSD { get; set; }
        public virtual ProjectSD ProjectSD { get; set; }
    }
}
