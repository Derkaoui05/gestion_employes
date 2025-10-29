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

        // 🔹 Ajouter une absence
        public bool AjouterAbsence(Absence absence)
        {
            try
            {
                var nouvelleAbsence = new Absence
                {
                    Penalite = absence.Penalite,
                    DateAbsence = absence.DateAbsence,
                    EmployeCin = absence.EmployeCin
                };

                _context.Absences.Add(nouvelleAbsence);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout de l'absence : {ex.Message}");
                return false;
            }
        }

        // 🔹 Récupérer toutes les absences
        public List<Absence> GetAllAbsences()
        {
            try
            {
                return _context.Absences
                    .Include(a => a.Employe)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des absences : {ex.Message}");
                return new List<Absence>();
            }
        }

        // 🔹 Récupérer les absences d’un employé
        public List<Absence> GetAbsencesByEmploye(string employeCin)
        {
            return _context.Absences
                .Include(a => a.Employe)
                .Where(a => a.EmployeCin == employeCin)
                .ToList();
        }

        // 🔹 Supprimer une absence
        public bool SupprimerAbsence(int id)
        {
            try
            {
                var absence = _context.Absences.Find(id);
                if (absence == null) return false;

                _context.Absences.Remove(absence);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la suppression de l'absence : {ex.Message}");
                return false;
            }
        }

        public async Task<Absence> CreateAbsenceAsync(Absence absence)
        {
            if (string.IsNullOrEmpty(absence.EmployeCin))
            {
                throw new ArgumentException("Le CIN de l'employé est obligatoire.");
            }

            if (absence.Penalite < 0)
            {
                throw new ArgumentException("La pénalité ne peut pas être négative.");
            }

            // ✅ Détacher toute instance d'Absence déjà suivie avec le même ID
            var existingAbsence = _context.ChangeTracker.Entries<Absence>()
                .FirstOrDefault(e => e.Entity.Id == absence.Id);

            if (existingAbsence != null)
            {
                existingAbsence.State = EntityState.Detached;
            }

            // ✅ Créer une nouvelle instance
            var nouvelleAbsence = new Absence
            {
                DateAbsence = absence.DateAbsence,
                Penalite = absence.Penalite,
                EmployeCin = absence.EmployeCin
            };

            // ✅ Vérifier que l'employé existe
            var employeExiste = await _context.Employes
                .AnyAsync(e => e.Cin == nouvelleAbsence.EmployeCin);

            if (!employeExiste)
            {
                throw new ArgumentException($"L'employé avec CIN {nouvelleAbsence.EmployeCin} n'existe pas.");
            }

            _context.Absences.Add(nouvelleAbsence);
            await _context.SaveChangesAsync();
            return nouvelleAbsence;
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