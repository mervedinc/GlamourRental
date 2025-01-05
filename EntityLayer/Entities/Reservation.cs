using System;
using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Entities
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        // ProductId alanı zorunlu olduğunda, Products alanı ile birlikte kullanılabilir
        [Required(ErrorMessage = "The Products field is required.")]
        public int ProductId { get; set; }

        // Bu virtual anahtar kelimesi, lazy loading için kullanılır. İstendiğinde ilişkili nesne yüklenir.
        public virtual Product Products { get; set; }

        [Required]
        public int UserId { get; set; }

        public virtual User? User { get; set; }
    }

}
