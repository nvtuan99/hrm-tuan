using HRManager.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class CompanyController : Controller
    {
        // GET: Company
        private HRManagerEntities db = new HRManagerEntities();
        int typeLog = 58;
        ActionLog action = new ActionLog();


        // GET: Degree
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var item = db.tblCompany.ToList();
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
                    var item = db.tblCompany.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 11, item.NameCompanyVN);
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
                        var item = db.tblCompany.Where(a => a.ID.Equals(id)).FirstOrDefault();
                        item.NameCompanyVN = collection["NameCompanyVN"];
                        item.NameCompanyEN = collection["NameCompanyEN"];
                        item.Address = collection["Address"];
                        item.Phone = collection["Phone"];
                        item.Tax = collection["Tax"];
                        item.Deputy = collection["Deputy"];
                        item.Title = collection["Title"];
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
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 8, item.NameCompanyVN);
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
        public ActionResult Create(tblCompany item)
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

                        db.tblCompany.Add(item);

                        db.SaveChanges();
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 7, item.NameCompanyVN);
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


        public ActionResult GetCompanyInfo(int companyID)
        {
            var company = db.tblCompany.FirstOrDefault(e => e.ID == companyID);
            if (company != null)
            {
                var data = new
                {
                    address = company.Address,
                    phone = company.Phone,
                    deputy = company.Deputy,
                    title = company.Title,
                    tax = company.Tax,
                };

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }


    }
}