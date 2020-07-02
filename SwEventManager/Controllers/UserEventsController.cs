using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SwEventManager.Models;
using SwEventManager.Utilities;

namespace SwEventManager.Controllers
{

    [SessionCheck]
    public class UserEventsController : Controller
    {
        private SummitWorksEventManagerEntities db = new SummitWorksEventManagerEntities();

        // GET: UserEvents
        public ActionResult Index()
        {
            return View(db.Events.ToList());
        }

        // GET: UserEvents/Details/5
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
