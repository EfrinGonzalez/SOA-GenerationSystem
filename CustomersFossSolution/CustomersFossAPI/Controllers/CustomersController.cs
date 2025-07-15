using Domain;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace CustomersFossAPI.Controllers
{
    [ApiController]
    [Route("customers")]
    public class CustomersController : Controller
    {

        private ICustomerService _service;

        public CustomersController(ICustomerService service) {
            _service = service; 
        
        }
        //Gets all Customers endpoint
        //Get Customer by Id endpoint
        //Create Customer endpoint


        [HttpGet("getCustomers")]
        public async Task<IActionResult> GetAll()
        {

            var customers = await _service.GetAllAsync();
            return Ok(customers);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var customer = await _service.GetByIdAsync(id);
            if (customer is null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPost("createCustomer")]
        public async Task<IActionResult> Create([FromBody] Customer customer)
        {
            await _service.AddAsync(customer);
            return CreatedAtAction(nameof(GetById), new { id =customer.Id }, customer);



        }

    }
}
