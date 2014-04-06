using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Helpers;
using System.Security.Cryptography;
using System.Web.WebPages;
using Microsoft.Internal.Web.Utils;
using AvtoMnenie.Models;

namespace AvtoMnenie.Providers
{
  public class CustomRoleProvider : RoleProvider
  {
    public override string[] GetRolesForUser(string email)
    {
      string[] role = new string[] { };
      using (SalonContext _db = new SalonContext())
      {
        try
        {
          // Получаем пользователя
          User user = (from u in _db.Users
                       where u.Email == email
                       select u).FirstOrDefault();
          if (user != null)
          {
            // получаем роль
            Role userRole = _db.Roles.Find(user.RoleId);

            if (userRole != null)
            {
              role = new string[] { userRole.Name };
            }
          }
        }
        catch
        {
          role = new string[] { };
        }
      }
      return role;
    }
    public override void CreateRole(string roleName)
    {
      Role newRole = new Role() { Name = roleName };
      SalonContext db = new SalonContext();
      db.Roles.Add(newRole);
      db.SaveChanges();
    }
    public override bool IsUserInRole(string username, string roleName)
    {
      bool outputResult = false;
      // Находим пользователя
      using (SalonContext _db = new SalonContext())
      {
        try
        {
          // Получаем пользователя
          User user = (from u in _db.Users
                       where u.Email == username
                       select u).FirstOrDefault();
          if (user != null)
          {
            int UserRolesCount = (from uir in _db.UsersInRoles where 
                                      uir.UserID == user.Id && 
                                      (from role in _db.Roles where 
                                          role.Id == uir.RoleID &&
                                          role.Name.Equals(roleName) select role).Count() > 0 
                                  select uir).Count();
            // проверим, имеет ли юзер соответствующую роль.
            if (UserRolesCount > 0) outputResult = true;
            #region old method
            // получаем роль
            /*
            Role userRole = _db.Roles.Find(user.RoleId);

            //сравниваем
            if (userRole != null && userRole.Name == roleName)
            {
              outputResult = true;
            }
             */
            #endregion
          }
        }
        catch
        {
          outputResult = false;
        }
      }
      return outputResult;
    }
    public override void AddUsersToRoles(string[] usernames, string[] roleNames)
    {
      throw new NotImplementedException();
    }

    public override string ApplicationName
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
    {
      throw new NotImplementedException();
    }

    public override string[] FindUsersInRole(string roleName, string usernameToMatch)
    {
      throw new NotImplementedException();
    }

    public override string[] GetAllRoles()
    {
      throw new NotImplementedException();
    }

    public override string[] GetUsersInRole(string roleName)
    {
      throw new NotImplementedException();
    }

    public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
    {
      throw new NotImplementedException();
    }

    public override bool RoleExists(string roleName)
    {
      throw new NotImplementedException();
    }
  }
}