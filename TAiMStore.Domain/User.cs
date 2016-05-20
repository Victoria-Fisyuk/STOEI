using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TAiMStore.Domain
{
    public class User : Entity
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Пожалуйста, укажите логин")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Пожалуйста, введите ваш пароль")]
        public string Password { get; set; }
        
        [HiddenInput(DisplayValue = false)]
        public bool? isActivate { get; set; }

        public virtual Role Role { get; set; }

        public virtual Contacts Contacts { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
