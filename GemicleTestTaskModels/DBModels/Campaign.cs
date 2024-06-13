namespace GemicleTestTaskModels.DBModels
{
    public class Campaign
    {
        public Guid Id { get; set; }
        public string TemplateName { get; set; }
        public string Condition { get; set; }
        public DateTime ScheduledTime { get; set; }
        public int Priority { get; set; }


        public ICollection<CustomerCampaignSchedule> CustomerLogs { get; set; }
    }
}
