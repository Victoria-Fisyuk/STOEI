using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TAiMStore.Domain
{
    public  class Role : Entity
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Display(Name = "Роль")]
        [Required(ErrorMessage = "Вы забыли ввести имя роли")]
        public string Name { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
