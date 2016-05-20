using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAiMStore.Model.ViewModels
{
    public class ProfileViewModel
    {
        public string PersonFullName { get; set; }
        public string Organization { get; set; }
        public string PostZip { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Room { get; set; }
        public string Telephone { get; set; }

        public string Email { get; set; }
        public string EmailForTextBox { get; set; }
    }
}
