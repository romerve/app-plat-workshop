using Newtonsoft.Json;
using System;

namespace app_plat_workshop
{
    public class HealthSubmission
    {
        [JsonProperty("id")]
        public string Id { get; private set; }
        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("submittedOn")]
        public DateTimeOffset SubmittedOn { get; set; }

        public HealthSubmission()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}