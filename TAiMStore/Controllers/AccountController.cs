using System;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TAiMStore.Domain;
using TAiMStore.HtmlHelpers;
using TAiMStore.Model;
using TAiMStore.Model.Classes;
using TAiMStore.Model.Repository;
using TAiMStore.Model.UnitOfWork;
using TAiMStore.Model.ViewModels;

namespace TAiMStore.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IContactsRepository _contactsRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _passwordMinLength = 6;
        private static bool IsEdit = false;

        public AccountController(IUserRepository userRepository, IRoleRepository roleRepository, IContactsRepository contactsRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _contactsRepository = contactsRepository;
            _unitOfWork = unitOfWork;
        }

        #region ViewResults

        /// <summary>
        /// Показать представление для регистрации
        /// </summary>
        /// <returns>Представление для регистрации</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Register()
        {
            return View(new MasterPageModel());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Register(string userName, string email, string password, string confirmPassword, string myCaptcha, string attempt)
        {
            var masterModel = new MasterPageModel();
            var manager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            if (ValidateRegistration(userName, email, password, confirmPassword, HttpContext, myCaptcha, attempt))
            {
                // Создание пользователя
                UserViewModel user;

                user = manager.RegisterUser(userName, email, password);

                // Вход
                FormsAuthentication.SetAuthCookie(userName, false);
                var ticket = new FormsAuthenticationTicket(userName, true, 10);
                var identity = new FormsIdentity(ticket);
                HttpContext.User = new RolePrincipal(identity);
                masterModel.UserModel = user;
                masterModel.UserRole = ConstantStrings.CustomerRole;
                return View("RegisterSuccess", masterModel);
            }

            // If we got this far, something failed, redisplay form
            masterModel.UserModel = new UserViewModel();
            return View(masterModel);
        }

        public ActionResult Login()
        {
            return RedirectToAction("Index", "Home");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Login(string userName, string password, bool rememberMe)
        {
            var model = new MasterPageModel();
            var manager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            if (ValidateLogOn(userName, password))
            {
                FormsAuthentication.SetAuthCookie(userName, rememberMe);
                var ticket = new FormsAuthenticationTicket(userName, true, 10);
                var identity = new FormsIdentity(ticket);
                HttpContext.User = new RolePrincipal(identity);
                model.UserModel = manager.GetUserViewModelByName(userName);
                var IsAdmin = manager.UserIsInRole(userName, ConstantStrings.AdministratorRole);
                if (IsAdmin) return RedirectToAction("Index", "Admin");
                else return View("LoginSucces");
            }

            return View("LoginNotSucces");
        }

        /// <summary>
        /// Выход
        /// </summary>
        /// <returns>Представление главной страницы</returns>
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("List", "Product");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ChangePassword()
        {
            var model = new MasterPageModel();
            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            InitializeUsersRoles(model, userManager);

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ChangePassword(string password, string newPassword, string confirmPassword)
        {
            var model = new MasterPageModel();
            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            InitializeUsersRoles(model, userManager);

            var userName = HttpContext.User.Identity.Name;
            if (userManager.ChangePassword(userName, password, newPassword, confirmPassword))
            {
                return View("PasswordChanged", model);
            }
            else return View("PasswordNotChanged", model);
        }

        public ActionResult Profile()
        {
            var model = new MasterPageModel();
            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            InitializeUsersRoles(model, userManager);

            model.ProfileView = userManager.GetProfileViewModelByName(model.UserModel.Name);

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ProfileAdd()
        {
            var model = new MasterPageModel();
            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var user = userManager.GetUserViewModelByName(HttpContext.User.Identity.Name);
                var userRole = string.Empty;
                if (userManager.UserIsInRole(user.Name, ConstantStrings.AdministratorRole) ||
                    userManager.UserIsInRole(user.Name, ConstantStrings.ModeratorRole))
                    userRole = ConstantStrings.AdministratorRole;
                else userRole = ConstantStrings.CustomerRole;
                model.UserModel = user;
                model.UserRole = userRole;
            }
            if (!IsEdit)
            {
                return View(new ProfileViewModel());
            }
            else
            {
                var profileView = userManager.GetProfileViewModelByName(model.UserModel.Name);
                return View(profileView);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProfileAdd(string fullName, string organization, string city, string street, string house, string room, string telephone, string postZip)
        {
            var model = new MasterPageModel();
            var userManager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var user = userManager.GetUserViewModelByName(HttpContext.User.Identity.Name);
                var userRole = string.Empty;
                if (userManager.UserIsInRole(user.Name, ConstantStrings.AdministratorRole) ||
                    userManager.UserIsInRole(user.Name, ConstantStrings.ModeratorRole))
                    userRole = ConstantStrings.AdministratorRole;
                else userRole = ConstantStrings.CustomerRole;
                model.UserModel = user;
                model.UserRole = userRole;
            }

            var profileView = new ProfileViewModel
            {
                PersonFullName = fullName,
                Organization = organization,
                City = city,
                Street = street,
                Email = model.UserModel.Email,
                House = house,
                PostZip = postZip,
                Room = room,
                Telephone = telephone
            };

            if (!IsEdit)
            {
                userManager.ContactsAdd(model.UserModel.Name, profileView);
            }
            else
            {
                userManager.ContactsEdit(model.UserModel.Name, profileView);
            }
            model.ProfileView = profileView;

            return View("Profile", model);
        }

        public ActionResult ProfileEdit()
        {
            IsEdit = true;
            return RedirectToAction("ProfileAdd");
        }

        #endregion

        #region Registration

        /// <summary>
        /// Проверка на занятость логина
        /// </summary>
        /// <param name="userName">Логин</param>
        /// <returns>Результат</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UserExists(string userName)
        {
            if (Request.IsAjaxRequest())
            {
                return new JsonResult { Data = !ValidateUserName(userName), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return null;
        }
        #endregion

        #region Validation
        /// <summary>
        /// Валидация при входе
        /// </summary>
        /// <param name="userName">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>Результат валидации</returns>
        private bool ValidateLogOn(string userName, string password)
        {
            var manager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", "Вы должны ввести логин.");
                return ModelState.IsValid;
            }
            if (String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "Вы должны ввести пароль.");
                return ModelState.IsValid;
            }
            if (!manager.ValidateUser(userName, password))
            {
                ModelState.AddModelError("_FORM", "Неправильный логин или пароль.");
                return ModelState.IsValid;
            }

            return ModelState.IsValid;
        }
        /// <summary>
        /// Валидация при указании нового логина OpenID пользователем
        /// </summary>
        /// <param name="userName">Логин</param>
        /// <returns>Результат валидации</returns>
        private bool ValidateUserName(string userName)
        {
            var manager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("_Form", "Вы должны ввести логин.");
            }
            else
            {
                if (manager.GetUserByName(userName) != null)
                {
                    ModelState.AddModelError("_Form", "Данный логин уже зарегистрирован.");
                    return ModelState.IsValid;
                }

                if (Regex.IsMatch(manager.ToLowerUserName(userName), @"(^[^A-Za-z])|[-]{2}|([-]$)|([^A-Za-z0-9-]+)"))
                {
                    ModelState.AddModelError("_Form", "Некорректный логин.");
                    return ModelState.IsValid;
                }

                if (userName.Length > 15)
                {
                    ModelState.AddModelError("_Form", "Логин должен быть не более 15 символов.");
                    return ModelState.IsValid;
                }

                if (userName.Length < 3)
                {
                    ModelState.AddModelError("_Form", "Логин должен быть не менее 3 символов.");
                    return ModelState.IsValid;
                }
            }
            return ModelState.IsValid;
        }

        /// <summary>
        /// Валидация при регистрации
        /// </summary>
        /// <param name="userName">Логин</param>
        /// <param name="email">E-Mail</param>
        /// <param name="password">Пароль</param>
        /// <param name="confirmPassword">Подтверждение пароля</param>
        /// <param name="context">Контекст (для captcha)</param>
        /// <param name="captcha">Идентификатор ответа на captcha</param>
        /// <param name="attempt">Ответ на captcha</param>
        /// <returns>Результат валидации</returns>
        private bool ValidateRegistration(string userName, string email, string password, string confirmPassword,
            HttpContextBase context, string captcha, string attempt)
        {
            var manager = new UserManager(_userRepository, _roleRepository, _contactsRepository, _unitOfWork);
            if (!ValidateUserName(userName))
            {
                ModelState.AddModelError("userName", "Некорректный логин");
            }

            if (!Captcha.VerifyAndExpireSolution(context, captcha, attempt))
            {
                ModelState.AddModelError("captcha", "Неверно введены символы");
            }

            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("email", "Введите E-Mail.");
            }
            else
            {
                if (email.Length > 20)
                {
                    ModelState.AddModelError("email", "E-Mail должен содержать не более 20 символов.");
                }
                if (!Regex.IsMatch(email, @"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$"))
                {
                    ModelState.AddModelError("email", "Введите корректный E-Mail.");
                }
                var user = manager.GetUserByEmail(email);
                if (user != null)
                {
                    ModelState.AddModelError("_FORM", "Такой email уже зарегистрирован.");
                    return ModelState.IsValid;
                }
            }
            if (string.IsNullOrEmpty(password) || password.Length < _passwordMinLength)
            {
                ModelState.AddModelError("password",
                    String.Format(CultureInfo.CurrentCulture,
                         "Пароль должен содержать {0} или более символов.",
                         _passwordMinLength));
            }
            if (!String.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", "Новый пароль и подтверждение пароля должны совпадать.");
            }
            return ModelState.IsValid;
        }

        public PartialViewResult UserMenu()
        {
            var userName = HttpContext.User.Identity.Name;
            var masterModel = new MasterPageModel();

            return PartialView(masterModel);
        }

        #endregion

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