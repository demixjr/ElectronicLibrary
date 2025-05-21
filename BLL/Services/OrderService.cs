using BLL.dto;
using BLL.IServices;
using DAL;
using BLL.Exceptions;
using DAL.Models;
using AutoMapper;

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
            var order = _unitOfWork.GetRepository<Order>().Get(orderId);
            var book = _unitOfWork.GetRepository<Book>().Get(bookId);
            if (book == null || order == null)
            {
                throw new NotFoundException();
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
                throw new NotFoundException();
            }

            var entity = _mapper.Map<Order>(dto);

            entity.Books = new List<Book>();

            _unitOfWork.GetRepository<Order>().Add(entity);
            _unitOfWork.Save();

            return _mapper.Map<OrderDto>(entity);
        }

        public void DeleteBookFromOrder(int orderId, int bookId)
        {
            var order = _unitOfWork.GetRepository<Order>().Get(orderId);
            if (order == null)
            {
                throw new NotFoundException();
            }

            var book = order.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                throw new NotFoundException();
            }

            order.Books.Remove(book);
            order.TotalPrice -= book.Price;

            _unitOfWork.Save();
        }

        public void DeleteOrder(int orderId)
        {
            var order = _unitOfWork.GetRepository<Order>().Get(orderId);
            if (order == null)
            {
                throw new NotFoundException();
            }

            _unitOfWork.GetRepository<Order>().Remove(order);
            _unitOfWork.Save();
        }

        public IEnumerable<BookDto> GetAllBooksInOrder(int orderId)
        {
            var order = _unitOfWork.GetRepository<Order>().Get(orderId);
            if (order == null)
            {
                throw new NotFoundException();
            }

            return _mapper.Map<IEnumerable<BookDto>>(order.Books);
        }

        public IEnumerable<OrderDto> GetAllOrders()
        {
            var entities = _unitOfWork.GetRepository<Order>().GetAll();
            return _mapper.Map<IEnumerable<OrderDto>>(entities);
        }

        public OrderDto GetOrder(int orderId)
        {
            var entity = _unitOfWork.GetRepository<Order>().Get(orderId);
            if (entity == null)
            {
                throw new NotFoundException();
            }
            return _mapper.Map<OrderDto>(entity);
        }
    }
}
