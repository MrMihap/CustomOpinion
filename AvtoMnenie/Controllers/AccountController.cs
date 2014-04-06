using System;
using System.Web;
using System.Web.Mvc;
using AvtoMnenie.Models;
using System.Web.Security;
using AvtoMnenie.Providers;
namespace AvtoMnenie.Controllers
{
  [AllowAnonymous]
  public class AccountController : Controller
  {
    public ActionResult Login()
    {
      return View();
    }
    [HttpPost]
    public ActionResult Login(logOnModel model, string returnUrl)
    {
      if (ModelState.IsValid)
      {
        if (Membership.ValidateUser(model.UserName, model.Password))
        {
          ViewBag.IsLogginIn = true;
          FormsAuthentication.SetAuthCookie(model.UserName, true);
          if (Url.IsLocalUrl(returnUrl))
          {
            return PartialView(model);
          }
          else
          {
            return PartialView(model);
          }
        }
        else
        {
          ViewBag.IsLogginIn = false;
          ModelState.AddModelError("", "Неправильный пароль или логин");
        }
      }
      return View(model);
    }
    public ActionResult LogOff()
    {
      FormsAuthentication.SignOut();

      return RedirectToAction("index", "Home");
    }
    public ActionResult Register()
    {
      return View();
    }
    [HttpPost]
    public ActionResult Register(RegisterModel model)
    {
      if (ModelState.IsValid)
      {
        MembershipUser membershipUser = ((CustomMembershipProvider)Membership.Provider).CreateUser(model.Email, model.Password, model.Name);

        if (membershipUser != null)
        {
          //TODO:
          //Block auto auth, send it only if user is varificated by email 
          
          FormsAuthentication.SetAuthCookie(model.Email, true);
          System.TimeSpan x = FormsAuthentication.Timeout;
          //new EmailController().SendEmail(model).Deliver(;)
          ViewBag.IsRegistered = true;
          return PartialView(model);
        }
        else
        {
          ModelState.AddModelError("", "Ошибка при регистрации");
          return PartialView(model);
        }
      }
      return PartialView(model);
    }
  }
}
