using HRManager.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class TimekeepingMachineController : Controller
    {
        // GET: TimekeepingMachine

        private HRManagerEntities db = new HRManagerEntities();
        int typeLog = 65;
        ActionLog action = new ActionLog();


        // GET: Degree
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var item = db.tblTimeKeeper.ToList();
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
                    var item = db.tblTimeKeeper.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 11, item.TimeKeeperNameVN);
                    SetViewBagForDetails(item);
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
                        var item = db.tblTimeKeeper.Where(a => a.ID.Equals(id)).FirstOrDefault();
                        SetViewBagForDetails(item);
                        item.TimeKeeperNameEN = collection["TimeKeeperNameEN"];
                        item.TimeKeeperNameVN = collection["TimeKeeperNameVN"];
                        item.Location = collection["Location"];
                        item.IPAddress = collection["IPAddress"];
                        if (collection["DateBuy"].ToString() != "")
                            item.DateBuy = DateTime.Parse(collection["DateBuy"]);

                        if (collection["DateWarranty"].ToString() != "")
                            item.DateWarranty = DateTime.Parse(collection["DateWarranty"]);
                        item.Brand = collection["Brand"];
                        item.Model = collection["Model"];
                        item.Note = collection["Note"];

                        item.UserUpdated = Encryptor.EncryptString(Session["UserID"].ToString());
                        item.DateUpdated = DateTime.Now;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 8, item.TimeKeeperNameVN);
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
        public ActionResult Create(tblTimeKeeper item)
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

                        db.tblTimeKeeper.Add(item);

                        db.SaveChanges();
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 7, item.TimeKeeperNameVN);
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
        public void SetViewBagForDetails(tblTimeKeeper item)
        {
           

            //Info time overtime
            ViewBag.dateBuy = DateTime.Parse(item.DateBuy.ToString()).ToString("yyyy-MM-dd");
            ViewBag.dateWarranty = DateTime.Parse(item.DateWarranty.ToString()).ToString("yyyy-MM-dd");
        }

        

        #endregion
    }
}