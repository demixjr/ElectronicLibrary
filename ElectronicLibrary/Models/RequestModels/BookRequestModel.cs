using DAL.Enums;

namespace ElectronicLibrary.Models.RequestModels
{
    public class BookRequestModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public decimal Price { get; set; }
        public BookTypes BookType { get; set; }
        public Genres Genre { get; set; }
    }
}
