using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace QuestionsAndAnswers.Models
{

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }

        [Email(ErrorMessageResourceName = "EmailNotValid", ErrorMessageResourceType = typeof(Resources.Global))]
        public string Email { get; set; }
        public bool? IsVerified { get; set; }
        public virtual ICollection<UserHasSubscribe> Subscriptions { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required(ErrorMessageResourceName = "RequiredUsername", ErrorMessageResourceType = typeof(Resources.Global))]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required(ErrorMessageResourceName = "RequiredOldPassword", ErrorMessageResourceType = typeof(Resources.Global))]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceName = "NewPasswordLengthNotProper", ErrorMessageResourceType=typeof(Resources.Global), MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessageResourceName = "ConfirmPasswordNotEqualToNewPassword", ErrorMessageResourceType = typeof(Resources.Global))]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessageResourceName = "RequiredUsername", ErrorMessageResourceType = typeof(Resources.Global))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredPassword", ErrorMessageResourceType = typeof(Resources.Global))]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessageResourceName = "RequiredUsername", ErrorMessageResourceType = typeof(Resources.Global))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredPassword", ErrorMessageResourceType = typeof(Resources.Global))]
        [StringLength(100, ErrorMessageResourceName = "NewPasswordLengthNotProper", ErrorMessageResourceType = typeof(Resources.Global), MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceName = "ConfirmPasswordNotEqualToNewPassword", ErrorMessageResourceType = typeof(Resources.Global))]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
