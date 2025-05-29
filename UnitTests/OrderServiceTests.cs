using AutoFixture;
using AutoMapper;
using BLL.IServices;
using DAL.Models;
using DAL;
using ElectronicLibrary;
using BLL.Exceptions;
using BLL.Services;
using NSubstitute;
using BLL.dto;
using System.Linq.Expressions;

namespace UnitTests
{
    public class OrderServiceTests
    {
        private readonly DIContainer _container;

        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IMapper _mapperMock;
        private readonly IRepository<Order> _orderRepositoryMock;
        private readonly IRepository<User> _userRepositoryMock;
        private readonly IOrderService _orderServiceMock;
        private readonly IFixture _fixture;

        public OrderServiceTests()
        {
            _container = new DIContainer();

            _unitOfWorkMock = _container.Get<IUnitOfWork>();
            _mapperMock = _container.Get<IMapper>();
            _orderRepositoryMock = _unitOfWorkMock.GetRepository<Order>();
            _orderServiceMock = _container.Get<IOrderService>();
            _fixture = _container.Get<IFixture>();


        }
      

        [Fact]
        public void AddOrder_UserNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var dto = new OrderDto
            {
                UserId = _fixture.Create<int>()
            };

            _unitOfWorkMock.GetRepository<User>().Get(dto.UserId).Returns((User)null);

            // Act & Assert
            var ex = Assert.Throws<NotFoundException>(() => _orderServiceMock.AddOrder(dto));
        }

        [Fact]
        public void AddOrder_ValidUser_AddsOrderAndReturnsDto()
        {
            // Arrange
            var dto = new OrderDto
            {
                UserId = _fixture.Create<int>()
            };

            var user = new User
            {
                Id = dto.UserId,
                Order = null 
            };

            _unitOfWorkMock.GetRepository<User>().Get(dto.UserId).Returns(user);

            var orderEntity = new Order
            {
                Id = _fixture.Create<int>(),
                User = user,
                Books = new List<Book>()
            };

            _mapperMock.Map<Order>(dto).Returns(orderEntity);
            _mapperMock.Map<OrderDto>(orderEntity).Returns(dto);

            Order addedOrder = null;
            _orderRepositoryMock
                .When(x => x.Add(Arg.Any<Order>()))
                .Do(ci => addedOrder = ci.Arg<Order>());

            // Act
            var result = _orderServiceMock.AddOrder(dto);

            // Assert
            _orderRepositoryMock.Received(1).Add(Arg.Any<Order>());
            _unitOfWorkMock.Received(1).Save();

            Assert.Equal(dto, result);
            Assert.NotNull(addedOrder);
            Assert.Empty(addedOrder.Books);
        }

        [Fact]
        public void DeleteBookFromOrder_OrderNotFound_ThrowsNotFoundException()
        {
            //Arrange
            int orderId = _fixture.Create<int>(), bookId = _fixture.Create<int>();
            _orderRepositoryMock.Get(orderId).Returns((Order)null);
            // Act & Assert
            var ex = Assert.Throws<NotFoundException>(() => _orderServiceMock.DeleteBookFromOrder(orderId, bookId));
        }

        [Fact]
        public void DeleteBookFromOrder_BookNotFound_ThrowsNotFoundException()
        {
            //Arrange
            int orderId = _fixture.Create<int>(), bookId = _fixture.Create<int>();
            var order = new Order { Id = orderId, Books = new List<Book>() };
            _orderRepositoryMock.Get(orderId).Returns(order);

            // Act & Assert
            var ex = Assert.Throws<NotFoundException>(() => _orderServiceMock.DeleteBookFromOrder(orderId, bookId));

        }

        [Fact]
        public void DeleteBookFromOrder_BookNotExists_ThrowsNotFoundException()
        {
            // Arrange
            int orderId = _fixture.Create<int>(), bookId = _fixture.Create<int>();
            var book = new Book { Id = bookId, Price = 100m };
            var order = new Order { Id = orderId, Books = new List<Book> { book }, TotalPrice = 100m };

            _orderRepositoryMock.Get(orderId).Returns(order);

            int nonExistentBookId = bookId + 1; 

            // Act & Assert
            Assert.Throws<NotFoundException>(() =>
                _orderServiceMock.DeleteBookFromOrder(orderId, nonExistentBookId));
        }


        [Fact]
        public void ClearOrder_OrderNotFound_ThrowsNotFoundException()
        {
            //Arrange
            int orderId = _fixture.Create<int>();
            _orderRepositoryMock.Get(orderId).Returns((Order)null);
            // Act & Assert
            var ex = Assert.Throws<NotFoundException>(() => _orderServiceMock.ClearOrder(orderId));

        }

      


        [Fact]
        public void GetAllBooksInOrder_OrderNotFound_ThrowsNotFoundException()
        {
            //Arrange
            int orderId = _fixture.Create<int>();
            _orderRepositoryMock.Get(orderId).Returns((Order)null);

            // Act & Assert
            var ex = Assert.Throws<NotFoundException>(() => _orderServiceMock.GetOrderById(orderId).Books);

        }

        [Fact]
        public void GetAllOrders_ReturnsMappedOrders()
        {
            // Arrange
            var users = new List<User>
    {
       new User { Id =  _fixture.Create<int>(), Order = null },
       new User { Id =  _fixture.Create<int>(), Order = null },
       new User { Id =  _fixture.Create<int>(), Order = null }
    };

            var orders = users.Select(u => new Order
            {
                User = u,
                Books = new List<Book> { new Book(), new Book() }
            }).ToList();

            var ordersDto = new List<OrderDto>
    {
       new OrderDto(),
       new OrderDto(),
       new OrderDto()
    };

            _unitOfWorkMock.GetRepository<Order>().GetAll(Arg.Any<Expression<Func<Order, object>>[]>())
                .Returns(orders);

            _mapperMock.Map<IEnumerable<OrderDto>>(orders).Returns(ordersDto);
            var _orderService = new OrderService(_unitOfWorkMock, _mapperMock);
            // Act
            var result = _orderService.GetAllOrders();

            // Assert
            Assert.Equal(ordersDto, result);
        }





        [Fact]
        public void GetOrder_OrderNotFound_ThrowsNotFoundException()
        {
            //Arrange
            int orderId = _fixture.Create<int>();
            _orderRepositoryMock.Get(orderId).Returns((Order)null);

            // Act & Assert
            var ex = Assert.Throws<NotFoundException>(() => _orderServiceMock.GetOrderById(orderId));
        }

       


    }
}
