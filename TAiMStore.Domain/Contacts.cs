using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace TAiMStore.Domain
{
    public class Contacts: Entity
    {
        [ForeignKey("User")]
        public int Id { get; set; }
        public string PersonFullName { get; set; }
        public string Organization { get; set; }
        public string PostZip { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Room { get; set; }
        public string Telephone { get; set; }
        
        //связи
        public virtual  User User { get; set; }
    }
}
