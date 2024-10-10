using HRManager.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class SalaryFormularController : Controller
    {
        HRManagerEntities db = new HRManagerEntities();
        LoadData loaddb = new LoadData();
        ActionLog action = new ActionLog();
        int typeLog = 28;


        // GET: SalaryFormular
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                var item = db.tblISalaryFormula.FirstOrDefault();
                return View(item);
            }
            else return RedirectToAction("Login", "Home");
        }

        [HttpGet]
        public ActionResult Details()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                var item = db.tblISalaryFormula.FirstOrDefault();
                return View(item);
            }
            else return RedirectToAction("Login", "Home");
        }


        [HttpPost]
        public ActionResult Details(FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserID"] != null)
                {
                    var item = db.tblISalaryFormula.FirstOrDefault();
                    item.BHYT = (collection["BHYT"].ToString() != "") ? double.Parse(collection["BHYT"]) : 0;
                    item.BHXH = (collection["BHXH"].ToString() != "") ? double.Parse(collection["BHXH"]) : 0;
                    item.BHTN = (collection["BHTN"].ToString() != "") ? double.Parse(collection["BHTN"]) : 0;
                    item.UnionDues = (collection["UnionDues"].ToString() != "") ? double.Parse(collection["UnionDues"]) : 0;
                    item.UnionDuesMax = (collection["UnionDuesMax"].ToString() != "") ? double.Parse(collection["UnionDuesMax"]) : 0;
                    item.Overtime = (collection["Overtime"].ToString() != "") ? double.Parse(collection["Overtime"]) : 0;
                    item.OvertimeSunday = (collection["OvertimeSunday"].ToString() != "") ? double.Parse(collection["OvertimeSunday"]) : 0;
                    item.OvertimeHoliday = (collection["OvertimeHoliday"].ToString() != "") ? double.Parse(collection["OvertimeHoliday"]) : 0;

                    item.DateUpdated = DateTime.Now;
                    item.UserUpdated = Encryptor.EncryptString(Session["UserID"].ToString());
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["update"] = "Cập nhật thành công";
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 8, "");

                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            else return RedirectToAction("Login", "Home");

        }
    }
}