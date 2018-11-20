﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

// reference the models
using MvcMusicStore_Wed_F2017.Models;

namespace MvcMusicStore_Wed_F2017.Controllers
{
    public class ShoppingCartController : Controller
    {
        // db
        MusicStoreModel db = new MusicStoreModel();

        // GET: ShoppingCart
        public ActionResult Index()
        {
            // get current cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetCartTotal()
            };

            return View(viewModel);
        }

        // GET: AddToCart
        public ActionResult AddToCart(int AlbumId)
        {
            // get current cart (if any) and selected album
            var cart = ShoppingCart.GetCart(this.HttpContext);
            var album = db.Albums.SingleOrDefault(a => a.AlbumId == AlbumId);

            // add selected album to current cart
            cart.AddToCart(album);

            // redirect to /ShoppingCart
            return RedirectToAction("Index");

        }
    }
}