using GemicleTestTaskData.Repositories.Interfaces;
using GemicleTestTaskModels.DBModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;

namespace GemicleTestTaskData.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CampaignDbContext _dbContext;

        public CustomerRepository(CampaignDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _dbContext.Customers.ToListAsync();
        }

        public async Task<List<Customer>?> GetAllCustomersWithConditionAsync(string condition)
        {
            try
            {
                var filteredCustomers = await _dbContext.Customers.AsQueryable().Where(condition).ToListAsync();
                return filteredCustomers;
            }
            catch (ParseException e)
            {
                Console.WriteLine("Condition parsing failed: " + e.Message);
                return null;
            }
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            return await _dbContext.Customers.FindAsync(id);
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await _dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Customers ON;");
                    await _dbContext.Customers.AddAsync(customer);
                    await _dbContext.SaveChangesAsync();
                    await _dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Customers OFF;");

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            _dbContext.Entry(customer).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = await _dbContext.Customers.FindAsync(id);
            if (customer != null)
            {
                _dbContext.Customers.Remove(customer);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
