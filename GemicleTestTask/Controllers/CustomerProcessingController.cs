using GemicleTestTask.Controllers;
using GemicleTestTaskApi.Services.Interfaces;
using GemicleTestTaskDataReader.CsvReader;
using Microsoft.AspNetCore.Mvc;

namespace GemicleTestTaskApi.Controllers
{
    [ApiController]
    [Route(nameof(CustomerProcessingController))]
    public class CustomerProcessingController : Controller
    {
        private readonly CsvReader _reader = new CsvReader("E:/Тестові/customers.csv");
        private readonly ICustomerProcessingService _customerProcessingService;

        public CustomerProcessingController(ICustomerProcessingService customerProcessingService)
        {
            _customerProcessingService = customerProcessingService;
        }

        [HttpPost]
        public async Task<IActionResult> WriteCustomerDataToDB()
        {
            var customers = _reader.GetUsers();
            await _customerProcessingService.WriteCustomerDataToDb(customers);

            return Ok();
        }
    }
}
