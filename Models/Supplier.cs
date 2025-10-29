using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionEmployes.Models
{
    public class Supplier
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation property
        public virtual ICollection<Facture> Factures { get; set; }

    }
}
