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

        public async Task<Avance> CreateAvanceAsync(Avance avance)
        {
            if (avance.Employe == null)
            {
                throw new ArgumentException("L'employé est obligatoire.");
            }

            if (avance.Montant <= 0)
            {
                throw new ArgumentException("Le montant doit être positif.");
            }

            // S'assurer d'utiliser un Employe existant (éviter insertion involontaire)
            if (avance.Employe != null)
            {
                avance.EmployeCin = avance.Employe.Cin;
                // Attacher l'employé existant au contexte sans le modifier
                _context.Employes.Attach(avance.Employe);
            }

            // Détacher la navigation pour éviter une réinsertion
            avance.Employe = null;

            _context.Avances.Add(avance);
            await _context.SaveChangesAsync();
            return avance;
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