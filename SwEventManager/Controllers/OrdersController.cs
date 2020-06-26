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
    public class OrdersController : Controller
    {
        private SummitWorksEventManagerEntities db = new SummitWorksEventManagerEntities();

        // GET: Orders
        public ActionResult Index()
        {
            int sessionId = Int32.Parse(Session["UserID"].ToString());

            if (Session["IsAdmin"].ToString().Equals("True"))
            {
                var orders = db.Orders.Include(o => o.Event).Include(o => o.User);
                
                return View(orders.ToList());
            }
            else
            {
                var orders = db.Orders.Where(o => o.UserID == sessionId);
                //var orders = db.Orders.Include(o => o.Event).Include(o => o.User);
                return View(orders.ToList());
            }
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventName");
            ViewBag.UserID = new SelectList(db.Users, "UserId", "Firstname");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,UserID,EventID,PhoneNum,Location,TotalAdult,TotalChild,OrderDate,totalPrice")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderDate = DateTime.Now;
                order.UserID = Int32.Parse(Session["UserID"].ToString());         
                Event event1 = db.Events.Find(order.EventID);
                order.totalPrice = order.TotalAdult*event1.AdultPrice + order.TotalChild*event1.ChildPrice;
                order.Location = event1.Location;
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //order.OrderDate = DateTime.Now;
            //order.UserID = Int32.Parse(Session["UserID"].ToString());
            //Event event2 = db.Events.Find(order.EventID);
            //order.totalPrice = order.TotalAdult * event2.AdultPrice + order.TotalChild * event2.ChildPrice;
            //order.Location = event2.Location;
            //db.Orders.Add(order);
            //db.SaveChanges();
            

            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventName", order.EventID);
            ViewBag.UserID = new SelectList(db.Users, "UserId", "Firstname", order.UserID);
            return RedirectToAction("Index");
            //return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            //ViewBag.EventID = new SelectList(db.Events, "EventID", "EventName", order.EventID);
            //ViewBag.EventID = new SelectList(db.Events, "EventID", "EventName", order.EventID).First();
            ViewData["EventName"] = db.Events.Find(order.EventID).EventName.ToString();
            Console.WriteLine(ViewData["EventName"]);
            String a = ViewData["EventName"].ToString();


            ViewBag.UserID = new SelectList(db.Users, "UserId", "Firstname", order.UserID);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,UserID,EventID,PhoneNum,Location,TotalAdult,TotalChild,OrderDate,totalPrice")] Order order)
        {
            
            if (ModelState.IsValid)
            {
                order.UserID = Int32.Parse(Session["UserID"].ToString());
                order.OrderDate = DateTime.Now;
                Event event1 = db.Events.Find(order.EventID);
                order.totalPrice = order.TotalAdult * event1.AdultPrice + order.TotalChild * event1.ChildPrice;
                
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventName", order.EventID);
            ViewBag.UserID = new SelectList(db.Users, "UserId", "Firstname", order.UserID);
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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
