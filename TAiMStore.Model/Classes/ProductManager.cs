using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TAiMStore.Domain;
using TAiMStore.Model.Repository;
using TAiMStore.Model.ViewModels;

namespace TAiMStore.Model.Classes
{
    public class ProductManager: Controller
    {
        private readonly IProductRepository _repository;

        public ProductManager(IProductRepository productsRepository)
        {
            _repository = productsRepository;
        }
        
        private ProductsViewModel GetProductsList(string category, int page, int pageSize)
        {
            var products = _repository.GetMany(p => category == null || p.Category.Name == category)
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            var productsViewModel = new List<ProductViewModel>();

            foreach (var product in products)
            {
                var productViewModel = new ProductViewModel();
                productViewModel.EntityToProductViewModel(product);
                productsViewModel.Add(productViewModel);
            }

            var model = new ProductsViewModel
            {
                Products = productsViewModel,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = category == null ?
                    _repository.GetCount() :
                    _repository.GetMany(e => e.Category.Name == category).Count()
                },
                CurrentCategory = category
            };

            return model;
        }

        public ProductViewModel GetProduct(int Id)
        {
            var product = new ProductViewModel();
            var tempProduct = _repository.GetById(Id);
            product.EntityToProductViewModel(tempProduct);
            return product;
        }

        public ProductsViewModel GetProducts(string category, int page, int pageSize)
        {
            var key = HttpRuntime.Cache.GenerateCacheKey("GetProductsList_" + category + "_" + page.ToString());
            return HttpRuntime.Cache.CheckCache(key, 10, () => GetProductsList(category, page, pageSize));
        }

        private FileContentResult GetImageById(int id)
        {
            var prod = _repository.GetById(id);
            if (prod != null) return File(prod.ImageData, prod.ImageMimeType);
            return null;
        }

        public FileContentResult GetImage(int id)
        {
            var key = HttpRuntime.Cache.GenerateCacheKey("GetImage" + id);
            return HttpRuntime.Cache.CheckCache(key, 60, () => GetImageById(id));
        }

    }
}
