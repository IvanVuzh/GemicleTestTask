using GemicleTestTaskApi.Services.Interfaces;
using GemicleTestTaskData.Repositories.Interfaces;
using GemicleTestTaskModels.ApiRequestModels;
using GemicleTestTaskModels.DBModels;
using System;
using System.IO;

namespace GemicleTestTaskApi.Services
{
    public class CampaignSchedulerService: ICampaignSchedulerService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerCampaignSchedulerRepository _customerCampaignSchedulerRepository;

        public CampaignSchedulerService(
            ICampaignRepository campaignRepository, 
            ICustomerRepository customerRepository,
            ICustomerCampaignSchedulerRepository customerCampaignLogRepository)
        {
            _campaignRepository = campaignRepository;
            _customerRepository = customerRepository;
            _customerCampaignSchedulerRepository = customerCampaignLogRepository;
        }

        public async Task<List<Campaign>> GetAllCampaigns()
        {
            var campaigns = await _campaignRepository.GetAllCampaignsAsync();

            return campaigns;
        }

        public async Task RemoveCampaign(Guid id)
        {
            await _campaignRepository.DeleteCampaignAsync(id);
        }

        public async Task ScheduleCampaignAsync(CampaignApiModel campaignData)
        {
            var possiblyAffectedCustomers = await _customerRepository.GetAllCustomersWithConditionAsync(campaignData.Condition);

            if (possiblyAffectedCustomers is null)
            {
                return;
            }
            
            var campaign = new Campaign()
            {
                Id = Guid.NewGuid(),
                Condition = campaignData.Condition,
                Priority = campaignData.Priority,
                ScheduledTime = campaignData.ScheduledTime,
                TemplateName = campaignData.TemplateName,
            };

            await _campaignRepository.AddCampaignAsync(campaign);

            var previouslyPlannedCampaignsForToday = await _campaignRepository.GetAllCampaignsAfterThisMomentAsync();

            var alreadySentTo = await _customerCampaignSchedulerRepository.AllAffectedTodayCustomerIdsAsync(DateTime.UtcNow);

            var plannedToSendTo = await _customerCampaignSchedulerRepository.AllPlannedForTodayCustomerIdsAsync(DateTime.UtcNow);

            var shouldSendToCustomers = possiblyAffectedCustomers.Where(c => !alreadySentTo.Any(ast => ast.CustomerId == c.Id));

            var shouldRescheduleForCustomers = plannedToSendTo
                .Where(plannedCampaign => previouslyPlannedCampaignsForToday.Any(cam => cam.Priority > campaign.Priority) 
                                                && shouldSendToCustomers.Select(customer => customer.Id).Contains(plannedCampaign.CustomerId));

            // remove all previously planned to send if current campaign has lower Priority (lower Priority number means higher Priority)
            shouldSendToCustomers.Where(c => !plannedToSendTo.Any(ast => ast.CustomerId == c.Id) || shouldRescheduleForCustomers.Any(sch => sch.CustomerId == c.Id));

            await _customerCampaignSchedulerRepository.DeleteSchedulesAsync(shouldRescheduleForCustomers);

            foreach (var customer in shouldSendToCustomers)
            {
                await _customerCampaignSchedulerRepository.ScheduleCampaignToSendAsync(customer.Id, campaign.Id, campaignData.ScheduledTime);
            }
        }

        public async Task ForceSendTemplates()
        {
            var campaigns = await _campaignRepository.GetAllCampaignsAfterThisMomentAsync();

            var teplatesToBeSent = await _customerCampaignSchedulerRepository.GetAllTemplatesToBeSentAsync();

            var campaignTasks = new List<Task>();

            foreach (var campaign in campaigns)
            {
                var task = Task.Run(async () =>
                {
                    await ExecuteCampaignAsync(campaign, teplatesToBeSent.Where(t => t.CampaignId == campaign.Id));
                });

                campaignTasks.Add(task);
            }

            await Task.WhenAll(campaignTasks);
        }

        private async Task ExecuteCampaignAsync(Campaign campaign, IEnumerable<CustomerCampaignSchedule> templatesToSend)
        {
            try
            {
                // i was unable to write to same file simultaneously, so i made different files and used timestamps
                using (StreamWriter writer = new StreamWriter($"sends_{campaign.Id}.txt", append: true))
                {
                    foreach (var template in templatesToSend)
                    {
                        await writer.WriteLineAsync($"{DateTime.UtcNow} - Sent campaign {campaign.Id}, template {campaign.TemplateName} to {template.Customer.Id} scheduled for {template.ToSendDate}");
                        Console.WriteLine($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} - Sent campaign {campaign.Id}, template {campaign.TemplateName} to {template.Customer.Id} scheduled for {template.ToSendDate}");
                    }
                }

                Console.WriteLine($"{DateTime.Now}: Campaign {campaign.Id} executed successfully.");

                await Task.Delay(TimeSpan.FromMinutes(.5));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now}: Campaign {campaign.Id} encountered an error: {ex.Message}");
                throw;
            }
        }
    }
}
