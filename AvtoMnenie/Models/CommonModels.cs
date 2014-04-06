using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Web.Mvc;
using AvtoMnenie.Models;
namespace AvtoMnenie.Models
{
  public class ContactUS
  {
    [Key]
    public int ID { get; set; }

    //If it was created By Some User
    public int? MasterUserID { get; set; }
    public User User { get; set; }

    // Date of Contact Messege Creation
    public DateTime PostDate { get; set; }
    //UserEmail
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    // Message header
    [Required(ErrorMessage = "вы не ввели имя")]
    [StringLength(100, ErrorMessage = "Too long header")]
    public string Name { get; set; }
    // Message text
    [Required(ErrorMessage = "необходимо написать сообщение")]
    [StringLength(3000, ErrorMessage = "Too long Comment text")]
    public string Text { get; set; }
    //Status
    public bool IsReaded { get; set; }
  }
  public class CreateNewsModel
  {
    [Required]
    [Display(Name = "Заголовок новости")]
    [DataType(DataType.Text)]
    public string Header { get; set; }
    [Required]
    [Display(Name = "название салона, транслитом (для поисковых систем и ссылок)")]
    public string TransName { get; set; }
    // File name in location dir
    [Required]
    [Display(Name = "название картинки, латиницей")]
    public string Name { get; set; }

    //Alt teg for picture
    [Required]
    [Display(Name = "Замещающая надпись")]
    public string Alt { get; set; }

    //Preview text of news wich may be showed on main page
    [Required]
    [Display(Name = "Краткое соержание новости")]
    [System.Web.Mvc.AllowHtml]
    public string PreViewText { get; set; }

    //Full news text html code inside
    [Required]
    [Display(Name = "Полный текст новости")]
    [System.Web.Mvc.AllowHtml]
    public string FullText { get; set; }
  }
  public class logOnModel
  {
    [Required]
    [Display(Name = "Почта")]
    public string UserName { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; }

    [Display(Name = "Запомнить")]
    public bool RememberMe { get; set; }
  }
  //Is shown on login page

  //Registration Model
  //Include all properys with out of DateCreation & user Role
  public class RegisterModel
  {
    [Required]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Электронная почта")]
    public string Email { get; set; }
    [Required]
    [StringLength(40, ErrorMessage = "Слишком длинное имя")]
    [Display(Name = "Ваше полное имя")]
    public string Name { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "Пароль должен иметь от 6 до 100 символов", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Подтвердить пароль")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
    public string ConfirmPassword { get; set; }
  }
  public class NavigationListModel
  {
    public List<string> LinkText;
    public List<string> ActionName;
    public List<string> ControllerName;
    public string TargetName;
    public NavigationListModel()
    {
      LinkText = new List<string>();
      ActionName = new List<string>();
      ControllerName = new List<string>();
      TargetName = "";
    }
    public NavigationListModel(string _TargetName)
    {
      LinkText = new List<string>();
      ActionName = new List<string>();
      ControllerName = new List<string>();
      TargetName = _TargetName;
    }
  }
}