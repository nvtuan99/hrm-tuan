using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class TimekeepingController : Controller
    {
        private HRManagerEntities db = new HRManagerEntities();
        // GET: Timekeeping
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetEvents()
        {
            List<tblHistoryTimekeeping> events = new List<tblHistoryTimekeeping>();
           
            // Thực hiện truy vấn cơ sở dữ liệu và đổ dữ liệu vào events
            var employeeID = int.Parse(Session["EmployeeID"].ToString());
            events = db.tblHistoryTimekeeping.Where(x => x.EmployeeID == employeeID).ToList();
            var eventList = events.Select(e => new
            {
                id = e.ID,
                title = e.TimekeepingHour.ToString(), // Sử dụng tên thuộc tính tương ứng
                start = e.TimekeepingDate // Sử dụng tên thuộc tính tương ứng
            });
            return Json(eventList, JsonRequestBehavior.AllowGet);
        }
    }
}