using TAiMStore.Domain;
using TAiMStore.Model.Factory;

namespace TAiMStore.Model.Repository
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(IFactory databaseFactory)
            : base(databaseFactory)
        {
        }

    }
}
