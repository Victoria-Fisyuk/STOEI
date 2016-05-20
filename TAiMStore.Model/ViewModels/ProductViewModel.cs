using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TAiMStore.Domain;
using TAiMStore.Model.Repository;
using TAiMStore.Model.UnitOfWork;

namespace TAiMStore.Model.ViewModels
{
    public class ProductViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите имя")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Пожалуйста введите описание")]
        public string Description { get; set; }

        [DataType(DataType.MultilineText)]
        public string DescriptionSecond { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Пожалуйста введите положительную цену")]
        public decimal Price { get; set; }

        public byte[] ImageData { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ImageMimeType { get; set; }

        [Required(ErrorMessage = "Please specify category")]
        public string Category { get; set; }

        [HiddenInput(DisplayValue = false)]
        public DateTime CreateDate { get; set; }

        public void ProductViewModelToProductEntity(Product product)
        {
            product.Id = this.Id;
            product.Name = this.Name;
            product.ImageData = this.ImageData;
            product.ImageMimeType = this.ImageMimeType;
            product.Description = this.Description;
            product.DescriptionSecond = this.DescriptionSecond;
            product.Price = this.Price;
        }

        public void ProductViewModelToEntity(Product product, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            product.Id = this.Id;
            product.Name = this.Name;
            product.ImageData = this.ImageData;
            product.ImageMimeType = this.ImageMimeType;
            product.Description = this.Description;
            product.DescriptionSecond = this.DescriptionSecond;
            product.Price = this.Price;
                        
            unitOfWork.Commit();

            var category = categoryRepository.Get(c => c.Name == this.Category);
            if (category != null) product.Category = category;
            else
            {
                var newCategory = new Category();
                newCategory.Name = this.Category;

                categoryRepository.Add(newCategory);
                unitOfWork.Commit();
                product.Category = newCategory;
            }
        }

        public void EntityToProductViewModel(Product product)
        {
            this.Id = product.Id;
            this.Name = product.Name;
            this.ImageData = product.ImageData;
            this.ImageMimeType = product.ImageMimeType;
            this.Description = product.Description;
            this.DescriptionSecond = product.DescriptionSecond;
            this.Price = product.Price;
            this.CreateDate = product.CreateDate;

            this.Category = product.Category.Name;
        }
    }
}