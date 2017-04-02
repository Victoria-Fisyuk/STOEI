using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAiMStore.Domain;
using TAiMStore.Model.ViewModels;

namespace TAiMStore.Model.Repository
{
    public interface IOrderProcessor
    {
        void ProcessOrder(IEnumerable<CartLine> lines, Order orderDetails);
    }
}
