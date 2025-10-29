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
    public class EmployeService:IEmployeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeService(ApplicationDbContext context)
        {
            _context = context;
        }
        // 🔹 Ajouter un employé
        public bool AjouterEmploye(Employe employe)
        {
            try
            {
                _context.Employes.Add(employe);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout de l'employé : {ex.Message}");
                return false;
            }
        }
        public List<Employe> GetAllEmployes()
        {
            try
            {
                return _context.Employes.Include(e => e.Avances).Include(e => e.Absences).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des employés : {ex.Message}");
                return new List<Employe>();
            }
        }
        // 🔹 Rechercher un employé par CIN
        public Employe GetEmployeByCin(string cin)
        {
            return _context.Employes
                .Include(e => e.Avances)
                .Include(e => e.Absences)
                .FirstOrDefault(e => e.Cin == cin);
        }

        // 🔹 Mettre à jour un employé
        public bool ModifierEmploye(Employe employe)
        {
            try
            {
                var existing = _context.Employes.Find(employe.Cin);
                if (existing == null) return false;

                _context.Entry(existing).CurrentValues.SetValues(employe);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la modification : {ex.Message}");
                return false;
            }
        }
        // 🔹 Supprimer un employé
        public bool SupprimerEmploye(string cin)
        {
            try
            {
                var employe = _context.Employes.Find(cin);
                if (employe == null) return false;

                _context.Employes.Remove(employe);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la suppression : {ex.Message}");
                return false;
            }
        }


        public async Task<Employe> CreateEmployeAsync(Employe employe)
        {

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
            //var existingByPassword = await _context.Employes
            //    .FirstOrDefaultAsync(e => e.MotDePasse == employe.MotDePasse && e.Cin != employe.Cin);

            //if (existingByPassword != null)
            //{
            //    throw new ArgumentException($"Le mot de passe {employe.MotDePasse} est déjà utilisé.");
            //}

            existingEmploye.Nom = employe.Nom;
            existingEmploye.Prenom = employe.Prenom;
            existingEmploye.Utilisateur = employe.Utilisateur;
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

        //public async Task<Employe> AuthenticateAsync(string utilisateur, int motDePasse)
        //{
        //    return await _context.Employes
        //        .FirstOrDefaultAsync(e => e.Utilisateur == utilisateur && e.MotDePasse == motDePasse);
        //}

        //public async Task<bool> CinExistsAsync(string cin)
        //{
        //    return await _context.Employes.AnyAsync(e => e.Cin == cin);
        //}

    }
}
