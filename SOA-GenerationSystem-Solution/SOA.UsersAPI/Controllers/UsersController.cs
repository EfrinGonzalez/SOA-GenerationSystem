using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SOA.DTOs;
using SOA.Interfaces;

namespace SOA.UsersAPI.Controllers
{
   
        [ApiController]
        [Route("users")]
        public class UsersController : ControllerBase
        {
            private readonly IUserService _userService;

            public UsersController(IUserService userService)
            {
                _userService = userService;
            }

            /// <summary>
            /// Self-registration endpoint for new users.
            /// </summary>
            [HttpPost("register")]
            [AllowAnonymous]
            public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
            {
                try
                {
                    var user = await _userService.RegisterUserAsync(dto);
                    return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
                }
                catch (InvalidOperationException ex)
                {
                    return Conflict(new { message = ex.Message });
                }
            }

            /// <summary>
            /// Retrieve user by ID (self or by authorized logic).
            /// </summary>
            [HttpGet("{id:guid}")]
            [Authorize]
            public async Task<IActionResult> GetById(Guid id)
            {
                var user = await _userService.GetByIdAsync(id);
                return user is null ? NotFound() : Ok(user);
            }

            /// <summary>
            /// Retrieve user by email.
            /// </summary>
            [HttpGet("by-email")]
            [Authorize]
            public async Task<IActionResult> GetByEmail([FromQuery] string email)
            {
                var user = await _userService.GetByEmailAsync(email);
                return user is null ? NotFound() : Ok(user);
            }
        }
    
}
