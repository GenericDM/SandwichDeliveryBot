using Newtonsoft.Json;

namespace SandwichDeliveryBot3.CustomClasses
{
    public class Transaction
    {
        [JsonProperty("user")]
        public ulong UserId { get; set; }
        [JsonProperty("amount")]
        public float Amount { get; set; }
        [JsonProperty("for")]
        public string To { get; set; }
    }
}
