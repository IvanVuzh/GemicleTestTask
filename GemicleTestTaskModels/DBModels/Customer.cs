using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemicleTestTaskModels.DBModels
{
    public class Customer
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; } = null!;
        public string City { get; set; } = null!;
        public int Deposit { get; set; }
        public bool NewCustomer { get; set; }

        public ICollection<CustomerCampaignSchedule> CustomerLogs { get; set; }
    }
}
