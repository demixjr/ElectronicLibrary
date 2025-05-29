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
            var book = GetBookOrThrowEx(id);
            ValidationOfTitle(dto);
            _unitOfWork.GetRepository<Book>().Update(_mapper.Map(dto, book));
            _unitOfWork.Save();
        }

        public BookDto GetBook(int id)
        {
            var entity = _unitOfWork.GetRepository<Book>().Get(id);
            return _mapper.Map<BookDto>(entity);
        }

        public void DeleteBook(int bookId)
        {
            var entity = GetBookOrThrowEx(bookId);
            _unitOfWork.GetRepository<Book>().Remove(entity);
            _unitOfWork.Save();
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
