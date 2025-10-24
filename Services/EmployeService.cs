using GestionEmployes.Data;
using GestionEmployes.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionEmployes.Services
{
    internal class EmployeService:IEmployeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Employe> CreateEmployeAsync(Employe employe)
        {
            // Validate CIN uniqueness
            if (await CinExistsAsync(employe.Cin))
            {
                throw new ArgumentException($"Un employé avec le CIN {employe.Cin} existe déjà.");
            }

            // Validate password uniqueness
            if (await PasswordExistsAsync(employe.MotDePasse))
            {
                throw new ArgumentException($"Le mot de passe {employe.MotDePasse} est déjà utilisé.");
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(employe.Nom))
            {
                throw new ArgumentException("Le nom est obligatoire.");
            }

            if (string.IsNullOrWhiteSpace(employe.Prenom))
            {
                throw new ArgumentException("Le prénom est obligatoire.");
            }

            _context.Employes.Add(employe);
            await _context.SaveChangesAsync();
            return employe;
        }
        public async Task<Employe> GetEmployeByCinAsync(string cin)
        {
            return await _context.Employes
                .Include(e => e.Avances)
                .Include(e => e.Absences)
                .FirstOrDefaultAsync(e => e.Cin == cin);
        }
        public async Task<List<Employe>> GetAllEmployesAsync()
        {
            return await _context.Employes
                .Include(e => e.Avances)
                .Include(e => e.Absences)
                .OrderBy(e => e.Nom)
                .ThenBy(e => e.Prenom)
                .ToListAsync();
        }

        public async Task<Employe> UpdateEmployeAsync(Employe employe)
        {
            var existingEmploye = await _context.Employes.FindAsync(employe.Cin);
            if (existingEmploye == null)
            {
                throw new ArgumentException($"Employé non trouvé avec le CIN: {employe.Cin}");
            }

            // Check password uniqueness (excluding current employee)
            var existingByPassword = await _context.Employes
                .FirstOrDefaultAsync(e => e.MotDePasse == employe.MotDePasse && e.Cin != employe.Cin);

            if (existingByPassword != null)
            {
                throw new ArgumentException($"Le mot de passe {employe.MotDePasse} est déjà utilisé.");
            }

            existingEmploye.Nom = employe.Nom;
            existingEmploye.Prenom = employe.Prenom;
            existingEmploye.Utilisateur = employe.Utilisateur;
            existingEmploye.MotDePasse = employe.MotDePasse;
            existingEmploye.Salaire = employe.Salaire;

            await _context.SaveChangesAsync();
            return existingEmploye;
        }
        public async Task DeleteEmployeAsync(string cin)
        {
            var employe = await _context.Employes.FindAsync(cin);
            if (employe == null)
            {
                throw new ArgumentException($"Employé non trouvé avec le CIN: {cin}");
            }

            _context.Employes.Remove(employe);
            await _context.SaveChangesAsync();
        }

        public async Task<Employe> AuthenticateAsync(string utilisateur, int motDePasse)
        {
            return await _context.Employes
                .FirstOrDefaultAsync(e => e.Utilisateur == utilisateur && e.MotDePasse == motDePasse);
        }

        public async Task<bool> CinExistsAsync(string cin)
        {
            return await _context.Employes.AnyAsync(e => e.Cin == cin);
        }

        public async Task<bool> PasswordExistsAsync(int motDePasse)
        {
            return await _context.Employes.AnyAsync(e => e.MotDePasse == motDePasse);
        }
    }
}
