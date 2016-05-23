using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TAiMStore.Domain;
using TAiMStore.Model.Repository;
using TAiMStore.Model.UnitOfWork;
using TAiMStore.Model.ViewModels;

namespace TAiMStore.Model.Classes
{
    public class ShipingManager : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderProductRepository _orderProductRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IContactsRepository _contactsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ShipingManager(IOrderRepository orderRepository, IOrderProductRepository orderProductRepository,
            IProductRepository productRepository, IUserRepository userRepository, IPaymentRepository paymentRepository,
            IRoleRepository roleRepository, IContactsRepository contactsRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _orderProductRepository = orderProductRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
            _roleRepository = roleRepository;
            _contactsRepository = contactsRepository;
            _unitOfWork = unitOfWork;
            _roleRepository.GetAll();
        }

        public void CheckOut(IEnumerable<CartLine> lines, string payment, decimal totalCost, User user)
        {
            
            var tmpOrder = SetOrder(user, totalCost, payment);
            SetOrderProduct(lines, tmpOrder);
        }

        public Order SetOrder(User user, decimal totalCost, string payment)
        {
            var paymentManager = new PaymentManager(_paymentRepository, _unitOfWork);
            var order = new Order()
            {
                User = user,
                TotalCost = (int)totalCost,
                Payment = paymentManager.GetPaymentByName(payment)
            };
            _orderRepository.Add(order);
            _unitOfWork.Commit();
            return order;
        }

        public void SetOrderProduct(IEnumerable<CartLine> lines, Order order)
        {
            var tmpOrder = _orderRepository.GetById(order.Id);
            foreach(var line in lines)
            {
                var orderProduct = new OrderProduct()
                {
                    Order = tmpOrder,
                    Product = _productRepository.GetById(line.Product.Id),
                    Quantity = line.Quantity
                };
                _orderProductRepository.Add(orderProduct);
            }
            _unitOfWork.Commit();
        }
    }
}
