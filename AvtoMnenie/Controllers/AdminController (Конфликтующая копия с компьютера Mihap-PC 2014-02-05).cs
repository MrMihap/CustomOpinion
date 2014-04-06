using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Validation;
using AvtoMnenie.Models;
using AvtoMnenie.Filters;
namespace AvtoMnenie.Controllers
{
  [CustomAuthorized(allowedRoles = new string[] { "admin", "root" })]
  public class AdminController : Controller
  {
    // GET: /Admin/

    public ActionResult Index()
    {
      return View();
    }
    #region UserHelpers
    public ActionResult UserManagment()
    {
      SalonContext users = new SalonContext();
      ViewBag.UsersBag = users.Users.ToList();
      return View();
    }
    [HttpPost]
    public ActionResult EditUserRoles()
    {
      AvtoMnenie.Models.User UserContext = null;

      using (SalonContext _db = new SalonContext())
      {
        int TargetUserID = 0;
        int TargetRoleID = 0;
        string ActionType = Request.Form["ActionType"];

        int.TryParse(Request.Form["UserID"], out TargetUserID);
        int.TryParse(Request.Form["RoleID"], out TargetRoleID);
        // Проверка на целостность данных + защита от левых запросов
        if (TargetUserID * TargetRoleID > 0 && Request.IsAuthenticated)
        {
          UserContext = _db.Users.Find(TargetUserID);
          switch (ActionType)
          {
            case "Add":
              //Добавить роль
              UsersInRoles uir = new UsersInRoles();
              uir.RoleID = TargetRoleID;
              uir.UserID = TargetUserID;
              _db.UsersInRoles.Add(uir);
              _db.SaveChanges();
              break;
            case "Remove":
              //Удалить роль
              UsersInRoles uir_to_remove = (from u in _db.UsersInRoles
                                            where u.UserID == TargetUserID && u.RoleID == TargetRoleID
                                            select u).FirstOrDefault();
              _db.UsersInRoles.Remove(_db.UsersInRoles.Find(uir_to_remove.id));
              _db.SaveChanges();
              break;
          }
        }
      }
      return PartialView("~/Views/Admin/_UserRolesEditPartial.cshtml", UserContext);
    }

