using System.ComponentModel.DataAnnotations;

namespace ElectronicLibrary.Models.RequestModels
{
    public class LoginRequestModel
    {
        const int minLength = 3;
        const int nameLength = 32;
        const int minPass = 6;
        const int passLength = 64;

        [Required(ErrorMessage = "Ім’я користувача є обов’язковим")]
        [MinLength(minLength, ErrorMessage = "Ім’я користувача має містити щонайменше 3 символи")]
        [MaxLength(nameLength, ErrorMessage = "Ім’я користувача не може перевищувати 32 символи")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Пароль є обов’язковим")]
        [MinLength(minPass, ErrorMessage = "Пароль має містити щонайменше 6 символів")]
        [MaxLength(passLength, ErrorMessage = "Пароль не може перевищувати 64 символи")]
        public string? Password { get; set; }
    }
}