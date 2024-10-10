using HRManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class SysLogController : Controller
    {
        private HRManagerEntities db = new HRManagerEntities();
        // GET: SysLog
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                List<tblSysLog> syslogs = db.tblSysLog.ToList();
                List<tblSysFunction> sysFunctions = db.tblSysFunction.ToList();
                List<tblSysAction> sysActions = db.tblSysAction.ToList();

                var syslogRecord = from s in syslogs
                                   join f in sysFunctions on s.FunctionLog equals f.ID into table1
                                     from f in table1.ToList()
                                     join a in sysActions on s.ActionLog equals a.ID into table2
                                     from a in table2.ToList()
                                     orderby s.ID descending
                                     select new ViewModel
                                     {
                                         sysLog = s,
                                         sysFunction = f,
                                         sysAction = a
                                     };
                return View(syslogRecord);
            }
            else return RedirectToAction("Login", "Home");
        }
    }
}