using AutoMapper;
using BLL.IServices;
using ElectronicLibrary.Models.RequestModels;
using ElectronicLibrary.Models.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BLL.Exceptions;
using System.Security.Claims;
using BLL.dto;

namespace ElectronicLibrary.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BookController(IBookService bookService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
        }

        //Actions for everyone
        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<BookResponseModel> Get(int id)
        {
            try
            {
                var dto = _bookService.GetBook(id);
                var response = _mapper.Map<BookResponseModel>(dto);
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { message = "Неочікувана помилка." });
            }
        }
        [HttpGet("{title}")]
        [AllowAnonymous]
        public ActionResult<BookResponseModel> GetByTitle(string title)
        {
            try
            {
                var dto = _bookService.FindBookByTitle(title);
                var response = _mapper.Map<BookResponseModel>(dto);
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { message = "Неочікувана помилка." });
            }
        }


        [HttpGet("search/{type}/{genre}")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<BookDto>> FindBooksByCriterion(string type, string genre)
        {
            try
            {
                if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(genre))
                    return BadRequest("Жанр та тип книги є обов'язковими.");

                var result = _bookService.FindBooksByCriterion(type, genre);
                if (result == null || !result.Any())
                    return NotFound("Книг не знайдено за даними критеріями");

                return Ok(result);
            }
            catch
            {
                return StatusCode(500, new { message = "Неочікувана помилка." });
            }
        }

        [HttpGet("sort/name")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<BookDto>> SortByName()
        {
            try
            {
                var result = _bookService.SortByName();
                return Ok(result);
            }
            catch
            {
                return StatusCode(500, new { message = "Неочікувана помилка." });
            }
        }

        [HttpGet("sort/type")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<BookDto>> SortByType()
        {
            try
            {
                var result = _bookService.SortByType();
                return Ok(result);
            }
            catch
            {
                return StatusCode(500, new { message = "Неочікувана помилка." });
            }
        }

        [HttpGet("sort/genre")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<BookDto>> SortByGenres()
        {
            try
            {
                var result = _bookService.SortByGenres();
                return Ok(result);
            }
            catch
            {
                return StatusCode(500, new { message = "Неочікувана помилка." });
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IEnumerable<BookResponseModel>> GetAll()
        {
            try
            {
                var dtos = _bookService.GetAllBooks();
                var response = _mapper.Map<IEnumerable<BookResponseModel>>(dtos);
                return Ok(response);
            }
            catch
            {
                return StatusCode(500, new { message = "Неочікувана помилка." });
            }
        }

        // Admin and manager actions
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<BookResponseModel> AddBook([FromBody] BookRequestModel request)
        {
            try
            {
                var dto = _mapper.Map<BLL.dto.BookDto>(request);
                var added = _bookService.AddBook(dto);
                var response = _mapper.Map<BookResponseModel>(added);
                return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { message = "Неочікувана помилка." });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult UpdateBook(int id, [FromBody] BookRequestModel request)
        {
            try
            {
                var dto = _mapper.Map<BookDto>(request);
                _bookService.UpdateBook(id, dto);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { message = "Неочікувана помилка." });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteBook(int id)
        {
            try
            {
                _bookService.DeleteBook(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { message = "Неочікувана помилка." });
            }
        }

    }
}
