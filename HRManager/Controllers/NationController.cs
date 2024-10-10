using HRManager.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class NationController : Controller
    {
        private HRManagerEntities db = new HRManagerEntities();
        int typeLog = 50;
        ActionLog action = new ActionLog();


        // GET: Degree
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var item = db.tblNation.ToList();
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 6, "");
                    return View(item);
                }
            }
            else return RedirectToAction("Login", "Home");
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var item = db.tblNation.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 11, item.NameNationVN);
                    return View(item);
                }

            }
            else return RedirectToAction("Login", "Home");
        }

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
                        var item = db.tblNation.Where(a => a.ID.Equals(id)).FirstOrDefault();
                        item.NameNationVN = collection["NameNationVN"];
                        item.NameNationEN = collection["NameNationEN"];
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
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 8, item.NameNationVN);
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




        [HttpGet]
        public ActionResult Create()
        {
            if (Session["UserID"] != null)
                return View();
            else return RedirectToAction("Login", "Home");
        }

        // POST: Position/Create
        [HttpPost]
        public ActionResult Create(tblNation item)
        {
            if (Session["UserID"] != null)
            {
                try
                {
                    using (HRManagerEntities db = new HRManagerEntities())
                    {
                        // TODO: Add insert logic here
                        item.isActive = true;
                        db.tblNation.Add(item);

                        db.SaveChanges();
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 7, item.NameNationVN);
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
    }
}