using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TAiMStore.Domain;
using TAiMStore.Model.Repository;
using TAiMStore.Model.UnitOfWork;
using TAiMStore.Model.ViewModels;
using TAiMStore.Classes;

namespace TAiMStore.Model.Classes
{
    public class UserManager
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleRepository;
        private readonly IContactsRepository _contactsRepository;

        public UserManager(IUserRepository userRepository, IRoleRepository roleRepository, IContactsRepository contactsRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _contactsRepository = contactsRepository;
            _unitOfWork = unitOfWork;
            _roleRepository.GetAll();
        }

        #region User
        public User GetUserByName(string name)
        {
            return _userRepository.Get(u => u.Name == name);
        }

        public UserViewModel GetUserViewModelByName(string userName)
        {
            var user = _userRepository.Get(u => u.Name == userName);
            return new UserViewModel { Name = user.Name, Email = user.Email };
        }

        public string ToLowerUserName(string userName)
        {
            return userName.Replace("_", "-").Replace(".", "-").ToLower();
        }

        public User GetUserByEmail(string email)
        {
            return _userRepository.Get(u => u.Email == email);
        }

        public UserViewModel RegisterUser(string userName, string email, string pass)
        {
            var user = new User();
            var userViewModel = new UserViewModel();
            user.Name = userName;
            user.Email = email;
            user.Password = Hash.HashPassword(pass);
            user.Role = _roleRepository.Get(r => r.Name == ConstantStrings.CustomerRole);
            if (user.Role.Name == ConstantStrings.CustomerRole) user.isActivate = false;
            else user.isActivate = true;
            _userRepository.Add(user);
            _unitOfWork.Commit();

            userViewModel.Name = userName;
            userViewModel.Email = email;
            return userViewModel;
        }

        public bool ValidateUser(string userName, string password)
        {
            var user = _userRepository.Get(u => u.Name == userName);
            if (user == null) return false;
            else
            {
                if (user.Password != Hash.HashPassword(password))
                {
                    return false;
                }
                else return true;
            }
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword, string confirmPassword)
        {
            if (ValidateUser(userName, oldPassword))
            {
                var user = _userRepository.Get(u => u.Name == userName);
                if (newPassword.Equals(confirmPassword))
                {
                    
                    user.Password = Hash.HashPassword(newPassword);
                    _userRepository.Update(user);
                    _unitOfWork.Commit();

                    return true;
                }
                else return false;
            }
            else return false;
        }
        #endregion

        #region Role
        public bool UserIsInRole(string userName, string roleName)
        {
            var user = _userRepository.Get(u => u.Name == userName);
            if (user.Role.Name == roleName) return true;
            else return false;
        }

        public void UserEdit(string userId, string roleName, bool activate)
        {
            var id = Convert.ToInt32(userId);
            var user = _userRepository.Get(u => u.Id == id);
            user.Role = _roleRepository.Get(r => r.Name == roleName);
            user.Contacts = _contactsRepository.Get(c => c.User.Id == user.Id);
            user.isActivate = activate;
            _userRepository.Update(user);
            _unitOfWork.Commit();
        }

        #endregion

        #region Profile
        public ProfileViewModel GetProfileViewModelByName(string userName)
        {
            var user = GetUserByName(userName);
            var contacts = _contactsRepository.Get(c => c.Id == user.Id);

            if (contacts == null) return null;
            else
            {
                var profile = new ProfileViewModel
                {
                    City = contacts.City,
                    //Email = ConstantStrings.LabelForEmailStart + user.Email + ConstantStrings.LabelForEmailEnd,
                    EmailForTextBox = user.Email,
                    House = contacts.House,
                    PostZip = contacts.PostZip.ToString(),
                    PersonFullName = contacts.PersonFullName,
                    Room = contacts.Room,
                    Street = contacts.Street,
                    Telephone = contacts.Telephone
                };

                return profile;
            }
        }

        public void ContactsAdd(string userName, ProfileViewModel profile)
        {
            var user = GetUserByName(userName);
            var contact = new Contacts
            {
                PersonFullName = profile.PersonFullName,
                City = profile.City,
                Street = profile.Street,
                House = profile.House,
                PostZip = Convert.ToInt32(profile.PostZip),
                Room = profile.Room,
                Telephone = profile.Telephone
            };
            user.Contacts = contact;
            _userRepository.Update(user);
            _unitOfWork.Commit();
        }

        public void ContactsEdit(string userName, ProfileViewModel profile)
        {
            var user = GetUserByName(userName);
            _contactsRepository.GetAll();
            var contact = user.Contacts;
            contact.PersonFullName = profile.PersonFullName;
            contact.City = profile.City;
            contact.Street = profile.Street;
            contact.House = profile.House;
            contact.PostZip = Convert.ToInt32(profile.PostZip);
            contact.Room = profile.Room;
            contact.Telephone = profile.Telephone;
            user.Email = profile.Email;

            _userRepository.Update(user);
            _contactsRepository.Update(contact);

            _unitOfWork.Commit();
        }

        #endregion

        #region Admin

        public List<UserViewModel> GetUsers()
        {
            var users = new List<UserViewModel>();
            var userEntities = _userRepository.GetAll();
            var roles = _roleRepository.GetAll();
            var contacts = _contactsRepository.GetAll();

            foreach (var userEntity in userEntities)
            {
                var tmp = new UserViewModel();
                tmp.Id = userEntity.Id;
                tmp.Name = userEntity.Name;
                tmp.Email = userEntity.Email;

                var list = new List<SelectListItem>();
                list.Add(new SelectListItem { Selected = true, Text = userEntity.Role.Name, Value = userEntity.Role.Name });
                foreach (var role in roles)
                {
                    if (role.Name != userEntity.Role.Name) list.Add(new SelectListItem { Selected = false, Text = role.Name, Value = role.Name });
                }
                tmp.Roles = list;

                users.Add(tmp);
            }

            return users;
        }

        #endregion
    }
}
