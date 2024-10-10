using HRManager.Common;
using HRManager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class ShiftArrangeController : Controller
    {
        HRManagerEntities db = new HRManagerEntities();
        LoadData loaddb = new LoadData();
        ActionLog action = new ActionLog();
        int typeLog = 68;

        // GET: Leave
        [HttpGet]
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                List<tblShiftArrange> shiftArranges = db.tblShiftArrange.Where(x => x.isActive == true).ToList();
                List<tblWorkingShift> workingShifts = db.tblWorkingShift.Where(x => x.isActive == true).ToList();
                List<tblEmployee> employees = db.tblEmployee.Where(x => x.isActive == true).ToList();
                List<tblDepartment> departments = db.tblDepartment.Where(x => x.isActive == true).ToList();
                List<tblPosition> positions = db.tblPosition.Where(x => x.isActive == true).ToList();
                var item = from sa in shiftArranges
                           join ws in workingShifts on sa.WorkingShiftID equals ws.ID into table1
                           from ws in table1.ToList()
                           join e in employees on sa.EmployeeID equals e.ID into table2
                           from e in table2.ToList()
                           join d in departments on e.DepartmentID equals d.ID into table3
                           from d in table3.ToList()
                           join p in positions on e.PositionID equals p.ID into table4
                           from p in table4.ToList()
                           orderby sa.ID descending
                           select new ViewModel
                           {
                               employee = e,
                               department = d,
                               workingShift = ws,
                               shiftArrange = sa,
                               position = p
                           };
                action.InsertActionLog(Session["UserName"].ToString(), typeLog, 6, "");
                return View(item);

            }
            else return RedirectToAction("Login", "Home");
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (ModelState.IsValid && id != null)
            {
                try
                {
                    if (Session["UserID"] != null)
                    {
                        var item = db.tblShiftArrange.Where(a => a.ID == id).FirstOrDefault();
                        SetViewBagForDetails(item);
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 11, "");
                        return View(item);
                    }
                    else return RedirectToAction("Login", "Home");
                }
                catch
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            else return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public ActionResult Details(int id, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserID"] != null)
                {
                    var item = db.tblShiftArrange.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    item.EmployeeID = (collection["EmployeeID"].ToString() != "") ? int.Parse(collection["EmployeeID"]) : 0;
                    item.WorkingShiftID = (collection["WorkingShiftID"].ToString() != "") ? int.Parse(collection["WorkingShiftID"]) : 0;
                    item.Year = (collection["Year"].ToString() != "") ? int.Parse(collection["Year"]) : 0;
                    item.Month = (collection["Month"].ToString() != "") ? int.Parse(collection["Month"]) : 0;

                    if (collection["fromDate"].ToString() != "")
                        item.fromDate = DateTime.Parse(collection["fromDate"].ToString());
                    if (collection["toDate"].ToString() != "")
                        item.toDate = DateTime.Parse(collection["toDate"].ToString());

                    item.DateUpdated = DateTime.Now;
                    item.UserUpdated = Encryptor.EncryptString(Session["UserID"].ToString());
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["update"] = "Cập nhật thành công";
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 8, item.EmployeeID.ToString());

                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            else return RedirectToAction("Login", "Home");
        }



        [HttpGet]
        public ActionResult Create()
        {
            if (Session["UserID"] != null)
            {
                getViewBagforCreate();
                return View();
            }
            else return RedirectToAction("Login", "Home");
        }

        // POST: Position/Create
        [HttpPost]
        public ActionResult Create(tblShiftArrange item, FormCollection collection)
        {
            if (Session["UserID"] != null)
            {
                try
                {
                    using (HRManagerEntities db = new HRManagerEntities())
                    {
                        // TODO: Add insert logic here
                       // item.WorkingShiftID = (collection["WorkingShiftID"].ToString() != "") ? int.Parse(collection["WorkingShiftID"].ToString()) : 1;
                        item.isActive = true;
                        item.UserCreated = Encryptor.EncryptString(Session["UserID"].ToString());
                        item.DateCreated = DateTime.Now;

                        db.tblShiftArrange.Add(item);
                        getViewBagforCreate();
                        db.SaveChanges();
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 7, item.EmployeeID.ToString());
                        TempData["insert"] = "Thêm mới thành công";
                        return RedirectToAction("Index");
                    }
                }
                catch
                {
                    return View();
                }
            }
            else return RedirectToAction("Login", "Home");
        }


        #region Set ViewBag
        public void SetViewBagForDetails(tblShiftArrange item)
        {
            ViewBag.EmployeeList = new SelectList(db.tblEmployee.ToList(), "ID", "EmployeeCode", item.EmployeeID);
            ViewBag.WorkingShift = new SelectList(db.tblWorkingShift.ToList(), "ID", "WorkingShiftVN", item.WorkingShiftID);


            //info Employee
            var employee = db.tblEmployee.Where(x => x.ID == item.EmployeeID).FirstOrDefault();
            ViewBag.Fullname = employee.FirstName + " " + employee.FullName;
            ViewBag.DepartmentName = db.tblDepartment.Where(x => x.ID == employee.DepartmentID).FirstOrDefault().NameDepartmentVN;
            ViewBag.PositionName = db.tblPosition.Where(x => x.ID == employee.PositionID).FirstOrDefault().PositionNameVN;

            //Info time overtime
            ViewBag.fromDate = DateTime.Parse(item.fromDate.ToString()).ToString("yyyy-MM-dd");
            ViewBag.toDate = DateTime.Parse(item.toDate.ToString()).ToString("yyyy-MM-dd");
        }

        public void getViewBagforCreate()
        {
            ViewBag.EmployeeList = new SelectList(db.tblEmployee.ToList(), "ID", "EmployeeCode");
            ViewBag.workingShiftID = new SelectList(db.tblWorkingShift.ToList(), "ID", "WorkingShiftVN");
        }

        #endregion

        public JsonResult CheckShiftArrangeExistence(string EmployeeID , string Year , string Month)
        {
            // Thực hiện kiểm tra tháng tồn tại trong cơ sở dữ liệu ở đây
            Boolean isExist = false;
            if(EmployeeID != null || Year != null || Month != null)
            {
                int year = int.Parse(Year);
                int month = int.Parse(Month);
                int emloyeeID = int.Parse(EmployeeID);
                var item = db.tblShiftArrange.Where(x => x.Year == year && x.Month == month && x.EmployeeID == emloyeeID && x.isActive == true).FirstOrDefault();
                if (item != null)
                {
                    isExist = true;
                }
            }
            return Json(new { exists = isExist }, JsonRequestBehavior.AllowGet);
        }

    }
}