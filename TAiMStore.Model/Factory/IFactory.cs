using System;

namespace TAiMStore.Model.Factory
{
    public interface IFactory : IDisposable
    {
        StoreContext Get();
    }
}
