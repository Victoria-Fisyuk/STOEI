using System.Collections.Generic;

namespace TAiMStore.Domain
{
    public class Order : Entity
    {
        public int Id { get; set; }
        public decimal TotalCost { get; set; }
        public bool? Status { get; set; }

        public virtual User User { get; set; }
        public virtual PaymentType Payment { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; }

    }
}
