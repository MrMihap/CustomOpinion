using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AvtoMnenie.Controllers
{
    public class ErrorController : Controller
    {
      public ActionResult AccessDenied()
      {
        return View();
      }
      public ActionResult PageNotFount()
      {
        return View();
      }
    }
}
