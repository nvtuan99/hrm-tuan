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
    public class EmployeeCardController : Controller
    {
        HRManagerEntities db = new HRManagerEntities();
        LoadData loaddb = new LoadData();
        ActionLog action = new ActionLog();
        int typeLog = 64;

        // GET: Leave
        [HttpGet]
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                List<tblEmployeeCard> employeecards = db.tblEmployeeCard.Where(x => x.isActive == true).ToList();
                List<tblTypeEmployeeCard> typeemployeecards = db.tblTypeEmployeeCard.Where(x => x.isActive == true).ToList();
                List<tblEmployee> employees = db.tblEmployee.Where(x => x.isActive == true).ToList();
                List<tblDepartment> departments = db.tblDepartment.Where(x => x.isActive == true).ToList();
                List<tblPosition> positions = db.tblPosition.Where(x => x.isActive == true).ToList();

                var item = from o in employeecards
                           join e in employees on o.EmployeeID equals e.ID into table1
                           from e in table1.ToList()
                           join d in departments on e.DepartmentID equals d.ID into table2
                           from d in table2.ToList()
                           join te in typeemployeecards on o.TypeEmployeeCardID equals te.ID into table3
                           from te in table3.ToList()
                           join p in positions on e.PositionID equals p.ID into table4
                           from p in table4.ToList()
                           orderby o.ID descending
                           select new ViewModel
                           {
                               employee = e,
                               department = d,
                               employeecard = o,
                               typeemployeecard = te,
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
                        var item = db.tblEmployeeCard.Where(a => a.ID == id).FirstOrDefault();
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
                    var item = db.tblEmployeeCard.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    item.EmployeeID = (collection["EmployeeID"].ToString() != "") ? int.Parse(collection["EmployeeID"]) : 0;
                    item.EmployeeCardID = collection["EmployeeCardID"];
                    item.TypeEmployeeCardID = (collection["TypeEmployeeCardID"].ToString() != "") ? int.Parse(collection["TypeEmployeeCardID"]) : 0;
                    item.Note = collection["Note"];

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
        public ActionResult Create(tblEmployeeCard item)
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

                        db.tblEmployeeCard.Add(item);
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
        public void SetViewBagForDetails(tblEmployeeCard item)
        {
            ViewBag.EmployeeList = new SelectList(db.tblEmployee.ToList(), "ID", "EmployeeCode", item.EmployeeID);
            ViewBag.typeEmployeeCard = new SelectList(db.tblTypeEmployeeCard.ToList(), "ID", "TypeCardVN", item.EmployeeID);


            //info Employee
            var employee = db.tblEmployee.Where(x => x.ID == item.EmployeeID).FirstOrDefault();
            ViewBag.Fullname = employee.FirstName + " " + employee.FullName;
            ViewBag.DepartmentName = db.tblDepartment.Where(x => x.ID == employee.DepartmentID).FirstOrDefault().NameDepartmentVN;
            ViewBag.PositionName = db.tblPosition.Where(x => x.ID == employee.PositionID).FirstOrDefault().PositionNameVN;

        }

        public void getViewBagforCreate()
        {
            ViewBag.EmployeeList = new SelectList(db.tblEmployee.ToList(), "ID", "EmployeeCode");
            ViewBag.typeEmployeeCard = new SelectList(db.tblTypeEmployeeCard.ToList(), "ID", "TypeCardVN");

        }

        #endregion

    }
}