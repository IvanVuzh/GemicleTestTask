using GemicleTestTaskModels.DBModels;

namespace GemicleTestTaskData.Repositories.Interfaces
{
    public interface ICustomerCampaignSchedulerRepository
    {
        public Task<bool> HasCustomerReceivedCampaignTodayAsync(int customerId, DateTime today);

        public Task<List<CustomerCampaignSchedule>> AllAffectedTodayCustomerIdsAsync(DateTime today);

        public Task<List<CustomerCampaignSchedule>> AllPlannedForTodayCustomerIdsAsync(DateTime today);

        public Task ScheduleCampaignToSendAsync(int customerId, Guid campaignId, DateTime sendDate);
        
        public Task DeleteSchedulesAsync(IEnumerable<CustomerCampaignSchedule> schedules);

        public Task<List<CustomerCampaignSchedule>> GetAllTemplatesToBeSentAsync();
    }
}
