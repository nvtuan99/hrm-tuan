using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManager.Models
{
    public class ViewAnnualLeaveDetails
    {
        public string DateCreated { get; set; }
        public decimal QtyAnnualLeave { get; set; }
        public string TypeAction { get; set; }
        public string Detail { get; set; }
    }
}