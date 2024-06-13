using GemicleTestTaskApi.Services.Interfaces;
using GemicleTestTaskData.Repositories.Interfaces;
using GemicleTestTaskModels.DBModels;

namespace GemicleTestTaskApi.Services
{
    public class CustomerProcessingService: ICustomerProcessingService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerProcessingService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task WriteCustomerDataToDb(List<Customer> customers)
        {
            foreach (var customer in customers)
            {
                await _customerRepository.AddCustomerAsync(customer);
            }
        }
    }
}
