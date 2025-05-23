
namespace ElectronicLibrary.Models.ResponseModels
{
    public class OrdedResponseModel
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public int UserId { get; set; }
        public UserResponseModel? User { get; set; }
        public ICollection<BookResponseModel>? Books { get; set; }
    }
}