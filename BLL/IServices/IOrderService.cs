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
        void ClearOrder(int orderId);
        void DeleteBookFromOrder(int orderId, int bookId);
        IEnumerable<OrderDto> GetAllOrders();
        OrderDto GetOrderById(int orderId);
        OrderDto GetOrderByUser(int userId);
    }
}