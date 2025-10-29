using GestionEmployes.Data;
using GestionEmployes.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Data.Entity;
using System.Linq;

namespace GestionEmployes.Services
{
    public class AvanceService : IAvanceService
    {
        private readonly ApplicationDbContext _context;

        public AvanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔹 Ajouter une avance (version synchrone)
        public bool AjouterAvance(Avance avance)
        {
            try
            {
                // ✅ Créer une nouvelle instance pour éviter les conflits
                var nouvelleAvance = new Avance
                {
                    Montant = avance.Montant,
                    DateAvance = avance.DateAvance,
                    EmployeCin = avance.EmployeCin
                };

                _context.Avances.Add(nouvelleAvance);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout de l'avance : {ex.Message}");
                return false;
            }
        }

        // 🔹 Récupérer toutes les avances
        public List<Avance> GetAllAvances()
        {
            try
            {
                return _context.Avances
                    .Include(a => a.Employe)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des avances : {ex.Message}");
                return new List<Avance>();
            }
        }

        // 🔹 Récupérer les avances d’un employé
        public List<Avance> GetAvancesByEmploye(string employeCin)
        {
            return _context.Avances
                .Include(a => a.Employe)
                .Where(a => a.EmployeCin == employeCin)
                .ToList();
        }

        // 🔹 Supprimer une avance
        public bool SupprimerAvance(int id)
        {
            try
            {
                var avance = _context.Avances.Find(id);
                if (avance == null) return false;

                _context.Avances.Remove(avance);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la suppression de l'avance : {ex.Message}");
                return false;
            }
        }


        // 🔹 Créer une avance (version asynchrone - CORRIGÉE)
        public async Task<Avance> CreateAvanceAsync(Avance avance)
        {
            if (string.IsNullOrEmpty(avance.EmployeCin))
            {
                throw new ArgumentException("Le CIN de l'employé est obligatoire.");
            }

            if (avance.Montant <= 0)
            {
                throw new ArgumentException("Le montant doit être positif.");
            }

            // ✅ Vérifier que l'employé existe
            var employeExiste = await _context.Employes
                .AnyAsync(e => e.Cin == avance.EmployeCin);

            if (!employeExiste)
            {
                throw new ArgumentException($"L'employé avec CIN {avance.EmployeCin} n'existe pas.");
            }

            // ✅ Créer une NOUVELLE instance
            var nouvelleAvance = new Avance
            {
                Montant = avance.Montant,
                DateAvance = avance.DateAvance,
                EmployeCin = avance.EmployeCin
            };

            _context.Avances.Add(nouvelleAvance);
            await _context.SaveChangesAsync();
            return nouvelleAvance;
        }

        public async Task<Avance> GetAvanceByIdAsync(long id)
        {
            return await _context.Avances
                .Include(a => a.Employe)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Avance>> GetAllAvancesAsync()
        {
            return await _context.Avances
                .Include(a => a.Employe)
                .OrderByDescending(a => a.DateAvance)
                .ToListAsync();
        }

        public async Task<List<Avance>> GetAvancesByEmployeAsync(string employeCin)
        {
            return await _context.Avances
                .Include(a => a.Employe)
                .Where(a => a.EmployeCin == employeCin)
                .OrderByDescending(a => a.DateAvance)
                .ToListAsync();
        }
        public async Task<List<Avance>> GetAvancesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Avances
                .Include(a => a.Employe)
                .Where(a => a.DateAvance >= startDate && a.DateAvance <= endDate)
                .OrderByDescending(a => a.DateAvance)
                .ToListAsync();
        }

        public async Task<Avance> UpdateAvanceAsync(Avance avance)
        {
            var existingAvance = await _context.Avances.FindAsync(avance.Id);
            if (existingAvance == null)
            {
                throw new ArgumentException($"Avance non trouvée avec l'ID: {avance.Id}");
            }

            existingAvance.Montant = avance.Montant;
            existingAvance.DateAvance = avance.DateAvance;
            existingAvance.EmployeCin = avance.EmployeCin;

            await _context.SaveChangesAsync();
            return existingAvance;
        }

        public async Task DeleteAvanceAsync(long id)
        {
            var avance = await _context.Avances.FindAsync(id);
            if (avance == null)
            {
                throw new ArgumentException($"Avance non trouvée avec l'ID: {id}");
            }

            _context.Avances.Remove(avance);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalAvancesByEmployeAndDateRangeAsync(string employeCin, DateTime startDate, DateTime endDate)
        {
            // Sum as nullable, then coalesce to 0 to avoid invalid cast when result is null
            var sum = await _context.Avances
                .Where(a => a.EmployeCin == employeCin && a.DateAvance >= startDate && a.DateAvance <= endDate)
                .Select(a => (decimal?)a.Montant)
                .SumAsync();

            return sum ?? 0m;
        }

    }
}