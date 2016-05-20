using TAiMStore.Domain;
using TAiMStore.Model.Factory;

namespace TAiMStore.Model.Repository
{
    public class PaymentRepository : RepositoryBase<PaymentType>, IPaymentRepository
    {
        public PaymentRepository(IFactory databaseFactory)
            : base(databaseFactory)
        {

        }
    }
}
