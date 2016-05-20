using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAiMStore.Model.Repository;
using TAiMStore.Model.ViewModels;

namespace TAiMStore.Controllers
{
    public class NavController : Controller
    {
        // GET: Nav
        protected readonly IProductRepository _productRepository;
        protected readonly ICategoryRepository _categoryRepository;

        public NavController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public PartialViewResult Menu(string category = null)
        {
            ViewBag.SelectedCategory = category;
            var categories = _categoryRepository.GetAll();
            var categoryList = new Dictionary<CategoryViewModel, double>();
            foreach (var entity in categories)
            {
                var newCat = new CategoryViewModel();
                newCat.CategoryName = entity.Name;
                categoryList.Add(newCat, 0);
            }
            return PartialView(categoryList);
        }
    }
    /**
            IEnumerable<string> categories = _categoryRepository.Categories
                .Select(p => p.Name)
                .Distinct()
                .OrderBy(x => x);
            return PartialView(categories);
**/
}

