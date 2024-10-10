using HRManager.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class WorkingShiftController : Controller
    {
        private HRManagerEntities db = new HRManagerEntities();
        int typeLog = 67;
        ActionLog action = new ActionLog();


        // GET: Degree
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var item = db.tblWorkingShift.ToList();
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
                    var item = db.tblWorkingShift.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    SetViewBagForDetails(item);
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 11, item.WorkingShiftVN);
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
                        var item = db.tblWorkingShift.Where(a => a.ID.Equals(id)).FirstOrDefault();
                        SetViewBagForDetails(item);
                        item.WorkingShiftVN = collection["WorkingShiftVN"];
                        item.WorkingShiftEN = collection["WorkingShiftEN"];

                        //AM
                        if (collection["fromTimeAM"].ToString() != "" && collection["fromTimeAM"].ToString().Length > 4)
                        {
                            item.fromTimeAM = TimeSpan.Parse(collection["fromTimeAM"].ToString().Substring(0, 5));
                        }

                        if (collection["toTimeAM"].ToString() != "" && collection["toTimeAM"].ToString().Length > 4)
                        {
                            item.toTimeAM = TimeSpan.Parse(collection["toTimeAM"].ToString().Substring(0, 5));
                        }


                        //PM
                        if (collection["fromTimePM"].ToString() != "" && collection["fromTimePM"].ToString().Length > 4)
                        {
                            item.fromTimePM = TimeSpan.Parse(collection["fromTimePM"].ToString().Substring(0, 5));
                        }

                        if (collection["toTimePM"].ToString() != "" && collection["toTimePM"].ToString().Length > 4)
                        {
                            item.toTimePM = TimeSpan.Parse(collection["toTimePM"].ToString().Substring(0, 5));
                        }
                        item.Note = collection["Note"];
                        
                        item.UserUpdated = Encryptor.EncryptString(Session["UserID"].ToString());
                        item.DateUpdated = DateTime.Now;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 8, item.WorkingShiftVN);
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
        public ActionResult Create(tblWorkingShift item)
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

                        db.tblWorkingShift.Add(item);

                        db.SaveChanges();
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 7, item.WorkingShiftVN);
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

        public void SetViewBagForDetails(tblWorkingShift item)
        {
            //Info time overtime
            ViewBag.hourStart = DateTime.Parse(item.fromTimeAM.ToString()).ToString("HH:mm:ss");
            ViewBag.hourEnd = DateTime.Parse(item.toTimeAM.ToString()).ToString("HH:mm:ss");

            ViewBag.hourStartPM = DateTime.Parse(item.fromTimePM.ToString()).ToString("HH:mm:ss");
            ViewBag.hourEndPM = DateTime.Parse(item.toTimePM.ToString()).ToString("HH:mm:ss");
        }
    }
}