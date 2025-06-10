using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using SOA.Commands;
using SOA.DTOs;
using SOA.Interfaces;
using SOA.Queries;

namespace SOA.UsersAPI.Controllers
{
   
        [ApiController]
        [Route("users")]
        public class UsersController : ControllerBase
        {
            private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
                _mediator = mediator;
        }

       
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
       
    }
    
}
