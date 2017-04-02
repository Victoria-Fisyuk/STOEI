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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderProcessor _orderProcessor;

        public ShipingManager(IOrderRepository orderRepository, IOrderProductRepository orderProductRepository,
            IProductRepository productRepository, IPaymentRepository paymentRepository, IOrderProcessor orderProcessor,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _orderProductRepository = orderProductRepository;
            _productRepository = productRepository;
            _paymentRepository = paymentRepository;
            _orderProcessor = orderProcessor;
            _unitOfWork = unitOfWork;
        }

        public Order CheckOut(IEnumerable<CartLine> lines, string payment, decimal totalCost, User user)
        {
            var tmpOrder = SetOrder(user, totalCost, payment);
            SetOrderProduct(lines, tmpOrder);
            _orderProcessor.ProcessOrder(lines, tmpOrder);
            return tmpOrder;
        }

        public Order SetOrder(User user, decimal totalCost, string payment)
        {
            var paymentManager = new PaymentManager(_paymentRepository, _unitOfWork);
            var order = new Order()
            {
                User = user,
                TotalCost = totalCost,
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
