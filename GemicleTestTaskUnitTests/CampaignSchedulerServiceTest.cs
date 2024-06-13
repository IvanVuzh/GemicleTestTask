using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using GemicleTestTaskData.Repositories.Interfaces;
using GemicleTestTaskModels.ApiRequestModels;
using GemicleTestTaskModels.DBModels;
using NSubstitute;
using GemicleTestTaskData.Repositories;
using GemicleTestTaskApi.Services;

namespace GemicleTestTaskUnitTests
{
    public class CampaignSchedulerServiceTest
    {
        private readonly CampaignSchedulerService _service;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerCampaignSchedulerRepository _customerCampaignSchedulerRepository;

        public CampaignSchedulerServiceTest()
        {
            var campaignId1 = Guid.NewGuid();
            var campaignId2 = Guid.NewGuid();
            var campaignId3 = Guid.NewGuid();
            _campaignRepository = Substitute.For<ICampaignRepository>();
            _campaignRepository.GetAllCampaignsAsync()
                .Returns(new List<Campaign>()
                {
                    new Campaign
                    {
                        Id = campaignId1,
                        TemplateName = "Test 1",
                        Condition = "Age > 10",
                        ScheduledTime = DateTime.UtcNow,
                        Priority = 1,
                    },
                    new Campaign
                    {
                        Id = campaignId2,
                        TemplateName = "Test 2",
                        Condition = "City == \"New York\"",
                        ScheduledTime = DateTime.UtcNow,
                        Priority = 2,
                    },
                    new Campaign
                    {
                        Id = Guid.NewGuid(),
                        TemplateName = "Test 1",
                        Condition = "Gende == \"Male\"",
                        ScheduledTime = DateTime.UtcNow,
                        Priority = 3,
                    }
                });
            _campaignRepository.GetAllCampaignsAfterThisMomentAsync()
                .Returns(new List<Campaign>()
                {
                    new Campaign
                    {
                        Id = Guid.NewGuid(),
                        TemplateName = "Test 1",
                        Condition = "Age > 10",
                        ScheduledTime = DateTime.UtcNow.AddHours(2),
                        Priority = 1,
                    },
                    new Campaign
                    {
                        Id = Guid.NewGuid(),
                        TemplateName = "Test 2",
                        Condition = "City == \"New York\"",
                        ScheduledTime = DateTime.UtcNow.AddMinutes(1),
                        Priority = 2,
                    },
                    new Campaign
                    {
                        Id = campaignId3,
                        TemplateName = "Test 1",
                        Condition = "Gende == \"Male\"",
                        ScheduledTime = DateTime.UtcNow.AddMinutes(40),
                        Priority = 3,
                    }
                });

            _customerRepository = Substitute.For<ICustomerRepository>();
            _customerCampaignSchedulerRepository = Substitute.For<ICustomerCampaignSchedulerRepository>();
            
            _service = new CampaignSchedulerService(_campaignRepository, _customerRepository, _customerCampaignSchedulerRepository);
        }

