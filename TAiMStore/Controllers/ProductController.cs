using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAiMStore.Domain;
using TAiMStore.Model;
using TAiMStore.Model.Classes;
using TAiMStore.Model.Repository;
using TAiMStore.Model.UnitOfWork;
using TAiMStore.Model.ViewModels;

namespace TAiMStore.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        private readonly IProductRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IContactsRepository _contactsRepository;
        private readonly IUnitOfWork _unitOfWork;
        public int PageSize = 4;


        public ProductController(IProductRepository productsRepository, ICategoryRepository categoryRepository, IUserRepository userRepository,
            IRoleRepository roleRepository, IContactsRepository contactsRepository, IUnitOfWork unitOfWork)
        {
            _repository = productsRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _contactsRepository = contactsRepository;
            _unitOfWork = unitOfWork;
            _categoryRepository.GetAll();
        }


        //получаем список продуктов, проверяем 
        public ViewResult List(string category, int page = 1)
        {
            var manager = new ProductManager(_repository);
            var model = manager.GetProducts(category, page, PageSize);
            var masterModel = new MasterPageModel();
            masterModel.ProductsViewModel = model;
            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            InitializeUsersRoles(masterModel, userManager);

            return View(masterModel);
        }



        public ViewResult Detail(int Id)
        {
            var manager = new ProductManager(_repository);
            var masterModel = new MasterPageModel();
            masterModel.ProductView = manager.GetProduct(Id);

            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            InitializeUsersRoles(masterModel, userManager);

            return View(masterModel);
        }

        public FileContentResult GetImage(int Id)
        {
            var manager = new ProductManager(_repository);
            var image = manager.GetImage(Id);

            if (image != null) return image;
            return null;
        }

        #region Initilaze
        private void InitializeUsersRoles(MasterPageModel masterViewModel, UserManager manager)
        {
            masterViewModel.Users = manager.GetUsers();

            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var user = userManager.GetUserViewModelByName(HttpContext.User.Identity.Name);
                var userRole = string.Empty;
                if (userManager.UserIsInRole(user.Name, ConstantStrings.AdministratorRole) ||
                    userManager.UserIsInRole(user.Name, ConstantStrings.ModeratorRole))
                    userRole = ConstantStrings.AdministratorRole;
                masterViewModel.UserModel = user;
                masterViewModel.UserRole = userRole;
            }
        }
        #endregion
    }
}