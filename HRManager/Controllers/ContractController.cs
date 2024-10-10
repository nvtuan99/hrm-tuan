using HRManager.Common;
using HRManager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class ContractController : Controller
    {
        // GET: Contract
        private HRManagerEntities db = new HRManagerEntities();
        int typeLog = 60;
        ActionLog action = new ActionLog();
        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                List<tblEmployee> employees = db.tblEmployee.Where(x => x.isActive == true).ToList();
                List<tblDepartment> departments = db.tblDepartment.ToList();
                List<tblContract> contracts = db.tblContract.ToList();
                List<tblTypeContract> typecontracts = db.tblTypeContract.ToList();
                List<tblPosition> positions = db.tblPosition.Where(x => x.isActive == true).ToList();

                var ItemContract = from c in contracts
                                   join tc in typecontracts on c.TypeContractID equals tc.ID into table1
                                   from tc in table1.ToList()
                                   join e in employees on c.EmployeeID equals e.ID into table2
                                   from e in table2.ToList()
                                   join d in departments on e.DepartmentID equals d.ID into table3
                                   from d in table3.ToList()
                                   join p in positions on e.PositionID equals p.ID into table4
                                   from p in table4.ToList()
                                   orderby c.ID descending
                                   select new ViewModel
                                   {
                                       employee = e,
                                       department = d,
                                       Contract = c,
                                       typeContract = tc,
                                       position = p
                                   };
                action.InsertActionLog(Session["UserName"].ToString(), typeLog, 6, "");
                return View(ItemContract);
            }
            else return RedirectToAction("Login", "Home");
        }

        [HttpGet]
        // GET: HistoryWork/Create
        public ActionResult Create()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                SetViewBagForCreate();
                return View();
            }
            else return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection collection, HttpPostedFileBase FileContract)
        {
            if (Session["UserID"] != null)
            {
                try
                {
                    // TODO: Add update logic here
                    using (HRManagerEntities db = new HRManagerEntities())
                    {
                        tblContract item = new tblContract();
                        item.ContractCode = collection["ContractCode"];
                        item.CompanyID = (collection["CompanyID"].ToString() != "") ? int.Parse(collection["CompanyID"].ToString()) : 1;
                        item.EmployeeID = (collection["EmployeeID"].ToString() != "") ? int.Parse(collection["EmployeeID"].ToString()) : 1;
                        item.TypeContractID = (collection["TypeContractID"].ToString() != "") ? int.Parse(collection["TypeContractID"].ToString()) : 1;
                        if (collection["DateSign"].ToString() != "")
                            item.DateSign = DateTime.Parse(collection["DateSign"].ToString());

                        if (collection["DateStart"].ToString() != "")
                            item.DateStart = DateTime.Parse(collection["DateStart"].ToString());

                        if (collection["DateEnd"].ToString() != "")
                            item.DateEnd = DateTime.Parse(collection["DateEnd"].ToString());

                        item.Salary = (collection["Salary"].ToString() != "") ? int.Parse(collection["Salary"].ToString()) : 0;
                        item.Allowance = (collection["Allowance"].ToString() != "") ? int.Parse(collection["Allowance"].ToString()) : 0;
                        item.PayForms = collection["PayForms"];
                        item.PayDate = (collection["PayDate"].ToString() != "") ? int.Parse(collection["PayDate"].ToString()) : 5;
                        item.WorkingTimeAM = collection["WorkingTimeAM"];
                        item.WorkingTimePM = collection["WorkingTimePM"];
                        item.ContractPlace = collection["ContractPlace"];
                        item.Note = collection["Note"];
                        if (FileContract != null && FileContract.ContentLength > 0)
                        {
                            string filename = Path.GetFileName(FileContract.FileName);
                            string path = Path.Combine(Server.MapPath("~/Uploads/Doc/Contract/"), filename);
                            FileContract.SaveAs(path);
                            item.FileContract = filename;
                        }

                        item.DateCreated = DateTime.Now;
                        item.UserCreated = Encryptor.EncryptString(Session["UserID"].ToString());

                        SetViewBagForCreate();
                        item.isActive = true;
                        db.tblContract.Add(item);
                        db.SaveChanges();
                        TempData["insert"] = "Thêm mới thành công";
                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 7, item.ContractCode);
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception exception)
                {
                    string tmp = exception.Message.ToString();
                    return RedirectToAction("Index");
                }
            }
            else return RedirectToAction("Login", "Home");
        }


        // GET: Reward/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["UserID"] != null)
                    {
                        var contract = db.tblContract.Where(x => x.ID == id).FirstOrDefault();
                        SetViewBagForDetails(contract);

                        action.InsertActionLog(Session["UserName"].ToString(), typeLog, 6, contract.ContractCode);

                        return View(contract);
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
        public ActionResult Details(int id, FormCollection collection, HttpPostedFileBase FileContract)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserID"] != null)
                {
                    var item = db.tblContract.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    item.CompanyID = (collection["CompanyID"] == null ) ? 0 : int.Parse(collection["CompanyID"]);
                    item.EmployeeID = (collection["EmployeeID"] == null ) ? 0 : int.Parse(collection["EmployeeID"]);
                    item.ContractCode = collection["ContractCode"].ToString();
                    item.TypeContractID = (collection["TypeContractID"] == null) ? 0 : int.Parse(collection["TypeContractID"]);
                    if (collection["DateSign"].ToString() != "" || collection["DateSign"] != null)
                        item.DateSign = DateTime.Parse(collection["DateSign"]);

                    if (collection["DateStart"].ToString() != "" || collection["DateStart"] != null)
                        item.DateStart = DateTime.Parse(collection["DateStart"]);

                    if (collection["DateEnd"].ToString() != "" || collection["DateEnd"] != null)
                        item.DateEnd = DateTime.Parse(collection["DateEnd"]);
                    item.Salary = (collection["Salary"] == null) ? 0 : int.Parse(collection["Salary"]);
                    item.Allowance = (collection["Allowance"] == null) ? 0 : int.Parse(collection["Allowance"]);
                    item.PayForms = collection["PayForms"].ToString();
                    item.PayDate = (collection["PayDate"] == null) ? 5 : int.Parse(collection["PayDate"]);
                    item.WorkingTimeAM = collection["WorkingTimeAM"].ToString();
                    item.WorkingTimePM = collection["WorkingTimePM"].ToString();
                    item.ContractPlace = collection["ContractPlace"].ToString();
                    item.Note = collection["Note"].ToString();

                    if (FileContract != null && FileContract.ContentLength > 0)
                    {
                        string filename = Path.GetFileName(FileContract.FileName);
                        string path = Path.Combine(Server.MapPath("~/Uploads/Doc/Contract/"), filename);
                        FileContract.SaveAs(path);
                        item.FileContract = filename;
                    }
                    item.isActive = true;
                    item.UserUpdated = Encryptor.EncryptString(Session["UserID"].ToString());
                    item.DateUpdated = DateTime.Now;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["update"] = "Cập nhật thành công";
                    action.InsertActionLog(Session["UserName"].ToString(), typeLog, 8, item.ContractCode);

                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            else return RedirectToAction("Login", "Home");
        }


        #region Set ViewBag
        public void SetViewBagForCreate()
        {
            ViewBag.CompanyList = new SelectList(db.tblCompany.ToList(), "ID", "NameCompanyVN");
            ViewBag.EmployeeList = new SelectList(db.tblEmployee.ToList(), "ID", "EmployeeCode");
            ViewBag.TypeContract = new SelectList(db.tblTypeContract.ToList(), "ID", "NameContractVN");
        }

        public void SetViewBagForDetails(tblContract contract)
        {
            ViewBag.CompanyList = new SelectList(db.tblCompany.ToList(), "ID", "NameCompanyVN", contract.CompanyID);
            ViewBag.EmployeeList = new SelectList(db.tblEmployee.ToList(), "ID", "EmployeeCode", contract.EmployeeID);
            ViewBag.TypeContract = new SelectList(db.tblTypeContract.ToList(), "ID", "NameContractVN", contract.TypeContractID);
            
            ViewBag.CompanyPhone = db.tblCompany.Where(x => x.ID == contract.CompanyID).FirstOrDefault().Phone;
            ViewBag.CompanyTax = db.tblCompany.Where(x => x.ID == contract.CompanyID).FirstOrDefault().Tax;
            ViewBag.CompanyAddress = db.tblCompany.Where(x => x.ID == contract.CompanyID).FirstOrDefault().Address;
            ViewBag.CompanyDeputy = db.tblCompany.Where(x => x.ID == contract.CompanyID).FirstOrDefault().Deputy;
            ViewBag.CompanyTitle = db.tblCompany.Where(x => x.ID == contract.CompanyID).FirstOrDefault().Title;

            var employee = db.tblEmployee.Where(x => x.ID == contract.EmployeeID).FirstOrDefault();
            ViewBag.Fullname = employee.FirstName + " " + employee.FullName;

            ViewBag.DepartmentName = db.tblDepartment.Where(x => x.ID == employee.DepartmentID).FirstOrDefault().NameDepartmentVN;
            ViewBag.PositionName = db.tblPosition.Where(x => x.ID == employee.PositionID).FirstOrDefault().PositionNameVN;
        }

        #endregion

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

    }
}