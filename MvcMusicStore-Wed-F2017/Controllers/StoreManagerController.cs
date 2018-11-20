﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcMusicStore_Wed_F2017.Models;

namespace MvcMusicStore_Wed_F2017.Controllers
{
   
    [Authorize(Roles = "Administrator")]
    public class StoreManagerController : Controller
    {
        // db connection moved to Models/EFStoreManagerRepository.cs
        //private MusicStoreModel db = new MusicStoreModel();

        // create 2 constructors for Dependency Injection: 1 mock, 1 using EF
        // db can now be either the moc
        private IStoreManagerRepository db;

        public StoreManagerController()
        {
            // no param passed, so use EF Repository for db access
            this.db = new EFStoreManagerRepository();
        }

        public StoreManagerController(IStoreManagerRepository smRepo)
        {
            this.db = smRepo;
        }

        private List<Album> FetchIndexData()
        {
            // original code using db directly
            var albums = db.Albums.Include(a => a.Artist).Include(a => a.Genre);

            ViewBag.AlbumCount = albums.Count();

            return (albums.OrderBy(a => a.Artist.Name).ThenBy(a => a.Title).ToList());

        }
        // GET: StoreManager
        public ViewResult Index()
        {
            //// original code using db directly
            //var albums = db.Albums.Include(a => a.Artist).Include(a => a.Genre);

            //// new code using our interfaces
            ////var albums = smRepo.Albums.Include(a => a.Artist).Include(a => a.Genre);

            //ViewBag.AlbumCount = albums.Count();
            var albums = FetchIndexData();
            ViewBag.AlbumCount = albums.Count();
            return View(albums);
        }

        // POST: StoreManager - Search Albums By Title
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(String Title)
        {
            var albums = from a in db.Albums
                         where a.Title.Contains(Title)
                         select a;

            ViewBag.AlbumCount = albums.Count();
            return View(albums);
        }

        [AllowAnonymous]
        // GET: StoreManager/Details/5
        public ViewResult Details(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            Album album = db.Albums.FirstOrDefault(a => a.AlbumId == id);
            if (album == null)
            {
                return View("Error");
                //return HttpNotFound();
            }
            return View(album);
        }

        // GET: StoreManager/Create
        public ViewResult Create()
        {
            // sort by Artist and Genre
            var artists = db.Artists.ToList().OrderBy(a => a.Name);
            var genres = db.Genres.ToList().OrderBy(g => g.Name);

            ViewBag.ArtistId = new SelectList(artists, "ArtistId", "Name");
            ViewBag.GenreId = new SelectList(genres, "GenreId", "Name");
            return View("Create");
        }

        // POST: StoreManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ViewResult Create([Bind(Include = "AlbumId,GenreId,ArtistId,Title,Price,AlbumArtUrl")] Album album)
        {
            if (album == null)
            {
                ViewBag.ArtistId = new SelectList(db.Artists, "ArtistId", "Name");
                ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "Name");
                return View("Create");
            }
            
            if (ModelState.IsValid)
            {
                if (Request != null) { 
                    // check for a new cover image upload
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];

                        if (file.FileName != null && file.ContentLength > 0)
                        {
                            string path = Server.MapPath("~/Content/Images/") + file.FileName;
                            file.SaveAs(path);

                            // add path to image name before saving
                            album.AlbumArtUrl = "/Content/Images/" + file.FileName;
                        }
                    }
                }

                //db.Albums.Add(album);
                //db.SaveChanges();
                db.Save(album);
                // original code using db directly
                var albums = FetchIndexData();
                ViewBag.AlbumCount = albums.Count();
                return View("Index", albums); // RedirectToAction("Index");
            }

            ViewBag.ArtistId = new SelectList(db.Artists, "ArtistId", "Name", album.ArtistId);
            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "Name", album.GenreId);
            return View(album);
        }

        //GET: StoreManager/Edit/5
        public ViewResult Edit(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            //Album album = db.Albums.Find(id);
            Album album = db.Albums.FirstOrDefault(a => a.AlbumId == id);

            if (album == null)
            {
                return View("Error");
            }

            // sort by Artist and Genre
            var artists = db.Artists.ToList().OrderBy(a => a.Name);
            var genres = db.Genres.ToList().OrderBy(g => g.Name);

            ViewBag.ArtistId = new SelectList(artists, "ArtistId", "Name", album.ArtistId);
            ViewBag.GenreId = new SelectList(genres, "GenreId", "Name", album.GenreId);
            return View("Edit", album);
        }

        // POST: StoreManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ViewResult Edit([Bind(Include = "AlbumId,GenreId,ArtistId,Title,Price,AlbumArtUrl")] Album album)
        {
            // if no album stay on the current view
            if (album == null)
            {
                ViewBag.ArtistId = new SelectList(db.Artists, "ArtistId", "Name");
                ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "Name");
                var albums = FetchIndexData();
                return View("Edit");
            }

            if (ModelState.IsValid)
            {
                if (Request != null)
                {
                    // check for a new cover image upload
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];

                        if (file.FileName != null && file.ContentLength > 0)
                        {
                            string path = Server.MapPath("~/Content/Images/") + file.FileName;
                            file.SaveAs(path);

                            // add path to image name before saving
                            album.AlbumArtUrl = "/Content/Images/" + file.FileName;
                        }
                    }
                }


                //db.Entry(album).State = EntityState.Modified;
                //db.SaveChanges();
                db.Save(album);

                // original code using db directly
                var albums = FetchIndexData();
                ViewBag.AlbumCount = albums.Count();

                return View("Index", albums);

                //return View("Index");
            }
            ViewBag.ArtistId = new SelectList(db.Artists, "ArtistId", "Name", album.ArtistId);
            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "Name", album.GenreId);
            return View("Edit", album);
        }

        // GET: StoreManager/Delete/5
        public ViewResult Delete(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            //Album album = db.Albums.Find(id);
            Album album = db.Albums.FirstOrDefault(a => a.AlbumId == id);
            if (album == null)
            {
                return View("Error");
            }
            return View("Delete", album);
        }

        // POST: StoreManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ViewResult DeleteConfirmed(int? id)
        {
            //Album album = db.Albums.Find(id);
            //db.Albums.Remove(album);
            //db.SaveChanges();
            //return RedirectToAction("Index");

            if (id == null)
            {
                return View("Error");
            }
            //Album album = db.Albums.Find(id);
            Album album = db.Albums.FirstOrDefault(a => a.AlbumId == id);
            if (album == null)
            {
                return View("Error");
            }

            db.Delete(album);

            // refresh album list and show index view after delete
            var albums = FetchIndexData();
            ViewBag.AlbumCount = albums.Count;

            return View("Index", albums);
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
