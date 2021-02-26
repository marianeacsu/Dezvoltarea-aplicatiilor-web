using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProiectFinal.Models;
using Microsoft.AspNet.Identity;

namespace ProiectFinal.Controllers
{
    public class CommentsController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Comments
        public ActionResult Index()
        {
            return View();
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            db.Comments.Remove(comm);
            db.SaveChanges();
            return Redirect("/Products/Show" + comm.ProdId);
        }

       
        public ActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);
            ViewBag.Comment = comm;
            return View();
        }
        
        [HttpPut]
        public ActionResult Edit(int id, Comment requestComment)
        {
            try
            {
                    Comment comm = db.Comments.Find(id);
               
                    if(TryUpdateModel(comm))
                    {
                        comm.Content = requestComment.Content;
                        comm.Rating = requestComment.Rating;
                        db.SaveChanges();
                    }
                    return Redirect("/Products/Show/" + comm.ProdId);
          
            }
            catch (Exception e)
            {
                return View();
            }
        }
    }
}