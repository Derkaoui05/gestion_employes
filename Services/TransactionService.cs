using GestionEmployes.Data;
using GestionEmployes.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace GestionEmployes.Services
{
    public class TransactionService
    {
        private ApplicationDbContext _context;

        public TransactionService()
        {
            try
            {
                _context = DatabaseHelper.CreateNewContext();
                if (_context == null)
                    throw new Exception("Impossible de créer le contexte de base de données");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur initialisation TransactionService: {ex.Message}");
                throw;
            }
        }

        // ==================== CRUD METHODS ====================

        public List<Transaction> GetAllTransactions()
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Transaction>()
                    .AsNoTracking()
                    .Include(t => t.Facture)
                    .Include(t => t.Facture.Supplier)
                    .OrderByDescending(t => t.TransactionDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetAllTransactions: {ex.Message}");
                return new List<Transaction>();
            }
        }

        public List<Transaction> GetTransactionsByFacture(int factureId)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Transaction>()
                    .AsNoTracking()
                    .Include(t => t.Facture)
                    .Where(t => t.FactureId == factureId)
                    .OrderByDescending(t => t.TransactionDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetTransactionsByFacture: {ex.Message}");
                return new List<Transaction>();
            }
        }

        public Transaction GetTransactionById(int id)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Transaction>()
                    .AsNoTracking()
                    .Include(t => t.Facture)
                    .FirstOrDefault(t => t.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetTransactionById: {ex.Message}");
                return null;
            }
        }

        public bool AddTransaction(Transaction transaction)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                if (transaction == null)
                    throw new ArgumentNullException(nameof(transaction));

                // Vérifier que la facture existe
                var facture = _context.Set<Facture>().Find(transaction.FactureId);
                if (facture == null)
                {
                    Console.WriteLine($"❌ Facture non trouvée: {transaction.FactureId}");
                    return false;
                }

                // Vérifier que le paiement ne dépasse pas le reste à payer
                decimal totalPaid = GetTotalPaidByFacture(transaction.FactureId);
                decimal remaining = facture.Amount - totalPaid;

                if (transaction.Amount > remaining)
                {
                    Console.WriteLine($"❌ Le montant du paiement ({transaction.Amount}) dépasse le reste à payer ({remaining})");
                    return false;
                }

                transaction.CreatedDate = DateTime.Now;
                _context.Set<Transaction>().Add(transaction);
                _context.SaveChanges();

                // Mettre à jour l'avance totale de la facture
                UpdateFactureAdvance(transaction.FactureId);

                Console.WriteLine($"✅ Transaction ajoutée: {transaction.Amount} DH");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur AddTransaction: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                return false;
            }
        }

        public bool UpdateTransaction(Transaction transaction)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                var existing = _context.Set<Transaction>().Find(transaction.Id);
                if (existing == null)
                {
                    Console.WriteLine($"❌ Transaction non trouvée: {transaction.Id}");
                    return false;
                }

                // Vérifier que le nouveau montant ne dépasse pas le reste à payer
                var facture = _context.Set<Facture>().Find(transaction.FactureId);
                if (facture == null)
                {
                    Console.WriteLine($"❌ Facture non trouvée: {transaction.FactureId}");
                    return false;
                }

                decimal totalPaidExcludingCurrent = GetTotalPaidByFacture(transaction.FactureId) - existing.Amount;
                decimal remaining = facture.Amount - totalPaidExcludingCurrent;

                if (transaction.Amount > remaining)
                {
                    Console.WriteLine($"❌ Le nouveau montant ({transaction.Amount}) dépasse le reste à payer ({remaining})");
                    return false;
                }

                existing.Amount = transaction.Amount;
                existing.TransactionDate = transaction.TransactionDate;
                existing.Description = transaction.Description;
                existing.PaymentMethod = transaction.PaymentMethod;
                existing.Reference = transaction.Reference;
                existing.Type = transaction.Type;

                _context.SaveChanges();

                // Mettre à jour l'avance totale de la facture
                UpdateFactureAdvance(transaction.FactureId);

                Console.WriteLine($"✅ Transaction modifiée: {transaction.Amount} DH");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur UpdateTransaction: {ex.Message}");
                return false;
            }
        }

        public bool DeleteTransaction(int id)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                var transaction = _context.Set<Transaction>().Find(id);
                if (transaction == null)
                {
                    Console.WriteLine($"❌ Transaction non trouvée: {id}");
                    return false;
                }

                int factureId = transaction.FactureId;
                _context.Set<Transaction>().Remove(transaction);
                _context.SaveChanges();

                // Mettre à jour l'avance totale de la facture
                UpdateFactureAdvance(factureId);

                Console.WriteLine($"✅ Transaction supprimée");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur DeleteTransaction: {ex.Message}");
                return false;
            }
        }

        // ==================== BUSINESS LOGIC ====================

        /// <summary>
        /// Recalcule et met à jour l'avance totale d'une facture en fonction de toutes ses transactions
        /// </summary>
        private void UpdateFactureAdvance(int factureId)
        {
            try
            {
                var facture = _context.Set<Facture>().Find(factureId);
                if (facture == null)
                {
                    Console.WriteLine($"❌ Facture non trouvée: {factureId}");
                    return;
                }

                // Calculer la somme de toutes les transactions de type "Paiement" ou "Avance"
                decimal totalPaid = _context.Set<Transaction>()
                    .Where(t => t.FactureId == factureId)
                    .Sum(t => (decimal?)t.Amount) ?? 0;

                facture.Advance = totalPaid;
                _context.SaveChanges();

                Console.WriteLine($"✅ Avance mise à jour pour facture {factureId}: {totalPaid} DH");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur UpdateFactureAdvance: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtient le total payé pour une facture
        /// </summary>
        public decimal GetTotalPaidByFacture(int factureId)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Transaction>()
                    .Where(t => t.FactureId == factureId)
                    .Sum(t => (decimal?)t.Amount) ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetTotalPaidByFacture: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Obtient le reste à payer pour une facture
        /// </summary>
        public decimal GetRemainingByFacture(int factureId)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                var facture = _context.Set<Facture>().AsNoTracking().FirstOrDefault(f => f.Id == factureId);
                if (facture == null)
                    return 0;

                decimal totalPaid = GetTotalPaidByFacture(factureId);
                return facture.Amount - totalPaid;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetRemainingByFacture: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Compte le nombre de transactions pour une facture
        /// </summary>
        public int CountTransactionsByFacture(int factureId)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Transaction>()
                    .Count(t => t.FactureId == factureId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur CountTransactionsByFacture: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Obtient les statistiques des transactions par méthode de paiement
        /// </summary>
        public Dictionary<string, decimal> GetPaymentMethodStats(int factureId)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Transaction>()
                    .Where(t => t.FactureId == factureId)
                    .GroupBy(t => t.PaymentMethod)
                    .ToDictionary(
                        g => g.Key ?? "Non spécifié",
                        g => g.Sum(t => t.Amount)
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetPaymentMethodStats: {ex.Message}");
                return new Dictionary<string, decimal>();
            }
        }
    }
}