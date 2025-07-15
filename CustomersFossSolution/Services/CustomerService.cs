using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CustomerService : ICustomerService
    {
        private List<Customer> _customers = new();

        public Task AddAsync(Customer customer)
        {
            customer.Id = Guid.NewGuid();
            customer.CreationDate = DateTime.UtcNow;
            _customers.Add(customer);
            return Task.FromResult(customer);


        }

        public Task<IEnumerable<Customer>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Customer>>(_customers);        
                
        }

        public Task<Customer> GetByIdAsync(Guid id)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(customer);
        
        }
      
    }
}
