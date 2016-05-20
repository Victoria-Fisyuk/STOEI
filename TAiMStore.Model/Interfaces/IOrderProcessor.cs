using TAiMStore.Model.ViewModels;

namespace TAiMStore.Model.Interfaces
{
    public interface IOrderProcessor
    {
        void ProcessOrder(Cart cart, ShippingDetails shippingDetails);
    }
}