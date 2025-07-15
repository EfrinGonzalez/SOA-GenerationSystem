using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface  ICustomerService
    {
        //Gets all Customers endpoint
        //Get Customer by Id endpoint
        //Create Customer endpoint

        Task<IEnumerable<Customer>> GetAllAsync();

        Task<Customer> GetByIdAsync(Guid id);

        Task AddAsync(Customer customer);
    }
}
