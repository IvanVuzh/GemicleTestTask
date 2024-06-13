using System.ComponentModel.DataAnnotations.Schema;

namespace GemicleTestTaskModels.DBModels
{
    public class CustomerCampaignSchedule
    {
        public Guid Id { get; set; }
        public int CustomerId { get; set; }
        public Guid CampaignId { get; set; }
        public DateTime ToSendDate { get; set; }
        public bool AlreadySent { get; set; } = false;


        public Campaign Campaign { get; set; }
        public Customer Customer { get; set; }
    }
}
