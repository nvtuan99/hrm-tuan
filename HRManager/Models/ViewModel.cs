using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManager.Models
{
    public class ViewModel
    {
        public tblEmployee employee { get; set; }
        public tblDepartment department { get; set; }
        public tblPosition position { get; set; }
        public tblContract Contract { get; set; }
        public tblTypeContract typeContract { get; set; }
        public tblReward reward { get; set; }
        public tblTypeReward typeReward { get; set; }
        public tblSysLog sysLog { get; set; }
        public tblSysAction sysAction { get; set; }
        public tblSysFunction sysFunction { get; set; }

        public tblDiscipline discipline { get; set; }
        public tblDisciplineType disciplineType { get; set; }

        public UserProfile userProfile { get; set; }
        public tblRole role { get; set; }
        public tblRoleCategory roleCategory { get; set; }
        public tblNation nation { get; set; }
        public tblNationality nationality { get; set; }
        public tblReligion religion { get; set; }
        public tblDegree degree { get; set; }
        public tblForeignLanguage foreign { get; set; }
        public tblMaritalStatus marital { get; set; }
        public tblCompany company { get; set; }
        public tblLeave leave { get; set; }
        public tblTypeLeave typeleave { get; set; }
        public tblOvertime overtime { get; set; }
        public tblEmployeeCard employeecard { get; set; }
        public tblTypeEmployeeCard typeemployeecard { get; set; }
        public tblHistoryTimekeeping historyTimekeeping { get; set; }
        public tblTimeKeeper timeKeeper { get; set; }

        public tblTypeInputTimekeeping typeInputTimekeeping { get; set; }

        public tblShiftArrange shiftArrange { get; set; }
        public tblWorkingShift workingShift { get; set; }
        public tblTimesheetDetail timesheetDetail { get; set; }
        public tblAllowance allowance { get; set; }
        public tblAllowanceCategory allowanceCategory { get; set; }
        public tblAnnualLeave annualLeave { get; set; }
        public tblSalarySheetDetail salarySheetDetail { get; set; }
        public tblSalarySheet salarySheet { get; set; }

    }
}