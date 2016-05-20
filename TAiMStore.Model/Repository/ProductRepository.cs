using TAiMStore.Domain;
using TAiMStore.Model.Factory;

namespace TAiMStore.Model.Repository
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(IFactory databaseFactory)
            : base(databaseFactory)
        {
        }

    }
}
