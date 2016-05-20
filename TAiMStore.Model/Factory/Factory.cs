namespace TAiMStore.Model.Factory
{
    public class Factory : Disposable, IFactory
    {
        private StoreContext _dataContext;

        public StoreContext Get()
        {
            return _dataContext ?? (_dataContext = new StoreContext());
        }

        protected override void DisposeCore()
        {
            if (_dataContext != null)
            {
                _dataContext.Dispose();
            }
        }
    }
}
