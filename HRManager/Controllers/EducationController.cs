using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HRManager;
using HRManager.Common;

namespace HRManager.Controllers
{
    public class EducationController : Controller
    {
        private HRManagerEntities db = new HRManagerEntities();
       
        // GET: tblEducations
        public ActionResult Index()
        {
            return View(db.tblEducation.ToList());
        }

        // GET: tblEducations/Details/5
        public ActionResult Details(int id)
        {
            tblEducation tblEducation = db.tblEducation.Find(id);
            if (tblEducation == null)
            {
                return HttpNotFound();
            }
            return View(tblEducation);
        }

        // GET: tblEducations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tblEducations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,EmployeeID,School,DegreeID,Majors,FromDate,ToDate,isDelete,isActive,DateCreated,UserCreated,DateUpdated,UserUpdated")] tblEducation tblEducation)
        {
            if (ModelState.IsValid)
            {
                db.tblEducation.Add(tblEducation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblEducation);
        }

        // GET: tblEducations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblEducation tblEducation = db.tblEducation.Find(id);
            if (tblEducation == null)
            {
                return HttpNotFound();
            }
            return View(tblEducation);
        }

        // POST: tblEducations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,EmployeeID,School,DegreeID,Majors,FromDate,ToDate,isDelete,isActive,DateCreated,UserCreated,DateUpdated,UserUpdated")] tblEducation tblEducation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblEducation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblEducation);
        }

        // GET: tblEducations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblEducation tblEducation = db.tblEducation.Find(id);
            if (tblEducation == null)
            {
                return HttpNotFound();
            }
            return View(tblEducation);
        }

        // POST: tblEducations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblEducation tblEducation = db.tblEducation.Find(id);
            db.tblEducation.Remove(tblEducation);
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


        public JsonResult GetEducationByEmployee(int employeeID)
        {
            // Simulate fetching data from the database for the specified employeeID.
            // Replace this with actual code to fetch data from your database using Entity Framework or any other ORM.
            var educationForEmployee = db.tblEducation.Where(r => r.EmployeeID == employeeID).ToList();

            return Json(educationForEmployee, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InsertEducation(List<tblEducation> tblEducations)
        {
            using (HRManagerEntities entities = new HRManagerEntities())
            {
                //Check for NULL.
                if (tblEducations == null)
                {
                    tblEducations = new List<tblEducation>();
                }

                //Loop and insert records.
                foreach (tblEducation education in tblEducations)
                {
                    education.DateCreated = DateTime.Now;
                    entities.tblEducation.Add(education);
                }
                int insertedRecords = entities.SaveChanges();
                return Json(insertedRecords);
            }
        }
        public JsonResult DeleteEducation(List<tblEducation> educations)
        {
            using (HRManagerEntities entities = new HRManagerEntities())
            {
                //Check for NULL.
                if (educations == null)
                {
                    educations = new List<tblEducation>();
                }

                //Loop and insert records.
                foreach (tblEducation education in educations)
                {
                    tblEducation tblEducation = entities.tblEducation.Find(education.ID);
                    entities.tblEducation.Remove(tblEducation);
                }
                int deletedRecords = entities.SaveChanges();
                return Json(deletedRecords);
            }
        }


        [HttpPost]
        public JsonResult UpdateEducation(tblEducation model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new HRManagerEntities())
                    {
                        var existingEducation = db.tblEducation.FirstOrDefault(r => r.ID == model.ID);

                        if (existingEducation != null)
                        {
                            existingEducation.EmployeeID = model.EmployeeID;
                            existingEducation.School = model.School;
                            existingEducation.DegreeID = model.DegreeID;
                            existingEducation.Majors = model.Majors;
                            existingEducation.FromDate = model.FromDate;
                            existingEducation.ToDate = model.ToDate;

                            db.SaveChanges(); // Save changes to the database
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
