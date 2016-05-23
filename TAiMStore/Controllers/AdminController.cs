using System.Web.Mvc;
using System.Linq;
using TAiMStore.Domain;
using TAiMStore.Model.Repository;
using System.Web;
using TAiMStore.Model.UnitOfWork;
using TAiMStore.Model.Classes;
using TAiMStore.Model;
using TAiMStore.Model.ViewModels;
using System.Collections.Generic;

namespace TAiMStore.WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        private readonly IProductRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderProductRepository _orderProductRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IContactsRepository _contactsRepository;
        private readonly IUnitOfWork _unitOfWork;
        private static bool _create = true;

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="categoryRepository"></param>
        /// <param name="orderRepository"></param>
        /// <param name="orderProductRepository"></param>
        /// <param name="paymentRepository"></param>
        /// <param name="userRepository"></param>
        /// <param name="roleRepository"></param>
        /// <param name="contactsRepository"></param>
        /// <param name="unitOfWork"></param>
        public AdminController(IProductRepository repo, ICategoryRepository categoryRepository, IOrderRepository orderRepository,
            IOrderProductRepository orderProductRepository, IPaymentRepository paymentRepository, IUserRepository userRepository, 
            IRoleRepository roleRepository, IContactsRepository contactsRepository, IUnitOfWork unitOfWork)
        {
            _repository = repo;
            _categoryRepository = categoryRepository;
            _orderProductRepository = orderProductRepository;
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _contactsRepository = contactsRepository;
            _unitOfWork = unitOfWork;
            categoryRepository.GetAll();
        }

        /// <summary>
        /// загружаем главную страницу админ-панели
        /// где у нас находится список всех продуктов
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var userName = HttpContext.User.Identity.Name;
            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            var IsAdmin = userManager.UserIsInRole(userName, ConstantStrings.AdministratorRole);
            var IsModerator = userManager.UserIsInRole(userName, ConstantStrings.ModeratorRole);

            //-----------------------------------------
            if (IsAdmin || IsModerator)
            {
                var masterViewModel = new MasterPageModel();
                var productsViewModel = new ProductsViewModel();
                var products = _repository.GetAll();
                var productList = new List<ProductViewModel>();
                foreach (var product in products)
                {
                    var tmp = new ProductViewModel();
                    tmp.EntityToProductViewModel(product);
                    productList.Add(tmp);
                }
                productsViewModel.Products = productList;
                masterViewModel.ProductsViewModel = productsViewModel;
                masterViewModel.UserModel = userManager.GetUserViewModelByName(userName);
                masterViewModel.UserRole = ConstantStrings.AdministratorRole;
                return View(masterViewModel);
            }
            else return RedirectToAction("List", "Product");
        }

        /// <summary>
        /// меню администратора в зависимости от уровня доступа
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _AdminMenu()
        {
            var userName = HttpContext.User.Identity.Name;
            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            var IsAdmin = userManager.UserIsInRole(userName, ConstantStrings.AdministratorRole);
            var IsModerator = userManager.UserIsInRole(userName, ConstantStrings.ModeratorRole);
            var masterModel = new MasterPageModel();

            if (IsAdmin) masterModel.UserRole = ConstantStrings.AdministratorRole;
            if (IsModerator) masterModel.UserRole = ConstantStrings.ModeratorRole;

            return PartialView(masterModel);
        }

        #region Products
        
        /// <summary>
        /// Получаем продукт по Id и редактируем данные в нём
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ViewResult Edit(int Id)
        {
            Product product = _repository.Get(p => p.Id == Id);
            var masterViewModel = new MasterPageModel();
            var viewModel = new ProductViewModel();
            if (product == null) _create = true;
            {
                viewModel.EntityToProductViewModel(product);
                _create = false;
            }
            masterViewModel.ProductView = viewModel;
            var manager = new CategoryManager(_categoryRepository, _unitOfWork);
            masterViewModel.CategoriesForDropDown = manager.GetCategoriesForDropDown();
            return View(masterViewModel);
        }

        [HttpPost]
        public ActionResult Edit(int Id, string name, string desc,string descSec, decimal price, string cat, HttpPostedFileBase image)
        {
            var viewModel = new ProductViewModel
            {
                Id = Id,
                Name = name,
                Category = cat,
                Description = desc,
                DescriptionSecond = descSec,
                Price = price,
            };

            var masterModel = new MasterPageModel();
            _categoryRepository.GetAll();
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    viewModel.ImageMimeType = image.ContentType;
                    viewModel.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(viewModel.ImageData, 0, image.ContentLength);
                }
                else
                {
                    var tmp = _repository.Get(p => p.Id == viewModel.Id);
                    viewModel.ImageData = tmp.ImageData;
                    viewModel.ImageMimeType = tmp.ImageMimeType;
                }
                var product = new Product();
                if (_create)
                {
                    viewModel.ProductViewModelToEntity(product, _categoryRepository, _unitOfWork);
                    _repository.Add(product);
                }
                else
                {
                    product = _repository.Get(p => p.Id == viewModel.Id);
                    viewModel.ProductViewModelToProductEntity(product);
                   
                    var category = product.Category;
                    
                    if (category.Name != viewModel.Category)
                    {
                        var newCategory = _categoryRepository.Get(c => c.Name == viewModel.Category);
                        if (newCategory != null) product.Category = newCategory;
                        else
                        {
                            var tmp = new Category { Name = viewModel.Category };
                            _categoryRepository.Add(tmp);
                            _unitOfWork.Commit();
                            product.Category = tmp;
                        }
                        _unitOfWork.Commit();
                    }
                    _repository.Update(product);
                }
                _unitOfWork.Commit();
                TempData["Message"] = string.Format("{0} has been saved", product.Name);
                return RedirectToAction("Index");
            }
            else
            {
                masterModel.ProductView = viewModel;
                return View(masterModel);
            }
        }

        public ViewResult Create()
        {
            _create = true;
            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            var masterModel = new MasterPageModel();
            InitializeUsersRoles(masterModel, userManager);
            masterModel.ProductView = new ProductViewModel();

            var manager = new CategoryManager(_categoryRepository, _unitOfWork);
            masterModel.CategoriesForDropDown = manager.GetCategoriesForDropDown();

            return View("Edit", masterModel);
        }

        [HttpPost]
        public ActionResult Delete(int productId)
        {
            var product = _repository.GetById(productId);
            _repository.Delete(product);
            if (product != null)
            {
                TempData["Message"] = string.Format("{0} was deleted", product.Name);
            }
            _unitOfWork.Commit();
            return RedirectToAction("Index");
        }
        #endregion

        #region Categories

        public ActionResult Categories()
        {
            var manager = new CategoryManager(_categoryRepository, _unitOfWork);
            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            var masterModel = new MasterPageModel();
            masterModel.Categories = manager.GetCategories();
            InitializeUsersRoles(masterModel, userManager);

            return View(masterModel);
        }

        public ActionResult CategoryDelete(int categoryId)
        {
            var manager = new CategoryManager(_categoryRepository, _unitOfWork);
            manager.DeleteCategory(categoryId);
            return RedirectToAction("Categories");
        }

        public ViewResult CategoryAdd()
        {
            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            var masterModel = new MasterPageModel();
            masterModel.Category = new CategoryViewModel();
            InitializeUsersRoles(masterModel, userManager);
            return View(masterModel);
        }

        [HttpPost]
        public ActionResult CategoryAdd(string categoryName)
        {
            var manager = new CategoryManager(_categoryRepository, _unitOfWork);
            manager.AddCategory(categoryName);
            return RedirectToAction("Categories");
        }

        [HttpGet]
        public ActionResult CategoryEdit(int categoryId)
        {
            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            var manager = new CategoryManager(_categoryRepository, _unitOfWork);
            var masterModel = new MasterPageModel();
            masterModel.Category = manager.GetCategoryViewModel(categoryId);
            InitializeUsersRoles(masterModel, userManager);
            return View(masterModel);
        }

        [HttpPost]
        public ActionResult CategoryEdit(string categoryName, int categoryId)
        {
            var manager = new CategoryManager(_categoryRepository, _unitOfWork);
            manager.EditCategory(categoryName, categoryId);
            return RedirectToAction("Categories");
        }

        #endregion

        #region Payments

        public ActionResult Payments()
        {
            var manager = new PaymentManager(_paymentRepository, _unitOfWork);
            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            var masterModel = new MasterPageModel();
            masterModel.Payments = manager.GetPayments();
            InitializeUsersRoles(masterModel, userManager);

            return View(masterModel);
        }

        public ActionResult PaymentDelete(int paymentId)
        {
            var manager = new PaymentManager(_paymentRepository, _unitOfWork);
            manager.DeletePayment(paymentId);
            return RedirectToAction("Payments");
        }

        public ViewResult PaymentAdd()
        {
            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            var masterModel = new MasterPageModel();
            masterModel.Payment = new PaymentViewModel();
            InitializeUsersRoles(masterModel, userManager);
            return View(masterModel);
        }

        [HttpPost]
        public ActionResult PaymentAdd(string paymentName)
        {
            var manager = new PaymentManager(_paymentRepository, _unitOfWork);
            manager.AddPayment(paymentName);
            return RedirectToAction("Payments");
        }

        [HttpGet]
        public ActionResult PaymentEdit(int paymentId)
        {
            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            var manager = new PaymentManager(_paymentRepository, _unitOfWork);
            var masterModel = new MasterPageModel();
            masterModel.Payment = manager.GetPaymentViewModel(paymentId);
            InitializeUsersRoles(masterModel, userManager);
            return View(masterModel);
        }

        [HttpPost]
        public ActionResult PaymentEdit(string paymentName, int paymentId)
        {
            var manager = new PaymentManager(_paymentRepository, _unitOfWork);
            manager.EditPaymenty(paymentName, paymentId);
            return RedirectToAction("Payments");
        }

        #endregion

        #region Users
        public ViewResult Users()
        {
            var masterViewModel = new MasterPageModel();
            var manager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            InitializeUsersRoles(masterViewModel, manager);

            return View(masterViewModel);
        }

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

        [HttpPost]
        public ActionResult DeleteUser(int userId)
        {
            var user = _userRepository.GetById(userId);
            var contacts = _contactsRepository.Get(c => c.User.Id == userId);
            _contactsRepository.Delete(contacts);
            _userRepository.Delete(user);
            if (user != null)
            {
                TempData["Message"] = string.Format("{0} was deleted", user.Name);
            }
            _unitOfWork.Commit();
            return RedirectToAction("Users");
        }

        
        [HttpPost]
        public ActionResult EditUser(string userId, string roles, bool isActivate)
        {
            var manager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);

            manager.UserEdit(userId, roles, isActivate);
            return RedirectToAction("Users");
        }

        #endregion

        #region Orders

        /// <summary>
        /// получаем список всех заказов
        /// </summary>
        /// <returns>модель</returns>
        public ActionResult OrderList()
        {
            var userName = HttpContext.User.Identity.Name;
            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            var IsAdmin = userManager.UserIsInRole(userName, ConstantStrings.AdministratorRole);
            var IsModerator = userManager.UserIsInRole(userName, ConstantStrings.ModeratorRole);

            //-----------------------------------------
            if (IsAdmin || IsModerator)
            {
                var masterViewModel = new MasterPageModel();
                var ordersViewModel = new OrdersViewModel();
                var orders = _orderRepository.GetAll();
                var orderList = new List<OrderViewModel>();
                foreach (var order in orders)
                {
                    var tmpOrder = new OrderViewModel();
                    tmpOrder.EntityToViewModel(order);
                    orderList.Add(tmpOrder);
                }
                ordersViewModel.Orders = orderList;
                masterViewModel.OrdersViewModel = ordersViewModel;
                masterViewModel.UserModel = userManager.GetUserViewModelByName(userName);
                masterViewModel.UserRole = ConstantStrings.AdministratorRole;
                return View(masterViewModel);
            }
            else return RedirectToAction("List", "Product");
        }

        /// <summary>
        /// формируем список заказов по заданному пользователю
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public ViewResult SortedOrderList(string userName)
        {
            return View(_orderRepository.GetMany(p => p.User.Name == userName));
        }
        
        /// <summary>
        /// загружаем подробности о заказе по его ID.
        /// передаём в MasterPageModel для построения страницы
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>модель страницы</returns>
        public ViewResult OrderDetail(int orderId)
        {
            var masterPage = new MasterPageModel();

            var order = _orderRepository.GetById(orderId);
            var orderViews = new OrderViewModel();
            orderViews.EntityToViewModel(order);
            masterPage.OrderViewModel = orderViews;

            var orderProducts = _orderProductRepository.GetMany(op => op.Order.Id == orderId);
            var cart = new Cart();
            foreach(var orderProduct in orderProducts)
            {
                cart.AddItem(orderProduct.Product, orderProduct.Quantity);
            }
            masterPage.CartView = new CartViewModel()
            {
                Cart = cart
            };
            
            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            InitializeUsersRoles(masterPage, userManager);
            
            return View(masterPage);
        }

        /// <summary>
        /// удаление заданного заказа 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteOrder(int orderId)
        {
            var order = _orderRepository.GetById(orderId);
            var orderProducts = _orderProductRepository.GetMany(o => o.Order.Id == orderId);
            foreach (var itemOP in orderProducts)
            {
                _orderProductRepository.Delete(itemOP);
            }
            _orderRepository.Delete(order);
            if (order != null)
            {
                TempData["Message"] = string.Format("{0} was deleted", order.Id);
            }
            _unitOfWork.Commit();
            return RedirectToAction("Index");
        }
        #endregion

    }
}