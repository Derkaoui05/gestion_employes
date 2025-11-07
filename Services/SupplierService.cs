using GestionEmployes.Data;
using GestionEmployes.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace GestionEmployes.Services
{
    public class SupplierService
    {
        private ApplicationDbContext _context;

        public SupplierService()
        {
            try
            {
                _context = DatabaseHelper.CreateNewContext();
                if (_context == null)
                    throw new Exception("Impossible de créer le contexte de base de données");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur initialisation SupplierService: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                throw;
            }
        }

        // ==================== CRUD METHODS ====================

        public List<Supplier> GetAllSuppliers()
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Supplier>()
                    .AsNoTracking()
                    .Include(s => s.Factures)
                    .OrderBy(s => s.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetAllSuppliers: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                return new List<Supplier>();
            }
        }

        public Supplier GetSupplierById(int id)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Supplier>()
                    .AsNoTracking()
                    .Include(s => s.Factures)
                    .FirstOrDefault(s => s.ID == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetSupplierById: {ex.Message}");
                return null;
            }
        }

        public bool AddSupplier(Supplier supplier)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                if (supplier == null)
                    throw new ArgumentNullException(nameof(supplier), "Le fournisseur ne peut pas être null");

                supplier.CreatedDate = DateTime.Now;
                supplier.IsActive = true;

                _context.Set<Supplier>().Add(supplier);
                _context.SaveChanges();

                Console.WriteLine($"✅ Fournisseur ajouté: {supplier.Name}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur AddSupplier: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                return false;
            }
        }

        public bool UpdateSupplier(Supplier supplier)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                var existing = _context.Set<Supplier>().Find(supplier.ID);
                if (existing == null)
                {
                    Console.WriteLine($"❌ Fournisseur non trouvé: {supplier.ID}");
                    return false;
                }

                existing.Name = supplier.Name;
                existing.Contact = supplier.Contact;
                existing.Phone = supplier.Phone;
                existing.Email = supplier.Email;
                existing.Address = supplier.Address;
                existing.IsActive = supplier.IsActive;

                _context.SaveChanges();
                Console.WriteLine($"✅ Fournisseur modifié: {supplier.Name}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur UpdateSupplier: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                return false;
            }
        }

        public bool DeleteSupplier(int id)
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                var supplier = _context.Set<Supplier>().Find(id);
                if (supplier == null)
                {
                    Console.WriteLine($"❌ Fournisseur non trouvé: {id}");
                    return false;
                }

                _context.Set<Supplier>().Remove(supplier);
                _context.SaveChanges();

                Console.WriteLine($"✅ Fournisseur supprimé: {supplier.Name}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur DeleteSupplier: {ex.Message}");
                return false;
            }
        }

        // ==================== SEARCH & STATISTICS ====================

        public List<Supplier> GetActiveSuppliers()
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Supplier>()
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetActiveSuppliers: {ex.Message}");
                return new List<Supplier>();
            }
        }

        public int CountSuppliers()
        {
            try
            {
                if (_context == null)
                    _context = DatabaseHelper.CreateNewContext();

                return _context.Set<Supplier>().Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur CountSuppliers: {ex.Message}");
                return 0;
            }
        }

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
    }
}