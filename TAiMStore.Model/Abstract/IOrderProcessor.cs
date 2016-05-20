using TAiMStore.Model.ViewModels;

namespace TAiMStore.Model.Abstract
{
    public interface IOrderProcessor
    {
        void ProcessOrder(Cart cart, ShippingDetails shippingDetails);
    }
}