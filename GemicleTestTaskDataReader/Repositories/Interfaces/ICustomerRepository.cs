using GemicleTestTaskModels.DBModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemicleTestTaskData.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        public Task<List<Customer>> GetAllCustomersAsync();

        public Task<List<Customer>?> GetAllCustomersWithConditionAsync(string condition);

        public Task<Customer> GetCustomerByIdAsync(int id);

        public Task AddCustomerAsync(Customer customer);

        public Task UpdateCustomerAsync(Customer customer);

        public Task DeleteCustomerAsync(int id);
    }
}
