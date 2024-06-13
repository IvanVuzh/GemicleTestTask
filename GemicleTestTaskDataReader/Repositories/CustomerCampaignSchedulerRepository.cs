using GemicleTestTaskData.Repositories.Interfaces;
using GemicleTestTaskModels.DBModels;
using Microsoft.EntityFrameworkCore;

namespace GemicleTestTaskData.Repositories
{
    public class CustomerCampaignSchedulerRepository : ICustomerCampaignSchedulerRepository
    {
        private readonly CampaignDbContext _dbContext;

        public CustomerCampaignSchedulerRepository(CampaignDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> HasCustomerReceivedCampaignTodayAsync(int customerId, DateTime today)
        {
            return await _dbContext.CustomerCampaignSchedule
                            .AnyAsync(log => log.CustomerId == customerId && log.ToSendDate.Date == today.Date);
        }

        public async Task<List<CustomerCampaignSchedule>> AllAffectedTodayCustomerIdsAsync(DateTime today)
        {
            return await _dbContext.CustomerCampaignSchedule
                            .Where(log => log.ToSendDate.Date == today.Date && log.AlreadySent)
                            .Distinct()
                            .ToListAsync();
        }

        public async Task<List<CustomerCampaignSchedule>> AllPlannedForTodayCustomerIdsAsync(DateTime today)
        {
            return await _dbContext.CustomerCampaignSchedule
                           .Where(log => log.ToSendDate.Date == today.Date && log.AlreadySent)
                           .Distinct()
                           .ToListAsync();
        }

        public async Task ScheduleCampaignToSendAsync(int customerId, Guid campaignId, DateTime sentDate)
        {
            var logEntry = new CustomerCampaignSchedule
            {
                CustomerId = customerId,
                CampaignId = campaignId,
                ToSendDate = sentDate
            };

            _dbContext.CustomerCampaignSchedule.Add(logEntry);

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteSchedulesAsync(IEnumerable<CustomerCampaignSchedule> schedules)
        {
            _dbContext.CustomerCampaignSchedule.RemoveRange(schedules);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<CustomerCampaignSchedule>> GetAllTemplatesToBeSentAsync()
        {
            return await _dbContext.CustomerCampaignSchedule.Include(x => x.Customer).Where(ccs => ccs.ToSendDate > DateTime.UtcNow).ToListAsync();
        }  
    }
}
