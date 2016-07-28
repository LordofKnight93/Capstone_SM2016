using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iVolunteer.Models.ViewModel
{
    public class ReportModel
    {
        [Required(ErrorMessage = "Vui lòng chọn lý do!")]
        [DisplayName("Lý do báo cáo")]
        public string Reason { get; set; }
        public string Detail { get; set; }

    }
}