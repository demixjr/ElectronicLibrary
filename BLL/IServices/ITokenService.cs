using BLL.dto;

namespace BLL.IServices
{
    public interface ITokenService
    {
        string GenerateToken(UserDto user);
    }
}
