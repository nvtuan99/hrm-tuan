using ExcelDataReader;
using LinqToExcel;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Http;
using HRManager.Common;
using HRManager.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace HRManager.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee

        HRManagerEntities db = new HRManagerEntities();
        LoadData loaddb = new LoadData();
        ActionLog action = new ActionLog();


        public ActionResult Index()
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                List<tblEmployee> employees = db.tblEmployee.Where(x => x.isActive == true).ToList();
                List<tblDepartment> departments = db.tblDepartment.ToList();
                List<tblPosition> positions = db.tblPosition.ToList();

                var employeeRecord = from e in employees
                                     join d in departments on e.DepartmentID equals d.ID into table1
                                     from d in table1.ToList()
                                     join i in positions on e.PositionID equals i.ID into table2
                                     from i in table2.ToList()
                                     orderby e.ID descending
                                     select new ViewModel
                                     {
                                         employee = e,
                                         department = d,
                                         position = i
                                     };
                action.InsertActionLog(Session["UserName"].ToString(), 6, 6, "");

                setViewBagIndex();
                return View(employeeRecord);

            }
            else return RedirectToAction("Login", "Home");
        }
        [HttpGet]
        public ActionResult EmployeeView(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["UserID"] != null)
                    {
                        var employeeDetail = db.tblEmployee.Where(a => a.ID.Equals(id)).FirstOrDefault();
                        SetViewBag(employeeDetail);
                        action.InsertActionLog(Session["UserName"].ToString(), 8, 11, employeeDetail.EmployeeCode);
                        return View(employeeDetail);
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
        public ActionResult EmployeeView(int id, System.Web.Mvc.FormCollection collection)
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                try
                {
                    // TODO: Add update logic here
                    using (HRManagerEntities db = new HRManagerEntities())
                    {
                        var employeeUpdate = db.tblEmployee.Where(a => a.ID.Equals(id)).FirstOrDefault();
                        //var contractUpdate = db.tblContract.Where(a => a.EmployeeID.Equals(id)).FirstOrDefault();

                        #region tabOne
                        employeeUpdate.EmployeeCode = collection["EmployeeCode"];
                        employeeUpdate.FirstName = collection["FirstName"];
                        employeeUpdate.FullName = collection["FullName"];
                        employeeUpdate.CommonName = collection["CommonName"];
                        employeeUpdate.Gender = (collection["Gender"].ToString() == "1") ? true : false;

                        if (collection["photo"] != null && collection["photo"] != "")
                            employeeUpdate.Images = collection["photo"];


                        if (collection["Birthday"] != null && collection["Birthday"] != "")
                        {
                            employeeUpdate.Birthday = DateTime.Parse(collection["Birthday"]);
                        }
                        employeeUpdate.MaritalID = (collection["MaritalID"] != "") ? int.Parse(collection["MaritalID"]) : 1;
                        employeeUpdate.PlaceOfBirth = collection["PlaceOfBirth"];
                        employeeUpdate.Address = collection["Address"];
                        employeeUpdate.AddressTmp = collection["AddressTmp"];
                        employeeUpdate.Phone = collection["Phone"];
                        employeeUpdate.Email = collection["Email"];
                        employeeUpdate.DateStartWord = (collection["DateStartWord"] != "") ? DateTime.Parse(collection["DateStartWord"]) : DateTime.Now;

                        employeeUpdate.NationalityID = (collection["NationalityID"] != "") ? int.Parse(collection["NationalityID"]) : 1;
                        employeeUpdate.NationID = (collection["NationID"] != "") ? int.Parse(collection["NationID"]) : 1;
                        employeeUpdate.ReligionID = (collection["ReligionID"] != "") ? int.Parse(collection["ReligionID"]) : 1;
                        employeeUpdate.DegreeID = (collection["DegreeID"] != "") ? int.Parse(collection["DegreeID"]) : 1;


                        employeeUpdate.ForeignID = collection["ForeignsID"];
                        employeeUpdate.Authority = (collection["Authority"].ToString().Substring(0, 4) == "true") ? true : false;

                        employeeUpdate.CCCD = collection["CCCD"];
                        employeeUpdate.DateCCCD = (collection["DateCCCD"] != "") ? DateTime.Parse(collection["DateCCCD"]) : DateTime.Now;
                        employeeUpdate.IssuedBy = collection["IssuedBy"];
                        employeeUpdate.Health = collection["Health"];
                        employeeUpdate.Weight = (collection["Weight"] != "") ? int.Parse(collection["Weight"]) : 1;
                        employeeUpdate.Height = (collection["Height"] != "") ? int.Parse(collection["Height"]) : 1;
                        employeeUpdate.StatusWork = (collection["StatusWork"] != "") ? int.Parse(collection["StatusWork"]) : 1;
                        employeeUpdate.Note = collection["Note"];
                        #endregion

                        #region tabTwo
                        employeeUpdate.DepartmentID = (collection["DepartmentID"] != "") ? int.Parse(collection["DepartmentID"]) : 1;
                        employeeUpdate.CompanyID = (collection["CompanyID"] != "") ? int.Parse(collection["CompanyID"]) : 1;
                        employeeUpdate.PositionID = (collection["PositionID"] != "") ? int.Parse(collection["PositionID"]) : 1;

                        employeeUpdate.NoBHXH = collection["NoBHXH"];
                        if (collection["DateBHXH"] != null && collection["DateBHXH"] != "")
                        {
                            employeeUpdate.DateBHXH = DateTime.Parse(collection["DateBHXH"]);
                        }
                        employeeUpdate.IssueByBHXH = collection["IssueByBHXH"];


                        employeeUpdate.NoBHYT = collection["NoBHYT"];
                        if (collection["fromDateBHYT"] != null && collection["fromDateBHYT"] != "")
                        {
                            employeeUpdate.fromDateBHYT = DateTime.Parse(collection["fromDateBHYT"]);
                        }

                        if (collection["toDateBHYT"] != null && collection["toDateBHYT"] != "")
                        {
                            employeeUpdate.toDateBHYT = DateTime.Parse(collection["toDateBHYT"]);
                        }

                        employeeUpdate.HopitalBHYT = collection["HopitalBHYT"];
                        employeeUpdate.ProvinceBHYT = collection["ProvinceBHYT"];


                        employeeUpdate.BHYT = (collection["BHYT"].ToString().Substring(0, 4) == "true") ? true : false;
                        employeeUpdate.BHXH = (collection["BHXH"].ToString().Substring(0, 4) == "true") ? true : false;
                        employeeUpdate.BHTN = (collection["BHTN"].ToString().Substring(0, 4) == "true") ? true : false;
                        employeeUpdate.UnionDues = (collection["UnionDues"].ToString().Substring(0, 4) == "true") ? true : false;

                        #endregion
                        employeeUpdate.DateUpdated = DateTime.Now;
                        employeeUpdate.UserLastUpdated = Encryptor.EncryptString(Session["UserID"].ToString());
                        db.Entry(employeeUpdate).State = EntityState.Modified;
                        db.SaveChanges();
                        action.InsertActionLog(Session["UserName"].ToString(), 7, 8, employeeUpdate.EmployeeCode);
                        TempData["update"] = "Cập nhật thành công";
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

        public ActionResult UploadImage(HttpPostedFileBase imageFile)
        {
            if (imageFile != null && imageFile.ContentLength > 0)
            {
                // Validate the image (optional)

                // Save the image to the server
                var fileName = Path.GetFileName(imageFile.FileName);
                var path = Path.Combine(Server.MapPath("~/Uploads/Images/Employee"), fileName);
                imageFile.SaveAs(path);

                // Perform any other required operations

                // Return a response (e.g., success message)
                return Content("Image uploaded successfully!");
            }

            // Handle the case where no image was selected
            return Content("Please select an image.");
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
        public ActionResult Create(tblEmployee employee)
        {
            if (!ModelState.IsValid)
            {
                if (Session["UserID"] != null)
                {
                    try
                    {
                        // TODO: Add update logic here
                        using (HRManagerEntities db = new HRManagerEntities())
                        {
                            // Check if the employeecode already exists in the database
                            if (db.tblEmployee.Any(e => e.EmployeeCode == employee.EmployeeCode))
                            {
                                // Show an alert to the user indicating that the employee code is already taken
                                ModelState.AddModelError("employee.employeecode", "Employee code already exists in the database.");
                                SetViewBagForCreate();
                                return View(employee);
                            }
                            //List<string> selectedForeignIDs = employee.ForeignsID;
                            //string selectedForeign = "";
                            //foreach (string item in selectedForeignIDs)
                            //{
                            //    selectedForeign += item.ToString() + ",";
                            //}
                            //employee.ForeignID = selectedForeign.Substring(0, selectedForeign.Length - 1);
                            if (employee.MaritalID == null)
                                employee.MaritalID = 1;
                            employee.StatusWork = 1;

                            employee.isActive = true;
                            employee.CreatedByDate = DateTime.Now;
                            employee.CreatedByUser = Encryptor.EncryptString(Session["UserID"].ToString());
                            db.tblEmployee.Add(employee);
                            db.SaveChanges();
                            TempData["insert"] = "Thêm mới thành công";
                        }
                        SetViewBagForCreate();
                        action.InsertActionLog(Session["UserName"].ToString(), 7, 7, "");
                        return RedirectToAction("Index");

                    }
                    catch (Exception exception)
                    {
                        string tmp = exception.Message.ToString();
                        return RedirectToAction("Index");
                    }
                }
                else return RedirectToAction("Login", "Home");
            }
            SetViewBagForCreate();
            return View(employee);
        }


        public JsonResult CheckEmployeeCode(string employeecode)
        {
            using (HRManagerEntities db = new HRManagerEntities())
            {
                bool exists = db.tblEmployee.Any(e => e.EmployeeCode == employeecode && e.isActive == true);
                return Json(new { exists }, JsonRequestBehavior.AllowGet);
            }
        }


        #region Set ViewBag
        public void SetViewBagForCreate()
        {
            //Tình trạng hôn nhân
            List<SelectListItem> items = new List<SelectListItem>();
            items = db.tblMaritalStatus.Select(a => new SelectListItem
            {
                Value = a.ID.ToString(),
                Text = a.MaritalStatusVN,
            }).ToList();
            ViewBag.ABC = items;

            //Quốc tịch
            ViewBag.Nationality = new SelectList(db.tblNationality.ToList(), "ID", "NameNationalityVN");
            //Dân tộc
            ViewBag.Nation = new SelectList(db.tblNation.ToList(), "ID", "NameNationVN");
            //Ngoại ngữ
            ViewBag.ForeignLanguage = new SelectList(db.tblForeignLanguage.ToList(), "ID", "ForeignLanguageVN");
            //Tôn giáo
            ViewBag.Religion = new SelectList(db.tblReligion.ToList(), "ID", "NameReligionVN");
            //Học vấn
            ViewBag.Degree = new SelectList(db.tblDegree.ToList(), "ID", "DegreeVN");
            //Trạng thái làm việc
            ViewBag.WorkStatus = new SelectList(db.tblStatusWork.ToList(), "ID", "StatusWorkVN");

            //Công ty
            ViewBag.CompanyInfor = new SelectList(db.tblCompany.ToList(), "ID", "NameCompanyVN");
            //Phòng ban
            ViewBag.Department = new SelectList(db.tblDepartment.ToList(), "ID", "NameDepartmentVN");
            //Vị trí
            ViewBag.Position = new SelectList(db.tblPosition.ToList(), "ID", "PositionNameVN");

        }

        public void setViewBagIndex()
        {
            //export excel - show data to dropdownlist
            List<SelectListItem> itemsCompany = new List<SelectListItem>();
            itemsCompany = db.tblCompany.Select(a => new SelectListItem
            {
                Value = a.ID.ToString(),
                Text = a.NameCompanyVN,
            }).ToList();
            ViewBag.ExportCompanyList = itemsCompany;


            //export excel - show data to dropdownlist
            List<SelectListItem> itemsDepartment = new List<SelectListItem>();
            itemsDepartment = db.tblDepartment.Select(a => new SelectListItem
            {
                Value = a.ID.ToString(),
                Text = a.NameDepartmentVN,
            }).ToList();
            ViewBag.ExportDepartmentList = itemsDepartment;
        }

        public void SetViewBag(tblEmployee tblEmployee)
        {
            //Tình trạng hôn nhân
            ViewBag.MaritalStatus = new SelectList(db.tblMaritalStatus.ToList(), "ID", "MaritalStatusVN", tblEmployee.MaritalID);
            //Quốc tịch
            ViewBag.Nationality = new SelectList(db.tblNationality.ToList(), "ID", "NameNationalityVN", tblEmployee.NationalityID);
            //Dân tộc
            ViewBag.Nation = new SelectList(db.tblNation.ToList(), "ID", "NameNationVN", tblEmployee.NationID);
            //Ngoại ngữ
            ViewBag.ForeignLanguage = new SelectList(db.tblForeignLanguage.ToList(), "ID", "ForeignLanguageVN", tblEmployee.ForeignID);
            //Tôn giáo
            ViewBag.Religion = new SelectList(db.tblReligion.ToList(), "ID", "NameReligionVN", tblEmployee.ReligionID);
            //Học vấn
            ViewBag.Degree = new SelectList(db.tblDegree.ToList(), "ID", "DegreeVN", tblEmployee.DegreeID);
            //Trạng thái làm việc
            ViewBag.WorkStatus = new SelectList(db.tblStatusWork.ToList(), "ID", "StatusWorkVN", tblEmployee.StatusWork);
            //Công ty
            ViewBag.CompanyInfor = new SelectList(db.tblCompany.ToList(), "ID", "NameCompanyVN", tblEmployee.CompanyID);
            //Phòng ban
            ViewBag.Department = new SelectList(db.tblDepartment.ToList(), "ID", "NameDepartmentVN", tblEmployee.DepartmentID);
            //Vị trí
            ViewBag.Position = new SelectList(db.tblPosition.ToList(), "ID", "PositionNameVN", tblEmployee.PositionID);

            //Tổ trưởng
            ViewBag.LeaderPerson = new SelectList(db.tblEmployee.ToList(), "ID", "FullName", tblEmployee.LeaderID);

            //quản lý
            ViewBag.ManagerPerson = new SelectList(db.tblEmployee.ToList(), "ID", "FullName", tblEmployee.ManagerID);


            ViewBag.Position = new SelectList(db.tblPosition.ToList(), "ID", "PositionNameVN", tblEmployee.PositionID);
            string birthdayString = (tblEmployee.Birthday.ToString() != "") ? DateTime.Parse(tblEmployee.Birthday.ToString()).ToString("yyyy-MM-dd") : "";
            ViewBag.BirthdayString = birthdayString;



            //Contract
            ViewBag.Contract = db.tblContract.Where(a => a.ID.Equals(tblEmployee.ID) && a.TypeContractID == 2).FirstOrDefault();
            ViewBag.ContractOfficial = db.tblContract.Where(a => a.ID.Equals(tblEmployee.ID) && a.TypeContractID == 1).FirstOrDefault();

            //Relationship
            ViewBag.RelaEmployee = db.tblRelationship.Where(a => a.EmployeeID == tblEmployee.ID).ToList();

            //Relationship type
            ViewBag.RelationShipType = new SelectList(db.tblRelationshipType, "RelationshipNameVN", "RelationshipNameVN");

            //Degree
            ViewBag.DegreeAll = new SelectList(db.tblDegree.Where(x => x.isActive == true), "DegreeVN", "DegreeVN");
            //PositionAll
            ViewBag.PositionAll = new SelectList(db.tblPosition.Where(x => x.isActive == true), "PositionNameVN", "PositionNameVN");
        }
        #endregion Set ViewBag



        #region Excel method 
        //Download Template Excel
        public FileResult DownloadExcel()
        {
            string path = "~/Uploads/Doc/template_upload.xlsx";
            return File(path, "application/vnd.ms-excel", "template_upload.xlsx");
        }


        //upload Excel to Database
        [HttpPost]
        public JsonResult UploadExcel1(tblEmployee users, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/Uploads/Doc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
                    var ds = new DataSet();
                    adapter.Fill(ds, "ExcelTable");
                    DataTable dtable = ds.Tables["ExcelTable"];
                    string sheetName = "Sheet1";
                    var excelFile = new ExcelQueryFactory(pathToExcelFile);

                    #region Import
                    //var artistAlbums = from a in excelFile.Worksheet<tblEmployee>(sheetName) select a;
                    var artistAlbums = from a in excelFile.Worksheet<tblEmployee>(sheetName) select a;
                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            if (a.EmployeeCode != "" && a.Address != "" && a.FullName != "" && a.FirstName != "")
                            {
                                tblEmployee TU = new tblEmployee();
                                TU.EmployeeCode = a.EmployeeCode;
                                TU.FirstName = a.FirstName;
                                TU.FullName = a.FullName;
                                TU.CommonName = a.CommonName;
                                TU.Gender = (a.Gender.ToString() == "Nam") ? true : false;
                                TU.Birthday = a.Birthday;
                                TU.PlaceOfBirth = a.PlaceOfBirth;
                                TU.Address = a.Address;
                                TU.AddressTmp = a.AddressTmp;
                                TU.Email = a.Email;
                                TU.CCCD = a.CCCD;
                                TU.DateCCCD = a.DateCCCD;
                                TU.IssuedBy = a.IssuedBy;
                                TU.DateStartWord = a.DateStartWord;
                                TU.Health = a.Health;
                                TU.Height = a.Height;
                                TU.Weight = a.Weight;
                                TU.Note = a.Note;
                                TU.NoBHXH = a.NoBHXH;
                                TU.DateBHXH = a.DateBHXH;
                                TU.IssueByBHXH = a.IssueByBHXH;
                                TU.NoBHYT = a.NoBHYT;
                                TU.fromDateBHYT = a.fromDateBHYT;
                                TU.toDateBHYT = a.toDateBHYT;
                                TU.ProvinceBHYT = a.ProvinceBHYT;
                                TU.HopitalBHYT = a.HopitalBHYT;


                                db.tblEmployee.Add(TU);
                                db.SaveChanges();
                            }
                            else
                            {
                                data.Add("<ul>");
                                if (a.EmployeeCode == "" || a.EmployeeCode == null) data.Add("<li> EmployeeCode is required</li>");
                                if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                                if (a.FullName == "" || a.FullName == null) data.Add("<li>ContactNo is required</li>");
                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }
                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {
                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {
                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                                }
                            }
                        }
                    }

                    #endregion
                    action.InsertActionLog(Session["UserName"].ToString(), 7, 12, "");

                    //deleting excel file from folder
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //alert message for invalid file format
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }




        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    Stream stream = upload.InputStream;
                    IExcelDataReader reader = null;

                    if (upload.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        return View();
                    }

                    DataTable dt = new DataTable();
                    DataRow row;

                    DataTable dt_ = new DataTable();
                    try
                    {
                        dt_ = reader.AsDataSet().Tables[0];

                        for (int i = 0; i < dt_.Columns.Count; i++)
                        {
                            dt.Columns.Add(dt_.Rows[0][i].ToString());
                        }

                        for (int row_ = 1; row_ < dt_.Rows.Count; row_++)
                        {
                            row = dt.NewRow();

                            for (int col = 0; col < dt_.Columns.Count; col++)
                            {
                                row[col] = dt_.Rows[row_][col].ToString();
                            }
                            dt.Rows.Add(row);
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError("File", "Unable to Upload file!");
                        return View();
                    }

                    reader.Close();
                    reader.Dispose();
                    int count = 0;
                    // Import data into the database using Entity Framework
                    foreach (DataRow dataRow in dt.Rows)
                    {
                        count += 1;
                        int nationalityID = (dataRow["NationalityID"].ToString() != "") ? loaddb.loadNationality(dataRow["NationalityID"].ToString()) : 1;
                        int nationID = (dataRow["NationID"].ToString() != "") ? loaddb.loadNation(dataRow["NationID"].ToString()) : 1;
                        int religionID = (dataRow["ReligionID"].ToString() != "") ? loaddb.loadReligion(dataRow["ReligionID"].ToString()) : 1;
                        int degreeID = (dataRow["DegreeID"].ToString() != "") ? loaddb.loadDegree(dataRow["DegreeID"].ToString()) : 1;
                        int maritalID = (dataRow["MaritalID"].ToString() != "") ? loaddb.loadMarital(dataRow["MaritalID"].ToString()) : 1;
                        int depID = (dataRow["DepartmentID"].ToString() != "") ? loaddb.loadDepartment(dataRow["DepartmentID"].ToString()) : 1;
                        int positionID = (dataRow["PositionID"].ToString() != "") ? loaddb.loadPosition(dataRow["PositionID"].ToString()) : 1;
                        int companyID = (dataRow["CompanyID"].ToString() != "") ? loaddb.loadCompany(dataRow["CompanyID"].ToString()) : 1;

                        var employee = new tblEmployee
                        {
                            EmployeeCode = dataRow["EmployeeCode"].ToString(),
                            FirstName = dataRow["FirstName"].ToString(),
                            FullName = dataRow["FullName"].ToString(),
                            Address = dataRow["Address"].ToString(),
                            Gender = (dataRow["Gender"].ToString() == "Nam") ? true : false,
                            Birthday = (dataRow["Birthday"].ToString() != "") ? DateTime.Parse(dataRow["Birthday"].ToString()) : DateTime.Now,
                            PlaceOfBirth = dataRow["PlaceOfBirth"].ToString(),
                            AddressTmp = dataRow["AddressTmp"].ToString(),
                            Phone = dataRow["Phone"].ToString(),
                            Email = dataRow["Email"].ToString(),
                            CCCD = dataRow["CCCD"].ToString(),
                            DateCCCD = (dataRow["DateCCCD"].ToString() != "") ? DateTime.Parse(dataRow["DateCCCD"].ToString()) : DateTime.Now,
                            IssuedBy = dataRow["IssuedBy"].ToString(),
                            DateStartWord = (dataRow["DateStartWord"].ToString() != "") ? DateTime.Parse(dataRow["DateStartWord"].ToString()) : DateTime.Now,
                            Health = dataRow["Health"].ToString(),
                            Height = (dataRow["Height"].ToString() == "") ? 0 : int.Parse(dataRow["Height"].ToString()),

                            Weight = (dataRow["Weight"].ToString() == "") ? 0 : int.Parse(dataRow["Weight"].ToString()),
                            Note = dataRow["Note"].ToString(),
                            NoBHXH = dataRow["NoBHXH"].ToString(),
                            DateBHXH = (dataRow["DateBHXH"].ToString() != "") ? DateTime.Parse(dataRow["DateBHXH"].ToString()) : DateTime.Now,
                            IssueByBHXH = dataRow["IssueByBHXH"].ToString(),

                            NoBHYT = dataRow["NoBHYT"].ToString(),
                            fromDateBHYT = (dataRow["DateBHXH"].ToString() != "") ? DateTime.Parse(dataRow["fromDateBHYT"].ToString()) : DateTime.Now,
                            toDateBHYT = (dataRow["DateBHXH"].ToString() != "") ? DateTime.Parse(dataRow["toDateBHYT"].ToString()) : DateTime.Now,
                            ProvinceBHYT = dataRow["ProvinceBHYT"].ToString(),
                            HopitalBHYT = dataRow["HopitalBHYT"].ToString(),

                            StatusWork = 1,
                            NationalityID = nationalityID,
                            NationID = nationID,
                            ReligionID = religionID,
                            DegreeID = degreeID,

                            BHTN = (dataRow["BHTN"].ToString() == "1") ? true : false,
                            BHYT = (dataRow["BHYT"].ToString() == "1") ? true : false,
                            BHXH = (dataRow["BHXH"].ToString() == "1") ? true : false,
                            UnionDues = (dataRow["UnionDues"].ToString() == "1") ? true : false,
                            Authority = (dataRow["Authority"].ToString() == "1") ? true : false,

                            MaritalID = maritalID,
                            DepartmentID = depID,
                            PositionID = positionID,
                            CompanyID = companyID,
                            isActive = true,
                        };

                        db.tblEmployee.Add(employee);
                    }

                    db.SaveChanges(); // Save changes to the database
                    TempData["Upload"] = "Data imported successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }

            return View();
        }



        //export data to excel
        public ActionResult Export(System.Web.Mvc.FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                var company = collection["CompanyID"];
                var deptList = collection["DepartmentID"];
                string sqlDept = (deptList == null) ? "" : "and dept.ID in (" + deptList + ")";

                string sql = "select *,nation.NameNationVN as Nation,nationality.NameNationalityVN as Nationality,reli.NameReligionVN as Religion," +
                      "degree.DegreeVN as Degree,marital.MaritalStatusVN as Marital,position.PositionNameVN as Position,dept.NameDepartmentVN as Department," +
                      "com.NameCompanyVN as Company " +
                      "from tblEmployee e " +
                      "left outer join tblCompany com on com.ID = e.CompanyID " +
                      "left outer join tblDepartment dept on dept.ID = e.DepartmentID " +
                      "left outer join tblNation nation on nation.ID = e.NationID " +
                      "left outer join tblNationality nationality on nationality.ID = e.NationalityID " +
                      "left outer join tblReligion reli on reli.ID = e.ReligionID " +
                      "left outer join tblDegree degree on degree.ID = e.DegreeID " +
                      "left outer join tblMaritalStatus marital on marital.ID = e.MaritalID " +
                      "left outer join tblPosition position on position.ID = e.PositionID " +
                      "where com.ID = " + company + sqlDept;
                var items = db.Database.SqlQuery<EmployeeDetail>(sql).ToList();
                ExcelPackage.LicenseContext = LicenseContext.Commercial;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var Ep = new ExcelPackage(new FileInfo("MyWorkbook.xlsx")))
                {
                    var boldFont = new System.Drawing.Font("Times New Roman", 11, System.Drawing.FontStyle.Bold);
                    ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
                    Sheet.Cells["A1"].Value = "EmployeeCode";
                    Sheet.Cells["B1"].Value = "FirstName";
                    Sheet.Cells["C1"].Value = "FullName";
                    Sheet.Cells["D1"].Value = "CommonName";
                    Sheet.Cells["E1"].Value = "Gender";
                    Sheet.Cells["F1"].Value = "Birthday";
                    Sheet.Cells["G1"].Value = "PlaceOfBirth";
                    Sheet.Cells["H1"].Value = "Address";
                    Sheet.Cells["I1"].Value = "AddressTmp";
                    Sheet.Cells["J1"].Value = "Phone";
                    Sheet.Cells["K1"].Value = "Email";
                    Sheet.Cells["L1"].Value = "CCCD";
                    Sheet.Cells["M1"].Value = "DateCCCD";
                    Sheet.Cells["N1"].Value = "IssuedBy";
                    Sheet.Cells["O1"].Value = "Health";
                    Sheet.Cells["P1"].Value = "Height";
                    Sheet.Cells["Q1"].Value = "Weight";
                    Sheet.Cells["R1"].Value = "Nationality";
                    Sheet.Cells["S1"].Value = "Nation";
                    Sheet.Cells["T1"].Value = "Religion";
                    Sheet.Cells["U1"].Value = "Degree";
                    Sheet.Cells["V1"].Value = "Foreign";
                    Sheet.Cells["W1"].Value = "BHXH";
                    Sheet.Cells["X1"].Value = "BHYT";
                    Sheet.Cells["Y1"].Value = "BHTN";
                    Sheet.Cells["Z1"].Value = "UnionDues";
                    Sheet.Cells["AA1"].Value = "Note";
                    Sheet.Cells["AB1"].Value = "Marital";
                    Sheet.Cells["AC1"].Value = "Authority";
                    Sheet.Cells["AD1"].Value = "Department";
                    Sheet.Cells["AE1"].Value = "Position";
                    Sheet.Cells["AF1"].Value = "Company";
                    Sheet.Cells["AG1"].Value = "BHXH";
                    Sheet.Cells["AH1"].Value = "DateBHXH";
                    Sheet.Cells["AI1"].Value = "IssueBHXH";
                    Sheet.Cells["AJ1"].Value = "BHYT";
                    Sheet.Cells["AK1"].Value = "FromDate";
                    Sheet.Cells["AL1"].Value = "ToDate";
                    Sheet.Cells["AM1"].Value = "Province";
                    Sheet.Cells["AN1"].Value = "Hopital";


                    Sheet.Cells["A1:AN1"].Style.Font.Bold = true;
                    Sheet.Cells["A1:AN1"].Style.Font.SetFromFont(boldFont.Name, boldFont.Size, false, false, false, false);
                    int row = 2;
                    foreach (var item in items)
                    {

                        Sheet.Cells[string.Format("A{0}", row)].Value = item.EmployeeCode;
                        Sheet.Cells[string.Format("B{0}", row)].Value = item.FirstName;
                        Sheet.Cells[string.Format("C{0}", row)].Value = item.FullName;
                        Sheet.Cells[string.Format("D{0}", row)].Value = item.CommonName;
                        Sheet.Cells[string.Format("E{0}", row)].Value = (item.Gender.ToString() == "1") ? "Nam" : "Nữ";
                        Sheet.Cells[string.Format("F{0}", row)].Value = (item.Birthday.ToString() != "") ? DateTime.Parse(item.Birthday.ToString()).ToString("yyyy-MM-dd") : "";
                        Sheet.Cells[string.Format("G{0}", row)].Value = item.PlaceOfBirth;
                        Sheet.Cells[string.Format("H{0}", row)].Value = item.Address;
                        Sheet.Cells[string.Format("I{0}", row)].Value = item.AddressTmp;
                        Sheet.Cells[string.Format("J{0}", row)].Value = item.Phone;
                        Sheet.Cells[string.Format("K{0}", row)].Value = item.Email;
                        Sheet.Cells[string.Format("L{0}", row)].Value = item.CCCD;
                        Sheet.Cells[string.Format("M{0}", row)].Value = (item.DateCCCD.ToString() != "") ? DateTime.Parse(item.DateCCCD.ToString()).ToString("yyyy-MM-dd") : "";
                        Sheet.Cells[string.Format("N{0}", row)].Value = item.IssuedBy;
                        Sheet.Cells[string.Format("O{0}", row)].Value = item.Health;
                        Sheet.Cells[string.Format("P{0}", row)].Value = item.Height;
                        Sheet.Cells[string.Format("Q{0}", row)].Value = item.Weight;
                        Sheet.Cells[string.Format("R{0}", row)].Value = item.Nationality;
                        Sheet.Cells[string.Format("S{0}", row)].Value = item.Nation;
                        Sheet.Cells[string.Format("T{0}", row)].Value = item.Religion;
                        Sheet.Cells[string.Format("U{0}", row)].Value = item.Degree;
                        Sheet.Cells[string.Format("V{0}", row)].Value = item.Foreign;
                        Sheet.Cells[string.Format("W{0}", row)].Value = item.BHXH;
                        Sheet.Cells[string.Format("X{0}", row)].Value = item.BHYT;
                        Sheet.Cells[string.Format("Y{0}", row)].Value = item.BHTN;
                        Sheet.Cells[string.Format("Z{0}", row)].Value = item.UnionDues;
                        Sheet.Cells[string.Format("AA{0}", row)].Value = item.Note;
                        Sheet.Cells[string.Format("AB{0}", row)].Value = item.Marital;
                        Sheet.Cells[string.Format("AC{0}", row)].Value = item.Authority;
                        Sheet.Cells[string.Format("AD{0}", row)].Value = item.Department;
                        Sheet.Cells[string.Format("AE{0}", row)].Value = item.Position;
                        Sheet.Cells[string.Format("AF{0}", row)].Value = item.Company;
                        Sheet.Cells[string.Format("AG{0}", row)].Value = item.NoBHXH;
                        Sheet.Cells[string.Format("AH{0}", row)].Value = (item.DateBHXH.ToString() != "") ? DateTime.Parse(item.DateBHXH.ToString()).ToString("yyyy-MM-dd") : "";
                        Sheet.Cells[string.Format("AI{0}", row)].Value = item.IssueByBHXH;
                        Sheet.Cells[string.Format("AJ{0}", row)].Value = item.NoBHYT;
                        Sheet.Cells[string.Format("AK{0}", row)].Value = (item.fromDateBHYT.ToString() != "") ? DateTime.Parse(item.fromDateBHYT.ToString()).ToString("yyyy-MM-dd") : "";
                        Sheet.Cells[string.Format("AL{0}", row)].Value = (item.toDateBHYT.ToString() != "") ? DateTime.Parse(item.toDateBHYT.ToString()).ToString("yyyy-MM-dd") : "";
                        Sheet.Cells[string.Format("AM{0}", row)].Value = item.ProvinceBHYT;
                        Sheet.Cells[string.Format("AN{0}", row)].Value = item.HopitalBHYT;
                        row++;
                    }
                    // Apply bold font to header cells
                    var headerRange = Sheet.Cells["A1:AN1"];
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSkyBlue);

                    // Apply border to header cells
                    headerRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    headerRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    headerRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    headerRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                    // Apply border to all data cells
                    var allDataRange = Sheet.Cells[2, 1, row - 1, 40];
                    allDataRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    allDataRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    allDataRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    allDataRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;


                    Sheet.Cells["A:AZ"].AutoFitColumns();
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment: filename=" + "EmployeeList" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
                    Response.BinaryWrite(Ep.GetAsByteArray());
                    Response.End();
                }


            }
            action.InsertActionLog(Session["UserName"].ToString(), 6, 10, "");

            return View();
        }

        #endregion

        [HttpPost]
        public ActionResult DeleteEmployee(int id)
        {
            if (ModelState.IsValid && Session["UserID"] != null)
            {
                // TODO: Add update logic here
                using (HRManagerEntities db = new HRManagerEntities())
                {
                    var employeeDelete = db.tblEmployee.Where(a => a.ID.Equals(id)).FirstOrDefault();
                    employeeDelete.isActive = false;
                    db.Entry(employeeDelete).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["delete"] = "Xóa thành công";
                    return RedirectToAction("Index");
                }
            }
            else return RedirectToAction("Login", "Home");
        }


        public ActionResult GetEmployeeData(int id)
        {
            string sql = "select e.EmployeeCode, e.FirstName ,e.FullName , dept.NameDepartmentVN as Department, p.PositionNameVN as Position, " +
                         "COALESCE(CAST(e.Salary AS DECIMAL(9,0)), 0) as SalaryBasic " +         
                         "from tblEmployee e " +
                         "left join tblDepartment dept on dept.ID = e.DepartmentID " +
                         "left join tblPosition p on p.ID = e.PositionID " +
                         "where e.isActive = true and e.ID = " + id;
            var item = db.Database.SqlQuery<EmployeeDetail>(sql).FirstOrDefault();

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult updateSalaryBasic(int EmployeeId , double SalaryBasic)
        {
            var item = db.tblEmployee.Where(x=>x.ID == EmployeeId).FirstOrDefault();
            item.Salary = SalaryBasic;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEmployeeInfo(int employeeID)
        {
            var employee = db.tblEmployee.FirstOrDefault(e => e.ID == employeeID && e.isActive == true);
            var department = db.tblDepartment.FirstOrDefault(e => e.ID == employee.DepartmentID);
            var position = db.tblPosition.FirstOrDefault(e => e.ID == employee.PositionID);
            var annualLeave = db.tblAnnualLeave.Where(x=> x.EmployeeID == employeeID).FirstOrDefault();
            if (employee != null)
            {
                var data = new
                {
                    fullName = employee.FullName,
                    department = department.NameDepartmentVN,
                    position = position.PositionNameVN,
                    annualLeave = annualLeave.AnnualLeave
                };

                return Json(data, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

    }
}