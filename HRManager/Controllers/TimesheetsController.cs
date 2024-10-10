using HRManager.Common;
using HRManager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace HRManager.Controllers
{
    public class TimesheetsController : Controller
    {
        // GET: timeSheet
        private HRManagerEntities db = new HRManagerEntities();
        int typeLog = 19;
        ActionLog action = new ActionLog();
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var item = db.tblTimesheet.Where(x => x.isActive == true).ToList();
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
                    List<tblTimesheetDetail> timesheetdetails = db.tblTimesheetDetail.Where(x => x.TimesheetCode == id && x.isActive == true).ToList();
                    List<tblEmployee> employees = db.tblEmployee.Where(x => x.isActive == true).ToList();
                    List<tblDepartment> departments = db.tblDepartment.Where(x => x.isActive == true).ToList();

                    var item = from tsd in timesheetdetails
                               join e in employees on tsd.EmployeeID equals e.ID into table1
                               from e in table1.ToList()
                               join d in departments on e.DepartmentID equals d.ID into table2
                               from d in table2.ToList()
                               orderby tsd.ID descending
                               select new ViewModel
                               {
                                   employee = e,
                                   department = d,
                                   timesheetDetail = tsd,
                               };
                    var timesheet = db.tblTimesheet.Where(x => x.ID == id).FirstOrDefault();
                    ViewBag.TimesheetCode = timesheet.TimesheetYear + "/" + timesheet.TimesheetMonth;
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 11, timesheet.TimesheetYear + "/" + timesheet.TimesheetMonth);
                    return View(item);
                }

            }
            else return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            if (Session["UserID"] != null)
            {
                try
                {
                    using (HRManagerEntities db = new HRManagerEntities())
                    {
                        // Insert tblTimeSheet
                        tblTimesheet item = new tblTimesheet();
                        item.TimesheetYear = int.Parse(DateTime.Now.ToString("yyyy"));
                        item.TimesheetMonth = int.Parse(collection["TimesheetMonth"]);
                        item.fromDate = DateTime.Parse(collection["fromDate"]);
                        item.toDate = DateTime.Parse(collection["toDate"]);
                        item.isActive = true;
                        item.DateCreate = DateTime.Now;
                        item.UserCreated = Encryptor.EncryptString(Session["UserID"].ToString());
                        db.tblTimesheet.Add(item);
                        db.SaveChanges();
                        var TimesheetID = item.ID;
                        //Insert tblTimeSheet Detail

                        //Step 01 . Lấy thông tin nhân viên
                        var employee = db.tblEmployee.Where(x => x.isActive == true).ToList();

                        foreach (var itemEmployee in employee)
                        {
                            int year = int.Parse(DateTime.Now.ToString("yyyy"));
                            int month = int.Parse(collection["TimesheetMonth"]);

                            //Step 02 . Lấy thông tin ca làm việc
                            var ShiftWorkID = db.tblShiftArrange.Where(x => x.EmployeeID == itemEmployee.ID && x.Year == year && x.Month == month).FirstOrDefault().WorkingShiftID;
                            if (ShiftWorkID == null) continue;

                            var shift = db.tblWorkingShift.Where(x => x.ID == ShiftWorkID).FirstOrDefault();
                            TimeSpan TimeSpanWorkingtimeAM = TimeSpan.Parse(shift.fromTimeAM.ToString());
                            TimeSpan TimeSpanWorkingtimePM = TimeSpan.Parse(shift.toTimePM.ToString());


                            //get number day in month
                            int days = DateTime.DaysInMonth(year, month);
                            var totaldayWork = 0;
                            var totalovertime = 0;

                            var totaldaySunday = 0;
                            var totalovertimeSunday = 0;

                            var totaldayHoliday = 0;
                            var totalovertimeHoliday = 0;

                            for (DateTime currentDate = DateTime.Parse(collection["fromDate"]); currentDate <= DateTime.Parse(collection["toDate"]); currentDate = currentDate.AddDays(1))
                            {
                                if(itemEmployee.ID== 2 &&  currentDate == DateTime.Parse("2023-08-05"))
                                {

                                }
                                Boolean CheckSunday = (currentDate.DayOfWeek == DayOfWeek.Sunday) ? true : false;
                                //Kiểm tra quét thẻ buổi sáng
                                var timeKeeping = db.tblHistoryTimekeeping.Where(x => x.EmployeeID == itemEmployee.ID && x.TimekeepingDate == currentDate && x.TimekeepingHour <= TimeSpanWorkingtimeAM).FirstOrDefault();
                                int overTimeAM = 0;
                                if (timeKeeping != null)
                                {
                                    var timeKeepingString = timeKeeping.TimekeepingHour.ToString();
                                    if ((TimeSpanWorkingtimeAM - TimeSpan.Parse(timeKeepingString)).Hours >= 1)
                                    {
                                        overTimeAM += 1;
                                    }
                                }


                                //Kiểm tra quét thẻ buổi trưa
                                var timeKeepingPM = db.tblHistoryTimekeeping.Where(x => x.EmployeeID == itemEmployee.ID && x.TimekeepingDate == currentDate && x.TimekeepingHour >= TimeSpanWorkingtimePM).FirstOrDefault();
                                int overTimePM = 0;
                                if (timeKeepingPM != null)
                                {
                                    var timeKeepingString = timeKeepingPM.TimekeepingHour.ToString();
                                    if ((TimeSpanWorkingtimePM - TimeSpan.Parse(timeKeepingString)).Hours >= 1)
                                    {
                                        overTimePM += 1;
                                    }
                                }


                                if (timeKeeping != null && timeKeepingPM != null)
                                {
                                    if (CheckSunday)
                                    {
                                        totaldaySunday += 1;
                                        totalovertimeSunday += (overTimeAM + overTimePM);
                                    }
                                    else if (CheckHoliday(currentDate, year))
                                    {
                                        totaldayHoliday += 1;
                                        totalovertimeHoliday += (overTimeAM + overTimePM);
                                    }
                                    else
                                    {
                                        totaldayWork += 1;
                                        totalovertime += (overTimeAM + overTimePM);
                                    }
                                }
                            }
                            var totalHourWork = totaldayWork * 8;
                            var totalHourSunday = totaldaySunday * 8;
                            var totalHourHoliday = totaldayHoliday * 8;

                            //Lấy số ngày , số giờ nghỉ có phép


                            ViewLeaveDetails leaveDetails = GetDayLeave(itemEmployee.ID , DateTime.Parse(collection["fromDate"]) , DateTime.Parse(collection["toDate"]) , 1)[0];


                            //Số ngày nghie không phép
                            ViewLeaveDetails leaveDetailsAbsend = GetDayLeave(itemEmployee.ID, DateTime.Parse(collection["fromDate"]), DateTime.Parse(collection["toDate"]), 2)[0];


                            tblTimesheetDetail timesheetDetail = new tblTimesheetDetail
                            {
                                TimesheetCode = TimesheetID,
                                totalDayWork = totaldayWork,
                                totalHourWork = totalHourWork,
                                totalHourOvertime = totalovertime,
                                totalHourOvertimeSunday = totalHourSunday + totalovertimeSunday,
                                totalHourOvertimeHoliday = totalHourHoliday + totalovertimeHoliday,
                                EmployeeID = itemEmployee.ID,
                                totalDayLeave = double.Parse(leaveDetails.totalDayLeave.ToString()),
                                totalHourLeave = double.Parse(leaveDetails.totalHoursLeave.ToString()),
                                totalDayAbsent = double.Parse(leaveDetailsAbsend.totalDayLeave.ToString()),
                                totalHourAbsent = double.Parse(leaveDetailsAbsend.totalHoursLeave.ToString()),
                                isActive = true
                            };
                            db.tblTimesheetDetail.Add(timesheetDetail);
                            db.SaveChanges();
                        }

                        
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
            else return RedirectToAction("Login", "Home");
        }


        public Boolean CheckHoliday(DateTime datework, int YearWork)
        {
            var item = db.tblHoliday.Where(x => x.isActive == true && x.YearHoliday == YearWork && (x.fromDateHoliday <= datework && x.toDateHoliday >= datework)).FirstOrDefault();
            Boolean isHoliday = (item != null) ? true : false;
            return isHoliday;
        }

        public List<ViewLeaveDetails> GetDayLeave(int EmployeeID, DateTime fromdate, DateTime todate, int typeLeaveID)
        {
            var Char = "= ";
            if (typeLeaveID > 1) Char = "!= ";
            string sql = "select " +
                "COALESCE(cast(count(ld.LeaveDate) as DECIMAL(9,1)),0) as totalDayLeave , " +
                "COALESCE(cast(sum(ld.LeaveQtyHours) as DECIMAL(9,1)),0) as totalHoursLeave " +
                "from tblLeave l " +
                "left join tblleaveDetail ld on ld.LeaveID = l.ID " +
                "where EmployeeID = " + EmployeeID +
                " and l.TypeLeaveID " + Char + "1" +
                " and not(l.fromDate > '" + todate.ToString("yyyy-MM-dd 00:00:00") + "' or l.toDate < '" + fromdate.ToString("yyyy-MM-dd 00:00:00") + "')";
            var Leave = db.Database.SqlQuery<ViewLeaveDetails>(sql).ToList();
            return Leave;
        }


        public JsonResult CheckTimesheetExistence(string month)
        {
            // Thực hiện kiểm tra tháng tồn tại trong cơ sở dữ liệu ở đây
            bool monthExistsInDatabase = false; // Giả sử tháng chưa tồn tại
            bool NotShiftArrange = false;

            int year = int.Parse(DateTime.Now.ToString("yyyy"));
            int Month = int.Parse(month);
            var item = db.tblTimesheet.Where(x=>x.TimesheetYear == year && x.TimesheetMonth == Month && x.isActive == true).FirstOrDefault();
            if(item != null)
            {
                monthExistsInDatabase = true;
            }

            //Đếm số lượng nhân viên
            var countEmployee = db.tblEmployee.Where(x=>x.isActive == true).Count();
            //Đếm số lượng nhân viên đã được xếp ca
            var countShiftArrange = db.tblShiftArrange.Where(x => x.Month == Month && x.Year == year).Count();
            //Sl nhân viên chưa xếp ca
            var EmployeeNotShiftArrange = countEmployee - countShiftArrange;

            if (EmployeeNotShiftArrange > 0)
            {
                NotShiftArrange = true;
            }

            return Json(new { exists = monthExistsInDatabase , NotShiftArrange = NotShiftArrange , countEmployee = EmployeeNotShiftArrange }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteTimeSheet(int id)
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                // TODO: Add update logic here
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    //delete timesheet
                    var item = db.tblTimesheet.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    item.isActive = false;
                    item.DateUpdated = DateTime.Now; 
                    item.UserUpdated = Encryptor.EncryptString(Session["UserID"].ToString());
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();


                    //delete timesheet detail
                    var itemdetail = db.tblTimesheetDetail.Where(a => a.TimesheetCode == id).ToList();
                    foreach(var detail in itemdetail)
                    {
                        detail.DateUpdated = DateTime.Now;
                        detail.UserUpdated = Encryptor.EncryptString(Session["UserID"].ToString());
                        detail.isActive = false;
                        db.Entry(detail).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    


                    TempData["delete"] = "Xóa thành công";
                    return RedirectToAction("Index");
                }
            }
            else return RedirectToAction("Login", "Home");
        }
    }
}