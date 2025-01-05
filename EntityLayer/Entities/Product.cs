using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Boş geçilemez")]
        [Display(Name = "Ad")]
        public string Name { get; set; }
        [Required (ErrorMessage = "Boş geçilemez")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Boş geçilemez")]
        [Display(Name = "Resim")]
        public string? Image { get; set; }

        [Required(ErrorMessage = "Boş geçilemez")]
        [Display(Name = "Fiyat")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Boş geçilemez")]
        [Display(Name = "Stok")]
        public int? Stock { get; set; }

        [Required(ErrorMessage = "Boş geçilemez")]
        [Display(Name = "Onay")]
        public bool? IsApproved { get; set; }

        [Required(ErrorMessage = "Boş geçilemez")]

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public  List<Sales>? Sales { get; set; }

        public  List<Cart> Carts { get; set; }

    }
}
