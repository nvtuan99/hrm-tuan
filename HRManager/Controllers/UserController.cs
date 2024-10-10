using HRManager.Common;
using HRManager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class UserController : Controller
    {
        private HRManagerEntities db = new HRManagerEntities();
        int typeLog = 4;
        ActionLog action = new ActionLog();
        setRole setRole = new setRole();
        // GET: User
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    List<tblEmployee> employees = db.tblEmployee.Where(x => x.isActive == true).ToList();
                    List<tblDepartment> departments = db.tblDepartment.Where(x => x.isActive == true).ToList();
                    List<tblPosition> positions = db.tblPosition.Where(x => x.isActive == true).ToList();
                    List<UserProfile> users = db.UserProfile.Where(x => x.IsActive == true).ToList();

                    var item = from e in employees
                               join d in users on e.ID equals d.EmployeeID into table1
                               from d in table1.ToList()
                               join dept in departments on e.DepartmentID equals dept.ID into table2
                               from dept in table2.ToList()
                               join pos in positions on e.PositionID equals pos.ID into table3
                               from pos in table3.ToList()

                               orderby e.ID descending
                               select new ViewModel
                               {
                                   employee = e,
                                   userProfile = d,
                                   position = pos,
                                   department = dept
                               };
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 6, "");
                    return View(item);
                }
            }
            else return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public ActionResult Index(int id)
        {
            return View();
        }


        [HttpGet]
        public ActionResult Details(int id)
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    //get info Account
                    var item = db.UserProfile.Where(a => a.UserId.Equals(id)).FirstOrDefault();
                    getViewBagDetail(item);

                    //get info RoleDetails
                    var roleDetails = setRole.loadRoleDetail(item.EmployeeID);
                    ViewBag.RoleDetails = roleDetails;

                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 11, item.UserName);

                    return View(item);
                }

            }
            else return RedirectToAction("Login", "Home");
        }


        [HttpPost]
        public ActionResult Details(int id, FormCollection collection, HttpPostedFileBase FileReward)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserID"] != null)
                {
                    try
                    {
                        // TODO: Add update logic here
                        using (HRManagerEntities db = new HRManagerEntities())
                        {
                            var item = db.UserProfile.Where(a => a.UserId.Equals(id)).FirstOrDefault();
                            item.UserName = collection["UserName"];
                            item.EmployeeID = (collection["EmployeeID"] != null) ? int.Parse(collection["EmployeeID"].ToString()) : 0;

                            if (collection["isActive"].ToString() == "1")
                            {
                                item.IsActive = true;
                            }
                            else
                            {
                                item.IsActive = false;
                            }

                            if(collection["isManagement"] != null)
                            {
                                item.isManagement = true;
                            } else item.isManagement = false;
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                            action.InsertActionLog(Session["UserName"].ToString(), typeLog, 8, item.UserName);
                            TempData["update"] = "Cập nhật thành công";
                            return RedirectToAction("Index");
                        }
                    }
                    catch
                    {
                        return View();
                    }
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
                getViewBagCreate();
                return View();
            }
            else return RedirectToAction("Login", "Home");
        }

        // POST: Position/Create
        [HttpPost]
        public ActionResult Create(UserProfile item, FormCollection collection)
        {
            if (Session["UserID"] != null)
            {
                try
                {
                    using (HRManagerEntities db = new HRManagerEntities())
                    {
                        // TODO: Add insert logic here
                        item.IsActive = true;
                        item.Password = Encryptor.EncryptString(item.Password);
                        item.DateCreate = DateTime.Now;
                        item.UserCreate = Session["UserName"].ToString();
                        if(collection["isManagement"] != null)
                        {
                            item.isManagement = true;
                        }
                        getViewBagCreate();
                        db.UserProfile.Add(item);
                        db.SaveChanges();
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 7, item.UserName);
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


        [HttpPost]
        public ActionResult ChangePassword(int userId, string newPass)
        {
            // Thực hiện kiểm tra và thay đổi mật khẩu tại đây
            // ...
            var user = db.UserProfile.Where(x => x.UserId.Equals(userId)).FirstOrDefault();
            if (user != null)
            {
                user.Password = Encryptor.EncryptString(newPass);
                db.SaveChanges();
                return Json(new { success = true, message = "Mật khẩu đã được thay đổi thành công." });
            }
            else
            {
                return Json(new { success = false, message = "Mật khẩu không được thay đổi." });
            }

        }

        public void getViewBagDetail(UserProfile item)
        {
            ViewBag.EmployeeList = new SelectList(db.tblEmployee.ToList(), "ID", "EmployeeCode", item.EmployeeID);
            var employee = db.tblEmployee.Where(x => x.ID == item.EmployeeID).FirstOrDefault();
            if (employee != null)
            {
                ViewBag.Fullname = employee.FullName;
            }

            var department = db.tblDepartment.Where(x => x.ID == employee.DepartmentID).FirstOrDefault();
            if (department != null)
            {
                ViewBag.Department = department.NameDepartmentVN;
            }

            ViewBag.RoleList = new SelectList(db.tblRoleCategory.ToList(), "ID", "CategoryNameVN");

            List<tblRole> roles = db.tblRole.Where(x => x.isActive == true).ToList();
            List<tblRoleCategory> roleCategories = db.tblRoleCategory.ToList();
            List<tblEmployee> employees = db.tblEmployee.ToList();

            //var roledetail = from rc in roleCategories
            //                 join r in roles on rc.ID equals r.CategoryRole into table1
            //                 from r in table1.ToList()
            //                 join e in employees on r.ID equals employee.ID into table2
            //                 from e in table2.ToList()
            //                 orderby e.ID descending
            //                 select new ViewModel
            //                 {
            //                     employee = e,
            //                     role = r,
            //                     roleCategory = rc
            //                 };
            //ViewBag.RoleDetail = roledetail;
        }

        public void getViewBagCreate()
        {
            ViewBag.EmployeeList = new SelectList(db.tblEmployee.ToList(), "ID", "EmployeeCode");
            
        }
        [HttpPost]
        public ActionResult UpdateRole(int employeeId, int categoryId, string role, int isChecked)
        {
            var item = db.tblRole.Where(x => x.EmployeeID == employeeId && x.CategoryRole == categoryId).FirstOrDefault();
            
            if(item == null) // insert new role
            {
                tblRole newRole = new tblRole();
                newRole.EmployeeID = employeeId;
                newRole.CategoryRole = categoryId;
                if (role == "roleView")
                {
                    newRole.RoleView = true;
                    newRole.RoleDelete = false;
                    newRole.RoleUpdate = false;
                } else if (role == "roleUpdate")
                {
                    newRole.RoleView = false;
                    newRole.RoleDelete = false;
                    newRole.RoleUpdate = true;
                }
                else if (role == "roleDelete")
                {
                    newRole.RoleView = false;
                    newRole.RoleDelete = true;
                    newRole.RoleUpdate = false;
                }
                db.tblRole.Add(newRole);
                db.SaveChanges();
            } 
            else // update
            {
                if (role == "roleView")
                {
                    item.RoleView = !item.RoleView;
                }
                else if (role == "roleUpdate")
                {
                    item.RoleUpdate = !item.RoleUpdate;
                }
                else if (role == "roleDelete")
                {
                    item.RoleDelete = !item.RoleDelete;
                }
                db.SaveChanges();
            }
            return Json(new { success = true }); // Return a JSON response indicating success
        }


        [HttpPost]
        public ActionResult UnlockAccount(int id)
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                // TODO: Add update logic here
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var item = db.UserProfile.Where(a => a.UserId == id).FirstOrDefault();
                    item.AccountStatus = 0;
                    item.DateUpdate = DateTime.Now;
                    item.UserUpdate = Encryptor.EncryptString(Session["UserID"].ToString());
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 13, item.UserName);
                    TempData["update"] = "Cập nhật thành công";
                    return RedirectToAction("Index");
                }
            }
            else return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public ActionResult LockAccount(int id)
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                // TODO: Add update logic here
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var item = db.UserProfile.Where(a => a.UserId == id).FirstOrDefault();
                    item.AccountStatus = 1;
                    item.DateUpdate = DateTime.Now;
                    item.UserUpdate = Encryptor.EncryptString(Session["UserID"].ToString());
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 13, item.UserName);
                    TempData["update"] = "Cập nhật thành công";
                    return RedirectToAction("Index");
                }
            }
            else return RedirectToAction("Login", "Home");
        }
    }
}