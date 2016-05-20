using TAiMStore.Model.Factory;

namespace TAiMStore.Model.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IFactory _databaseFactory;
        private StoreContext _context;

        public UnitOfWork(IFactory databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }

        protected StoreContext Context
        {
            get { return _context ?? (_context = _databaseFactory.Get()); }
        }

        public void Commit()
        {
            Context.Commit();
        }
    }
}
