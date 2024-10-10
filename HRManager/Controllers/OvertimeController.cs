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
    public class OvertimeController : Controller
    {
        HRManagerEntities db = new HRManagerEntities();
        LoadData loaddb = new LoadData();
        ActionLog action = new ActionLog();
        int typeLog = 22;

        // GET: Leave
        [HttpGet]
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                List<tblOvertime> overtimes = db.tblOvertime.Where(x => x.isActive == true).ToList();
                List<tblEmployee> employees = (userInfo.isManager) ? db.tblEmployee.Where(x => x.isActive == true).ToList() : db.tblEmployee.Where(x => x.isActive == true && x.ID == userInfo.employeeID).ToList();
                List<tblDepartment> departments = db.tblDepartment.Where(x => x.isActive == true).ToList();
                List<tblPosition> positions = db.tblPosition.Where(x => x.isActive == true).ToList();
                var item = from o in overtimes
                           join e in employees on o.EmployeeID equals e.ID into table1
                           from e in table1.ToList()
                           join d in departments on e.DepartmentID equals d.ID into table2
                           from d in table2.ToList()
                           join p in positions on e.PositionID equals p.ID into table4
                           from p in table4.ToList()
                           orderby o.ID descending
                           select new ViewModel
                           {
                               employee = e,
                               department = d,
                               overtime = o,
                               position = p,
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
                        var item = db.tblOvertime.Where(a => a.ID == id).FirstOrDefault();
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
                    var item = db.tblOvertime.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    item.EmployeeID = (collection["EmployeeID"].ToString() != "") ? int.Parse(collection["EmployeeID"]) : 0;
                    item.Reason = collection["Reason"];
                    if (collection["DateOvertime"].ToString() != "")
                        item.DateOvertime = DateTime.Parse(collection["DateOvertime"]);

                    if (collection["FromTime"].ToString() != "" && collection["FromTime"].ToString().Length > 4)
                    {
                        item.FromTime = TimeSpan.Parse(collection["FromTime"].ToString().Substring(0, 5));
                    }


                    if (collection["ToTime"].ToString() != "" && collection["ToTime"].ToString().Length > 4)
                    {
                        item.ToTime = TimeSpan.Parse(collection["ToTime"].ToString().Substring(0, 5));
                    }



                    item.totalHours = (collection["totalHours"].ToString() != "") ? double.Parse(collection["totalHours"]) : 0;
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
        public ActionResult Create(tblOvertime item)
        {
            if (Session["UserID"] != null)
            {
                try
                {
                    using (HRManagerEntities db = new HRManagerEntities())
                    {
                        // TODO: Add insert logic here
                        item.isActive = true;
                        item.UserCreated = Encryptor.EncryptString(Session["UserID"].ToString());
                        item.DateCreated = DateTime.Now;

                        db.tblOvertime.Add(item);
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
        public void SetViewBagForDetails(tblOvertime item)
        {
            ViewBag.EmployeeList = new SelectList(db.tblEmployee.ToList(), "ID", "EmployeeCode", item.EmployeeID);


            //info Employee
            var employee = db.tblEmployee.Where(x => x.ID == item.EmployeeID).FirstOrDefault();
            ViewBag.Fullname = employee.FirstName + " " + employee.FullName;
            ViewBag.DepartmentName = db.tblDepartment.Where(x => x.ID == employee.DepartmentID).FirstOrDefault().NameDepartmentVN;
            ViewBag.PositionName = db.tblPosition.Where(x => x.ID == employee.PositionID).FirstOrDefault().PositionNameVN;

            //Info time overtime
            ViewBag.Dateovertime = DateTime.Parse(item.DateOvertime.ToString()).ToString("yyyy-MM-dd");
            ViewBag.hourStart = DateTime.Parse(item.FromTime.ToString()).ToString("HH:mm:ss");
            ViewBag.hourEnd = DateTime.Parse(item.ToTime.ToString()).ToString("HH:mm:ss");
        }

        public void getViewBagforCreate()
        {
            ViewBag.EmployeeList = new SelectList(db.tblEmployee.ToList(), "ID", "EmployeeCode");
        }

        #endregion


        [HttpPost]
        public ActionResult DeleteOvertime(int id)
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                // TODO: Add update logic here
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var item = db.tblOvertime.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    item.isActive = false;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["delete"] = "Xóa thành công";
                    return RedirectToAction("Index");
                }
            }
            else return RedirectToAction("Login", "Home");
        }

    }
}