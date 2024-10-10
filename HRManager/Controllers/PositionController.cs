using HRManager.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace HRManager.Controllers
{
    public class PositionController : Controller
    {
        private HRManagerEntities db = new HRManagerEntities();
        int typeLog = 31;
        ActionLog action = new ActionLog();


        // GET: Position
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var position = db.tblPosition.ToList();
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 6, "");
                    return View(position);
                }
            }
            else return RedirectToAction("Login", "Home");
        }

        public ActionResult PositionDetail(int id)
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var positionDetail = db.tblPosition.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 11, positionDetail.PositionNameVN);
                    return View(positionDetail);
                }

            }
             else return RedirectToAction("Login","Home");
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
                        var item = db.tblPosition.Where(a => a.ID.Equals(id)).FirstOrDefault();
                        item.PositionNameVN = collection["PositionNameVN"];
                        item.PositionNameEN = collection["PositionNameEN"];
                        if(collection["isActive"].ToString() == "1")
                        {
                            item.isActive = true;
                        }
                        else
                        {
                            item.isActive = false;
                        }
                        item.DateUpdated = DateTime.Now;
                        item.UserUpdated = Encryptor.EncryptString(Session["UserID"].ToString());

                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 8, "");
                        TempData["update"] = "Cập nhật thành công";
                        return RedirectToAction("Index");
                    }
                }
                catch
                {
                    return View();
                }
            }
            else return RedirectToAction("Login","Home");
          
        }

        // GET: Position/Create
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["UserID"] != null) 
                return View(); 
            else return RedirectToAction("Login", "Home");
        }

        // POST: Position/Create
        [HttpPost]
        public ActionResult Create(tblPosition position)
        {
            if (Session["UserID"] != null)
            {
                try
                {
                    using (HRManagerEntities db = new HRManagerEntities())
                    {
                        // TODO: Add insert logic here
                        position.isActive = true;
                        db.tblPosition.Add(position);

                        db.SaveChanges();
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 7, "");
                        TempData["insert"] = "Thêm mới thành công";
                        return RedirectToAction("Index");
                    }
                }
                catch
                {
                    return View();
                }
            }
            else return RedirectToAction("Login","Home");
        }
    }
}