//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HRManager
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblEducation
    {
        public int ID { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public string School { get; set; }
        public string DegreeID { get; set; }
        public string Majors { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public Nullable<bool> isActive { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DateUpdated { get; set; }
        public string UserUpdated { get; set; }
    }
}
