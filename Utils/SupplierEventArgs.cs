using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionEmployes.Utils
{
    // Classes d'arguments pour les événements
    public class SupplierEventArgs : EventArgs
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }

        public SupplierEventArgs(int id, string name)
        {
            SupplierId = id;
            SupplierName = name;
        }
    }

    public class EmployeEventArgs : EventArgs
    {
        public string EmployeCin { get; set; }
        public string EmployeNom { get; set; }
        public string EmployePrenom { get; set; }

        public EmployeEventArgs(string cin, string nom, string prenom)
        {
            EmployeCin = cin;
            EmployeNom = nom;
            EmployePrenom = prenom;
        }
    }

    public class AvanceEventArgs : EventArgs
    {
        public long AvanceId { get; set; }
        public string EmployeCin { get; set; }
        public decimal Montant { get; set; }

        public AvanceEventArgs(long id, string cin, decimal montant)
        {
            AvanceId = id;
            EmployeCin = cin;
            Montant = montant;
        }
    }

    public class AbsenceEventArgs : EventArgs
    {
        public long AbsenceId { get; set; }
        public string EmployeCin { get; set; }
        public decimal Penalite { get; set; }

        public AbsenceEventArgs(long id, string cin, decimal penalite)
        {
            AbsenceId = id;
            EmployeCin = cin;
            Penalite = penalite;
        }
    }

    public class FactureEventArgs : EventArgs
    {
        public int FactureId { get; set; }
        public string FactureNumber { get; set; }
        public int SupplierId { get; set; }
        public decimal Amount { get; set; }

        public FactureEventArgs(int id, string number, int supplierId, decimal amount)
        {
            FactureId = id;
            FactureNumber = number;
            SupplierId = supplierId;
            Amount = amount;
        }
    }

    public class GenericEventArgs : EventArgs
    {
        public string Message { get; set; }

        public GenericEventArgs(string message)
        {
            Message = message;
        }
    }

    public static class EventBus
    {
        // Événements pour les employés
        public static event EventHandler<EmployeEventArgs> EmployeAdded;
        public static event EventHandler<EmployeEventArgs> EmployeUpdated;
        public static event EventHandler<EmployeEventArgs> EmployeDeleted;

        // Événements pour les avances
        public static event EventHandler<AvanceEventArgs> AvanceAdded;
        public static event EventHandler<AvanceEventArgs> AvanceUpdated;
        public static event EventHandler<AvanceEventArgs> AvanceDeleted;

        // Événements pour les absences
        public static event EventHandler<AbsenceEventArgs> AbsenceAdded;
        public static event EventHandler<AbsenceEventArgs> AbsenceUpdated;
        public static event EventHandler<AbsenceEventArgs> AbsenceDeleted;

        // Événements pour les fournisseurs
        public static event EventHandler<SupplierEventArgs> SupplierAdded;
        public static event EventHandler<SupplierEventArgs> SupplierUpdated;
        public static event EventHandler<SupplierEventArgs> SupplierDeleted;

        // Événements pour les factures
        public static event EventHandler<FactureEventArgs> FactureAdded;
        public static event EventHandler<FactureEventArgs> FactureUpdated;
        public static event EventHandler<FactureEventArgs> FactureDeleted;

        // Événement générique pour tout rafraîchissement
        public static event EventHandler<GenericEventArgs> DataChanged;

        // Méthodes pour déclencher les événements - EMPLOYÉS
        public static void OnEmployeAdded(object source, string cin, string nom, string prenom)
        {
            try
            {
                EmployeAdded?.Invoke(source, new EmployeEventArgs(cin, nom, prenom));
                OnDataChanged(source, "Employé ajouté");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans OnEmployeAdded: {ex.Message}");
            }
        }

        public static void OnEmployeUpdated(object source, string cin, string nom, string prenom)
        {
            try
            {
                EmployeUpdated?.Invoke(source, new EmployeEventArgs(cin, nom, prenom));
                OnDataChanged(source, "Employé modifié");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans OnEmployeUpdated: {ex.Message}");
            }
        }

        public static void OnEmployeDeleted(object source, string cin, string nom, string prenom)
        {
            try
            {
                EmployeDeleted?.Invoke(source, new EmployeEventArgs(cin, nom, prenom));
                OnDataChanged(source, "Employé supprimé");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans OnEmployeDeleted: {ex.Message}");
            }
        }

        // Méthodes pour déclencher les événements - AVANCES
        public static void OnAvanceAdded(object source, long id, string cin, decimal montant)
        {
            try
            {
                AvanceAdded?.Invoke(source, new AvanceEventArgs(id, cin, montant));
                OnDataChanged(source, "Avance ajoutée");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans OnAvanceAdded: {ex.Message}");
            }
        }

        public static void OnAvanceUpdated(object source, long id, string cin, decimal montant)
        {
            try
            {
                AvanceUpdated?.Invoke(source, new AvanceEventArgs(id, cin, montant));
                OnDataChanged(source, "Avance modifiée");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans OnAvanceUpdated: {ex.Message}");
            }
        }

        public static void OnAvanceDeleted(object source, long id, string cin, decimal montant)
        {
            try
            {
                AvanceDeleted?.Invoke(source, new AvanceEventArgs(id, cin, montant));
                OnDataChanged(source, "Avance supprimée");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans OnAvanceDeleted: {ex.Message}");
            }
        }

        // Méthodes pour déclencher les événements - ABSENCES
        public static void OnAbsenceAdded(object source, long id, string cin, decimal penalite)
        {
            try
            {
                AbsenceAdded?.Invoke(source, new AbsenceEventArgs(id, cin, penalite));
                OnDataChanged(source, "Absence ajoutée");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans OnAbsenceAdded: {ex.Message}");
            }
        }

        public static void OnAbsenceUpdated(object source, long id, string cin, decimal penalite)
        {
            try
            {
                AbsenceUpdated?.Invoke(source, new AbsenceEventArgs(id, cin, penalite));
                OnDataChanged(source, "Absence modifiée");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans OnAbsenceUpdated: {ex.Message}");
            }
        }

        public static void OnAbsenceDeleted(object source, long id, string cin, decimal penalite)
        {
            try
            {
                AbsenceDeleted?.Invoke(source, new AbsenceEventArgs(id, cin, penalite));
                OnDataChanged(source, "Absence supprimée");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans OnAbsenceDeleted: {ex.Message}");
            }
        }

        // Méthodes pour déclencher les événements - FOURNISSEURS
        public static void OnSupplierAdded(object source, int supplierId, string supplierName)
        {
            try
            {
                SupplierAdded?.Invoke(source, new SupplierEventArgs(supplierId, supplierName));
                OnDataChanged(source, "Fournisseur ajouté");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans OnSupplierAdded: {ex.Message}");
            }
        }

        public static void OnSupplierUpdated(object source, int supplierId, string supplierName)
        {
            try
            {
                SupplierUpdated?.Invoke(source, new SupplierEventArgs(supplierId, supplierName));
                OnDataChanged(source, "Fournisseur modifié");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans OnSupplierUpdated: {ex.Message}");
            }
        }

        public static void OnSupplierDeleted(object source, int supplierId, string supplierName)
        {
            try
            {
                SupplierDeleted?.Invoke(source, new SupplierEventArgs(supplierId, supplierName));
                OnDataChanged(source, "Fournisseur supprimé");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans OnSupplierDeleted: {ex.Message}");
            }
        }

        // Méthodes pour déclencher les événements - FACTURES
        public static void OnFactureAdded(object source, int id, string number, int supplierId, decimal amount)
        {
            try
            {
                FactureAdded?.Invoke(source, new FactureEventArgs(id, number, supplierId, amount));
                OnDataChanged(source, "Facture ajoutée");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans OnFactureAdded: {ex.Message}");
            }
        }

        public static void OnFactureUpdated(object source, int id, string number, int supplierId, decimal amount)
        {
            try
            {
                FactureUpdated?.Invoke(source, new FactureEventArgs(id, number, supplierId, amount));
                OnDataChanged(source, "Facture modifiée");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans OnFactureUpdated: {ex.Message}");
            }
        }

        public static void OnFactureDeleted(object source, int id, string number, int supplierId, decimal amount)
        {
            try
            {
                FactureDeleted?.Invoke(source, new FactureEventArgs(id, number, supplierId, amount));
                OnDataChanged(source, "Facture supprimée");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans OnFactureDeleted: {ex.Message}");
            }
        }

        // Méthode générique pour tout changement de données
        public static void OnDataChanged(object source, string message = "Données mises à jour")
        {
            try
            {
                DataChanged?.Invoke(source, new GenericEventArgs(message));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans OnDataChanged: {ex.Message}");
            }
        }

        // Méthode utilitaire pour nettoyer tous les abonnements (optionnel)
        public static void ClearAllSubscriptions()
        {
            try
            {
                EmployeAdded = null;
                EmployeUpdated = null;
                EmployeDeleted = null;

                AvanceAdded = null;
                AvanceUpdated = null;
                AvanceDeleted = null;

                AbsenceAdded = null;
                AbsenceUpdated = null;
                AbsenceDeleted = null;

                SupplierAdded = null;
                SupplierUpdated = null;
                SupplierDeleted = null;

                FactureAdded = null;
                FactureUpdated = null;
                FactureDeleted = null;

                DataChanged = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans ClearAllSubscriptions: {ex.Message}");
            }
        }
    }
}