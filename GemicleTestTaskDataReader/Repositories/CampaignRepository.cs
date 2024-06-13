using GemicleTestTaskData.Repositories.Interfaces;
using GemicleTestTaskModels.DBModels;
using Microsoft.EntityFrameworkCore;

namespace GemicleTestTaskData.Repositories
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly CampaignDbContext _dbContext;

        public CampaignRepository(CampaignDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Campaign>> GetAllCampaignsAsync()
        {
            return await _dbContext.Campaigns.ToListAsync();
        }

        public async Task<List<Campaign>> GetAllCampaignsAfterThisMomentAsync()
        {
            return await _dbContext.Campaigns.Where(c => c.ScheduledTime > DateTime.UtcNow).ToListAsync();
        }

        public async Task<Campaign> GetCampaignByIdAsync(int id)
        {
            return await _dbContext.Campaigns.FindAsync(id);
        }

        public async Task AddCampaignAsync(Campaign campaign)
        {
            _dbContext.Campaigns.Add(campaign);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCampaignAsync(Campaign campaign)
        {
            _dbContext.Entry(campaign).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteCampaignAsync(Guid id)
        {
            var campaign = await _dbContext.Campaigns.FindAsync(id);
            if (campaign != null)
            {
                _dbContext.Campaigns.Remove(campaign);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
