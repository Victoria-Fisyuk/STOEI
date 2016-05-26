using System.Collections.Generic;
namespace TAiMStore.Domain
{
    public class PaymentType : Entity
    {
        public int Id { get; set; }
        public string NameMethod { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
