using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // List<T> kullanmak için
using EntityLayer.Entities; // Bağlı olduğunuz sınıfın namespace'i

namespace EntityLayer.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string? Description { get; set; }

        public virtual List<Product>? Products { get; set; }
    }
}
