using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionEmployes.Models
{
    public class WeeklyReport
    {
        public string Cin { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public decimal? Salaire { get; set; }
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
        public decimal? TotalAvances { get; set; }
        public decimal? TotalPenalites { get; set; }
        public int NombreAbsences { get; set; }
        public DateTime? LastAvanceDate { get; set; }
        public DateTime? LastAbsenceDate { get; set; }
        public decimal? SalaireNet { get; private set; }

        public WeeklyReport() { }

        public WeeklyReport(string cin, string nom, string prenom, decimal? salaire,
                           DateTime weekStart, DateTime weekEnd, decimal? totalAvances,
                           decimal? totalPenalites, int nombreAbsences)
        {
            Cin = cin;
            Nom = nom;
            Prenom = prenom;
            Salaire = salaire;
            WeekStart = weekStart;
            WeekEnd = weekEnd;
            TotalAvances = totalAvances;
            TotalPenalites = totalPenalites;
            NombreAbsences = nombreAbsences;
            CalculateSalaireNet();
        }

        private void CalculateSalaireNet()
        {
            decimal baseSalaire = Salaire ?? 0;
            decimal avances = TotalAvances ?? 0;
            decimal penalites = TotalPenalites ?? 0;
            SalaireNet = baseSalaire - avances - penalites;
        }

        public void SetSalaire(decimal? salaire)
        {
            Salaire = salaire;
            CalculateSalaireNet();
        }

        public void SetTotalAvances(decimal? totalAvances)
        {
            TotalAvances = totalAvances;
            CalculateSalaireNet();
        }

        public void SetTotalPenalites(decimal? totalPenalites)
        {
            TotalPenalites = totalPenalites;
            CalculateSalaireNet();
        }
    }
}
