using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemicleTestTaskModels.ApiRequestModels
{
    public class CampaignApiModel
    {
        public string TemplateName { get; set; }
        public string Condition { get; set; }
        public DateTime ScheduledTime { get; set; }
        public int Priority { get; set; }
    }
}
