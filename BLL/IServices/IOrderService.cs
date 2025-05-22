using BLL.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.IServices
{
    public interface IOrderService
    {
        OrderDto AddOrder(OrderDto dto);
        void AddBookToOrder(int orderId, int bookId);
        void DeleteOrder(int orderId);
        void DeleteBookFromOrder(int orderId, int bookId);
        IEnumerable<BookDto> GetAllBooksInOrder(int orderId);
        IEnumerable<OrderDto> GetAllOrders();
        OrderDto GetOrder(int orderId);
    }
}