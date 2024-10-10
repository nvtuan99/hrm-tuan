using HRManager.Common;
using HRManager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class DisciplineController : Controller
    {
        // GET: Reward
        ActionLog action = new ActionLog();
        HRManagerEntities db = new HRManagerEntities();
        LoadData loaddb = new LoadData();
        int typeLog = 17;

        // GET: Discipline
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                List<tblEmployee> employees = db.tblEmployee.Where(x => x.isActive == true).ToList();
                List<tblDepartment> departments = db.tblDepartment.ToList();
                List<tblDiscipline> disciplines = db.tblDiscipline.ToList().Where(x => x.isActive == true).ToList();
                List<tblDisciplineType> disciplinesTypes = db.tblDisciplineType.ToList();
                List<tblPosition> positions = db.tblPosition.Where(x => x.isActive == true).ToList();
                var employeeRecord = from e in employees
                                     join d in departments on e.DepartmentID equals d.ID into table1
                                     from d in table1.ToList()
                                     join i in disciplines on e.ID equals i.EmployeeID into table2
                                     from i in table2.ToList()

                                     join t in disciplinesTypes on i.TypeDiscipline equals t.ID into table3
                                     from t in table3.ToList()
                                     join p in positions on e.PositionID equals p.ID into table4
                                     from p in table4.ToList()
                                     orderby e.ID descending
                                     select new ViewModel
                                     {
                                         employee = e,
                                         department = d,
                                         discipline = i,
                                         disciplineType = t,
                                         position = p,
                                     };
                action.InsertActionLog(Session["UserName"].ToString(), typeLog, 6, "");
                return View(employeeRecord);
            }
            else return RedirectToAction("Login", "Home");
        }

        // GET: Discipline/Details/5
        public ActionResult Details(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["UserID"] != null)
                    {
                        List<tblEmployee> employees = db.tblEmployee.Where(x => x.isActive == true).ToList();
                        List<tblDepartment> departments = db.tblDepartment.ToList();
                        List<tblDiscipline> disciplines = db.tblDiscipline.ToList().Where(x => x.isActive == true).ToList();
                        List<tblDisciplineType> disciplinesTypes = db.tblDisciplineType.ToList();

                        var rewardDetail = from e in employees
                                           join d in departments on e.DepartmentID equals d.ID into table1
                                           from d in table1.ToList()

                                           join i in disciplines on e.ID equals i.EmployeeID into table2
                                           from i in table2.ToList().Where(x => x.ID == id)

                                           join t in disciplinesTypes on i.TypeDiscipline equals t.ID into table3
                                           from t in table3.ToList()

                                           orderby e.ID descending
                                           select new ViewModel
                                           {
                                               employee = e,
                                               department = d,
                                               discipline = i,
                                               disciplineType = t,
                                           };

                        var rew = db.tblDiscipline.Where(a => a.ID.Equals(id)).FirstOrDefault();
                        getViewBagDetail(rew);

                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 6, rew.NoDiscipline);

                        return View(rewardDetail);
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
        public ActionResult Details(int id, FormCollection collection, HttpPostedFileBase FileDiscipline)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserID"] != null)
                {
                    var items = db.tblDiscipline.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    items.NoDiscipline = collection["NoDiscipline"];
                    items.EmployeeID = (collection["item.discipline.EmployeeID"].ToString() != "") ? int.Parse(collection["item.discipline.EmployeeID"]) : 0;

                    var emp = db.tblEmployee.Where(a => a.ID == items.EmployeeID).FirstOrDefault();
                    items.EmployeeCode = emp.EmployeeCode;

                    if (collection["item.discipline.DateDiscipline"].ToString() != "" || collection["item.discipline.DateDiscipline"] != null)
                        items.DateDiscipline = DateTime.Parse(collection["item.discipline.DateDiscipline"]);
                    items.TypeDiscipline = (collection["item.discipline.TypeDiscipline"].ToString() != "") ? int.Parse(collection["item.discipline.TypeDiscipline"]) : 0;
                    items.Reason = collection["Reason"];
                   

                    if (collection["item.discipline.ApproveDate"].ToString() != "")
                        items.ApproveDate = DateTime.Parse(collection["item.discipline.ApproveDate"]);
                    items.ApproveBy = collection["item.discipline.ApproveBy"];

                    if (FileDiscipline != null && FileDiscipline.ContentLength > 0)
                    {
                        string filename = Path.GetFileName(FileDiscipline.FileName);
                        string path = Path.Combine(Server.MapPath("~/Uploads/Doc/DocumentUpload/"), filename);
                        FileDiscipline.SaveAs(path);
                        items.FileDiscipline = filename;
                    }
                    items.UpdatedDate = DateTime.Now;
                    items.UpdatedBy = Encryptor.EncryptString(Session["UserID"].ToString());
                    db.Entry(items).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["update"] = "Cập nhật thành công";
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 8, items.NoDiscipline);

                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            else return RedirectToAction("Login", "Home");
        }


        // GET: Discipline/Create
        public ActionResult Create()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                getViewBagforCreate();
                return View();
            }
            else return RedirectToAction("Login", "Home");
        }

        // POST: Discipline/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection, HttpPostedFileBase FileDiscipline)
        {
            try
            {
                // TODO: Add insert logic here
                getViewBagforCreate();

                tblDiscipline item = new tblDiscipline();
                item.NoDiscipline = collection["NoDiscipline"];
                item.EmployeeID = (collection["EmployeeCode"].ToString() != "") ? int.Parse(collection["EmployeeCode"]) : 0;

                var emp = db.tblEmployee.Where(a => a.ID == item.EmployeeID).FirstOrDefault();
                item.EmployeeCode = emp.EmployeeCode;

                if (collection["DateDiscipline"].ToString() != "" || collection["DateDiscipline"] != null)
                    item.DateDiscipline = DateTime.Parse(collection["DateDiscipline"]);

                item.TypeDiscipline = (collection["TypeDiscipline"].ToString() != "") ? int.Parse(collection["TypeDiscipline"]) : 0;
                item.Reason = collection["Reason"];
                if (collection["ApproveDate"] == null || collection["ApproveDate"].ToString() == "")
                    item.ApproveDate = DateTime.Parse(collection["ApproveDate"]);
                item.ApproveBy = collection["ApproveBy"];

                if (FileDiscipline != null && FileDiscipline.ContentLength > 0)
                {
                    string filename = Path.GetFileName(FileDiscipline.FileName);
                    string path = Path.Combine(Server.MapPath("~/Uploads/Doc/DocumentUpload/"), filename);
                    FileDiscipline.SaveAs(path);
                    item.FileDiscipline = filename;
                }
                item.CreateDate = DateTime.Now;
                item.CreateBy = Encryptor.EncryptString(Session["UserID"].ToString());
                item.isActive = true;


                db.tblDiscipline.Add(item);

                db.SaveChanges();
                TempData["insert"] = "Thêm mới thành công";
                action.InsertActionLog(Session["UserName"].ToString(), typeLog, 7, item.NoDiscipline);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Discipline/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Discipline/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Discipline/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Discipline/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public void getViewBagDetail(tblDiscipline item)
        {
            ViewBag.DisciplineType = new SelectList(db.tblDisciplineType.ToList(), "ID", "NameVN", item.TypeDiscipline);
            ViewBag.EmployeeList = new SelectList(db.tblEmployee.ToList(), "ID", "EmployeeCode", item.EmployeeID);
            ViewBag.ApproveBy = new SelectList(db.tblEmployee.ToList(), "ID", "Fullname", item.ApproveBy);
        }

        public void getViewBagforCreate()
        {
            ViewBag.DisciplineType = new SelectList(db.tblDisciplineType.ToList(), "ID", "NameVN");
            ViewBag.EmployeeList = new SelectList(db.tblEmployee.ToList(), "ID", "EmployeeCode");
            ViewBag.ApproveBy = new SelectList(db.tblEmployee.ToList(), "ID", "Fullname");
        }

        [HttpPost]
        public ActionResult DeleteDiscipline(int id)
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                // TODO: Add update logic here
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var item = db.tblDiscipline.Where(a => a.ID == id).FirstOrDefault();
                    item.isActive = false;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["delete"] = "Xóa thành công";
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 9, item.NoDiscipline);
                    return RedirectToAction("Index");
                }
            }
            else return RedirectToAction("Login", "Home");
        }
    
        
    }
}
