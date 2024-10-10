using HRManager.Common;
using HRManager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class LeaveController : Controller
    {
        HRManagerEntities db = new HRManagerEntities();
        LoadData loaddb = new LoadData();
        ActionLog action = new ActionLog();
        int typeLog = 61;

        // GET: Leave
        [HttpGet]
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                List<tblLeave> leaves = db.tblLeave.Where(x => x.isActive == true).ToList();
                List<tblTypeLeave> typeleaves = db.tblTypeLeave.Where(x => x.isActive == true).ToList();
                List<tblEmployee> employees = (userInfo.isManager) ? db.tblEmployee.Where(x => x.isActive == true).ToList() : db.tblEmployee.Where(x => x.isActive == true && x.ID == userInfo.employeeID).ToList();
                List<tblDepartment> departments = db.tblDepartment.Where(x => x.isActive == true).ToList();
                List<tblPosition> positions = db.tblPosition.Where(x => x.isActive == true).ToList();

                var item = from l in leaves
                           join tl in typeleaves on l.TypeLeaveID equals tl.ID into table1
                           from tl in table1.ToList()
                           join e in employees on l.EmployeeID equals e.ID into table2
                           from e in table2.ToList()
                           join d in departments on e.DepartmentID equals d.ID into table3
                           from d in table3.ToList()
                           join p in positions on e.PositionID equals p.ID into table4
                           from p in table4.ToList()
                           orderby l.ID descending
                           select new ViewModel
                           {
                               employee = e,
                               department = d,
                               typeleave = tl,
                               leave = l,
                               position = p,
                           };
                action.InsertActionLog(Session["UserName"].ToString(), typeLog, 6, "");
                return View(item);

            }
            else return RedirectToAction("Login", "Home");
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (ModelState.IsValid && id != null)
            {
                try
                {
                    if (Session["UserID"] != null)
                    {
                        var item = db.tblLeave.Where(a => a.ID == id).FirstOrDefault();
                        SetViewBagForDetails(item);
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 11, "");
                        return View(item);
                    }
                    else return RedirectToAction("Login", "Home");
                }
                catch
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            else return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public ActionResult Details(int id, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserID"] != null)
                {
                    // cập nhật lại số ngày phép năm = cộng lại số ngày phép năm đã trừ , và trừ đi số ngày nghỉ mới 
                    var AnnualLeaveDetail = db.tblAnnualLeaveDetail.Where(x => x.LeaveID == id).FirstOrDefault();
                    var employeeID = AnnualLeaveDetail.EmployeeID;
                    var Qty_AnnualLEave_old = AnnualLeaveDetail.QtyAnnualLeave;

                    //delete dữ liệu detail cũ
                    AnnualLeaveDetail.isActive = false;
                    db.Entry(AnnualLeaveDetail).State = EntityState.Modified;
                    db.SaveChanges();

                    //cộng lại số ngày phép năm cũ 
                   var totaldayNew = (collection["totalDay"].ToString() != "") ? float.Parse(collection["totalDay"]) : 0;
                    var annualLeave = db.tblAnnualLeave.Where(x=>x.EmployeeID == employeeID).FirstOrDefault();
                    var newAnnualLeave = annualLeave.AnnualLeave + Qty_AnnualLEave_old - totaldayNew;
                    annualLeave.AnnualLeave = newAnnualLeave;
                    db.Entry(annualLeave).State = EntityState.Modified;
                    db.SaveChanges();


                    //ghi lại lịch sử phép năm
                    tblAnnualLeaveDetail annualLeaveDetail = new tblAnnualLeaveDetail();
                    annualLeaveDetail.EmployeeID = employeeID;
                    annualLeaveDetail.LeaveID = id;
                    annualLeaveDetail.TypeAction = "Trừ nghỉ phép tự động";
                    annualLeaveDetail.QtyAnnualLeave = totaldayNew;
                    annualLeaveDetail.QtyAnnualLeaveFinal = newAnnualLeave;
                    annualLeaveDetail.Detail = collection["Reason"];
                    annualLeaveDetail.isActive = true;
                    annualLeaveDetail.DateCreated = DateTime.Now;
                    db.tblAnnualLeaveDetail.Add(annualLeaveDetail);
                    db.SaveChanges();



                    //delete old data in table detail 
                    var itemDetail = db.tblLeaveDetail.Where(x => x.LeaveID == id).ToList();
                    foreach(var itemdetail  in itemDetail)
                    {
                        itemdetail.isActive = false;
                        itemdetail.DateUpdated = DateTime.Now;
                        itemdetail.UserUpdated = Encryptor.EncryptString(Session["UserID"].ToString());
                        db.Entry(itemdetail).State = EntityState.Modified;
                        db.SaveChanges();
                    }


                    var item = db.tblLeave.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    item.EmployeeID = (collection["EmployeeID"].ToString() != "") ? int.Parse(collection["EmployeeID"]) : 0;
                    item.TypeLeaveID = (collection["TypeLeaveID"].ToString() != "") ? int.Parse(collection["TypeLeaveID"]) : 0;
                    if (collection["fromDate"].ToString() != "")
                        item.fromDate = DateTime.Parse(collection["fromDate"]);
                    if (collection["toDate"].ToString() != "")
                        item.toDate = DateTime.Parse(collection["toDate"]);

                    item.totalDay = (collection["totalDay"].ToString() != "") ? float.Parse(collection["totalDay"]) : 0;
                    item.totalHours = (collection["totalHours"].ToString() != "") ? float.Parse(collection["totalHours"]) : 0;
                    item.Reason = collection["Reason"];

                    item.DateUpdated = DateTime.Now;
                    item.UserUpdated = Encryptor.EncryptString(Session["UserID"].ToString());


                    //insert new data in table detail
                    //Lấy thời gian làm việc
                    var itemShift = db.tblShiftArrange.Where(x => x.EmployeeID == item.EmployeeID).FirstOrDefault();
                    if (itemShift != null)
                    {
                        var shift = db.tblWorkingShift.Where(x => x.ID == itemShift.WorkingShiftID).FirstOrDefault();
                        if (shift != null)
                        {
                            DateTime fromDateLeave = DateTime.Parse(item.fromDate.ToString());
                            DateTime toDateLeave = DateTime.Parse(item.toDate.ToString());
                            TimeSpan totimeKeeper = TimeSpan.Parse(shift.toTimePM.ToString());
                            insertLeaveDetail(id, fromDateLeave, toDateLeave, totimeKeeper);
                        }
                    }

                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["update"] = "Cập nhật thành công";
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 8, item.EmployeeID.ToString());

                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            else return RedirectToAction("Login", "Home");
        }



        [HttpGet]
        public ActionResult Create()
        {
            if (Session["UserID"] != null)
            {
                getViewBagforCreate();
                return View();
            }
            else return RedirectToAction("Login", "Home");
        }

        // POST: Position/Create
        [HttpPost]
        public ActionResult Create(tblLeave item)
        {
            if (Session["UserID"] != null)
            {
                try
                {
                    using (HRManagerEntities db = new HRManagerEntities())
                    {
                        string Reason = item.Reason.ToString();
                        // TODO: Add insert logic here
                        item.isActive = true;
                        item.UserCreated = Encryptor.EncryptString(Session["UserID"].ToString());
                        item.DateCreated = DateTime.Now;
                        
                        db.tblLeave.Add(item);
                        getViewBagforCreate();
                        db.SaveChanges();

                        int LeaveID = item.ID;

                        //Lấy thời gian làm việc
                        var itemShift = db.tblShiftArrange.Where(x => x.EmployeeID == item.EmployeeID).FirstOrDefault();
                        if (itemShift != null)
                        {
                            var shift = db.tblWorkingShift.Where(x => x.ID == itemShift.WorkingShiftID).FirstOrDefault();
                            if (shift != null)
                            {
                                DateTime fromDateLeave = DateTime.Parse(item.fromDate.ToString());
                                DateTime toDateLeave = DateTime.Parse(item.toDate.ToString());
                                TimeSpan totimeKeeper = TimeSpan.Parse(shift.toTimePM.ToString());
                                insertLeaveDetail(LeaveID, fromDateLeave, toDateLeave, totimeKeeper);
                            }
                        }


                        //nếu nghỉ phép năm , insert vào detail phép năm
                        if(item.TypeLeaveID == 1)
                        {
                            //lấy thông tin phép năm
                            var annualeave = db.tblAnnualLeave.Where(x => x.EmployeeID == item.EmployeeID).FirstOrDefault();


                            //trừ phép năm dựa theo số ngày nghỉ
                            var newAnnualLeave = annualeave.AnnualLeave - item.totalDay;

                            //update lại số ngày phép năm mới
                            annualeave.AnnualLeave = newAnnualLeave;
                            annualeave.DateLastUpdate = DateTime.Now;
                            db.Entry(annualeave).State = EntityState.Modified;
                            db.SaveChanges();


                            //ghi lại lịch sử phép năm
                            tblAnnualLeaveDetail annualLeaveDetail = new tblAnnualLeaveDetail();
                            annualLeaveDetail.EmployeeID = item.EmployeeID;
                            annualLeaveDetail.LeaveID = LeaveID;
                            annualLeaveDetail.TypeAction = "Trừ nghỉ phép tự động";
                            annualLeaveDetail.QtyAnnualLeave = item.totalDay;
                            annualLeaveDetail.QtyAnnualLeaveFinal = newAnnualLeave;
                            annualLeaveDetail.Detail = Reason;
                            annualLeaveDetail.isActive = true;
                            annualLeaveDetail.DateCreated = DateTime.Now;
                            db.tblAnnualLeaveDetail.Add(annualLeaveDetail);
                            db.SaveChanges();
                        }

                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 7, item.EmployeeID.ToString());
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



        public void insertLeaveDetail(int ID, DateTime fromdate, DateTime todate, TimeSpan timeoutPM)
        {
            tblLeaveDetail detail = new tblLeaveDetail();
            detail.LeaveID = ID;
            for (DateTime currentDate = fromdate; currentDate <= todate; currentDate = currentDate.AddDays(1))
            {
                //nếu là chủ nhật thì không tính
                if (currentDate.DayOfWeek == DayOfWeek.Sunday) continue;
                detail.LeaveDate = currentDate;
                if (currentDate < todate)
                {
                    detail.LeaveQtyHours = 8;
                }
                else
                {
                    //kiểm tra xem nghỉ nữa ngày hay 1 ngày
                    //Số giờ nghỉ phép trong ngày
                    var leaveTimeSpan = todate.TimeOfDay;
                    var leaveTime = Math.Round( 8 - (timeoutPM - leaveTimeSpan).TotalHours ,0);
                    detail.LeaveQtyHours = leaveTime;
                }
                detail.isActive = true;
                db.tblLeaveDetail.Add(detail);
                db.SaveChanges();
            }
            
        }



        #region Set ViewBag
        public void SetViewBagForDetails(tblLeave item)
        {
            ViewBag.fromDate = DateTime.Parse(item.fromDate.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
            ViewBag.toDate = DateTime.Parse(item.toDate.ToString()).ToString("yyyy-MM-dd HH:mm:ss");

            ViewBag.EmployeeList = new SelectList(db.tblEmployee.ToList(), "ID", "EmployeeCode", item.EmployeeID);
            ViewBag.TypeLeave = new SelectList(db.tblTypeLeave.ToList(), "ID", "TypeLeaveVN", item.TypeLeaveID);

            var employee = db.tblEmployee.Where(x => x.ID == item.EmployeeID).FirstOrDefault();
            ViewBag.Fullname = employee.FirstName + " " + employee.FullName;

            ViewBag.annualleave = db.tblAnnualLeave.Where(x => x.EmployeeID == item.EmployeeID).FirstOrDefault().AnnualLeave;


            ViewBag.DepartmentName = db.tblDepartment.Where(x => x.ID == employee.DepartmentID).FirstOrDefault().NameDepartmentVN;
            ViewBag.PositionName = db.tblPosition.Where(x => x.ID == employee.PositionID).FirstOrDefault().PositionNameVN;
        }

        public void getViewBagforCreate()
        {
            ViewBag.typeLeave = new SelectList(db.tblTypeLeave.ToList(), "ID", "TypeLeaveVN");
            ViewBag.EmployeeList = new SelectList(db.tblEmployee.ToList(), "ID", "EmployeeCode");
            ViewBag.TypeLeave = new SelectList(db.tblTypeLeave.ToList(), "ID", "TypeLeaveVN");
        }

        #endregion
        [HttpPost]
        public ActionResult DeleteLeave(int id)
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                // TODO: Add update logic here
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var item = db.tblLeave.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    item.isActive = false;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["delete"] = "Xóa thành công";
                    return RedirectToAction("Index");
                }
            }
            else return RedirectToAction("Login", "Home");
        }
    }
}