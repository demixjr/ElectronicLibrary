using BLL.dto;
using BLL.IServices;
using DAL;
using AutoMapper;
using DAL.Models;
using BLL.Exceptions;
using DAL.Enums;
using System.Security.Authentication;

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

        public UserDto RegisterUser(UserDto dto)
        {
            ValidateUsernameIsUnique(dto);

            var entity = _mapper.Map<User>(dto);
            entity.Role = Roles.Registered;

            _unitOfWork.GetRepository<User>().Add(entity);
            _unitOfWork.Save();

            return _mapper.Map<UserDto>(entity);
        }

        public UserDto CreatePrivilegedUser(UserDto dto)
        {
            if (dto.Role != Enums.Roles.Admin && dto.Role != Enums.Roles.Manager)
            {
                throw new ValidationException("Недопустима роль для цієї команди.");
            }

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

        public UserDto Authenticate(string username, string password)
        {
            var user = _unitOfWork.GetRepository<User>().Find(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                throw new AuthenticationException($"Помилка аутентифікації: Користувача з ім'ям {username} не існує.");
            }

            if (user.Password != password)
            {
                throw new AuthenticationException("Помилка аутентифікації: Неправильний пароль.");
            }

            return _mapper.Map<UserDto>(user);
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
