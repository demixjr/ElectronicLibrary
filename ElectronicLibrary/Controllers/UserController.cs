using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BLL.IServices;
using AutoMapper;
using ElectronicLibrary.Models.ResponseModels;
using ElectronicLibrary.Models.RequestModels;
using BLL.dto;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authorization;
using BLL.Exceptions;
using System.Security.Claims;
using ElectronicLibrary.Helpers;

namespace ElectronicLibrary.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public UserController(IUserService userService, IMapper mapper, ITokenService tokenService)
        {
            _userService = userService;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "UserPolicy")]
        public ActionResult<UserResponseModel> Get(int id)
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

                var dto = _userService.GetUserById(id);
                var responseModel = _mapper.Map<UserResponseModel>(dto);

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
        public ActionResult<IEnumerable<UserResponseModel>> GetAll()
        {
            try
            {
                var dtos = _userService.GetAllUsers();
                var responseModels = _mapper.Map<IEnumerable<UserResponseModel>>(dtos);

                return Ok(responseModels);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Сталася неочікувана помилка." });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult<UserResponseModel> Register([FromBody] UserRequestModel requestModel)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var dto = _mapper.Map<UserDto>(requestModel);
                var createdDto = _userService.RegisterUser(dto);
                var responseModel = _mapper.Map<UserResponseModel>(createdDto);

                return CreatedAtAction(nameof(Get), new { id = responseModel.Id }, responseModel);
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

        [HttpPost("privileged")]
        [Authorize(Roles = "Admin")]
        public ActionResult<UserResponseModel> CreatePrivilegedUser([FromBody] PrivilegedUserRequestModel requestModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var dto = _mapper.Map<UserDto>(requestModel);
                var createdDto = _userService.CreatePrivilegedUser(dto);
                var responseModel = _mapper.Map<UserResponseModel>(createdDto);

                return CreatedAtAction(nameof(Get), new { id = responseModel.Id }, responseModel);
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

        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult<LoginResponseModel> Login([FromBody] LoginRequestModel requestModel)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var dto = _userService.Authenticate(requestModel.Username!, requestModel.Password!);
                var token = _tokenService.GenerateToken(dto);

                return Ok(new LoginResponseModel
                {
                    Token = token,
                    Username = dto.Username,
                    Email = dto.Email,
                    Role = dto.Role,
                });
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Сталася неочікувана помилка." });
            }
        }

        [HttpPut("change-password/{id}")]
        [Authorize(Policy = "UserPolicy")]
        public ActionResult<UserResponseModel> ChangeUserPassword(int id, [FromBody] UserRequestModel requestModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (!User.TryGetUserId(out var currentUserId))
                {
                    return Unauthorized();
                }

                if (currentUserId != id)
                {
                    return Forbid();
                }

                _userService.ChangeUserPassword(id, requestModel.Password!);
                var dto = _userService.GetUserById(id);
                var responseModel = _mapper.Map<UserResponseModel>(dto);

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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                _userService.DeleteUser(id);
                return NoContent();
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