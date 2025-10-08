using System.ComponentModel.DataAnnotations;

namespace CoffeShopSystem.ViewModels
{
    public class MenuVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام محصول لازمی است")]
        [MinLength(2, ErrorMessage = "نام محصول باید بیشتر از 2 حرف باشد")]
        [MaxLength(100, ErrorMessage = "نام محصول نباید بیشتر از 100 حرف باشد")]
        public string Item { get; set; } = string.Empty;

        [Required(ErrorMessage = "قیمت محصول لازمی است")]
        public decimal Price { get; set; } = 0;
    }
}
