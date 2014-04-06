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
  //Main web site context
  public class SalonContext : DbContext
  {
    public DbSet<SalonModel> Salons { get; set; }
    public DbSet<NewsModel> News { get; set; }
    public DbSet<CommentModel> Comments { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<ContactUS> FeedBacks { get; set; }
    public DbSet<SalonHitModel> SalonHits { get; set; }
    public DbSet<NewsHitModel> NewsHits { get; set; }
    public DbSet<ImagesModel> Images { get; set; }
    public DbSet<SalosAreasModel> Areas { get; set; }
    public DbSet<MapCoordsModel> MapCoords { get; set; }
    public DbSet<SalonImages> SalonImages { get; set; }
    public DbSet<SubComments> SubComments { get; set; }
    public DbSet<UsersInRoles> UsersInRoles { get; set; }
  }
  // User property Model
  public class User
  {
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string UnhashedPassword { get; set; }
    public DateTime CreationDate { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsBanned { get; set; }
    public int? RoleId { get; set; }
    public Role Role { get; set; }
  }
  // Role property
  public class Role
  {
    public int Id { get; set; }
    public string Name { get; set; }
  }
  public class UsersInRoles
  {
    public int id { get; set; }
    public int UserID { get; set; }
    public int RoleID { get; set; }
    public User User { get; set; }
    public Role Role { get; set; }

  }
  //Full salon model, with out date of creation
  [Table("Salons")]
  public class SalonModel
  {
    [Key]
    [Required]
    public int ID { get; set; }
    [Required(ErrorMessage = "необходимо ввести")]
    [Display(Name = "Название")]
    [StringLength(100, ErrorMessage = "Too long name")]
    public string Name { get; set; }
    [Display(Name = "название салона, транслитом (для поисковых систем и ссылок)")]
    public string TransName { get; set; }
    [Required(ErrorMessage = "необходимо ввести")]
    [System.Web.Mvc.AllowHtml]
    [Display(Name = "О салоне кратко")]
    [StringLength(540, ErrorMessage = "Too long about text")]
    public string About { get; set; }
    [Required(ErrorMessage = "необходимо ввести")]
    [System.Web.Mvc.AllowHtml]
    [Display(Name = "О салоне полностью")]
    [StringLength(3000, ErrorMessage = "Too long about text")]
    public string AboutFull { get; set; }

    [Required(ErrorMessage = "необходимо ввести")]
    [Display(Name = "Адрес")]
    [StringLength(150, ErrorMessage = "Too long addres")]
    public string Addres { get; set; }

    [Required(ErrorMessage = "необходимо ввести")]
    [Display(Name = "Время/режим работы, до 60 символов")]
    [StringLength(60, ErrorMessage = " слишком длинное описание")]
    public string TimeWorkingMode { get; set; }

    [Required(ErrorMessage = "необходимо ввести")]
    [Display(Name = "Контактный телефон")]
    [StringLength(18, ErrorMessage = "Too long phone number")]
    public string PhoneNamber { get; set; }

    [Required]
    public DateTime DateCreation { get; set; }

    [Required]
    public int? MasterUserID { get; set; }
    public User MasterUser { get; set; }
    //Ссылка на координаты области.
    [Display(Name = "Город или область салона")]
    public int? SalonAreaID { get; set; }
    public SalosAreasModel SalonArea { get; set; }

    //Ссылка на координаты адреса.		
    public int? SalonCoordsID { get; set; }
    public MapCoordsModel SalonCoords { get; set; }
  }
  //Comment's
  [Table("Comments")]
  public class CommentModel
  {
    [Key]
    public int ID { get; set; }
    //Allowto show

    public bool IsNew { get; set; }
    public bool IsAllowedToShow { get; set; }
    public int? MasterUserID { get; set; }
    public User User { get; set; }
    public int? SalonID { get; set; }
    public SalonModel SalonModel { get; set; }
    public bool Like { get; set; }
    public DateTime PostDate { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Too long header")]
    public string Header { get; set; }
    [StringLength(3000, ErrorMessage = "Too long Comment text")]
    public string Text { get; set; }

  }
  [Table("News")]
  public class NewsModel
  {
    [Key]
    public int ID { get; set; }
    public bool IsAllowed { get; set; }
    public DateTime CreationTime { get; set; }
    public int? MasterUserID { get; set; }
    public User MasterUser { get; set; }
    [Required(ErrorMessage = "необходимо ввести")]
    public string Header { get; set; }
    [Display(Name = "название салона, транслитом (для поисковых систем и ссылок)")]
    public string TransName { get; set; }
    [Required(ErrorMessage = "необходимо ввести")]
    [System.Web.Mvc.AllowHtml]
    public string PreViewText { get; set; }
    [Required(ErrorMessage = "необходимо ввести")]
    [System.Web.Mvc.AllowHtml]
    public string FullText { get; set; }
    public int? FullImageID { get; set; }
    public ImagesModel FullImage { get; set; }
    public int? PreviewImageID { get; set; }
    public ImagesModel PreviewImage { get; set; }
  }
  [Table("Images")]
  public class ImagesModel
  {
    [Key]
    public int ID { get; set; }
    // File raw data
    public byte[] ImageData { get; set; }
    //Image content type
    public string ImageMimeType { get; set; }
    [Required]
    [Display(Name = "название картинки, латиницей")]
    public string Name { get; set; }
    [Required]
    [Display(Name = "Замещающая надпись")]
    public string Alt { get; set; }
    [Required]
    public DateTime CreationDate { get; set; }

    public bool IsAllowed { get; set; }

    public int? MasterUserID { get; set; }
    public User MasterUser { get; set; }
  }
  [Table("Areas")]
  public class SalosAreasModel
  {
    [Key]
    public int ID { get; set; }
    public string Name { get; set; }
    public int? MapCoordsID { get; set; }
    public MapCoordsModel MapCoords { get; set; }
    [Required]
    [System.Web.Mvc.AllowHtml]
    public string About { get; set; }
    public int? MasterUserID { get; set; }
    public User MasterUser { get; set; }
  }
  [Table("MapCoords")]
  public class MapCoordsModel
  {
    [Key]
    public int ID { get; set; }
    //coordinates data for google or yandex maps
    public double X { get; set; }
    public double Y { get; set; }
    public int zoom { get; set; }
  }
  [Table("SalonHits")]
  public class SalonHitModel
  {
    [Key]
    public int Id { get; set; }
    public int? SalonID { get; set; }
    public SalonModel SalonModel { get; set; }
    public DateTime VisitDate { get; set; }
    public bool IsAuthorised { get; set; }
  }
  [Table("NewsHits")]
  public class NewsHitModel
  {
    [Key]
    public int Id { get; set; }
    public int? NewsID { get; set; }
    public NewsModel NewsModel { get; set; }
    public DateTime VisitDate { get; set; }
    public bool IsAuthorised { get; set; }
  }
  [Table("SalonImages")]
  public class SalonImages
  {
    [Key]
    public int Id { get; set; }
    public int? SalonID { get; set; }
    public SalonModel Salon { get; set; }
    public int? FullImageID { get; set; }
    public ImagesModel FullImage { get; set; }
  }
  [Table("SubComments")]
  public class SubComments
  {
    [Key]
    public int id { get; set; }
    [Required]
    public int? MainCommentID { get; set; }
    public DateTime PostDate { get; set; }
    [Required(ErrorMessage = "необходимо ввести")]
    [StringLength(4000, ErrorMessage = "Too long sub comment")]
    public string CommentText { get; set; }
    public int MasterUserID { get; set; }
    public User MasterUser { get; set; }
  }


}
