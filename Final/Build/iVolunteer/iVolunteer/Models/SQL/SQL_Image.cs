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
    
    public partial class SQL_Image
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SQL_Image()
        {
            this.SQL_AcIm_Relation = new HashSet<SQL_AcIm_Relation>();
        }
    
        public string ImageID { get; set; }
        public string AlbumID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SQL_AcIm_Relation> SQL_AcIm_Relation { get; set; }
        public virtual SQL_Album SQL_Album { get; set; }
    }
}
