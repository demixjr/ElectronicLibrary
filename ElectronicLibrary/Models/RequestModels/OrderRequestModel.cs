using System.ComponentModel.DataAnnotations;

namespace ElectronicLibrary.Models.RequestModels
{
    public class OrderRequestModel
    {
        [Required(ErrorMessage = "ID користувача є обов’язковим")]
        public int UserId { get; set; }
    }
}