        [Fact]
        public async Task ScheduleCampaignAsync_WithCustomers_SchedulesCampaign()
        {
            // given
            var campaignData = new CampaignApiModel()
            {
                TemplateName = "Test_2",
                Condition = "Age > 20",
                ScheduledTime = DateTime.UtcNow,
                Priority = 2,
            };

            var customers = new List<Customer>
            {
                new Customer { Id = 1, Age = 20, Gender = "Male", City = "London", Deposit = 100, NewCustomer = false },
                new Customer { Id = 2, Age = 25, Gender = "Female", City = "New York", Deposit = 1000, NewCustomer = true },
                new Customer { Id = 3, Age = 25, Gender = "Female", City = "London", Deposit = 10, NewCustomer = false },
                new Customer { Id = 4, Age = 60, Gender = "Female", City = "Paris", Deposit = 80, NewCustomer = true },
            };

            _customerRepository.GetAllCustomersWithConditionAsync(campaignData.Condition).Returns(customers);
            _customerCampaignSchedulerRepository.AllAffectedTodayCustomerIdsAsync(Arg.Any<DateTime>())
                .Returns(new List<CustomerCampaignSchedule>()
                {
                    new CustomerCampaignSchedule()
                    {
                        AlreadySent = true,
                        CampaignId = Guid.NewGuid(),
                        CustomerId = 0,
                        Id = Guid.NewGuid(),
                        ToSendDate = DateTime.UtcNow,
                    }
                });
            _customerCampaignSchedulerRepository.AllPlannedForTodayCustomerIdsAsync(Arg.Any<DateTime>())
                .Returns(new List<CustomerCampaignSchedule>()
                {
                    new CustomerCampaignSchedule()
                    {
                        AlreadySent = false,
                        CampaignId = Guid.NewGuid(),
                        CustomerId = 100,
                        Id = Guid.NewGuid(),
                        ToSendDate = DateTime.UtcNow,
                    }
                });
            // when
            await _service.ScheduleCampaignAsync(campaignData);

            // then
            foreach (var customer in customers)
            {
                await _customerCampaignSchedulerRepository.Received(1)
                    .ScheduleCampaignToSendAsync(customer.Id, Arg.Any<Guid>(), campaignData.ScheduledTime);
            }
        }

        [Fact]
        public async Task ScheduleCampaignAsync_NullCustomers_ReturnsImmediately()
        {
            // given
            var campaignData = new CampaignApiModel()
            {
                TemplateName = "Test",
                Condition = "Age > 30",
                ScheduledTime = DateTime.UtcNow,
                Priority = 1,
            };

            _customerRepository.GetAllCustomersWithConditionAsync(campaignData.Condition)
                .Returns(Task.FromResult<List<Customer>?>(null));

            // when
            await _service.ScheduleCampaignAsync(campaignData);

            // then
            await _customerCampaignSchedulerRepository.DidNotReceive()
                .ScheduleCampaignToSendAsync(Arg.Any<int>(), Arg.Any<Guid>(), Arg.Any<DateTime>());
        }

        [Fact]
        // tested with Task.Delay(TimeSpan.FromMinutes(.5));
        public async Task ForceSendTemplates_SuccessfulExecution()
        {
            // Arrange
            var campaigns = new List<Campaign>
            {
                new Campaign { Id = Guid.NewGuid(), TemplateName = "Test1", ScheduledTime = DateTime.UtcNow },
                new Campaign { Id = Guid.NewGuid(), TemplateName = "Test2", ScheduledTime = DateTime.UtcNow }
            };

            var templatesToBeSent = new List<CustomerCampaignSchedule>
            {
                new CustomerCampaignSchedule { CampaignId = campaigns[0].Id, Customer = new Customer { Id = 1 }, ToSendDate = DateTime.UtcNow },
                new CustomerCampaignSchedule { CampaignId = campaigns[1].Id, Customer = new Customer { Id = 2 }, ToSendDate = DateTime.UtcNow }
            };

            _campaignRepository.GetAllCampaignsAfterThisMomentAsync().Returns(campaigns);
            _customerCampaignSchedulerRepository.GetAllTemplatesToBeSentAsync().Returns(templatesToBeSent);

            // when
            await _service.ForceSendTemplates();

            // then
            foreach (var campaign in campaigns)
            {
                var filePath = $"sends_{campaign.Id}.txt";
                File.Exists(filePath).ShouldBeTrue();

                var lines = await File.ReadAllLinesAsync(filePath);
                lines.Length.ShouldBe(1);

                // we cant compare them fully because of timestamps
                var expectedSubString = $" - Sent campaign {campaign.Id}, template {campaign.TemplateName} to {templatesToBeSent.First(t => t.CampaignId == campaign.Id).Customer.Id} scheduled for {templatesToBeSent.First(t => t.CampaignId == campaign.Id).ToSendDate}";
                lines[0].ShouldContain(expectedSubString);
            }
        }
    }
}