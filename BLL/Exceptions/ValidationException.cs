
namespace BLL.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base("Помилка валідації даних: " + message) { }
    }
}
