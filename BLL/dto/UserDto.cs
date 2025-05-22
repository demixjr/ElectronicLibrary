
namespace BLL.dto
{
    public class UserDto
    {
        public UserDto() { }
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Address { get; set; }
        public virtual ICollection<OrderDto>? Orders { get; set; }
    }
}
