using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AvtoMnenie.Models;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace AvtoMnenie.Controllers
{
  public class HomeController : Controller
  {
    [AllowAnonymous]
    public ActionResult Index(bool DoRedirect = false, string SRequest = "")
    {
      ViewBag.PageName = "Каталог Автосалонов";
      if (DoRedirect)
      {
        ViewBag.DoRedirect = true;
        ViewBag.SRequest = SRequest;
      }
      else
      {
        ViewBag.DoRedirect = false;
        ViewBag.SRequest = "";
      }
      return View();
    }

    public ActionResult About()
    {
      ViewBag.PageName = "О проекте";

      return View();
    }

    public ActionResult News()
    {
      ViewBag.PageName = "Новости";

      return View();
    }

    public ActionResult SalonComments(string name)
    {
      return View();
    }

    [HttpPost]
    public ActionResult GetTopNewsOrTopReviews()
    {
      return PartialView();
    }

    [HttpPost]
    public ActionResult GetSalonsByArea()
    {
      string areaname = Request.Form["AreaSelect"];
      SalosAreasModel areamodel;
      using (SalonContext _db = new SalonContext())
      {
        areamodel = (from area in _db.Areas where area.Name.Equals(areaname) select area).FirstOrDefault();
      }
      return PartialView("~/Views/Home/_SalonsListPartial.cshtml", areamodel);
    }

    [HttpPost]
    public ActionResult GetSalonsByName()
    {
      return PartialView("~/Views/Home/GetSalonsByName.cshtml");
    }

    public ActionResult Salon(string name)
    {
      int SalonID = 0;
      using (SalonContext _db = new SalonContext())
      {
        var Salon = (from salon in _db.Salons where salon.TransName.Equals(name) select salon).FirstOrDefault();
      }
      if (SalonID == 0) return RedirectToAction("index");
      return RedirectToAction("ShowSalon", new { id = SalonID });
    }

    public ActionResult ShowSalon(int id)
    {
      ViewBag.PageName = "Автосалон";
      SalonContext _db = new SalonContext();
      return View();
    }

    public ActionResult ShowSalonByName(string name)
    {
      ViewBag.PageName = "Автосалон";
      SalonContext _db = new SalonContext();
      try
      {
        SalonModel model = (from salon in _db.Salons where salon.TransName == name select salon).FirstOrDefault();
        ViewBag.id = model.ID;
        return View();
      }
      catch
      {
        return RedirectToAction("Index");
      }
    }

    public ActionResult ShowSalonComments(string name)
    {
      using (SalonContext _db = new SalonContext())
      {
        ViewBag.TargetSalonID = (from s in _db.Salons where s.TransName.Equals(name) select s).FirstOrDefault().ID;
      }
      return View();

    }

    [HttpGet]
    public ActionResult PostComment(string name)
    {
      using (SalonContext _db = new SalonContext())
      {
        ViewBag.TargetSalonID = (from s in _db.Salons where s.TransName.Equals(name) select s).FirstOrDefault().ID;
      }
      return View();
    }

    [HttpPost]
    public ActionResult PostAnswer()
    {
      using (SalonContext _db = new SalonContext())
      {
        try
        {
          int TargetSalonID = 0;
          int TargetCommentID = 0;
          int.TryParse(Request.Form["SalonID"], out TargetSalonID);
          int.TryParse(Request.Form["CommentID"], out TargetCommentID);
          SubComments model = new SubComments();
          model.CommentText = Request.Form["Text"];
          model.MainCommentID = TargetCommentID;
          model.MasterUserID = (from u in _db.Users where u.Email.Equals(HttpContext.User.Identity.Name) select u).FirstOrDefault().Id;
          model.PostDate = DateTime.Now;
          _db.SubComments.Add(model);
          _db.SaveChanges();
        }
        catch
        {
          return Redirect(Request.UrlReferrer.ToString());
        }
      }
      return Redirect(Request.UrlReferrer.ToString());

    }

    private ActionResult Redirect()
    {
      throw new NotImplementedException();
    }

    [HttpPost]
    public ActionResult SaveComment(string name)
    {
      using (SalonContext _db = new SalonContext())
      {
        try
        {
          CommentModel model = new CommentModel();
          model.Header = Request.Form["Header"];
          model.Text = Request.Form["Text"];
          model.Like = Request.Form["Like"].Equals("Yes");
          model.IsAllowedToShow = false;
          model.IsNew = true;
          model.MasterUserID = model.MasterUserID = (from u in _db.Users where u.Email == User.Identity.Name select u).FirstOrDefault().Id;
          model.PostDate = DateTime.Now;
          int TargetID = 0;
          int.TryParse(Request.Form["TargetSalonID"], out TargetID);
          model.SalonID = (from s in _db.Salons where s.ID == TargetID select s).FirstOrDefault().ID;
          _db.Comments.Add(model);
          _db.SaveChanges();
        }
        catch (DbEntityValidationException ex)
        {
          var error = ex.EntityValidationErrors.First().ValidationErrors.First();
          this.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
          return RedirectToAction("Index");
        }
      }
      return Redirect(Request.UrlReferrer.ToString());
    }

    [HttpPost]
    public ActionResult GetReviews()
    {
      return View();
    }

    public ActionResult ShowNews(int id)
    {
      using (SalonContext _db = new SalonContext())
      {
        ViewBag.id = id;
        ViewBag.PageName = _db.News.Find(id).Header;
      }
      return View();
    }

    public ActionResult ShowNewsByName(string name)
    {
      int NewsID = 1;
      using (SalonContext _db = new SalonContext())
      {
        NewsID = (from news in _db.News where news.TransName.Equals(name) select news).FirstOrDefault().ID;
      }
      ViewBag.id = NewsID;
      return View("~/Views/Home/ShowNews.cshtml");
      //return RedirectToAction("ShowNews", new { id = NewsID });
    }

    [AllowAnonymous]
    public ActionResult ContactUs()
    {
      ViewBag.PageName = "Связаться снами";

      return View();
    }

    [HttpPost]
    public ActionResult ContactUs(ContactUS model)
    {
      if (ModelState.IsValid)
      {

        using (SalonContext _db = new SalonContext())
        {
          model.PostDate = DateTime.Now;
          model.IsReaded = false;
          if (User.Identity.IsAuthenticated)
          {
            model.MasterUserID = (from u in _db.Users where u.Email == User.Identity.Name select u).FirstOrDefault().Id;
          }
          else
          {
            model.MasterUserID = null;
          }
          _db.FeedBacks.Add(model);
          _db.SaveChanges();
        }
        return PartialView("~/Views/Home/ContactUsResult.cshtml");
      }
      else
      {
        ModelState.AddModelError("", "Вы не заполнили одно из полей");
        return PartialView("~/Views/Home/ContactUsResult.cshtml", model);
      }
    }

    // return image by id
    public FileContentResult GetImage(int id)
    {
      using (SalonContext _db = new SalonContext())
      {
        ImagesModel Image = _db.Images.Find(id);
        if (Image != null)
        {
          return File(Image.ImageData, Image.ImageMimeType, Image.Name);
        }
        else
        {
          return null;
        }
      }
    }
  }
}
