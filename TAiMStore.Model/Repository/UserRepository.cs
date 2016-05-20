using TAiMStore.Domain;
using TAiMStore.Model.Factory;

namespace TAiMStore.Model.Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(IFactory databaseFactory)
            : base(databaseFactory)
        {
        }

    }
}
