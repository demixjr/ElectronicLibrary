using DAL.Enums;

namespace BLL.dto
{
    public class UserDto
    {
        public UserDto() { }
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Roles Role { get; set; }
        public OrderDto Order { get; set; }
    }
}