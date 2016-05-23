using System.Collections.Generic;
using System.Web.Mvc;
using System.Threading.Tasks;
using TAiMStore.Domain;
using TAiMStore.Model.Repository;
using TAiMStore.Model.UnitOfWork;
using TAiMStore.Model.ViewModels;

namespace TAiMStore.Model.Classes
{
    public class PaymentManager
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentManager(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
        }

        
        #region GetPayments
        /// <summary>
        /// получаем типы оплаты по ID
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        public PaymentViewModel GetPaymentViewModel(int paymentId)
        {
            var entity = _paymentRepository.Get(c => c.Id == paymentId);

            return new PaymentViewModel { Id = entity.Id, PaymentMethod = entity.NameMethod };
        }


        public List<PaymentViewModel> GetPayments()
        {
            var list = new List<PaymentViewModel>();
            var payments = _paymentRepository.GetAll();

            foreach (var payment in payments)
            {
                var tmp = new PaymentViewModel();
                tmp.PaymentMethod = payment.NameMethod;
                tmp.Id = payment.Id;
                list.Add(tmp);
            }

            return list;
        }

        public PaymentType GetPaymentByName(string name)
        {
            return _paymentRepository.Get(u => u.NameMethod == name);
        }

        #endregion

        public void DeletePayment(int paymentId)
        {
            var payment = _paymentRepository.Get(c => c.Id == paymentId);
            _paymentRepository.Delete(payment);
            _unitOfWork.Commit();
        }

        public void AddPayment(string nameMethod)
        {
            var payment = new PaymentType { NameMethod = nameMethod };
            _paymentRepository.Add(payment);
            _unitOfWork.Commit();
        }

        public void EditPaymenty(string nameMethod, int paymentId)
        {
            var payment = _paymentRepository.Get(c => c.Id == paymentId);
            payment.NameMethod = nameMethod;
            _paymentRepository.Update(payment);
            _unitOfWork.Commit();
        }
    }
}
