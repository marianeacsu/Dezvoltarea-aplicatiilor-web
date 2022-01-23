using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.IO;
using ProiectFinal.Models;

namespace ProiectFinal.Controllers
{
    
    public class ShopCartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ShopCart
        public ActionResult Index()
        {
            var products = new List<Product>();
            int totalPrice = 0;
            HttpCookie ShopCartCookie = Request.Cookies["ShopCart"];
            if (ShopCartCookie != null && !string.IsNullOrEmpty(ShopCartCookie.Value))
            {
                string[] split = ShopCartCookie.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in split)
                {
                    int prodId = int.Parse(item);
                    var product = db.Products.Find(prodId);
                    products.Add(product);
                    totalPrice += product.ProdPrice;
                }
            }

            ViewBag.Products = products;
            ViewBag.TotalPrice = totalPrice;
            return View();

        }

        public ActionResult New(int id)
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("Editor") && !User.IsInRole("User"))
                return Redirect("/Account/Register");

            else
            {

                HttpCookie ShopCartCookie = new HttpCookie("ShopCart");
                if (Request.Cookies["ShopCart"] != null)
                    ShopCartCookie = Request.Cookies["ShopCart"];

                if (string.IsNullOrEmpty(ShopCartCookie.Value))
                    ShopCartCookie.Value = id.ToString();
                else
                    ShopCartCookie.Value += "," + id.ToString();
                ShopCartCookie.Expires = DateTime.Now.AddHours(24);

                Response.Cookies.Add(ShopCartCookie);

                return Redirect("/Cart");
            }
        }

        public ActionResult Empty()
        {
            if(Request.Cookies["ShopCart"] != null)
            {
                HttpCookie ShopCartCookie = Request.Cookies["ShopCart"];
                ShopCartCookie.Value = "";
                ShopCartCookie.Expires = DateTime.Now;
                Response.Cookies.Add(ShopCartCookie);
            }
            return Redirect("/ShopCart");
        }
    }
}