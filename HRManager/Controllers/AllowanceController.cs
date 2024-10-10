using HRManager.Common;
using HRManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class AllowanceController : Controller
    {
        HRManagerEntities db = new HRManagerEntities();
        LoadData loaddb = new LoadData();
        ActionLog action = new ActionLog();

        int typeLog = 22;
        // GET: Allowance
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                setViewBag();

                List<tblEmployee> employees = db.tblEmployee.Where(x => x.isActive == true).ToList();
                List<tblDepartment> departments = db.tblDepartment.Where(x => x.isActive == true).ToList();
                List<tblPosition> positions = db.tblPosition.Where(x => x.isActive == true).ToList();

                var item = from e in employees
                           join d in departments on e.DepartmentID equals d.ID into table1
                           from d in table1.ToList()
                           join p in positions on e.PositionID equals p.ID into table2
                           from p in table2.ToList()
                          
                           orderby e.ID descending
                           select new ViewModel
                           {
                               employee = e,
                               department = d,
                               position = p,
                           };
                action.InsertActionLog(Session["UserName"].ToString(), typeLog, 6, "");
                return View(item);

            }
            else return RedirectToAction("Login", "Home");
        }

        public ActionResult GetAllowanceDetails(int id)
        {
            string sql = "select COALESCE(CAST(a.ID AS INT), 0) as ID, ac.AllowanceNameVN as AllowanceNameVN , " +
                         " COALESCE(CAST(ac.Money AS INT), 0) AS MoneyAllowance, " +
                         " COALESCE(CAST(a.EmployeeID AS INT), 0) AS EmployeeID " +
                         "from tblAllowance a " +
                         "left join tblAllowanceCategory ac on ac.ID = a.AllowanceCategoryID " +
                         "where a.EmployeeID = " + id;
            var item = db.Database.SqlQuery<ViewAllowanceDetails>(sql).ToList();
            //var allowanceDetail = db.tblAllowance.Where(x => x.EmployeeID == id).ToList();
            return Json(item, JsonRequestBehavior.AllowGet);
        }


        public void setViewBag()
        {
            ViewBag.AllowanceCategory = new SelectList(db.tblAllowanceCategory, "ID", "AllowanceNameVN");
        }
        public JsonResult DeleteAllowance(List<tblAllowance> allowances)
        {
            using (HRManagerEntities entities = new HRManagerEntities())
            {
                //Check for NULL.
                if (allowances == null)
                {
                    allowances = new List<tblAllowance>();
                }

                //Loop and insert records.
                foreach (tblAllowance all in allowances)
                {
                    tblAllowance tblAllowance = entities.tblAllowance.Find(all.ID);
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 8, tblAllowance.EmployeeID.ToString());
                    entities.tblAllowance.Remove(tblAllowance);
                }
                int insertedRecords = entities.SaveChanges();
                

                return Json(insertedRecords);
            }
        }


        public JsonResult InsertAllowance(List<tblAllowance> allowances)
        {
            using (HRManagerEntities entities = new HRManagerEntities())
            {
                //Check for NULL.
                if (allowances == null)
                {
                    allowances = new List<tblAllowance>();
                }

                //Loop and insert records.
                foreach (tblAllowance item in allowances)
                {
                    item.isActive = true;
                    item.DateCreated = DateTime.Now;
                    item.UserCreated = Encryptor.EncryptString(Session["UserID"].ToString());
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 7, "");
                    entities.tblAllowance.Add(item);
                }
                int insertedRecords = entities.SaveChanges();
                return Json(insertedRecords);
            }
        }

    }
}