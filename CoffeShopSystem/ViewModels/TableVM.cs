using System.ComponentModel.DataAnnotations;

namespace CoffeShopSystem.ViewModels
{
    public class TableVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نمبر میز لازمی است")]
        [Range(1, 100, ErrorMessage = "نمبر میز باید بین 1 تا 100 باشد")]
        public int TableNumber { get; set; }

        [Required(ErrorMessage = "نمبر منزل لازمی است")]
        [Range(0, 100, ErrorMessage = "نمبر منزل باید بین 1 تا 100 باشد")]
        public int FloorNumber { get; set; } = 0;
    }
}
