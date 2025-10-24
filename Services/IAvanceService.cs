using GestionEmployes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionEmployes.Services
{
    public interface IAvanceService
    {
        Task<Avance> CreateAvanceAsync(Avance avance);
        Task<Avance> GetAvanceByIdAsync(long id);
        Task<List<Avance>> GetAllAvancesAsync();
        Task<List<Avance>> GetAvancesByEmployeAsync(string employeCin);
        Task<List<Avance>> GetAvancesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Avance> UpdateAvanceAsync(Avance avance);
        Task DeleteAvanceAsync(long id);
        Task<decimal> GetTotalAvancesByEmployeAndDateRangeAsync(string employeCin, DateTime startDate, DateTime endDate);
    }
}
