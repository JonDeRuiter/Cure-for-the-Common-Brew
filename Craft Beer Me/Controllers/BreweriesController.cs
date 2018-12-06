using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Craft_Beer_Me.Models;

namespace Craft_Beer_Me.Controllers
{
    public class BreweriesController : Controller
    {
        private BreweryContext db = new BreweryContext();

        // GET: Breweries
        public ActionResult Index()
        {
            return View(db.Breweries.ToList());
        }

        // GET: Breweries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brewery brewery = db.Breweries.Find(id);
            if (brewery == null)
            {
                return HttpNotFound();
            }
            return View(brewery);
        }

        // GET: Breweries/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Breweries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Url,PictureUrl,Name,BreweryID")] Brewery brewery)
        {
            if (ModelState.IsValid)
            {
                db.Breweries.Add(brewery);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(brewery);
        }

        // GET: Breweries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brewery brewery = db.Breweries.Find(id);
            if (brewery == null)
            {
                return HttpNotFound();
            }
            return View(brewery);
        }

        // POST: Breweries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Url,PictureUrl,Name,BreweryID")] Brewery brewery)
        {
            if (ModelState.IsValid)
            {
                db.Entry(brewery).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(brewery);
        }

        // GET: Breweries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brewery brewery = db.Breweries.Find(id);
            if (brewery == null)
            {
                return HttpNotFound();
            }
            return View(brewery);
        }

        // POST: Breweries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Brewery brewery = db.Breweries.Find(id);
            db.Breweries.Remove(brewery);
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
