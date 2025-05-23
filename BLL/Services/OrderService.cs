using BLL.dto;
using BLL.IServices;
using DAL;
using BLL.Exceptions;
using DAL.Models;
using AutoMapper;
using System.Net;

namespace BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void AddBookToOrder(int orderId, int bookId)
        {
            var order = GetOrderOrThrow(orderId);

            var book = _unitOfWork.GetRepository<Book>().Get(bookId);
            if (book == null)
            {
                throw new NotFoundException($"Книгу з ID {bookId} не знайдено.");
            }
            if (order.Books.Any(b => b.Id == book.Id))
            {
                throw new ValidationException($"Книга з ID {bookId} вже додана до заказу.");
            }
            order.Books.Add(book);
            order.TotalPrice += book.Price;

            _unitOfWork.Save();
        }

        public OrderDto AddOrder(OrderDto dto)
        {
            var user = _unitOfWork.GetRepository<User>().Get(dto.UserId);
            if (user == null)
            {
                throw new NotFoundException($"Користувача з ID {dto.UserId} не знайдено.");
            }
            if (user.Order != null)
            {
                throw new ValidationException($"Заказ для користувача з ID {user.Id} вже існує.");
            }

            var entity = _mapper.Map<Order>(dto);

            entity.Books = new List<Book>();

            _unitOfWork.GetRepository<Order>().Add(entity);
            _unitOfWork.Save();

            return _mapper.Map<OrderDto>(entity);
        }

        public void DeleteBookFromOrder(int orderId, int bookId)
        {
            var order = GetOrderOrThrow(orderId);

            var book = order.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                throw new NotFoundException($"Книгу з ID {bookId} не знайдено.");
            }

            order.Books.Remove(book);
            order.TotalPrice -= book.Price;

            _unitOfWork.Save();
        }

        public void ClearOrder(int orderId)
        {
            var order = GetOrderOrThrow(orderId);

            order.Books.Clear();
            order.TotalPrice = 0;

            _unitOfWork.Save();
        }

        public IEnumerable<BookDto> GetAllBooksInOrder(int orderId)
        {
            var order = GetOrderOrThrow(orderId);

            return _mapper.Map<IEnumerable<BookDto>>(order.Books);
        }

        public IEnumerable<OrderDto> GetAllOrders()
        {
            var entities = _unitOfWork.GetRepository<Order>().GetAll();

            return _mapper.Map<IEnumerable<OrderDto>>(entities);
        }

        public OrderDto GetOrder(int orderId)
        {
            var entity = GetOrderOrThrow(orderId);

            return _mapper.Map<OrderDto>(entity);
        }

        private Order GetOrderOrThrow(int id)
        {
            var order = _unitOfWork.GetRepository<Order>().Get(id);
            if (order == null)
            {
                throw new NotFoundException($"Заказ з ID {id} не знайдено.");
            }
            return order;
        }
    }
}
