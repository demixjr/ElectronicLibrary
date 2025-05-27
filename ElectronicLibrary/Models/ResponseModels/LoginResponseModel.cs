using DAL.Enums;

namespace ElectronicLibrary.Models.ResponseModels
{
    public class LoginResponseModel
    {
        public string? Token { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public Roles Role { get; set; }
    }
}