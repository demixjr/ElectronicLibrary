using AutoMapper;
using BLL.dto;
using BLL.IServices;
using DAL.Models;
using DAL;
using BLL.Exceptions;
using DAL.Enums;
using Microsoft.EntityFrameworkCore.Storage;

namespace BLL.Services
{
    public class BookService : IBookService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public BookDto AddBook(BookDto dto)
        {
            ValidationOfTitle(dto);
            var entity = _mapper.Map<Book>(dto);
            _unitOfWork.GetRepository<Book>().Add(entity);
            _unitOfWork.Save();

            return _mapper.Map<BookDto>(entity);
        }

        public void UpdateBook(int id, BookDto dto)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                var book = GetBookOrThrowEx(id);
                if (book.Title != dto.Title)
                {
                    ValidationOfTitle(dto);
                }
                decimal difference = dto.Price - book.Price;
                var orders = _unitOfWork.GetRepository<Order>().GetAll();
                foreach (var order in orders)
                {
                    var books = order.Books;
                    if (books != null)
                    {
                        foreach (var b in books)
                        {
                            if (b!= null && b.Id == id)
                            {
                                order.TotalPrice += difference;
                            }
                        }
                    }
                    _unitOfWork.GetRepository<Order>().Update(order);
                }
                _unitOfWork.GetRepository<Book>().Update(_mapper.Map(dto, book));
                _unitOfWork.Save();
                _unitOfWork.Commit();
            }
            catch(Exception)
            {
                _unitOfWork.Rollback();
            }
        }

        public BookDto GetBook(int id)
        {
            var entity = _unitOfWork.GetRepository<Book>().Get(id);
            return _mapper.Map<BookDto>(entity);
        }

        public void DeleteBook(int bookId)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                var entity = GetBookOrThrowEx(bookId);
                decimal price = entity.Price;
                var orders = _unitOfWork.GetRepository<Order>().GetAll();
                foreach (var order in orders)
                {
                    var books = order.Books;
                    if (books != null)
                    {
                        foreach (var b in books)
                        {
                            if (b != null && b.Id == bookId)
                            {
                                order.TotalPrice -= price;
                            }
                        }
                    }
                    _unitOfWork.GetRepository<Order>().Update(order);
                }

                _unitOfWork.GetRepository<Book>().Remove(entity);
                _unitOfWork.Save();
                _unitOfWork.Commit();
            }
            catch(Exception)
            {
                _unitOfWork.Rollback();
            }
        }

        public BookDto FindBookByTitle(string title)
        {
            var entity = _unitOfWork.GetRepository<Book>().Find(b => b.Title.Equals(title));
            return _mapper.Map<BookDto>(entity);
        }

        public IEnumerable<BookDto> FindBooksByCriterion(string type, string genre)
        {
            if (!Enum.GetNames(typeof(BookTypes)).Any(t => t.Equals(type, StringComparison.OrdinalIgnoreCase)))
                return Enumerable.Empty<BookDto>();
            var enumType = (BookTypes)Enum.Parse(typeof(BookTypes), type, true);

            
            if (!Enum.GetNames(typeof(Genres)).Any(g => g.Equals(genre, StringComparison.OrdinalIgnoreCase)))
                return Enumerable.Empty<BookDto>();
            var enumGenre = (Genres)Enum.Parse(typeof(Genres), genre, true);

            var books = _unitOfWork.GetRepository<Book>().FindAll(b => b.BookType.Equals(enumType) && b.Genre.Equals(enumGenre));
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public IEnumerable<BookDto> SortByName()
        {
            var books = _unitOfWork.GetRepository<Book>().GetAll().OrderBy(b => b.Title);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public IEnumerable<BookDto> SortByGenres()
        {
            var books = _unitOfWork.GetRepository<Book>().GetAll().OrderBy(b => b.Genre);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public IEnumerable<BookDto> SortByType()
        {
            var books = _unitOfWork.GetRepository<Book>().GetAll().OrderBy(b => b.BookType);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public IEnumerable<BookDto> GetAllBooks()
        {
            var books = _unitOfWork.GetRepository<Book>().GetAll();
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }
        private void ValidationOfTitle(BookDto dto)
        {
            var existingEntities = _unitOfWork.GetRepository<Book>().GetAll();

            bool usernameExists = existingEntities.Any(b => b.Title == dto.Title);

            if (usernameExists)
            {
                throw new ValidationException($"Книга {dto.Title} вже існує.");
            }
        }
        private Book GetBookOrThrowEx(int id)
        {
            var book = _unitOfWork.GetRepository<Book>().Get(id);
            if (book == null)
            {
                throw new NotFoundException($"Книгу з ID {id} не знайдено.");
            }
            return book;
        }
    }
}
