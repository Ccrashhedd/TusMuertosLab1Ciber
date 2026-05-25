using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comidasa.Models
{
    public class Product
    {
        [Key]
        public int IdProduct { get; set; }

        [Required(ErrorMessage = "La imagen es obligatoria")]
        [StringLength(255)]
        public string ImagenProduct { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        public string NameProduct { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Descrip { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(20,4)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;
    }
}
