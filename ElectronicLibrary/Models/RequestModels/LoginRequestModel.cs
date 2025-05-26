using System.ComponentModel.DataAnnotations;

namespace ElectronicLibrary.Models.RequestModels
{
    public class LoginRequestModel
    {
        [Required(ErrorMessage = "Ім’я користувача є обов’язковим")]
        [MinLength(3, ErrorMessage = "Ім’я користувача має містити щонайменше 3 символи")]
        [MaxLength(32, ErrorMessage = "Ім’я користувача не може перевищувати 32 символи")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Пароль є обов’язковим")]
        [MinLength(6, ErrorMessage = "Пароль має містити щонайменше 6 символів")]
        [MaxLength(64, ErrorMessage = "Пароль не може перевищувати 64 символи")]
        public string? Password { get; set; }
    }
}