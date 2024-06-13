using GemicleTestTaskModels.DBModels;

namespace GemicleTestTaskApi.Services.Interfaces
{
    public interface ICustomerProcessingService
    {
        public Task WriteCustomerDataToDb(List<Customer> customers);
    }
}
