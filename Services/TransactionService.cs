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
                Console.WriteLine($"Stack: {ex.StackTrace}");
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
                    .Include(t => t.Facture)
                    .Include(t => t.Employee)
                    .OrderByDescending(t => t.TransactionDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetAllTransactions: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
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

        public List<Transaction> GetTransactionsByEmployee(string employeeCin)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Transaction>()
                    .Include(t => t.Employee)
                    .Where(t => t.EmployeeCin == employeeCin)
                    .OrderByDescending(t => t.TransactionDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetTransactionsByEmployee: {ex.Message}");
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
                    .Include(t => t.Facture)
                    .Include(t => t.Employee)
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
                    throw new ArgumentNullException(nameof(transaction), "La transaction ne peut pas être null");

                transaction.CreatedDate = DateTime.Now;
                _context.Set<Transaction>().Add(transaction);
                _context.SaveChanges();

                Console.WriteLine($"✅ Transaction ajoutée: {transaction.Amount} DH");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur AddTransaction: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
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

                existing.FactureId = transaction.FactureId;
                existing.Type = transaction.Type;
                existing.Amount = transaction.Amount;
                existing.TransactionDate = transaction.TransactionDate;
                existing.PaymentMethod = transaction.PaymentMethod;
                existing.Reference = transaction.Reference;
                existing.Description = transaction.Description;

                _context.SaveChanges();
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

                _context.Set<Transaction>().Remove(transaction);
                _context.SaveChanges();

                Console.WriteLine($"✅ Transaction supprimée: {transaction.Amount} DH");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur DeleteTransaction: {ex.Message}");
                return false;
            }
        }

        // ==================== SEARCH & STATISTICS ====================

        public decimal GetTotalByFacture(int factureId)
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
                Console.WriteLine($"❌ Erreur GetTotalByFacture: {ex.Message}");
                return 0;
            }
        }

        public decimal GetTotalBySupplier(int supplierId)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                var factureIds = _context.Set<Facture>()
                    .Where(f => f.SupplierId == supplierId)
                    .Select(f => f.Id)
                    .ToList();

                return _context.Set<Transaction>()
                    .Where(t => factureIds.Contains(t.FactureId))
                    .Sum(t => (decimal?)t.Amount) ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetTotalBySupplier: {ex.Message}");
                return 0;
            }
        }

        public decimal GetTotalByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Transaction>()
                    .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                    .Sum(t => (decimal?)t.Amount) ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetTotalByDateRange: {ex.Message}");
                return 0;
            }
        }

        public int CountTransactions()
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Transaction>().Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur CountTransactions: {ex.Message}");
                return 0;
            }
        }

        public Dictionary<string, decimal> GetTotalByPaymentMethod()
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Transaction>()
                    .GroupBy(t => t.PaymentMethod)
                    .Select(g => new { Method = g.Key, Total = g.Sum(t => t.Amount) })
                    .ToDictionary(x => x.Method ?? "Non spécifié", x => x.Total);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetTotalByPaymentMethod: {ex.Message}");
                return new Dictionary<string, decimal>();
            }
        }

        public Dictionary<string, decimal> GetTotalByTransactionType()
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Transaction>()
                    .GroupBy(t => t.Type)
                    .Select(g => new { Type = g.Key, Total = g.Sum(t => t.Amount) })
                    .ToDictionary(x => x.Type ?? "Non spécifié", x => x.Total);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetTotalByTransactionType: {ex.Message}");
                return new Dictionary<string, decimal>();
            }
        }

        public List<Transaction> GetTransactionsByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Transaction>()
                    .Include(t => t.Facture)
                    .Include(t => t.Employee)
                    .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                    .OrderByDescending(t => t.TransactionDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetTransactionsByDateRange: {ex.Message}");
                return new List<Transaction>();
            }
        }

        public List<Transaction> SearchByReference(string reference)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Transaction>()
                    .Include(t => t.Facture)
                    .Where(t => t.Reference != null && t.Reference.Contains(reference))
                    .OrderByDescending(t => t.TransactionDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur SearchByReference: {ex.Message}");
                return new List<Transaction>();
            }
        }

        public List<Transaction> GetRecentTransactions(int count = 10)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Transaction>()
                    .Include(t => t.Facture)
                    .Include(t => t.Employee)
                    .OrderByDescending(t => t.TransactionDate)
                    .Take(count)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetRecentTransactions: {ex.Message}");
                return new List<Transaction>();
            }
        }
    }
}