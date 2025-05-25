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
        public void AddOrder_UserHasOrder_ThrowsValidationException()
        {
            // Arrange
            var dto = new OrderDto
            {
                UserId = _fixture.Create<int>()
            };

            var userWithOrder = new User
            {
                Id = dto.UserId,
                Order = new Order() 
            };

            _unitOfWorkMock.GetRepository<User>().Get(dto.UserId).Returns(userWithOrder);

            // Act & Assert
            var ex = Assert.Throws<ValidationException>(() => _orderServiceMock.AddOrder(dto));
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
        public void DeleteBookFromOrder_BookExists_RemovesBookAndUpdatesPrice()
        {
            //Arrangre
            int orderId = _fixture.Create<int>(), bookId = _fixture.Create<int>();
            var book = new Book { Id = bookId, Price = 100m };
            var order = new Order { Id = orderId, Books = new List<Book> { book }, TotalPrice = 100m };

            _orderRepositoryMock.Get(orderId).Returns(order);

            //Act
            _orderServiceMock.DeleteBookFromOrder(orderId, bookId);

            // Assert
            Assert.DoesNotContain(book, order.Books);
            Assert.Equal(0m, order.TotalPrice);
            _unitOfWorkMock.Received(1).Save();
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
        public void ClearOrder_OrderExists_ClearsBooksAndSetsTotalPriceToZero()
        {
            //Arrange
            int orderId = _fixture.Create<int>();
            var books = new List<Book> { new Book(), new Book() };
            var order = new Order { Id = orderId, Books = books, TotalPrice = _fixture.Create<decimal>() };

            _orderRepositoryMock.Get(orderId).Returns(order);

            //Act
            _orderServiceMock.ClearOrder(orderId);

            //Assert
            Assert.Empty(order.Books);
            Assert.Equal(0m, order.TotalPrice);
            _unitOfWorkMock.Received(1).Save();
        }


        [Fact]
        public void GetAllBooksInOrder_OrderNotFound_ThrowsNotFoundException()
        {
            //Arrange
            int orderId = _fixture.Create<int>();
            _orderRepositoryMock.Get(orderId).Returns((Order)null);

            // Act & Assert
            var ex = Assert.Throws<NotFoundException>(() => _orderServiceMock.GetAllBooksInOrder(orderId));

        }

        [Fact]
        public void GetAllBooksInOrder_OrderExists_ReturnsMappedBooks()
        {
            //Arrange
            int orderId = _fixture.Create<int>();
            var books = new List<Book> { _fixture.Create<Book>(), _fixture.Create<Book>() };
            var order = new Order { Id = orderId, Books = books };
            var booksDto = _fixture.CreateMany<BookDto>(books.Count).ToList();

            _orderRepositoryMock.Get(orderId).Returns(order);
            _mapperMock.Map<IEnumerable<BookDto>>(books).Returns(booksDto);

            //Act

            var result = _orderServiceMock.GetAllBooksInOrder(orderId);

            //Assert
            Assert.Equal(booksDto, result);
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

            _unitOfWorkMock.GetRepository<Order>().GetAll().Returns(orders);
            _mapperMock.Map<IEnumerable<OrderDto>>(orders).Returns(ordersDto);

            //Act

            var result = _orderServiceMock.GetAllOrders();

            //Assert

            Assert.Equal(ordersDto, result);
        }




        [Fact]
        public void GetOrder_OrderNotFound_ThrowsNotFoundException()
        {
            //Arrange
            int orderId = _fixture.Create<int>();
            _orderRepositoryMock.Get(orderId).Returns((Order)null);

            // Act & Assert
            var ex = Assert.Throws<NotFoundException>(() => _orderServiceMock.GetOrder(orderId));
        }

        [Fact]
        public void GetOrder_OrderExists_ReturnsMappedOrderDto()
        {
            //Arrange
            int orderId = _fixture.Create<int>();

            var user = new User { Id = _fixture.Create<int>() };
            var order = new Order
            {
                Id = orderId,
                User = user,
                Books = new List<Book> { new Book(), new Book() }
            };

            var dto = new OrderDto
            {
                Id = orderId,
                UserId = user.Id,
            };

            _unitOfWorkMock.GetRepository<Order>().Get(orderId).Returns(order);
            _mapperMock.Map<OrderDto>(order).Returns(dto);

            //Act

            var result = _orderServiceMock.GetOrder(orderId);

            //Assert
            Assert.Equal(dto, result);
        }

    }
}
