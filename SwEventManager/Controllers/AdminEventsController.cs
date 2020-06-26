using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SwEventManager.Models;
using System.IO;
using SwEventManager.Utilities;

namespace SwEventManager.Controllers
{
    [SessionCheck]
    [AdminCheck]
    public class AdminEventsController : Controller
    {
        private SummitWorksEventManagerEntities db = new SummitWorksEventManagerEntities();

        // GET: Events
        public ActionResult Index()
        {
            return View(db.Events.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            #region ViewBag
            ViewBag.MyCatagories = new List<SelectListItem>() {
                new SelectListItem { Text = "Conference", Value = "Conference" },
                new SelectListItem { Text = "Seminar", Value = "Seminar" },
                new SelectListItem { Text = "Presentation", Value = "Presentation" },
            };
            #endregion
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventID,EventName,EventDescription,EventCategory,StartDate,EndDate,StartTime,EndTime,Location,OpenForRegistration,AdultPrice,ChildPrice,CompanyName")] Event @event, HttpPostedFileBase file)
        {
            #region ViewBag
            ViewBag.MyCatagories = new List<SelectListItem>() {
                new SelectListItem { Text = "Conference", Value = "Conference" },
                new SelectListItem { Text = "Seminar", Value = "Seminar" },
                new SelectListItem { Text = "Presentation", Value = "Presentation" },
            };
            #endregion
            try
            {
                if (ModelState.IsValid)
                {
                    if (file != null)
                    {
                        string pic = System.IO.Path.GetFileName(file.FileName);
                        string path = System.IO.Path.Combine(Server.MapPath("/images"), pic);
                        // file is uploaded
                        file.SaveAs(path);

                        // save the image path path to the database or you can send image 
                        // directly to database
                        // in-case if you want to store byte[] ie. for DB
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.InputStream.CopyTo(ms);
                            byte[] array = ms.GetBuffer();
                            db.Events.Add(@event);
                            @event.imagePath = "/images/" + file.FileName;
                            db.SaveChanges();
                        }

                    }
                    // after successfully uploading redirect the user
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                Console.Write("Bad Input");
            }

            return View(@event);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            #region ViewBag
            ViewBag.MyCatagories = new List<SelectListItem>() {
                new SelectListItem { Text = "Conference", Value = "Conference" },
                new SelectListItem { Text = "Seminar", Value = "Seminar" },
                new SelectListItem { Text = "Presentation", Value = "Presentation" },
            };
            #endregion
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            TempData["oldPath"] = @event.imagePath;
           
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
           
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventID,EventName,EventDescription,EventCategory,StartDate,EndDate,StartTime,EndTime,Location,OpenForRegistration,imagePath,AdultPrice,ChildPrice,CompanyName")] Event @event, HttpPostedFileBase file)
        {
            #region ViewBag
            ViewBag.MyCatagories = new List<SelectListItem>() {
                new SelectListItem { Text = "Conference", Value = "Conference" },
                new SelectListItem { Text = "Seminar", Value = "Seminar" },
                new SelectListItem { Text = "Presentation", Value = "Presentation" },
            };
            #endregion
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(@event).State = EntityState.Modified;
                    if (file != null)
                    {
                        string pic = System.IO.Path.GetFileName(file.FileName);
                        string path = System.IO.Path.Combine(Server.MapPath("/images"), pic);
                        // file is uploaded
                        file.SaveAs(path);

                        // save the image path path to the database or you can send image 
                        // directly to database
                        // in-case if you want to store byte[] ie. for DB
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.InputStream.CopyTo(ms);
                            byte[] array = ms.GetBuffer();
                            @event.imagePath = "/images/" + file.FileName;
                            db.SaveChanges();
                        }

                    }
                    else
                    {
                        @event.imagePath = TempData["oldPath"].ToString();
                        db.SaveChanges();
                    }
                    //db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch
                {
                    Console.WriteLine("Bad Input");
                }
            }
            return View(@event);
     
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            db.Events.Remove(@event);
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
    }

}

