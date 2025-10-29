using GestionEmployes.Data;
using GestionEmployes.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestionEmployes.Services
{
    public class DashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Récupère le total de toutes les factures
        public decimal GetTotalFactures()
        {
            return _context.Factures.Sum(f => f.Amount);
        }

        // ✅ Récupère le total des avances (montants payés)
        public decimal GetTotalAvances()
        {
            return _context.Factures.Sum(f => f.Advance);
        }

        // ✅ Récupère le total restant à payer
        public decimal GetTotalRestant()
        {
            return _context.Factures.Sum(f => f.Amount - f.Advance);
        }

        // ✅ Récupère le nombre total de fournisseurs actifs
        public int GetTotalFournisseurs()
        {
            return _context.Suppliers.Count(s => s.IsActive);
        }

        // ✅ Récupère le nombre total d’employés
        public int GetTotalEmployes()
        {
            return _context.Employes.Count();
        }

        // ✅ Récupère le total des transactions effectuées
        public decimal GetTotalTransactions()
        {
            return _context.Transactions.Sum(t => t.Amount);
        }

        // ✅ Détails du dashboard global
        public DashboardSummary GetGlobalSummary()
        {
            return new DashboardSummary
            {
                TotalFactures = GetTotalFactures(),
                TotalAvances = GetTotalAvances(),
                TotalRestant = GetTotalRestant(),
                TotalTransactions = GetTotalTransactions(),
                TotalFournisseurs = GetTotalFournisseurs(),
                TotalEmployes = GetTotalEmployes()
            };
        }

        // ✅ Détails du dashboard pour un employé spécifique
        public DashboardSummary GetSummaryByEmployee(string employeeCin)
        {
            var transactions = _context.Transactions
                .Where(t => t.EmployeeCin == employeeCin)
                .ToList();

            return new DashboardSummary
            {
                TotalTransactions = transactions.Sum(t => t.Amount),
                TotalFactures = _context.Factures
                    .Where(f => f.Transactions.Any(t => t.EmployeeCin == employeeCin))
                    .Sum(f => f.Amount),
                TotalAvances = _context.Factures
                    .Where(f => f.Transactions.Any(t => t.EmployeeCin == employeeCin))
                    .Sum(f => f.Advance),
                TotalRestant = _context.Factures
                    .Where(f => f.Transactions.Any(t => t.EmployeeCin == employeeCin))
                    .Sum(f => f.Amount - f.Advance),
                TotalFournisseurs = _context.Suppliers.Count(s => s.IsActive),
                TotalEmployes = 1 // Juste cet employé
            };
        }
    }

    // ✅ Classe pour stocker le résumé
    public class DashboardSummary
    {
        public decimal TotalFactures { get; set; }
        public decimal TotalAvances { get; set; }
        public decimal TotalRestant { get; set; }
        public decimal TotalTransactions { get; set; }
        public int TotalFournisseurs { get; set; }
        public int TotalEmployes { get; set; }
    }
}
