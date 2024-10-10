using HRManager.Common;
using HRManager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class SalarySheetController : Controller
    {
        private HRManagerEntities db = new HRManagerEntities();
        int typeLog = 23;
        ActionLog action = new ActionLog();

        // GET: SalarySheet
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var item = db.tblSalarySheet.Where(x => x.isActive == true).ToList();
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 6, "");
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
                        // Insert New SalarySheet
                        tblSalarySheet item = new tblSalarySheet();
                        int year = int.Parse(DateTime.Now.ToString("yyyy"));
                        int month = int.Parse(collection["SalarySheetMonth"]);

                        item.SalarySheetYear = year;
                        item.SalarySheetMonth = month;


                        item.isActive = true;
                        item.DateCreate = DateTime.Now;
                        item.UserCreated = Encryptor.EncryptString(Session["UserID"].ToString());
                        db.tblSalarySheet.Add(item);
                        db.SaveChanges();
                        var SalarySheetID = item.ID;
                        //Insert tblSalarySheet Detail

                        //Step 01 . Lấy thông tin nhân viên
                        var employee = db.tblEmployee.Where(x => x.isActive == true).ToList();

                        foreach (var itemEmployee in employee)
                        {
                            //Lấy thông tin tblTimeSheetDetail
                            var timesheet = db.tblTimesheet.Where(x => x.isActive == true && x.TimesheetYear == year && x.TimesheetMonth == month).FirstOrDefault();

                            if (timesheet != null)
                            {
                                var timesheetdetail = db.tblTimesheetDetail.Where(x => x.EmployeeID == itemEmployee.ID && x.isActive == true && x.TimesheetCode == timesheet.ID).FirstOrDefault();
                                if (timesheetdetail != null)
                                {
                                    double totalHourWork = (timesheetdetail.totalHourWork.ToString() != "") ? double.Parse(timesheetdetail.totalHourWork.ToString()) : 0;
                                    double totalHourOvertime = (timesheetdetail.totalHourOvertime.ToString() != "") ? double.Parse(timesheetdetail.totalHourOvertime.ToString()) : 0;
                                    double totalHourHoliday = (timesheetdetail.totalHourOvertimeHoliday.ToString() != "") ? double.Parse(timesheetdetail.totalHourOvertimeHoliday.ToString()) : 0;
                                    double totalHourSunday = (timesheetdetail.totalHourOvertimeSunday.ToString() != "") ? double.Parse(timesheetdetail.totalHourOvertimeSunday.ToString()) : 0;
                                    double totalHourLeave = (timesheetdetail.totalHourLeave.ToString() != "") ? double.Parse(timesheetdetail.totalHourLeave.ToString()) : 0;
                                    double totalHourAbsent = (timesheetdetail.totalHourAbsent.ToString() != "") ? double.Parse(timesheetdetail.totalHourAbsent.ToString()) : 0;

                                    //Ghi lại tổng phụ cấp
                                    var allowance = db.tblAllowance.Where(x => x.isActive == true && x.EmployeeID == itemEmployee.ID).ToList();
                                    double totalAllowance = 0;
                                    if (allowance != null)
                                    {
                                        foreach (var allowanceItem in allowance)
                                        {
                                            //tong tien
                                            var allowanceCategory = db.tblAllowanceCategory.Where(x => x.ID == allowanceItem.AllowanceCategoryID).FirstOrDefault();

                                            if (allowanceCategory != null)
                                            {
                                                if (allowanceCategory.Money.ToString() != "")
                                                    totalAllowance += double.Parse(allowanceCategory.Money.ToString());

                                                //ghi lai chi tiet phu cap theo thang
                                                tblSalarySheetAllowance salarySheetAllowance = new tblSalarySheetAllowance();
                                                salarySheetAllowance.SalarySheetID = SalarySheetID;
                                                salarySheetAllowance.EmployeeID = itemEmployee.ID;
                                                salarySheetAllowance.AllowanceID = allowanceItem.AllowanceCategoryID;
                                                salarySheetAllowance.AllowanceMoney = allowanceCategory.Money;
                                                salarySheetAllowance.isActive = true;
                                                salarySheetAllowance.DateCreate = DateTime.Now;
                                                salarySheetAllowance.UserCreate = Encryptor.EncryptString(Session["UserID"].ToString());
                                                db.tblSalarySheetAllowance.Add(salarySheetAllowance);
                                                db.SaveChanges();
                                            }
                                        }

                                    }

                                    //Cong thuc tinh luong
                                    var salaryFormular = db.tblISalaryFormula.FirstOrDefault();

                                    //Basic Salary 
                                    double basicSalary = (itemEmployee.Salary.ToString() != "") ? double.Parse(itemEmployee.Salary.ToString()) : 0;
                                    if (basicSalary > 0)
                                    {
                                        // So tien cong di lam trong 1 gio = luong cb / 26 ngay / thang / 8 gio lam viec
                                        double moneyOneHour = Math.Round(basicSalary / 26 / 8, 0);

                                        //so tien cong di lam 1 thang = 1 gio lam viec * so gio lam viec
                                        var moneyWork = Math.Round(moneyOneHour * totalHourWork, 0);
                                        var moneyOvertime = Math.Round(moneyOneHour * totalHourOvertime * double.Parse(salaryFormular.Overtime.ToString()), 0);
                                        var moneyOvertimeSunday = Math.Round(moneyOneHour * totalHourSunday * double.Parse(salaryFormular.OvertimeSunday.ToString()), 0);
                                        var moneyOvertimeHoliday = Math.Round(moneyOneHour * totalHourHoliday * double.Parse(salaryFormular.OvertimeHoliday.ToString()), 0);
                                        var moneyPaidLeave = Math.Round(moneyOneHour * totalHourLeave, 0);

                                        var totalPay = moneyWork + moneyOvertime + moneyOvertimeSunday + moneyOvertimeHoliday + moneyPaidLeave + totalAllowance;

                                        //so tien dong bao hiem
                                        var moneyUnPaidLeave = Math.Round(moneyOneHour * totalHourAbsent, 0);
                                        var bhxh = Math.Round(basicSalary * double.Parse(salaryFormular.BHXH.ToString()) / 100, 0);
                                        var bhyt = Math.Round(basicSalary * double.Parse(salaryFormular.BHYT.ToString()) / 100, 0);
                                        var bhtn = Math.Round(basicSalary * double.Parse(salaryFormular.BHTN.ToString()) / 100, 0);
                                        var uniondues = Math.Round(basicSalary * double.Parse(salaryFormular.UnionDues.ToString()) / 100, 0);

                                        var totalUnPaid = bhxh + bhyt + bhtn + uniondues;

                                        //tong so tien
                                        var totalSalaryInMonth = totalPay - totalUnPaid;


                                        //insert salarySheetDetail 
                                        tblSalarySheetDetail salarySheetDetail = new tblSalarySheetDetail();
                                        salarySheetDetail.EmployeeID = itemEmployee.ID;
                                        salarySheetDetail.SalarySheetID = SalarySheetID;
                                        salarySheetDetail.BasicSalary = basicSalary;
                                        salarySheetDetail.TimesheetDetailID = timesheetdetail.ID;


                                        //money work
                                        salarySheetDetail.MoneyWork = moneyWork;
                                        salarySheetDetail.MoneyOvertime = moneyOvertime;
                                        salarySheetDetail.MoneySunday = moneyOvertimeSunday;
                                        salarySheetDetail.MoneyHoliday = moneyOvertimeHoliday;
                                        salarySheetDetail.MoneyPadLeave = moneyPaidLeave;
                                        salarySheetDetail.MoneyUnpaidLeave = moneyUnPaidLeave;


                                        //insurance - union
                                        salarySheetDetail.BHXH = bhxh;
                                        salarySheetDetail.BHYT = bhyt;
                                        salarySheetDetail.BHTN = bhtn;
                                        salarySheetDetail.UnionDues = uniondues;


                                        //total
                                        salarySheetDetail.TotalAllowance = totalAllowance;
                                        salarySheetDetail.TotalSalary = totalSalaryInMonth;
                                        salarySheetDetail.isActive = true;
                                        salarySheetDetail.DateCreate = DateTime.Now;
                                        salarySheetDetail.UserCreate = Encryptor.EncryptString(Session["UserID"].ToString());
                                        db.tblSalarySheetDetail.Add(salarySheetDetail);
                                        db.SaveChanges();
                                    }
                                }
                            }
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


        [HttpGet]
        public ActionResult Details(int id)
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var salarysheet = db.tblSalarySheet.Where(x => x.ID == id).FirstOrDefault();
                    var timesheets = db.tblTimesheet.Where(x => x.isActive == true && x.TimesheetYear == salarysheet.SalarySheetYear && x.TimesheetMonth == salarysheet.SalarySheetMonth).FirstOrDefault();

                    List<tblSalarySheetDetail> salarySheetDetails = db.tblSalarySheetDetail.Where(x => x.SalarySheetID == id && x.isActive == true).ToList();
                    List<tblEmployee> employees = db.tblEmployee.Where(x => x.isActive == true).ToList();
                    List<tblDepartment> departments = db.tblDepartment.Where(x => x.isActive == true).ToList();
                    List<tblTimesheetDetail> timesheetDetails = db.tblTimesheetDetail.Where(x => x.isActive == true && x.TimesheetCode == timesheets.ID).ToList();
                    var item = from ssd in salarySheetDetails
                               join e in employees on ssd.EmployeeID equals e.ID into table1
                               from e in table1.ToList()
                               join d in departments on e.DepartmentID equals d.ID into table2
                               from d in table2.ToList()
                               join tsd in timesheetDetails on ssd.TimesheetDetailID equals tsd.ID into table3
                               from tsd in table3.ToList()
                               orderby ssd.ID descending
                               select new ViewModel
                               {
                                   employee = e,
                                   department = d,
                                   salarySheetDetail = ssd,
                                   timesheetDetail = tsd
                               };

                    ViewBag.SalarySheetCode = salarysheet.SalarySheetYear + "/" + salarysheet.SalarySheetMonth;
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 11, salarysheet.SalarySheetYear + "/" + salarysheet.SalarySheetMonth);
                    return View(item);
                }

            }
            else return RedirectToAction("Login", "Home");
        }

        [HttpGet]
        public ActionResult SalaryByEmployee()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    List<tblSalarySheetDetail> salarySheetDetails = db.tblSalarySheetDetail.Where(x => x.isActive == true && x.EmployeeID == userInfo.employeeID).ToList();
                    List<tblSalarySheet> salarySheets = db.tblSalarySheet.Where(x => x.isActive == true).ToList();
                    List<tblEmployee> employees = db.tblEmployee.Where(x => x.isActive == true && x.ID == userInfo.employeeID).ToList();
                    List<tblDepartment> departments = db.tblDepartment.Where(x => x.isActive == true).ToList();
                    var item = from ssd in salarySheetDetails
                               join e in employees on ssd.EmployeeID equals e.ID into table1
                               from e in table1.ToList()
                               join d in departments on e.DepartmentID equals d.ID into table2
                               from d in table2.ToList()
                               join ss in salarySheets on ssd.SalarySheetID equals ss.ID into table3
                               from ss in table3.ToList()
                               orderby ssd.ID descending
                               select new ViewModel
                               {
                                   employee = e,
                                   department = d,
                                   salarySheetDetail = ssd,
                                   salarySheet = ss
                               };

                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 11, "");
                    return View(item);
                }

            }
            else return RedirectToAction("Login", "Home");
        }

        [HttpGet]
        public ActionResult SalaryByEmployeeDetails(int id)
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    List<tblSalarySheetDetail> salarySheetDetails = db.tblSalarySheetDetail.Where(x => x.isActive == true && x.EmployeeID == userInfo.employeeID && x.SalarySheetID == id).ToList();
                    List<tblTimesheetDetail> timesheetDetails = db.tblTimesheetDetail.Where(x => x.isActive == true && x.EmployeeID == userInfo.employeeID).ToList();
                    List<tblSalarySheet> salarySheets = db.tblSalarySheet.Where(x => x.isActive == true && x.ID == id).ToList();
                    List<tblEmployee> employees = db.tblEmployee.Where(x => x.isActive == true && x.ID == userInfo.employeeID).ToList();
                    List<tblDepartment> departments = db.tblDepartment.Where(x => x.isActive == true).ToList();
                    var item = from ssd in salarySheetDetails
                               join e in employees on ssd.EmployeeID equals e.ID into table1
                               from e in table1.ToList()
                               join d in departments on e.DepartmentID equals d.ID into table2
                               from d in table2.ToList()
                               join ss in salarySheets on ssd.SalarySheetID equals ss.ID into table3
                               from ss in table3.ToList()
                               join tsd in timesheetDetails on ssd.TimesheetDetailID equals tsd.ID into table4
                               from tsd in table4.ToList()
                               orderby ssd.ID descending
                               select new ViewModel
                               {
                                   employee = e,
                                   department = d,
                                   salarySheetDetail = ssd,
                                   salarySheet = ss,
                                   timesheetDetail = tsd,
                               };

                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 11, "");
                    return View(item);
                }

            }
            else return RedirectToAction("Login", "Home");
        }

        public JsonResult CheckSalarySheetExist(string month)
        {
            bool monthExistsInDatabase = false;
            bool NoTimeSheet = false;

            int year = int.Parse(DateTime.Now.ToString("yyyy"));
            int Month = int.Parse(month);

            //kiểm tra bảng lương đã được tạo hay chưa ?
            var item = db.tblSalarySheet.Where(x => x.SalarySheetYear == year && x.SalarySheetMonth == Month && x.isActive == true).FirstOrDefault();
            if (item != null)
            {
                monthExistsInDatabase = true;
            }


            //kiểm tra bảng công đã được tạo hay chưa ?
            var itemTimeSheet = db.tblTimesheet.Where(x => x.TimesheetYear == year && x.TimesheetMonth == Month && x.isActive == true).FirstOrDefault();
            if (itemTimeSheet == null)
            {
                NoTimeSheet = true;
            }

            return Json(new { exists = monthExistsInDatabase, NoTimeSheet = NoTimeSheet }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteSalarySheet(int id)
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                // TODO: Add update logic here
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    //delete timesheet
                    var item = db.tblSalarySheet.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    item.isActive = false;
                    item.DateUpdated = DateTime.Now;
                    item.UserUpdated = Encryptor.EncryptString(Session["UserID"].ToString());
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();


                    ////delete timesheet detail
                    //var itemdetail = db.tblTimesheetDetail.Where(a => a.TimesheetCode == id.ToString()).ToList();
                    //foreach (var detail in itemdetail)
                    //{
                    //    detail.DateUpdated = DateTime.Now;
                    //    detail.UserUpdated = Encryptor.EncryptString(Session["UserID"].ToString());
                    //    detail.isActive = false;
                    //    db.Entry(detail).State = EntityState.Modified;
                    //    db.SaveChanges();
                    //}



                    TempData["delete"] = "Xóa thành công";
                    return RedirectToAction("Index");
                }
            }
            else return RedirectToAction("Login", "Home");
        }
    }
}