using BLL.dto;

namespace BLL.IServices
{
    public interface IUserService
    {
        UserDto AddUser(UserDto dto);
        IEnumerable<UserDto> GetAllUsers();
        UserDto GetUserById(int id);
        void ChangeUserPassword(int id, string newPassword);
        void ChangeUserAddress(int id, string newAddress);
        void DeleteUser(int id);
        IEnumerable<OrderDto> GetAllOrders(int id);
    }
}
