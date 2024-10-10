using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManager.Models
{
    public class EmployeeDetail
    {
        public int ID { get; set; }
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string CommonName { get; set; }
        public bool Gender { get; set; }
        public string Images { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public string PlaceOfBirth { get; set; }
        public Nullable<bool> Married { get; set; }
        public string Address { get; set; }
        public string AddressTmp { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CCCD { get; set; }
        public Nullable<System.DateTime> DateCCCD { get; set; }
        public string IssuedBy { get; set; }
        public Nullable<System.DateTime> DateStartWord { get; set; }
        public string Health { get; set; }
        public Nullable<int> Height { get; set; }
        public Nullable<int> Weight { get; set; }
        public Nullable<int> StatusWork { get; set; }
        public Nullable<int> NationalityID { get; set; }
        public Nullable<int> NationID { get; set; }
        public Nullable<int> ReligionID { get; set; }
        public Nullable<int> DegreeID { get; set; }
        public string ForeignID { get; set; }
        public List<string> ForeignsID { get; set; }
        public bool BHXH { get; set; } = true;
        public bool BHYT { get; set; } = true;
        public bool BHTN { get; set; } = true;
        public bool UnionDues { get; set; } = true;

        public bool Authority { get; set; } = true;

        public string Note { get; set; }
        public string CreatedByUser { get; set; }
        public Nullable<System.DateTime> CreatedByDate { get; set; }
        public Nullable<int> MaritalID { get; set; }
        public string Comment { get; set; }
        public Nullable<int> DepartmentID { get; set; }
        public Nullable<System.DateTime> DateUpdated { get; set; }
        public string UserLastUpdated { get; set; }
        public Nullable<int> PositionID { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public string NoBHXH { get; set; }
        public Nullable<System.DateTime> DateBHXH { get; set; }
        public string IssueByBHXH { get; set; }
        public string NoBHYT { get; set; }
        public Nullable<System.DateTime> fromDateBHYT { get; set; }
        public Nullable<System.DateTime> toDateBHYT { get; set; }
        public string ProvinceBHYT { get; set; }
        public string HopitalBHYT { get; set; }
        public Nullable<bool> isDelete { get; set; }



        public string Nationality { get; set; }
        public string Nation { get; set; }
        public string Religion { get; set; }

        public string Degree { get; set; }
        public string Foreign { get; set; }
       
        public string Marital { get; set; }

       
        public string Department { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }
        public decimal SalaryBasic { get; set; }
    }
}