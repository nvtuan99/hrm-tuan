using HRManager.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class HolidayController : Controller
    {
        private HRManagerEntities db = new HRManagerEntities();
        int typeLog = 62;
        ActionLog action = new ActionLog();

        // GET: Holiday
       
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var item = db.tblHoliday.Where(x => x.isActive == true ).ToList();
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
                    var item = db.tblHoliday.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    ViewBag.fromDateHoliday = DateTime.Parse(item.fromDateHoliday.ToString()).ToString("yyyy-MM-dd");
                    ViewBag.toDateHoliday = DateTime.Parse(item.toDateHoliday.ToString()).ToString("yyyy-MM-dd");
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 11, item.NameHolidayVN);
                    return View(item);
                }

            }
            else return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public ActionResult Details(int id, FormCollection collection)
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                try
                {
                    // TODO: Add update logic here
                    using (HRManagerEntities db = new HRManagerEntities())
                    {
                        var item = db.tblHoliday.Where(a => a.ID.Equals(id)).FirstOrDefault();
                        item.YearHoliday = int.Parse(DateTime.Now.ToString("yyyy"));
                        item.NameHolidayVN = collection["NameHolidayVN"];
                        item.NameHolidayEN = collection["NameHolidayEN"];
                        item.fromDateHoliday = DateTime.Parse(collection["fromDateHoliday"]);
                        item.toDateHoliday = DateTime.Parse(collection["toDateHoliday"]);
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
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 8, item.NameHolidayVN);
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
        public ActionResult Create(tblHoliday item)
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
                        db.tblHoliday.Add(item);
                        db.SaveChanges();
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 7, item.NameHolidayVN);
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