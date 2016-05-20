namespace TAiMStore.Model.Abstract
{
    public interface IAuthProvider
    {
        bool Authenticate(string username, string password);
    }
}
