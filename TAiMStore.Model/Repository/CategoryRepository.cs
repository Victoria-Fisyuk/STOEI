using TAiMStore.Domain;
using TAiMStore.Model.Factory;

namespace TAiMStore.Model.Repository
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(IFactory databaseFactory)
            : base(databaseFactory)
        {
        }

    }
}
