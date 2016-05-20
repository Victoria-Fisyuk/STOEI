using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace TAiMStore.Domain
{
    public class OrderProduct : Entity
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Display(Name = "Количество товара")]
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Пожалуйста, введите положительное количество товаров")]
        public int Quantity { get; set; }

        public virtual Product Product { get; set; }
        public virtual Order Order { get; set; }
    }
}
