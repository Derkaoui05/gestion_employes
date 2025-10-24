using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionEmployes.Models
{
    [Table("Absence")]
    public class Absence
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Penalite { get; set; }

        [Required]
        public DateTime DateAbsence { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("Employe")]
        public string EmployeCin { get; set; }

        // Navigation Property
        public virtual Employe Employe { get; set; }

        public Absence() { }

        public Absence(decimal penalite, DateTime dateAbsence, Employe employe)
        {
            Penalite = penalite;
            DateAbsence = dateAbsence;
            Employe = employe;
            EmployeCin = employe?.Cin;
        }
    }
}
