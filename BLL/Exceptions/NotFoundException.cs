
namespace BLL.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() : base("Сутність не знайдено в базі даних.") { }
    }
}
