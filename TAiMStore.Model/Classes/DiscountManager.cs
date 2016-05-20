using System;
using System.Collections.Generic;
using TAiMStore.Domain;
using TAiMStore.Model.Constants;
using TAiMStore.Model.Repository;
using TAiMStore.Model.UnitOfWork;
using TAiMStore.Model.ViewModels;

namespace Store.Model.Classes
{
    public class DiscountManager
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DiscountManager(IDiscountRepository discountRepository, IProductRepository productRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _discountRepository = discountRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        #region Get
        public List<DiscountViewModel> GetDiscountViewModels(string type)
        {
            var retVal = new List<DiscountViewModel>();

            if (type == ConstantStrings.Category)
            {
                _categoryRepository.GetAll();
                var tmp = _discountRepository.GetMany(d => d.Category != null);
                foreach (var discount in tmp)
                {
                    if (discount.Ammount != 0)
                    {
                        retVal.Add(new DiscountViewModel
                        {
                            Id =  discount.Id,
                            Ammount = discount.Ammount,
                            CategoryName = discount.Category.Name,
                        });
                    }
                }
            }
            else if (type == ConstantStrings.Products)
            {
                _productRepository.GetAll();
                var tmp = _discountRepository.GetMany(d => d.Product != null);
                foreach (var discount in tmp)
                {
                    if (discount.Ammount != 0)
                    {
                        retVal.Add(new DiscountViewModel
                        {
                            Id = discount.Id,
                            Ammount = discount.Ammount,
                            ProductName = discount.Product.Name,
                        });
                    }
                }
            }

            return retVal;
        }
        #endregion
        #region Set
        public void AddDiscountToCategory(string discountType, string ammount)
        {
            var category = _categoryRepository.Get(c => c.Name == discountType);
            var discount = new Discount() {Ammount =  Convert.ToDouble(ammount), Category = category};

            try
            {
                _discountRepository.Add(discount);
                _unitOfWork.Commit();
            }
            catch{}
        }

        public void EditDiscountCategory()
        {

        }

        #endregion

        public void DeleteDiscount(string id)
        {
            var _id = Convert.ToInt32(id);
            var tmp = _discountRepository.Get(d => d.Id == _id);
            _discountRepository.Delete(tmp);
            _unitOfWork.Commit();
        }
    }
}
