using BLL.dto;

namespace BLL.IServices
{
    public interface IUserService
    {
        UserDto RegisterUser(UserDto dto);
        UserDto CreatePrivilegedUser(UserDto dto);
        IEnumerable<UserDto> GetAllUsers();
        UserDto GetUserById(int id);
        void ChangeUserPassword(int id, string newPassword);
        void DeleteUser(int id);
        UserDto Authenticate(string username, string password);
    }
}