using GemicleTestTaskApi.Services.Interfaces;
using GemicleTestTaskDataReader.CsvReader;
using GemicleTestTaskModels.ApiRequestModels;
using GemicleTestTaskModels.DBModels;
using Microsoft.AspNetCore.Mvc;

namespace GemicleTestTask.Controllers
{
    [ApiController]
    [Route(nameof(TemplateSenderController))]
    public class TemplateSenderController : ControllerBase
    {
        private readonly ILogger<TemplateSenderController> _logger;
        private readonly ICampaignSchedulerService _campaignSchedulerService;

        public TemplateSenderController(
            ICampaignSchedulerService campaignSchedulerService,
            ILogger<TemplateSenderController> logger)
        {
            _campaignSchedulerService = campaignSchedulerService;
            _logger = logger;
        }

        [HttpPost(nameof(AddCampaign))]
        public async Task<IActionResult> AddCampaign(CampaignApiModel campaign)
        {
            await _campaignSchedulerService.ScheduleCampaignAsync(campaign);

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<Campaign>>> GetCampaigns()
        {
            var templates = await _campaignSchedulerService.GetAllCampaigns();

            return Ok(templates);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveCampaign(Guid campaignId)
        {
            await _campaignSchedulerService.RemoveCampaign(campaignId);

            return Ok();
        }

        [HttpPost(nameof(ForceSendAllTodayTemplates))]
        public async Task<IActionResult> ForceSendAllTodayTemplates()
        {
            await _campaignSchedulerService.ForceSendTemplates();

            return Ok();
        }
    }
}
