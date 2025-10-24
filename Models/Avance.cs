using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionEmployes.Models
{
    [Table("Avance")]
    public class Avance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Montant { get; set; }

        [Required]
        public DateTime DateAvance { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey("Employe")]
        public string EmployeCin { get; set; }

        // Navigation Property
        public virtual Employe Employe { get; set; }

        public Avance() { }

        public Avance(decimal montant, DateTime dateAvance, Employe employe)
        {
            Montant = montant;
            DateAvance = dateAvance;
            Employe = employe;
            EmployeCin = employe?.Cin;
        }
    }
}
