using AutoFixture;
using AutoMapper;
using BLL.dto;
using BLL.Exceptions;
using DAL.Enums;
using BLL.IServices;
using BLL.Services;
using DAL.Models;
using DAL;
using NSubstitute;
using System.Linq.Expressions;
using ElectronicLibrary;

namespace UnitTests
{
        public class BookServiceTests
        {
        private readonly DIContainer _container;

        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IMapper _mapperMock;
        private readonly IRepository<Book> _bookRepositoryMock;
        private readonly IBookService _bookServiceMock;
        private readonly IFixture _fixture;

        public BookServiceTests()
        {
            _container = new DIContainer();

            _unitOfWorkMock = _container.Get<IUnitOfWork>();
            _mapperMock = _container.Get<IMapper>();
            _bookRepositoryMock = _unitOfWorkMock.GetRepository<Book>();
            _bookServiceMock = _container.Get<IBookService>();
            _fixture = _container.Get<IFixture>();

        }

        [Fact]
            public void AddBook_ShouldAddBook_WhenTitleIsUnique()
            {
                // Arrange
                var dto = _fixture.Create<BookDto>();
                var entity = _fixture.Create<Book>();

                _bookRepositoryMock.GetAll().Returns(new List<Book>()); 
                _mapperMock.Map<Book>(dto).Returns(entity);
                _mapperMock.Map<BookDto>(entity).Returns(dto);

                // Act
                var result = _bookServiceMock.AddBook(dto);

                // Assert
                _bookRepositoryMock.Received(1).Add(entity);
                _unitOfWorkMock.Received(1).Save();
                Assert.Equal(dto, result);
            }

            [Fact]
            public void AddBook_ShouldThrowValidationException_WhenTitleExists()
            {
                // Arrange
                var dto = _fixture.Create<BookDto>();
                var existingBook = _fixture.Create<Book>();
                existingBook.Title = dto.Title;

                _bookRepositoryMock.GetAll().Returns(new List<Book> { existingBook });

                // Act & Assert
                var ex = Assert.Throws<ValidationException>(() => _bookServiceMock.AddBook(dto));
                Assert.Contains(dto.Title, ex.Message);
            }
        [Fact]
        public void UpdateBook_ShouldUpdateBook_WhenDataIsValid()
        {
            // Arrange
            int bookId = _fixture.Create<int>();
            var dto = _fixture.Create<BookDto>();
            var existingBook = _fixture.Create<Book>();

            _bookRepositoryMock.Get(bookId).Returns(existingBook);
            _bookRepositoryMock.GetAll().Returns(new List<Book> { existingBook });

            // Act
            _bookServiceMock.UpdateBook(bookId, dto);

            // Assert
            _mapperMock.Received(1).Map(dto, existingBook);
            _unitOfWorkMock.Received(1).Save();
        }

        [Fact]
        public void UpdateBook_ShouldThrowNotFoundException_WhenBookNotFound()
        {
            // Arrange
            int bookId = _fixture.Create<int>();
            var dto = _fixture.Create<BookDto>();

            _bookRepositoryMock.Get(bookId).Returns((Book)null);
            var bookService = new BookService(_unitOfWorkMock, _mapperMock);

            // Act & Assert
            var ex = Assert.Throws<NotFoundException>(() => bookService.UpdateBook(bookId, dto));
        }

        [Fact]
        public void UpdateBook_ShouldThrowValidationException_WhenTitleExists()
        {
            // Arrange
            int bookId = _fixture.Create<int>();
            var dto = _fixture.Create<BookDto>();

            var existingBook = _fixture.Create<Book>();
            existingBook.Title = _fixture.Create<string>();


            _bookRepositoryMock.Get(bookId).Returns(existingBook);
            var conflictingBook = _fixture.Create<Book>();
            conflictingBook.Title = dto.Title;

            _bookRepositoryMock.GetAll().Returns(new List<Book> { conflictingBook });

            // Act & Assert
            var ex = Assert.Throws<ValidationException>(() => _bookServiceMock.UpdateBook(bookId, dto));
            Assert.Contains(dto.Title, ex.Message);
        }
        [Fact]
        public void GetBook_ShouldReturnBookDto_WhenBookExists()
        {
            // Arrange
            int bookId = _fixture.Create<int>();
            var bookEntity = _fixture.Create<Book>();
            var bookDto = _fixture.Create<BookDto>();

            _bookRepositoryMock.Get(bookId).Returns(bookEntity);
            _mapperMock.Map<BookDto>(bookEntity).Returns(bookDto);

            // Act
            var result = _bookServiceMock.GetBook(bookId);

            // Assert
            Assert.Equal(bookDto, result);
            _bookRepositoryMock.Received(1).Get(bookId);
            _mapperMock.Received(1).Map<BookDto>(bookEntity);
        }

