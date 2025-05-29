using System.ComponentModel.DataAnnotations;
using DAL.Enums;

namespace ElectronicLibrary.Models.RequestModels
{
    public class BookRequestModel
    {
        const double minRange = 0.01;
        const double maxRange = 99999.99;
        const int nameLength = 32;
        const int descriptionLength = 256;

        [Required(ErrorMessage = "Назва є обов’язковою")]
        [MaxLength(nameLength, ErrorMessage = "Назва не може перевищувати 32 символи")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Опис є обов’язковим")]
        [MaxLength(descriptionLength, ErrorMessage = "Назва не може перевищувати 256 символів")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Автор є обов’язковим")]
        [MaxLength(nameLength, ErrorMessage = "Ім'я автора не може перевищувати 32 символи")]
        public string Author { get; set; }
   
        [Required(ErrorMessage = "Ціна є обов’язковою")]
        [Range(minRange, maxRange, ErrorMessage = "Ціна повинна бути від 0.01 до 99999.99")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Тип книги є обов’язковим")]
        public BookTypes BookType { get; set; }

        [Required(ErrorMessage = "Жанр є обов’язковим")]
        public Genres Genre { get; set; }
    }
}
