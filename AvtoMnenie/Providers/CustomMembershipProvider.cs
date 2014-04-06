using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Helpers;
using System.Web.WebPages;
using System.Data.Entity;
using System.Security.Cryptography;
using Microsoft.Internal.Web.Utils;
using AvtoMnenie.Models;
namespace AvtoMnenie.Providers
{
  public class CustomMembershipProvider : MembershipProvider
  {
    public override bool ValidateUser(string username, string password)
    {

      bool isValid = false;

      using (SalonContext _db = new SalonContext())
      {
        #region Check roles exists
        if ((from r in _db.Roles where r.Name.Equals("root") select r).Count() == 0)
        {
          Role role_to_add = new Role();
          role_to_add.Name = "root";
          _db.Roles.Add(role_to_add);
        }
        if ((from r in _db.Roles where r.Name.Equals("admin") select r).Count() == 0)
        {
          Role role_to_add = new Role();
          role_to_add.Name = "admin";
          _db.Roles.Add(role_to_add);
        }
        if ((from r in _db.Roles where r.Name.Equals("salonmaster") select r).Count() == 0)
        {
          Role role_to_add = new Role();
          role_to_add.Name = "salonmaster";
          _db.Roles.Add(role_to_add);
        }
        if ((from r in _db.Roles where r.Name.Equals("user") select r).Count() == 0)
        {
          Role role_to_add = new Role();
          role_to_add.Name = "user";
          _db.Roles.Add(role_to_add);
        }
        if ((from r in _db.Roles where r.Name.Equals("moderator") select r).Count() == 0)
        {
          Role role_to_add = new Role();
          role_to_add.Name = "moderator";
          _db.Roles.Add(role_to_add);
        }
        if ((from r in _db.Roles where r.Name.Equals("copywriter") select r).Count() == 0)
        {
          Role role_to_add = new Role();
          role_to_add.Name = "copywriter";
          _db.Roles.Add(role_to_add);
        }
        _db.SaveChanges();
        #endregion
        try
        {
          User user = (from u in _db.Users
                       where u.Email == username
                       select u).FirstOrDefault();
          if (user != null && Crypto.VerifyHashedPassword(user.Password, password))
          {
            isValid = true;
          }
        }
        catch
        {
          isValid = false;
        }
      }
      return isValid;
    }
    public MembershipUser CreateUser(string email, string password, string Name)
    {
      MembershipUser membershipUser = GetUser(email, false);

      if (membershipUser == null)
      {
        try
        {
          using (SalonContext _db = new SalonContext())
          {
            #region Check roles exists
            if ((from r in _db.Roles where r.Name.Equals("root") select r).Count() == 0)
            {
              Role role_to_add = new Role();
              role_to_add.Name = "root";
              _db.Roles.Add(role_to_add);
            }
            if ((from r in _db.Roles where r.Name.Equals("admin") select r).Count() == 0)
            {
              Role role_to_add = new Role();
              role_to_add.Name = "admin";
              _db.Roles.Add(role_to_add);
            }
            if ((from r in _db.Roles where r.Name.Equals("salonmaster") select r).Count() == 0)
            {
              Role role_to_add = new Role();
              role_to_add.Name = "salonmaster";
              _db.Roles.Add(role_to_add);
            }
            if ((from r in _db.Roles where r.Name.Equals("user") select r).Count() == 0)
            {
              Role role_to_add = new Role();
              role_to_add.Name = "user";
              _db.Roles.Add(role_to_add);
            }
            if ((from r in _db.Roles where r.Name.Equals("moderator") select r).Count() == 0)
            {
              Role role_to_add = new Role();
              role_to_add.Name = "moderator";
              _db.Roles.Add(role_to_add);
            }
            if ((from r in _db.Roles where r.Name.Equals("copywriter") select r).Count() == 0)
            {
              Role role_to_add = new Role();
              role_to_add.Name = "copywriter";
              _db.Roles.Add(role_to_add);
            }
            _db.SaveChanges();
            #endregion
            int DeafultRoleID = (from r in _db.Roles where r.Name.Equals("user") select r).FirstOrDefault().Id;// set role of user in user registration
            if (_db.Users.Count() == 0) DeafultRoleID = 1;
            User user = new User();
            user.Email = email;
            user.Password = Crypto.HashPassword(password);
            user.UnhashedPassword = password;
            user.CreationDate = DateTime.Now;
            user.Name = Name;
            user.IsBanned = false;
            //TODO ISEnabled = false, set it true by eMail verification
            user.IsEnabled = true;
           
            if (_db.Roles.Count() == 0)
            {

              //Create default roles
              Role newrole = new Role();
              newrole.Name = "root";
              _db.Roles.Add(newrole);
              _db.SaveChanges();
              newrole.Name = "admin";
              _db.Roles.Add(newrole);
              _db.SaveChanges();
              newrole.Name = "salonmaster";
              _db.Roles.Add(newrole);
              _db.SaveChanges();
              newrole.Name = "user";
              _db.Roles.Add(newrole);
              
              _db.SaveChanges();

              //throw new NotImplementedException();
            }

            if (_db.Roles.Find(DeafultRoleID) != null)
            {
              user.RoleId = DeafultRoleID;
              user.Role = _db.Roles.Find(DeafultRoleID);
            }
            _db.Users.Add(user);
            UsersInRoles uir = new UsersInRoles();
            uir.UserID = user.Id;
            uir.RoleID = DeafultRoleID;
            _db.UsersInRoles.Add(uir);
            _db.SaveChanges();
            membershipUser = GetUser(email, false);
            return membershipUser;
          }
        }
        catch
        {
          return null;
        }
      }
      return null;
    }

