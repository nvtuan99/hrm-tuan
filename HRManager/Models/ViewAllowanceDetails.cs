using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManager.Models
{
    public class ViewAllowanceDetails
    {
        public int ID { get; set; }
        public string AllowanceNameVN { get; set; }
        public int MoneyAllowance { get; set; }
        public int EmployeeID { get; set; }
    }
}