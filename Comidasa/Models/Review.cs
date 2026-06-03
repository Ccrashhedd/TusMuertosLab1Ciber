using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Comidasa.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public IdentityUser? User { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [StringLength(50)]
        public string Documento { get; set; } = string.Empty;

        [StringLength(255)]
        public string? DocumentPath { get; set; }

        [Required(ErrorMessage = "El comentario es obligatorio")]
        [StringLength(1000, ErrorMessage = "El comentario no puede superar los 1000 caracteres")]
        public string Comment { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "La calificación debe ser entre 1 y 5 estrellas")]
        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
