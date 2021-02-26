using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProiectFinal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Security.Application;
using System.IO;
using System.Data.Entity;

namespace ProiectFinal.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Products

        public ActionResult Index()
        {
            var search = Request.Params["search"];
            var sort = Request.Params["sort"];

            if (search == null)
                search = "";
            if (sort == null)
                sort = "";


            var products = db.Products.Include("Category").Include("User").Where(product => product.ProdName.Contains(search)).ToList(); ;
            foreach (var product in products)
                SumRatings(products);

            if (sort == "price-asc")
                products = products.OrderBy(product => product.ProdPrice).ToList();
            if (sort == "price-desc")
                products = products.OrderByDescending(product => product.ProdPrice).ToList();
            if (sort == "rating-asc")
                products = products.OrderBy(product => product.ProdRating).ToList();
            if (sort == "rating-desc")
                products = products.OrderByDescending(product => product.ProdRating).ToList();

            ViewBag.Products = products;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            ViewBag.UserId = User.Identity.GetUserId();
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            return View();
        }
        private void SumRatings(List<Product> products)
        {
            foreach (Product product in products)
                SumRating(product);
        }

        static public void SumRating(Product product)
        {
            var numar_com = product.Comments.Count;
            foreach (Comment comm in product.Comments)
                product.ProdRating += comm.Rating;
            if (numar_com != 0)
                product.ProdRating = product.ProdRating / numar_com;
            else
                product.ProdRating = 0;
        }

        public ActionResult Show(int id)
        {
            Product product = db.Products.Find(id);
            
            SumRating(product);

            ViewBag.Product = product;
            ViewBag.Category = product.Category;
            ViewBag.FileName = product.FileName;

            ViewBag.Comments = from comment in product.Comments
                               select comment;

            return View();
        }
            

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Show(Comment comm)
        {
            comm.Date = DateTime.Now;
            comm.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Comments.Add(comm);
                    db.SaveChanges();
                    return Redirect("/Products/Show/" + comm.ProdId);
                }

                else
                {
                    Product a = db.Products.Find(comm.ProdId);
                    return View(a);
                }

            }

            catch (Exception e)
            {
                Product a = db.Products.Find(comm.ProdId);
                return View(a);
            }

        }

        [Authorize(Roles = "Editor,Admin")]
        public ActionResult New()
        {
            Product product = new Product();

            var categories = from cat in db.Categories
                             select cat;
            ViewBag.Categories = categories;

            product.UserId = User.Identity.GetUserId();
            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult New(Product product)
        {
           
            var categories = from cat in db.Categories
                             select cat;
            ViewBag.Categories = categories;
            product.UserId = User.Identity.GetUserId();
            string fileExtension = Path.GetExtension(product.Image.FileName);

            try
            {
                if(ModelState.IsValid && (fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == ".jpeg"))
                {
                    string uploadFolderPath = Server.MapPath("~//Files//");
                    product.Image.SaveAs(uploadFolderPath + product.Image.FileName);
                    product.FileName = product.Image.FileName;
                    db.Products.Add(product);
                    db.SaveChanges();
                    TempData["message"] = "Produs adaugat!";
                    return Redirect("/Products/Index");
                }
                else
                {
                    return Redirect("/Products/Adauga");
                }
            }
            catch (Exception e)
            {
                return View(product);
            }
        }

        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Edit(int id)
        {
            Product product = db.Products.Find(id);
            ViewBag.Product = product;
            ViewBag.Category = product.Category;
            var categories = from cat in db.Categories
                             select cat;
            ViewBag.Categories = categories;

            if (product.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(product);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui produs care nu va apartine";
                return RedirectToAction("Index");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Edit(int id, Product requestProduct)
        {
            try
            {
                
                
                    Product product = db.Products.Find(id);

                    if (product.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))

                    {
                        if (TryUpdateModel(product))
                        {
                            product.ProdName = requestProduct.ProdName;
                            product.Content = requestProduct.Content;
                            product.Date = requestProduct.Date;
                            product.ProdPrice = requestProduct.ProdPrice;
                            product.CategoryId = requestProduct.CategoryId;
                            db.SaveChanges();
                            TempData["message"] = "Produsul a fost modificat cu succes!";
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui produs care nu va apartine!";
                        return RedirectToAction("Index");
                    }
                
                

            }
            catch (Exception e)
            {
                
                return View();
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Delete(int id)
        {
            Product product = db.Products.Find(id);

            if (product.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Products.Remove(product);
                db.SaveChanges();
                TempData["message"] = "Produsul a fost sters!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti acest produs, deoarece nu va apartine";
                return RedirectToAction("Index");
            }
        }

        

        


        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            var selectList = new List<SelectListItem>();

            var categories = from cat in db.Categories
                             select cat;

            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }

            /*
            foreach (var category in categories)
            {
                var listItem = new SelectListItem();
                listItem.Value = category.CategoryId.ToString();
                listItem.Text = category.CategoryName.ToString();

                selectList.Add(listItem);
            }*/

            return selectList;
        }

        

        [Authorize(Roles = "Admin" )]
        public ActionResult Accept()
        {
            var products = db.Products.Include("Category").Include("User");
            ViewBag.Products = products;
            if (TempData.ContainsKey("message"))
                ViewBag.Message = TempData["message"];
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin" )]
        public ActionResult Accept(int id)
        {
            Product product = db.Products.Find(id);
            product.Accepted = true;
            if(TryUpdateModel(product))
            {
                db.SaveChanges();
                TempData["message"] = "Produsul a fost acceptat";
                return RedirectToAction("Index");
            }

            return View();
        }
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Adauga()
        {

            return View();
        }
    }
}