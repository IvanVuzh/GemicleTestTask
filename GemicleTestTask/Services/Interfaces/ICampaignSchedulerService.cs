using GemicleTestTaskModels.ApiRequestModels;
using GemicleTestTaskModels.DBModels;

namespace GemicleTestTaskApi.Services.Interfaces
{
    public interface ICampaignSchedulerService
    {
        public Task<List<Campaign>> GetAllCampaigns();

        public Task RemoveCampaign(Guid id);

        public Task ScheduleCampaignAsync(CampaignApiModel campaign);

        public Task ForceSendTemplates();
    }
}
