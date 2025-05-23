using BLL.Enums;

namespace ElectronicLibrary.Models.RequestModels
{
    public class PrivilegedUserRequestModel
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Roles Role {  get; set; }
    }
}