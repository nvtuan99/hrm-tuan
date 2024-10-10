using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManager.Models
{
    public class ViewRoleDetails
    {
        public int id { get; set; }
        public int employeeID { get; set; }
        public int idCategory { get; set; }
        public string nameCategoryNameVN { get; set; }
        public int roleView { get; set; }
        public int roleUpdate { get; set; }
        public int roleDelete { get; set; }
    }
}