using GestionEmployes.Data;
using GestionEmployes.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Data.Entity;
using System.Linq;

namespace GestionEmployes.Services
{
    public class AbsenceService : IAbsenceService
    {
        private readonly ApplicationDbContext _context;
        public AbsenceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Absence> CreateAbsenceAsync(Absence absence)
        {
            if (absence.Employe == null)
            {
                throw new ArgumentException("L'employé est obligatoire.");
            }

            if (absence.Penalite < 0)
            {
                throw new ArgumentException("La pénalité ne peut pas être négative.");
            }

            // S'assurer d'utiliser un Employe existant (éviter insertion involontaire)
            if (absence.Employe != null)
            {
                absence.EmployeCin = absence.Employe.Cin;
                _context.Employes.Attach(absence.Employe);
            }

            // Détacher la navigation pour éviter une réinsertion
            absence.Employe = null;

            _context.Absences.Add(absence);
            await _context.SaveChangesAsync();
            return absence;
        }

        public async Task<Absence> GetAbsenceByIdAsync(long id)
        {
            return await _context.Absences
                .Include(a => a.Employe)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Absence>> GetAllAbsencesAsync()
        {
            return await _context.Absences
                .Include(a => a.Employe)
                .OrderByDescending(a => a.DateAbsence)
                .ToListAsync();
        }

        public async Task<List<Absence>> GetAbsencesByEmployeAsync(string employeCin)
        {
            return await _context.Absences
                .Include(a => a.Employe)
                .Where(a => a.EmployeCin == employeCin)
                .OrderByDescending(a => a.DateAbsence)
                .ToListAsync();
        }

        public async Task<List<Absence>> GetAbsencesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Absences
                .Include(a => a.Employe)
                .Where(a => a.DateAbsence >= startDate && a.DateAbsence <= endDate)
                .OrderByDescending(a => a.DateAbsence)
                .ToListAsync();
        }

        public async Task<Absence> UpdateAbsenceAsync(Absence absence)
        {
            var existingAbsence = await _context.Absences.FindAsync(absence.Id);
            if (existingAbsence == null)
            {
                throw new ArgumentException($"Absence non trouvée avec l'ID: {absence.Id}");
            }

            existingAbsence.Penalite = absence.Penalite;
            existingAbsence.DateAbsence = absence.DateAbsence;
            existingAbsence.EmployeCin = absence.EmployeCin;

            await _context.SaveChangesAsync();
            return existingAbsence;
        }

        public async Task DeleteAbsenceAsync(long id)
        {
            var absence = await _context.Absences.FindAsync(id);
            if (absence == null)
            {
                throw new ArgumentException($"Absence non trouvée avec l'ID: {id}");
            }

            _context.Absences.Remove(absence);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalPenalitesByEmployeAndDateRangeAsync(string employeCin, DateTime startDate, DateTime endDate)
        {
            // Sum as nullable then coalesce to 0 to avoid invalid cast when result is null
            var sum = await _context.Absences
                .Where(a => a.EmployeCin == employeCin && a.DateAbsence >= startDate && a.DateAbsence <= endDate)
                .Select(a => (decimal?)a.Penalite)
                .SumAsync();

            return sum ?? 0m;
        }

        public async Task<int> CountAbsencesByEmployeAndDateRangeAsync(string employeCin, DateTime startDate, DateTime endDate)
        {
            return await _context.Absences
                .CountAsync(a => a.EmployeCin == employeCin && a.DateAbsence >= startDate && a.DateAbsence <= endDate);
        }
    }
}