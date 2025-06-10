using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SOA.Commands;
using SOA.DTOs;
using SOA.Interfaces;
using SOA.Queries;

namespace SOA.UsersAPI.Controllers
{
    [ApiController]
    [Route("admin/users")]
    //[Authorize(Roles = "Admin")] //JWT authentication and authorization. Not setup in this PoC.
    public class AdminsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMediator _mediator;

        public AdminsController(IUserService userService, IMediator mediator)
        {
            _userService = userService;
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            var createdUserId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = createdUserId }, new { id = createdUserId });
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery { Id = id });
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllUsersQuery());
            return Ok(result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
        {
            // if (id != command.UserId) return BadRequest("Mismatched user ID");
            command.UserId = id;
            var result = await _mediator.Send(command);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteUserCommand { UserId = id });
            return result ? NoContent() : NotFound();
        }
    }

}
