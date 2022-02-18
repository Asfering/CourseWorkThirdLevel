using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CourseWorkThirdLevel.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Логин")]
        public string UserLogin { get; set; }
        [Required]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "Логин")]
        public string UserLogin { get; set; }
        [Required]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }
        [Required]
        [Display(Name = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [Compare("UserPassword", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
        [Required]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Фамилия")]
        public string SecondName { get; set; }
    }

}