        [Fact]
        public void GetBook_ShouldReturnNull_WhenBookDoesNotExist()
        {
            // Arrange
            int bookId = _fixture.Create<int>();

            _bookRepositoryMock.Get(bookId).Returns((Book)null);
            _mapperMock.Map<BookDto>(null).Returns((BookDto)null);

            // Act
            var result = _bookServiceMock.GetBook(bookId);

            // Assert
            Assert.Null(result);
            _bookRepositoryMock.Received(1).Get(bookId);
            _mapperMock.Received(1).Map<BookDto>(null);
        }
        [Fact]
            public void DeleteBook_ShouldRemoveBook_WhenBookExists()
            {
                // Arrange
                var bookId = _fixture.Create<int>();
                var book = _fixture.Create<Book>();

                _bookRepositoryMock.Get(bookId).Returns(book);

                // Act
                _bookServiceMock.DeleteBook(bookId);

                // Assert
                _bookRepositoryMock.Received(1).Remove(book);
                _unitOfWorkMock.Received(1).Save();
            }


        [Fact]
        public void DeleteBook_ShouldThrowNotFoundException_WhenBookNotFound()
        {
            // Arrange
            var bookId = _fixture.Create<int>();

            _bookRepositoryMock.Get(bookId).Returns((Book)null);
            var service = new BookService(_unitOfWorkMock, _mapperMock);

            // Act & Assert
            Assert.Throws<NotFoundException>(() => service.DeleteBook(bookId));
        }

        [Fact]
        public void FindBookByTitle_ShouldReturnBookDto_WhenBookExists()
        {
            // Arrange
            string title = _fixture.Create<string>();
            var bookEntity = _fixture.Create<Book>();
            var bookDto = _fixture.Create<BookDto>();

            _bookRepositoryMock.Find(Arg.Any<Expression<Func<Book, bool>>>()).Returns(bookEntity);
            _mapperMock.Map<BookDto>(bookEntity).Returns(bookDto);

            // Act
            var result = _bookServiceMock.FindBookByTitle(title);

            // Assert
            Assert.Equal(bookDto, result);
            _bookRepositoryMock.Received(1).Find(Arg.Any<Expression<Func<Book, bool>>>());
            _mapperMock.Received(1).Map<BookDto>(bookEntity);
        }

        [Fact]
        public void FindBookByTitle_ShouldReturnNull_WhenBookDoesNotExist()
        {
            // Arrange
            string title = _fixture.Create<string>();

            _bookRepositoryMock.Find(Arg.Any<Expression<Func<Book, bool>>>()).Returns((Book)null);
            _mapperMock.Map<BookDto>(null).Returns((BookDto)null);

            // Act
            var result = _bookServiceMock.FindBookByTitle(title);

            // Assert
            Assert.Null(result);
            _bookRepositoryMock.Received(1).Find(Arg.Any<Expression<Func<Book, bool>>>());
            _mapperMock.Received(1).Map<BookDto>(null);
        }

        [Theory]
        [InlineData("Paper", "Comedy")]
        [InlineData("paper", "comedy")]
        public void FindBooksByCriterion_ShouldReturnMappedBooks_WhenValidTypeAndGenre(string type, string genre)
        {
            // Arrange
            var bookList = _fixture.CreateMany<Book>(3).ToList();
            var dtoList = _fixture.CreateMany<BookDto>(3).ToList();

            // Исправлено на Func<Book, bool>
            _bookRepositoryMock.FindAll(Arg.Any<Func<Book, bool>>()).Returns(bookList);
            _mapperMock.Map<IEnumerable<BookDto>>(bookList).Returns(dtoList);

            // Act
            var result = _bookServiceMock.FindBooksByCriterion(type, genre);

            // Assert
            Assert.Equal(dtoList, result);
            _bookRepositoryMock.Received(1).FindAll(Arg.Any<Func<Book, bool>>());
            _mapperMock.Received(1).Map<IEnumerable<BookDto>>(bookList);
        }

