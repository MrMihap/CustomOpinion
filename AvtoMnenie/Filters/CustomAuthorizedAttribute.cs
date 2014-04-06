using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity;
using AvtoMnenie.Models;
using System.Security;

namespace AvtoMnenie.Filters
{
  public class CustomAuthorizedAttribute : AuthorizeAttribute
  {
    public string[] allowedRoles;

    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
      if (httpContext.Request.IsAuthenticated && Role(httpContext)) return true;
      return false;
    }
    private bool Role(HttpContextBase httpContext)
    {
      using (SalonContext _db = new SalonContext())
        if (allowedRoles.Length > 0)
        {

          //allowedRoles = allowedRoles.Split('');
          foreach (string rolename in allowedRoles)
          {
            //if (httpContext.User.IsInRole(allowedRoles[i]))
            string TargetUserName = httpContext.User.Identity.Name;
            int TargetUserID;
            User TargetUser = (from u in _db.Users
                                where u.Email.Equals(TargetUserName)
                                select u).FirstOrDefault();
            if (TargetUser == null)
            {
              return false;
            }
            else
            {
              TargetUserID = TargetUser.Id;
            }
            if ((from uir in _db.UsersInRoles
                 join role in _db.Roles on uir.RoleID equals role.Id
                 where uir.UserID == TargetUserID && role.Name.Equals(rolename)
                 select role).Count() > 0)
              return true;
          }
          return false;
        }
      return false;
    }
    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
      filterContext.Result = new RedirectToRouteResult(new
            RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
      //base.HandleUnauthorizedRequest(filterContext);
    }
  }
}