using GestionEmployes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionEmployes.Services
{
    public interface IEmployeService
    {
        Task<Employe> CreateEmployeAsync(Employe employe);
        Task<Employe> GetEmployeByCinAsync(string cin);
        Task<List<Employe>> GetAllEmployesAsync();
        Task<Employe> UpdateEmployeAsync(Employe employe);
        Task DeleteEmployeAsync(string cin);
        //Task<Employe> AuthenticateAsync(string utilisateur, int motDePasse);
        //Task<bool> CinExistsAsync(string cin);
        //Task<bool> PasswordExistsAsync(int motDePasse);
    }
}
