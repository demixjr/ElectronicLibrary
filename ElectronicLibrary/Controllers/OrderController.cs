using AutoMapper;
using BLL.IServices;
using ElectronicLibrary.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using BLL.Exceptions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ElectronicLibrary.Helpers;
using DAL.Models;

namespace ElectronicLibrary.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<OrderResponseModel> Get(int id)
        {
            try
            {
                var dto = _orderService.GetOrderById(id);
                var responseModel = _mapper.Map<OrderResponseModel>(dto);

                return Ok(responseModel);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Сталася неочікувана помилка." });
            }
        }

        [HttpGet("by-user/{id}")]
        [Authorize(Policy = "UserPolicy")]
        public ActionResult<UserResponseModel> GetByUser(int id)
        {
            try
            {
                if (!User.TryGetUserId(out var currentUserId))
                {
                    return Unauthorized();
                }

                if (currentUserId != id && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var dto = _orderService.GetOrderByUser(id);
                var responseModel = _mapper.Map<OrderResponseModel>(dto);

                return Ok(responseModel);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Сталася неочікувана помилка." });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<OrderResponseModel>> GetAll()
        {
            try
            {
                var dtos = _orderService.GetAllOrders();
                var responseModels = _mapper.Map<IEnumerable<OrderResponseModel>>(dtos);

                return Ok(responseModels);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Сталася неочікувана помилка." });
            }
        }

        [HttpPost("add-book")]
        [Authorize(Policy = "UserPolicy")]
        public IActionResult AddBookToOrder([FromQuery] int orderId, [FromQuery] int bookId)
        {
            try
            {
                if (!User.TryGetUserId(out var currentUserId))
                {
                    return Unauthorized();
                }

                var userOrder = _orderService.GetOrderByUser(currentUserId);
                if (userOrder.Id != orderId)
                {
                    return Forbid();
                }

                _orderService.AddBookToOrder(orderId, bookId);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Сталася неочікувана помилка." });
            }
        }

        [HttpDelete("remove-book")]
        [Authorize(Policy = "UserPolicy")]
        public IActionResult RemoveBook([FromQuery] int orderId, [FromQuery] int bookId)
        {
            try
            {
                if (!User.TryGetUserId(out var currentUserId))
                {
                    return Unauthorized();
                }

                var userOrder = _orderService.GetOrderByUser(currentUserId);
                if (userOrder.Id != orderId)
                {
                    return Forbid();
                }

                _orderService.DeleteBookFromOrder(orderId, bookId);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Сталася неочікувана помилка." });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "UserPolicy")]
        public IActionResult ClearOrder(int id)
        {
            try
            {
                if (!User.TryGetUserId(out var currentUserId))
                {
                    return Unauthorized();
                }

                var userOrder = _orderService.GetOrderByUser(currentUserId);
                if (userOrder.Id != id)
                {
                    return Forbid();
                }

                _orderService.ClearOrder(id);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Сталася неочікувана помилка." });
            }
        }
    }
}
