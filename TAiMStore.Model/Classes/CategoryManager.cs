using System.Collections.Generic;
using System.Web.Mvc;
using TAiMStore.Domain;
using TAiMStore.Model.Repository;
using TAiMStore.Model.UnitOfWork;
using TAiMStore.Model.ViewModels;

namespace TAiMStore.Model.Classes
{
    public class CategoryManager
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryManager(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        #region GetCategories
        public CategoryViewModel GetCategoryViewModel(int categoryId)
        {
            var entity = _categoryRepository.Get(c => c.Id == categoryId);

            return new CategoryViewModel{Id = entity.Id, CategoryName = entity.Name};
        }

        public List<CategoryViewModel> GetCategories()
        {
            var list = new List<CategoryViewModel>();
            var categories = _categoryRepository.GetAll();

            foreach (var category in categories)
            {
                var tmp = new CategoryViewModel();
                tmp.CategoryName = category.Name;
                tmp.Id = category.Id;
                list.Add(tmp);
            }

            return list;
        }

        public List<SelectListItem> GetCategoriesForDropDown()
        {
            var list = new List<SelectListItem>();
            var categories = _categoryRepository.GetAll();

            foreach (var category in categories)
            {
                list.Add(new SelectListItem{Selected = false, Text = category.Name, Value = category.Name});
            }

            return list;
        }

        #endregion

        public void DeleteCategory(int categoryId)
        {
            var category = _categoryRepository.Get(c => c.Id == categoryId);
            _categoryRepository.Delete(category);
            _unitOfWork.Commit();
        }

        public void AddCategory(string categoryName)
        {
            var category = new Category {Name = categoryName};
            _categoryRepository.Add(category);
            _unitOfWork.Commit();
        }

        public void EditCategory(string categoryName, int categoryId)
        {
            var category = _categoryRepository.Get(c => c.Id == categoryId);
            category.Name = categoryName;
            _categoryRepository.Update(category);
            _unitOfWork.Commit();
        }
    }
}
