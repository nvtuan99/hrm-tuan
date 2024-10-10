using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HRManager;

namespace HRManager.Controllers
{
    public class HistoryWorkController : Controller
    {
        private HRManagerEntities db = new HRManagerEntities();

        // GET: HistoryWork
        public ActionResult Index()
        {
            return View(db.tblHistoryWork.ToList());
        }

        // GET: HistoryWork/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblHistoryWork tblHistoryWork = db.tblHistoryWork.Find(id);
            if (tblHistoryWork == null)
            {
                return HttpNotFound();
            }
            return View(tblHistoryWork);
        }

        // GET: HistoryWork/Create
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,EmployeeID,Company,PositionID,JobID,FromDate,ToDate,isDelete,isActive,DateCreated,UserCreated,DateUpdated,UserUpdated")] tblHistoryWork tblHistoryWork)
        {
            if (ModelState.IsValid)
            {
                db.tblHistoryWork.Add(tblHistoryWork);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblHistoryWork);
        }

        // GET: HistoryWork/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblHistoryWork tblHistoryWork = db.tblHistoryWork.Find(id);
            if (tblHistoryWork == null)
            {
                return HttpNotFound();
            }
            return View(tblHistoryWork);
        }

        // POST: HistoryWork/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,EmployeeID,Company,PositionID,JobID,FromDate,ToDate,isDelete,isActive,DateCreated,UserCreated,DateUpdated,UserUpdated")] tblHistoryWork tblHistoryWork)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblHistoryWork).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblHistoryWork);
        }

        // GET: HistoryWork/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblHistoryWork tblHistoryWork = db.tblHistoryWork.Find(id);
            if (tblHistoryWork == null)
            {
                return HttpNotFound();
            }
            return View(tblHistoryWork);
        }

        // POST: HistoryWork/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblHistoryWork tblHistoryWork = db.tblHistoryWork.Find(id);
            db.tblHistoryWork.Remove(tblHistoryWork);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpGet]
        public JsonResult GetHistoryWorkByEmployee(int employeeID)
        {
            // Simulate fetching data from the database for the specified employeeID.
            // Replace this with actual code to fetch data from your database using Entity Framework or any other ORM.
            var historyWorkForEmployee = db.tblHistoryWork.Where(r => r.EmployeeID == employeeID).ToList();

            return Json(historyWorkForEmployee, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InsertHistoryWork(List<tblHistoryWork> tblHistoryWorks)
        {
            using (HRManagerEntities entities = new HRManagerEntities())
            {
                //Check for NULL.
                if (tblHistoryWorks == null)
                {
                    tblHistoryWorks = new List<tblHistoryWork>();
                }

                //Loop and insert records.
                foreach (tblHistoryWork historywork in tblHistoryWorks)
                {
                    historywork.DateCreated = DateTime.Now;
                    entities.tblHistoryWork.Add(historywork);
                }
                int insertedRecords = entities.SaveChanges();
                return Json(insertedRecords);
            }
        }
        public JsonResult Deletehistorywork(List<tblHistoryWork> historyworks)
        {
            using (HRManagerEntities entities = new HRManagerEntities())
            {
                //Check for NULL.
                if (historyworks == null)
                {
                    historyworks = new List<tblHistoryWork>();
                }

                //Loop and insert records.
                foreach (tblHistoryWork historywork in historyworks)
                {
                    tblHistoryWork tblHistoryWork = entities.tblHistoryWork.Find(historywork.ID);
                    entities.tblHistoryWork.Remove(tblHistoryWork);
                }
                int deletedRecords = entities.SaveChanges();
                return Json(deletedRecords);
            }
        }


        [HttpPost]
        public JsonResult Updatehistorywork(tblHistoryWork model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new HRManagerEntities())
                    {
                        var existinghistorywork = db.tblHistoryWork.FirstOrDefault(r => r.ID == model.ID);

                        if (existinghistorywork != null)
                        {
                            existinghistorywork.EmployeeID = model.EmployeeID;
                            existinghistorywork.Company = model.Company;
                            existinghistorywork.PositionID = model.PositionID;
                            existinghistorywork.JobID = model.JobID;
                            existinghistorywork.FromDate = model.FromDate;
                            existinghistorywork.ToDate = model.ToDate;
                            db.SaveChanges();
                        }
                        else
                        {
                            return Json(new { success = false, message = "Not found data." });
                        }
                    }

                    return Json(new { success = true, message = "Data updated successfully!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error occurred while updating data: " + ex.Message });
                }
            }
            else
            {
                // If the model validation fails, return error message with the validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return Json(new { success = false, message = "Validation error: " + string.Join("; ", errors) });
            }
        }
    }
}
