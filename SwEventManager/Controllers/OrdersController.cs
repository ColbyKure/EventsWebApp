using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics.Eventing.Reader;
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

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,UserID,EventID,PhoneNum,Location,TotalAdult,TotalChild,OrderDate,totalPrice")] Order order, int? eventID)
        {
            if (eventID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event event1 = db.Events.Find(order.EventID);
            if (event1 == null)
            {
                return HttpNotFound();
            }
            order.OrderDate = DateTime.Now;
            order.UserID = Int32.Parse(Session["UserID"].ToString());
            order.EventID = Int32.Parse(eventID.ToString());
            order.totalPrice = order.TotalAdult * event1.AdultPrice + order.TotalChild * event1.ChildPrice;
            order.Location = event1.Location;
            order.confirmed = false;
            db.Orders.Add(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET
        public ActionResult ConfirmOrder(int? id)
        {
            Order @order = db.Orders.Find(id);
            if (@order == null)
            {
                return HttpNotFound();
            }
            return View(@order);
        }

        //SET
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmOrder([Bind(Include = "OrderID,UserID,EventID,PhoneNum,Location,TotalAdult,TotalChild,OrderDate,totalPrice")] Order order)
        {
            order.confirmed = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ConfirmOrderAction(int? id)
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
            order.confirmed = true;
            db.SaveChanges();
            return RedirectToAction("Index");
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
            if (ViewBag.OrderConfirmation != null)
            {
                if (ViewBag.OrderConfirmation)
                {
                    return;
                }
            }
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
