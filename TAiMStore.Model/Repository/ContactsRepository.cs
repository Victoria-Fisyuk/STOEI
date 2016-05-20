using TAiMStore.Domain;
using TAiMStore.Model.Factory;

namespace TAiMStore.Model.Repository
{

    public class ContactsRepository : RepositoryBase<Contacts>, IContactsRepository
    {
        public ContactsRepository(IFactory databaseFactory)
            : base(databaseFactory)
        {
        }

    }
}