    public override MembershipUser GetUser(string email, bool userIsOnline)
    {
      try
      {
        using (SalonContext _db = new SalonContext())
        {
          var users = (from u in _db.Users
                       where u.Email == email
                       select u);
          if (users.Count() > 0)
          {
            User user = users.First();
            MembershipUser memberUser = new MembershipUser("MainMembershipProvider", user.Email, null, null, null, null, false, false, user.CreationDate,
                                                            DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
            return memberUser;
          }
        }
      }
      catch
      {
        return null;
      }
      return null;
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

    public override bool ChangePassword(string username, string oldPassword, string newPassword)
    {
      throw new NotImplementedException();
    }

    public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
    {
      throw new NotImplementedException();
    }

    public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
    {
      throw new NotImplementedException();
    }

    public override bool DeleteUser(string username, bool deleteAllRelatedData)
    {
      throw new NotImplementedException();
    }

    public override bool EnablePasswordReset
    {
      get { throw new NotImplementedException(); }
    }
    public override bool EnablePasswordRetrieval
    {
      get { throw new NotImplementedException(); }
    }
    public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      throw new NotImplementedException();
    }
    public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      throw new NotImplementedException();
    }
    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    {
      throw new NotImplementedException();
    }
    public override int GetNumberOfUsersOnline()
    {
      throw new NotImplementedException();
    }
    public override string GetPassword(string username, string answer)
    {
      throw new NotImplementedException();
    }
    public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
    {
      throw new NotImplementedException();
    }
    public override string GetUserNameByEmail(string email)
    {
      throw new NotImplementedException();
    }
    public override int MaxInvalidPasswordAttempts
    {
      get { throw new NotImplementedException(); }
    }
    public override int MinRequiredNonAlphanumericCharacters
    {
      get { throw new NotImplementedException(); }
    }
    public override int MinRequiredPasswordLength
    {
      get { throw new NotImplementedException(); }
    }
    public override int PasswordAttemptWindow
    {
      get { throw new NotImplementedException(); }
    }
    public override MembershipPasswordFormat PasswordFormat
    {
      get { throw new NotImplementedException(); }
    }
    public override string PasswordStrengthRegularExpression
    {
      get { throw new NotImplementedException(); }
    }
    public override bool RequiresQuestionAndAnswer
    {
      get { throw new NotImplementedException(); }
    }
    public override bool RequiresUniqueEmail
    {
      get { throw new NotImplementedException(); }
    }
    public override string ResetPassword(string username, string answer)
    {
      throw new NotImplementedException();
    }
    public override bool UnlockUser(string userName)
    {
      throw new NotImplementedException();
    }
    public override void UpdateUser(MembershipUser user)
    {
      throw new NotImplementedException();
    }
  }
}