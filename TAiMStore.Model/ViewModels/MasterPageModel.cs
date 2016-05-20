using System.Collections.Generic;
using System.Web.Mvc;

namespace TAiMStore.Model.ViewModels
{
    public class MasterPageModel
    {
        public UserViewModel UserModel { get; set; }
        public ProductsViewModel ProductsViewModel { get; set; }
        public ProductViewModel ProductView { get; set; }
        public ProfileViewModel ProfileView { get; set; }
        public OrdersViewModel OrdersViewModel { get; set; }
        public OrderViewModel OrderViewModel { get; set; }
        
        public CartIndexViewModel CartView { get; set; }

        public List<UserViewModel> Users { get; set; }
        public List<SelectListItem> RolesList { get; set; }
        public List<PaymentViewModel> Payments { get; set; }
        public List<SelectListItem> PaymentsForDropDown { get; set; }
        public PaymentViewModel Payment { get; set; }
        public CategoryViewModel Category { get; set; }
        public List<SelectListItem> CategoriesForDropDown { get; set; }
        public List<CategoryViewModel> Categories { get; set; }
        public string UserRole { get; set; }
        public string ReturnUrl { get; set; }
    }
}
