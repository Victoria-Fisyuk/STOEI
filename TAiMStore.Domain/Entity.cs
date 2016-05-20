using System;
using System.Web.Mvc;

namespace TAiMStore.Domain
{
    public class Entity
    {
        [HiddenInput(DisplayValue = false)]
        public DateTime CreateDate { get; set; }

        [HiddenInput(DisplayValue = false)]
        public Nullable<DateTime> ModifiedDate { get; set; }
    }
}
