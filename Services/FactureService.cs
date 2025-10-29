using GestionEmployes.Data;
using GestionEmployes.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace GestionEmployes.Services
{
    public class FactureService
    {
        private ApplicationDbContext _context;

        public FactureService()
        {
            try
            {
                _context = DatabaseHelper.CreateNewContext();
                if (_context == null)
                    throw new Exception("Impossible de créer le contexte de base de données");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur initialisation FactureService: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                throw;
            }
        }

        // ==================== CRUD METHODS ====================

        public List<Facture> GetAllFactures()
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Facture>()
                    .Include(f => f.Supplier)
                    .Include(f => f.Transactions)
                    .OrderByDescending(f => f.InvoiceDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetAllFactures: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                return new List<Facture>();
            }
        }

        public List<Facture> GetFacturesBySupplier(int supplierId)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Facture>()
                    .Include(f => f.Supplier)
                    .Include(f => f.Transactions)
                    .Where(f => f.SupplierId == supplierId)
                    .OrderByDescending(f => f.InvoiceDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetFacturesBySupplier: {ex.Message}");
                return new List<Facture>();
            }
        }

        public Facture GetFactureById(int id)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Facture>()
                    .Include(f => f.Supplier)
                    .Include(f => f.Transactions)
                    .FirstOrDefault(f => f.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetFactureById: {ex.Message}");
                return null;
            }
        }

        public bool AddFacture(Facture facture)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                if (facture == null)
                    throw new ArgumentNullException(nameof(facture), "La facture ne peut pas être null");

                facture.CreatedDate = DateTime.Now;
                _context.Set<Facture>().Add(facture);
                _context.SaveChanges();

                Console.WriteLine($"✅ Facture ajoutée: {facture.Number}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur AddFacture: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                return false;
            }
        }

        public bool UpdateFacture(Facture facture)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                var existing = _context.Set<Facture>().Find(facture.Id);
                if (existing == null)
                {
                    Console.WriteLine($"❌ Facture non trouvée: {facture.Id}");
                    return false;
                }

                existing.Number = facture.Number;
                existing.SupplierId = facture.SupplierId;
                existing.Amount = facture.Amount;
                existing.Advance = facture.Advance;
                existing.InvoiceDate = facture.InvoiceDate;
                existing.DueDate = facture.DueDate;
                existing.Notes = facture.Notes;

                _context.SaveChanges();
                Console.WriteLine($"✅ Facture modifiée: {facture.Number}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur UpdateFacture: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                return false;
            }
        }

        public bool DeleteFacture(int id)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                var facture = _context.Set<Facture>().Find(id);
                if (facture == null)
                {
                    Console.WriteLine($"❌ Facture non trouvée: {id}");
                    return false;
                }

                _context.Set<Facture>().Remove(facture);
                _context.SaveChanges();

                Console.WriteLine($"✅ Facture supprimée: {facture.Number}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur DeleteFacture: {ex.Message}");
                return false;
            }
        }

        // ==================== SEARCH & STATISTICS ====================

        public decimal GetTotalFacturesBySupplier(int supplierId)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Facture>()
                    .Where(f => f.SupplierId == supplierId)
                    .Sum(f => (decimal?)f.Amount) ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetTotalFacturesBySupplier: {ex.Message}");
                return 0;
            }
        }

        public decimal GetTotalPaidBySupplier(int supplierId)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Facture>()
                    .Where(f => f.SupplierId == supplierId)
                    .Sum(f => (decimal?)f.Advance) ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetTotalPaidBySupplier: {ex.Message}");
                return 0;
            }
        }

        public int CountFactures()
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Facture>().Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur CountFactures: {ex.Message}");
                return 0;
            }
        }

        public List<Facture> GetUnpaidFactures()
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Facture>()
                    .Include(f => f.Supplier)
                    .Where(f => f.Remaining > 0)
                    .OrderBy(f => f.DueDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetUnpaidFactures: {ex.Message}");
                return new List<Facture>();
            }
        }

        public List<Facture> GetOverdueFactures()
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Facture>()
                    .Include(f => f.Supplier)
                    .Where(f => f.DueDate < DateTime.Today && f.Remaining > 0)
                    .OrderBy(f => f.DueDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetOverdueFactures: {ex.Message}");
                return new List<Facture>();
            }
        }
    }
}