        [Theory]
        [InlineData("InvalidType", "Horror")]
        [InlineData("Audio", "InvalidGenre")]
        [InlineData("InvalidType", "InvalidGenre")]
        public void FindBooksByCriterion_ShouldReturnEmpty_WhenInvalidTypeOrGenre(string type, string genre)
        {
            // Act
            var result = _bookServiceMock.FindBooksByCriterion(type, genre);

            // Assert
            Assert.Empty(result);
            _bookRepositoryMock.DidNotReceive().FindAll(Arg.Any<Func<Book, bool>>());
            _mapperMock.DidNotReceive().Map<IEnumerable<BookDto>>(Arg.Any<IEnumerable<Book>>());
        }

        [Fact]
        public void SortByName_ShouldReturnBooksSortedByTitle()
        {
            // Arrange
            var books = new List<Book>
    {
        new Book { Title = "C Title" },
        new Book { Title = "A Title" },
        new Book { Title = "B Title" }
    };
            var sortedBooks = books.OrderBy(b => b.Title).ToList();

            var dtoList = _fixture.CreateMany<BookDto>(3).ToList();

            _bookRepositoryMock.GetAll().Returns(books);
            _mapperMock.Map<IEnumerable<BookDto>>(Arg.Is<IEnumerable<Book>>(b => b.SequenceEqual(sortedBooks))).Returns(dtoList);

            // Act
            var result = _bookServiceMock.SortByName();

            // Assert
            Assert.Equal(dtoList, result);
            _bookRepositoryMock.Received(1).GetAll();
            _mapperMock.Received(1).Map<IEnumerable<BookDto>>(Arg.Any<IEnumerable<Book>>());
        }

        [Fact]
        public void SortByGenres_ShouldReturnBooksSortedByGenre()
        {
            // Arrange
            var books = new List<Book>
    {
        new Book { Genre = Genres.Horror },
        new Book { Genre = Genres.Comedy },
        new Book { Genre = Genres.Novel }
    };
            var sortedBooks = books.OrderBy(b => b.Genre).ToList();

            var dtoList = _fixture.CreateMany<BookDto>(3).ToList();

            _bookRepositoryMock.GetAll().Returns(books);
            _mapperMock.Map<IEnumerable<BookDto>>(Arg.Is<IEnumerable<Book>>(b => b.SequenceEqual(sortedBooks))).Returns(dtoList);

            // Act
            var result = _bookServiceMock.SortByGenres();

            // Assert
            Assert.Equal(dtoList, result);
            _bookRepositoryMock.Received(1).GetAll();
            _mapperMock.Received(1).Map<IEnumerable<BookDto>>(Arg.Any<IEnumerable<Book>>());
        }

        [Fact]
        public void SortByType_ShouldReturnBooksSortedByBookType()
        {
            // Arrange
            var books = new List<Book>
    {
        new Book { BookType = BookTypes.Paper },
        new Book { BookType = BookTypes.Paper },
        new Book { BookType = BookTypes.Audio }
    };
            var sortedBooks = books.OrderBy(b => b.BookType).ToList();

            var dtoList = _fixture.CreateMany<BookDto>(3).ToList();

            _bookRepositoryMock.GetAll().Returns(books);
            _mapperMock.Map<IEnumerable<BookDto>>(Arg.Is<IEnumerable<Book>>(b => b.SequenceEqual(sortedBooks))).Returns(dtoList);

            // Act
            var result = _bookServiceMock.SortByType();

            // Assert
            Assert.Equal(dtoList, result);
            _bookRepositoryMock.Received(1).GetAll();
            _mapperMock.Received(1).Map<IEnumerable<BookDto>>(Arg.Any<IEnumerable<Book>>());
        }

        [Fact]
        public void GetAllBooks_ShouldReturnAllBooksMapped()
        {
            // Arrange
            var books = _fixture.CreateMany<Book>(3).ToList();
            var dtoList = _fixture.CreateMany<BookDto>(3).ToList();

            _bookRepositoryMock.GetAll().Returns(books);
            _mapperMock.Map<IEnumerable<BookDto>>(books).Returns(dtoList);

            // Act
            var result = _bookServiceMock.GetAllBooks();

            // Assert
            Assert.Equal(dtoList, result);
            _bookRepositoryMock.Received(1).GetAll();
            _mapperMock.Received(1).Map<IEnumerable<BookDto>>(books);
        }

    }
}
