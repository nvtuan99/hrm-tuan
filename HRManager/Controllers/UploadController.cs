using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class UploadController : Controller
    {
        HRManagerEntities db = new HRManagerEntities();
        // GET: Upload
        public ActionResult Index()
        {
            DataTable dt = new DataTable();

            //if ((String)Session["tmpdata"] != null)
            //{
            try
            {
                dt = (DataTable)Session["tmpdata"];
            }
            catch 
            {

            }

            //}


            return View(dt);
        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(HttpPostedFileBase upload)
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

                    // Import data into the database using Entity Framework
                    foreach (DataRow dataRow in dt.Rows)
                    {
                        var employee = new tblEmployee
                        {
                            EmployeeCode = dataRow["EmployeeCode"].ToString(),
                            FirstName = dataRow["EmployeeCode"].ToString(),
                            FullName = dataRow["FullName"].ToString(),
                            Address = dataRow["Address"].ToString()
                        };

                        db.tblEmployee.Add(employee);
                    }

                    db.SaveChanges(); // Save changes to the database

                    TempData["SuccessMessage"] = "Data imported successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }

            return View();
        }
    }
}