using GestionEmployes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionEmployes.Services
{
    public interface IAbsenceService
    {
        Task<Absence> CreateAbsenceAsync(Absence absence);
        Task<Absence> GetAbsenceByIdAsync(long id);
        Task<List<Absence>> GetAllAbsencesAsync();
        Task<List<Absence>> GetAbsencesByEmployeAsync(string employeCin);
        Task<List<Absence>> GetAbsencesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Absence> UpdateAbsenceAsync(Absence absence);
        Task DeleteAbsenceAsync(long id);
        Task<decimal> GetTotalPenalitesByEmployeAndDateRangeAsync(string employeCin, DateTime startDate, DateTime endDate);
        Task<int> CountAbsencesByEmployeAndDateRangeAsync(string employeCin, DateTime startDate, DateTime endDate);
    }
}
