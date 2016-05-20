using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TAiMStore.Domain
{
    public class Category: Entity
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Имя категории")]
        [Required(ErrorMessage = "Пожалуйста, введите наименование категории")]
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
