using BLL.dto;

namespace BLL.IServices
{
    public interface IBookService
    {
        BookDto AddBook(BookDto dto);
        void UpdateBook(int id, BookDto dto);
        BookDto GetBook(int id);
        void DeleteBook(int bookId);
        BookDto FindBookByTitle(string title);
        IEnumerable<BookDto> FindBooksByCriterion(string type, string genre);
        IEnumerable<BookDto> SortByName();
        IEnumerable<BookDto> SortByGenres();
        IEnumerable<BookDto> SortByType();
        IEnumerable<BookDto> GetAllBooks();
    }
}
