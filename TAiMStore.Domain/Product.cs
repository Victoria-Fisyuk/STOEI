using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TAiMStore.Domain
{
    public class Product:Entity
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string DescriptionSecond { get; set; }
        public virtual Category Category { get; set; }

        [Display(Name = "Цена (руб)")]
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Пожалуйста, введите положительное значение для цены")]
        public decimal Price { get; set; }

        [Column(TypeName = "image")]
        public byte[] ImageData { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string ImageMimeType { get; set; }
        
        

        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
