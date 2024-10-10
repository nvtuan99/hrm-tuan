using HRManager.Common;
using HRManager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace HRManager.Controllers
{
    public class RewardController : Controller
    {
        // GET: Reward
        ActionLog action = new ActionLog();
        HRManagerEntities db = new HRManagerEntities();
        LoadData loaddb = new LoadData();
        int typeLog = 16;
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                List<tblEmployee> employees = db.tblEmployee.Where(x => x.isActive == true).ToList();
                List<tblDepartment> departments = db.tblDepartment.ToList();
                List<tblPosition> positions = db.tblPosition.ToList();
                List<tblReward> rewards = db.tblReward.ToList().Where(x => x.isActive == true).ToList(); 
                List<tblTypeReward> typerewards = db.tblTypeReward.ToList();
                var employeeRecord = from e in employees
                                     join d in departments on e.DepartmentID equals d.ID into table1
                                     from d in table1.ToList()
                                     join i in rewards on e.ID equals i.EmployeeID into table2
                                     from i in table2.ToList()

                                     join t in typerewards on i.TypeReward equals t.ID into table3
                                     from t in table3.ToList()
                                     join p in positions on e.PositionID equals p.ID into table4
                                     from p in table4.ToList()

                                     orderby e.ID descending
                                     select new ViewModel
                                     {
                                         employee = e,
                                         department = d,
                                         reward = i,
                                         typeReward = t,
                                         position = p,
                                     };
                action.InsertActionLog(Session["UserName"].ToString(), typeLog, 6, "");
                return View(employeeRecord);
            }
            else return RedirectToAction("Login", "Home");
        }

        // GET: Reward/Details/5
        public ActionResult Details(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["UserID"] != null)
                    {
                        List<tblEmployee> employees = db.tblEmployee.Where(x => x.isActive == true).ToList();
                        List<tblDepartment> departments = db.tblDepartment.ToList();
                        List<tblReward> rewards = db.tblReward.ToList();
                        List<tblTypeReward> typerewards = db.tblTypeReward.ToList();

                        var rewardDetail = from e in employees
                                           join d in departments on e.DepartmentID equals d.ID into table1
                                           from d in table1.ToList()

                                           join i in rewards on e.ID equals i.EmployeeID into table2
                                           from i in table2.ToList().Where(x => x.ID == id)

                                           join t in typerewards on i.TypeReward equals t.ID into table3
                                           from t in table3.ToList()

                                           orderby e.ID descending
                                           select new ViewModel
                                           {
                                               employee = e,
                                               department = d,
                                               reward = i,
                                               typeReward = t,
                                           };

                        var rew = db.tblReward.Where(a => a.ID.Equals(id)).FirstOrDefault();
                        getViewBagDetail(rew);


                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 6, rew.NoReward);

                        return View(rewardDetail);
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
        public ActionResult Details(int id, FormCollection collection, HttpPostedFileBase FileReward)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserID"] != null)
                {
                    var item = db.tblReward.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    item.NoReward = collection["NoReward"];
                    item.EmployeeID = (collection["item.reward.EmployeeID"].ToString() != "") ? int.Parse(collection["item.reward.EmployeeID"]) : 0;

                    var emp = db.tblEmployee.Where(a => a.ID == item.EmployeeID).FirstOrDefault();
                    item.EmployeeCode = emp.EmployeeCode;

                    if (collection["item.reward.DateReward"].ToString() != "")
                        item.DateReward = DateTime.Parse(collection["item.reward.DateReward"]);
                    item.TypeReward = (collection["item.reward.TypeReward"].ToString() != "") ? int.Parse(collection["item.reward.TypeReward"]) : 0;
                    item.Reason = collection["Reason"];
                    item.RewardForm = collection["item.reward.RewardForm"];
                    item.Money = int.Parse(collection["Money"]);

                    if (collection["item.reward.ApproveDate"].ToString() != "")
                        item.ApproveDate = DateTime.Parse(collection["item.reward.ApproveDate"]);
                    item.ApproveBy = collection["item.reward.ApproveBy"];

                    if (FileReward != null && FileReward.ContentLength > 0)
                    {
                        string filename = Path.GetFileName(FileReward.FileName);
                        string path = Path.Combine(Server.MapPath("~/Uploads/Doc/DocumentUpload/"), filename);
                        FileReward.SaveAs(path);
                        item.FileReward = filename;
                    }
                    item.UpdateDate = DateTime.Now;
                    item.UpdateBy = Encryptor.EncryptString(Session["UserID"].ToString());
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["update"] = "Cập nhật thành công";
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 8, item.NoReward);

                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            else return RedirectToAction("Login", "Home");
        }



        // GET: Reward/Create
        public ActionResult Create()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                getViewBagforCreate();
                return View();
            }
            else return RedirectToAction("Login", "Home");
        }

        // POST: Reward/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection, HttpPostedFileBase FileReward)
        {
            try
            {
                // TODO: Add insert logic here
                getViewBagforCreate();

                tblReward reward = new tblReward();
                reward.NoReward = collection["NoReward"];
                reward.EmployeeID = (collection["EmployeeCode"].ToString() != "") ? int.Parse(collection["EmployeeCode"]) : 0;
                
                var emp = db.tblEmployee.Where(a => a.ID == reward.EmployeeID).FirstOrDefault();
                reward.EmployeeCode = emp.EmployeeCode;

                if (collection["DateReward"].ToString() != "" || collection["DateReward"] != null)
                    reward.DateReward = DateTime.Parse(collection["DateReward"]);

                reward.TypeReward = (collection["TypeReward"].ToString() != "") ? int.Parse(collection["TypeReward"]) : 0;
                reward.Reason = collection["Reason"];
                reward.RewardForm = collection["RewardForm"];
                reward.Money = (collection["Money"] != null || collection["Money"].ToString() != "") ?int.Parse(collection["Money"]) : 0;
                if (collection["ApproveDate"] == null || collection["ApproveDate"].ToString() == "")
                    reward.ApproveDate = DateTime.Parse(collection["ApproveDate"]);
                reward.ApproveBy = collection["ApproveBy"];

                if (FileReward != null && FileReward.ContentLength > 0)
                {
                    string filename = Path.GetFileName(FileReward.FileName);
                    string path = Path.Combine(Server.MapPath("~/Uploads/Doc/DocumentUpload/"), filename);
                    FileReward.SaveAs(path);
                    reward.FileReward = filename;
                }
                reward.CreateDate = DateTime.Now;
                reward.CreateBy = Encryptor.EncryptString(Session["UserID"].ToString());
                reward.isActive = true;

                db.tblReward.Add(reward);

                db.SaveChanges();
                TempData["insert"] = "Thêm mới thành công";
                action.InsertActionLog(Session["UserName"].ToString(), typeLog, 7, reward.NoReward);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reward/Edit/5
        public ActionResult Edit(int id)
        {

            return View();
        }

        // POST: Reward/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        // GET: Reward/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Reward/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public void getViewBagDetail(tblReward reward)
        {
            ViewBag.TypeReward = new SelectList(db.tblTypeReward.ToList(), "ID", "NameVN", reward.TypeReward);
            ViewBag.EmployeeList = new SelectList(db.tblEmployee.ToList(), "ID", "EmployeeCode", reward.EmployeeID);
            ViewBag.RewardForm = new SelectList(db.tblRewardForm.ToList(), "ID", "RewardFormVN", reward.RewardForm);
            ViewBag.ApproveBy = new SelectList(db.tblEmployee.ToList(), "ID", "Fullname", reward.ApproveBy);
        }


        public void getViewBagforCreate()
        {
            ViewBag.TypeReward = new SelectList(db.tblTypeReward.ToList(), "ID", "NameVN");
            ViewBag.EmployeeList = new SelectList(db.tblEmployee.ToList(), "ID", "EmployeeCode");
            ViewBag.RewardForm = new SelectList(db.tblRewardForm.ToList(), "ID", "RewardFormVN");
            ViewBag.ApproveBy = new SelectList(db.tblEmployee.ToList(), "ID", "Fullname");
        }

        


        //Download file
        public ActionResult DownloadFile(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                string filePath = Path.Combine(Server.MapPath("~/Uploads/Doc/DocumentUpload/"), filename);

                if (System.IO.File.Exists(filePath))
                {
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 10, filename);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return File(fileBytes, "application/octet-stream", filename);
                }
            }

            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult DeleteReward(int id)
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                // TODO: Add update logic here
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var reward = db.tblReward.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    reward.isActive = false;
                    db.Entry(reward).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["delete"] = "Xóa thành công";
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 9, reward.NoReward);
                    return RedirectToAction("Index");
                }
            }
            else return RedirectToAction("Login", "Home");
        }

    }
}
