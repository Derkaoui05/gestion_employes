using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionEmployes.Models
{
    public class Facture
    {
        public int Id { get; set; }
        public string Number { get; set; } // NFacture
        public int SupplierId { get; set; }
        public decimal Amount { get; set; } // Montant facture
        public decimal Advance { get; set; } // Avance
        public decimal Remaining => Amount - Advance; // Reste à payer
        public DateTime InvoiceDate { get; set; } // Date de facture
        public DateTime DueDate { get; set; } // Date d'échéance
        public string Status
        {
            get
            {
                if (Remaining == Amount)
                    return "Non payée";  // Aucune avance
                else if (Remaining > 0)
                    return "En cours";   // Avance partielle
                else
                    return "Payée";      // Tout payé
            }
        }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