    public ActionResult BannUser(int id)
    {
      using (SalonContext _db = new SalonContext())
      {
        User user_to_bann = _db.Users.Find(id);
        user_to_bann.IsBanned = true;
        _db.SaveChanges();
      }
      return RedirectToAction("UserManagment");
    }
    #endregion
    #region Salon Actions
    public ActionResult SalonManagment()
    {
      return View();
    }
    public ActionResult CreateSalon()
    {
      return View();
    }
    public ActionResult DeleteSalon(int id)
    {
      using (SalonContext _db = new SalonContext())
      {
        _db.Salons.Remove(_db.Salons.Find(id));
        _db.SaveChanges();
      }
      return RedirectToAction("SalonManagment");
    }
    [HttpGet]
    public ActionResult EditSalon(int id)
    {
      ViewBag.id = id;
      ;
      using (SalonContext _db = new SalonContext())
      {
        var salonModel = (from salon in _db.Salons where salon.ID == id select salon).FirstOrDefault();
        return View(salonModel);
      }

    }
    public ActionResult EditSalon(int id, SalonModel model)
    {
      using (SalonContext _db = new SalonContext())
      {
        try
        {
          SalonModel salon_to_update = (from salons in _db.Salons where salons.ID == id select salons).FirstOrDefault();
          salon_to_update.About = model.About;
          salon_to_update.Addres = model.Addres;
          salon_to_update.Name = model.Name;
          salon_to_update.TransName = model.TransName;
          salon_to_update.PhoneNamber = model.PhoneNamber;
          salon_to_update.SalonAreaID = model.SalonAreaID;
          salon_to_update.TimeWorkingMode = model.TimeWorkingMode;
          salon_to_update.AboutFull = model.AboutFull;
          _db.SaveChanges();
        }
        catch (DbEntityValidationException ex)
        {
          var error = ex.EntityValidationErrors.First().ValidationErrors.First();
          this.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
          return RedirectToAction("Index");
        }
      }
      return RedirectToAction("EditSalon/" + id.ToString(), "Admin");
    }
    [HttpPost]
    public ActionResult CreateSalon(SalonModel model)
    {
      using (SalonContext _db = new SalonContext())
      {
        try
        {

          model.DateCreation = DateTime.Now;

          model.SalonArea = _db.Areas.Find(model.SalonAreaID);
          model.SalonCoords = new MapCoordsModel();
          model.SalonCoordsID = 0;
          if (Request.IsAuthenticated)
          {
            model.MasterUserID = (from u in _db.Users where u.Email == User.Identity.Name select u).FirstOrDefault().Id;
            model.MasterUser = (from u in _db.Users where u.Email == User.Identity.Name select u).FirstOrDefault();
          }
          else
          {
            model.MasterUserID = (from u in _db.Users where u.RoleId == 1 select u).FirstOrDefault().Id;
            model.MasterUser = (from u in _db.Users where u.RoleId == 1 select u).FirstOrDefault();
          }

          _db.Salons.Add(model);
          _db.SaveChanges();
        }
        catch (DbEntityValidationException ex)
        {
          var error = ex.EntityValidationErrors.First().ValidationErrors.First();
          this.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
          return RedirectToAction("Index");
        }
      }
      return RedirectToAction("SalonManagment");
    }
    public ActionResult CreateCoordsForSalon(int id)
    {
      ViewBag.id = id;
      return View();
    }
    [HttpPost]
    public ActionResult CreateCoordsForSalon(MapCoordsModel model, int id)
    {
      using (SalonContext _db = new SalonContext())
      {
        model.X = model.X / 100000;
        model.Y /= 100000;
        _db.MapCoords.Add(model);
        _db.SaveChanges();
        SalonModel salon = _db.Salons.Find(id);
        salon.SalonCoordsID = model.ID;
        _db.SaveChanges();
      }

      return RedirectToAction("SalonManagment");
    }
    #endregion
    #region Area Actions
    public ActionResult AreasManagment()
    {
      return View();
    }
    [HttpGet]
    public ActionResult CreateArea()
    {
      return View();
    }
    [HttpPost]
    public ActionResult CreateArea(SalosAreasModel model)
    {
      using (SalonContext _db = new SalonContext())
      {
        //TODO insert here
        model.MapCoordsID = null;
        model.MapCoords = null;
        //model User Hoster
        model.MasterUserID = (from u in _db.Users where u.Email == User.Identity.Name select u).FirstOrDefault().Id;
        model.MasterUser = (from u in _db.Users where u.Email == User.Identity.Name select u).FirstOrDefault();
        //add to dB
        _db.Areas.Add(model);
        _db.SaveChanges();
      }
      return RedirectToAction("");
    }
    #endregion
    #region CommentModeration
    public ActionResult CommentManagment()
    {
      return View();
    }
    [HttpPost]
    public ActionResult ComentModeration()
    {
      if (Request.Form["ModerationType"].Equals("New"))
        return PartialView("~/Views/Admin/ComModByNew.cshtml");
      if (Request.Form["ModerationType"].Equals("BySalons"))
        return PartialView("~/Views/Admin/ComModByNew.cshtml");
      if (Request.Form["ModerationType"].Equals("ByTime"))
        return PartialView("~/Views/Admin/ComModByNew.cshtml");
      return null;
    }
    [HttpPost]
    public ActionResult EditCommentRooles()
    {
      CommentModel comment = null;
      using (SalonContext _db = new SalonContext())
      {
        int comID = 0;
        int.TryParse(Request.Form["CommentID"], out comID);
        if (comID == 0)
        {
          return null;
        }
        else
        {
          comment = (from com in _db.Comments where com.ID == comID select com).FirstOrDefault();
        }
        if (Request.Form["ActionType"].Equals("None"))
        {
        }
        if (Request.Form["ActionType"].Equals("Allow"))
        {
          comment.IsNew = false;
          comment.IsAllowedToShow = true;
        }
        if (Request.Form["ActionType"].Equals("DisAllow"))
        {
          comment.IsNew = false;
          comment.IsAllowedToShow = false;
        }
        _db.SaveChanges();
      }
      return PartialView("~/Views/AdminCommon/CommentAllowDisAllow.cshtml", comment);
    }
    #endregion
    #region NewsHelpers
    [HttpGet]
    public ActionResult CreateNews()
    {
      return View();
    }
    [HttpPost]
    public ActionResult CreateNews(CreateNewsModel model, HttpPostedFileBase image)
    {
      using (SalonContext _db = new SalonContext())
      {
        //Add Image to DataBase, at first
        ImagesModel new_image = new ImagesModel();
        new_image.Alt = model.Alt;
        new_image.CreationDate = DateTime.Now;
        new_image.ImageMimeType = image.ContentType;
        new_image.ImageData = new byte[image.ContentLength];
        new_image.IsAllowed = true;
        new_image.Name = model.Name;
        new_image.MasterUserID = (from u in _db.Users where u.Email == User.Identity.Name select u).FirstOrDefault().Id;
        new_image.MasterUser = (from u in _db.Users where u.Email == User.Identity.Name select u).FirstOrDefault();
        //read from filestream to the model data field
        image.InputStream.Read(new_image.ImageData, 0, image.ContentLength);
        _db.Images.Add(new_image);
        //Finaly, image added to DB
        _db.SaveChanges();

        try
        {
          //Start Add News to DB
          NewsModel new_news = new NewsModel();
          //new_news = model;

          new_news.CreationTime = DateTime.Now;
          new_news.Header = model.Header;
          new_news.PreViewText = model.PreViewText;
          new_news.FullText = model.FullText;
          new_news.FullImageID = new_image.ID;
          new_news.FullImage = _db.Images.Find(new_image.ID);
          if (Request.IsAuthenticated)
          {
            new_news.MasterUserID = (from u in _db.Users where u.Email == User.Identity.Name select u).FirstOrDefault().Id;
            new_news.MasterUser = (from u in _db.Users where u.Email == User.Identity.Name select u).FirstOrDefault();
          }
          new_news.PreviewImageID = new_news.FullImageID;
          new_news.PreviewImage = new_news.FullImage;

          _db.News.Add(new_news);
          _db.SaveChanges();
        }
        catch (DbEntityValidationException ex)
        {
          ModelState.AddModelError("", ex.Message);
          throw new DbEntityValidationException();
        }
      }
      return RedirectToAction("NewsManagment");
    }
    [HttpGet]
    public ActionResult EditNews(int id)
    {
      ViewBag.id = id;
      return View();
    }
    [HttpPost]
    public ActionResult EditNews(int id, CreateNewsModel model, HttpPostedFileBase image)
    {
      using (SalonContext _db = new SalonContext())
      {
        //Get News elem
        NewsModel news_to_edit = (from n in _db.News where n.ID == id select n).FirstOrDefault();
        //updateNewsElem
        news_to_edit.CreationTime = DateTime.Now;
        news_to_edit.Header = model.Header;
        news_to_edit.TransName = model.TransName;
        news_to_edit.PreViewText = model.PreViewText;
        news_to_edit.FullText = model.FullText;
        news_to_edit.FullImageID = (from imageElems in _db.Images orderby imageElems.CreationDate descending select imageElems).First().ID;
        news_to_edit.FullImage = (from imageElems in _db.Images orderby imageElems.CreationDate descending select imageElems).First();
        if (Request.IsAuthenticated)
        {
          news_to_edit.MasterUserID = (from u in _db.Users where u.Email == User.Identity.Name select u).FirstOrDefault().Id;
          news_to_edit.MasterUser = (from u in _db.Users where u.Email == User.Identity.Name select u).FirstOrDefault();
        }

        _db.SaveChanges();
        //UpdateImage
        if (image != null && news_to_edit.FullImageID != null)
        {
          ImagesModel image_to_edit = (from img in _db.Images where img.ID == news_to_edit.FullImageID select img).FirstOrDefault();
          image_to_edit.Alt = model.Alt;
          image_to_edit.CreationDate = DateTime.Now;
          image_to_edit.ImageMimeType = image.ContentType;
          image_to_edit.ImageData = new byte[image.ContentLength];
          image_to_edit.IsAllowed = true;
          image_to_edit.Name = model.Name;
          image_to_edit.MasterUserID = (from u in _db.Users where u.Email == User.Identity.Name select u).FirstOrDefault().Id;
          image_to_edit.MasterUser = (from u in _db.Users where u.Email == User.Identity.Name select u).FirstOrDefault();
          //read from filestream to the model data field
          image.InputStream.Read(image_to_edit.ImageData, 0, image.ContentLength);
          _db.SaveChanges();
        }
        //Add image to DB and 
        if (image != null && news_to_edit.FullImageID == null)
        {
          ImagesModel new_image = new ImagesModel();
          new_image.Alt = model.Alt;
          new_image.CreationDate = DateTime.Now;
          new_image.ImageMimeType = image.ContentType;
          new_image.ImageData = new byte[image.ContentLength];
          new_image.IsAllowed = true;
          new_image.Name = model.Name;
          new_image.MasterUserID = (from u in _db.Users where u.Email == User.Identity.Name select u).FirstOrDefault().Id;
          new_image.MasterUser = (from u in _db.Users where u.Email == User.Identity.Name select u).FirstOrDefault();
          //read from filestream to the model data field
          image.InputStream.Read(new_image.ImageData, 0, image.ContentLength);
          _db.Images.Add(new_image);
          news_to_edit.FullImageID = (from imageElems in _db.Images orderby imageElems.CreationDate descending select imageElems).First().ID;
          news_to_edit.FullImage = (from imageElems in _db.Images orderby imageElems.CreationDate descending select imageElems).First();
          news_to_edit.PreviewImageID = news_to_edit.FullImageID;
          news_to_edit.PreviewImage = news_to_edit.FullImage;
          //Finaly, image added to DB
          _db.SaveChanges();
        }

      }
      return RedirectToAction("EditNews/" + id.ToString(), "Admin");
    }
    [Authorize(Roles = "root")]
    public ActionResult DeleteNews(int id)
    {
      using (SalonContext _db = new SalonContext())
      {
        _db.News.Remove(_db.News.Find(id));
        _db.SaveChanges();
      }
      return RedirectToAction("NewsManagment");
    }
    public ActionResult NewsManagment()
    {
      return View();
    }
    public ActionResult BlockNews(int id, string backurl)
    {
      using (SalonContext _db = new SalonContext())
      {
        NewsModel news_to_block = (from news in _db.News where news.ID == id select news).FirstOrDefault();
        news_to_block.IsAllowed = false;
        _db.SaveChanges();
      }
      return Redirect(System.Web.HttpContext.Current.Request.UrlReferrer.ToString());
    }
    public ActionResult ActivateNews(int id, string backurl)
    {
      using (SalonContext _db = new SalonContext())
      {
        NewsModel news_to_activate = (from news in _db.News where news.ID == id select news).FirstOrDefault();
        news_to_activate.IsAllowed = true;
        _db.SaveChanges();
      }
      return Redirect(System.Web.HttpContext.Current.Request.UrlReferrer.ToString());
    }
    #endregion
    #region FeedBack_Helpers
    public ActionResult FeedBacks()
    {
      return View();
    }
    public ActionResult SetFeedBackAsReaded(int id)
    {
      using (SalonContext _db = new SalonContext())
      {
        ContactUS feedback = _db.FeedBacks.Find(id);
        feedback.IsReaded = true;
        _db.SaveChanges();
      }
      return RedirectToAction("FeedBacks");
    }
    public ActionResult SetFeedBackAsUnReaded(int id)
    {
      using (SalonContext _db = new SalonContext())
      {
        ContactUS feedback = _db.FeedBacks.Find(id);
        feedback.IsReaded = false;
        _db.SaveChanges();
      }
      return RedirectToAction("FeedBacks");
    }
    #endregion
  }
}
