using HRManager.Common;
using HRManager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class AllowanceCategoryController : Controller
    {
        private HRManagerEntities db = new HRManagerEntities();
        int typeLog = 69;
        ActionLog action = new ActionLog();


        // GET: Degree
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var item = db.tblAllowanceCategory.ToList();
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
                    var item = db.tblAllowanceCategory.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 11, item.AllowanceNameVN);
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
                        var item = db.tblAllowanceCategory.Where(a => a.ID.Equals(id)).FirstOrDefault();
                        item.AllowanceNameVN = collection["AllowanceNameVN"];
                        item.AllowanceNameEN = collection["AllowanceNameEN"];
                        item.Money = (collection["Money"].ToString() == "") ? 0 : float.Parse(collection["Money"].ToString());
                        
                        item.UserUpdated = Encryptor.EncryptString(Session["UserID"].ToString());
                        item.DateUpdated = DateTime.Now;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 8, item.AllowanceNameVN);
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
        public ActionResult Create(tblAllowanceCategory item)
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

                        db.tblAllowanceCategory.Add(item);

                        db.SaveChanges();
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 7, item.AllowanceNameVN);
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

        public ActionResult GetAllowanceCategoryDetails(int id)
        {
            var AllowanceCategory = db.tblAllowanceCategory.Where(x => x.ID == id).FirstOrDefault();
            return Json(AllowanceCategory, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllowanceDetails_ByEmployee(int id, int EmployeeID)
        {
            //check trùng dữ liệu
            var check = db.tblAllowance.Where(x => x.EmployeeID == EmployeeID && x.AllowanceCategoryID == id).FirstOrDefault();
            if (check != null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Dữ liệu đã tồn tại.");
            }

            //string sql = "select COALESCE(CAST(a.ID AS INT), 0) as ID, ac.AllowanceNameVN as AllowanceNameVN , " +
            //             " COALESCE(CAST(ac.Money AS INT), 0) AS MoneyAllowance, " +
            //             " COALESCE(CAST(a.EmployeeID AS INT), 0) AS EmployeeID " +
            //             "from tblAllowance a " +
            //             "left join tblAllowanceCategory ac on ac.ID = a.AllowanceCategoryID " +
            //             "where a.ID = " + id;

            //var item = db.Database.SqlQuery<ViewAllowanceDetails>(sql).ToList();

            var item = db.tblAllowanceCategory.Where(x => x.ID == id).FirstOrDefault();
            //var allowanceDetail = db.tblAllowance.Where(x => x.EmployeeID == id).ToList();
            return Json(item, JsonRequestBehavior.AllowGet);
        }
    }
}