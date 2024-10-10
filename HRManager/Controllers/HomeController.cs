using HRManager.Common;
using HRManager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;
using System.Web.Security;
using ActionLog = HRManager.Common.ActionLog;

namespace HRManager.Controllers
{
    public class HomeController : Controller
    {
        ActionLog action = new ActionLog();
        HRManagerEntities db = new HRManagerEntities();
        LoadData loaddb = new LoadData();
        setRole setRoleDetails = new setRole();

        public ActionResult Login()
        {
            if (Session["lang"] == null)
                Session["lang"] = "vn";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserProfile objUser)
        {
            if (ModelState.IsValid)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    string username = objUser.UserName;
                    string password = Encryptor.EncryptString(objUser.Password);

                    //check account
                    var checkusername = checkAccount(username);
                    if (username == "" || objUser.Password == null)
                    {
                        ModelState.AddModelError("", "Vui lòng nhập Tài khoản hoặc mật khẩu !");
                    }
                    else if (checkusername == 0)
                    {
                        ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng !");
                    }
                    else // neu tài khoản tồn tại
                    {
                        if (checkusername == 1) // tài khoản đã bị khóa
                        {
                            ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa , vui lòng liên hệ bộ phận quản lý !");
                        }
                        else
                        {
                            var obj = db.UserProfile.Where(a => a.UserName.Equals(username) && a.Password.Equals(password)).FirstOrDefault();
                            if (obj == null) // sai mật khẩu
                            {
                                //ghi lại lịch sử đăng nhập thất bại
                                obj = db.UserProfile.Where(a => a.UserName.Equals(username)).FirstOrDefault();
                                int countLoginFailed = getCountLoginFail(username);
                                obj.CountLoginFailed = countLoginFailed + 1;
                                if (countLoginFailed >= 2) // đã nhập sai 3 lần  - khóa account
                                {
                                    obj.AccountStatus = 1;
                                    //reset counter login failed
                                    obj.CountLoginFailed = 0;
                                }

                                db.Entry(obj).State = EntityState.Modified;
                                db.SaveChanges();
                                ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng , Bạn đã nhập sai " + (countLoginFailed + 1) + " lần !");
                            }
                            else //tài khoản và mật khẩu hợp lệ
                            {
                                //kiểm tra trạng thái tài khoản xem có bị khóa không
                                if (obj.AccountStatus == 1) // Account đã bị khóa
                                {
                                    ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa , vui lòng liên hệ bộ phận quản lý !");
                                }
                                else // login thành công
                                {

                                    //Ghi lại lịch sử login
                                    obj.IPLastLogin = Common.userInfo.GetLocalIPAddress();
                                    obj.DateLastLogin = DateTime.Now;
                                    db.Entry(obj).State = EntityState.Modified;
                                    db.SaveChanges();

                                    //Ghi lại session
                                    Session["UserID"] = obj.UserId.ToString();
                                    Session["UserName"] = obj.UserName.ToString();
                                    Session["EmployeeID"] = obj.EmployeeID.ToString();
                                    Session["Password"] = password;

                                    //set Role left menu
                                    var userid = db.UserProfile.Where(x => x.UserName == obj.UserName.ToString()).FirstOrDefault();
                                    var role = setRoleDetails.loadRoleDetail(userid.EmployeeID);
                                    setRole.setRoleMenu(role);

                                    //insert action log
                                    action.InsertActionLog(obj.UserName.ToString(), 2, 2, "");

                                    //send email
                                    //emailConfig.sendEmail("haphutoan@gmail.com","haphutoanit@gmail.com","login","admin login sucess!");

                                    if (userid.isManagement == true)
                                    {
                                        Session["isManagerment"] = "Managerment";
                                        userInfo.isManager = true;
                                        return RedirectToAction("ManagermentDashBoard");
                                    }
                                    else
                                    {
                                        Session["isManagerment"] = "Employee";
                                        var employeeID = userid.EmployeeID;
                                        userInfo.isManager = false;
                                        userInfo.employeeID = employeeID;
                                        return RedirectToAction("UserDashBoard");
                                    }

                                }
                            }
                        }

                    }
                }
            }
            return View(objUser);
        }




        public ActionResult UserDashBoard()
        {
            if (Session["UserID"] != null)
            {
                getViewBag();
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult ManagermentDashBoard()
        {
            if (Session["UserID"] != null)
            {
                getViewBag();
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        

        public ActionResult LogOut()
        {
            action.InsertActionLog(Session["UserName"].ToString(), 2, 3, "");
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Login");
        }

        public ActionResult ChangeLanguage(string lang)
        {
            Session["lang"] = lang;
            string returnUrl = Request.UrlReferrer?.AbsolutePath;

            if (returnUrl == null)
            {
                // Default to a specific page (e.g., Home/Index) if no return URL is available
                returnUrl = Url.Action("Index", "Home");
            }

            return Redirect($"{returnUrl}?language={lang}");
        }
        [HttpPost]
        public ActionResult ChangePasswordAction(string oldPassword, string newPassword)
        {
            // Kiểm tra mật khẩu cũ và thực hiện logic thay đổi mật khẩu tại đây

            // Ví dụ kiểm tra mật khẩu cũ và lưu mật khẩu mới vào session:
            var sessionPassword = (string)Session["Password"];
            if (Encryptor.EncryptString(oldPassword) != sessionPassword)
            {
                return Json(new { success = false, message = "Mật khẩu cũ không đúng." });
            }
            newPassword = Encryptor.EncryptString(newPassword);
            Session["Password"] = newPassword;
            int userID = int.Parse(Session["UserID"].ToString());
            // Lưu mật khẩu mới vào session
            var users = db.UserProfile.Where(x => x.UserId.Equals(userID)).FirstOrDefault();
            if (users != null)
            {
                users.Password = newPassword;
                db.SaveChanges();
                action.InsertActionLog(Session["UserName"].ToString(), 2, 4, "");
            }
            return Json(new { success = true, message = "Mật khẩu đã được thay đổi thành công , vui lòng đăng nhập lại ." });
        }

        public void getViewBag()
        {
            var year = int.Parse(DateTime.Now.ToString("yyyy"));
            var month = int.Parse(DateTime.Now.ToString("MM"));
            //Total Employee
            var totalEmployee = db.tblEmployee.Where(x => x.isActive == true && x.StatusWork == 1).ToList();
            ViewBag.TotalEmployee = totalEmployee.Count;


            //Total birthday in month
            DateTime? currentDateNullable = DateTime.Now;
            DateTime currentDate = currentDateNullable.Value;
            int currentMonth = currentDate.Month;
            int nextmonth = DateTime.Now.AddMonths(1).Month;
            int birthdayCount = 0;
            if (currentDateNullable.HasValue)
            {
                birthdayCount = db.tblEmployee
                    .Where(employee => employee.Birthday.HasValue && employee.Birthday.Value.Month == currentMonth)
                    .Count();
            }
            ViewBag.BirthdayInMonth = birthdayCount;



            //Total contract expire
            int contractExpireInMonth = 0;
            int contractExpireNextMonth = 0;
            if (currentDateNullable.HasValue)
            {

                contractExpireInMonth = db.tblContract
                    .Where(c => c.DateEnd.HasValue && c.DateEnd.Value.Month == currentMonth)
                    .Count();

                contractExpireNextMonth = db.tblContract
                .Where(c => c.DateEnd.HasValue && c.DateEnd.Value.Month == nextmonth)
                .Count();
            }
            ViewBag.ContractExpire = contractExpireInMonth;
            ViewBag.ContractExpireNextMonth = contractExpireNextMonth;

            DateTime time = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00");



            //Số lượng nghỉ phép trong ngày
            var LeaveOnDay = db.tblLeaveDetail.Where(x => x.LeaveDate == time).Count();
            ViewBag.TotalCountLeaveOnDay = LeaveOnDay;



            //số lượng nhân viên đã xếp ca trong tháng
            var EmployeeShiftArrange = db.tblShiftArrange.Where(x => x.Year == year && x.Month == month).Count();
            string text = EmployeeShiftArrange.ToString() + " / " + totalEmployee.Count.ToString();
            ViewBag.EmployeeShift = text;
        }

        public int checkAccount(string Username)
        {
            int result = 0;
            var check = db.UserProfile.Where(x => x.UserName == Username && x.IsActive == true ).FirstOrDefault();
            if (check != null)
            {
                result = (check.AccountStatus == 1) ? 1 : 2;
            }
            return result;
        }
        public bool checkPassword(string username, string password)
        {
            bool result = false;
            var check = db.UserProfile.Where(x => x.UserName == username && x.Password == password).FirstOrDefault();
            if (check != null)
            {
                result = true;
            }
            return result;
        }

        public int getCountLoginFail(string username)
        {
            string countS = db.UserProfile.Where(x => x.UserName == username).FirstOrDefault().CountLoginFailed.ToString();
            int count = (countS == "") ? 0 : int.Parse(countS);
            return count;
        }
    }
}