using HRManager.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class DepartmentController : Controller
    {
        // GET: Department
        int typeLog = 29;
        ActionLog action = new ActionLog();
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var dept = db.tblDepartment.ToList();
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 6, "");
                    return View(dept);
                }

            }
            else return RedirectToAction("Login","Home");
        }

        // GET: Department/Details/5
        public ActionResult Details(int id)
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var dept = db.tblDepartment.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 11, dept.NameDepartmentVN);
                    return View(dept);
                }
            }
            else return RedirectToAction("Login", "Home");
        }

        // GET: Department/Create
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["UserID"] != null)
                return View();
            else return RedirectToAction("Login", "Home");
        }

        // POST: Department/Create
        [HttpPost]
        public ActionResult Create(tblDepartment dept)
        {
            if (Session["UserID"] != null)
            {
                try
                {
                    using (HRManagerEntities db = new HRManagerEntities())
                    {
                        // TODO: Add insert logic here
                        dept.isActive = true;
                        db.tblDepartment.Add(dept);

                        db.SaveChanges();
                        TempData["insert"] = "Thêm mới thành công";
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 7, dept.NameDepartmentVN);
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

        // GET: Department/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Department/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                try
                {
                    // TODO: Add update logic here
                    using (HRManagerEntities db = new HRManagerEntities())
                    {
                        var item = db.tblDepartment.Where(a => a.ID.Equals(id)).FirstOrDefault();
                        item.NameDepartmentVN = collection["NameDepartmentVN"];
                        item.NameDepartmentEN = collection["NameDepartmentEN"];
                        if (collection["isActive"].ToString() == "1")
                        {
                            item.isActive = true;
                        }
                        else
                        {
                            item.isActive = false;
                        }
                        item.UserUpdated = Encryptor.EncryptString(Session["UserID"].ToString());
                        item.DateUpdated = DateTime.Now;

                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 8, item.NameDepartmentVN);
                        TempData["update"] = "Cập nhật thành công";
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
        // GET: Department/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Department/Delete/5
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
    }
}
