using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionEmployes.Models
{
    [Table("Employe")]
    public class Employe
    {
        [Key]
        [StringLength(20)]
        public string Cin { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        [Required]
        [StringLength(100)]
        public string Prenom { get; set; }

        [Required]
        [StringLength(50)]
        public string Utilisateur { get; set; }


        [Column(TypeName = "decimal")]
        public decimal? Salaire { get; set; }

        // Navigation Properties
        public virtual ICollection<Avance> Avances { get; set; } = new List<Avance>();
        public virtual ICollection<Absence> Absences { get; set; } = new List<Absence>();

        public Employe()
        {
            Avances = new List<Avance>();
            Absences = new List<Absence>();
        }

        public Employe(string cin, string nom, string prenom, string utilisateur,  decimal? salaire)
        {
            Cin = cin;
            Nom = nom;
            Prenom = prenom;
            Utilisateur = utilisateur;
            Salaire = salaire;
            Avances = new List<Avance>();
            Absences = new List<Absence>();
        }

        public override string ToString()
        {
            return $"{Nom} {Prenom} ({Cin})";
        }
    }
}
