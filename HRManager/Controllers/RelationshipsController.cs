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
    public class RelationshipsController : Controller
    {
        private HRManagerEntities db = new HRManagerEntities();

        // GET: tblRelationships
        public ActionResult Index()
        {
            return View(db.tblRelationship.ToList());
        }

        // GET: tblRelationships/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblRelationship tblRelationship = db.tblRelationship.Find(id);
            if (tblRelationship == null)
            {
                return HttpNotFound();
            }
            return View(tblRelationship);
        }

        // GET: tblRelationships/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tblRelationships/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,EmployeeID,Fullname,Relationship,Phone,Address,Note,Birthday,IsDelete,DateCreated,UserCreated,DateUpdated,UserUpdated")] tblRelationship tblRelationship)
        {
            if (ModelState.IsValid)
            {
                db.tblRelationship.Add(tblRelationship);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblRelationship);
        }

        // GET: tblRelationships/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblRelationship tblRelationship = db.tblRelationship.Find(id);
            if (tblRelationship == null)
            {
                return HttpNotFound();
            }
            return View(tblRelationship);
        }

        // POST: tblRelationships/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,EmployeeID,Fullname,Relationship,Phone,Address,Note,Birthday,IsDelete,DateCreated,UserCreated,DateUpdated,UserUpdated")] tblRelationship tblRelationship)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblRelationship).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblRelationship);
        }

        // GET: tblRelationships/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblRelationship tblRelationship = db.tblRelationship.Find(id);
            if (tblRelationship == null)
            {
                return HttpNotFound();
            }
            return View(tblRelationship);
        }

        // POST: tblRelationships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblRelationship tblRelationship = db.tblRelationship.Find(id);
            db.tblRelationship.Remove(tblRelationship);
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


        public JsonResult InsertRelationship(List<tblRelationship> relationships)
        {
            using (HRManagerEntities entities = new HRManagerEntities())
            {
                //Check for NULL.
                if (relationships == null)
                {
                    relationships = new List<tblRelationship>();
                }

                //Loop and insert records.
                foreach (tblRelationship relationship in relationships)
                {
                    relationship.IsActive = true;
                    relationship.DateCreated = DateTime.Now;
                    entities.tblRelationship.Add(relationship);
                }
                int insertedRecords = entities.SaveChanges();
                return Json(insertedRecords);
            }
        }


        public JsonResult DeleteRelationship(List<tblRelationship> relationships)
        {
            using (HRManagerEntities entities = new HRManagerEntities())
            {
                //Check for NULL.
                if (relationships == null)
                {
                    relationships = new List<tblRelationship>();
                }

                //Loop and insert records.
                foreach (tblRelationship relationship in relationships)
                {
                    tblRelationship tblRelationship = entities.tblRelationship.Find(relationship.ID);
                    entities.tblRelationship.Remove(tblRelationship);
                }
                int insertedRecords = entities.SaveChanges();
                return Json(insertedRecords);
            }
        }
        public JsonResult GetAllRelationships(int employeeID)
        {
            // Simulate fetching data from the database for the specified employeeID.
            // Replace this with actual code to fetch data from your database using Entity Framework or any other ORM.
            var relationshipsForEmployee = db.tblRelationship.Where(r => r.EmployeeID == employeeID).ToList();

            return Json(relationshipsForEmployee, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult UpdateRelationship(tblRelationship model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new HRManagerEntities())
                    {
                        var existingRelationship = db.tblRelationship.FirstOrDefault(r => r.ID == model.ID);

                        if (existingRelationship != null)
                        {
                            existingRelationship.EmployeeID = model.EmployeeID;
                            existingRelationship.Fullname = model.Fullname;
                            existingRelationship.Relationship = model.Relationship;
                            existingRelationship.Phone = model.Phone;
                            existingRelationship.Address = model.Address;
                            existingRelationship.Birthday = model.Birthday;

                            db.SaveChanges(); // Save changes to the database
                        }
                        else
                        {
                            return Json(new { success = false, message = "Relationship not found." });
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
