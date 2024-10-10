using HRManager.Common;
using HRManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class AnnualLeaveController : Controller
    {
        private HRManagerEntities db = new HRManagerEntities();
        int typeLog = 70;
        ActionLog action = new ActionLog();

        // GET: AnnualLeave
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    List<tblEmployee> employees = db.tblEmployee.Where(x => x.isActive == true).ToList();
                    List<tblDepartment> departments = db.tblDepartment.Where(x => x.isActive == true).ToList();
                    List<tblPosition> positions = db.tblPosition.Where(x => x.isActive == true).ToList();
                    List<tblAnnualLeave> annualLeaves = db.tblAnnualLeave.ToList();
                   

                    var item = from e in employees
                               join d in departments on e.DepartmentID equals d.ID into table1
                               from d in table1.ToList()
                               join p in positions on e.PositionID equals p.ID into table2
                               from p in table2.ToList()
                               join a in annualLeaves on e.ID equals a.EmployeeID into table3
                               from a in table3.ToList()
                              

                               orderby e.ID descending
                               select new ViewModel
                               {
                                   employee = e,
                                   department = d,
                                   position = p,
                                   annualLeave = a,
                               };
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 6, "");
                    return View(item);
                }
            }
            else return RedirectToAction("Login", "Home");
        }

        public ActionResult GetAnnualLeaveDetails(int id)
        {
            string sql = "select COALESCE(CAST(QtyAnnualLeave AS decimal(9,0)), 0) as QtyAnnualLeave, " +
                         "CONVERT(varchar, DateCreated, 101) as DateCreated, " +
                         "Detail ,TypeAction " +
                         "from tblAnnualLeaveDetail " +
                         "where EmployeeID = " + id;
            var item = db.Database.SqlQuery<ViewAnnualLeaveDetails>(sql).ToList();
            //var allowanceDetail = db.tblAllowance.Where(x => x.EmployeeID == id).ToList();
            return Json(item, JsonRequestBehavior.AllowGet);
        }
    }
}