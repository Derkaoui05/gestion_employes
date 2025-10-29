using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionEmployes.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int FactureId { get; set; }
        public string EmployeeCin { get; set; } // Nullable for supplier transactions
        public string Type { get; set; } // "Paiement", "Avance", "Réglement"
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public string PaymentMethod { get; set; } // "Espèces", "Chèque", "Virement"
        public string Reference { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public virtual Facture Facture { get; set; }
        public virtual Employe Employee { get; set; }
    }
}
