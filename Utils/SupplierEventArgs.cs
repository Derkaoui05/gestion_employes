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
        public static event EventHandler<AvanceEventArgs> AvanceDeleted;

        // Événements pour les absences
        public static event EventHandler<AbsenceEventArgs> AbsenceAdded;
        public static event EventHandler<AbsenceEventArgs> AbsenceDeleted;

        // Événements pour les fournisseurs (utilisent votre classe existante)
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
            => EmployeAdded?.Invoke(source, new EmployeEventArgs(cin, nom, prenom));

        public static void OnEmployeUpdated(object source, string cin, string nom, string prenom)
            => EmployeUpdated?.Invoke(source, new EmployeEventArgs(cin, nom, prenom));

        public static void OnEmployeDeleted(object source, string cin, string nom, string prenom)
            => EmployeDeleted?.Invoke(source, new EmployeEventArgs(cin, nom, prenom));

        // Méthodes pour déclencher les événements - AVANCES
        public static void OnAvanceAdded(object source, long id, string cin, decimal montant)
            => AvanceAdded?.Invoke(source, new AvanceEventArgs(id, cin, montant));

        public static void OnAvanceDeleted(object source, long id, string cin, decimal montant)
            => AvanceDeleted?.Invoke(source, new AvanceEventArgs(id, cin, montant));

        // Méthodes pour déclencher les événements - ABSENCES
        public static void OnAbsenceAdded(object source, long id, string cin, decimal penalite)
            => AbsenceAdded?.Invoke(source, new AbsenceEventArgs(id, cin, penalite));

        public static void OnAbsenceDeleted(object source, long id, string cin, decimal penalite)
            => AbsenceDeleted?.Invoke(source, new AbsenceEventArgs(id, cin, penalite));

        // Méthodes pour déclencher les événements - FOURNISSEURS (votre code existant)
        public static void OnSupplierAdded(object source, int supplierId, string supplierName)
            => SupplierAdded?.Invoke(source, new SupplierEventArgs(supplierId, supplierName));

        public static void OnSupplierUpdated(object source, int supplierId, string supplierName)
            => SupplierUpdated?.Invoke(source, new SupplierEventArgs(supplierId, supplierName));

        public static void OnSupplierDeleted(object source, int supplierId, string supplierName)
            => SupplierDeleted?.Invoke(source, new SupplierEventArgs(supplierId, supplierName));

        // Méthodes pour déclencher les événements - FACTURES
        public static void OnFactureAdded(object source, int id, string number, int supplierId, decimal amount)
            => FactureAdded?.Invoke(source, new FactureEventArgs(id, number, supplierId, amount));

        public static void OnFactureUpdated(object source, int id, string number, int supplierId, decimal amount)
            => FactureUpdated?.Invoke(source, new FactureEventArgs(id, number, supplierId, amount));

        public static void OnFactureDeleted(object source, int id, string number, int supplierId, decimal amount)
            => FactureDeleted?.Invoke(source, new FactureEventArgs(id, number, supplierId, amount));

        // Méthode générique pour tout changement de données
        public static void OnDataChanged(object source, string message = "Données mises à jour")
            => DataChanged?.Invoke(source, new GenericEventArgs(message));
    }
}