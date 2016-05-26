using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace TAiMStore.Domain
{
    public class OrderProduct : Entity
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        public int Quantity { get; set; }

        public virtual Product Product { get; set; }
        public virtual Order Order { get; set; }
    }
}
