using GestionEmployes.Data;
using GestionEmployes.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace GestionEmployes.Services
{
    public class ReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmployeService _employeService;
        private readonly IAvanceService _avanceService;
        private readonly IAbsenceService _absenceService;

        public ReportService(ApplicationDbContext context, IEmployeService employeService,
                           IAvanceService avanceService, IAbsenceService absenceService)
        {
            _context = context;
            _employeService = employeService;
            _avanceService = avanceService;
            _absenceService = absenceService;
        }

        public DateTime[] GetCurrentWeekPeriod()
        {
            var today = DateTime.Today;
            var dayOfWeek = (int)today.DayOfWeek;
            var daysToSubtract = (dayOfWeek + 1) % 7; // Days since last Friday

            var weekStart = today.AddDays(-daysToSubtract).Date;
            var weekEnd = weekStart.AddDays(6).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            return new DateTime[] { weekStart, weekEnd };
        }

        public DateTime[] GetWeekPeriodForDate(DateTime date)
        {
            var dayOfWeek = (int)date.DayOfWeek;
            var daysToSubtract = (dayOfWeek + 1) % 7;

            var weekStart = date.AddDays(-daysToSubtract).Date;
            var weekEnd = weekStart.AddDays(6).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            return new DateTime[] { weekStart, weekEnd };
        }
        public async Task<List<WeeklyReport>> GenerateWeeklyReportAsync(DateTime weekStart, DateTime weekEnd)
        {
            var employees = await _employeService.GetAllEmployesAsync();
            var reports = new List<WeeklyReport>();

            foreach (var emp in employees)
            {
                var report = await GenerateEmployeeWeeklyReportAsync(emp, weekStart, weekEnd);
                reports.Add(report);
            }

            return reports;
        }
        public async Task<List<WeeklyReport>> GenerateCurrentWeekReportAsync()
        {
            var period = GetCurrentWeekPeriod();
            return await GenerateWeeklyReportAsync(period[0], period[1]);
        }
        public async Task<WeeklyReport> GenerateEmployeeWeeklyReportAsync(Employe employe, DateTime weekStart, DateTime weekEnd)
        {
            var totalAvances = await _avanceService.GetTotalAvancesByEmployeAndDateRangeAsync(employe.Cin, weekStart, weekEnd);
            var totalPenalites = await _absenceService.GetTotalPenalitesByEmployeAndDateRangeAsync(employe.Cin, weekStart, weekEnd);
            var nombreAbsences = await _absenceService.CountAbsencesByEmployeAndDateRangeAsync(employe.Cin, weekStart, weekEnd);

            // Get last dates
            var avances = await _avanceService.GetAvancesByDateRangeAsync(weekStart, weekEnd);
            var absences = await _absenceService.GetAbsencesByDateRangeAsync(weekStart, weekEnd);

            var lastAvanceDate = avances.Where(a => a.EmployeCin == employe.Cin).FirstOrDefault()?.DateAvance;
            var lastAbsenceDate = absences.Where(a => a.EmployeCin == employe.Cin).FirstOrDefault()?.DateAbsence;

            var report = new WeeklyReport(
                employe.Cin,
                employe.Nom,
                employe.Prenom,
                employe.Salaire,
                weekStart,
                weekEnd,
                totalAvances,
                totalPenalites,
                nombreAbsences
            );

            report.LastAvanceDate = lastAvanceDate;
            report.LastAbsenceDate = lastAbsenceDate;

            return report;
        }
        public async Task<bool> ExportToExcelAsync(List<WeeklyReport> reportList, string filePath)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Rapport Hebdomadaire");

                    // Create header row
                    var headers = new string[] { "CIN", "Nom", "Prénom", "Salaire", "Avances", "Date Avance",
                                               "Absences", "Date Absence", "Pénalités", "Salaire Net",
                                               "Date de début", "Date de fin" };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = worksheet.Cell(1, i + 1);
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                    }

                    // Add data rows
                    int row = 2;
                    foreach (var report in reportList)
                    {
                        worksheet.Cell(row, 1).Value = report.Cin;
                        worksheet.Cell(row, 2).Value = report.Nom;
                        worksheet.Cell(row, 3).Value = report.Prenom;
                        worksheet.Cell(row, 4).Value = report.Salaire ?? 0;
                        worksheet.Cell(row, 5).Value = report.TotalAvances ?? 0;
                        worksheet.Cell(row, 6).Value = report.LastAvanceDate;
                        worksheet.Cell(row, 7).Value = report.NombreAbsences;
                        worksheet.Cell(row, 8).Value = report.LastAbsenceDate;
                        worksheet.Cell(row, 9).Value = report.TotalPenalites ?? 0;
                        worksheet.Cell(row, 10).Value = report.SalaireNet ?? 0;
                        worksheet.Cell(row, 11).Value = report.WeekStart;
                        worksheet.Cell(row, 12).Value = report.WeekEnd;

                        // Format currency cells
                        worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00 DH";
                        worksheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00 DH";
                        worksheet.Cell(row, 9).Style.NumberFormat.Format = "#,##0.00 DH";
                        worksheet.Cell(row, 10).Style.NumberFormat.Format = "#,##0.00 DH";

                        // Format date cells
                        worksheet.Cell(row, 6).Style.DateFormat.Format = "dd/mm/yyyy";
                        worksheet.Cell(row, 8).Style.DateFormat.Format = "dd/mm/yyyy";
                        worksheet.Cell(row, 11).Style.DateFormat.Format = "dd/mm/yyyy";
                        worksheet.Cell(row, 12).Style.DateFormat.Format = "dd/mm/yyyy";

                        row++;
                    }

                    // Auto-fit columns
                    worksheet.ColumnsUsed().AdjustToContents();

                    workbook.SaveAs(filePath);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de l'exportation Excel: {ex.Message}");
                return false;
            }
        }
    }
}
