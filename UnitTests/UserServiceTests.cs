using AutoFixture;
using AutoMapper;
using BLL.dto;
using BLL.Exceptions;
using BLL.IServices;
using BLL.Enums;
using DAL;
using DAL.Models;
using DAL.Enums;
using ElectronicLibrary;
using NSubstitute;
using System.Security.Authentication;
using System.Linq.Expressions;

namespace UnitTests
{
    public class UserServiceTests
    {
        private readonly DIContainer _container;

        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IMapper _mapperMock;
        private readonly IRepository<Order> _orderRepositoryMock;
        private readonly IRepository<User> _userRepositoryMock;
        private readonly IUserService _userServiceMock;
        private readonly IFixture _fixture;

        public UserServiceTests()
        {
            _container = new DIContainer();

            _unitOfWorkMock = _container.Get<IUnitOfWork>();
            _mapperMock = _container.Get<IMapper>();
            _orderRepositoryMock = _unitOfWorkMock.GetRepository<Order>();
            _userServiceMock = _container.Get<IUserService>();
            _fixture = _container.Get<IFixture>();

        }
        [Fact]
        public void RegisterUser_UsernameNotUnique_ThrowsValidationException()
        {
            // Arrange
            var username = _fixture.Create<string>();

            var dto = new UserDto
            {
                Id = 1,
                Username = username,
                Password = _fixture.Create<string>(),
                Role = BLL.Enums.Roles.Registered
            };

            var existingUser = new User
            {
                Id = 2,
                Username = username,
                Password = _fixture.Create<string>(),
                Role = DAL.Enums.Roles.Registered
            };

            _unitOfWorkMock.GetRepository<User>()
                .GetAll()
                .Returns(new List<User> { existingUser });

            // Act & Assert
            Assert.Throws<ValidationException>(() => _userServiceMock.RegisterUser(dto));
        }


        [Fact]
        public void CreatePrivilegedUser_InvalidRole_ThrowsValidationException()
        {
            // Arrange 
            var dto = new UserDto
            {
                Id = _fixture.Create<int>(),
                Username = _fixture.Create<string>(),
                Password = _fixture.Create<string>(),
                Role = BLL.Enums.Roles.Registered 
            };

            // Act & Assert
            Assert.Throws<ValidationException>(() => _userServiceMock.CreatePrivilegedUser(dto));
        }

        [Fact]
        public void CreatePrivilegedUser_UsernameNotUnique_ThrowsValidationException()
        {
            // Arrange 
            var username = _fixture.Create<string>();

            var dto = new UserDto
            {
                Id = _fixture.Create<int>(),
                Username = username,
                Password = _fixture.Create<string>(),
                Role = BLL.Enums.Roles.Admin
            };

            var existingUser = new User
            {
                Id = _fixture.Create<int>(),
                Username = username,
                Password = _fixture.Create<string>(),
                Role = DAL.Enums.Roles.Registered
            };

            _unitOfWorkMock.GetRepository<User>().GetAll().Returns(new List<User> { existingUser });

            // Act & Assert
            Assert.Throws<ValidationException>(() => _userServiceMock.CreatePrivilegedUser(dto));
        }
        [Fact]
        public void DeleteUser_UserNotFound_ThrowsNotFoundException()
        {
            // Arrange
            int id = _fixture.Create<int>();
            _unitOfWorkMock.GetRepository<User>().Get(id).Returns((User)null);

            // Act & Assert
            Assert.Throws<NotFoundException>(() => _userServiceMock.DeleteUser(id));
        }

        [Fact]
        public void GetUserById_UserNotFound_ThrowsNotFoundException()
        {
            // Arrange
            int id = _fixture.Create<int>();
            _unitOfWorkMock.GetRepository<User>().Get(id).Returns((User)null);

            // Act & Assert
            Assert.Throws<NotFoundException>(() => _userServiceMock.GetUserById(id));
        }

        [Fact]
        public void ChangeUserPassword_UserNotFound_ThrowsNotFoundException()
        {
            // Arrange
            int id = _fixture.Create<int>();
            _unitOfWorkMock.GetRepository<User>().Get(id).Returns((User)null);

            // Act & Assert
            Assert.Throws<NotFoundException>(() => _userServiceMock.ChangeUserPassword(id, "newPassword"));
        }

        [Fact]
        public void GetOrderByUser_UserNotFound_ThrowsNotFoundException()
        {
            // Arrange
            int id = _fixture.Create<int>();
            _unitOfWorkMock.GetRepository<User>().Get(id).Returns((User)null);

            // Act & Assert
            Assert.Throws<NotFoundException>(() => _userServiceMock.GetOrderByUser(id));
        }

        [Fact]
        public void Authenticate_UserNotFound_ThrowsAuthenticationException()
        {
            // Arrange
            var username = _fixture.Create<string>();
            var password = _fixture.Create<string>();

            User user = null;

            _unitOfWorkMock.GetRepository<User>()
                .Find(Arg.Any<Expression<Func<User, bool>>>())
                .Returns(user);

            // Act & Assert
            Assert.Throws<AuthenticationException>(() => _userServiceMock.Authenticate(username, password));
        }


        [Fact]
        public void Authenticate_WrongPassword_ThrowsAuthenticationException()
        {
            // Arrange
            var username = _fixture.Create<string>();
            var correctPassword = _fixture.Create<string>();
            var wrongPassword = _fixture.Create<string>();

            var dto = new UserDto
            {
                Username = username,
                Password = wrongPassword
            };

            var user = new User
            {
                Id = _fixture.Create<int>(),
                Username = username,
                Password = correctPassword,
                Role = DAL.Enums.Roles.Registered
            };

            _unitOfWorkMock.GetRepository<User>()
                .Find(Arg.Any<Expression<Func<User, bool>>>())
                .Returns(user);

            // Act & Assert
            Assert.Throws<AuthenticationException>(() => _userServiceMock.Authenticate(dto.Username, dto.Password));
        }


    }
}