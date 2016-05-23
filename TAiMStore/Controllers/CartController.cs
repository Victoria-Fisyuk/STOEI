using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TAiMStore.Domain;
using TAiMStore.Model;
using TAiMStore.Model.Classes;
using TAiMStore.Model.Repository;
using TAiMStore.Model.UnitOfWork;
using TAiMStore.Model.ViewModels;

namespace TAiMStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IContactsRepository _contactsRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderProductRepository _orderProductRepository;

        public CartController(IProductRepository productRepository, ICategoryRepository categoryRepository, IUserRepository userRepository,
            IPaymentRepository paymentRepository, IRoleRepository roleRepository, IContactsRepository contactsRepository,
            IOrderRepository orderRepository, IOrderProductRepository orderProductRepository, IUnitOfWork unitOfWork)
        {
            _repository = productRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
            _roleRepository = roleRepository;
            _contactsRepository = contactsRepository;
            _orderRepository = orderRepository;
            _orderProductRepository = orderProductRepository;
            _unitOfWork = unitOfWork;
            categoryRepository.GetAll();
        }

        public ViewResult Index(Cart cart, string returnUrl)
        {
            var masterPage = new MasterPageModel();
            masterPage.CartView = new CartViewModel
            {
                Cart = GetCart(),
                ReturnUrl = returnUrl
            };
            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var user = userManager.GetUserViewModelByName(HttpContext.User.Identity.Name);
                var userRole = string.Empty;
                if (userManager.UserIsInRole(user.Name, ConstantStrings.AdministratorRole) ||
                    userManager.UserIsInRole(user.Name, ConstantStrings.ModeratorRole))
                    userRole = ConstantStrings.AdministratorRole;
                else userRole = ConstantStrings.CustomerRole;
                masterPage.UserModel = user;
                masterPage.UserRole = userRole;
            }

            return View(masterPage);
        }

        public RedirectToRouteResult AddToCart(int Id, string returnUrl)
        {
            Product product = _repository.GetById(Id);
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                if (product != null)
                {
                    GetCart().AddItem(product, 1);
                }
                return RedirectToAction("Index", new { returnUrl });
            } 
            else  return RedirectToAction("Error", new { returnUrl });
        }

        public RedirectToRouteResult RemoveFromCart(int Id, string returnUrl)
        {
            Product product = _repository.GetById(Id);

            if (product != null)
            {
                GetCart().RemoveLine(product);
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        
        public PartialViewResult Summary(Cart cart)
        {
            return PartialView(GetCart());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Checkout(string paymentType, decimal totalCost)
        {
            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            var shipingManager = new ShipingManager(_orderRepository,_orderProductRepository,_repository,
                _userRepository,_paymentRepository,_roleRepository,_contactsRepository, _unitOfWork);
            var lines = GetCart().Lines;
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var user = userManager.GetUserByName(HttpContext.User.Identity.Name);
                shipingManager.CheckOut(lines, paymentType, totalCost, user);
            }
            return RedirectToAction("List", "Product"); ;
        }

        public ActionResult Error()
        {
            return View("Error");
        }

        public Cart GetCart()
        {
            Cart cart = (Cart)Session["Cart"];
            if (cart == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }
    }
}