using GemicleTestTaskModels.DBModels;

namespace GemicleTestTaskData.Repositories.Interfaces
{
    public interface ICampaignRepository
    {
        public Task<List<Campaign>> GetAllCampaignsAsync();

        public Task<List<Campaign>> GetAllCampaignsAfterThisMomentAsync();

        public Task<Campaign> GetCampaignByIdAsync(int id);

        public Task AddCampaignAsync(Campaign campaign);

        public Task UpdateCampaignAsync(Campaign campaign);

        public Task DeleteCampaignAsync(Guid id);
    }
}
