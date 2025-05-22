using BLL.dto;
using BLL.IServices;
using DAL;
using AutoMapper;
using DAL.Models;
using BLL.Exceptions;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public UserDto AddUser(UserDto dto)
        {
            ValidateUsernameIsUnique(dto);

            var entity = _mapper.Map<User>(dto);

            _unitOfWork.GetRepository<User>().Add(entity);
            _unitOfWork.Save();

            return _mapper.Map<UserDto>(entity);
        }

        public void DeleteUser(int id)
        {
            var entity = GetUserOrThrow(id);

            _unitOfWork.GetRepository<User>().Remove(entity);
            _unitOfWork.Save();
        }

        public UserDto GetUserById(int id)
        {
            var entity = GetUserOrThrow(id);

            return _mapper.Map<UserDto>(entity);
        }

        public IEnumerable<UserDto> GetAllUsers()
        {
            var entities = _unitOfWork.GetRepository<User>().GetAll();
            return _mapper.Map<IEnumerable<UserDto>>(entities);
        }

        public void ChangeUserPassword(int id, string newPassword)
        {
            var entity = GetUserOrThrow(id);

            entity.Password = newPassword;
            _unitOfWork.Save();
        }

        public void ChangeUserAddress(int id, string newAddress)
        {
            var entity = GetUserOrThrow(id);

            entity.Address = newAddress;
            _unitOfWork.Save();
        }

        public IEnumerable<OrderDto> GetAllOrders(int id)
        {
            var entity = GetUserOrThrow(id);

            return _mapper.Map<IEnumerable<OrderDto>>(entity.Orders);
        }

        private void ValidateUsernameIsUnique(UserDto dto)
        {
            var existingEntities = _unitOfWork.GetRepository<User>().GetAll();

            bool usernameExists = existingEntities.Any(user => user.Username == dto.Username);

            if (usernameExists)
            {
                throw new ValidationException($"Користувач з ім'ям {dto.Username} вже існує.");
            }
        }

        private User GetUserOrThrow(int id)
        {
            var user = _unitOfWork.GetRepository<User>().Get(id);
            if (user == null)
            {
                throw new NotFoundException($"Користувача з ID {id} не знайдено.");
            }
            return user;
        }
    }
}
