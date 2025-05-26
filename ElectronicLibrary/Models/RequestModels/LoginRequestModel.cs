using System.ComponentModel.DataAnnotations;

namespace ElectronicLibrary.Models.RequestModels
{
    public class LoginRequestModel
    {
        [Required(ErrorMessage = "Email є обов’язковим")]
        [EmailAddress(ErrorMessage = "Некоректний формат email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Пароль є обов’язковим")]
        [MinLength(6, ErrorMessage = "Пароль має містити щонайменше 6 символів")]
        [MaxLength(64, ErrorMessage = "Пароль не може перевищувати 64 символи")]
        public string? Password { get; set; }
    }
}